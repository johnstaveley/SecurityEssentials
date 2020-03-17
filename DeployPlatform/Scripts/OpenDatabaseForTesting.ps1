<#
.SYNOPSIS
    Enables Azure Devops agent to connect to the sql database
.NOTES
    Author: John Staveley
    Date:   17/03/2020    
#>
param (
	[string] $Address, 
	[string] $Token, 
	[string] $ResourceGroup, 
	[string] $ServerName,
	[string] $RuleName
	)
# Calling this function opens a port in the firewall so that the unit tests can change the database state before running integration tests
Write-Host ("Eanble Database access for Testing")
Write-Host "Making call to $Address at Resource Group $ResourceGroup, Server Name $ServerName and Rule Name $RuleName"
$basicAuth = [System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes(("{0}:{1}" -f 'test', $Token)))
$headers = @{Authorization=("Basic {0}" -f $basicAuth)}
Invoke-RestMethod http://ipinfo.io/json | Select -exp ip
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$result = Invoke-RestMethod -Uri $Address -headers $headers -Method Get
Write-Host ("IP Address " + $result.value)
New-AzureRmSqlServerFirewallRule -ResourceGroupName $ResourceGroup -ServerName $ServerName -FirewallRuleName $RuleName -StartIpAddress "$($result.value)" -EndIpAddress "$($result.value)"
Write-Host ("Eanble Database access for Testing Complete")