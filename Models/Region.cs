using System;
using System.Collections.Generic;

namespace ApiPrincipal_Ferremas.Models;

public partial class Region
{
    public int IdRegion { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Comuna> Comunas { get; set; } = new List<Comuna>();
}
