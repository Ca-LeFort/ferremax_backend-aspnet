using ApiPrincipal_Ferremas.Models;
using ApiPrincipal_Ferremas.PATCHModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
                    .Select(p => new
                    {
                        p.IdPedido,
                        p.FechaPedido,
                        p.PrecioTotal,
                        Estado = _context.EstadoPedidos
                        .Where(e => e.IdEstPedido == p.IdEstPedido)
                        .Select(e => e.Nombre)
                        .FirstOrDefault(),
                    })
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
        /*[Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "EncargadoOnly")]
        [Authorize(Policy = "BodegueroOnly")]
        [Authorize(Policy = "ClienteOnly")]*/
        [HttpGet("{idPedido}")]
        public async Task<IActionResult> ObtenerPedido(int idPedido)
        {
            var pedido = await _context.Pedidos
                .Where(p => p.IdPedido == idPedido)
                .Select(p => new
                {
                    p.IdPedido,
                    p.RutCliente,
                    p.FechaPedido,
                    p.PrecioTotal,
                    Estado = _context.EstadoPedidos
                        .Where(e => e.IdEstPedido == p.IdEstPedido)
                        .Select(e => e.Nombre)
                        .FirstOrDefault(),

                    Productos = _context.ProductoCarritos
                        .Where(pc => pc.IdCarrito == p.IdCarrito)
                        .Select(pc => new
                        {
                            pc.Cantidad,
                            NombreProducto = _context.Productos
                                .Where(prod => prod.IdProducto == pc.IdProducto)
                                .Select(prod => prod.Nombre)
                                .FirstOrDefault()
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (pedido == null)
                return NotFound("Pedido no encontrado");

            return Ok(pedido);
        }

        // POST: api/pedidos/crear
        [Authorize(Policy = "ClienteOnly")]
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

                carrito.Estado = "Procesado";

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                return StatusCode(201, new
                {
                    mensaje = "Pedido creado",
                    idPedido = pedido.IdPedido,
                    monto = pedido.PrecioTotal
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

                return Ok(new
                {
                    mensaje = "Pedido actualizado"
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

        // GET: api/pedidos/reportes
        [HttpGet("reportes")]
        public IActionResult GenerarReportePedidos()
        {
            var pedidos = _context.Pedidos.ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Título
                Font tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Paragraph titulo = new Paragraph("Reporte de Pedidos", tituloFont);
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);
                doc.Add(new Paragraph("\nFecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")));

                // Tabla de pedidos
                PdfPTable table = new PdfPTable(4); // 4 columnas: ID, Cliente, Fecha, Total
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1f, 3f, 2f, 2f });

                // Encabezados
                string[] headers = { "ID", "RUT Cliente", "Fecha", "Total" };
                foreach (var header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, FontFactory.GetFont(FontFactory.HELVETICA_BOLD)));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                }

                // Datos de pedidos
                foreach (var pedido in pedidos)
                {
                    table.AddCell(pedido.IdPedido.ToString());
                    table.AddCell(pedido.RutCliente);
                    table.AddCell(pedido.FechaPedido.ToString("dd/MM/yyyy"));
                    table.AddCell(pedido.PrecioTotal.ToString("C"));
                }

                doc.Add(table);
                doc.Close();
                return File(ms.ToArray(), "application/pdf", "Reporte_Pedidos.pdf");
            }
        }
    }
}
