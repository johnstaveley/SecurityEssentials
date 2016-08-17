Feature: Security
	In order to avoid information disclosure
	As a pen tester
	I want to be sure the application has the correct security settings and behaviour

Scenario: The web application will return the correct security headers
	When I call http get on the website
	Then the response headers will contain:
	| Key                    | Value         |
	| X-Frame-Options        | Deny          |
	| X-Content-Type-Options | nosniff       |
	| X-XSS-Protection       | 1; mode=block |
	And the response headers will not contain:
	| Key                 |
	| X-AspNet-Version    |
	| X-AspNetMvc-Version |
	| Server              |

 
	