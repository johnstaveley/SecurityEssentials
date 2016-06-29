Feature: Account
	In order to securely access the application
	As a user
	I want to be manage my account

Background: 
	#Given the following user is setup in the database:
	#| Field    | Value         |
	#| UserName | Test          |
	#| Password | Testeration12 |

Scenario: Home Page Loads
	Given I navigate to the website
	When I am taken to the homepage

@Ignore
Scenario: I can login
	Given I navigate to the website
	And I am taken to the homepage
	When I click login
	Then I am taken to the login page

@Ignore
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
	And I click register
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

