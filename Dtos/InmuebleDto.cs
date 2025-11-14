using Inmobiliaria.Api.Models;
namespace Inmobiliaria.Api.Dtos;

public record InmuebleDto(
    int IdInmueble, string Direccion, string Uso, string Tipo, int Ambientes,
    double Superficie, double Latitud, double Valor, string? Imagen, bool Disponible,
    double Longitud, int IdPropietario, PropietarioDto? Duenio, bool TieneContratoVigente
){
    public static InmuebleDto FromEntity(Inmueble i, bool vigente) =>
        new(i.IdInmueble, i.Direccion, i.Uso, i.Tipo, i.Ambientes, i.Superficie,
            i.Latitud, i.Valor, i.Imagen, i.Disponible, i.Longitud, i.IdPropietario,
            i.Duenio is null ? null :
                new PropietarioDto(i.Duenio.IdPropietario, i.Duenio.Nombre, i.Duenio.Apellido,
                    i.Duenio.Dni, i.Duenio.Telefono, i.Duenio.Email, i.Duenio.ClaveHash),
            vigente);
}