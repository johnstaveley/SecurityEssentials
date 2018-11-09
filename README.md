# Security Essentials MVC Project Template

### Created by John Staveley - Last updated 09/11/2018

## Introduction
This Mvc solution was adapted from the standard MVC4 template in VS 2017, MVC5, .net 4.6.1. Following is how using this solution protects you against the Open Web Application Security Project (OWASP) Top 10 security threats in the world today.

## Security Enhancements
This list is based on the OWASP Top 10 2013
* SQL Injection: It uses Entity Framework ORM
* Weak account management: 
	+ Uses claims based auth
	+ Uses the strong hash PBKDF2 with an adaptable number of iterations with the experimental Argon2 hashing routine available, new hashing algorithms can be added as better ones are identified or existing ones have weaknesses identified
	+ Enforces a strong password - Bans weak passwords, enforces minimum password strength, allows special characters, bans too many repeated characters, bans use of previous N passwords
	+ Has a water tight account management process
	+ Prevents anti-enumeration through well designed system messages
	+ Logs account activity which can be checked by the user to see if there is any illicit activity
	+ Emails on key account events and gives anti-phishing advice
	+ Verifies email by sending an email to the specified address
	+ Re-verifies email when requesting a change of email
	+ Prevents brute force of logon
	+ Prevents brute force of registration or password reset through anti-throttling and CAPTCHA (optional)
	+ Encryption of security question data using the RijndaelManaged AES 512 encryption algorithm
	+ Increasing wait time on logon failure rather than account lock out
	+ Unit tests for password hashing and authorization attributes
* XSS:
	+ Incorporation of the WPL AntiXSS library to encode all output
	+ Enforce the location of the scripts the browser can run using a content security policy header
	+ Enables browser's anti-xss capabilities by sending the XSS-Protection header
* Insecure direct object references: In user edit page it checks the user is entitled to be there
* Security misconfiguration: Doesn't turn on anything you don't really need
* Sensitive data exposure: 
	+ Auto-complete off on registration page
	+ Enforces TLS of all data in production through use of web.configs
	+ Ensures website can only ever be requested over TLS using HSTS header
	+ Turns off verbose errors and trace in production
	+ Removes unnecessary headers which indicate .net framework version
	+ Removes server information disclosure headers from responses
* Missing Function Level Access Control: Sensitive functions decorated with Authorize and Role attributes. Unit tests to ensure admin functions require the admin role
* CSRF: Ensures anti-forgery token is used on all Post/Put/Ajax operations by checking through use of a base controller
    + Unit tests to ensure all state changing mvc or web api methods validate an anti forgery token
* Using components with known vulnerabilities: .Net framework is the latest version and all NuGet packages kept updated
* Unvalidated redirects and forwards: Covered by RedirectToLocal in MVC4

Other threats it protects against and features:

* Clickjacking: Disallow site appearing in frame by applying header and disallowing site from being opened in an iFrame
* Form overposting: Example given of how to avoid this
* Acceptance tests for key functionality
* Forces user to change their password if their password has expired
* Professionally pentested

##OWASP Top 10 2017
The Top 10 list of vulnerabilities has been updated in 2017. The list is much the same with the addition of:
* A4 - XML External Entities (XXE). This application does not parse XML documents and so would not be affected
* A8 - Insecure Deserialisation. Only affects Java applications
* A10 - Insufficient Logging and Monitoring. Extensive logging on security violations using Serilog which can be used by an operator to detect an attacker. Covers Account management, XSS, Form overposting, CSRF, unvalidated requests and forwards, content security policy and http public key pinning violations

***Note:** Runs on SQL Express and IIS Express, requires mail server and recaptcha (optional) set up. See readme.txt in project for more information*