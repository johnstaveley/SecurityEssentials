To set up the project take the following steps:

1) Register for a recaptcha account here: https://www.google.com/recaptcha/admin#list (I used localhost for the site name)

2) Put your settings into the web.config/AppSettings under recaptchaPublicKey and recaptchaPrivateKey

3) Turn on recaptcha in web.config HasRecaptcha = true

4) Create a file Config/PrivateMailSettings.config with your values here:

<network host="localhost" port="25" defaultCredentials="true" />

5) Set in web.config HasEmailConfigured = true

6) if you deploy this into production make sure you remove the test users from the databaseInitializer


Developer Notes
---------------

In order to get the acceptance tests running on my CI server I have included the 32 bit version of GeckoDriver.exe in the project. If you use Firefpx 64bit, you will need to replace this with the 64 bit version

Known Issues
------------

If you use the Argon2 hashing algorithm then it will make use of an unmanaged dll called libargon2.dll. This needs to be put in the path (e.g. C:/Windows/SysWOW64/) for any executing code (either the web application or a CI server running the Argon2 unit tests). If you find a way around this restriction using unmanaged code then please let me know. 
