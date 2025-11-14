namespace Inmobiliaria.Api.Models;
public class Inquilino {
    public int IdInquilino { get; set; }    
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Dni { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Email { get; set; } = "";
    public ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
}