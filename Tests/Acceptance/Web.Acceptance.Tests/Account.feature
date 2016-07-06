Feature: Account
	In order to securely access the application
	As a user
	I want to be manage my account

Background: 
	Given I delete all cookies from the cache
	#Given the following users are setup in the database:
	#| # | UserName       | FirstName | LastName | Password          | LastLoginAttempt | SecurityQuestion                    | SecurityAnswer | PasswordResetToken                   | PasswordResetExpiry |
	#| # | user@user.com  | Standard  | User     | x12a;pP02icdjshER | Never            | What is the name of your first pet? | Mr Miggins     |                                      |                     |
	#| # | user2@user.com | Standard  | User     | x12a;pP02icdjshER | Never            | What is the name of your first pet? | Mr Miggins     | 83ababb4-a0c1-4f2c-8593-32dd40b920d2 | [One day from now]  |
	#| # | user3@user.com | Standard  | User     | x12a;pP02icdjshER | Never            | What is the name of your first pet? | Mr Miggins     |                                      |                     |

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

Scenario: When I enter registration details which are currently being used I am advised of registration success
	Given I navigate to the website
	And I click register in the title bar
	And I am navigated to the 'Register' page
	And I enter the following registration details:
	| Field            | Value                               |
	| Username         | user@user.com                       |
	| FirstName        | Standard                            |
	| LastName         | User                                |
	| SecurityQuestion | What is the name of your first pet? |
	| SecurityAnswer   | Mr Miggins                          |
	| Password         | x12a;pP02icdjshER                   |
	| ConfirmPassword  | x12a;pP02icdjshER                   |
	When I submit my registration details
	Then I am navigated to the 'Register Success' page
	#And I am notified via email

Scenario: When I attempt password recovery using a valid account I am notified of success
	Given I navigate to the website
	And I click login
	And I am navigated to the 'Login' page
	And I click recover password
	And I am navigated to the 'Recover' page
	And I enter the following recover data:
	| Field    | Value         |
	| UserName | Test@test.com |
	When I submit the recover form
	Then I am navigated to the 'Recover Success' page
	#And I receive an email with a reset link

Scenario: When I attempt password recovery using an invalid account I am notified of success
	Given I navigate to the website
	And I click login
	And I am navigated to the 'Login' page
	And I click recover password
	And I am navigated to the 'Recover' page
	And I enter the following recover data:
	| Field    | Value           |
	| UserName | Bogus@bogus.com |
	When I submit the recover form
	Then I am navigated to the 'Recover Success' page

Scenario: When I click on a valid password reset link, I can enter my security information and change my password	
	Given I navigate to the password reset link with token '83ababb4-a0c1-4f2c-8593-32dd40b920d2'
	And I am navigated to the 'Recover Password' page
	And I enter the following recover password data:
	| Field            | Value            |
	| SecurityAnswer   | Mr Miggins       |
	| Password         | NewPassword45678 |
	| Confirm Password | NewPassword45678 |
	When I submit the recover passord form
	Then I am navigated to the 'Recover Password Success' page
	#And I receive an email notifying me of the password change
	#And the password reset token is removed from the database

Scenario: I can change my password
	Given I navigate to the website
	And I click login
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | user3@user.com    |
	| Password | x12a;pP02icdjshER |
	And I click the login button
	And I am navigated to the 'Landing' page
	And I select Admin -> Change Password from the menu
	And I am navigated to the 'Change Password' page
	And I enter the following change password data:
	| Field              | Value             |
	| CurrentPassword    | x12a;pP02icdjshER |
	| NewPassword        | NewPassword45678  |
	| ConfirmNewPassword | NewPassword45678  |
	When I submit the change password form
	Then A confirmation message 'Your password has been changed.' is shown
	# And an email is sent
	# And a log entry is made

Scenario: The application will prevent a brute force login attempt
	Given I navigate to the website
	And I am taken to the homepage
	And I click login
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | attempt1@user.com |
	| Password | rhubarb           |
	And I click the login button
	And I navigate to the website
	And I am taken to the homepage
	And I click login
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | attempt2@user.com |
	| Password | rhubarb           |
	And I click the login button
	And I navigate to the website
	And I am taken to the homepage
	And I click login
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | attempt3@user.com |
	| Password | rhubarb           |
	And I click the login button
	And I navigate to the website
	And I am taken to the homepage
	And I click login
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | attempt4@user.com |
	| Password | rhubarb           |
	When I click the login button
	Then an error message is shown 'You have performed this action more than 3 times in the last 60 seconds.'