using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.Edge.SeleniumTools;
using System;
using System.Collections.Generic;

namespace UI_Test
{
    public class Tests
    {
        IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            if ("edge" == TestContext.Parameters["webDriver"])              // Setup for Ms Edge
            {
                var options = new EdgeOptions
                {
                    UseChromium = true
                };
                driver = new EdgeDriver(@"E:\Uni\Software\Softwareprojekt\Übungsblätter\Aufgabe6\UI-Test", options);
            }
            else if ("chrome" == TestContext.Parameters["webDriver"])      // Setup for Chrome
            {
                driver = new ChromeDriver();
            }
        }


        [Test]
        public void TestForValidBooking()
        {
            bool valid = CreateBooking("75", "45", "20062020 1400", "20062020 1300");
            Assert.IsTrue(valid);
        }

        [Test]
        public void TestForInvalidBooking()
        {
            bool invalid = CreateBooking("-50", "45", "20062020 1400", "20062020 1300");
            Assert.IsFalse(invalid);
        }

        [Test]
        public void TestForEmptyBooking()
        {
            bool empty = CreateBooking(" ", "45", "20062020 1400", " ");
            Assert.IsFalse(empty);
        }

        private bool CreateBooking(string state_of_charge, string needed_distance, string start_time, string end_time)
        {
            // Open website
            driver.Navigate().GoToUrl("https://localhost:44394/Booking");
            // Check how many bookings are displayed on Index View
            IReadOnlyCollection<IWebElement> list_items = driver.FindElements(By.CssSelector("#booking-table tbody tr"));
            int num_start = list_items.Count;
            // Navigate to Create View
            driver.FindElement(By.CssSelector(".TableHeadline a[href='/Booking/Create']")).Click();

            // Enter Input and submit form
            IWebElement soc_btn = driver.FindElement(By.CssSelector("#StateOfCharge"));
            IWebElement needed_distance_btn = driver.FindElement(By.CssSelector("#NeededDistance"));
            IWebElement start_time_btn = driver.FindElement(By.CssSelector("#StartTime"));
            IWebElement end_time_btn = driver.FindElement(By.CssSelector("#EndTime"));
            IWebElement create_btn = driver.FindElement(By.CssSelector("#create-booking-btn"));

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
                list_items = driver.FindElements(By.CssSelector("#booking-table tbody tr"));
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