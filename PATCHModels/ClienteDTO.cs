public class ClienteDTO
{
    public string? PNombre { get; set; }
    public string? SNombre { get; set; }
    public string? PApellido { get; set; }
    public string? SApellido { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public int? Telefono { get; set; }
    public string? Direccion { get; set; }
    public DateOnly? FechaNacimiento { get; set; }

    public int? IdGenero { get; set; }

    public int? IdEstCivil { get; set; }

    public int? IdComuna { get; set; }

    public int? IdNotificacion { get; set; }
}