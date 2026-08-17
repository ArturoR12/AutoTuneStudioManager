using System;
using System.IO;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace AutoTuneStudio.Tests
{
    [TestFixture]
    public class AutomationTests
    {
        private static IWebDriver _driver;
        private static WebDriverWait _wait;
        private static string _basePath;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--window-size=1920,1080");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "AutoTune.Web"));
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }

        private void NavegarTab(string tabTargetId)
        {
            IWebElement tabBtn = _wait.Until(d => d.FindElement(By.XPath($"//button[@data-bs-target='#{tabTargetId}']")));
            tabBtn.Click();
        }

        [Test, Order(1)]
        public void HU01_RegistroDeUsuario()
        {
            string indexUrl = new Uri(Path.Combine(_basePath, "index.html")).AbsoluteUri;
            _driver.Navigate().GoToUrl(indexUrl);

            IWebElement linkRegistro = _wait.Until(d => d.FindElement(By.XPath("//a[contains(text(),'Regístrate')]")));
            linkRegistro.Click();

            _driver.FindElement(By.Id("reg-nombre")).SendKeys("Carlos Administrador");
            _driver.FindElement(By.Id("reg-email")).SendKeys("admin@autotune.com");
            _driver.FindElement(By.Id("reg-password")).SendKeys("AdminPass123");

            IWebElement selectRolElem = _driver.FindElement(By.Id("reg-rol"));
            selectRolElem.FindElement(By.XPath("//option[@value='Administrador']")).Click();

            _driver.FindElement(By.Id("btn-register")).Click();
            _wait.Until(d => d.Url.Contains("dashboard.html"));
        }

        [Test, Order(2)]
        public void HU02_VisualizacionDashboard()
        {
            Assert.That(_driver.PageSource.Contains("Panel de Control"), Is.True);
        }

        [Test, Order(3)]
        public void HU03_AgendarCita()
        {
            NavegarTab("tab-citas");

            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            string fechaFutura = DateTime.Now.AddDays(2).ToString("yyyy-MM-ddTHH:mm");
            js.ExecuteScript($"document.getElementById('cita-fecha').value = '{fechaFutura}';");

            IWebElement btnSolicitar = _driver.FindElement(By.XPath("//form[@onsubmit='agendarCita(event)']//button"));
            btnSolicitar.Click();

            IAlert alertCita = _wait.Until(d => d.SwitchTo().Alert());
            alertCita.Accept();
        }

        [Test, Order(4)]
        public void HU04_AprobarCita()
        {
            IWebElement btnAprobar = _wait.Until(d => d.FindElement(By.XPath("//button[contains(text(),'Aprobar')]")));
            btnAprobar.Click();

            IWebElement badgeEstado = _driver.FindElement(By.XPath("//span[contains(@class,'badge')]"));
            Assert.That(badgeEstado.Text, Is.EqualTo("Aprobada"));
        }

        [Test, Order(5)]
        public void HU05_AsignarTecnico()
        {
            _driver.FindElement(By.XPath("//button[contains(text(),'Técnico')]")).Click();
            
            IAlert promptTecnico = _wait.Until(d => d.SwitchTo().Alert());
            promptTecnico.SendKeys("Mecánico Roberto");
            promptTecnico.Accept();

            Assert.That(_driver.PageSource.Contains("Mecánico Roberto"), Is.True);
        }

        [Test, Order(6)]
        public void HU06_ConsultaDeOrden()
        {
            NavegarTab("tab-ordenes");
            _driver.FindElement(By.Id("codigo-orden")).SendKeys("ORD-999");
            _driver.FindElement(By.XPath("//button[contains(text(),'Buscar')]")).Click();

            bool ordenEncontrada = _wait.Until(d => {
                var elem = d.FindElement(By.Id("resultado-orden"));
                return elem.Text.Contains("ORD-999");
            });

            Assert.That(ordenEncontrada, Is.True);
        }

        [Test, Order(7)]
        public void HU07_RegistroYAlmacen()
        {
            NavegarTab("tab-almacen");
            _driver.FindElement(By.Id("p-codigo")).SendKeys("P-TEST");
            _driver.FindElement(By.Id("p-nombre")).SendKeys("Filtro Cónico K&N");
            _driver.FindElement(By.Id("p-stock")).SendKeys("2");
            _driver.FindElement(By.Id("p-precio")).SendKeys("85");
            _driver.FindElement(By.XPath("//form[@onsubmit='registrarPieza(event)']//button")).Click();
        }

        [Test, Order(8)]
        public void HU08_AlertaStockMinimo()
        {
            IWebElement filaCritica = _wait.Until(d => d.FindElement(By.ClassName("stock-alert")));
            Assert.That(filaCritica.Text.Contains("Bajo Stock"), Is.True);
        }

        [Test, Order(9)]
        public void HU09_CancelarCita()
        {
            NavegarTab("tab-citas");
            IWebElement btnCancelar = _wait.Until(d => d.FindElement(By.XPath("//table//button[contains(text(),'Cancelar')]")));
            btnCancelar.Click();

            try
            {
                IAlert confirmCancelar = _driver.SwitchTo().Alert();
                confirmCancelar.Accept();
            }
            catch (NoAlertPresentException) { }

            bool existeBadgeCancelada = _wait.Until(d => {
                var badges = d.FindElements(By.XPath("//table//span[contains(@class,'badge')]"));
                foreach (var b in badges)
                {
                    if (b.Text.Contains("Cancelada")) return true;
                }
                return false;
            });

            Assert.That(existeBadgeCancelada, Is.True);
        }

        [Test, Order(10)]
        public void HU10_HistorialDeServicios()
        {
            NavegarTab("tab-ordenes");
            
            bool historialEncontrado = _wait.Until(d => {
                var historial = d.FindElement(By.Id("historial-vehiculo"));
                return historial.Text.Contains("Stage 1");
            });

            Assert.That(historialEncontrado, Is.True, "Error en HU10: No se encontró 'Stage 1' en el historial de servicios.");
        }

        [Test, Order(11)]
        public void HU11_ReporteOperativo()
        {
            NavegarTab("tab-reportes");
            _driver.FindElement(By.XPath("//button[contains(text(),'Exportar a PDF')]")).Click();

            bool reporteGenerado = _wait.Until(d => {
                var output = d.FindElement(By.Id("reporte-output"));
                string texto = output.Text.Trim();
                return !string.IsNullOrEmpty(texto) && texto.ToLower().Contains("exportad");
            });

            Assert.That(reporteGenerado, Is.True);
        }

        [Test, Order(12)]
        public void HU12_EditarPerfil()
        {
            NavegarTab("tab-perfil");
            IWebElement inputNombre = _driver.FindElement(By.Id("perfil-nombre"));
            inputNombre.Clear();
            inputNombre.SendKeys("Carlos Editado");

            IWebElement inputTel = _driver.FindElement(By.Id("perfil-telefono"));
            inputTel.Clear();
            inputTel.SendKeys("809-555-9999");

            _driver.FindElement(By.XPath("//form[@onsubmit='actualizarPerfil(event)']//button")).Click();

            IAlert alertPerfil = _wait.Until(d => d.SwitchTo().Alert());
            alertPerfil.Accept();

            IWebElement headerBienvenida = _driver.FindElement(By.Id("bienvenida"));
            Assert.That(headerBienvenida.Text.Contains("Carlos Editado"), Is.True);
        }
    }
}