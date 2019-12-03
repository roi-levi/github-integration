 namespace Applitools
{
    using System;
    using System.Drawing;
    using Applitools.Selenium;
    using NUnit.Framework;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Remote;

    [TestFixture]
    public class MergeDemo
    { 
        //[TestCase(600, 800)]
        [TestCase(1024, 768)]
        public void TestHelloWorld(int width, int height)
        {
            var eyes = new Eyes(new Uri(@"https://398bbbdd.ngrok.io"));
        
            var driver = GetWebDriver(); 
              
            try 
            {
                // Start the test and set the browser's viewport size 
                eyes.Open(driver, "Hello World!", "My first Selenium C# test!", new Size(width, height));

                // Navigate the browser to the "hello world!" web-site. 
                driver.Url = "https://applitools.com/helloworld?diff1";

                // Visual checkpoint #1.
                eyes.CheckWindow("Hello!"); 

                // Click the "Click me!" button.     
                driver.FindElement(By.TagName("button")).Click();

                // Visual checkpoint #2.   
                eyes.CheckWindow("Click!"); 
                 
                // End the test.   
                eyes.Close(false);
            }
            finally
            {
                // Close the browser. 
                driver.Quit(); 

                // If the test was aborted before eyes. Close was called, ends the test as aborted.
                eyes.AbortIfNotClosed();
            }
        }

        private IWebDriver GetWebDriver()
        {
            Func<string, string> env = name => Environment.GetEnvironmentVariable(name);

            var caps = new DesiredCapabilities();
            caps.SetCapability("browserName", "Chrome");
            caps.SetCapability("platform", "Windows 10");
            caps.SetCapability("version", "65.0");

            caps.SetCapability("username", env("SauceLabsUsername"));
            caps.SetCapability("accesskey", env("SauceLabsAccessKey"));

            var driver = new RemoteWebDriver(
                new Uri("http://ondemand.saucelabs.com/wd/hub"), caps, TimeSpan.FromMinutes(4));

            return driver;
        }
    }
}
