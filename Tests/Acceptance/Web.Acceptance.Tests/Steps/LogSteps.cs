using SecurityEssentials.Acceptance.Tests.Extensions;
using SecurityEssentials.Acceptance.Tests.Pages;
using SecurityEssentials.Model;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SecurityEssentials.Acceptance.Tests.Steps
{
    [Binding]
    public class LogSteps : TechTalk.SpecFlow.Steps
    {
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        public LogSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }
        [Then(@"I am shown the following logs:")]
        public void ThenIAmShownTheFollowingLogs(Table table)
        {
            var page = _scenarioContext.GetPage<LogPage>();
            var recordsDisplayed = page.GetLogsDisplayed();
            recordsDisplayed.Remove(new Log { Level = "Information", Message = "Application Started" });
            table.CompareToSet(recordsDisplayed);
        }

    }
}
