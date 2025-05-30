using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApiPrincipal_Ferremas.Services
{
    public class ResendEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apikey;

        public ResendEmailService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apikey = config["Resend:ApiKey"] ?? "";

            _httpClient.BaseAddress = new Uri("https://api.resend.com");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apikey);
        }

        public async Task EnviarEmailAsync(string destinatario, string asunto, string mensaje)
        {
            var datosEmail = new
            {
                from = "Acme <onboarding@resend.dev>",
                to = destinatario,
                subject = asunto,
                text = mensaje
            };

            var contenido = new StringContent(JsonSerializer.Serialize(datosEmail), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/emails", contenido);

            response.EnsureSuccessStatusCode();
        }
    }
}
