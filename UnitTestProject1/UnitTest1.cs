using Applitools;
using Applitools.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Drawing;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(1, 2);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Assert.AreEqual(2, 2);
        }

        [TestMethod]
        public void VisualTest()
        {
            string appName = "GitHubApp";
            string testName = "GitHubTest";
            string weburl = "https://applitools.com/helloworld2";
            var eyes = new Eyes(new Uri("http://b6bada57.ngrok.io"));

            // obtain the API key from an environment variable and set it
            eyes.ApiKey = Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY");

            // obtain the ID from the environment variables - the name should be specified as null
            var batchName = Environment.GetEnvironmentVariable("APPLITOOLS_BATCH_ID");
            var batchId = Environment.GetEnvironmentVariable("APPLITOOLS_BATCH_ID");

            // set the batch
            var batchInfo = new Applitools.BatchInfo(batchName)
            {
                Id = batchId
            };
            eyes.Batch = batchInfo;

            var SauceLabsAccessKey = Environment.GetEnvironmentVariable("SauceLabsAccessKey");
            var SauceLabsUsername = Environment.GetEnvironmentVariable("SauceLabsUsername");

            var viewportSizeLandscape = new Size(/*width*/1024, /*height*/ 768);

            IWebDriver innerDriver;
            var caps = new DesiredCapabilities();
            caps.SetCapability("browserName", "Chrome");
            caps.SetCapability("platform", "Windows 10");
            caps.SetCapability("version", "65.0");
            var remoteUrl = $"{SauceLabsAccessKey}";
            innerDriver = new RemoteWebDriver(new Uri($"http://{SauceLabsUsername}:{SauceLabsAccessKey}@ondemand.saucelabs.com:80/wd/hub"), caps);

            TestResults result = null;
            IWebDriver driver = eyes.Open(innerDriver, appName, testName, viewportSizeLandscape);
            try
            {
                driver.Url = weburl;
                eyes.CheckWindow("Before enter name");                 // Visual checkpoint 1

                driver.FindElement(By.Id("name")).SendKeys("My Name"); //enter the name
                eyes.CheckWindow("After enter name");                  // Visual checkpoint 2

                driver.FindElement(By.TagName("button")).Click();      // Click the  button
                eyes.CheckWindow("After Click");                       // Visual checkpoint 3

                result = eyes.Close(false);     //false means don't thow exception for failed tests
                
                HandleResult(result);
            }
            finally
            {
                innerDriver.Quit();

                eyes.AbortIfNotClosed();
            }

            if (result != null)
            {
                Assert.IsTrue(result.IsPassed);
            }
            else
            {
                Assert.Fail();
            }            
        }

        private static void HandleResult(TestResults result)
        {
            string resultStr;
            string url = result.Url;
            if (result == null)
            {
                resultStr = "Test aborted";
                url = "undefined";
            }
            else
            {
                url = result.Url;
                int totalSteps = result.Steps;
                if (result.IsNew)
                {
                    resultStr = "New Baseline Created: " + totalSteps + " steps";
                }
                else if (result.IsPassed)
                {
                    resultStr = "All steps passed:     " + totalSteps + " steps";
                }
                else
                {
                    resultStr = "Test Failed     :     " + totalSteps + " steps";
                    resultStr += " matches=" + result.Matches;      /*  matched the baseline */
                    resultStr += " missing=" + result.Missing;       /* missing in the test*/
                    resultStr += " mismatches=" + result.Mismatches; /* did not match the baseline */
                }
            }
            resultStr += "\n" + "results at " + url;
            Console.WriteLine(resultStr);
        }
    }
}
