using EventFlow.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets - Tablas de la base de datos
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Tarjeta> Tarjetas { get; set; }
    public DbSet<Compra> Compras { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Producto
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Precio).HasColumnType("decimal(18,2)");
        });

        // Configuración de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
        });

        // Configuración de Tarjeta
        modelBuilder.Entity<Tarjeta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroTarjeta).IsRequired().HasMaxLength(20);
            entity.Property(e => e.SaldoDisponible).HasColumnType("decimal(18,2)");

            // Relación: Una tarjeta pertenece a un usuario
            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.Tarjetas)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Compra
        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MontoTotal).HasColumnType("decimal(18,2)");

            // Relación: Una compra pertenece a un usuario
            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.Compras)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Relación: Una compra pertenece a un producto
            entity.HasOne(e => e.Producto)
                  .WithMany(p => p.Compras)
                  .HasForeignKey(e => e.ProductoId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Relación: Una compra se paga con una tarjeta
            entity.HasOne(e => e.Tarjeta)
                  .WithMany(t => t.Compras)
                  .HasForeignKey(e => e.TarjetaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}