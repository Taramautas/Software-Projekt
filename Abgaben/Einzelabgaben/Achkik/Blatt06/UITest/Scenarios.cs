using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.Edge.SeleniumTools;
using System;
using System.Collections.Generic;
using System.Threading;

namespace UITest.Scenarios
{
    public class Scenarios
    {
        IWebDriver chromeDriver;
        IWebDriver edgeDriver;

        /// <summary>
        /// Setup function to initiate chrome and edge driver
        /// </summary>
        [SetUp]
        public void Setup()
        {
            chromeDriver = new ChromeDriver();
            chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            // Launch Microsoft Edge (Chromium)
            var options = new EdgeOptions();
            options.UseChromium = true;
            edgeDriver = new EdgeDriver(@"C:\Users\Radi\Documents\Projekte\Softwareprojekt\tutorium-c-team-11\Abgaben\Einzelabgaben\Achkik\Blatt06\UITest", options);
            edgeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TearDown]
        public void TearDown()
        {
            chromeDriver.Quit();
            edgeDriver.Quit();
        }

        /// <summary>
        /// Test if the scenario of creating a booking with valid inputs is working as expected
        /// </summary>
        [Test]
        public void TestCreateBookingValidInput()
        {
            bool success = CreateBooking("10", "10", "01062020 1100", "01062020 1300");
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
            GoToUrl("https://localhost:5001/Booking");
            // Check how many bookings are displayed on Index View
            Tuple<IReadOnlyCollection<IWebElement>, IReadOnlyCollection<IWebElement>> list_items = FindElementsByCSSSelector("#booking-table tbody tr");
            Assert.AreEqual(list_items.Item1.Count, list_items.Item2.Count, "Both drivers should get the same amount of bookings");
            int num_start = list_items.Item1.Count;
            // Navigate to Create View
            Click(FindElementByCSSSelector(".TableHeadline a[href='/Booking/Create']"));

            // Enter Input and submit form
            Write(FindElementByCSSSelector("#StateOfCharge"), state_of_charge);
            Write(FindElementByCSSSelector("#NeededDistance"), needed_distance);
            start_time = start_time.Replace(" ", Keys.Right);
            Write(FindElementByCSSSelector("#StartTime"), start_time);
            end_time = end_time.Replace(" ", Keys.Right);
            Write(FindElementByCSSSelector("#EndTime"), end_time);
            Click(FindElementByCSSSelector("#create-booking-btn"));

            // Redirect to Index View and count bookings on success
            try
            {
                FindElementByCSSSelector("#booking-table");
                Refresh();
                list_items = FindElementsByCSSSelector("#booking-table tbody tr");
                return num_start + 2 == list_items.Item1.Count;
            }
            // Else return false
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Visit url with chrome and edge
        /// </summary>
        /// <param name="url"></param>
        private void GoToUrl(string url)
        {
            chromeDriver.Navigate().GoToUrl(url);
            edgeDriver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Find elements by css selector with both drivers
        /// </summary>
        /// <param name="selector">CSS selector</param>
        /// <returns>Tuple<chromeElement, edgeElement></returns>
        private Tuple<IWebElement, IWebElement> FindElementByCSSSelector(string selector)
        {
            IWebElement chromeElement = chromeDriver.FindElement(By.CssSelector(selector));
            IWebElement edgeElement = edgeDriver.FindElement(By.CssSelector(selector));

            return Tuple.Create(chromeElement, edgeElement);
        }

        /// <summary>
        /// Find elements by css selector with both drivers
        /// </summary>
        /// <param name="selector">CSS selector</param>
        /// <returns>Tuple of two lists (chromeElements, edgeElements)</returns>
        private Tuple<IReadOnlyCollection<IWebElement>, IReadOnlyCollection<IWebElement>> FindElementsByCSSSelector(string selector)
        {
            IReadOnlyCollection<IWebElement> chromeElements = chromeDriver.FindElements(By.CssSelector(selector));
            IReadOnlyCollection<IWebElement> edgeElements = chromeDriver.FindElements(By.CssSelector(selector));

            Assert.AreEqual(chromeElements.Count, edgeElements.Count, "Both drivers should get the same number of elements");

            return Tuple.Create(chromeElements, edgeElements);
        }

        /// <summary>
        /// Send keys to element with both drivers
        /// </summary>
        /// <param name="elements">Tuple of chrome and edge element (f.e. text input)</param>
        /// <param name="key">The value to send</param>
        private void Write(Tuple<IWebElement, IWebElement> elements, string key)
        {
            IWebElement chromeElement = elements.Item1;
            IWebElement edgeElement = elements.Item2;
            chromeElement.Clear();
            edgeElement.Clear();
            chromeElement.SendKeys(key);
            edgeElement.SendKeys(key);
        }

        /// <summary>
        /// Click on element with both drivers
        /// </summary>
        /// <param name="elements">Tuple of chrome and edge element (f.e. button)</param>
        private void Click(Tuple<IWebElement, IWebElement> elements)
        {
            IWebElement chromeElement = elements.Item1;
            IWebElement edgeElement = elements.Item2;
            chromeElement.Click();
            edgeElement.Click();
        }

        /// <summary>
        /// Refresh both drivers
        /// </summary>
        private void Refresh()
        {
            chromeDriver.Navigate().Refresh();
            edgeDriver.Navigate().Refresh();
        }
    }
}