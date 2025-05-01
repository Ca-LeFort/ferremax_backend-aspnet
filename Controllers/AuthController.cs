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
            var empleadoBase = new BaseUser { Email = empleado.Email };

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
            var clienteBase = new BaseUser { Email = cliente.Email };

            var resultado = _passwordHasher.VerifyHashedPassword(clienteBase, cliente.Password, dto.Password);

            if (cliente == null || resultado != PasswordVerificationResult.Success)
            {
                return Unauthorized(new 
                { 
                    mensaje = "Correo y/o contraseña incorrectas" 
                });
            }

            LogEmail = cliente.Email;
            LogRol = "cliente";
        }

        var token = AddTokenJWT(LogEmail, LogRol);

        return Ok(new 
        {
            token, LogRol
        });
    }

    private string AddTokenJWT(string Email, string Rol)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, Email),
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