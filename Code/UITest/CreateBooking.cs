using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.Edge.SeleniumTools;
using System;
using System.Collections.Generic;
using System.Threading;

namespace UITest.BookingsCreateDelete
{
    [TestFixture]
    public class BookingsCreateDelete
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
                web_driver= new ChromeDriver();
            } else if ("edge" == TestContext.Parameters["webDriver"])
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
        /// Test if the scenario of creating a booking with valid inputs is working as expected
        /// </summary>
        [Test]
        public void TestCreateBookingValidInput()
        {
            bool success = CreateBooking("10", "11", "01082020 1100", "01082020 1300");
            Assert.IsTrue(success);
        }
        
        /// <summary>
        /// Test if the scenario of creating a booking with invalid input is working
        /// </summary>
        [Test]
        public void TestCreateBookingInvalidInput()
        {
            bool success = CreateBooking("105", "10", "01062020 1100", "01062020 1300");
            Assert.IsFalse(success);
        }

        /// <summary>
        /// Test if the scenario of creating a booking with empty input is working
        /// </summary>
        [Test]
        public void TestCreateBookingEmptyInput()
        {
            bool success = CreateBooking("10", "10", "", "01062020 1300");
            Assert.IsFalse(success);
        }
        /// <summary>
        /// Test if the delete function is working
        /// </summary>
        [Test]
        public void Delete()
        {
            CreateBooking("10", "11", "01082020 1100", "01082020 1300");
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
        /// Try to create a booking via the Booking/Create View
        /// </summary>
        /// <param name="current_soc">State of charge as string</param>
        /// <param name="target_soc">Needed distance as string</param>
        /// <param name="start_time">Start time in format (ddmmyyyy HHMM)</param>
        /// <param name="end_time">End time in format (ddmmyyyy HHMM)</param>
        /// <returns>Whether the booking could have been created</returns>
        private bool CreateBooking(string current_soc, string target_soc, string start_time, string end_time)
        {
            // Open website
            web_driver.Navigate().GoToUrl("https://localhost:44394");
            // Log In
            IWebElement user = web_driver.FindElement(By.CssSelector("#email"));
            user.Clear();
            user.SendKeys("user@user.de");
            IWebElement pw = web_driver.FindElement(By.CssSelector("#password"));
            pw.Clear();
            pw.SendKeys("user");
            web_driver.FindElement(By.CssSelector(".submit")).Click();
            // Go To Bookings
            web_driver.Navigate().GoToUrl("https://localhost:44394/UserDashboard/Bookings");

            // Check how many bookings are displayed on Index View
            IReadOnlyCollection<IWebElement> list_items = web_driver.FindElements(By.CssSelector("#booking-table tbody tr"));
            int num_start = list_items.Count;
            // Navigate to Create View
            web_driver.FindElement(By.CssSelector(".submit")).Click();

            // Enter Input and submit form
            IWebElement soc_btn = web_driver.FindElement(By.CssSelector("#StateOfCharge"));
            IWebElement target_soc_btn = web_driver.FindElement(By.CssSelector("#NeededDistance"));
            IWebElement start_time_btn = web_driver.FindElement(By.CssSelector("#StartTime"));
            IWebElement end_time_btn = web_driver.FindElement(By.CssSelector("#EndTime"));
            IWebElement create_btn = web_driver.FindElement(By.CssSelector("#btnsub"));

            soc_btn.Clear();
            soc_btn.SendKeys(current_soc);

            target_soc_btn.Clear();
            target_soc_btn.SendKeys(target_soc);


            start_time_btn.Clear();
            start_time = start_time.Replace(" ", Keys.Right);
            start_time_btn.SendKeys(start_time);


            end_time_btn.Clear();
            end_time = end_time.Replace(" ", Keys.Right);
            end_time_btn.SendKeys(end_time);
            Thread.Sleep(4000);

            Thread.Sleep(100);
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