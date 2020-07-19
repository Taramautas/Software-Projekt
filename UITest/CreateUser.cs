using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.Edge.SeleniumTools;
using System;
using System.Collections.Generic;
using System.Threading;

namespace UITest.UserCreateDelete
{
    [TestFixture]
    public class UserCreateDelete
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
                web_driver = new EdgeDriver(@"C:\Users\elena\Desktop\tutorium-c-team-11\UITest", options);
            }
            web_driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TearDown]
        public void TearDown()
        {
            web_driver.Quit();
        }

        /// <summary>
        /// Test if the scenario of creating an user with valid inputs is working as expected
        /// </summary>
        [Test]
        public void TestCreateUserValidInput()
        {
            bool success = CreateUser("bla", "blabla@bla.bla", "blabla");
            Assert.IsTrue(success);
        }

        /// <summary>
        /// Test if the scenario of creating an user with empty input is working
        /// </summary>
        [Test]
        public void TestCreateUserEmptyInput()
        {
            bool success = CreateUser("bla", "blabla@bla.bla", "");
            Assert.IsFalse(success);
        }
        /// <summary>
        /// Test if the delete function is working
        /// </summary>
        [Test]
        public void TestCreateUserDelete()
        {
            CreateUser("bla", "blabla@bla.bla", "blabla");
            IReadOnlyCollection<IWebElement> list_items = web_driver.FindElements(By.CssSelector("#booking-table tbody tr"));
            int num_start = list_items.Count;
            bool succes = false;

            IWebElement delete = web_driver.FindElement(By.CssSelector("#delete_btn_uitest"));
            delete.Click();

            if (num_start == list_items.Count)
                succes = true;
            Assert.IsTrue(succes);
        }
        /// <summary>
        /// Test if the log in function with a new user is working
        /// </summary>
        [Test]
        public void TestCreateUserLogIn()
        {
            CreateUser("bla", "blabla@bla.bla", "blabla");
            web_driver.Navigate().GoToUrl("https://localhost:44394/Home/Logout");
            bool succes = true;
            // Log In
            IWebElement user = web_driver.FindElement(By.CssSelector("#email"));
            user.Clear();
            user.SendKeys("blabla@bla.bla");
            IWebElement pw = web_driver.FindElement(By.CssSelector("#password"));
            pw.Clear();
            pw.SendKeys("blabla");
            web_driver.FindElement(By.CssSelector(".submit")).Click();
            web_driver.Navigate().GoToUrl("https://localhost:44394/UserDashboard/Bookings");
            try
            {
                web_driver.FindElement(By.ClassName("btn-secondary"));
            }
            catch (NoSuchElementException)
            {
                succes = false;
            }
            Thread.Sleep(4000);
            Assert.IsTrue(succes);
        }

        /// <summary>
        /// Try to create a booking via the Booking/Create View
        /// </summary>
        /// <param name="current_soc">State of charge as string</param>
        /// <param name="target_soc">Needed distance as string</param>
        /// <param name="start_time">Start time in format (ddmmyyyy HHMM)</param>
        /// <param name="end_time">End time in format (ddmmyyyy HHMM)</param>
        /// <returns>Whether the booking could have been created</returns>
        private bool CreateUser(string name_user, string e_mail, string password)
        {
            // Open website
            web_driver.Navigate().GoToUrl("https://localhost:44394");
            // Log In
            IWebElement user = web_driver.FindElement(By.CssSelector("#email"));
            user.Clear();
            user.SendKeys("assistant@assistant.de");
            IWebElement pw = web_driver.FindElement(By.CssSelector("#password"));
            pw.Clear();
            pw.SendKeys("assistant");
            web_driver.FindElement(By.CssSelector(".submit")).Click();
            // Go To Bookings
            web_driver.Navigate().GoToUrl("https://localhost:44394/UserDashboard/Users");

            // Check how many bookings are displayed on Index View
            IReadOnlyCollection<IWebElement> list_items = web_driver.FindElements(By.CssSelector("#booking-table tbody tr"));
            int num_start = list_items.Count;
            // Navigate to Create View
            web_driver.FindElement(By.CssSelector(".submit")).Click();

            // Enter Input and submit form
            IWebElement name = web_driver.FindElement(By.CssSelector(".name_uitest"));
            IWebElement mail = web_driver.FindElement(By.CssSelector(".email_uitest"));
            IWebElement psw = web_driver.FindElement(By.CssSelector(".password_uitest"));
            IWebElement create_btn = web_driver.FindElement(By.CssSelector(".col-12"));

            name.Clear();
            name.SendKeys(name_user);

            mail.Clear();
            mail.SendKeys(e_mail);

            psw.Clear();
            psw.SendKeys(password);

            create_btn.Click();

            // Redirect to Index View and count bookings on success
            try
            {
                list_items = web_driver.FindElements(By.CssSelector("#booking-table tbody tr"));
                return num_start + 1 == list_items.Count;
            }
            // Else return false
            catch
            {
                return false;
            }
        }
    }
}

