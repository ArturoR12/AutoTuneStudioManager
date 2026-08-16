# AutoTune Studio Manager

Sistema de gestión para taller de afinamiento y personalización vehicular (AutoTune Studio). Proyecto full-stack con arquitectura desacoplada que incluye Backend (Web API REST), Frontend Web y Pruebas Automatizadas de interfaz.

---

## Tecnologías Utilizadas

* **Backend:** C# .NET / Web API
* **Base de Datos:** SQL Server (Entity Framework Core)
* **Frontend:** HTML5, CSS3, JavaScript (Fetch API), Bootstrap 5
* **Pruebas Automatizadas:** NUnit + Selenium WebDriver
* **Control de Versiones:** Git & GitHub

---

## Estructura de la Solución

AutoTuneStudioManager/
├── AutoTune.API/       # Web API REST (Modelos, Controladores, DbContext)
├── AutoTune.Web/       # Frontend (HTML, CSS, JS)
└── AutoTune.Tests/     # Suite de Pruebas Automatizadas con Selenium
Configuración e Instalación
1. Base de Datos
Abre SQL Server Management Studio (SSMS).

Ejecuta el script de creación de la base de datos AutoTuneStudioDB y la tabla Usuarios.

Inserta el usuario inicial de prueba:

SQL
INSERT INTO Usuarios (Nombre, Email, PasswordHash, Rol)
VALUES ('Administrador', 'admin@autotune.com', '123456', 'Admin');
2. Backend (API)
Navega a la carpeta del backend:

Bash
cd AutoTune.API
Asegúrate de actualizar la cadena de conexión en appsettings.json con el nombre de tu servidor local.

Inicia la Web API:

Bash
dotnet run
Accede a la documentación en Swagger: http://localhost:5090/swagger

3. Frontend
Abre el archivo AutoTune.Web/index.html en tu navegador.

Ingresa las credenciales de prueba:

Correo: admin@autotune.com

Contraseña: 123456

Pruebas Automatizadas
El proyecto incluye un conjunto de pruebas de extremo a extremo realizadas con Selenium WebDriver y NUnit.

Para ejecutar las pruebas:

Bash
cd AutoTune.Tests
dotnet test