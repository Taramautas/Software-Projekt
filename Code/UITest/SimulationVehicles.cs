using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.Edge.SeleniumTools;
using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium.Support.PageObjects;
using Newtonsoft.Json.Serialization;

namespace UITest.SimulationVehicles
{
    [TestFixture]
    public class SimulationVehicles
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
        [TestCase("12", true)]
        public void TestSimulationVehicles(String count_veh, bool expected)
        {
            bool success = InputSimulationVehicles(count_veh);
            Assert.AreEqual(expected, success);
        }

        private bool InputSimulationVehicles(String count_veh)
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
            tick_minutes.SendKeys("1");

            min.Clear();
            min.SendKeys("1");

            max.Clear();
            max.SendKeys("1");

            spread.Clear();
            spread.SendKeys("1");

            weeks.Clear();
            weeks.SendKeys("1");

            btn_submit.Click();

            IWebElement count = web_driver.FindElement(By.Name("count"));
            IWebElement btn_add = web_driver.FindElement(By.ClassName("btn-primary"));

            IReadOnlyCollection<IWebElement> list_items = web_driver.FindElements(By.CssSelector("#booking-table tbody tr"));

            int num_veh_before = list_items.Count;

            count.Clear();
            count.SendKeys(count_veh);

            btn_add.Click();

            list_items = web_driver.FindElements(By.CssSelector("#booking-table tbody tr"));

            int num_veh_after = list_items.Count;

            if(num_veh_before < num_veh_after)
            {
                return true;
            }
            
            return false;

        }
    }
}