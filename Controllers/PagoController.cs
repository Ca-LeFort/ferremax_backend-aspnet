using ApiPrincipal_Ferremas.Models;
using ApiPrincipal_Ferremas.PATCHModels;
using ApiPrincipal_Ferremas.Services;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ApiPrincipal_Ferremas.Controllers
{
    [Route("api/pagos")]
    [ApiController]
    public class PagoController : ControllerBase
    {
        private readonly SistemaFerremasContext _context;
        private readonly IConfiguration _config;

        public PagoController(SistemaFerremasContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        /* PROCESO DE PAGO CON MERCADO PAGO */

        // POST: api/pagos/crear
        [Authorize(Policy = "ClienteOnly")]
        [HttpPost("crear")]
        public async Task<IActionResult> CrearPagoMercadoPago([FromBody] PedidoDTO request)
        {
            try
            {
                var pago = await _context.Pagos
                .FirstOrDefaultAsync(p => p.IdPedido == request.IdPedido);

                var mensaje = "";

                if (pago == null)
                {
                    var pagoTarjeta = new Pago
                    {
                        Monto = request.PrecioTotal,
                        FechaPago = DateTime.Now,
                        Referencia = null,
                        IdPedido = request.IdPedido,
                        IdMedioPago = 1,
                        IdEstPago = 1
                    };

                    mensaje = "Se guardó de forma parcial en la base de datos";

                    _context.Pagos.Add(pagoTarjeta);
                    await _context.SaveChangesAsync();
                }
                else if (pago.IdEstPago == 1)
                {
                    mensaje = "El pago se encuentra pendiente del pago";
                }
                else
                {
                    return BadRequest(new
                    {
                        mensaje = "El pago se encuentra completado y/o rechazado"
                    });
                }

                // Token de Acceso a Mercado Pago
                MercadoPagoConfig.AccessToken = _config["MercadoPago:AccessToken"];

                // Request para la API externa de Mercado Pago
                var preference = new PreferenceRequest
                {
                    Items = new List<PreferenceItemRequest>
                    {
                        new PreferenceItemRequest
                        {
                            Title = "Productos de Ferremas",
                            Quantity = 1,
                            CurrencyId = "CLP",
                            UnitPrice = request.PrecioTotal
                        }
                    },
                    BackUrls = new PreferenceBackUrlsRequest
                    {
                        Success = "https://localhost:7007/api/pagos/Success",
                        Failure = "https://localhost:7007/api/pagos/Failure",
                        Pending = "https://localhost:7007/api/pagos/Pending"
                    },
                    AutoReturn = "approved",
                    ExternalReference = request.IdPedido.ToString(),
                    PaymentMethods = new PreferencePaymentMethodsRequest
                    {
                        ExcludedPaymentMethods = [],
                        ExcludedPaymentTypes = [],
                        Installments = 6 // Cantidad de cuotas que aceptará según el negocio
                    }
                };

                var client = new PreferenceClient();
                Preference pref = await client.CreateAsync(preference);

                return Ok(new 
                { 
                    status = mensaje, 
                    id = pref.Id, 
                    init_point = pref.InitPoint 
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

        // Endpoints para resultados de pagos
        [HttpGet("Success")]
        public async Task<IActionResult> Success([FromQuery] PagoResponse pagoResponse)
        {
            if (pagoResponse == null || string.IsNullOrEmpty(pagoResponse.payment_id))
            {
                return BadRequest("Los datos de pago no fueron proporcionados correctamente.");
            }

            int idPedido = int.Parse(pagoResponse.external_reference);

            var pago = await _context.Pagos
                .FirstOrDefaultAsync(p => p.IdPedido ==  idPedido);

            if (pago == null)
            {
                return NotFound(new
                {
                    mensaje = "Pago no encontrado"
                });
            }

            pago.IdEstPago = 3;
            pago.Referencia = pagoResponse.payment_id;
            await _context.SaveChangesAsync();

            return Redirect($"https://localhost:4200/comprobante/aprobado?pedido={idPedido}&ref={pagoResponse.payment_id}");
        }

        [HttpGet("Failure")]
        public async Task<IActionResult> Failure([FromQuery] PagoResponse pagoResponse)
        {
            if (pagoResponse == null || string.IsNullOrEmpty(pagoResponse.payment_id))
                return BadRequest("Los datos de pago no fueron proporcionados correctamente.");

            int idPedido = int.Parse(pagoResponse.external_reference);

            var pago = await _context.Pagos
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido);

            if (pago == null)
            {
                return NotFound(new
                {
                    mensaje = "Pago no encontrado"
                });
            }

            if (pagoResponse.status.Equals("null"))
            {
                pago.IdEstPago = 5;
                pago.Referencia = null;
                await _context.SaveChangesAsync();
            }
            else
            {
                pago.IdEstPago = 4;
                pago.Referencia = pagoResponse.payment_id;
                await _context.SaveChangesAsync();
            }

            return Redirect($"https://localhost:4200/comprobante/rechazado?pedido={idPedido}&status={pagoResponse.status}&ref={pagoResponse.payment_id}");
        }

        [HttpGet("Pending")]
        public async Task<IActionResult> Pending([FromQuery] PagoResponse pagoResponse)
        {
            if (pagoResponse == null || string.IsNullOrEmpty(pagoResponse.payment_id))
                return BadRequest("Los datos de pago no fueron proporcionados correctamente.");

            int idPedido = int.Parse(pagoResponse.external_reference);

            var pago = await _context.Pagos
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido);

            if (pago == null)
            {
                return NotFound(new
                {
                    mensaje = "Pago no encontrado"
                });
            }

            pago.Referencia = pagoResponse.payment_id;
            await _context.SaveChangesAsync();

            return Redirect($"https://localhost:4200/comprobante/pendiente?pedido={idPedido}&ref={pagoResponse.payment_id}");
        }

        /* FIN PROCESO DE PAGO CON MERCADO PAGO */

        // PROCESO DE PAGO MEDIANTE TRANSFERENCIA
        // POST: api/pagos/transferencia
        [Authorize(Policy = "ClienteOnly")]
        [HttpPost("transferencia")]
        public async Task<IActionResult> CrearPagoTransferencia([FromBody] Pedido request)
        {
            try
            {
                var pago = await _context.Pagos
                    .FirstOrDefaultAsync(p => p.IdPedido == request.IdPedido);

                var pagoTransferencia = new Pago
                {
                    Monto = request.PrecioTotal,
                    FechaPago = DateTime.Now,
                    Referencia = null,
                    IdPedido = request.IdPedido,
                    IdMedioPago = 2,
                    IdEstPago = 2
                };

                _context.Pagos.Add(pagoTransferencia);
                await _context.SaveChangesAsync();

                return StatusCode(201, new
                {
                    mensaje = "Pago por transferencia agregado"
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

        // GET: api/pagos/todos
        //[Authorize(Policy = "AdminOnly")]
        //[Authorize(Policy = "ContadorOnly")]
        [HttpGet("todos")]
        public async Task<ActionResult<IEnumerable<Pago>>> ListarPagos()
        {
            try
            {
                return await _context.Pagos.ToListAsync();
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

        // GET: api/pagos/{idPago}
        //[Authorize(Policy = "AdminOnly")]
        //[Authorize(Policy = "ContadorOnly")]
        [HttpGet("{idPago}")]
        public async Task<ActionResult> VerPago(int idPago)
        {
            try
            {
                var pago = await _context.Pagos.FindAsync(idPago);
                if (pago == null)
                {
                    return NotFound(new
                    {
                        mensaje = "Pago no encontrado"
                    });
                }

                return Ok(pago);
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

        // PUT: api/pagos/{idPago}
        //[Authorize(Policy = "AdminOnly")]
        //[Authorize(Policy = "ContadorOnly")]
        [HttpPut("{idPago}")]
        public async Task<IActionResult> ActualizarPago(int idPago, [FromBody] PagoDTO request)
        {
            try
            {
                var pago = await _context.Pagos.FindAsync(idPago);
                if (pago == null)
                {
                    return NotFound(new
                    {
                        mensaje = "Pago no encontrado"
                    });
                }

                pago.IdEstPago = request.IdEstPago;
                await _context.SaveChangesAsync();

                return Ok("Estado de Pago actualizado");
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

        // DELETE: api/pagos/{idPago}
        //[Authorize(Policy = "AdminOnly")]
        //[Authorize(Policy = "ContadorOnly")]
        [HttpDelete("{idPago}")]
        public async Task<IActionResult> EliminarPago(int idPago)
        {
            try
            {
                var pago = await _context.Pagos.FindAsync(idPago);
                if (pago == null)
                {
                    return NotFound(new
                    {
                        mensaje = "Pago no encontrado"
                    });
                }

                _context.Pagos.Remove(pago);
                await _context.SaveChangesAsync();

                return Ok("Pago eliminado correctamente");
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

        // GET: api/pagos/reportes
        [HttpGet("reportes")]
        public IActionResult GenerarReportePagos()
        {
            var pagos = _context.Pagos.ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Título
                Font tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Paragraph titulo = new Paragraph("Reporte de Pagos", tituloFont);
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);
                doc.Add(new Paragraph("\nFecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")));

                // Tabla de pedidos
                PdfPTable table = new PdfPTable(4); // 4 columnas: ID, Cliente, Fecha, Total
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1f, 3f, 2f, 2f });

                // Encabezados
                string[] headers = { "ID", "RUT Cliente", "Fecha", "Monto" };
                foreach (var header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, FontFactory.GetFont(FontFactory.HELVETICA_BOLD)));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                }

                // Datos de pagos
                foreach (var pago in pagos)
                {
                    table.AddCell(pago.IdPago.ToString());
                    table.AddCell(pago.IdPedido.ToString());
                    table.AddCell(pago.FechaPago.ToString("dd/MM/yyyy"));
                    table.AddCell(pago.Monto.ToString("C"));
                }

                doc.Add(table);
                doc.Close();
                return File(ms.ToArray(), "application/pdf", "Reporte_Pagos.pdf");
            }
        }
    }
}
