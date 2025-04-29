using ApiPrincipal_Ferremas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class ClienteController : ControllerBase
{

    private readonly SistemaFerremasContext _context;

    public ClienteController(SistemaFerremasContext context)
    {
        _context = context;
    }

    // GET: api/cliente
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
    {
        return await _context.Clientes.ToListAsync();
    }
}