using System;
using System.Collections.Generic;

namespace ApiPrincipal_Ferremas.Models;

public partial class TipoEmpleado
{
    public int IdTipoEmp { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
