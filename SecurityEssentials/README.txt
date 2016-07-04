To set up the project take the following steps:

1) Register for a recaptcha account here: https://www.google.com/recaptcha/admin#list (I used localhost for the site name)

2) Put your settings into the web.config/AppSettings under recaptchaPublicKey and recaptchaPrivateKey

3) Turn on recaptcha in web.config HasRecaptcha = true

4) Create a file Config/PrivateMailSettings.config with your values here:

<network host="localhost" port="25" defaultCredentials="true" />

5) if you deploy this into production make sure you remove the two test users from the databaseInitializer