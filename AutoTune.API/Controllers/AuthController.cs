using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoTune.API.Data;
using AutoTune.API.Models;

namespace AutoTune.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario request)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == request.PasswordHash);

            if (user == null)
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas." });
            }

            return Ok(new { mensaje = "Inicio de sesión exitoso.", usuario = user });
        }
    }
}