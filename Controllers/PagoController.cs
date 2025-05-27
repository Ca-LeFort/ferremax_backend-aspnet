using ApiPrincipal_Ferremas.Models;
using ApiPrincipal_Ferremas.PATCHModels;
using ApiPrincipal_Ferremas.Services;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

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
        //[Authorize(Policy = "ClienteOnly")]
        [HttpPost("crear")]
        public async Task<IActionResult> CrearPago()
        {
            try
            {
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
                            UnitPrice = 25000
                        }
                    },
                    BackUrls = new PreferenceBackUrlsRequest
                    {
                        Success = "https://localhost:7007/api/pagos/Success",
                        Failure = "https://localhost:7007/api/pagos/Failure",
                        Pending = "https://localhost:7007/api/pagos/Pending"
                    },
                    AutoReturn = "approved",
                    PaymentMethods = new PreferencePaymentMethodsRequest
                    {
                        ExcludedPaymentMethods = [],
                        ExcludedPaymentTypes = [],
                        Installments = 6 // Cantidad de cuotas que aceptará según el negocio
                    }
                };

                var client = new PreferenceClient();
                Preference pref = await client.CreateAsync(preference);

                return Ok(new { id = pref.Id, init_point = pref.InitPoint });
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
        public IActionResult Success([FromQuery] PagoResponse pagoResponse)
        {
            if (pagoResponse == null || string.IsNullOrEmpty(pagoResponse.payment_id))
            {
                return BadRequest("Los datos de pago no fueron proporcionados correctamente.");
            }

            return new JsonResult(new
            {
                CollectionId = pagoResponse.collection_id,
                CollectionStatus = pagoResponse.collection_status,
                PaymentId = pagoResponse.payment_id,
                Status = pagoResponse.status,
                ExternalReference = pagoResponse.external_reference,
                PaymentType = pagoResponse.payment_type,
                MerchantOrderId = pagoResponse.merchant_order_id,
                PreferenceId = pagoResponse.preference_id,
                SiteId = pagoResponse.site_id,
                ProcessingMode = pagoResponse.processing_mode,
                MerchantAccountId = pagoResponse.merchant_account_id
            });
        }

        [HttpGet("Failure")]
        public IActionResult Failure([FromQuery] PagoResponse pagoResponse)
        {
            if (pagoResponse == null || string.IsNullOrEmpty(pagoResponse.payment_id))
                return BadRequest("Los datos de pago no fueron proporcionados correctamente.");

            return new JsonResult(new
            {
                CollectionId = pagoResponse.collection_id,
                CollectionStatus = pagoResponse.collection_status,
                PaymentId = pagoResponse.payment_id,
                Status = pagoResponse.status,
                ExternalReference = pagoResponse.external_reference,
                PaymentType = pagoResponse.payment_type,
                MerchantOrderId = pagoResponse.merchant_order_id,
                PreferenceId = pagoResponse.preference_id,
                SiteId = pagoResponse.site_id,
                ProcessingMode = pagoResponse.processing_mode,
                MerchantAccountId = pagoResponse.merchant_account_id
            });
        }

        [HttpGet("Pending")]
        public IActionResult Pending([FromQuery] PagoResponse pagoResponse)
        {
            if (pagoResponse == null || string.IsNullOrEmpty(pagoResponse.payment_id))
                return BadRequest("Los datos de pago no fueron proporcionados correctamente.");

            return new JsonResult(new
            {
                CollectionId = pagoResponse.collection_id,
                CollectionStatus = pagoResponse.collection_status,
                PaymentId = pagoResponse.payment_id,
                Status = pagoResponse.status,
                ExternalReference = pagoResponse.external_reference,
                PaymentType = pagoResponse.payment_type,
                MerchantOrderId = pagoResponse.merchant_order_id,
                PreferenceId = pagoResponse.preference_id,
                SiteId = pagoResponse.site_id,
                ProcessingMode = pagoResponse.processing_mode,
                MerchantAccountId = pagoResponse.merchant_account_id
            });
        }

        /* FIN PROCESO DE PAGO CON MERCADO PAGO */

        // PROCESO DE PAGO MEDIANTE TRANSFERENCIA
        // POST: api/pagos/transferencia
        [Authorize(Policy = "ClienteOnly")]
        [HttpPost("transferencia")]
        public async Task<IActionResult> CrearPagoTransferencia([FromBody] PagoDTO request)
        {
            try
            {
                return Ok();
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
        [Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "ContadorOnly")]
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

        // GET: api/pagos/mis-pagos
        [Authorize(Policy = "ClienteOnly")]
        [HttpGet("mis-pagos")]
        public async Task<IActionResult> ListarPagosCliente()
        {
            try
            {
                var identidad = HttpContext.User.Identity as ClaimsIdentity;
                var rutCliente = identidad?.FindFirst("rut")?.Value;

                if (string.IsNullOrWhiteSpace(rutCliente))
                {
                    return Unauthorized("El usuario debe estar autenticado");
                }

                // Sentencia SQL

                return Ok();
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
        [Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "ClienteOnly")]
        [Authorize(Policy = "ContadorOnly")]
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
        [Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "ContadorOnly")]
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
        [Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "ContadorOnly")]
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
    }
}
