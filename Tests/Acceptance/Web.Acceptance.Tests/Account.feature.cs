﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.1.0.0
//      SpecFlow Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SecurityEssentials.Acceptance.Tests
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.1.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class AccountFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Account.feature"
#line hidden
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner(null, 0);
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Account", "\tIn order to securely access the application\r\n\tAs a user\r\n\tI want to be manage my" +
                    " account", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
        public virtual void TestInitialize()
        {
            if (((testRunner.FeatureContext != null) 
                        && (testRunner.FeatureContext.FeatureInfo.Title != "Account")))
            {
                SecurityEssentials.Acceptance.Tests.AccountFeature.FeatureSetup(null);
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 6
#line 7
 testRunner.Given("I delete all cookies from the cache", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Home Page Loads")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Account")]
        public virtual void HomePageLoads()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Home Page Loads", ((string[])(null)));
#line 14
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 15
 testRunner.Given("I navigate to the website", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 16
 testRunner.When("I am taken to the homepage", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("When I enter correct login details I am taken to the landing page")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Account")]
        public virtual void WhenIEnterCorrectLoginDetailsIAmTakenToTheLandingPage()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("When I enter correct login details I am taken to the landing page", ((string[])(null)));
#line 18
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 19
 testRunner.Given("I navigate to the website", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 20
 testRunner.And("I am taken to the homepage", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 21
 testRunner.And("I click login", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 22
 testRunner.And("I am navigated to the \'login\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table1.AddRow(new string[] {
                        "UserName",
                        "user@user.com"});
            table1.AddRow(new string[] {
                        "Password",
                        "x12a;pP02icdjshER"});
#line 23
 testRunner.And("I enter the following login data:", ((string)(null)), table1, "And ");
#line 27
 testRunner.When("I click the login button", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 28
 testRunner.Then("I am navigated to the \'Landing\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 29
 testRunner.Then("the following last activity message is shown: \'The last actvity logged against yo" +
                    "ur account was\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("When I enter incorrect login details then a warning is displayed")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Account")]
        public virtual void WhenIEnterIncorrectLoginDetailsThenAWarningIsDisplayed()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("When I enter incorrect login details then a warning is displayed", ((string[])(null)));
#line 31
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 32
 testRunner.Given("I navigate to the website", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 33
 testRunner.And("I am taken to the homepage", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 34
 testRunner.And("I click login", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 35
 testRunner.And("I am navigated to the \'login\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table2.AddRow(new string[] {
                        "UserName",
                        "user@user.com"});
            table2.AddRow(new string[] {
                        "Password",
                        "y12a;pP02icdjshET"});
#line 36
 testRunner.And("I enter the following login data:", ((string)(null)), table2, "And ");
#line 40
 testRunner.When("I click the login button", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field"});
            table3.AddRow(new string[] {
                        "Invalid credentials or the account is locked"});
#line 41
 testRunner.Then("The following errors are displayed:", ((string)(null)), table3, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("When I enter valid registration details I can register a new user")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Account")]
        public virtual void WhenIEnterValidRegistrationDetailsICanRegisterANewUser()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("When I enter valid registration details I can register a new user", ((string[])(null)));
#line 45
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 46
 testRunner.Given("I navigate to the website", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 47
 testRunner.And("I click register in the title bar", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 48
 testRunner.And("I am navigated to the \'Register\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table4.AddRow(new string[] {
                        "Username",
                        "test@test.com"});
            table4.AddRow(new string[] {
                        "FirstName",
                        "Tester"});
            table4.AddRow(new string[] {
                        "LastName",
                        "Tester"});
            table4.AddRow(new string[] {
                        "SecurityQuestion",
                        "What is your mother\'s maiden name?"});
            table4.AddRow(new string[] {
                        "SecurityAnswer",
                        "Bloggs"});
            table4.AddRow(new string[] {
                        "Password",
                        "Test456789"});
            table4.AddRow(new string[] {
                        "ConfirmPassword",
                        "Test456789"});
#line 49
 testRunner.And("I enter the following registration details:", ((string)(null)), table4, "And ");
#line 58
 testRunner.When("I submit my registration details", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 59
 testRunner.Then("I am navigated to the \'Register Success\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("When I enter registration details which are currently being used I am advised of " +
            "registration success")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Account")]
        public virtual void WhenIEnterRegistrationDetailsWhichAreCurrentlyBeingUsedIAmAdvisedOfRegistrationSuccess()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("When I enter registration details which are currently being used I am advised of " +
                    "registration success", ((string[])(null)));
#line 62
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 63
 testRunner.Given("I navigate to the website", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 64
 testRunner.And("I click register in the title bar", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 65
 testRunner.And("I am navigated to the \'Register\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table5.AddRow(new string[] {
                        "Username",
                        "user@user.com"});
            table5.AddRow(new string[] {
                        "FirstName",
                        "Standard"});
            table5.AddRow(new string[] {
                        "LastName",
                        "User"});
            table5.AddRow(new string[] {
                        "SecurityQuestion",
                        "What is the name of your first pet?"});
            table5.AddRow(new string[] {
                        "SecurityAnswer",
                        "Mr Miggins"});
            table5.AddRow(new string[] {
                        "Password",
                        "x12a;pP02icdjshER"});
            table5.AddRow(new string[] {
                        "ConfirmPassword",
                        "x12a;pP02icdjshER"});
#line 66
 testRunner.And("I enter the following registration details:", ((string)(null)), table5, "And ");
#line 75
 testRunner.When("I submit my registration details", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 76
 testRunner.Then("I am navigated to the \'Register Success\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("When I attempt password recovery using a valid account I am notified of success")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Account")]
        public virtual void WhenIAttemptPasswordRecoveryUsingAValidAccountIAmNotifiedOfSuccess()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("When I attempt password recovery using a valid account I am notified of success", ((string[])(null)));
#line 79
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 80
 testRunner.Given("I navigate to the website", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 81
 testRunner.And("I click login", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 82
 testRunner.And("I am navigated to the \'Login\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 83
 testRunner.And("I click recover password", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 84
 testRunner.And("I am navigated to the \'Recover\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table6.AddRow(new string[] {
                        "UserName",
                        "Test@test.com"});
#line 85
 testRunner.And("I enter the following recover data:", ((string)(null)), table6, "And ");
#line 88
 testRunner.When("I submit the recover form", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 89
 testRunner.Then("I am navigated to the \'Recover Success\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("When I attempt password recovery using an invalid account I am notified of succes" +
            "s")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Account")]
        public virtual void WhenIAttemptPasswordRecoveryUsingAnInvalidAccountIAmNotifiedOfSuccess()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("When I attempt password recovery using an invalid account I am notified of succes" +
                    "s", ((string[])(null)));
#line 92
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 93
 testRunner.Given("I navigate to the website", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 94
 testRunner.And("I click login", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 95
 testRunner.And("I am navigated to the \'Login\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 96
 testRunner.And("I click recover password", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 97
 testRunner.And("I am navigated to the \'Recover\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table7.AddRow(new string[] {
                        "UserName",
                        "Bogus@bogus.com"});
#line 98
 testRunner.And("I enter the following recover data:", ((string)(null)), table7, "And ");
#line 101
 testRunner.When("I submit the recover form", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 102
 testRunner.Then("I am navigated to the \'Recover Success\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("When I click on a valid password reset link, I can enter my security information " +
            "and change my password")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Account")]
        public virtual void WhenIClickOnAValidPasswordResetLinkICanEnterMySecurityInformationAndChangeMyPassword()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("When I click on a valid password reset link, I can enter my security information " +
                    "and change my password", ((string[])(null)));
#line 104
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 105
 testRunner.Given("I navigate to the password reset link with token \'83ababb4-a0c1-4f2c-8593-32dd40b" +
                    "920d2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 106
 testRunner.And("I am navigated to the \'Recover Password\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table8.AddRow(new string[] {
                        "SecurityAnswer",
                        "Mr Miggins"});
            table8.AddRow(new string[] {
                        "Password",
                        "NewPassword45678"});
            table8.AddRow(new string[] {
                        "Confirm Password",
                        "NewPassword45678"});
#line 107
 testRunner.And("I enter the following recover password data:", ((string)(null)), table8, "And ");
#line 112
 testRunner.When("I submit the recover passord form", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 113
 testRunner.Then("I am navigated to the \'Recover Password Success\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("I can change my account information")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Account")]
        public virtual void ICanChangeMyAccountInformation()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("I can change my account information", ((string[])(null)));
#line 117
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 118
 testRunner.Given("I navigate to the website", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 119
 testRunner.And("I click login", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 120
 testRunner.And("I am navigated to the \'Login\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table9 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table9.AddRow(new string[] {
                        "UserName",
                        "user@user.com"});
            table9.AddRow(new string[] {
                        "Password",
                        "x12a;pP02icdjshER"});
#line 121
 testRunner.And("I enter the following login data:", ((string)(null)), table9, "And ");
#line 125
 testRunner.And("I click the login button", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 126
 testRunner.And("I am navigated to the \'Landing\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 127
 testRunner.And("I select Admin -> Manage Account from the menu", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 128
 testRunner.And("I am navigated to the \'User Edit\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table10 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table10.AddRow(new string[] {
                        "Title",
                        "Mrs"});
            table10.AddRow(new string[] {
                        "FirstName",
                        "Sarah"});
            table10.AddRow(new string[] {
                        "LastName",
                        "Page"});
            table10.AddRow(new string[] {
                        "WorkTelephoneNumber",
                        "0123456789"});
            table10.AddRow(new string[] {
                        "HomeTelephoneNumber",
                        "0987654321"});
            table10.AddRow(new string[] {
                        "MobileTelephoneNumber",
                        "0778412457"});
            table10.AddRow(new string[] {
                        "Town",
                        "Leeds"});
            table10.AddRow(new string[] {
                        "PostCode",
                        "LS10 1EF"});
            table10.AddRow(new string[] {
                        "SkypeName",
                        "SarahPage"});
#line 129
 testRunner.And("I enter the following change account information data:", ((string)(null)), table10, "And ");
#line 140
 testRunner.When("I submit the manage account form", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 141
 testRunner.Then("A confirmation message \'Your account information has been changed\' is shown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("I can change my password")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Account")]
        public virtual void ICanChangeMyPassword()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("I can change my password", ((string[])(null)));
#line 143
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 144
 testRunner.Given("I navigate to the website", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 145
 testRunner.And("I click login", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 146
 testRunner.And("I am navigated to the \'Login\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table11 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table11.AddRow(new string[] {
                        "UserName",
                        "user3@user.com"});
            table11.AddRow(new string[] {
                        "Password",
                        "x12a;pP02icdjshER"});
#line 147
 testRunner.And("I enter the following login data:", ((string)(null)), table11, "And ");
#line 151
 testRunner.And("I click the login button", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 152
 testRunner.And("I am navigated to the \'Landing\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 153
 testRunner.And("I select Admin -> Change Password from the menu", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 154
 testRunner.And("I am navigated to the \'Change Password\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table12 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table12.AddRow(new string[] {
                        "CurrentPassword",
                        "x12a;pP02icdjshER"});
            table12.AddRow(new string[] {
                        "NewPassword",
                        "NewPassword45678"});
            table12.AddRow(new string[] {
                        "ConfirmNewPassword",
                        "NewPassword45678"});
#line 155
 testRunner.And("I enter the following change password data:", ((string)(null)), table12, "And ");
#line 160
 testRunner.When("I submit the change password form", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 161
 testRunner.Then("A confirmation message \'Your password has been changed.\' is shown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("I can change my security information")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Account")]
        public virtual void ICanChangeMySecurityInformation()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("I can change my security information", ((string[])(null)));
#line 165
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 166
 testRunner.Given("I navigate to the website", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 167
 testRunner.And("I click login", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 168
 testRunner.And("I am navigated to the \'Login\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table13 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table13.AddRow(new string[] {
                        "UserName",
                        "user3@user.com"});
            table13.AddRow(new string[] {
                        "Password",
                        "x12a;pP02icdjshER"});
#line 169
 testRunner.And("I enter the following login data:", ((string)(null)), table13, "And ");
#line 173
 testRunner.And("I click the login button", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 174
 testRunner.And("I am navigated to the \'Landing\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 175
 testRunner.And("I select Admin -> Change Security Information from the menu", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 176
 testRunner.And("I am navigated to the \'Change security Information\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table14 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table14.AddRow(new string[] {
                        "Password",
                        "x12a;pP02icdjshER"});
            table14.AddRow(new string[] {
                        "SecurityQuestion",
                        "What was your childhood nickname?"});
            table14.AddRow(new string[] {
                        "SecurityAnswer",
                        "Adelweiss"});
            table14.AddRow(new string[] {
                        "SecurityAnswerConfirm",
                        "Adelweiss"});
#line 177
 testRunner.And("I enter the following change security information data:", ((string)(null)), table14, "And ");
#line 183
 testRunner.When("I submit the change security information form", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 184
 testRunner.Then("I am navigated to the \'Change Security Information Success\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("The application will prevent a brute force login attempt")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Account")]
        public virtual void TheApplicationWillPreventABruteForceLoginAttempt()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("The application will prevent a brute force login attempt", ((string[])(null)));
#line 188
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 189
 testRunner.Given("I navigate to the website", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 190
 testRunner.And("I am taken to the homepage", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 191
 testRunner.And("I click login", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 192
 testRunner.And("I am navigated to the \'login\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table15 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table15.AddRow(new string[] {
                        "UserName",
                        "attempt1@user.com"});
            table15.AddRow(new string[] {
                        "Password",
                        "rhubarb"});
#line 193
 testRunner.And("I enter the following login data:", ((string)(null)), table15, "And ");
#line 197
 testRunner.And("I click the login button", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 198
 testRunner.And("I navigate to the website", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 199
 testRunner.And("I am taken to the homepage", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 200
 testRunner.And("I click login", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 201
 testRunner.And("I am navigated to the \'login\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table16 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table16.AddRow(new string[] {
                        "UserName",
                        "attempt2@user.com"});
            table16.AddRow(new string[] {
                        "Password",
                        "rhubarb"});
#line 202
 testRunner.And("I enter the following login data:", ((string)(null)), table16, "And ");
#line 206
 testRunner.And("I click the login button", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 207
 testRunner.And("I navigate to the website", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 208
 testRunner.And("I am taken to the homepage", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 209
 testRunner.And("I click login", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 210
 testRunner.And("I am navigated to the \'login\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table17 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table17.AddRow(new string[] {
                        "UserName",
                        "attempt3@user.com"});
            table17.AddRow(new string[] {
                        "Password",
                        "rhubarb"});
#line 211
 testRunner.And("I enter the following login data:", ((string)(null)), table17, "And ");
#line 215
 testRunner.And("I click the login button", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 216
 testRunner.And("I navigate to the website", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 217
 testRunner.And("I am taken to the homepage", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 218
 testRunner.And("I click login", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 219
 testRunner.And("I am navigated to the \'login\' page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table18 = new TechTalk.SpecFlow.Table(new string[] {
                        "Field",
                        "Value"});
            table18.AddRow(new string[] {
                        "UserName",
                        "attempt4@user.com"});
            table18.AddRow(new string[] {
                        "Password",
                        "rhubarb"});
#line 220
 testRunner.And("I enter the following login data:", ((string)(null)), table18, "And ");
#line 224
 testRunner.When("I click the login button", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 225
 testRunner.Then("an error message is shown \'You have performed this action more than 3 times in th" +
                    "e last 60 seconds.\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion