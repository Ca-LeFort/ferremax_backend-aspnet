using System;
using System.Collections.Generic;

namespace ApiPrincipal_Ferremas.Models;

public partial class Pedido
{
    public int IdPedido { get; set; }

    public DateTime FechaPedido { get; set; }

    public int PrecioTotal { get; set; }

    public int? IdCarrito { get; set; }

    public string? RutCliente { get; set; }

    public int? IdEstPedido { get; set; }

    public int? IdDespacho { get; set; }

    public int? IdSucursal { get; set; }

    public virtual Carrito? IdCarritoNavigation { get; set; }

    public virtual Despacho? IdDespachoNavigation { get; set; }

    public virtual EstadoPedido? IdEstPedidoNavigation { get; set; }

    public virtual Sucursal? IdSucursalNavigation { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual Cliente? RutClienteNavigation { get; set; }
}
