using System;
using System.Collections.Generic;

namespace ApiPrincipal_Ferremas.Models;

public partial class Notificacion
{
    public int IdNotificacion { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
