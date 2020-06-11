using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System.Collections.Generic;

namespace UITests
{
    [TestFixture]
    class UnitTest1
    {
        private static string _Website;
        private static IWebDriver driver;
        private static bool flag;
        
        /// <summary>
        /// SetupRoutine which initalizes the Website URL and the Webdriver
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _Website = "https://localhost:5001/";
            driver = new ChromeDriver();
        }

        /// <summary>
        /// Testing a CreateBooking-Call with valid input and checks if this was a successs
        /// a flag is set to true that indicatates if there should be one booking (in case the InvalidInput method is called first)
        /// </summary>
        [Test]
        public void TestCreateBookingWithValidInput()
        {
            flag = true;
            Assert.IsTrue(CreateBooking("19", "103", "12122020", "0000", "13122020", "1800"));
        }
        /// <summary>
        /// Testing a CreateBooking-Call with invalid input and checks if this hasn't created a booking
        /// </summary>
        [Test]
        public void TestCreateBookingWithInvalidInput()
        {
            Assert.IsTrue(CreateBooking("20", "-2", "12122020", "0000", "13122020", "1800"));
        }
        /// <summary>
        /// Testing a CreateBooking-Call with an empty input and checks if this hasn't created a booking
        /// </summary>
        [Test]
        public void TestCreateBookingWithEmptyInput()
        {
            Assert.IsTrue(CreateBooking("20", "", "12122020", "0000", "13122020", "1800"));
        }
        /// <summary>
        /// Method to simulate UI-Input. The given parameters are send as keys to the CreateView. At the end the sum of current bookings will be calculated and based
        /// on that number true/false returned
        /// </summary>
        /// <param name="charge">current charge string as string</param>
        /// <param name="needed_distance">needed distance as string</param>
        /// <param name="start_time">start time in format ddmmyyyy as string</param>
        /// <param name="start_time_time">start time time in format mmhh </param>
        /// <param name="end_time">end time in format ddmmyyyy as string</param>
        /// <param name="end_time_time">end time time in format ddmmyyyy as string</param>
        /// <returns></returns>
        public static bool CreateBooking(string charge, string needed_distance, string start_time, string start_time_time
            , string end_time, string end_time_time)
        {
            driver.Navigate().GoToUrl(_Website + "Booking/Create");

            driver.FindElement(By.Name("Charge")).SendKeys(charge);
            driver.FindElement(By.Name("Needed_distance")).SendKeys(needed_distance);
            driver.FindElement(By.Name("Start_time")).SendKeys(start_time);
            driver.FindElement(By.Name("Start_time")).SendKeys(Keys.Tab);
            driver.FindElement(By.Name("Start_time")).SendKeys(start_time_time);

            driver.FindElement(By.Name("End_time")).SendKeys(end_time);
            driver.FindElement(By.Name("End_time")).SendKeys(Keys.Tab);
            driver.FindElement(By.Name("End_time")).SendKeys(end_time_time);

            System.Threading.Thread.Sleep(100);
            driver.FindElement(By.Name("btnsub")).Click();
            driver.Navigate().GoToUrl(_Website + "Booking/");

            IReadOnlyCollection<IWebElement> booking_counter = driver.FindElements(By.CssSelector("#booking-table tbody tr"));
            Console.WriteLine(booking_counter.Count);
            if (booking_counter.Count == 1 && flag == true) return true;
            if (booking_counter.Count == 0 && flag == false) return true;
            return false;
        }
    }
}