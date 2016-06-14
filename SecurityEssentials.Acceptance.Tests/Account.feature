@Ignore
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

Scenario: Can register
	Given I navigate to the website
	And I click register
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
	And I click submit
	Then I am shown a confirmation message

