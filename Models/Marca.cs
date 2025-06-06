﻿using System;
using System.Collections.Generic;

namespace ApiPrincipal_Ferremas.Models;

public partial class Marca
{
    public int IdMarca { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
