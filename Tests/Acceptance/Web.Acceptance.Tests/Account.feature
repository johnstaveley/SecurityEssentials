Feature: Account
	In order to securely access the application
	As a user
	I want to be manage my account

Background: 
	#Given the following user is setup in the database:
	#| Field            | Value                |
	#| UserName         | Test@test.com        |
	#| Password         | Testeration12        |
	#| LastLoginAttempt | 2016-06-30 12:00:01  |
	#| SecurityQuestion | Mother's maiden name |
	#| SecurityAnswer   | Baggins              |

Scenario: Home Page Loads
	Given I navigate to the website
	When I am taken to the homepage

@Ignore
Scenario: When I enter correct login details I am taken to the landing page
	Given I navigate to the website
	And I am taken to the homepage
	And I click login
	And I am taken to the login page
	And I enter the following login data:
	| Field    | Value         |
	| UserName | Test@test.com |
	| Password | Testeration12 |
	When I click the login button
	Then I am taken to the landing page and the following message is shown:
	| Field                                      |
	| User last logged in at 30/06/2016 12:00:01 |	

@Ignore
Scenario: When I enter valid registration details I can register a new user
	Given I navigate to the website
	And I click register in the title bar
	And I navigate to the 'Register' page
	And I enter the following registration details:
	| Field            | Value                              |
	| Username         | test                               |
	| Email            | test@staveley.org                  |
	| FirstName        | Test                               |
	| LastName         | Test                               |
	| SecurityQuestion | What is your mother's maiden name? |
	| SecurityAnswer   | Bloggs                             |
	| Password         | Test456789                         |
	| ConfirmPassword  | Test45678                          |
	When I submit my registration details
	Then I am taken to the registration success page

@Ignore
Scenario: When I enter a valid account and security information I can reset my password
	Given I navigate to the website
	And I click login
	And I click recover password
	And I am taken to the password recovery page
	And I enter the following recover data:
	| Field    | Value         |
	| UserName | Test@test.com |
	When I submit the password recovery form
	Then I am taken to the recover success page
	And I receive an email with a link

