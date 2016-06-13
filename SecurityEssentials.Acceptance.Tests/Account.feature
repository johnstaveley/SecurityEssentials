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


