using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace AutoTune.Tests
{
    [TestFixture]
    public class AutomationTests
    {
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Test]
        public void Test_Login_Exitoso()
        {
            string filePath = "C:/Users/rartu/AutoTuneStudioManager/AutoTune.Web/index.html";
            driver.Navigate().GoToUrl(filePath);

            driver.FindElement(By.Id("email")).SendKeys("admin@autotune.com");
            driver.FindElement(By.Id("password")).SendKeys("123456");
            
            driver.FindElement(By.CssSelector("button.btn-primary")).Click();

            // Esperar hasta 5 segundos a que el texto del mensaje cambie y contenga la respuesta de la API
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.FindElement(By.Id("mensaje")).Text.Length > 0);

            IWebElement mensaje = driver.FindElement(By.Id("mensaje"));
            Assert.That(mensaje.Text, Does.Contain("Inicio de sesión exitoso"));
        }

        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }
    }
}