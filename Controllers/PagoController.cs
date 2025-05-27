using ApiPrincipal_Ferremas.Models;
using ApiPrincipal_Ferremas.PATCHModels;
using ApiPrincipal_Ferremas.Services;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
