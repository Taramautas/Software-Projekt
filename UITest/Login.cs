using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.Edge.SeleniumTools;
using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium.Support.PageObjects;
using Newtonsoft.Json.Serialization;

namespace UITest.Login
{
    [TestFixture]
    public class Login
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
        [TestCase("user@user.de", "user", true)]
        [TestCase("error", "error", false)]
        [TestCase("false@false.de", "false", false)]
        [TestCase("user@user.de", "", false)]
        [TestCase("", "user", false)]
        public void LoginAsUser(string usermail, string password, bool expected)
        {
            bool success = logIntoWebpage(usermail, password);
            Assert.AreEqual(expected, success);
        }

        private bool logIntoWebpage(string usermail, string password)
        {
            // Open website
            web_driver.Navigate().GoToUrl("https://localhost:44394/");

            IWebElement email = web_driver.FindElement(By.Name("email"));
            IWebElement passw = web_driver.FindElement(By.Name("password"));
            IWebElement btn = web_driver.FindElement(By.ClassName("btn-outline-primary"));

            email.Clear();
            email.SendKeys(usermail);

            passw.Clear();
            passw.SendKeys(password);

            btn.Click();

            web_driver.Navigate().GoToUrl("https://localhost:44394/UserDashboard/Bookings");

            try
            {
                web_driver.FindElement(By.ClassName("btn-secondary"));
            } catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }
    }
}