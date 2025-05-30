using ApiPrincipal_Ferremas.Models;
using ApiPrincipal_Ferremas.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiPrincipal_Ferremas.Controllers
{
    [Route("api/notificacion")]
    [ApiController]
    public class NotificacionController : ControllerBase
    {
        private readonly SistemaFerremasContext _context;
        private readonly ResendEmailService _emailService;

        public NotificacionController(SistemaFerremasContext context, ResendEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // POST: api/notificacion/enviar-notificacion
        [HttpPost("enviar-notificacion")]
        public async Task<IActionResult> EnviarNotificacion([FromServices] ResendEmailService emailService)
        {
            try
            {
                var clientes = await _context.Clientes
                    .Where(c => c.IdNotificacion == 1)
                    .ToListAsync();

                string asunto = "Oferta exclusiva";
                string mensaje = "Tenemos un 40% de descuento si agregas al menos 4 productos en el carrito de compras";

                foreach (var cliente in clientes)
                {
                    await emailService.EnviarEmailAsync(cliente.Email, asunto, mensaje);
                }

                return Ok($"Se enviaron notificaciones a {clientes.Count} clientes");
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
