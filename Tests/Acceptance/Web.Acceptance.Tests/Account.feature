Feature: Account
	In order to securely access the application
	As a user
	I want to be manage my account

Background: 
	Given I delete all cookies from the cache 
	#Given the following users are setup in the database:
	#| UserName       | FirstName | LastName | Password          | LastLoginAttempt | SecurityQuestion                    | SecurityAnswer | PasswordResetToken                   | PasswordResetExpiry | NewEmailAddress  | NewEmailAddressToken                 | NewEmailAddressRequestExpiryDate |
	#| user@user.com  | Standard  | User     | x12a;pP02icdjshER | Never            | What is the name of your first pet? | Mr Miggins     |                                      |                     |                  |                                      |                                  |
	#| user2@user.com | Standard  | User     | x12a;pP02icdjshER | Never            | What is the name of your first pet? | Mr Miggins     | 83ababb4-a0c1-4f2c-8593-32dd40b920d2 | [One day from now]  |                  |                                      |                                  |
	#| user3@user.com | Standard  | User     | x12a;pP02icdjshER | Never            | What is the name of your first pet? | Mr Miggins     |                                      |                     |                  |                                      |                                  |
	#| user4@user.com | Standard  | User     | x12a;pP02icdjshER | Never            | What is the name of your first pet? | Mr Miggins     |                                      |                     | samuel@pepys.org | B386B07A-FF0C-4B2B-9DAD-7D32CFD5A92F | [One day from now]               |
	#| user5@user.com | Standard  | User     | x12a;pP02icdjshER | Never            | What is the name of your first pet? | Mr Miggins     |                                      |                     |                  |                                      |                                  |

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

@PAT
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

@PAT
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

@PAT
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

@PAT
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

@PAT
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

@PAT
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

@PAT
Scenario: I can change my account information
	Given I navigate to the website
	And I click login
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | user@user.com     |
	| Password | x12a;pP02icdjshER |
	And I click the login button
	And I am navigated to the 'Landing' page
	And I select Admin -> Manage Account from the menu
	And I am navigated to the 'User Edit' page
	And I enter the following change account information data:
	| Field                 | Value      |
	| Title                 | Mrs        |
	| FirstName             | Sarah      |
	| LastName              | Page       |
	| WorkTelephoneNumber   | 0123456789 |
	| HomeTelephoneNumber   | 0987654321 |
	| MobileTelephoneNumber | 0778412457 |
	| Town                  | Leeds      |
	| PostCode              | LS10 1EF   |
	| SkypeName             | SarahPage  |
	When I submit the manage account form
	Then A confirmation message 'Your account information has been changed' is shown
	#And The database now contains the following user information

@PAT
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

@PAT
Scenario: I can change my email address
	Given I navigate to the website
	And I click login
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | user5@user.com    |
	| Password | x12a;pP02icdjshER |
	And I click the login button
	And I am navigated to the 'Landing' page
	And I select Admin -> Change Email Address from the menu
	And I am navigated to the 'Change email address' page
	And I enter the following change email address data:
	| Field           | Value             |
	| Password        | x12a;pP02icdjshER |
	| NewEmailAddress | joe@bloggs.com    |
	When I submit the change email address form
	Then I am navigated to the 'Change Email Address Pending' page
	# And an email is sent
	# And a log entry is made

@PAT
Scenario: When I click on a valid change email address link, I change my email address to a new one
	When I navigate to the change email address link with token 'B386B07A-FF0C-4B2B-9DAD-7D32CFD5A92F'
	Then I am navigated to the 'Change Email Address Success' page
	#And I receive an email notifying me of the email address change
	#And The new email address receives an email notifying of the change
	#And the change email address token is removed from the database
	#And a log is made of the activity

@PAT
Scenario: I can change my security information
	Given I navigate to the website
	And I click login
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | user3@user.com    |
	| Password | x12a;pP02icdjshER |
	And I click the login button
	And I am navigated to the 'Landing' page
	And I select Admin -> Change Security Information from the menu
	And I am navigated to the 'Change security Information' page
	And I enter the following change security information data:
	| Field                 | Value                             |
	| Password              | x12a;pP02icdjshER                 |
	| SecurityQuestion      | What was your childhood nickname? |
	| SecurityAnswer        | Adelweiss                         |
	| SecurityAnswerConfirm | Adelweiss                         |
	When I submit the change security information form
	Then I am navigated to the 'Change Security Information Success' page
	# And an email is sent
	# And a log entry is made

@PAT
Scenario: I can view my user activity log information
	Given I navigate to the website
	And I wait 15 seconds
	# ^ Seemd to help the CI build fail less frequently due to repeated logon attempts in the same time period
	And I click login
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | user3@user.com    |
	| Password | x12a;pP02icdjshER |
	And I click the login button
	And I am navigated to the 'Landing' page
	When I select Admin -> Account Log from the menu
	Then I am navigated to the 'Account Log' page
	And I am shown the message 'Viewing ten most recent log entries for user3@user.com'

@PAT
Scenario: As an admin I can manage my users
	Given I navigate to the website
	And I click login
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | admin@admin.com   |
	| Password | xsHDjxshdjkKK917& |
	And I click the login button
	And I am navigated to the 'Landing' page
	And I select Admin -> Manage Users from the menu
	And I am navigated to the 'Manage Users' page
	When I click on user 1
	Then I am navigated to the 'User Edit' page
	And The following user edit information is displayed:
	| Field                 | Value           |
	| UserName              | admin@admin.com |
	| Title                 | Mrs             |
	| FirstName             | Admin           |
	| Surname               | User            |
	| MobileTelephoneNumber | 07740101235     |
	| Approved              | true            |
	| EmailVerified         | true            |
	| Enabled               | true            |


