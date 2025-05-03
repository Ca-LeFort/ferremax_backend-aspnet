using ApiPrincipal_Ferremas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/clientes")]
[ApiController]
public class ClienteController : ControllerBase
{
    private readonly IPasswordHasher<BaseUser> _passwordHasher;
    private readonly SistemaFerremasContext _context;
    private readonly ClienteService _clienteService;

    public ClienteController(ClienteService clienteService,
                            IPasswordHasher<BaseUser> passwordHasher,
                            SistemaFerremasContext context)
    {
        _clienteService = clienteService;
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
            return NotFound(new
            {
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
                return BadRequest(new
                {
                    mensaje = "Ya existe cliente con ese RUT"
                });
            }
            var validaCliente = _clienteService.CrearCliente(cliente);
            if (!validaCliente.Exito)
            {
                return BadRequest(new
                {
                    mensaje = validaCliente.Mensaje
                });
            }

            var clienteBase = new BaseUser { Email = cliente.Email };
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
                return NotFound(new
                {
                    mensaje = "No se ha encontrado el cliente"
                });
            }
            var clienteBase = new BaseUser { Email = cliente.Email };
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
                return NotFound(new
                {
                    mensaje = "No se ha encontrado el cliente"
                });
            }
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return Ok(new
            {
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

    // PATCH: api/clientes/{rut}
    [HttpPatch("{rut}")]
    public async Task<IActionResult> ModificarClientePatch(string rut, [FromBody] ClienteDTO patchCliente)
    {
        try
        {
            var cliente = await _context.Clientes.FindAsync(rut);
            if (cliente == null)
            {
                return NotFound(new
                {
                    mensaje = "No se ha encontrado el cliente"
                });
            }

            // Actualización de datos existentes
            if (patchCliente.PNombre != null) cliente.PNombre = patchCliente.PNombre;
            if (patchCliente.SNombre != null) cliente.SNombre = patchCliente.SNombre;
            if (patchCliente.PApellido != null) cliente.PApellido = patchCliente.PApellido;
            if (patchCliente.SApellido != null) cliente.SApellido = patchCliente.SApellido;
            if (patchCliente.Email != null) cliente.Email = patchCliente.Email;
            if (patchCliente.Password != null) 
            {
                var clienteBase = new BaseUser { Email = patchCliente.Email };
                patchCliente.Password = _passwordHasher.HashPassword(clienteBase, patchCliente.Password);
                cliente.Password = patchCliente.Password;
            }
            if (patchCliente.Telefono != null) cliente.Telefono = (int)patchCliente.Telefono;
            if (patchCliente.Direccion != null) cliente.Direccion = patchCliente.Direccion;
            if (patchCliente.FechaNacimiento != null) cliente.FechaNacimiento = (DateOnly)patchCliente.FechaNacimiento;
            if (patchCliente.IdGenero != null) cliente.IdGenero = patchCliente.IdGenero;
            if (patchCliente.IdEstCivil != null) cliente.IdEstCivil = patchCliente.IdEstCivil;
            if (patchCliente.IdComuna != null) cliente.IdComuna = patchCliente.IdComuna;
            if (patchCliente.IdNotificacion != null) cliente.IdNotificacion = patchCliente.IdNotificacion;

            await _context.SaveChangesAsync();
            return Ok(new
            {
                mensaje = "Se ha actualizado parcialmente el Cliente"
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