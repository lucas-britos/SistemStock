using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemStock.Models.Enum;
using SistemStock.Models.ViewModels;

public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel();

        model.ValorTotalInventario = await _context.StockDepositos
                                            .Include(s => s.Producto)
                                            .SumAsync(s => s.Cantidad * s.Producto!.PrecioCosto);

        model.TotalProductosActivos = await _context.Productos.CountAsync(p => p.Activo);
        model.TotalProveedoresActivos = await _context.Proveedores.CountAsync(p => p.Activo);
        model.TotalClientesActivos = await _context.Clientes.CountAsync(c => c.Activo);
        model.TotalDepositosActivos = await _context.Depositos.CountAsync(d => d.Activo);

        model.MovimientosHoy = await _context.Movimientos
            .CountAsync(m => m.Fecha >= DateTime.Today && m.Fecha < DateTime.Today.AddDays(1));

        model.ValorVentasHoy = await _context.Movimientos
            .Include(s => s.Producto)
            .Where(m => m.Fecha >= DateTime.Today && m.Fecha < DateTime.Today.AddDays(1) && m.Tipo == TipoMovimiento.Salida)
            .SumAsync(s => s.Cantidad * s.Producto!.PrecioVenta);

        var stockProductos = await _context.Productos
            .AsNoTracking()
            .Include(p => p.Stocks)
            .Include(p => p.Categoria)
            .Where(p => p.Activo)
            .ToListAsync();

        var alertas = stockProductos
            .Where(p => p.StockTotal <= p.StockMinimo)
            .OrderBy(p => p.StockTotal)
            .Select(p => new AlertaReposicion
            {
                idProducto = p.id,
                Producto = p.Nombre,
                Categoria = p.Categoria.Nombre,
                StockTotal = p.StockTotal,
                StockMinimo = p.StockMinimo
            })
            .ToList();

        model.AlertasBajoStock = alertas.Count;
        model.AlertasReposicion = alertas.Take(15).ToList();

        model.TopMayorStock = stockProductos
            .OrderByDescending(p => p.StockTotal).Take(5)
            .Select(p => new StockProducto { Producto = p.Nombre, StockTotal = p.StockTotal })
            .ToList();

        model.TopMenorStock = stockProductos
            .OrderBy(p => p.StockTotal).Take(5)
            .Select(p => new StockProducto { Producto = p.Nombre, StockTotal = p.StockTotal })
            .ToList();

        return View(model);
    }
}
