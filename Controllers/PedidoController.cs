using ApiPrincipal_Ferremas.Models;
using ApiPrincipal_Ferremas.PATCHModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace ApiPrincipal_Ferremas.Controllers
{
    //[Authorize]
    [Route("api/pedidos")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly SistemaFerremasContext _context;

        public PedidoController(SistemaFerremasContext context)
        {
            _context = context;
        }

        // GET: api/pedidos/todos (sólo empleados mencionadas)
        /*[Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "EncargadoOnly")]
        [Authorize(Policy = "BodegueroOnly")]*/
        [HttpGet("todos")]
        public async Task<IActionResult> ListarPedidos()
        {
            try
            {
                var pedidos = await _context.Pedidos
                    .Include(p => p.IdCarritoNavigation)
                    .ThenInclude(c => c.ProductoCarritos)
                    .ThenInclude(pc => pc.IdProductoNavigation)
                    .OrderByDescending(p => p.FechaPedido)
                    .ToListAsync();

                return Ok(pedidos);
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

        // GET: api/pedidos/mis-pedidos (los clientes pueden ver SUS pedidos)
        [Authorize(Policy = "ClienteOnly")]
        [HttpGet("mis-pedidos")]
        public async Task<IActionResult> ListarPedidosCliente()
        {
            try
            {
                var identidad = HttpContext.User.Identity as ClaimsIdentity;
                var rutCliente = identidad?.FindFirst("rut")?.Value;

                if (string.IsNullOrWhiteSpace(rutCliente))
                {
                    return Unauthorized("El usuario debe estar autenticado");
                }

                var pedidos = await _context.Pedidos
                    .Where(p => p.RutCliente == rutCliente)
                    .OrderByDescending(p => p.FechaPedido)
                    .ToListAsync();

                return Ok(pedidos);
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

        // GET: api/pedidos/{idPedido}
        [Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "EncargadoOnly")]
        [Authorize(Policy = "BodegueroOnly")]
        [Authorize(Policy = "ClienteOnly")]
        [HttpGet("{idPedido}")]
        public async Task<ActionResult<Pedido>> VerPedido(int idPedido)
        {
            try
            {
                var pedido = await _context.Pedidos.FindAsync(idPedido);
                if (pedido == null)
                {
                    return NotFound(new
                    {
                        mensaje = "No se ha encontrado el pedido"
                    });
                }

                return Ok(pedido);
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

        // POST: api/pedidos/crear
        //[Authorize(Policy = "ClienteOnly")]
        [HttpPost("crear")]
        public async Task<IActionResult> CrearPedido([FromBody] PedidoDTO request)
        {
            try
            {
                var identidad = HttpContext.User.Identity as ClaimsIdentity;
                var rutCliente = identidad?.FindFirst("rut")?.Value;

                if (string.IsNullOrWhiteSpace(rutCliente))
                {
                    return Unauthorized("El usuario debe estar autenticado");
                }

                var carrito = await _context.Carritos
                    .FirstOrDefaultAsync(c => c.RutCliente == rutCliente && c.Estado == "Activo");

                if (carrito == null)
                {
                    return NotFound("No se encontró un carrito activo.");
                }

                var pedido = new Pedido
                {
                    FechaPedido = DateTime.Now,
                    IdCarrito = carrito.IdCarrito,
                    RutCliente = rutCliente,
                    IdEstPedido = 1,
                    IdDespacho = request.IdDespacho,
                    IdSucursal = request.IdSucursal,
                    PrecioTotal = request.PrecioTotal
                };

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                return StatusCode(201, "Pedido creado");
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

        // PUT: api/pedidos/{idPedido}
        /*[Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "EncargadoOnly")]
        [Authorize(Policy = "BodegueroOnly")]*/
        [HttpPut("{idPedido}")]
        public async Task<IActionResult> ActualizarPedido(int idPedido, [FromBody] PedidoDTO request)
        {
            try
            {
                var pedido = await _context.Pedidos.FindAsync(idPedido);
                if (pedido == null)
                {
                    return NotFound(new 
                    { 
                        mensaje = "Pedido no encontrado" 
                    });
                }

                pedido.IdEstPedido = request.IdEstPedido;
                await _context.SaveChangesAsync();

                return Ok("Pedido actualizado");
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

        // DELETE: api/pedidos/{idPedido}
        /*[Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "EncargadoOnly")]
        [Authorize(Policy = "BodegueroOnly")]*/
        [HttpDelete("{idPedido}")]
        public async Task<IActionResult> EliminarPedido(int idPedido)
        {
            try
            {
                var pedido = await _context.Pedidos.FindAsync(idPedido);
                if (pedido == null)
                {
                    return NotFound(new 
                    { 
                        mensaje = "Pedido no encontrado" 
                    });
                }

                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    mensaje = "Pedido eliminado" 
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
}
