namespace ApiPrincipal_Ferremas.PATCHModels
{
    public class PagoDTO
    {
        public int? Monto { get; set; }

        public string? Referencia { get; set; }

        public int? IdPedido { get; set; }

        public int? IdMedioPago { get; set; }

        public int? IdEstPago { get; set; }
    }
}
