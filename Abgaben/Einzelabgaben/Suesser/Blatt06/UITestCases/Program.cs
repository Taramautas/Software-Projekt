using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System.Collections.Generic;

namespace UITestCases
{
    class Program
    {
        public static string _Website;
        public static IWebDriver driver;

        public static void Main(string[] args)
        {
            _Website = "https://localhost:5001/";
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl(_Website + "Booking/");
            TestCreateBookingWithValidInput();
            TestCreateBookingWithInvalidInput();
            TestCreateBookingWithEmptyInput();
            IReadOnlyCollection<IWebElement> booking_counter = driver.FindElements(By.CssSelector("#booking-table tbody tr"));
            Console.WriteLine(booking_counter.Count);
        }


        public static void TestCreateBookingWithValidInput()
        {
            bool success = CreateBooking("10", "10", "12122020", "1100", "12122020", "1300");
            Assert.IsTrue(success);
        }
        public static void TestCreateBookingWithInvalidInput()
        {
            bool valid = CreateBooking("20", "-2", "12122020", "0000", "13122020", "1800");
            Assert.IsTrue(valid);
        }

        public static void TestCreateBookingWithEmptyInput()
        {
            bool valid = CreateBooking("20", "", "12122020", "0000", "13122020", "1800");
            Assert.IsTrue(valid);
        }


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

            return true;
        }

        

    }
}
