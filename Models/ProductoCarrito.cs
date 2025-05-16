using System;
using System.Collections.Generic;

namespace ApiPrincipal_Ferremas.Models;

public partial class ProductoCarrito
{
    public int IdProducto { get; set; }

    public int IdCarrito { get; set; }

    public int Cantidad { get; set; }

    public virtual Carrito? IdCarritoNavigation { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }
}
