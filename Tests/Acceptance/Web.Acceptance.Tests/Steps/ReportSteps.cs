using System.Configuration;
using System.Globalization;
using System.Net;
using NUnit.Framework;
using RestSharp;
using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Model;
using SecurityEssentials.Acceptance.Tests.Utility;
using SecurityEssentials.Model;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
	[Binding]
	public class ReportSteps
	{

		[Given(@"I have a content security policy violation with details:")]
		public void GivenIHaveAContentSecurityPolicyViolationWithDetails(Table table)
		{
			var cspInstance = table.CreateInstance<CspModel>();
			var cspReport = new CspReport
				{
					BlockedUri = cspInstance.BlockedUri,
					DocumentUri = cspInstance.DocumentUri,
					LineNumber = cspInstance.LineNumber,
					OriginalPolicy = cspInstance.OriginalPolicy,
					Referrer = cspInstance.Referrer,
					ScriptSample = cspInstance.ScriptSample,
					SourceFile = cspInstance.SourceFile,
					ViolatedDirective = cspInstance.ViolatedDirective
				};
			ScenarioContext.Current.SetCspReport(cspReport);
		}
		[Given(@"I have a http public key pinning violation with details:")]
		public void GivenIHaveAHttpPublicKeyPinningViolationWithDetails(Table table)
		{
			var hpkpModel = table.CreateInstance<HpkpModel>();
			var hpkpReport = new HpkpReport
			{
				DateTime = hpkpModel.DateTime.ToString(CultureInfo.CurrentCulture),
				ExpirationDate = hpkpModel.ExpirationDate.ToString(CultureInfo.CurrentCulture),
				HostName = hpkpModel.HostName,
				IncludeSubDomains = hpkpModel.IncludeSubDomains.ToString(),
				KnownPins = hpkpModel.KnownPinsDelimited.Split(','),
				NotedHostName = hpkpModel.NotedHostName,
				Port = hpkpModel.Port,
				ServedCertificateChain = hpkpModel.ServedCertificateChainDelimited.Split(','),
				ValidatedCertificateChain = hpkpModel.ValidatedCertificateChainDelimited.Split(',')
			};
			ScenarioContext.Current.SetHpkpReport(hpkpReport);
		}

		[When(@"I post the content security policy violation to the website")]
		public void WhenIPostTheContentSecurityPolicyViolationToTheWebsite()
		{
			var cspReport = ScenarioContext.Current.GetCspReport();
			var url = $"{ConfigurationManager.AppSettings["WebServerUrl"]}Security/CspReporting/";
			var response = HttpWeb.PostJsonStream(url, new CspHolder { CspReport = cspReport });
			Assert.That(response.ResponseStatus, Is.EqualTo(ResponseStatus.Completed));
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}
		[When(@"I post the http public key pinning violation to the website")]
		public void WhenIPostTheHttpPublicKeyPinningViolationToTheWebsite()
		{
			var hpkpReport = ScenarioContext.Current.GetHpkpReport();
			var url = $"{ConfigurationManager.AppSettings["WebServerUrl"]}Security/HpkpReporting/";
			var response = HttpWeb.PostJsonStream(url, hpkpReport);
			Assert.That(response.ResponseStatus, Is.EqualTo(ResponseStatus.Completed));
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}


	}
}
