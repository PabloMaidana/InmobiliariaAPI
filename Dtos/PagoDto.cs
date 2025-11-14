using Inmobiliaria.Api.Models;
namespace Inmobiliaria.Api.Dtos;

public record PagoDto(
    int IdPago, DateOnly FechaPago, decimal Monto, string Detalle, bool Estado,
    int IdContrato, Contrato? Contrato
){
    public static PagoDto FromEntity(Pago p) =>
        new(p.IdPago, p.FechaPago, p.Monto, p.Detalle, p.Estado, p.IdContrato, p.Contrato);
}