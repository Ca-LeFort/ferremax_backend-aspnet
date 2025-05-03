using ApiPrincipal_Ferremas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/empleados")]
[ApiController]
public class EmpleadoController : ControllerBase
{
    private readonly IPasswordHasher<BaseUser> _passwordHasher;
    private readonly SistemaFerremasContext _context;

    public EmpleadoController(IPasswordHasher<BaseUser> passwordHasher, SistemaFerremasContext context)
    {
        _passwordHasher = passwordHasher;
        _context = context;
    }

    // GET: api/empleados
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Empleado>>> ListarEmpleados()
    {
        try
        {
            return await _context.Empleados.ToListAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                mensaje = "Error interno, vuelve a intentar más tarde",
                detalle = ex.Message
            });
        }
    }
}