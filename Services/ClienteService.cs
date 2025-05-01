using System.Text.RegularExpressions;
using ApiPrincipal_Ferremas.Models;

public class ClienteService : IClienteService
{
    public ResultadoOperacionService CrearCliente(Cliente cliente)
    {
        // Validación de largos de Nombre y Apellido
        if (cliente.PNombre.Length <= 3)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "El nombre debe tener a lo menos 3 caracteres"
            };
        }

        if (cliente.PApellido.Length <= 3 && cliente.SApellido.Length <= 3)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "Los apellidos debe tener a lo menos 3 caracteres"
            };
        }

        // Validación de email
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(cliente.Email))
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "Formato de email, no es válido"
            };
        }

        // Validación de contraseña
        

        return new ResultadoOperacionService
        {
            Exito = true
        };
    }
}