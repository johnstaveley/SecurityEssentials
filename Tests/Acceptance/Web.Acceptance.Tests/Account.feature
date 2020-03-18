@CheckForErrors
Feature: Account
	In order to securely access the application
	As a user
	I want to be manage my account

Background: 
	Given I delete all cookies from the cache 
	And I clear down the database
	And I have the standard set of lookups 

Scenario: When I enter correct login details I am taken to the landing page
	Given the following users are setup in the database:
	| UserName      | FirstName | LastName | Password          | SecurityQuestion                    | SecurityAnswer | PasswordResetToken | PasswordResetExpiry | NewEmailAddress | NewEmailAddressToken | NewEmailAddressRequestExpiryDate |
	| user@test.net | Standard  | User     | x12a;pP02icdjshER | What is the name of your first pet? | Mr Miggins     |                    |                     |                 |                      |                                  |  
	And I navigate to the website 
	And I am taken to the homepage
	And I maximise the browser window
	And I click the login link in the navigation bar 
	And I am navigated to the 'login' page 
	And I enter the following login data:  
	| Field    | Value             |
	| UserName | user@test.net     |
	| Password | x12a;pP02icdjshER | 
	When I click the login button
	Then I am navigated to the 'Landing' page
	And the following last activity message is shown: 'the last activity logged against your account was'
	And I have the following user logs in the system:
	| Description      |
	| User Logged On   |

@PAT
Scenario: When I enter incorrect login details then a warning is displayed
	Given the following users are setup in the database:
	| UserName      | FirstName | LastName | Password          | SecurityQuestion                    | SecurityAnswer | PasswordResetToken | PasswordResetExpiry | NewEmailAddress | NewEmailAddressToken | NewEmailAddressRequestExpiryDate |
	| user@test.net | Standard  | User     | x12a;pP02icdjshER | What is the name of your first pet? | Mr Miggins     |                    |                     |                 |                      |                                  |
	And I navigate to the website	
	And I am taken to the homepage
	And I maximise the browser window
	And I click the login link in the navigation bar
	And I am navigated to the 'login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | user@test.net     |
	| Password | y12a;pP02icdjshET |
	When I click the login button
	Then The following errors are displayed on the 'Login' page:
	| Field                                        |
	| Invalid credentials or the account is locked |
	And I have the following user logs in the system:
	| Description          |
	| Failed Logon attempt |

@PAT @Smoke
Scenario: When I enter valid registration details I can register a new user
	Given I navigate to the website
	And I maximise the browser window
	And I click register in the title bar
	And I am navigated to the 'Register' page
	And I enter the following registration details:
	| Field            | Value                              |
	| Username         | test@test.net                      |
	| FirstName        | Tester1                            |
	| LastName         | Tester2                            |
	| SecurityQuestion | What is your mother's maiden name? |
	| SecurityAnswer   | Bloggs                             |
	| Password         | Test456789££2123435                |
	| ConfirmPassword  | Test456789££2123435                |
	When I submit my registration details
	Then I am navigated to the 'Register Success' page
	And I have the following user logs in the system:
	| Description     |
	| Account Created |
	And I have the following users in the system:
	| UserName      | HashStrategy         | Enabled | Approved | EmailVerified | FirstName | LastName |
	| test@test.net | Pbkdf28000Iterations | True    | True     | False         | Tester1   | Tester2  |
	#And I receive a registration email

Scenario: When I enter valid registration details with a pwned password, an error is displayed
	Given I navigate to the website
	And I maximise the browser window
	And I click register in the title bar
	And I am navigated to the 'Register' page
	And I enter the following registration details:
	| Field            | Value                              |
	| Username         | test@test.net                      |
	| FirstName        | Tester1                            |
	| LastName         | Tester2                            |
	| SecurityQuestion | What is your mother's maiden name? |
	| SecurityAnswer   | Bloggs                             |
	| Password         | Password12345                      |
	| ConfirmPassword  | Password12345                      |
	When I submit my registration details
	Then The following errors are displayed on the 'Register' page:
	| Field                                                                           |
	| Your password has previously been found in a data breach, please choose another |
	And I have the following user logs in the system:
	| Description     |
	And I have the following users in the system:
	| UserName      | HashStrategy         | Enabled | Approved | EmailVerified | FirstName | LastName |

@PAT
Scenario: When I enter registration details which are currently being used I am advised of registration success
	Given the following users are setup in the database:
	| UserName      | FirstName | LastName | Password          | SecurityQuestion                    | SecurityAnswer | PasswordResetToken | PasswordResetExpiry | NewEmailAddress | NewEmailAddressToken | NewEmailAddressRequestExpiryDate |
	| user@test.net | Standard  | User     | x12a;pP02icdjshER | What is the name of your first pet? | Mr Miggins     |                    |                     |                 |                      |                                  |
	And I navigate to the website	
	And I maximise the browser window
	And I click register in the title bar
	And I am navigated to the 'Register' page
	And I enter the following registration details:
	| Field            | Value                               |
	| Username         | user@test.net                       |
	| FirstName        | Standard2                           |
	| LastName         | User2                               |
	| SecurityQuestion | What is the name of your first pet? |
	| SecurityAnswer   | Mr Miggins                          |
	| Password         | x12a;pP02icdjshER                   |
	| ConfirmPassword  | x12a;pP02icdjshER                   |
	When I submit my registration details
	Then I am navigated to the 'Register Success' page
	And I have the following users in the system:
	| UserName      | HashStrategy         | Enabled | Approved | EmailVerified | FirstName | LastName |
	| user@test.net | Pbkdf28000Iterations | True    | True     | True          | Standard  | User     |
	And I have the following user logs in the system:
	| Description |
	#And I am notified via email

@PAT @Smoke
Scenario: When I attempt password recovery using a valid account I am notified of success
	Given the following users are setup in the database:
	| UserName      | FirstName | LastName | Password          | SecurityQuestion                    | SecurityAnswer | PasswordResetToken | PasswordResetExpiry | NewEmailAddress | NewEmailAddressToken | NewEmailAddressRequestExpiryDate |
	| user@test.net | Standard  | User     | x12a;pP02icdjshER | What is the name of your first pet? | Mr Miggins     |                    |                     |                 |                      |                                  |
	And I navigate to the website	
	And I maximise the browser window
	And I click the login link in the navigation bar
	And I am navigated to the 'Login' page
	And I click recover password
	And I am navigated to the 'Recover' page
	And I enter the following recover data:
	| Field    | Value        |
	| UserName | user@test.net |
	When I submit the recover form
	Then I am navigated to the 'Recover Success' page
	And I have the following users in the system:
	| UserName      | HashStrategy         | Enabled | Approved | EmailVerified | FirstName | LastName |
	| user@test.net | Pbkdf28000Iterations | True    | True     | True          | Standard  | User     |
	And the user 'user@test.net' has the password reset token set and password reset expiry is at least 14 minutes from now
	And I have the following user logs in the system:
	| Description                            |
	| Password reset link generated and sent |
	#And I receive an email with a reset link

@PAT
Scenario: When I attempt password recovery using an invalid account I am notified of success
	Given I navigate to the website
	And I maximise the browser window
	And I click the login link in the navigation bar
	And I am navigated to the 'Login' page
	And I click recover password
	And I am navigated to the 'Recover' page
	And I enter the following recover data:
	| Field    | Value           |
	| UserName | Bogus@bogus.com |
	When I submit the recover form
	Then I am navigated to the 'Recover Success' page
	And I have the following user logs in the system:
	| Description |

@PAT 
Scenario: When I click on a valid password reset link, I can enter my security information and change my password	
	Given the following users are setup in the database:
	| UserName       | FirstName | LastName | Password          | SecurityQuestion                    | SecurityAnswer | PasswordResetToken                   | PasswordResetExpiry | NewEmailAddress | NewEmailAddressToken | NewEmailAddressRequestExpiryDate |
	| user2@test.net | Standard  | User     | x12a;pP02icdjshER | What is the name of your first pet? | Mr Miggins     | 83ababb4-a0c1-4f2c-8593-32dd40b920d2 | [One day from now]  |                 |                      |                                  |
	And I have 0 entry(ies) in the password history table
	And I navigate to the password reset link with token '83ababb4-a0c1-4f2c-8593-32dd40b920d2'
	And I am navigated to the 'Recover Password' page
	And I maximise the browser window
	And I enter the following recover password data:
	| Field            | Value            |
	| SecurityAnswer   | Mr Miggins       |
	| Password         | NewPassword45678 |
	| Confirm Password | NewPassword45678 |
	When I submit the recover passord form
	Then I am navigated to the 'Recover Password Success' page
	And I have the following users in the system:
	| UserName       | HashStrategy         | Enabled | Approved | EmailVerified | FirstName | LastName |
	| user2@test.net | Pbkdf28000Iterations | True    | True     | True          | Standard  | User     |
	And the password reset token and expiry for user 'user2@test.net' are not set
	And I have the following user logs in the system:
	| Description                  |
	| User Logged On               |
	| Password changed using token |
	And I have 1 entry(ies) in the password history table
	#And I receive an email notifying me of the password change

@PAT @Smoke
Scenario: I can change my password
	Given the following users are setup in the database:
	| UserName       | FirstName | LastName | Password          | SecurityQuestion                    | SecurityAnswer | PasswordResetToken | PasswordResetExpiry | NewEmailAddress | NewEmailAddressToken | NewEmailAddressRequestExpiryDate |
	| user3@test.net | Standard  | User     | x12a;pP02icdjshER | What is the name of your first pet? | Mr Miggins     |                    |                     |                 |                      |                                  |
	And I have 0 entry(ies) in the password history table
	And I make a note of the password and salt for 'user3@test.net'
	And I navigate to the website
	And I maximise the browser window
	And I click the login link in the navigation bar
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | user3@test.net    |
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
	Then I am navigated to the 'Change Password Success' page
	And The password for 'user3@test.net' has changed
	And I have the following user logs in the system:
	| Description      |
	| User Logged On   |
	| Password changed |
	| User Logged Off  |
	And I have 1 entry(ies) in the password history table 
	# And I am logged off
	# And an email is sent

@PAT
Scenario: I can change my email address
	Given the following users are setup in the database:
	| UserName       | FirstName | LastName | Password          | SecurityQuestion                    | SecurityAnswer | PasswordResetToken | PasswordResetExpiry | NewEmailAddress | NewEmailAddressToken | NewEmailAddressRequestExpiryDate |
	| user5@test.net | Standard  | User     | x12a;pP02icdjshER | What is the name of your first pet? | Mr Miggins     |                    |                     |                 |                      |                                  |
	And I navigate to the website
	And I maximise the browser window
	And I click the login link in the navigation bar
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | user5@test.net    |
	| Password | x12a;pP02icdjshER |
	And I click the login button
	And I am navigated to the 'Landing' page
	And I select Admin -> Change Email Address from the menu
	And I am navigated to the 'Change email address' page
	And I enter the following change email address data:
	| Field           | Value             |
	| Password        | x12a;pP02icdjshER |
	| NewEmailAddress | joe@test.net      |
	When I submit the change email address form
	Then I am navigated to the 'Change Email Address Pending' page
	And I have the following users in the system:
	| UserName       | HashStrategy         | Enabled | Approved | EmailVerified | FirstName | LastName |
	| user5@test.net | Pbkdf28000Iterations | True    | True     | True          | Standard  | User     |
	And the user 'user5@test.net' has the new email address token set and new email address expiry is at least 14 minutes from now
	And I have the following user logs in the system:
	| Description                                                                        |
	| User Logged On                                                                     |
	| Change email address request started to change from user5@test.net to joe@test.net |
	# And an email is sent

@PAT
Scenario: When I click on a valid change email address link, I change my email address to a new one
	Given the following users are setup in the database:
	| UserName       | FirstName | LastName | Password          | SecurityQuestion                    | SecurityAnswer | PasswordResetToken | PasswordResetExpiry | NewEmailAddress | NewEmailAddressToken                 | NewEmailAddressRequestExpiryDate |
	| user4@test.net | Standard  | User     | x12a;pP02icdjshER | What is the name of your first pet? | Mr Miggins     |                    |                     | samuel@test.net | B386B07A-FF0C-4B2B-9DAD-7D32CFD5A92F | [One day from now]               |
	When I navigate to the change email address link with token 'B386B07A-FF0C-4B2B-9DAD-7D32CFD5A92F'
	Then I am navigated to the 'Change Email Address Success' page
	And I have the following users in the system:
	| UserName        | HashStrategy         | Enabled | Approved | EmailVerified | FirstName | LastName |
	| samuel@test.net | Pbkdf28000Iterations | True    | True     | True          | Standard  | User     |
	And the user 'samuel@test.net' has the new email address token and expiry cleared
	And I have the following user logs in the system:
	| Description                                                                             |
	| Change email address request confirmed to change from user4@test.net to samuel@test.net |
	#And I receive an email notifying me of the email address change
	#And The new email address receives an email notifying of the change

@PAT
Scenario: I can change my security information
	Given the following users are setup in the database:
	| UserName       | FirstName | LastName | Password          | SecurityQuestion                    | SecurityAnswer | PasswordResetToken | PasswordResetExpiry | NewEmailAddress | NewEmailAddressToken | NewEmailAddressRequestExpiryDate |
	| user3@test.net | Standard  | User     | x12a;pP02icdjshER | What is the name of your first pet? | Mr Miggins     |                    |                     |                 |                      |                                  |
	And I make a note of the security information and salt for user 'user3@test.net'
	And I navigate to the website
	And I maximise the browser window
	And I click the login link in the navigation bar
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | user3@test.net    |
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
	And I have the following users in the system:
	| UserName       | HashStrategy         | Enabled | Approved | EmailVerified | FirstName | LastName | SecurityQuestionLookupItemId      |
	| user3@test.net | Pbkdf28000Iterations | True    | True     | True          | Standard  | User     | What was your childhood nickname? |
	And The security information for 'user3@test.net' has changed
	And I have the following user logs in the system:
	| Description                       |
	| User Logged On                    |
	| User Changed Security Information |
	# And an email is sent

@PAT
Scenario: I can view my user activity log information
	Given the following users are setup in the database:
	| UserName       | FirstName | LastName | Password          | SecurityQuestion                    | SecurityAnswer | PasswordResetToken | PasswordResetExpiry | NewEmailAddress | NewEmailAddressToken | NewEmailAddressRequestExpiryDate |
	| user3@test.net | Standard  | User     | x12a;pP02icdjshER | What is the name of your first pet? | Mr Miggins     |                    |                     |                 |                      |                                  |
	And I navigate to the website
	And I maximise the browser window
	And I click the login link in the navigation bar
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value             |
	| UserName | user3@test.net    |
	| Password | x12a;pP02icdjshER |
	And I click the login button
	And I am navigated to the 'Landing' page
	When I select Admin -> Account Log from the menu
	Then I am navigated to the 'Account Log' page
	And I am shown the following user logs:
	| Description    |
	| User Logged On |
	And I have the following user logs in the system:
	| Description    |
	| User Logged On |