using System.Text.RegularExpressions;
using ApiPrincipal_Ferremas.Models;

public class EmpleadoService
{
    public ResultadoOperacionService CrearEmpleado(Empleado empleado)
    {
        // Validación de Nombres y Apellidos
        if (empleado.PNombre.Length <= 3)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "El nombre debe tener a lo menos 3 caracteres"
            };
        }

        if (empleado.PApellido.Length <= 3)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "Los apellidos debe tener a lo menos 3 caracteres"
            };
        }

        if (empleado.SApellido.Length <= 3)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "Los apellidos debe tener a lo menos 3 caracteres"
            };
        }

        // Validación de email
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(empleado.Email))
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "Formato de email, no es válido"
            };
        }

        // Validación de dominio email
        var dominio = empleado.Email.Split('@').Last().ToLower();
        if (dominio != "ferremas.cl")
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "Se requiere un dominio institucional para crear/modificar un Empleado"
            };
        }

        // Validación de contraseña
        if (empleado.Password.Length < 8)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "La contraseña debe tener a lo menos 8 caracteres"
            };
        }

        // Validación del teléfono
        var ValidaTelefono = empleado.Telefono.ToString();
        if (ValidaTelefono.Length <= 8 && ValidaTelefono.Length >= 10)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "El número de teléfono debe tener 9 dígitos (empezando con 9)"
            };
        }

        // Validación de la dirección
        if (empleado.Direccion.Length <= 5)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "La dirección debe tener a lo menos 5 caracteres"
            };
        }

        // Validación de la fecha de nacimiento (Edad mínimo: 18)
        if ((DateTime.Today.Year - empleado.FechaNacimiento.Year) < 18)
        {
            return new ResultadoOperacionService
            {
                Exito = false,
                Mensaje = "El empleado debe tener 18 años para registrar"
            };
        }

        return new ResultadoOperacionService
        {
            Exito = true
        };
    }
}