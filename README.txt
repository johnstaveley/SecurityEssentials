Created by John Staveley - 29/01/2015
This Mvc solution was adapted from the standard MVC4 template in VS 2013 with the following security features:

Template built on the latest technology - MVC 5, EF 6, Bootstrap 3, Claims based authentication and authorisation, Kendo UI
No components with know vulnerabilities, built on the latest version of .Net and up to date NuGet packages
Secure password storage - Stores passwords with a cryptographically secure Salt and Hash using the PBKDF2 algorithm with an adaptable number of iterations
Passwords harder to crack - Enforces a strong password policy and allows special characters
Passwords harder to guess - Compares password entered with a database of frequently used (and cracked) passwords and no personal information is allowed to be reused in the password
Account management - Prevents account enumeration on register and reset password forms
Account management - Each user can see a summary of their most recent account activity to check for any illicit activity
Account Management - Protects sensitive functions against brute force attacks by throttling web requests and using CAPTCHA
Account Management - Prevents overposting on all forms
Account Management - All account activity is logged which is viewable by the user, user is reminded of last logon time when they logon
Controllers and AJAX are protected against the CSRF attack by using an AntiForgeryToken
Prevents the site from being opened inside an iFrame (anti-clickjacking)
Prevents the site in production from ever being requested over http
Removes server information disclosure headers from responses
Unit tests for password hashing and authorization attributes
Auto-Complete switched off on registration form to protect sensitive data
Increasing wait time on logon failure

NB: Runs on SQL Express and IIS Express, requires mail server and recaptcha set up. See readme.txt in project for more information