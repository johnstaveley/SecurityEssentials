<#
.SYNOPSIS
    Enables Azure Devops agent to connect to the sql database
.DESCRIPTION
	Calling this function opens a port in the firewall so that the unit tests can change the database state before running integration tests
.NOTES
    Author: John Staveley
    Date:   17/03/2020    
#>
param (
	[Parameter(Mandatory=$true)]
	[string] $ResourceGroup, 
	[Parameter(Mandatory=$true)]
	[string] $ServerName,
	[Parameter(Mandatory=$true)]
	[string] $RuleName
	)
Write-Host ("Eanble Database access for Testing")
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
#NB: This puts a dependency on the website http://ipinfo.io/json, but it seems to work fine
$agentIPAddresss = Invoke-RestMethod http://ipinfo.io/json | Select -exp ip
Write-Host ("Adding firewall rule $RuleName for Server $ServerName allowing IP Address $agentIPAddresss")
New-AzureRmSqlServerFirewallRule -ResourceGroupName $ResourceGroup -ServerName $ServerName -FirewallRuleName $RuleName -StartIpAddress "$($agentIPAddresss)" -EndIpAddress "$($agentIPAddresss)"
Write-Host ("Eanble Database access for Testing Complete")