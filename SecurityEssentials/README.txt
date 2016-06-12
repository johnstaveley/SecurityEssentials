To set up the project take the following steps:

1) Register for a recaptcha account here: https://www.google.com/recaptcha/admin#list (I used localhost for the site name)

2) Create a file Config/PrivateAppSettings.config with your values here:

<appSettings>
  <add key="recaptchaPublicKey" value="" />
  <add key="recaptchaPrivateKey" value="" />
</appSettings>

3) Turn on recaptcha in web.config

4) Create a file Config/PrivateMailSettings.config with your values here:

<network host="localhost" port="25" defaultCredentials="true" />

