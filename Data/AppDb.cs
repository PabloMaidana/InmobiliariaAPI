using Microsoft.EntityFrameworkCore;
using InmobilariaWebAPI.Models;

namespace InmobilariaWebAPI.Data
{
    public class AppDb : DbContext
    {
        public AppDb(DbContextOptions<AppDb> options) : base(options)
        {

        }

        public DbSet<Inmueble> Inmuebles { get; set; }
        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Inquilino> Inquilinos { get; set; }
        public DbSet<Alquiler> Alquileres { get; set; }
        public DbSet<Pago> Pagos { get; set; }
    }
}