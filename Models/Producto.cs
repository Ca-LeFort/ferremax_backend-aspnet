using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPrincipal_Ferremas.Models;

public partial class Producto
{
    [Column("id_producto")]
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public int Precio { get; set; }

    public int Stock { get; set; }

    public string ImagenUrl { get; set; } = null!;

    public int? IdMarca { get; set; }

    public int? IdTipoProd { get; set; }

    public virtual Marca? IdMarcaNavigation { get; set; }

    public virtual TipoProducto? IdTipoProdNavigation { get; set; }

    public virtual ICollection<ProductoCarrito> ProductoCarritos { get; set; } = new List<ProductoCarrito>();
}
