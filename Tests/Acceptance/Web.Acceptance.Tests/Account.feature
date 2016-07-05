Feature: Account
	In order to securely access the application
	As a user
	I want to be manage my account

Background: 
	#Given the following user is setup in the database:
	#| Field            | Value                |
	#| UserName         | user@user.com        |
	#| Password         | Testeration12        |
	#| LastLoginAttempt | 2016-06-30 12:00:01  |
	#| SecurityQuestion | Mother's maiden name |
	#| SecurityAnswer   | Baggins              |

Scenario: Home Page Loads
	Given I navigate to the website
	When I am taken to the homepage

Scenario: When I enter correct login details I am taken to the landing page
	Given I navigate to the website
	And I am taken to the homepage
	And I click login
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | user@user.com     |
	| Password | x12a;pP02icdjshER |
	When I click the login button
	Then I am navigated to the 'Landing' page
	Then the following last activity message is shown: 'The last actvity logged against your account was'

Scenario: When I enter incorrect login details then a warning is displayed
	Given I navigate to the website
	And I am taken to the homepage
	And I click login
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | user@user.com     |
	| Password | y12a;pP02icdjshET |
	When I click the login button
	Then The following errors are displayed:
	| Field                                        |
	| Invalid credentials or the account is locked |

Scenario: When I enter valid registration details I can register a new user
	Given I navigate to the website
	And I click register in the title bar
	And I am navigated to the 'Register' page
	And I enter the following registration details:
	| Field            | Value                              |
	| Username         | test@test.com                      |
	| FirstName        | Tester                             |
	| LastName         | Tester                             |
	| SecurityQuestion | What is your mother's maiden name? |
	| SecurityAnswer   | Bloggs                             |
	| Password         | Test456789                         |
	| ConfirmPassword  | Test456789                         |
	When I submit my registration details
	Then I am navigated to the 'Register Success' page
	#And I receive a registration email

Scenario: When I enter a valid account and security information I can reset my password
	Given I navigate to the website
	And I click login
	And I am navigated to the 'Login' page
	And I click recover password
	And I am navigated to the 'Recover' page
	And I enter the following recover data:
	| Field    | Value         |
	| UserName | Test@test.com |
	When I submit the password recovery form
	Then I am navigated to the 'Recover Success' page
	#And I receive an email with a reset link

