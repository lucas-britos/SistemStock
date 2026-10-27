using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class StockController : Controller
{
    private readonly AppDbContext _context;

    public StockController(AppDbContext context)
    {
        _context = context;
    }

    // GET: StockController
    public async Task<IActionResult> Index()
    {
        var productos = await _context.Productos
            .Include(p => p.Stocks)
                .ThenInclude(s => s.Deposito)
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
        return View(productos);
    }
}
