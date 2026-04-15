using System;
using Microsoft.EntityFrameworkCore;
using TiendaChizitos.Entidades;

namespace TiendaChizitos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<DetalleVenta> DetalleVentas { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>()
            .ToTable("Cliente");

        modelBuilder.Entity<Cliente>()
            .Property(x => x.Nombre)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Cliente>()
            .Property(x => x.Extension)
            .HasMaxLength(2);

        modelBuilder.Entity<Cliente>()
            .Property(x => x.PorcentajeDescuento)
            .HasColumnType("decimal(5,2)");


        modelBuilder.Entity<Venta>()
            .ToTable("Venta");

        modelBuilder.Entity<Venta>()
            .HasOne(x => x.Cliente)
            .WithMany(x => x.Ventas)
            .HasForeignKey(x => x.ClienteId);

        modelBuilder.Entity<Venta>()
            .Property(x => x.Total)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<DetalleVenta>()
            .ToTable("DetalleVenta");

        modelBuilder.Entity<DetalleVenta>()
            .HasOne(x => x.Venta)
            .WithMany(x => x.DetalleVentas)
            .HasForeignKey(x => x.VentaId);

        modelBuilder.Entity<DetalleVenta>()
            .HasOne(x => x.Producto)
            .WithMany(x => x.DetalleVentas)
            .HasForeignKey(x => x.ProductoId);

        modelBuilder.Entity<DetalleVenta>()
            .Property(x => x.Precio)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Producto>()
            .ToTable("Producto");

        modelBuilder.Entity<Producto>()
            .Property(x => x.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Producto>()
            .Property(x => x.Precio)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Producto>()
            .HasOne(x => x.Categoria)
            .WithMany(x => x.Productos)
            .HasForeignKey(x => x.CategoriaId);
            
        modelBuilder.Entity<Producto>()
            .Property(x => x.EsVigente)
            .HasDefaultValue(true);



        modelBuilder.Entity<Categoria>()
            .ToTable("Categoria");

        modelBuilder.Entity<Categoria>()
            .Property(x => x.Nombre)
            .HasMaxLength(50)
            .IsRequired();
    }
}