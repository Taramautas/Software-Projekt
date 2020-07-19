using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.Edge.SeleniumTools;
using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium.Support.PageObjects;
using Newtonsoft.Json.Serialization;

namespace UITest.SimulationConfig
{
    [TestFixture]
    public class SimulationConfig
    {

        IWebDriver web_driver;

        /// <summary>
        /// Setup function to initiate chrome and edge driver
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Launch Chrome
            if ("chrome" == TestContext.Parameters["webDriver"])
            {
                web_driver = new ChromeDriver();
            }
            else if ("edge" == TestContext.Parameters["webDriver"])
            {
                // Launch Microsoft Edge (Chromium)
                var options = new EdgeOptions
                {
                    UseChromium = true
                };
                web_driver = new EdgeDriver(@"C:\Users\bene\tutorium-c-team-11\UITest", options);
            }
            web_driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TearDown]
        public void TearDown()
        {
            web_driver.Quit();
        }

        [Test]
        [TestCase("1", "1", "2", "1", "1", true)]
        [TestCase("1", "1", "0", "1", "1", false)]
        [TestCase("0", "1", "2", "1", "1", false)]
        [TestCase("1", "1", "2", "0", "1", false)]
        [TestCase("1", "1", "2", "1", "0", false)]
        public void TestSimulationConfig(string ticks, string minimum, string maximum, string spread_bok, string weeks_bok, bool expected)
        {
            bool success = InputSimulationConfig(ticks, minimum, maximum, spread_bok, weeks_bok);
            Assert.AreEqual(expected, success);
        }

        private bool InputSimulationConfig(string ticks, string minimum, string maximum, string spread_bok, string weeks_bok)
        {
            // Open website
            web_driver.Navigate().GoToUrl("https://localhost:44394/");

            IWebElement email = web_driver.FindElement(By.Name("email"));
            IWebElement passw = web_driver.FindElement(By.Name("password"));
            IWebElement btn = web_driver.FindElement(By.ClassName("btn-outline-primary"));

            email.Clear();
            email.SendKeys("admin@admin.de");

            passw.Clear();
            passw.SendKeys("admin");

            btn.Click();

            web_driver.Navigate().GoToUrl("https://localhost:44394/administration/SimulationConfig");

            IWebElement tick_minutes = web_driver.FindElement(By.Name("tick_minutes"));
            IWebElement min = web_driver.FindElement(By.Name("min"));
            IWebElement max = web_driver.FindElement(By.Name("max"));
            IWebElement spread = web_driver.FindElement(By.Name("spread"));
            IWebElement weeks = web_driver.FindElement(By.Name("weeks"));
            IWebElement btn_submit = web_driver.FindElement(By.Id("btnsub"));

            tick_minutes.Clear();
            tick_minutes.SendKeys(ticks);

            min.Clear();
            min.SendKeys(minimum);

            max.Clear();
            max.SendKeys(maximum);

            spread.Clear();
            spread.SendKeys(spread_bok);

            weeks.Clear();
            weeks.SendKeys(weeks_bok);

            btn_submit.Click();

            try
            {
                web_driver.FindElement(By.Name("vehicle_id"));
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;

        }
    }
}