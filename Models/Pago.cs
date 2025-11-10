namespace InmobilariaWebAPI.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public int AlquilerId { get; set; }
        public int NroPago { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Importe { get; set; }
    }
}