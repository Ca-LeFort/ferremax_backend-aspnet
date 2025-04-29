using System;
using System.Collections.Generic;

namespace ApiPrincipal_Ferremas.Models;

public partial class EstadoPedido
{
    public int IdEstPedido { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
