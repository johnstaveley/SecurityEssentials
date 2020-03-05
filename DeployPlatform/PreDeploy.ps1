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
    [string] $CloudFlareZoneName
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

# https://api.cloudflare.com/#zone-list-zones
#Write-Host ("Get CloudFlare Zone")
#[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
#$cloudFlareUrl = $cloudFlareBaseUrl + 'zones?name=' + $CloudFlareZoneName
#$result = Invoke-RestMethod -Method Get -Uri $cloudFlareUrl -Headers $cloudFlareHeaders
#$zoneId = $result.result.id
#Write-Host ("CloudFlare Zone " + $zoneId)

# https://api.cloudflare.com/#cloudflare-ips-properties
#Write-Host ("Get Cloudflare Ips")
#$cloudflareUrl = $cloudflareBaseUrl + 'ips'
#$result = Invoke-RestMethod -Method Get -Uri $cloudflareUrl -Headers $cloudflareHeaders
#$cloudFlareIpv4 = $result.result.ipv4_cidrs -join ","
#Write-Host "##vso[task.setvariable variable=CloudFlareIPv4;]$cloudFlareIpv4"
#$cloudFlareIpv6 = $result.result.ipv6_cidrs -join ","
#Write-Host "##vso[task.setvariable variable=CloudFlareIPv6;]$cloudFlareIpv6"
    
# https://api.cloudFlare.com/#zone-settings-get-all-zone-settings
#Write-Host ("Get Cloudflare Settings for zone")
#$cloudFlareUrl = $cloudFlareBaseUrl + 'zones/' + $zoneId + '/settings'
#$result = Invoke-RestMethod -Method Get -Uri $cloudFlareUrl -Headers $cloudFlareHeaders
#$result.result

# https://api.cloudflare.com/#page-rules-for-a-zone-list-page-rules
#Write-Host ("Get Cloudflare Page Rules for zone")
#$cloudFlareUrl = $cloudFlareBaseUrl + 'zones/' + $zoneId + '/pagerules'
#$result = Invoke-RestMethod -Method Get -Uri $cloudFlareUrl -Headers $cloudFlareHeaders
#$result.result
# This should show that caching is disabled for the site

# https://api.cloudflare.com/#dns-records-for-a-zone-properties
#Write-Host ("Get DNS Records")
#$cloudflareUrl = $cloudflareBaseUrl + 'zones/' + $zoneId + '/dns_records?type=CNAME&per_page=100&name=staged&match=any'
#$result = Invoke-RestMethod -Method Get -Uri $cloudflareUrl -Headers $cloudflareHeaders
#$cloudFlareWafEntries = $result.result

# https://api.cloudFlare.com/#dns-records-for-a-zone-properties
#Write-Host ("DNS Records:")

# Create entries for Azure App Service (Proxied)
#$cloudFlareUrl = $cloudFlareBaseUrl + 'zones/' + $zoneId + '/dns_records?type=CNAME&per_page=100&name=' + $SiteBaseUrl + '&match=all'
#$result = Invoke-RestMethod -Method Get -Uri $cloudFlareUrl -Headers $cloudFlareHeaders
#$cloudFlareWafEntries = $result.result
#$azureCustomerUrl = $SiteName.ToLower() + $EnvironmentName.ToLower() + ".azurewebsites.net"

#$customerCloudFlareEntry = $cloudFlareWafEntries | Where-Object { $_.name -eq $customerUrl }
#if ($customerCloudFlareEntry -eq $null) {
#    Write-Host ("Url '" + $customerUrl + "' not present in DNS, creating, linking to " + $azureCustomerUrl)
#    $cloudFlareUrl = $cloudFlareBaseUrl + 'zones/' + $zoneId + '/dns_records'
#    $dnsEntry = '{"type":"CNAME","name":"' + $customerUrl + '","content":"' + $azureCustomerUrl + '","ttl":1,"proxied":true}'
#    Invoke-RestMethod -Method Post -Uri $cloudFlareUrl -Body $dnsEntry -Headers $cloudFlareHeaders
#} else {
#    Write-Host ("Url '" + $customerUrl + "' present in DNS")
#    # NB: Proxied means it is behind the WAF
#    if ($customerCloudFlareEntry.proxied -eq $true) {
#        Write-Host ("Url '" + $customerUrl + "' present in WAF")
#    } else {
#        Write-Host ("Url '" + $customerUrl + "' not present in WAF, switching WAF on")
#        $cloudFlareUrl = $cloudFlareBaseUrl + 'zones/' + $zoneId + '/dns_records/' + $customerCloudFlareEntry.id
#        $dnsEntry = '{"type":"CNAME","name":"' + $customerUrl + '","content":"' + $azureCustomerUrl + '","ttl":1,"proxied":true}'
#        $result = Invoke-RestMethod -Method PUT -Uri $cloudFlareUrl -Body $dnsEntry -Headers $cloudFlareHeaders
#    }
#}

# Create AWVerify Entries (Not proxied)
#$customerUrl = 'awverify.' + $SiteBaseUrl
#$cloudFlareUrl = $cloudFlareBaseUrl + 'zones/' + $zoneId + '/dns_records?type=CNAME&per_page=100&name=' + $customerUrl + '&match=all'
#$result = Invoke-RestMethod -Method Get -Uri $cloudFlareUrl -Headers $cloudFlareHeaders
#$cloudFlareWafEntries = $result.result
#$azureCustomerUrl = $SiteName.ToLower() + $EnvironmentName.ToLower() + ".azurewebsites.net"

#$customerCloudFlareEntry = $cloudFlareWafEntries | Where-Object { $_.name -eq $customerUrl }
#if ($customerCloudFlareEntry -eq $null) {
#    Write-Host ("Url '" + $customerUrl + "' not present in DNS, creating, linking to " + $azureCustomerUrl)
#    $cloudFlareUrl = $cloudFlareBaseUrl + 'zones/' + $zoneId + '/dns_records'
#    $dnsEntry = '{"type":"CNAME","name":"' + $customerUrl + '","content":"' + $azureCustomerUrl + '","ttl":1,"proxied":false}'
#    Invoke-RestMethod -Method Post -Uri $cloudFlareUrl -Body $dnsEntry -Headers $cloudFlareHeaders
#} else {
#    Write-Host ("Url '" + $customerUrl + "' present in DNS")
#}

#Write-Host ("Removing IP Address restrictions from scm site")
# NB: You need to do this otherwise Azure Devops can't deploy the site
#$apiVersion = ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq sites).apiVersions[0]
#$webAppConfig = (Get-AzureRmResource -ResourceType Microsoft.Web/sites/config -ResourceName $webSiteName -ResourceGroupName $resourceGroupName -apiVersion $apiVersion)
#$webAppConfig.Properties.scmIpSecurityRestrictions = @()
#Set-AzureRmResource -ResourceId $webAppConfig.ResourceId -Properties $webAppConfig.Properties -apiVersion $apiVersion -Force
#Write-Host ("Removed IP Address restrictions from scm site")

Write-Host ("Pre-Deploy complete")
