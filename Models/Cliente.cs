using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiPrincipal_Ferremas.Models;

public partial class Cliente
{
    [Key]
    public string RutCliente { get; set; } = null!;
    public string PNombre { get; set; } = null!;
    public string? SNombre { get; set; }
    public string PApellido { get; set; } = null!;
    public string SApellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int Telefono { get; set; }
    public string Direccion { get; set; } = null!;
    public DateOnly FechaNacimiento { get; set; }

    public int? IdGenero { get; set; }

    public int? IdEstCivil { get; set; }

    public int? IdComuna { get; set; }

    public int? IdNotificacion { get; set; }

    public virtual ICollection<Carrito> Carritos { get; set; } = new List<Carrito>();

    public virtual Comuna? IdComunaNavigation { get; set; }

    public virtual EstadoCivil? IdEstCivilNavigation { get; set; }

    public virtual Genero? IdGeneroNavigation { get; set; }

    public virtual Notificacion? IdNotificacionNavigation { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
