# Security Essentials MVC Project Template

### Created by <a href="https://twitter.com/intent/follow?original_referer=https%3A%2F%2Fabout.twitter.com%2Fresources%2Fbuttons&amp;region=follow_link&amp;screen_name=johnstaveley&amp;tw_p=followbutton&amp;variant=2.0">@johnstaveley</a> - Last updated 12/12/2020

## Introduction
This Mvc solution was adapted from the standard MVC template in VS 2019, MVC5, .net 4.8 and Bootstrap 4. Following is how using this solution protects you against the Open Web Application Security Project (OWASP) Top 10 security threats in the world today.

## Security Enhancements
This list is based on the OWASP Top 10 2013/2017
* SQL Injection: It uses Entity Framework ORM
* Weak account management: 
	+ Uses claims based auth
	+ Uses the strong hash PBKDF2 with an adaptable number of iterations, also the Argon2 hashing routine is available, new hashing algorithms can be added as better ones are identified or existing ones have weaknesses identified
	+ Enforces a strong password - Checks for previously pwned passwords, bans weak passwords, enforces minimum password strength, allows special characters, bans too many repeated characters, bans use of previous N passwords
	+ Has a water tight account management process
	+ Prevents anti-enumeration through well designed system messages
	+ Logs account activity which can be checked by the user to see if there is any illicit activity
	+ Emails on key account events and gives anti-phishing advice
	+ Verifies user's email address by sending an email to the specified address
	+ Re-verifies user's email address when requesting a change of email
	+ Prevents brute force of logon, registration or password reset through anti-throttling and CAPTCHA (optional)
	+ Encryption of security question data using the RijndaelManaged AES 512 encryption algorithm
	+ Increasing wait time on logon failure rather than account lock out
	+ Unit tests for password hashing and authorization attributes
* XSS:
	+ Incorporation of the WPL AntiXSS library to encode all output
	+ Enforce the location of the scripts/assets/actions the browser can run using a content security policy header
	+ Feature Policy disallows features used in the browser such as camera, sync-xhr etc
	+ Enables browser's anti-xss capabilities by sending the XSS-Protection header
* Insecure direct object references: In user edit page it checks the user is entitled to be there
* Security misconfiguration:
	+ Base Application keeps as much switched off as possible
    + AzSK Arm template checker ensures unwanted features are switched off and platform is secure as possible
* Sensitive data exposure: 
	+ Auto-complete off on registration page
	+ Enforces TLS of all data in production through use of web.configs
	+ Ensures website can only ever be requested over TLS using HSTS header
	+ Turns off verbose errors and trace in production
	+ Removes unnecessary headers which indicate .net framework version
	+ Removes server information disclosure headers from responses
	+ Do not publish package.json file which IIS will do by default
	+ NoCache header applied to any controller which publishes user sensitive data, unit tests for same
* Missing Function Level Access Control: Sensitive functions decorated with Authorize and Role attributes. Unit tests to ensure admin functions require the admin role
* CSRF: Ensures anti-forgery token is used on all Post/Put/Ajax operations by checking through use of a base controller
    + Unit tests to ensure all state changing mvc or web api methods validate an anti forgery token
	+ Uses SameSite Lax attribute on session cookie
* Using components with known vulnerabilities: 
    + .Net framework is the latest version and all NuGet packages kept updated
	+ Binaries are scanned using OWASP Dependency checker, uses a supression file for mitigated vulnerabilities
	+ All components are scanned using Whitesource bolt on checkin for finding open source vulnerabilities
* Unvalidated redirects and forwards: Covered by RedirectToLocal in MVC4

Other threats it protects against and features:

* Clickjacking: Disallow site appearing in frame by applying header and disallowing site from being opened in an iFrame
* Form overposting: Example given of how to avoid this
* Acceptance tests for key functionality
* Forces user to change their password if their password has expired
* FxCop security rules enabled to check the code at build for security flaws https://docs.microsoft.com/en-us/visualstudio/code-quality/security-warnings
* Professionally pentested
* Additional Http Headers such as Permissions-Policy, X-Frame-Options, Referrer-Policy, Expect-CT, X-Content-Type-Options

## OWASP Top 10 2017
The Top 10 list of vulnerabilities has been updated in 2017. The list is much the same with the addition of:
* A4 - XML External Entities (XXE). This application does not parse XML documents and so would not be affected. However if it did this only applies to .Net 4.5 and earlier.
* A8 - Insecure Deserialisation. Only affects Java applications
* A10 - Insufficient Logging and Monitoring. Extensive logging on security violations using Serilog which can be used by an operator to detect an attacker. Covers Account management, XSS, Form overposting, CSRF, unvalidated requests and forwards, content security policy and http public key pinning violations

***Note:** Runs on SQL Express and IIS Express, requires mail server and recaptcha (optional) set up. See readme.txt in project for more information*

## How the incorporated build process ensures you remain secure over time.
This solution comes with azure-pipelines.yml build and test script. Running this in Azure Devops ensures the following:
* 1. SQL Injection - Check OWASP Zap report for vulnerabilities
* 2. Account management process - Partially checked using Unit tests and Acceptance tests
* 3. XSS - Check OWASP Zap report for vulnerabilities
* 4. Insecure direct object references - Partially checked using Unit tests
* 5. Security misconfiguration - AzSK Arm template checker ensures the platform is secure and default features are switched off, Check OWASP Zap report for vulnerabilities
* 6. NoCache checked using Unit tests, Check OWASP Zap report for vulnerabilities
* 7. Missing file level access control - Partially checked using Unit tests
* 8. CSRF - Partially checked using Unit tests, Check OWASP Zap report for vulnerabilities
* 9. Vulnerable dependencies - Checked using <a href="https://www.owasp.org/index.php/OWASP_Dependency_Check" target="_blank">OWASP Dependency checker</a> and <a href="https://bolt.whitesourcesoftware.com/" target="_blank">Whitesource bolt</a>, check the reports in Azure Devops, stored as a build artefact for OWASP DC and in the Whitesource bolt tab under pipelines. These can also be set to fail the build, if required</li>
* 10. Unvalidated redirects and forwards - Partially checked using Unit tests, Check OWASP Zap report for vulnerabilities

## Platform features
* Completely automated deployment using azure pipelines, See Developer readme for more details
* Setup DNS and WAF settings in cloudflare
* Enforce Cloudflare rules: Always use https, Min tls level 1.2, allow tls 1.3, set ssl level to full, allow http2
* PaaS: Restrict access to only from Cloudflare, enforce https, lock down to virtual network, auto scale PaaS on heavy load, enforce TLS 1.2, disable ftp
* Prove Azure are upgrading their PaaS platform over time
* Storage - lock down to virtual network, enable soft delete for 7 days
* Web Test - Prove the website is available from at least two world locations, alert admins if not
* SQL Database - Backup databases at regular intervals, encrypt database at rest, audit changes to database settings, perform regular vulnerability checking, restrict ip address access, send automated vulnerability checks to admins
* Alerts - Alert admins on security warnings
* Logins - Log errors and metrics in application insights

[![Build Status](https://johnstaveley.visualstudio.com/Security%20Essentials/_apis/build/status/johnstaveley.SecurityEssentials?branchName=master)](https://johnstaveley.visualstudio.com/Security%20Essentials/_build/latest?definitionId=6&branchName=master)