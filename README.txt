Created by John Staveley - Last updated 13/10/2016

This Mvc solution was adapted from the standard MVC4 template in VS 2015, MVC5, .net 4.6.1. Following is how using this solution protects you against the Open Web Application Security Project (OWASP) Top 10 security threats in the world today.

o) SQL Injection: It uses Entity Framework ORM
o) Weak account management: 
	+ Uses claims based auth
	+ Uses the strong hash PBKDF2 with an adaptable number of iterations with the experimental Argon2 hashing routine available, new hashing algorithms can be added as better ones are identified or existing ones have weaknesses identified
	+ Enforces a strong password - Bans weak passwords, enforces minimum password strength, allows special characters, bans too many repeated characters
	+ Has a water tight account management process
	+ Prevents anti-enumeration through well designed messages
	+ Logs account activity which can be checked by the user to see if there is any illicit activity
	+ Emails on key account events and gives anti-phishing advice
	+ Verifies email by sending an email to the specified address
	+ Re-verifies email when requesting a change of email
	+ Prevents brute force of logon
	+ Prevents brute force of registration or password reset through anti-throttling and CAPTCHA
	+ Encryption of security question data using the RijndaelManaged AES 512 encryption algorithm
	+ Increasing wait time on logon failure rather than account lock out
	+ Unit tests for password hashing and authorization attributes
o) XSS: Incorporation of the WPL AntiXSS library to encode all output
o) Insecure direct object references: In user edit page it checks the user is entitled to be there
o) Security misconfiguration: Doesn't turn on anything you don't really need
o) Sensitive data exposure: 
	+ Auto-complete off on registration page
	+ Enforces TLS of all data in production through use of web.configs
	+ Ensures website can only ever be requested over TLS using HSTS header
	+ Turns off verbose errors and trace in production
	+ Removes unnecessary headers which indicate .net framework version
	+ Removes server information disclosure headers from responses
o) Missing Function Level Access Control: Sensitive functions decorated with Authorize and Role attributes. Unit tests to ensure admin functions require the admin role
o) CSRF: Ensures anti-forgery token is used on all Post/Put/Ajax operations by checking through use of a base controller
o) Using components with known vulnerabilities: .Net framework is the latest version and all NuGet packages kept updated
o) Unvalidated redirects and forwards: Covered by RedirectToLocal in MVC4

Other threats it protects against and features:
o) Clickjacking: Disallow site appearing in frame by applying header and disallowing site from being opened in an iFrame
o) Form overposting: Example given of how to avoid this
o) Acceptance tests for key functionality
o) Extensive logging on security violations using Serilog which can be used by an operator to detect an attacker

NB: Runs on SQL Express and IIS Express, requires mail server and recaptcha (optional) set up. See readme.txt in project for more information
