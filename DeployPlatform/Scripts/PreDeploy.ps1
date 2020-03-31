<#
.SYNOPSIS
    Carries out pre deployment steps for deployment of site Security Essentials
.DESCRIPTION
    Carries out the following functions:
    Creates the resource group, blob storage and key vault in Azure
    Sets up secure keys in the key vault
.PARAMETER Path
    The path to the .
.PARAMETER LiteralPath
    Specifies a path to one or more locations. Unlike Path, the value of 
    LiteralPath is used exactly as it is typed. No characters are interpreted 
    as wildcards. If the path includes escape characters, enclose it in single
    quotation marks. Single quotation marks tell Windows PowerShell not to 
    interpret any characters as escape sequences.
.EXAMPLE
    C:\PS> 
    <Description of example>
.NOTES
    Author: John Staveley
    Date:   24/06/2019    
#>
Param(
	[Parameter(Mandatory=$true)]
	[string] $AzureLocation,
	[Parameter(Mandatory=$true)]
	[string] $EnvironmentName,
	[Parameter(Mandatory=$true)]
	[string] $SiteName,
    [Parameter(Mandatory=$true)]
    [string] $SiteBaseUrl,
	[Parameter(Mandatory=$true)]
	[string] $SubscriptionId,
    [Parameter(Mandatory=$true)]
    [string] $CloudFlareAuthEmail,
    [Parameter(Mandatory=$true)]
    [string] $CloudFlareUserServiceKey,
	[Parameter(Mandatory=$true)]
    [string] $CloudFlareZoneName,
    [Parameter(Mandatory=$true)]
    [string] $SqlAdminPassword,
    [string] $CloudFlarePlan = 'Free'
)

if ((Get-AzureRmContext) -eq $null -or [string]::IsNullOrEmpty($(Get-AzureRmContext).Account)) { 
    Write-Host "Logging in...";
    Login-AzureRmAccount 
}

Write-Host("Setup variables")
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3
[string] $resourceGroupName = $SiteName + '-' + $EnvironmentName
[string] $siteNameLowerCase = $SiteName.ToLower()
[string] $webSiteName = $siteNameLowerCase + $EnvironmentName.ToLower()
[string] $vaultName = $siteNameLowerCase + $EnvironmentName.ToLower()
$cloudFlareBaseUrl = 'https://api.cloudflare.com/client/v4/'
$cloudFlareHeaders = @{"Content-Type"= "application/json; charset=utf-8"; 'X-Auth-Key' = $CloudFlareUserServiceKey; 'X-Auth-Email' = $CloudFlareAuthEmail}
[string] $keyVaultDiagnosticsName = $siteNameLowerCase + $EnvironmentName.ToLower() + 'diagnostics'

# Select subscription
Write-Host "Selecting subscription '$SubscriptionId'"
Select-AzureRmSubscription -SubscriptionID $SubscriptionId -ErrorAction Stop
Set-AzureRmContext -SubscriptionId $SubscriptionId -ErrorAction Stop

# Create or check for existing resource group
$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction SilentlyContinue
if(!$resourceGroup)
{
    Write-Host "Creating resource group '$resourceGroupName' in location '$AzureLocation'";
    New-AzureRmResourceGroup -Name $resourceGroupName -Location $AzureLocation
    Start-Sleep(5) # Give it some time to create the resource group
}
else{
    Write-Host "Using existing resource group '$resourceGroupName'";
}
# Create the storage accounts if they don't already exist
$vNetStorageAccountName = $siteNameLowercase + $EnvironmentName.tolower() + 'vnt'
$vNetStorageAccount = (Get-AzureRmStorageAccount | Where-Object {$_.StorageAccountName -eq $vNetStorageAccountName})
if ($vNetStorageAccount -eq $null) {
	Write-Host "Creating Storage Account '$vNetStorageAccountName' in $resourceGroupName" 
    $vNetStorageAccount = New-AzureRmStorageAccount -StorageAccountName $vNetStorageAccountName -Type 'Standard_GRS' -ResourceGroupName $resourceGroupName -Location $AzureLocation -EnableHttpsTrafficOnly $True
} else {
	Write-Host "Storage Account '$vNetStorageAccountName' in $resourceGroupName already exists" 
}

$storageAccountName = $siteNameLowercase + $EnvironmentName.tolower()
$storageAccount = (Get-AzureRmStorageAccount | Where-Object {$_.StorageAccountName -eq $storageAccountName})
if ($storageAccount -eq $null) {
	Write-Host "Creating Storage Account '$storageAccountName' in $resourceGroupName" 
    $storageAccount = New-AzureRmStorageAccount -StorageAccountName $storageAccountName -Type 'Standard_LRS' -ResourceGroupName $resourceGroupName -Location $AzureLocation -EnableHttpsTrafficOnly $True
} else {
	Write-Host "Storage Account '$storageAccountName' in $resourceGroupName already exists" 
}

ipconfig /flushdns
Sleep 2

# create key vault
Write-Host "Checking key Vault" 
$matchingVaults = (Get-AzureRMKeyVault | Where-Object { $_.ResourceGroupName -eq $resourceGroupName -and $_.vaultName -eq $vaultName })
if ($matchingVaults -eq $null) { 
    # create vault and enable the key vault for template deployment
    Write-Host "Creating Vault '$vaultName' in $resourceGroupName" 
    $vault = New-AzureRMKeyVault -vaultName $vaultName -ResourceGroupName $resourceGroupName -location $AzureLocation -enabledfortemplatedeployment
} else {
    Write-Host "Vault '$vaultName' in $resourceGroupName already exists" 
    $vault = $matchingVaults[0]
}

# Enable logging for key vault
# TODO: Change AuditEvent to AuditEvent,AllMetrics when the powershell command supports it. In the meantime, set this manually
Set-AzureRmDiagnosticSetting -ResourceId $vault.ResourceId -StorageAccountId $storageAccount.Id -Enabled $true -Categories AuditEvent -RetentionEnabled $true -RetentionInDays 365 `
    -Name $keyVaultDiagnosticsName

# create secure database key if not already present
$secretKey = 'SqlAzurePassword'
Write-Host "Checking key Vault $vaultName for secret $secretKey" 
$matchingSecrets = Get-AzureKeyVaultSecret -VaultName $vaultName -ErrorAction Stop | Where-Object { $_.Name -eq $secretKey }
if ($matchingSecrets -eq $null) { 
	Write-Host "Settings '$secretKey' in vault $vaultName" 
    $secret = ConvertTo-SecureString -string $SqlAdminPassword -asplaintext -force
	Write-Host "."
    Set-AzureKeyVaultSecret -VaultName $vaultName -name $secretKey -SecretValue $secret -ErrorAction Stop
    Sleep 5
} else {
    Write-Host "Secret key '$secretKey' already exists in vault $vaultName" 
}

# https://api.cloudflare.com/#zone-list-zones
Write-Host ("Get CloudFlare Zone")
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$cloudFlareUrl = $cloudFlareBaseUrl + 'zones?name=' + $CloudFlareZoneName
$result = Invoke-RestMethod -Method Get -Uri $cloudFlareUrl -Headers $cloudFlareHeaders
$zoneId = $result.result.id
Write-Host ("CloudFlare Zone " + $zoneId)

# https://api.cloudflare.com/#cloudflare-ips-properties
Write-Host ("Get Cloudflare Ips")
$cloudflareUrl = $cloudflareBaseUrl + 'ips'
$result = Invoke-RestMethod -Method Get -Uri $cloudflareUrl -Headers $cloudflareHeaders
$cloudFlareIpv4 = $result.result.ipv4_cidrs -join ","
Write-Host "##vso[task.setvariable variable=CloudFlareIPv4;]$cloudFlareIpv4"
$cloudFlareIpv6 = $result.result.ipv6_cidrs -join ","
Write-Host "##vso[task.setvariable variable=CloudFlareIPv6;]$cloudFlareIpv6"


function Set-CloudFlareSetting {
	Param ([string] $cloudFlareBaseUrl, [string] $zoneId, [string] $settingName, [string] $settingDesiredState, $cloudFlareHeaders)
    # https://api.cloudflare.com/#zone-settings-properties
    Write-Host ("Get Cloudflare Settings for zone")
    $cloudFlareUrl = $cloudFlareBaseUrl + 'zones/' + $zoneId + '/settings'
    $result = Invoke-RestMethod -Method Get -Uri $cloudFlareUrl -Headers $cloudFlareHeaders
    $zoneProperties = $result.result
    $settingValue = $zoneProperties | Where-Object { $_.id -eq $settingName }
    if ($settingValue.value -eq $settingDesiredState) {
        Write-Host("Cloudflare: $settingName already set to $settingDesiredState")
    } else {
        Write-Host("Cloudflare: Setting $settingName")
        $settingsUrl = $cloudFlareUrl + "/" + $settingName
        $result = Invoke-RestMethod -Method Patch -Uri $settingsUrl -Headers $cloudflareHeaders -Body ('{"value":"' + $settingDesiredState + '"}')
        if ($result.success -ne $true) {    
            Write-Host("Updating $settingName failed with result " + $result.result)
            Write-Host($result.messages)
            Write-Host($result.errors)
        }
    }
}

Set-CloudFlareSetting $cloudflareBaseUrl $zoneId "always_use_https" "on" $cloudFlareHeaders
Set-CloudFlareSetting $cloudflareBaseUrl $zoneId "min_tls_version" "1.2" $cloudFlareHeaders
Set-CloudFlareSetting $cloudflareBaseUrl $zoneId "tls_1_3" "on" $cloudFlareHeaders
Set-CloudFlareSetting $cloudflareBaseUrl $zoneId "ssl" "full" $cloudFlareHeaders
Set-CloudFlareSetting $cloudflareBaseUrl $zoneId "http2" "on" $cloudFlareHeaders
if ($CloudFlarePlan -ne "Free") {
    Set-CloudFlareSetting $cloudflareBaseUrl $zoneId "waf" "on" $cloudFlareHeaders
}

# https://api.cloudflare.com/#dns-records-for-a-zone-properties
Write-Host ("Gettting existing DNS Records for site:")
$externalWebsiteUrl = $SiteName.ToLower() + $EnvironmentName.ToLower() +"." + $SiteBaseUrl
# Create entries for Azure App Service (Proxied)
$cloudFlareUrl = $cloudFlareBaseUrl + 'zones/' + $zoneId + '/dns_records?type=CNAME&per_page=100&match=all&name=' + $externalWebsiteUrl
$result = Invoke-RestMethod -Method Get -Uri $cloudFlareUrl -Headers $cloudFlareHeaders
$cloudFlareDnsEntries = $result.result
$azureWebsiteUrl = $SiteName.ToLower() + $EnvironmentName.ToLower() + ".azurewebsites.net"

$cloudFlareDnsEntry = $cloudFlareDnsEntries | Where-Object { $_.name -eq $externalWebsiteUrl }
if ($cloudFlareDnsEntry -eq $null) {
    Write-Host ("Url '" + $externalWebsiteUrl + "' not present in DNS, creating, linking to " + $azureWebsiteUrl)
    $cloudFlareUrl = $cloudFlareBaseUrl + 'zones/' + $zoneId + '/dns_records'
    $dnsEntry = '{"type":"CNAME","name":"' + $externalWebsiteUrl + '","content":"' + $azureCustomerUrl + '","ttl":1,"proxied":true}'
    Invoke-RestMethod -Method Post -Uri $cloudFlareUrl -Body $dnsEntry -Headers $cloudFlareHeaders
} else {
    Write-Host ("Url '" + $externalWebsiteUrl + "' present in DNS")
    # NB: Proxied means it is behind the WAF
    if ($cloudFlareDnsEntry.proxied -eq $true) {
        Write-Host ("Url '" + $externalWebsiteUrl + "' present in Proxy")
    } else {
        Write-Host ("Url '" + $externalWebsiteUrl + "' not Proxied, switching Proxy on")
        $cloudFlareUrl = $cloudFlareBaseUrl + 'zones/' + $zoneId + '/dns_records/' + $cloudFlareDnsEntry.id
        $dnsEntry = '{"type":"CNAME","name":"' + $externalWebsiteUrl + '","content":"' + $azureCustomerUrl + '","ttl":1,"proxied":true}'
        $result = Invoke-RestMethod -Method PUT -Uri $cloudFlareUrl -Body $dnsEntry -Headers $cloudFlareHeaders
    }
}

# Create AWVerify Entries (Not proxied) for manually registering TLS certificates, if required
$externalWebsiteVerifyUrl = 'awverify.' + $externalWebsiteUrl 
$azureWebsiteVerifyUrl = 'awverify.' + $SiteName.ToLower() + $EnvironmentName.ToLower() + ".azurewebsites.net"
$cloudFlareUrl = $cloudFlareBaseUrl + 'zones/' + $zoneId + '/dns_records?type=CNAME&per_page=100&name=' + $externalWebsiteVerifyUrl + '&match=all'
$result = Invoke-RestMethod -Method Get -Uri $cloudFlareUrl -Headers $cloudFlareHeaders
$cloudFlareDnsEntries = $result.result

$cloudFlareDnsEntry = $cloudFlareDnsEntries | Where-Object { $_.name -eq $externalWebsiteVerifyUrl }
if ($cloudFlareDnsEntry -eq $null) {
    Write-Host ("Url '" + $externalWebsiteVerifyUrl + "' not present in DNS, creating, linking to " + $azureWebsiteVerifyUrl)
    $cloudFlareUrl = $cloudFlareBaseUrl + 'zones/' + $zoneId + '/dns_records'
    $dnsEntry = '{"type":"CNAME","name":"' + $externalWebsiteVerifyUrl + '","content":"' + $azureWebsiteVerifyUrl + '","ttl":1,"proxied":false}'
    Invoke-RestMethod -Method Post -Uri $cloudFlareUrl -Body $dnsEntry -Headers $cloudFlareHeaders
} else {
    Write-Host ("Url '" + $externalWebsiteVerifyUrl + "' present in DNS")
}

Write-Host ("Removing IP Address restrictions from scm site")
# NB: You need to do this otherwise Azure Devops can't deploy the site
$apiVersion = ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq sites).apiVersions[0]
$webAppConfig = (Get-AzureRmResource -ResourceType Microsoft.Web/sites/config -ResourceName $webSiteName -ResourceGroupName $resourceGroupName -apiVersion $apiVersion -ErrorAction Continue)
if ($webAppConfig -ne $null) {
    $webAppConfig.Properties.scmIpSecurityRestrictions = @()
    Set-AzureRmResource -ResourceId $webAppConfig.ResourceId -Properties $webAppConfig.Properties -apiVersion $apiVersion -Force
}
Write-Host ("Removed IP Address restrictions from scm site")

Write-Host ("Pre-Deploy complete")
