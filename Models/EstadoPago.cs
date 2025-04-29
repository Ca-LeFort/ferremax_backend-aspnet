using System;
using System.Collections.Generic;

namespace ApiPrincipal_Ferremas.Models;

public partial class EstadoPago
{
    public int IdEstPago { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
