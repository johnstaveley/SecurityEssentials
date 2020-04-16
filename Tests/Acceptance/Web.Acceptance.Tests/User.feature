@CheckForErrors
Feature: User
	In order to users can effectively access the system
	As a user, or admin
	I want to manage user permissions and data

Background:
	Given I delete all cookies from the cache 
	And I clear down the database
	And I have the standard set of lookups
	And the following users are setup in the database:
	| UserName       | Title | FirstName | LastName | Password         | SecurityQuestion                    | SecurityAnswer | IsAdmin | Approved | Enabled | WorkTelephoneNumber | HomeTelephoneNumber | MobileTelephoneNumber | Town | Postcode | SkypeName |
	| user1@test.net | Mr    | User      | One      | zasXX8576jFj123  | What is the name of your first pet? | Beatrix        | false   | true     | true    | 0123                | 0456                | 0789                  | town | postcode | skype     |
	| user2@test.net | Mrs   | User      | Two      | hdfhjreyYURTR123 | What is the name of your first pet? | Pettles        | false   | true     | true    |                     |                     |                       |      |          |           |
	| admin@test.net | Mr    | Admin     | User     | 654dfhjeritjJDFK | What is the name of your first pet? | Beatrix        | true    | true     | true    |                     |                     |                       |      |          |           |

@PAT
Scenario: As an admin I can manage my users
	Given I navigate to the website 
	And I maximise the browser window
	And I click the login link in the navigation bar 
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value            |
	| UserName | admin@test.net   |  
	| Password | 654dfhjeritjJDFK | 
	And I click the login button
	And I am navigated to the 'Landing' page
	And I select Admin -> Manage Users from the menu
	And I am navigated to the 'Manage Users' page
	When I click edit on the user with name 'user1@test.net'
	Then I am navigated to the 'User Edit' page
	And I can edit the username
	And The following user edit information is displayed:
	| Field                 | Value          |
	| UserName              | user1@test.net |
	| Title                 | Mr             |
	| FirstName             | User           |
	| LastName              | One            |
	| HomeTelephoneNumber   | 0456           |
	| WorkTelephoneNumber   | 0123           |
	| MobileTelephoneNumber | 0789           |
	| SkypeName             | skype          |
	| Town                  | town           |
	| Postcode              | postcode       |
	| Approved              | true           |
	| EmailVerified         | true           |
	| Enabled               | true           |

@PAT @Smoke
Scenario: I can change my account information
	Given the following users are setup in the database:
	| UserName       | FirstName | LastName | Password        | SecurityQuestion                    | SecurityAnswer | IsAdmin |
	| user3@test.net | Patient   | User     | zasXX8576jFj123 | What is the name of your first pet? | Beatrix        | false   |
	And the following user roles are setup in the system for user 'user3@test.net'
	| Description |
	Given I navigate to the website
	And I maximise the browser window
	And I click the login link in the navigation bar
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value           |
	| UserName | user3@test.net  |
	| Password | zasXX8576jFj123 |
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
	Then A confirmation message 'Your account information has been saved' is shown
	And I cannot edit the username
	And I have the following user logs in the system:
	| Description    |
	| User Logged On |
	And I have the following users in the system:
	| UserName       | HashStrategy          | Enabled | Approved | EmailVerified | FirstName | LastName | TelNoWork  | TelNoHome  | TelNoMobile | Town  | PostCode | SkypeName |
	| admin@test.net | Pbkdf210001Iterations | True    | True     | True          | Admin     | User     |            |            |             |       |          |           |
	| user1@test.net | Pbkdf210001Iterations | True    | True     | True          | User     | One | 0123       | 0456       | 0789        | town  | postcode | skype     |
	| user2@test.net | Pbkdf210001Iterations | True    | True     | True          | User     | Two     |            |            |             |       |          |           |
	| user3@test.net | Pbkdf210001Iterations | True    | True     | True          | Sarah     | Page     | 0123456789 | 0987654321 | 0778412457  | Leeds | LS10 1EF | SarahPage |

Scenario: I can add the administrator privilege
	Given I navigate to the website
	And I maximise the browser window
	And I click the login link in the navigation bar
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value            |
	| UserName | admin@test.net   |
	| Password | 654dfhjeritjJDFK |
	And I click the login button
	And I am navigated to the 'Landing' page
	And I select Admin -> Manage Users from the menu
	And I am navigated to the 'Manage Users' page
	And I click edit on the user with name 'user1@test.net'
	And I am navigated to the 'User Edit' page
	And The text indicating the user is a system administrator is not displayed
	And I click Make Administrator Privilege
	And I am navigated to the 'Make Admin' page
	And I am shown the following make admin details:
	| Field    | Value          |
	| UserName | user1@test.net |
	When I click confirm make admin
	Then I am navigated to the 'User Edit' page
	And The following user edit information is displayed:
	| Field    | Value          |
	| UserName | user1@test.net |
	And The text indicating the user is a system administrator is displayed
	And I have the following user logs in the system:
    | Description                                    |
    | User Logged On                                 |
    | User was made a system admin by admin@test.net |

Scenario: I can remove the administrator privilege
	Given the following users are setup in the database:
	| UserName       | FirstName | LastName | Password        | SecurityQuestion                    | SecurityAnswer | IsAdmin | Approved | Enabled |
	| user3@test.net | User      | Three    | zasXX8576jFj123 | What is the name of your first pet? | Beatrix        | true    | true     | true    |
	And I navigate to the website
	And I maximise the browser window
	And I click the login link in the navigation bar
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value            |
	| UserName | admin@test.net   |
	| Password | 654dfhjeritjJDFK |
	And I click the login button
	And I am navigated to the 'Landing' page
	And I select Admin -> Manage Users from the menu
	And I am navigated to the 'Manage Users' page
	And I click edit on the user with name 'user3@test.net'
	And I am navigated to the 'User Edit' page
	And The text indicating the user is a system administrator is displayed
	And I click Remove Administrator Privilege
	And I am navigated to the 'Remove Admin' page
	And I am shown the following remove admin details:
	| Field    | Value          |
	| UserName | user3@test.net |
	When I click confirm remove admin
	Then I am navigated to the 'User Edit' page
	And I can edit the username
	And The following user edit information is displayed:
	| Field    | Value          |
	| UserName | user3@test.net |
	And The text indicating the user is a system administrator is not displayed
	And I have the following user logs in the system:
    | Description                                                 |
    | User Logged On                                              |
    | User had administrator privileges removed by admin@test.net |

Scenario: As an admin I can reset a user's password
	Given I navigate to the website
	And I maximise the browser window
	And I have 0 entry(ies) in the password history table
	And I click the login link in the navigation bar
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value            |
	| UserName | admin@test.net    |
	| Password | 654dfhjeritjJDFK |
	And I click the login button
	And I am navigated to the 'Landing' page
	And I select Admin -> Manage Users from the menu
	And I am navigated to the 'Manage Users' page
	And I click edit on the user with name 'user1@test.net'
	And I am navigated to the 'User Edit' page
	And I click Reset Password
	And I am navigated to the 'Reset Password' page
	And I am shown the following reset password details:
	| Field    | Value          |
	| UserName | user1@test.net |
	When I click confirm reset password
	Then I am navigated to the 'User Edit' page
	And The following user edit information is displayed:
	| Field    | Value          |
	| UserName | user1@test.net |
	And I have 1 entry(ies) in the password history table
	And the user 'user1@test.net' has the password expiry date set
	And I have the following user logs in the system:
    | Description                                                  |
    | User Logged On                                               |
    | User had password reset sent out via email by admin@test.net |
	# And an email was sent indicating the new password 

Scenario: As an admin I can delete a user
	Given I navigate to the website
	And I maximise the browser window
	And I click the login link in the navigation bar
	And I am navigated to the 'Login' page
	And I enter the following login data:
	| Field    | Value            |
	| UserName | admin@test.net   |
	| Password | 654dfhjeritjJDFK |
	And I click the login button
	And I am navigated to the 'Landing' page
	And I select Admin -> Manage Users from the menu
	And I am navigated to the 'Manage Users' page
	And I am shown the following users:
	| UserName       | FullName   | TelNoMobile | Enabled | Approved |
	| admin@test.net | Admin User |             | True    | True     |
	| user2@test.net | User Two   |             | True    | True     |
	| user1@test.net | User One   | 0789        | True    | True     |
	And I click edit on the user with name 'user1@test.net'
	And I am navigated to the 'User Edit' page
	And I click Delete User
	And I am navigated to the 'Delete User' page
	And I am shown the following delete user details:
	| Field    | Value          |
	| UserName | user1@test.net |
	When I click confirm delete user
	Then I am navigated to the 'manage users' page
	And I am shown the following users:
	| UserName       | FullName   | TelNoMobile | Enabled | Approved |
	| admin@test.net | Admin User |             | True    | True     |
	| user2@test.net | User Two   |             | True    | True     |
