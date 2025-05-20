using ApiPrincipal_Ferremas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiPrincipal_Ferremas.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly IPasswordHasher<BaseUser> _passwordHasher;
        private readonly SistemaFerremasContext _context;
        private readonly IConfiguration _config;

        public AuthController(IPasswordHasher<BaseUser> passwordHasher, SistemaFerremasContext context, IConfiguration config)
        {
            _passwordHasher = passwordHasher;
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var parts = dto.Email.Split('@');
            var dominio = parts[parts.Length - 1].ToLower();
            string LogRut = "";
            string LogRol = "";
            
            if (dominio == "ferremas.cl")
            {
                var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.Email == dto.Email);
                
                #pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
                var empleadoBase = new BaseUser { Email = empleado.Email };
                #pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.

                var resultado = _passwordHasher.VerifyHashedPassword(empleadoBase, empleado.Password, dto.Password);

                if (empleado == null || resultado != PasswordVerificationResult.Success)
                {
                    return Unauthorized(new 
                    { 
                        mensaje = "Correo y/o contraseña incorrectas" 
                    });
                }

                LogRut = empleado.RutEmpleado;

                // Asignación de tipos de empleado
                if(empleado.IdTipoEmp == 1)
                {
                    LogRol = "administrador";
                } 
                else if (empleado.IdTipoEmp == 2)
                {
                    LogRol = "encargado";
                }
                else if (empleado.IdTipoEmp == 3)
                {
                    LogRol = "bodeguero";
                }
                else if (empleado.IdTipoEmp == 4)
                {
                    LogRol = "contador";
                }
                
            }
            else
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == dto.Email);

                if (cliente == null)
                {
                    return Unauthorized(new
                    {
                        mensaje = "Correo y/o contraseña incorrectas"
                    });
                }

                #pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
                var clienteBase = new BaseUser { Email = cliente.Email };
                #pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.

                var resultado = _passwordHasher.VerifyHashedPassword(clienteBase, cliente.Password, dto.Password);

                if (resultado != PasswordVerificationResult.Success)
                {
                    return Unauthorized(new
                    {
                        mensaje = "Correo y/o contraseña incorrectas"
                    });
                }

                LogRut = cliente.RutCliente;
                LogRol = "cliente";
            }

            var token = AddTokenJWT(LogRut, LogRol);

            return Ok(new 
            {
                token, LogRol
            });
        }

        private string AddTokenJWT(string Rut, string Rol)
        {
            #pragma warning disable CS8604 // Posible argumento de referencia nulo
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );
            #pragma warning restore CS8604 // Posible argumento de referencia nulo

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("rut", Rut),
                new Claim("rol", Rol)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}