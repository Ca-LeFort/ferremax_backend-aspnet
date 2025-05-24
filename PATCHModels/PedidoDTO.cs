namespace ApiPrincipal_Ferremas.PATCHModels
{
    public class PedidoDTO
    {
        public int? PrecioTotal { get; set; }

        public int? IdCarrito { get; set; }

        public string? RutCliente { get; set; }

        public int? IdEstPedido { get; set; }

        public int? IdDespacho { get; set; }

        public int? IdSucursal { get; set; }
    }
}
