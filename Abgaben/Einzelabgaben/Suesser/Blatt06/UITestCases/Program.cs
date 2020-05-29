using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace UITestCases
{
    class Program
    {
      
        static void Main(string[] args)
        {

            string _Website = "https://localhost:5001/";

            IWebDriver driver = new ChromeDriver();

            System.Threading.Thread.Sleep(1000);


            //Create Valid Input
            driver.Navigate().GoToUrl(_Website + "Booking/Create");
            driver.FindElement(By.Name("Charge")).SendKeys("3");
            driver.FindElement(By.Name("Needed_distance")).SendKeys("34");
            driver.FindElement(By.Name("Start_time")).SendKeys("10102020");
            driver.FindElement(By.Name("Start_time")).SendKeys(Keys.Tab);
            driver.FindElement(By.Name("Start_time")).SendKeys("1010");
            driver.FindElement(By.Name("End_time")).SendKeys("10102020");
            driver.FindElement(By.Name("End_time")).SendKeys(Keys.Tab);
            driver.FindElement(By.Name("End_time")).SendKeys("1230");
            System.Threading.Thread.Sleep(2000);
            driver.FindElement(By.Name("btnsub")).Click();
            System.Threading.Thread.Sleep(1000);


            //Create Invalid Input

            driver.Navigate().GoToUrl(_Website + "Booking/Create");
            System.Threading.Thread.Sleep(1000);
            driver.FindElement(By.Name("Charge")).SendKeys("-3");
            driver.FindElement(By.Name("Needed_distance")).SendKeys("34");
            driver.FindElement(By.Name("Start_time")).SendKeys("10102020");
            driver.FindElement(By.Name("Start_time")).SendKeys(Keys.Tab);
            driver.FindElement(By.Name("Start_time")).SendKeys("1010");
            driver.FindElement(By.Name("End_time")).SendKeys("10102020");
            driver.FindElement(By.Name("End_time")).SendKeys(Keys.Tab);
            driver.FindElement(By.Name("End_time")).SendKeys("1230");
            System.Threading.Thread.Sleep(2000);
            driver.FindElement(By.Name("btnsub")).Click();


            //Create Input with one missing field


            driver.Navigate().GoToUrl(_Website + "Booking/Create");
            System.Threading.Thread.Sleep(1000);
            driver.FindElement(By.Name("Needed_distance")).SendKeys("34");
            driver.FindElement(By.Name("Start_time")).SendKeys("10102020");
            driver.FindElement(By.Name("Start_time")).SendKeys(Keys.Tab);
            driver.FindElement(By.Name("Start_time")).SendKeys("1010");
            driver.FindElement(By.Name("End_time")).SendKeys("10102020");
            driver.FindElement(By.Name("End_time")).SendKeys(Keys.Tab);
            driver.FindElement(By.Name("End_time")).SendKeys("1230");
            System.Threading.Thread.Sleep(2000);
            driver.FindElement(By.Name("btnsub")).Click();


            System.Threading.Thread.Sleep(2000);
            driver.Navigate().GoToUrl(_Website + "Booking");


        }


    }
}
