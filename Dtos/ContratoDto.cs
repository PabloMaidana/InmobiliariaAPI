using Inmobiliaria.Api.Models;
namespace Inmobiliaria.Api.Dtos;

public record InquilinoSimpleDto(int IdInquilino, string Nombre, string Apellido, string Dni, string Telefono, string Email);
public record InmuebleSimpleDto(int IdInmueble, string Direccion, string Uso, string Tipo, int Ambientes, double Superficie, double Latitud, double Valor, string? Imagen, bool Disponible, double Longitud, int IdPropietario, PropietarioDto? Duenio, bool TieneContratoVigente);

public record ContratoDto(
    int IdContrato, DateOnly FechaInicio, DateOnly FechaFinalizacion,
    decimal MontoAlquiler, bool Estado, int IdInquilino, int IdInmueble,
    InquilinoSimpleDto? Inquilino, InmuebleSimpleDto? Inmueble
){
    public static ContratoDto FromEntity(Contrato c, bool inmuebleTieneVigente) =>
        new(c.IdContrato, c.FechaInicio, c.FechaFinalizacion, c.MontoAlquiler, c.Estado,
            c.IdInquilino, c.IdInmueble,
            c.Inquilino is null ? null :
                new InquilinoSimpleDto(c.Inquilino.IdInquilino, c.Inquilino.Nombre, c.Inquilino.Apellido, c.Inquilino.Dni, c.Inquilino.Telefono, c.Inquilino.Email),
            c.Inmueble is null ? null :
                new InmuebleSimpleDto(
                    c.Inmueble.IdInmueble, c.Inmueble.Direccion, c.Inmueble.Uso, c.Inmueble.Tipo,
                    c.Inmueble.Ambientes, c.Inmueble.Superficie, c.Inmueble.Latitud, c.Inmueble.Valor,
                    c.Inmueble.Imagen, c.Inmueble.Disponible, c.Inmueble.Longitud, c.Inmueble.IdPropietario,
                    c.Inmueble.Duenio is null ? null :
                        new PropietarioDto(c.Inmueble.Duenio.IdPropietario, c.Inmueble.Duenio.Nombre, c.Inmueble.Duenio.Apellido, c.Inmueble.Duenio.Dni, c.Inmueble.Duenio.Telefono, c.Inmueble.Duenio.Email, c.Inmueble.Duenio.ClaveHash),
                    inmuebleTieneVigente));
}