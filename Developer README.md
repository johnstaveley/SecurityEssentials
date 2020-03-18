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

Azure Devops Setup
------------------
In order to keep your solution secure you should run the acceptance tests on each checkin. Take the following steps to set up an automated build in Azure:

* Create a new build pipeline in Azure Devops and reference file azure-pipelines.yml from this solution
* Create the following pipeline variables as follows:
	+ AppServiceName - Name of the app service resource in Azure e.g. securityessentialsint
	+ AdminEmailAddresses - email addresses for notifications and alerts NB: This can only support 1 email address at the moment
	+ AzureLocation - location of azure data centre e.g. UKSouth
	+ CloudFlareAuthEmail - Username for Cloudflare
	+ CloudFlareUserServiceKey - Service Key for Cloudflare
	+ CloudFlareZoneName - Zone id in Cloudflare
	+ DeveloperIpAddresses - Comma delimited list of IP addresses of any developers that need access to the backend database TOOD: This can only accept 1 IP address at the moment
	+ EnvironmentName - environment short hand, e.g. int, qa, uat, live
	+ ResourceGroup - Name of resource group to deploy to e.g. SecurityEssentials-Int
	+ ServiceConnection - Azure Devops service connection approved to deploy to Azure from Azure Devops. In Azure Devops, go to Project settings -> Service Connections
	+ SiteName - name of the website to set up e.g. SecurityEssentials
	+ SqlAdminPassword - admin password for sql azure e.g. a secure randomly generated 15 digit password
	+ SqlAdminUserName - admin username for sql azure e.g. AdminUser1486
	+ SqlServerName - the name of the sql server to use e.g. securityessentialsint
	+ StorageAccountNonVNetName - the name of the storage container which is not on the vnet e.g. securityessentialsint
	+ SubscriptionId - guid representing the subscription in Azure. Search in Azure portal under subscriptions
	+ WebDatabaseName - name of the sql database e.g. securityessentialsint
* Add the following task types from the Azure Marketplace as follows:
	+ OWASP Dependency checker
	+ White source bolt. This needs activation to run. See video here: https://bolt.whitesourcesoftware.com/whitesource-bolt-azure-devops#activate (You don't need to select to receive emails)
	+ Secure DevOps Kit (AzSK) CICD Extensions for Azure tasks which have to be added to Azure Devops from the marketplace	
* Set up service connection in Azure Devops -> Project Settings -> Service Connections -> Add new Azure Resource Manager service connection. This is to allow control of the Resource Group in Azure by Azure Devops
* In Azure
	+ Manage Service Principal -> View API Permissions -> Add Permission -> Azure Key Vault. Grant Admin Consent for Default Directory
* Run the deployment once, this will fail but create the key vault. 
	+ In Key Vault -> Access Policies -> Add Access Policy -> Configure from template (key, Secret and Certificate Management), select the principal from Azure Devops
	+ Repeat the above but add yourself as a user with all permissions

Notes
-----

Screenshots from failed selenium tests are stored in StorageAccountNonVNetName in container Selenium
Secure DevOps Kit for Azure (AzSK). Documentation is here: https://github.com/azsk/DevOpsKit-docs and here: https://azsk.azurewebsites.net/03-Security-In-CICD/Readme.html#enable-azsk-extension-for-your-vsts-1
	Azure_SQLDatabase_AuthZ_Use_AAD_Admin - switched off because we don't run an AD server
	Azure_SQLDatabase_Audit_Enable_Threat_Detection_Server - switched off because threat detection is enabled at the server level and so doesn't need to be enabled at the database level
	Azure_AppService_AuthN_Use_AAD_for_Client_AuthN - removed as application has its own user database
Recaptcha repository: https://github.com/tanveery/recaptcha-net, Documentation: https://github.com/tanveery/recaptcha-net/blob/master/README.md
