using System;
using System.Collections.Generic;

namespace ApiPrincipal_Ferremas.Models;

public partial class Comuna
{
    public int IdComuna { get; set; }

    public string Nombre { get; set; } = null!;

    public int? IdRegion { get; set; }

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

    public virtual Region? IdRegionNavigation { get; set; }
}
