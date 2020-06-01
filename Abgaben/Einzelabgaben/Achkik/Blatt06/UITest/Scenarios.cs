using NUnit.Framework;
using NUnit.Framework.Constraints;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
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
            chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            edgeDriver = new EdgeDriver();
            edgeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TearDown]
        public void TearDown()
        {
            chromeDriver.Quit();
            edgeDriver.Quit();
        }

        [Test]
        public void TestCreateBooking()
        {
            // Check two times to be sure
            foreach (int i in Enumerable.Range(1, 2))
            {
                // Open website
                GoToUrl("https://localhost:5001/Booking");
                // Check how many bookings are displayed on Index View
                Tuple<IReadOnlyCollection<IWebElement>, IReadOnlyCollection<IWebElement>> list_items = FindElementsByCSSSelector("#booking-table tbody tr");
                Assert.AreEqual(list_items.Item1.Count, list_items.Item2.Count, "Both drivers should get the same amount of bookings");
                int num_start = list_items.Item1.Count;
                // Navigate to Create View
                Tuple<IWebElement, IWebElement> createButton = FindElementByCSSSelector(".TableHeadline a[href='/Booking/Create']");
                Click(createButton);

                // Enter Valid Input and submit form
                Tuple<IWebElement, IWebElement> stateOfChargeInput = FindElementByCSSSelector("#StateOfCharge");
                SendKeys(stateOfChargeInput, "2");
                Tuple<IWebElement, IWebElement> NeededDistanceInput = FindElementByCSSSelector("#NeededDistance");
                SendKeys(NeededDistanceInput, "5");
                Tuple<IWebElement, IWebElement> createBookingButton = FindElementByCSSSelector("#create-booking-btn");
                Click(createBookingButton);

                // Return to Index View and count number of bookings in table
                FindElementByCSSSelector("#booking-table");
                list_items = FindElementsByCSSSelector("#booking-table tbody tr");
                Assert.AreEqual(list_items.Item1.Count, list_items.Item2.Count, "Both drivers should get the same amount of bookings");
                Assert.AreEqual(num_start + 1, list_items.Item1.Count, "There should be excactly one more booking in the table after creating one");
            }
        }

        public void GoToUrl(string url)
        {
            chromeDriver.Navigate().GoToUrl(url);
            edgeDriver.Navigate().GoToUrl(url);
        }

        public Tuple<IWebElement, IWebElement> FindElementByCSSSelector(string selector)
        {
            IWebElement chromeElement = chromeDriver.FindElement(By.CssSelector(selector));
            IWebElement edgeElement = edgeDriver.FindElement(By.CssSelector(selector));

            return Tuple.Create(chromeElement, edgeElement);
        }

        public Tuple<IReadOnlyCollection<IWebElement>, IReadOnlyCollection<IWebElement>> FindElementsByCSSSelector(string selector)
        {
            IReadOnlyCollection<IWebElement> chromeElements = chromeDriver.FindElements(By.CssSelector(selector));
            IReadOnlyCollection<IWebElement> EdgeElements = chromeDriver.FindElements(By.CssSelector(selector));

            return Tuple.Create(chromeElements, EdgeElements);
        }

        public void SendKeys(Tuple<IWebElement, IWebElement> elements, string key)
        {
            IWebElement chromeElement = elements.Item1;
            IWebElement edgeElement = elements.Item2;
            edgeElement.SendKeys(key);
            edgeElement.SendKeys(key);
        }

        public void Click(Tuple<IWebElement, IWebElement> elements)
        {
            IWebElement chromeElement = elements.Item1;
            IWebElement edgeElement = elements.Item2;
            edgeElement.Click();
            edgeElement.Click();
        }
    }
}