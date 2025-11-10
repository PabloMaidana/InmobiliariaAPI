namespace InmobilariaWebAPI.Models
{
    public class Inmueble
    {
        public int Id { get; set; }
        public string Direccion { get; set; } = "";
        public int CantidadAmbientes { get; set; }
        public string Tipo { get; set; } = "";
        public string Uso { get; set; } = "";
        public decimal Precio { get; set; }
        public bool Disponible { get; set; }
        public int PropietarioId { get; set; }
    }
}