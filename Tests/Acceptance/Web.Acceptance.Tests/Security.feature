Feature: Security
	In order to avoid information disclosure
	As a pen tester
	I want to be sure the application has the correct security settings and behaviour

Scenario: The web application will return the correct security headers
	When I call http get on the website
	Then the response headers will contain:
	| Key                    | Value         |
	| X-Frame-Options        | Deny          |
	| X-Content-Type-Options | nosniff       |
	| X-XSS-Protection       | 1; mode=block |
	And the response headers will not contain:
	| Key                 |
	| X-AspNet-Version    |
	| X-AspNetMvc-Version |
	| Server              |
 
Scenario: The application will prevent a brute force login attempt
	Given I navigate to the website
	And I am taken to the homepage
	And I click login
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | attempt1@user.com |
	| Password | rhubarb22         |
	And I click the login button
	And I navigate to the website
	And I am taken to the homepage
	And I click login
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | attempt2@user.com |
	| Password | rhubarb22         |
	And I click the login button as quickly as possible
	And I navigate to the website
	And I am taken to the homepage
	And I click login
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | attempt3@user.com |
	| Password | rhubarb22         |
	And I click the login button as quickly as possible
	And I navigate to the website
	And I am taken to the homepage
	And I click login
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | attempt4@user.com |
	| Password | rhubarb22         |
	When I click the login button as quickly as possible
	And I wait 2 seconds
	Then an error message is shown 'You have performed this action more than 3 times in the last 60 seconds.'

