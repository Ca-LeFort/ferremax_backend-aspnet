﻿using System;
using System.Collections.Generic;

namespace ApiPrincipal_Ferremas.Models;

public partial class EstadoCivil
{
    public int IdEstCivil { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
