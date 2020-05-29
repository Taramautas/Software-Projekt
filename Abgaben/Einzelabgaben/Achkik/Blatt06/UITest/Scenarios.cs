using NUnit.Framework;
using NUnit.Framework.Constraints;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using System;
using System.Threading;

namespace UITest.Scenarios
{
    public class Scenarios
    {
        IWebDriver chromeDriver;
        IWebDriver edgeDriver;

        [SetUp]
        public void Setup()
        {
            chromeDriver = new ChromeDriver();
            chromeDriver.Manage().Window.Maximize();
        }

        [TearDown]
        public void TearDown()
        {
            chromeDriver.Quit();
            //edgeDriver.Quit();
        }

        [Test]
        public void TestCreateBooking()
        {
            GoToUrl("https://localhost:5001/Booking");
            IWebElement createButton = FindElementByCSSSelector(".TableHeadline a[href='/Booking/Create']");
            createButton.Click();
            IWebElement stateOfChargeInput = FindElementByCSSSelector("#StateOfCharge");
            stateOfChargeInput.SendKeys("20");
            IWebElement NeededDistanceInput = FindElementByCSSSelector("#NeededDistance");
            NeededDistanceInput.SendKeys("50");
            IWebElement createBookingButton = FindElementByCSSSelector("#create-booking-btn");
            createBookingButton.Click();
            Assert.IsNotNull(createBookingButton);
        }

        public void GoToUrl(string url)
        {
            chromeDriver.Navigate().GoToUrl(url);
            //edgeDriver.Navigate().GoToUrl(url);
        }

        public IWebElement FindElementByCSSSelector(string selector)
        {
            IWebElement chrome_element = chromeDriver.FindElement(By.CssSelector(selector));
            //IWebElement edge_element = edgeDriver.FindElement(By.CssSelector(selector));

            return chrome_element;
        }
    }
}