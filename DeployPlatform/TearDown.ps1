<#
.SYNOPSIS
    Removes all charging assets created by the  site Security Essentials
.NOTES
    Author: John Staveley
    Date:   15/03/2020    
#>
Param(
	[Parameter(Mandatory=$true)]
	[string] $AzureLocation,
	[Parameter(Mandatory=$true)]
	[string] $EnvironmentName,
	[Parameter(Mandatory=$true)]
	[string] $SiteName,
    [Parameter(Mandatory=$true)]
	[string] $SubscriptionId
    [Parameter(Mandatory=$true)]
)


Write-Host("Setup variables")
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3
[string] $resourceGroupName = $SiteName + '-' + $EnvironmentName

# Select subscription
Write-Host "Selecting subscription '$SubscriptionId'"
Select-AzureRmSubscription -SubscriptionID $SubscriptionId -ErrorAction Stop
Set-AzureRmContext -SubscriptionId $SubscriptionId -ErrorAction Stop

# Create or check for existing resource group
$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction SilentlyContinue
if($resourceGroup)
{
    Write-Host "Resource group '$resourceGroupName' found";
}

Write-Host ("Teardown complete")
