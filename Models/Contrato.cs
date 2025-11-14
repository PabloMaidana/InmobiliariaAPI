namespace Inmobiliaria.Api.Models;
public class Contrato {
    public int IdContrato { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFinalizacion { get; set; }
    public decimal MontoAlquiler { get; set; }
    public bool Estado { get; set; }

    public int IdInquilino { get; set; }
    public Inquilino? Inquilino { get; set; }

    public int IdInmueble { get; set; }
    public Inmueble? Inmueble { get; set; }

    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}