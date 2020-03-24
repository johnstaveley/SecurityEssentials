<#
.SYNOPSIS
    Removes all charging assets created by the Security Essentials website
.NOTES
    Author: John Staveley
    Date:   15/03/2020    
#>
Param(
	[Parameter(Mandatory=$true)]
	[string] $EnvironmentName,
	[Parameter(Mandatory=$true)]
	[string] $SiteName,
    [Parameter(Mandatory=$true)]
	[string] $SubscriptionId
)

Write-Host("Teardown started")
$ErrorActionPreference = 'Continue'
[string] $resourceGroupName = $SiteName + '-' + $EnvironmentName
[string] $siteNameLowerCase = $SiteName.ToLower()
[string] $appServiceName = $siteNameLowerCase + $EnvironmentName.ToLower()
[string] $appServicePlanName = $siteNameLowerCase + $EnvironmentName.ToLower()
[string] $sqlServerName = $siteNameLowerCase + $EnvironmentName.ToLower()
[string] $virtualNetworkName = $siteNameLowerCase + $EnvironmentName.ToLower()
[string] $networkSecurityGroupName = $siteNameLowerCase + $EnvironmentName.ToLower() + '-frontend'
[string] $applicationInsightsName = $siteNameLowerCase + $EnvironmentName.ToLower()
[string] $webTestName = $siteNameLowerCase + $EnvironmentName.ToLower() + '-webtestisalive'

# Create or check for existing resource group
$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction SilentlyContinue
if($resourceGroup) {
    Write-Host "Resource group '$resourceGroupName' found";
	Write-Host("Removing App Service $appServiceName from resource group $resourceGroupName")
	Remove-AzureRmResource -ResourceGroupName $resourceGroupName -ResourceName $appServiceName -ResourceType Microsoft.Web/sites -Force 
	Write-Host("Removing App Service Plan $appServicePlanName from resource group $resourceGroupName")
	Remove-AzureRmResource -ResourceGroupName $resourceGroupName -ResourceName $appServicePlanName -ResourceType Microsoft.Web/serverfarms -Force 
	Write-Host("Removing Sql Server $sqlServerName from resource group $resourceGroupName")
	Remove-AzureRmResource -ResourceGroupName $resourceGroupName -ResourceName $sqlServerName -ResourceType Microsoft.Sql/servers -Force
	Write-Host("Removing Virtual network $virtualNetworkName from resource group $resourceGroupName")
	Remove-AzureRmResource -ResourceGroupName $resourceGroupName -ResourceName $virtualNetworkName -ResourceType Microsoft.Network/virtualNetworks -Force
	Write-Host("Removing Network security group $networkSecurityGroupName from resource group $resourceGroupName")
	Remove-AzureRmResource -ResourceGroupName $resourceGroupName -ResourceName $networkSecurityGroupName -ResourceType Microsoft.Network/networkSecurityGroups -Force
	Write-Host("Removing Web Test $webTestName from resource group $resourceGroupName")
	Remove-AzureRmResource -ResourceGroupName $resourceGroupName -ResourceName $webTestName -ResourceType microsoft.insights/webtests -Force
	Write-Host("Removing Application Insights $applicationInsightsName from resource group $resourceGroupName")
	Remove-AzureRmResource -ResourceGroupName $resourceGroupName -ResourceName $applicationInsightsName -ResourceType Microsoft.Insights/components -Force
} else {
	Write-Host "Resource group '$resourceGroupName' not found";
}

Write-Host ("Teardown complete")
