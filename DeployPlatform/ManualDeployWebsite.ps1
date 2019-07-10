#Requires -Version 3.0

Param(
    [string] $ResourceGroupLocation = 'UKSouth',
	[string] $SiteName = 'SecurityEssentials',
	[string] $EnvironmentName = 'QA',
    [string] $TemplateFile = 'Website.json',
    [string] $TemplateParametersFile = 'WebSite.qa.parameters.json',
	[Parameter(Mandatory=$true)]
	[string] $SubscriptionId,
    [Parameter(Mandatory=$true)]
    [string] $SqlAdminPassword,
    [switch] $ValidateOnly = $False
)

try {
    [Microsoft.Azure.Common.Authentication.AzureSession]::ClientFactory.AddUserAgent("VSAzureTools-$UI$($host.name)".replace(' ','_'), '3.0.0')
} catch { }

[string] $ResourceGroupName = $SiteName + '-' + $EnvironmentName
[string] $siteNameLowercase = $SiteName.ToLower();
[string] $vaultName = $siteNameLowerCase + $EnvironmentName.ToLower()
[string] $keyVaultDiagnosticsName = $siteNameLowerCase + $EnvironmentName.ToLower() + 'diagnostics'

# Use Disconnect-AzureRmAccount first if not pointing at correct subscription
if ((Get-AzureRmContext) -eq $null -or [string]::IsNullOrEmpty($(Get-AzureRmContext).Account)) { 
    Write-Host "Logging in...";
    Login-AzureRmAccount 
}

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3

# Select subscription
Write-Host "Selecting subscription '$SubscriptionId'"
Select-AzureRmSubscription -SubscriptionID $SubscriptionId -ErrorAction Stop
Set-AzureRmContext -SubscriptionId $SubscriptionId -ErrorAction Stop

# Create or check for existing resource group
$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction SilentlyContinue
if(!$resourceGroup)
{
    Write-Host "Creating resource group '$resourceGroupName' in location '$resourceGroupLocation'";
    New-AzureRmResourceGroup -Name $resourceGroupName -Location $resourceGroupLocation
    Start-Sleep(5) # Give it some time to create the resource group
}
else{
    Write-Host "Using existing resource group '$resourceGroupName'";
}

# Create the storage accounts if they don't already exist
$vNetStorageAccountName = $siteNameLowercase + $EnvironmentName.tolower() + 'vnet'
$vNetStorageAccount = (Get-AzureRmStorageAccount | Where-Object {$_.StorageAccountName -eq $vNetStorageAccountName})
if ($vNetStorageAccount -eq $null) {
	Write-Host "Creating Storage Account '$vNetStorageAccountName' in $resourceGroupName" 
    $vNetStorageAccount = New-AzureRmStorageAccount -StorageAccountName $vNetStorageAccountName -Type 'Standard_GRS' -ResourceGroupName $resourceGroupName -Location $ResourceGroupLocation -EnableHttpsTrafficOnly $True
} else {
	Write-Host "Storage Account '$vNetStorageAccountName' in $resourceGroupName already exists" 
}

$storageAccountName = $siteNameLowercase + $EnvironmentName.tolower()
$storageAccount = (Get-AzureRmStorageAccount | Where-Object {$_.StorageAccountName -eq $storageAccountName})
if ($storageAccount -eq $null) {
	Write-Host "Creating Storage Account '$storageAccountName' in $resourceGroupName" 
    $storageAccount = New-AzureRmStorageAccount -StorageAccountName $storageAccountName -Type 'Standard_LRS' -ResourceGroupName $resourceGroupName -Location $ResourceGroupLocation -EnableHttpsTrafficOnly $True
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
    $vault = New-AzureRMKeyVault -vaultName $vaultName -ResourceGroupName $resourceGroupName -location $ResourceGroupLocation -enabledfortemplatedeployment
} else {
    Write-Host "Vault '$vaultName' in $resourceGroupName already exists" 
    $vault = $matchingVaults[0]
}

# Enable logging for key vault
# TODO: Change AuditEvent to AuditEvent,AllMetrics when the powershell command supports it. In the meantime, set this manually
Set-AzureRmDiagnosticSetting -ResourceId $vault.ResourceId -StorageAccountId $storageAccount.Id -Enabled $true -Categories AuditEvent -RetentionEnabled $true -RetentionInDays 365 `
    -Name $keyVaultDiagnosticsName

ipconfig /flushdns


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

function Format-ValidationOutput {
    param ($ValidationOutput, [int] $Depth = 0)
    Set-StrictMode -Off
    return @($ValidationOutput | Where-Object { $_ -ne $null } | ForEach-Object { @('  ' * $Depth + ': ' + $_.Message) + @(Format-ValidationOutput @($_.Details) ($Depth + 1)) })
}

$TemplateFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $TemplateFile))
$TemplateParametersFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $TemplateParametersFile))

if ($ValidateOnly) {
    $ErrorMessages = Format-ValidationOutput (Test-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName `
                                                                                  -TemplateFile $TemplateFile `
                                                                                  -TemplateParameterFile $TemplateParametersFile)
    if ($ErrorMessages) {
        Write-Output '', 'Validation returned the following errors:', @($ErrorMessages), '', 'Template is invalid.'
    }
    else {
        Write-Output '', 'Template is valid.'
    }
}
else {
    New-AzureRmResourceGroupDeployment -Name ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
                                       -ResourceGroupName $ResourceGroupName `
                                       -TemplateFile $TemplateFile `
                                       -TemplateParameterFile $TemplateParametersFile `
                                       -Force -Verbose `
                                       -ErrorVariable ErrorMessages
    if ($ErrorMessages) {
        Write-Output '', 'Template deployment returned the following errors:', @(@($ErrorMessages) | ForEach-Object { $_.Exception.Message.TrimEnd("`r`n") })
    }
}