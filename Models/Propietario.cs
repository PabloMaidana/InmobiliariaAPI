namespace Inmobiliaria.Api.Models;
public class Propietario {
    public int IdPropietario { get; set; }
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Dni { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Email { get; set; } = "";
    public string ClaveHash { get; set; } = "";
    public byte[]? ClaveSalt { get; set; }
    public ICollection<Inmueble> Inmuebles { get; set; } = new List<Inmueble>();
}