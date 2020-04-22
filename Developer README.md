Local Project Setup
-------------------

To set up the project take the following steps:

1) If using Recaptcha:
	1.1) Register for a recaptcha account here: https://www.google.com/recaptcha/admin#list (I used localhost for the site name)
	1.2) Put your settings into the web.config/AppSettings under recaptchaPublicKey and recaptchaPrivateKey
	1.3) Turn on recaptcha in web.config HasRecaptcha = true
	1.4) Amend your content security policy thus: (taken from https://developers.google.com/recaptcha/docs/faq)

    script-src https://www.google.com/recaptcha/, https://www.gstatic.com/recaptcha/
    frame-src https://www.google.com/recaptcha/
    style-src 'unsafe-inline'

2) Enable mail to be sent
	2.1) Update your mail settings in web.config here:
	<smtp host="localhost" port="25" defaultCredentials="true" />
	2.2) Set in web.config HasEmailConfigured = true

3) If you deploy this into production make sure you remove the test users from the databaseInitializer
4) Apply the NoCache attribute to any controllers which have sensitive data
5) Search for TODO in code to see further actions which could be taken
6) When creating a build pipeline for your company, ensure you add in the extensions mentioned in azure-pipelines.yml
7) If you want to do threat modelling then install Microsoft threat modelling tool preview https://aka.ms/tmtpreview

Azure Devops Setup
------------------
In order to keep your solution secure you should run the acceptance tests on each checkin. Take the following steps to set up an automated build in Azure:

* Create a new build pipeline in Azure Devops and reference file azure-pipelines.yml from this solution
* Set up account in sonarcloud.io and generate a token in Account -> Security
* Create the following pipeline variables as follows: Those marked with * should be created as a secret
	+ AdminEmailAddresses - email addresses for notifications and alerts NB: This can only support 1 email address at the moment
	+ AppServiceName - Name of the app service resource in Azure e.g. securityessentialsint
	+ ArmTemplateOutput - set to blank string. Check the box 'Let users override this value when running this pipeline'
	+ AzureLocation - location of azure data centre e.g. UKSouth
	+ CloudFlareAuthEmail - Username for Cloudflare
	+ CloudFlarePlan - One of Free, Pro, Business or Enterprise
	+ CloudFlareUserServiceKey - * Service Key for Cloudflare (this can be the global api key in your profile or an api key created with sufficient permissions). 
	+ CloudFlareZoneName - Zone in Cloudflare such as the name of your site
	+ EncryptionPassword - * Cryptographically secure random string of at least 20 characters/digits/symbols in length
	+ DeveloperIpAddresses - Comma delimited list of IP addresses of any developers that need access to the backend database TOOD: This can only accept 1 IP address at the moment
	+ EnvironmentName - environment short hand, e.g. int, qa, uat, live
	+ ResourceGroup - Name of resource group to deploy to e.g. SecurityEssentials-Int
	+ ServiceConnection - Azure Devops service connection approved to deploy to Azure from Azure Devops. In Azure Devops, go to Project settings -> Service Connections
	+ SiteBaseUrl - A stem url for the site e.g. securityessentials.org
	+ SiteName - name of the website to set up e.g. SecurityEssentials
	+ SonarCloudConnectionName - connection name set up in AzureDevops to connect to SonarCloud e.g. SonarCloud.io
	+ SonarCloudOrganisation - name of the organisation from sonarcloud.io setup e.g. Acme
	+ SonarCloudProjectKey - Project key from sonarcloud.io indicating where to store the reports e.g. SecurityEssentials
	+ SonarCloudProjectName - Project name from sonarcloud.io e.g. SecurityEssentials
	+ SqlAdminPassword - * admin password for sql azure e.g. a secure randomly generated 15 digit password
	+ SqlAdminUserName - admin username for sql azure e.g. AdminUser1486
	+ SqlServerName - the name of the sql server to use e.g. securityessentialsint
	+ StorageAccountNonVNetName - the name of the storage container which is not on the vnet e.g. securityessentialsint
	+ SubscriptionId - guid representing the subscription in Azure. Search in Azure portal under subscriptions
	+ WebDatabaseName - name of the sql database e.g. securityessentialsint
* Add the following task types from the Azure Marketplace as follows:
	+ OWASP Dependency checker
	+ OWASP Zap Scanner. This is the version created by the MS Devops team
	+ White source bolt. This needs activation to run. See video here: https://bolt.whitesourcesoftware.com/whitesource-bolt-azure-devops#activate (You don't need to select to receive emails)
	+ Secure DevOps Kit (AzSK) CICD Extensions for Azure tasks which have to be added to Azure Devops from the marketplace
	+ Microsoft Security Code Analysis. If you purchase this extension it is shared with you, add from shared from the marketplace. If you do not purchase this then remove any tasks marked MSCA from azure-pipelines.yml
	+ SonarCloud. If your project is public you can use the free version otherwise you have to use the paid version
* Set up service connection in Azure Devops -> Project Settings -> Service Connections ->
    + Add new Azure Resource Manager service connection. This is to allow control of the Resource Group in Azure by Azure Devops
	+ Add new SonarCloud connection with the same name as set in variable SonarCloudConnectionName above and created using token from setup sonarcloud step
* In Azure
	+ Manage Service Principal -> View API Permissions -> Add Permission -> Azure Key Vault. Grant Admin Consent for Default Directory
* Run the deployment once, this will fail but create the key vault. 
	+ In Key Vault -> Access Policies -> Add Access Policy -> Configure from template (key, Secret and Certificate Management), select the service connection principal from Azure Devops and yourself
	+ Repeat the above but add yourself as a user with all permissions
	+ Note that the app service custom domain setup may fail until the DNS settings in cloudflare are propagated

Notes
-----

Screenshots from failed selenium tests are stored in StorageAccountNonVNetName in container Selenium

Whitesource bolt free will allow you to scan your provide up to 5 times per day, for unlimited, get the paid version.

Secure DevOps Kit for Azure (AzSK). Documentation is here: https://github.com/azsk/DevOpsKit-docs and here: https://azsk.azurewebsites.net/03-Security-In-CICD/Readme.html#enable-azsk-extension-for-your-vsts-1
	Azure_SQLDatabase_AuthZ_Use_AAD_Admin - switched off because we don't run an AD server
	Azure_AppService_AuthN_Use_AAD_for_Client_AuthN - removed as application has its own user database
	Azure_SQLDatabase_Audit_Enable_Threat_Detection_Server - removed as this rule signature enforces a format which does not comply with ARM Template format
	Azure_Storage_BCDR_Enable_Soft_Delete - Soft delete off just for assets not on the virtual network

The Microsoft Security Code Analysis tool set contains the following build tasks to check for security. Details are here: https://docs.microsoft.com/en-us/azure/security/develop/security-code-analysis-overview
	Anti-Malware scanner
	Binskim
	Credential Scanner
	Microsoft Security Risk Detection
	Roslyn Analyzers
	TSLint

BinSkim throws an exception with setting "AnalyzeTarget: '$(Build.ArtifactStagingDirectory)\*.dll'" due to the following issues:
	Pdb files aren't included with all NuGet packages. This includes: AntiXssLibrary.dll, HtmlSanitizationLibrary.dll, libargon2.dll etc
	some libraries are signed using SHA1. This includes: Microsoft.Azure.KeyVault.Core.dll, System.Web.Http.OData.dll etc
I have temporarily got around this problem by just scanning SecurityEssentials.dll without packages

Recaptcha repository: https://github.com/tanveery/recaptcha-net, Documentation: https://github.com/tanveery/recaptcha-net/blob/master/README.md

Known Issues
------------

Creating secrets such as passwords with $^ in them can cause the build pipeline to fail, this is because the characters are handled properly in powershell.