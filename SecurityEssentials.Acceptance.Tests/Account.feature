Feature: Account
	In order to securely access the application
	As a user
	I want to be manage my account

Background: 
	Given the following user is setup in the database:
	| Field    | Value         |
	| UserName | Test          |
	| Password | Testeration12 |

@mytag
Scenario: Log in to account
	Given I navigate to the login page
	And I enter the following user credentials
	When I login
	Then I will be displayed the following landing page
