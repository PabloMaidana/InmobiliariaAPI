using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Api.Models;

namespace Inmobiliaria.Api.Data;
public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Propietario> Propietarios => Set<Propietario>();
    public DbSet<Inquilino> Inquilinos => Set<Inquilino>();
    public DbSet<Inmueble> Inmuebles => Set<Inmueble>();
    public DbSet<Contrato> Contratos => Set<Contrato>();
    public DbSet<Pago> Pagos => Set<Pago>();

    protected override void OnModelCreating(ModelBuilder b) 
    {
        b.Entity<Propietario>(e =>
        {
            e.HasKey(x => x.IdPropietario);
            e.Property(x => x.Email).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
        });

        b.Entity<Inquilino>(e =>
        {
            e.HasKey(x => x.IdInquilino); 
        });

        b.Entity<Inmueble>(e =>
        {
            e.HasKey(x => x.IdInmueble);
            e.HasOne(x => x.Duenio)
                .WithMany(p => p.Inmuebles)
                .HasForeignKey(x => x.IdPropietario)
                .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Contrato>(e =>
        {
            e.HasKey(x => x.IdContrato);
            e.Property(x => x.MontoAlquiler).HasPrecision(18, 2); 
            e.HasOne(x => x.Inquilino)
            .WithMany(i => i.Contratos)
            .HasForeignKey(x => x.IdInquilino);
            e.HasOne(x => x.Inmueble)
            .WithMany(m => m.Contratos)
            .HasForeignKey(x => x.IdInmueble);
        });
        
        b.Entity<Pago>(e =>
        {
            e.HasKey(x => x.IdPago);
            e.Property(x => x.Monto).HasPrecision(18, 2); 
            e.HasOne(x => x.Contrato)
            .WithMany(c => c.Pagos)
            .HasForeignKey(x => x.IdContrato);
        }); 
    }
}