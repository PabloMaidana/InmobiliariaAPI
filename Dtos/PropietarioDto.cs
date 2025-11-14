namespace Inmobiliaria.Api.Dtos;
public record PropietarioDto(
    int IdPropietario, string Nombre, string Apellido, string Dni,
    string Telefono, string Email, string Clave
);