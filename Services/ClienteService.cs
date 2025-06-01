using System.Text.RegularExpressions;
using ApiPrincipal_Ferremas.Models;

public class ClienteService : InterfaceService
{
    public ResultadoOperacionService CrearCliente(Cliente cliente)
    {
        // Validación de largos de Nombre y Apellido
        if (cliente.PNombre.Length < 3)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "El nombre debe tener a lo menos 3 caracteres"
            };
        }

        if (cliente.PApellido.Length < 3)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "Los apellidos debe tener a lo menos 3 caracteres"
            };
        }

        if (cliente.SApellido.Length < 3)
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
        if (cliente.Password.Length < 8)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "La contraseña debe tener a lo menos 8 caracteres"
            };
        }

        // Validación del teléfono
        var ValidaTelefono = cliente.Telefono.ToString();
        if (ValidaTelefono.Length <= 8 && ValidaTelefono.Length >= 10)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "El número de teléfono debe tener 9 dígitos (empezando con 9)"
            };
        }

        // Validación de la dirección
        if (cliente.Direccion.Length < 5)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "La dirección debe tener a lo menos 5 caracteres"
            };
        }

        // Validación de la fecha de nacimiento (Edad mínimo: 18)
        if ((DateTime.Today.Year - cliente.FechaNacimiento.Year) < 18)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "Debes tener al menos 18 años para registrarte"
            };
        }

        return new ResultadoOperacionService
        {
            Exito = true
        };
    }
}