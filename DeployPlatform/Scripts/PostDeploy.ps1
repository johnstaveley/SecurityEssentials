<#
.SYNOPSIS
    Setups up IP address restrictions so site can only be accessed via cloudflare
	Wakes up the site after deployment
.NOTES
    Author: John Staveley
    Date:   16/03/2020    
#>
Param(
    [string] $AzureLocation,
	[Parameter(Mandatory=$true)]
	[string] $EnvironmentName,
	[Parameter(Mandatory=$true)]
    [string] $SiteBaseUrl,
	[Parameter(Mandatory=$true)]
    [string] $SiteName,
	[Parameter(Mandatory=$true)]
	[string] $ArmTemplateOutput,
	[string] $CloudFlareIpAddresses,
	[string] $DeveloperIpAddresses
)

[string] $vnetStorageApiKey = ''
[string] $nonVnetStorageApiKey = ''
[string] $publishProfilePassword = ''

# Parse ARM Template Output
if ($ArmTemplateOutput -ne $null -and $ArmTemplateOutput.Length -gt 3) {
	$armTemplateJson = $ArmTemplateOutput | ConvertFrom-Json
	$vnetStorageApiKey = $armTemplateJson.vNetStorageConnectionString.value
	$nonVnetStorageApiKey = $armTemplateJson.nonVNetStorageConnectionString.value
	$publishProfilePassword = $armTemplateJson.publishProfilePassword.value
}

[string] $resourceGroupName = $SiteName + '-' + $EnvironmentName
[string] $siteNameLowerCase = $SiteName.ToLower()
[string] $webSiteName = $siteNameLowerCase + $EnvironmentName.ToLower()
[string] $vNetStorageAccountName = $siteNameLowerCase + $EnvironmentName.ToLower() + "vnt"
[string] $websiteUrl = "https://" + $siteNameLowerCase + $EnvironmentName.ToLower() + "." + $SiteBaseUrl
[int] $softDeletePolicyDays = 7

Write-Host ("Cloudflare IP Address restrictions. $CloudFlareIpAddresses rules to process")

function AddRules($rulesToAdd) {
	$rules = @()
	foreach ($ruleToAdd in $rulesToAdd) 
	{
		$rule = [PSCustomObject] @{
			ipAddress = $ruleToAdd.ipAddress; 
			action = $ruleToAdd.action; 
			priority = $ruleToAdd.priority; 
			name = $ruleToAdd.name; 
			description = $ruleToAdd.description 
			}
		$rules += $rule
	}
return $rules
}

# Access to the main site should only be allowed through cloudflare
$cloudFlareIpAddressCount = $CloudFlareIpAddresses.Length
Write-Host ("Cloudflare firewall enabled - Starting IP Address restrictions. $cloudFlareIpAddressCount cloudflare rules to process")
[PSCustomObject] $websiteRulesToAdd = New-Object System.Collections.ArrayList
[int] $cloudFlareRuleId = 1
foreach ($cloudFlareIpAddress in $cloudFlareIpAddresses.Split(",")) {
    $newRule = @{ipAddress=$cloudFlareIpAddress;action="Allow";priority="100";name="CF" + $cloudFlareRuleId.ToString().PadLeft(2, "0");description="CloudFlare IP Address"}
	$websiteRulesToAdd.Add($newRule) | Out-Null
	$cloudFlareRuleId += 1
}

# Access to the deployment (scm) site should be locked down to developers (NB: These rules are temporarily disabled when deploying to allow azure devops access)
[PSCustomObject] $scmRulesToAdd = New-Object System.Collections.ArrayList
[int] $devRuleId = 1
if ($DeveloperIpAddresses -ne '') {
	foreach($developerIpAddress in $DeveloperIpAddresses.Split(",")) {
		$scmRulesToAdd.Add(@{ipAddress=$developerIpAddress;action="Allow";priority="100";name="DEV" + $devRuleId.ToString().PadLeft(2, "0");description="Developer IP Address"}) | Out-Null
	}
}
$apiVersion = ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq sites).apiVersions[0]
$webAppConfig = (Get-AzureRmResource -ResourceType Microsoft.Web/sites/config -ResourceName $webSiteName -ResourceGroupName $resourceGroupName -apiVersion $apiVersion)
Write-Host ("Writing IP Address restrictions")
$webAppConfig.Properties.ipSecurityRestrictions = AddRules -rulesToAdd $websiteRulesToAdd
$webAppConfig.Properties.scmIpSecurityRestrictions = AddRules -rulesToAdd $scmRulesToAdd
Set-AzureRmResource -ResourceId $webAppConfig.ResourceId -Properties $webAppConfig.Properties -apiVersion $apiVersion -Force
Write-Host ("Completed IP Address restrictions")


Write-Host ("Enabling access restrictions for storage $vNetStorageAccountName")
Update-AzureRmStorageAccountNetworkRuleSet -ResourceGroupName $resourceGroupName -Name $vNetStorageAccountName -DefaultAction Deny
Write-Host ("Updated access restrictions for storage $vNetStorageAccountName")

# Wait for website to restart
$secondsToWait = 60
Write-Host ("Waiting $secondsToWait seconds for site to restart")
Start-Sleep($secondsToWait)

# Wake websites up
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$ProgressPreference = 'SilentlyContinue'
Write-Host ("Hitting url " + $websiteUrl)
$webRequest = [System.Net.WebRequest]::Create($websiteUrl)
$webRequest.Timeout = 5 * 60 * 1000
$output = $webRequest.GetResponse()
$ProgressPreference = 'Continue'
Write-Host ("Warmup finished")

Write-Host ("Post Deploy complete")
