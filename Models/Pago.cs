using System;
using System.Collections.Generic;

namespace ApiPrincipal_Ferremas.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public int Monto { get; set; }

    public DateOnly FechaPago { get; set; }

    public string? Referencia { get; set; }

    public int? IdPedido { get; set; }

    public int? IdMedioPago { get; set; }

    public int? IdEstPago { get; set; }

    public virtual EstadoPago? IdEstPagoNavigation { get; set; }

    public virtual MedioPago? IdMedioPagoNavigation { get; set; }

    public virtual Pedido? IdPedidoNavigation { get; set; }
}
