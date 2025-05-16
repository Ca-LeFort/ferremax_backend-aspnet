using ApiPrincipal_Ferremas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApiPrincipal_Ferremas.Controllers
{
    [Route("api/carrito")]
    [ApiController]
    public class CarritoController : ControllerBase
    {
        private readonly SistemaFerremasContext _context;

        public CarritoController(SistemaFerremasContext context)
        {
            _context = context;
        }

        // Primer paso: Agregar el carrito de compras antes de que agregue el producto al carrito
        // POST: api/carrito/crear
        [HttpPost("crear")]
        public async Task<IActionResult> crearCarrito()
        {
            var identidad = HttpContext.User.Identity as ClaimsIdentity;
            var rutCliente = identidad?.FindFirst("rut_cliente")?.Value;

            if (string.IsNullOrWhiteSpace(rutCliente))
            {
                return Unauthorized("El usuario debe estar autenticado");
            }

            var nuevoCarrito = new Carrito
            {
                FechaCreacion = DateOnly.FromDateTime(DateTime.Now),
                Estado = "Activo",
                RutCliente = rutCliente
            };

            _context.Carritos.Add(nuevoCarrito);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/carrito/{carritoId}
        [HttpPost("{carritoId}/agregar-producto")]
        public IActionResult agregarProducto(int carritoId, int productoId, int cantidad)
        {
            var productoCarrito = _context.ProductoCarritos
                .FirstOrDefault(pc => pc.IdCarrito == carritoId && pc.IdProducto == productoId);

            if (productoCarrito != null)
            {
                productoCarrito.Cantidad += cantidad;
            }
            else
            {
                productoCarrito = new ProductoCarrito
                {
                    IdCarrito = carritoId,
                    IdProducto = productoId,
                    Cantidad = cantidad
                };
                _context.ProductoCarritos.Add(productoCarrito);
            }
            _context.SaveChanges();
            return Ok("Producto agregado al carrito");
        }

        // GET: api/carrito/mi-carrito
        [HttpGet("mi-carrito")]
        public async Task<IActionResult> VerCarrito()
        {
            // Rescatamos el RUT del Cliente desde el token generado
            var identificador = HttpContext.User.Identity as ClaimsIdentity;
            var rutCliente = identificador?.FindFirst("rut-cliente")?.Value;

            if (string.IsNullOrEmpty(rutCliente))
            {
                return Unauthorized("El usuario debe estar autenticado o sesión iniciada");
            }

            // Ahora buscamos el carrito activo por el cliente autentificado
            var carrito = _context.Carritos
                .Include(c => c.ProductoCarritos)
                .ThenInclude(pc => pc.IdProductoNavigation)
                .FirstOrDefault(c => c.RutCliente == rutCliente && c.Estado == "Activo");

            if (carrito == null)
            {
                return NotFound("No se encontró el carrito activo para este cliente");
            }

            return Ok(carrito);
        }
    }
}
