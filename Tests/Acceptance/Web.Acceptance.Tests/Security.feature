@CheckForErrors
Feature: Security
	In order to avoid information disclosure
	As a pen tester
	I want to be sure the application has the correct security settings and behaviour

Background:
	Given I delete all cookies from the cache 
	And I clear down the database
	And I have the standard set of lookups

Scenario: The web application will return the correct security headers
	When I call http get on the website
	Then the response headers will contain: 
	| Key                    | Value         |
	| X-Frame-Options        | Deny          |
	| X-Content-Type-Options | nosniff       |
	| X-XSS-Protection       | 1; mode=block |
	| Referrer-Policy        | origin        | 
	And the response headers will not contain:
	| Key                 |
	| X-AspNet-Version    | 
	| X-AspNetMvc-Version |
	| Server              |
 
Scenario: The application will prevent a brute force login attempt
	Given I navigate to the website
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

Scenario: A user with an expired password is redirected to the change password page until they change the password
	Given the following users are setup in the database:
	| UserName       | Title | FirstName | LastName | Password        | SecurityQuestion                    | SecurityAnswer | IsAdmin | Approved | Enabled | WorkTelephoneNumber | HomeTelephoneNumber | MobileTelephoneNumber | Town | Postcode | SkypeName | PasswordExpiryDate |
	| user1@test.net | Mr    | Needs     | Approval | zasXX8576jFj123 | What is the name of your first pet? | Beatrix        | false   | true     | true    | 0123                | 0456                | 0789                  | town | postcode | skype     | [Expired]          |
	And the user 'user1@test.net' has the password expiry date set
	And I navigate to the website
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

