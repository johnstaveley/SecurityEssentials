Feature: Account
	In order to securely access the application
	As a user
	I want to be manage my account

Background: 
	#Given the following user is setup in the database:
	#| Field    | Value         |
	#| UserName | Test          |
	#| Password | Testeration12 |
	#| LastLoginAttempt |2016-06-30 12:00:01 |

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
	| UserName | Test          |
	| Password | Testeration12 |
	When I submit the login form
	Then I am taken to the landing page and the following message is shown:
	| Field                                      |
	| User last logged in at 30/06/2016 12:00:01 |	

@ignore
Scenario: Can register
	Given I navigate to the website
	And I click register in the title bar
	And I navigate to the 'register' page
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
	Then I am shown a confirmation message

@Ignore
Scenario: Can reset password
	Given I navigate to the website
	And I click login
	And I click password reminder
	And I am taken to the password reminder page
	And I enter my email address
	When I click submit
	Then I am shown a confirmation message
	And I receive an email with a link

