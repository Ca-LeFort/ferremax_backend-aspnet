using System;
using System.Collections.Generic;

namespace ApiPrincipal_Ferremas.Models;

public partial class Empleado
{
    public string RutEmpleado { get; set; } = null!;

    public string PNombre { get; set; } = null!;

    public string? SNombre { get; set; }

    public string PApellido { get; set; } = null!;

    public string SApellido { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Telefono { get; set; }

    public string Direccion { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public DateOnly FechaContrato { get; set; }

    public int? IdGenero { get; set; }

    public int? IdEstCivil { get; set; }

    public int? IdComuna { get; set; }

    public int? IdTipoEmp { get; set; }
    
    public int cambioPassword { get; set; }

    public virtual Comuna? IdComunaNavigation { get; set; }

    public virtual EstadoCivil? IdEstCivilNavigation { get; set; }

    public virtual Genero? IdGeneroNavigation { get; set; }

    public virtual TipoEmpleado? IdTipoEmpNavigation { get; set; }
}
