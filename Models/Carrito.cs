using System;
using System.Collections.Generic;

namespace ApiPrincipal_Ferremas.Models;

public partial class Carrito
{
    public int IdCarrito { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public string Estado { get; set; } = null!;

    public string? RutCliente { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public virtual ICollection<ProductoCarrito> ProductoCarritos { get; set; } = new List<ProductoCarrito>();

    public virtual Cliente? RutClienteNavigation { get; set; }
}
