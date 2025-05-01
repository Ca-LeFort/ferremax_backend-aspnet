using ApiPrincipal_Ferremas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

[Route("api/clientes")]
[ApiController]
public class ClienteController : ControllerBase
{
    private readonly IPasswordHasher<BaseUser> _passwordHasher;
    private readonly SistemaFerremasContext _context;

    public ClienteController(IPasswordHasher<BaseUser> passwordHasher, SistemaFerremasContext context)
    {
        _passwordHasher = passwordHasher;
        _context = context;
    }

    // GET: api/clientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> ListarClientes()
    {
        try
        {
            return await _context.Clientes.ToListAsync();
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

    // GET: api/clientes/{rut}
    [HttpGet("{rut}")]
    public async Task<ActionResult<Cliente>> BuscarCliente(string rut)
    {
        var cliente = await _context.Clientes.FindAsync(rut);
        if (cliente == null)
        {
            return NotFound(new {
                mensaje = "No se ha encontrado el cliente"
            });
        }
        return cliente;
    }

    // POST: api/clientes
    [HttpPost]
    public async Task<ActionResult<Cliente>> AgregarCliente(Cliente cliente)
    {
        try
        {
            var cli = await _context.Clientes.FindAsync(cliente.RutCliente);
            if (cli != null)
            {
                return BadRequest(new {
                    mensaje = "Ya existe cliente con ese RUT"
                });
            }

            var clienteBase = new BaseUser { Email = cliente.Email};
            cliente.Password = _passwordHasher.HashPassword(clienteBase, cliente.Password);

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return StatusCode(201, new
            {
                mensaje = "Se ha creado el cliente con éxito!"
            });
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

    // PUT: api/clientes/{rut}
    [HttpPut("{rut}")]
    public async Task<IActionResult> ActualizarCliente(string rut, Cliente cliente)
    {
        try
        {
            if (rut != cliente.RutCliente)
            {
                return NotFound(new {
                    mensaje = "No se ha encontrado el cliente"
                });
            }
            var clienteBase = new BaseUser { Email = cliente.Email};
            cliente.Password = _passwordHasher.HashPassword(clienteBase, cliente.Password);
            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new 
            {
                mensaje = "Se ha actualizado el cliente con éxito!"
            });
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

    // DELETE: api/clientes/{rut}
    [HttpDelete("{rut}")]
    public async Task<IActionResult> EliminarCliente(string rut)
    {
        try
        {
            var cliente = await _context.Clientes.FindAsync(rut);
            if (cliente == null)
            {
                return NotFound(new {
                    mensaje = "No se ha encontrado el cliente"
                });
            }
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return Ok(new {
                mensaje = "Se ha eliminado el cliente!"
            });
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