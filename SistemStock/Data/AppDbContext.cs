using Microsoft.EntityFrameworkCore;
using SistemStock.Models.Entities;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Movimiento> Movimientos { get; set; }
    public DbSet<Deposito> Depositos { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<StockDeposito> StockDepositos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Evitar delete cascade
        modelBuilder.Entity<Producto>()
            .HasOne(m => m.Categoria) // Un Producto tiene una Categoria
            .WithMany(m => m.Productos) // Una Categoria tiene muchos Productos
            .HasForeignKey(m => m.idCategoria) // La clave foránea en Producto
            .OnDelete(DeleteBehavior.Restrict); // Evita que operaciones en cascada afecten a la categoría

        modelBuilder.Entity<Movimiento>()
            .HasOne(m => m.DepositoOrigen)
            .WithMany(d => d.MovimientosOrigen)
            .HasForeignKey(m => m.idDepositoOrigen)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Movimiento>()
            .HasOne(m => m.DepositoDestino)
            .WithMany(d => d.MovimientosDestino)
            .HasForeignKey(m => m.idDepositoDestino)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Movimiento>()
            .HasOne(m => m.Producto)
            .WithMany(p => p.Movimientos)
            .HasForeignKey(m => m.idProducto)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockDeposito>()
            .HasOne(s => s.Producto)
            .WithMany(p => p.Stocks)
            .HasForeignKey(s => s.idProducto)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockDeposito>()
            .HasOne(s => s.Deposito)
            .WithMany(d => d.Stocks)
            .HasForeignKey(s => s.idDeposito)
            .OnDelete(DeleteBehavior.Restrict);

        // Datos semilla de prueba para las tablas
        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { id = 1, Nombre = "Alimentos", Activo = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null },
            new Categoria { id = 2, Nombre = "Limpieza", Activo = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null },
            new Categoria { id = 3, Nombre = "Lacteos", Activo = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null }
        );

        modelBuilder.Entity<Deposito>().HasData(
            new Deposito { id = 1, Nombre = "Depósito General", Ubicacion = "Planta baja", Activo = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null },
            new Deposito { id = 2, Nombre = "Depósito Principal", Ubicacion = "Planta alta", Activo = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null },
            new Deposito { id = 3, Nombre = "Depósito Secundario", Ubicacion = "Planta baja", Activo = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null }
        );

        modelBuilder.Entity<Proveedor>().HasData(
            new Proveedor { id = 1, Nombre = "Proveedor A", Activo = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null }
        );

        modelBuilder.Entity<Cliente>().HasData(
            new Cliente { id = 1, Nombre = "Cliente A", Activo = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = null }
        );
    }
}
