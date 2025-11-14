using Inmobiliaria.Api.Models;
using Inmobiliaria.Api.Services;

namespace Inmobiliaria.Api.Data;
public static class DbSeeder {
    public static void Seed(WebApplication app) {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var pwd = scope.ServiceProvider.GetRequiredService<PasswordService>();

        db.Database.EnsureCreated();

        if (!db.Propietarios.Any()) {
            var (hash, salt) = pwd.HashPassword("DEEKQW");
            var p = new Propietario {
                Nombre = "el profe",
                Apellido = "Mercados Sosa",
                Dni = "39652369",
                Telefono = "11253333333",
                Email = "luisprofessor@gmail.com",
                ClaveHash = hash,
                ClaveSalt = salt
            };
            db.Propietarios.Add(p);
            db.SaveChanges();

            var inm = new Inmueble {
                Direccion = "Belgrano 812", Uso = "Residencial", Tipo = "Departamento",
                Ambientes = 2, Superficie = 131, Latitud = 6, Longitud = 9, Valor = 80400,
                Imagen = "Uploads\\avatar_4.jpg", Disponible = false, IdPropietario = p.IdPropietario
            };
            db.Inmuebles.Add(inm);
            db.Inquilinos.Add(new Inquilino { Nombre = "Lautaro", Apellido = "Martinez", Dni = "999", Telefono="8884453", Email="toro@gmail.com" });
            db.SaveChanges();

            var inq = db.Inquilinos.First();
            db.Contratos.Add(new Contrato {
                FechaInicio = new DateOnly(2016,03,02),
                FechaFinalizacion = new DateOnly(2020,07,03),
                MontoAlquiler = 324423m, Estado = true,
                IdInquilino = inq.IdInquilino, IdInmueble = inm.IdInmueble
            });
            db.SaveChanges();

            db.Pagos.Add(new Pago {
                FechaPago = new DateOnly(2024,04,10), Monto = 23m, Detalle = "Mes abril",
                Estado = false, IdContrato = db.Contratos.First().IdContrato
            });
            db.SaveChanges();
        }
    }
}