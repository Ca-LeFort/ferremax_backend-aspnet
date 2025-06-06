﻿using System;
using System.Collections.Generic;

namespace ApiPrincipal_Ferremas.Models;

public partial class Despacho
{
    public int IdDespacho { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
