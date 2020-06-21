using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.Edge.SeleniumTools;
using System;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

namespace UI_Tests
{
    [TestFixture]
    public class Tests
    {
        IWebDriver web_driver;

        [SetUp]
        public void Setup()
        {
            if ("chrome" == TestContext.Parameters["webDriver"])
            {
                web_driver = new ChromeDriver(@"C:\Users\Marcel\Desktop\Uni stuff\Uni Übungsaufgaben\Semester 4\SoftwareProjekt\Blatt 6\Blatt06_mittests\UI-Tests");
            }
            else if ("edge" == TestContext.Parameters["webDriver"])
            {
                var options = new EdgeOptions
                {
                    UseChromium = true
                };
                web_driver = new EdgeDriver(@"C:\Users\Marcel\Desktop\Uni stuff\Uni Übungsaufgaben\Semester 4\SoftwareProjekt\Blatt 6\Blatt06_mittests\UI-Tests", options);
            }
            web_driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TearDown]
        public void TearDown()
        {
            web_driver.Quit();
        }

        private bool CreateBooking(string charge, string needed_distance, string start_time, string end_time, string con_type)
        {
            web_driver.Navigate().GoToUrl("https://localhost:44394/Booking");

            // Count How many Bookings there are before creating a new one
            IReadOnlyCollection<IWebElement> rows = web_driver.FindElements(By.CssSelector("#booking_table tbody tr"));
            int count = rows.Count;

            // click on button create new booking
            web_driver.FindElement(By.CssSelector(".ml-1")).Click();

            // identify required fields
            IWebElement charge_btn = web_driver.FindElement(By.Id("Charge"));
            IWebElement needed_distance_btn = web_driver.FindElement(By.Id("Needed_distance"));
            IWebElement start_time_btn = web_driver.FindElement(By.Id("Start_time"));
            IWebElement end_time_btn = web_driver.FindElement(By.Id("End_time"));
            SelectElement con_type_btn = new SelectElement(web_driver.FindElement(By.Name("Connector_Type")));
            IWebElement create_btn = web_driver.FindElement(By.CssSelector(".btn"));

            // fill out form
            charge_btn.Clear();
            charge_btn.SendKeys(charge);

            needed_distance_btn.Clear();
            needed_distance_btn.SendKeys(needed_distance);

            start_time_btn.Clear();
            start_time_btn.SendKeys(start_time);

            end_time_btn.Clear();
            end_time_btn.SendKeys(end_time);

            con_type_btn.SelectByText(con_type);

            create_btn.Click();

            // Count How many Bookings there are after creating a new booking
            rows = web_driver.FindElements(By.CssSelector("#booking_table tbody tr"));
            int count2 = rows.Count;
            return count + 1 == count2;
        }

        [Test]
        public void TestCreateBookingValidInput()
        {
            bool success = CreateBooking("40", "300", "05072020" + Keys.Right + "1400", "05072020" + Keys.Right + "1800", "Tesla Supercharger");
            Assert.IsTrue(success);
        }

        [Test]
        public void TestCreateBookingInvalidInput()
        {
            bool success = CreateBooking("400", "300", "05072020" + Keys.Right + "1400", "05072020" + Keys.Right + "1800", "Tesla Supercharger");
            Assert.IsFalse(success);
        }

        [Test]
        public void TestCreateBookingMissingInput()
        {
            bool success = CreateBooking("40", "", "05072020" + Keys.Right + "1400", "05072020" + Keys.Right + "1800", "Tesla Supercharger");
            Assert.IsFalse(success);
        }


    }
}