Feature: Security
	In order to avoid information disclosure
	As a pen tester
	I want to be sure the application has the correct security settings and behaviour

Background:
	Given I delete all cookies from the cache 
	And I clear down the database
	And I have the standard set of lookups 

Scenario: The web application will log a content security policy violation
	Given I have a content security policy violation with details:
	| Field             | Value                                       |
	| BlockedUri        | http://myevilsite.com/stealdetails/capture/ |
	| DocumentUri       | http://mysite.com/innocentpage/             |
	| LineNumber        | 1                                           |
	| Referrer          |                                             | 
	| OriginalPolicy    | default-src http://localhost:4845           | 
	| ScriptSample      | #modernizr{font:0/0 a}#modernizr:after{c... | 
	| SourceFile        | http://mysite.com/innocentpage/             |
	| ViolatedDirective | default-src http://mysite.com               | 
	When I post the content security policy violation to the website 
	And I wait 2 seconds
	Then I have 1 content security policy violation in the system  
	And I have a log in the system matching the following:
	| Field   | Value                             |
	| Level   | Warning                           |
	| Message | Content Security Policy Violation |

Scenario: The web application will log a http public key pinning violation
	Given I have a http public key pinning violation with details:
	| Field                              | Value                            |
	| DateTime                           | 2017-12-07                       |
	| HostName                           | http://mysite.com/               | 
	| Port                               | 8080                             |
	| ExpirationDate                     | 2018-12-01                       |
	| IncludeSubDomains                  | True                             |
	| NotedHostName                      |                                  |
	| ServedCertificateChainDelimited    | pem1,pem2,pem3                   |
	| ValidatedCertificateChainDelimited | pem1,pem2,pem4                   |
	| KnownPinsDelimited                 | known-pin1,known-pin2,known-pin3 |
	When I post the http public key pinning violation to the website
	And I wait 2 seconds
	Then I have 1 http public key pinning violation in the system  
	And I have a log in the system matching the following:
	| Field   | Value                          |
	| Level   | Warning                        |
	| Message | HostName: "http://mysite.com/" |

Scenario: The web application will log a certificate policy violation
	Given I have a certificate policy violation with details:
	| Field          | Value              |
	| FailureDate    | [Now]              |
	| ExpirationDate | [1 Month From Now] |
	| HostName       | example.com        |
	| Port           | 8080               |
	| Source         | web                |
	And I have the following certificate policy violation scts:
	| SerialisedSct | Source        | Status | Version |
	| ABCDEFG       | tls-extension | valid  | 1       |
	| CDEFGHIJ      | tls-extension | valid  | 2       |
	When I post the certificate policy violation to the website
	And I wait 2 seconds
	Then I have 1 certificate policy violation in the system  
	And I have a log in the system matching the following:
	| Field   | Value                              |
	| Level   | Warning                            |
	| Message | Certificate Transparency Violation |

# TODO: Need two versions of this test, one for debugging locally and one for server based regression tests, then enable as @Smoke
@CheckForErrors
Scenario: The web application will return the correct security headers
	When I call http get on the website
	Then the response headers will contain: 
	| Key                     | Value                                                                                                                                                                                                                                                                                                                    |
	| X-Frame-Options         | Deny                                                                                                                                                                                                                                                                                                                     |
	| X-Content-Type-Options  | nosniff                                                                                                                                                                                                                                                                                                                  |
	| X-XSS-Protection        | 1; mode=block; report=/Security/CspReporting                                                                                                                                                                                                                                                                             |
	| Referrer-Policy         | origin                                                                                                                                                                                                                                                                                                                   |
	| Permissions-Policy      | geolocation=(), midi=(), camera=(),usb=(), magnetometer=(), sync-xhr=(), microphone=(), camera=(), gyroscope=(), speaker=(), payment=()                                                                                                                                                                                  |
	| Content-Security-Policy | default-src 'self'; style-src 'self' 'unsafe-inline' https://unpkg.com/gridjs/; img-src * data:; font-src 'self' https: data:; script-src 'self' https://unpkg.com/gridjs-jquery/; connect-src 'self'; frame-ancestors 'self'; form-action 'self'; base-uri 'self'; object-src 'none'; report-uri /Security/CspReporting |
	And the response headers will not contain:
	| Key                 |
	| X-AspNet-Version    | 
	| X-AspNetMvc-Version |
	| Server              | 
 
@CheckForErrors
Scenario: The application will prevent a brute force login attempt 
	Given I navigate to the website
	And I maximise the browser window
	And I am taken to the homepage
	And I click the login link in the navigation bar
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | attempt1@test.net |
	| Password | rhubarb22         |
	And I click the login button
	And I navigate to the website
	And I am taken to the homepage
	And I click the login link in the navigation bar
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | attempt2@test.net |
	| Password | rhubarb22         |
	And I click the login button as quickly as possible
	And I navigate to the website
	And I am taken to the homepage
	And I click the login link in the navigation bar
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | attempt3@test.net |
	| Password | rhubarb22         |
	And I click the login button as quickly as possible
	And I navigate to the website
	And I am taken to the homepage
	And I click the login link in the navigation bar
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | attempt4@test.net |
	| Password | rhubarb22         |
	When I click the login button as quickly as possible
	And I wait 2 seconds
	Then an error message is shown 'You have requested this resource too many times in the last 60 seconds.'

@CheckForErrors
Scenario: A user with an expired password is redirected to the change password page until they change the password
	Given the following users are setup in the database:
	| UserName       | Title | FirstName | LastName | Password        | SecurityQuestion                    | SecurityAnswer | IsAdmin | Approved | Enabled | WorkTelephoneNumber | HomeTelephoneNumber | MobileTelephoneNumber | Town | Postcode | SkypeName | PasswordExpiryDate |
	| user1@test.net | Mr    | Needs     | Approval | zasXX8576jFj123 | What is the name of your first pet? | Beatrix        | false   | true     | true    | 0123                | 0456                | 0789                  | town | postcode | skype     | [Expired]          |
	And the user 'user1@test.net' has the password expiry date set
	And I navigate to the website
	And I maximise the browser window
	And I click the login link in the navigation bar
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value           |
	| UserName | user1@test.net  |
	| Password | zasXX8576jFj123 |
	And I click the login button
	And I am navigated to the 'Change Password' page
	And I select Admin -> Change Security Information from the menu
	And I am navigated to the 'Change Password' page
	And I select Admin -> Manage Account from the menu
	And I am navigated to the 'Change Password' page
	And I enter the following change password data:
	| Field              | Value            |
	| CurrentPassword    | zasXX8576jFj123  |
	| NewPassword        | NewPassword45678 |
	| ConfirmNewPassword | NewPassword45678 |
	When I submit the change password form
	Then I am navigated to the 'Change Password Success' page
	And the user 'user1@test.net' does not have the password expiry date set
	And I select Admin -> Change Security Information from the menu
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value            |
	| UserName | user1@test.net   |
	| Password | NewPassword45678 |
	And I click the login button
	And I am navigated to the 'Change Security Information' page

