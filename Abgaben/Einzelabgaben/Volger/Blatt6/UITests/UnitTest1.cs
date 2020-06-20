using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using System;
using System.Collections.Generic;

namespace UITests
{
    public class Tests
    {
        IWebDriver web_driver;

        [SetUp]
        public void Setup()
        {
            web_driver = new ChromeDriver();
            web_driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [Test]
        [TestCase("98", "100", "07072020 1100", "07072020 1300", true)]
        [TestCase("101", "100", "07072020 1100", "07072020 1300", false)]
        [TestCase("", "100", "07072020 1100", "07072020 1300", false)]
        public void Test1(string state_of_charge, string needed_distance, string start_time, string end_time, bool expected)
        {
            bool success = CreateBooking(state_of_charge, needed_distance, start_time, end_time);
            Assert.AreEqual(expected, success);
        }

        private bool CreateBooking(string state_of_charge, string needed_distance, string start_time, string end_time)
        {
            web_driver.Navigate().GoToUrl("https://localhost:44394/Booking");
            IReadOnlyCollection<IWebElement> list_items = web_driver.FindElements(By.CssSelector("#booking-table tbody tr"));
            int num_start = list_items.Count;
            web_driver.FindElement(By.CssSelector(".TableHeadline a[href='/Booking/Create']")).Click();

            IWebElement soc_btn = web_driver.FindElement(By.CssSelector("#SoC"));
            IWebElement needed_distance_btn = web_driver.FindElement(By.CssSelector("#NeededDistance"));
            IWebElement start_time_btn = web_driver.FindElement(By.CssSelector("#Start"));
            IWebElement end_time_btn = web_driver.FindElement(By.CssSelector("#End"));
            IWebElement create_btn = web_driver.FindElement(By.CssSelector("#createbtn"));

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

            try
            {
                list_items = web_driver.FindElements(By.CssSelector("#booking-table tbody tr"));
                return num_start + 1 == list_items.Count;
            }
            catch
            {
                return false;
            }
        }
    }
}