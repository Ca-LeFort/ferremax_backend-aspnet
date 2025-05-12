using ApiPrincipal_Ferremas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
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
        var dominio = dto.Email.Split('@').Last().ToLower();
        string LogEmail = "";
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

            LogEmail = empleado.Email;
            LogRol = "empleado";
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

            LogEmail = cliente.PNombre;
            LogRol = "cliente";
        }

        var token = AddTokenJWT(LogEmail, LogRol);

        return Ok(new 
        {
            token, LogRol
        });
    }

    private string AddTokenJWT(string Nombre, string Rol)
    {
        #pragma warning disable CS8604 // Posible argumento de referencia nulo
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );
        #pragma warning restore CS8604 // Posible argumento de referencia nulo

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("nombre", Nombre),
            new Claim(ClaimTypes.Role, Rol)
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