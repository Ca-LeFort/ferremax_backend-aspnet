﻿using ApiPrincipal_Ferremas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApiPrincipal_Ferremas.Controllers
{
    [Authorize(Policy = "ClienteOnly")]
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
        public async Task<IActionResult> CrearCarrito()
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
                var nuevoCarrito = new Carrito
                {
                    FechaCreacion = DateOnly.FromDateTime(DateTime.Now),
                    Estado = "Activo",
                    RutCliente = rutCliente
                };

                _context.Carritos.Add(nuevoCarrito);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Se agregó el carrito del cliente",
                    idCarrito = nuevoCarrito.IdCarrito
                });
            }

            return Ok(new
            {
                mensaje = "Se usará el carrito existente y activo",
                idCarrito = carrito.IdCarrito
            });
        }

        // GET: api/carrito/carrito-activo
        [Authorize(Policy = "ClienteOnly")]
        [HttpGet("carrito-activo")]
        public IActionResult ObtenerCarritoActivo()
        {
            var rutCliente = User.Claims.FirstOrDefault(c => c.Type == "rut")?.Value;

            if (rutCliente == null)
                return Unauthorized("No se encontró el RUT en el token.");

            var carrito = _context.Carritos
                .Include(c => c.ProductoCarritos)
                    .ThenInclude(pc => pc.IdProductoNavigation)
                .FirstOrDefault(c => c.RutCliente == rutCliente && c.Estado == "Activo");

            if (carrito == null)
                return NotFound("No se encontró un carrito activo.");

            return Ok(carrito);
        }

        // POST: api/carrito/{carritoId}/agregar-producto
        [HttpPost("{carritoId}/agregar-producto")]
        public async Task<IActionResult> agregarProducto(int carritoId, [FromBody] ProductoCarritoDTO request)
        {
            var productos = await _context.Productos.ToListAsync();
            Console.WriteLine($"Total de productos en la BD: {productos.Count}");

            foreach (var p in productos)
            {
                Console.WriteLine($"Producto encontrado: {p.IdProducto} - {p.Nombre}");
            }

            Console.WriteLine($"Producto ID recibido: {request.IdProducto}");
            Console.WriteLine($"Cantidad recibida: {request.cantidad}");

            int idProducto = Convert.ToInt32(request.IdProducto);
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.IdProducto == idProducto);

            Console.WriteLine($"Producto encontrado: {producto}");

            if (producto == null) 
            { 
                return NotFound("El producto no existe");
            }

            var productoCarrito = _context.ProductoCarritos
                .FirstOrDefault(pc => pc.IdCarrito == carritoId && pc.IdProducto == request.IdProducto);

            if (productoCarrito != null)
            {
                productoCarrito.Cantidad += request.cantidad;
            }
            else
            {
                productoCarrito = new ProductoCarrito
                {
                    IdCarrito = carritoId,
                    IdProducto = request.IdProducto,
                    IdProductoNavigation = producto,
                    Cantidad = request.cantidad
                };
                _context.ProductoCarritos.Add(productoCarrito);
            }
            
            await _context.SaveChangesAsync();
            return Ok(new
            {
                mensaje = "Producto agregado al carrito"
            });
        }

        // GET: api/carrito/mi-carrito
        [HttpGet("mi-carrito")]
        public async Task<IActionResult> VerCarrito()
        {
            // Rescatamos el RUT del Cliente desde el token generado
            var identificador = HttpContext.User.Identity as ClaimsIdentity;
            var rutCliente = identificador?.FindFirst("rut")?.Value;

            if (string.IsNullOrEmpty(rutCliente))
            {
                return Unauthorized("El usuario debe estar autenticado o sesión iniciada");
            }

            // Ahora buscamos el carrito activo por el cliente autentificado
            var carrito = await _context.Carritos
                .Include(c => c.ProductoCarritos)
                .ThenInclude(pc => pc.IdProductoNavigation)
                .FirstOrDefaultAsync(c => c.RutCliente == rutCliente && c.Estado == "Activo");

            if (carrito == null)
            {
                return NotFound("No se encontró el carrito activo para este cliente");
            }

            return Ok(carrito);
        }

        // PUT: api/carrito/{carritoId}/actualizar-producto
        [HttpPut("{carritoId}/actualizar-producto")]
        public async Task<IActionResult> ActualizarProdCarrito(int carritoId, [FromBody] ProductoCarritoDTO request)
        {
            try
            {
                var productoCarrito = await _context.ProductoCarritos
                    .FirstOrDefaultAsync(pc => pc.IdCarrito == carritoId && pc.IdProducto == request.IdProducto);

                if (productoCarrito == null)
                {
                    return NotFound("No se encuentra producto en el carrito");
                }

                if (request.nuevaCantidad <= 0)
                {
                    return BadRequest("La cantidad debe ser mayor a 0.");
                }

                productoCarrito.Cantidad = request.nuevaCantidad;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Cantidad de producto actualizada correctamente."
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

        // DELETE: api/carrito/{carritoId}/eliminar-carrito/{productoId}
        [HttpDelete("{carritoId}/eliminar-carrito/{productoId}")]
        public async Task<IActionResult> EliminarProdCarrito(int carritoId, int productoId)
        {
            try
            {
                var productoCarrito = await _context.ProductoCarritos
                    .FirstOrDefaultAsync(pc => pc.IdCarrito == carritoId && pc.IdProducto == productoId);

                if (productoCarrito == null)
                {
                    return NotFound("No se encuentra producto en el carrito");
                }

                _context.ProductoCarritos.Remove(productoCarrito);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Producto eliminado del carrito."
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
