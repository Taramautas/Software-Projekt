using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.Edge.SeleniumTools;
using System;
using System.Collections.Generic;
using System.Threading;

namespace UITest.Scenarios
{
    [TestFixture]
    public class UITest
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
                web_driver = new EdgeDriver(@"C:\Users\marci\OneDrive\Desktop\SS20\Software Projekt\Übung\Beispiel\tutorium-c-team-11\Abgaben\Einzelabgaben\Kuhnert\Blatt06\UITest\UITest.csproj", options);
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
        public void ValidInput()
        {
            bool success = CreateBooking("10", "10", "01062020 1100", "01062020 1300");
            Assert.IsTrue(success);
        }
        
        /// <summary>
        /// Test if the scenario of creating a booking with invalid input is working
        /// </summary>
        [Test]
        public void InvalidInput()
        {
            bool success = CreateBooking("105", "10", "01062020 1100", "01062020 1300");
            Assert.IsFalse(success);
        }

        /// <summary>
        /// Test if the scenario of creating a booking with empty input is working
        /// </summary>
        [Test]
        public void EmptyInput()
        {
            bool success = CreateBooking("10", "10", "", "01062020 1300");
            Assert.IsFalse(success);
        }

        /// <summary>
        /// Try to create a booking via the Booking/Create View
        /// </summary>
        /// <param name="state_of_charge">State of charge as string</param>
        /// <param name="needed_distance">Needed distance as string</param>
        /// <param name="start_time">Start time in format (ddmmyyyy HHMM)</param>
        /// <param name="end_time">End time in format (ddmmyyyy HHMM)</param>
        /// <returns>Whether the booking could have been created</returns>
        private bool CreateBooking(string state_of_charge, string needed_distance, string start_time, string end_time)
        {
            // Open website
            web_driver.Navigate().GoToUrl("https://localhost:44394/Booking");
            // Check how many bookings are displayed on Index View
            IReadOnlyCollection<IWebElement> list_items = web_driver.FindElements(By.CssSelector("#booking-table tbody tr"));
            int num_start = list_items.Count;
            // Navigate to Create View
            web_driver.FindElement(By.CssSelector(".TableHeadline a[href='/Booking/Create']")).Click();

            // Enter Input and submit form
            IWebElement soc_btn = web_driver.FindElement(By.CssSelector("#StateOfCharge"));
            IWebElement needed_distance_btn = web_driver.FindElement(By.CssSelector("#NeededDistance"));
            IWebElement start_time_btn = web_driver.FindElement(By.CssSelector("#StartTime"));
            IWebElement end_time_btn = web_driver.FindElement(By.CssSelector("#EndTime"));
            IWebElement create_btn = web_driver.FindElement(By.CssSelector("#create-booking-btn"));

            soc_btn.Clear();
            soc_btn.SendKeys(state_of_charge);

            needed_distance_btn.Clear();
            needed_distance_btn.SendKeys(needed_distance);


            start_time_btn.Clear();
            start_time = start_time.Replace(" ", Keys.Right);
            start_time_btn.SendKeys(start_time);


            end_time_btn.Clear();
            end_time = end_time.Replace(" ", Keys.Right);
            end_time_btn.SendKeys(end_time);

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