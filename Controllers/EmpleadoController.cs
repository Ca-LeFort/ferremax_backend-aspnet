using ApiPrincipal_Ferremas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/empleados")]
[ApiController]
public class EmpleadoController : ControllerBase
{
    private readonly EmpleadoService _empleadoService;
    private readonly IPasswordHasher<BaseUser> _passwordHasher;
    private readonly SistemaFerremasContext _context;

    public EmpleadoController(EmpleadoService empleadoService,
                            IPasswordHasher<BaseUser> passwordHasher,
                            SistemaFerremasContext context)
    {
        _empleadoService = empleadoService;
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

    // GET: api/empleados/{rut}
    [HttpGet("{rut}")]
    public async Task<ActionResult<Empleado>> BuscarEmpleado(string rut)
    {
        try
        {
            var empleado = await _context.Empleados.FindAsync(rut);
            if (empleado == null)
            {
                return NotFound(new
                {
                    mensaje = "No se ha encontrado el empleado"
                });
            }
            return empleado;
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

    // POST: api/empleados
    [HttpPost]
    public async Task<ActionResult<Empleado>> AgregarEmpleado(Empleado empleado)
    {
        try
        {
            // Verificar si existe Empleado con el RUT
            var emp = await _context.Empleados.FindAsync(empleado.RutEmpleado);
            if (emp != null)
            {
                return BadRequest(new
                {
                    mensaje = "Ya existe empleado con ese RUT"
                });
            }
            // Validación de datos
            var validaEmpleado = _empleadoService.CrearEmpleado(empleado);
            if (!validaEmpleado.Exito)
            {
                return BadRequest(new
                {
                    mensaje = validaEmpleado.Mensaje
                });
            }
            var empBase = new BaseUser { Email = empleado.Email };
            empleado.Password = _passwordHasher.HashPassword(empBase, empleado.Password);

            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            // Lógica de negocio: Si el empleado es un Administrador, debe almacenar en una tabla a parte
            if (empleado.IdTipoEmp == 1)
            {
                string sql = "INSERT INTO sesiones_admin VALUES ({0}, {1}, {2})";
                await _context.Database.ExecuteSqlRawAsync(sql, empleado.RutEmpleado, empleado.Email, 0);
            }

            return StatusCode(201, new
            {
                mensaje = "Se ha creado el empleado con éxito!"
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

    // PUT: api/empleados/{rut}
    [HttpPut("{rut}")]
    public async Task<IActionResult> ActualizarEmpleado(string rut, Empleado empleado)
    {
        try
        {
            if (rut != empleado.RutEmpleado)
            {
                return NotFound(new
                {
                    mensaje = "No se ha encontrado el empleado"
                });
            }
            // Validación de datos
            var validaEmpleado = _empleadoService.CrearEmpleado(empleado);
            if (!validaEmpleado.Exito)
            {
                return BadRequest(new
                {
                    mensaje = validaEmpleado.Mensaje
                });
            }
            var empBase = new BaseUser { Email = empleado.Email };
            empleado.Password = _passwordHasher.HashPassword(empBase, empleado.Password);

            _context.Entry(empleado).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Lógica de negocio: Si el empleado es un Administrador, debe modificar el correo de una tabla a parte
            if (empleado.IdTipoEmp == 1)
            {
                string sql = "UPDATE sesiones_admin SET email = {0} WHERE rut_emp = {1}";
                await _context.Database.ExecuteSqlRawAsync(sql, empleado.Email, empleado.RutEmpleado);
            }

            return Ok(new
            {
                mensaje = "Se ha actualizado el empleado con éxito!"
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

    // DELETE: api/empleados/{rut}
    [HttpDelete("{rut}")]
    public async Task<IActionResult> EliminarEmpleado(string rut)
    {
        try
        {
            var empleado = await _context.Empleados.FindAsync(rut);
            if (empleado == null)
            {
                return NotFound(new
                {
                    mensaje = "No se ha encontrado el empleado"
                });
            }
            _context.Empleados.Remove(empleado);
            await _context.SaveChangesAsync();

            // Lógica de negocio: Si el empleado es un Administrador, debe eliminar de una tabla a parte
            if (empleado.IdTipoEmp == 1)
            {
                string sql = "DELETE FROM sesiones_admin WHERE rut_emp = {0}";
                await _context.Database.ExecuteSqlRawAsync(sql, empleado.RutEmpleado);
            }

            return Ok(new
            {
                mensaje = "Se ha eliminado el empleado!"
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

    // PATCH: api/empleados/{rut}
    [HttpPatch("{rut}")]
    public async Task<IActionResult> ModificarEmpleadoPatch(string rut, [FromBody] EmpleadoDTO patchEmpleado)
    {
        try
        {
            var empleado = await _context.Empleados.FindAsync(rut);
            if (empleado == null)
            {
                return NotFound(new
                {
                    mensaje = "No se ha encontrado el empleado"
                });
            }

            // Actualización de datos existentes
            if (patchEmpleado.PNombre != null) empleado.PNombre = patchEmpleado.PNombre;
            if (patchEmpleado.SNombre != null) empleado.SNombre = patchEmpleado.SNombre;
            if (patchEmpleado.PApellido != null) empleado.PApellido = patchEmpleado.PApellido;
            if (patchEmpleado.SApellido != null) empleado.SApellido = patchEmpleado.SApellido;
            if (patchEmpleado.Email != null) 
            { 
                empleado.Email = patchEmpleado.Email; 
                // Lógica de negocio: Si el empleado es un Administrador, debe modificar el correo de una tabla a parte
                if (patchEmpleado.IdTipoEmp == 1)
                {
                    string sql = "UPDATE sesiones_admin SET email = {0} WHERE rut_emp = {1}";
                    await _context.Database.ExecuteSqlRawAsync(sql, empleado.Email, empleado.RutEmpleado);
                }
            }
            if (patchEmpleado.Password != null)
            {
                var clienteBase = new BaseUser { Email = patchEmpleado.Email };
                patchEmpleado.Password = _passwordHasher.HashPassword(clienteBase, patchEmpleado.Password);
                empleado.Password = patchEmpleado.Password;
            }
            if (patchEmpleado.Telefono != null) empleado.Telefono = (int)patchEmpleado.Telefono;
            if (patchEmpleado.Direccion != null) empleado.Direccion = patchEmpleado.Direccion;
            if (patchEmpleado.FechaNacimiento != null) empleado.FechaNacimiento = (DateOnly)patchEmpleado.FechaNacimiento;
            if (patchEmpleado.IdGenero != null) empleado.IdGenero = patchEmpleado.IdGenero;
            if (patchEmpleado.IdEstCivil != null) empleado.IdEstCivil = patchEmpleado.IdEstCivil;
            if (patchEmpleado.IdComuna != null) empleado.IdComuna = patchEmpleado.IdComuna;
            if (patchEmpleado.IdTipoEmp != null) empleado.IdTipoEmp = patchEmpleado.IdTipoEmp;

            await _context.SaveChangesAsync();
            return Ok(new
            {
                mensaje = "Se ha actualizado parcialmente el Empleado"
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