using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemStock.Models.Entities;
using SistemStock.Models.ViewModels;

public class DepositoController : Controller
{
    private readonly AppDbContext _context;

    public DepositoController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var depositos = await _context.Depositos.ToListAsync();
        var model = depositos.Select(d => new DepositoViewModel
        {
            id = d.id,
            Nombre = d.Nombre,
            Ubicacion = d.Ubicacion,
            Activo = d.Activo
        }).ToList();
        return View(model);
    }

    public IActionResult Create() => View(new DepositoViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DepositoViewModel model)
    {
        if (ModelState.IsValid)
        {
            var deposito = new Deposito
            {
                Nombre = model.Nombre,
                Ubicacion = model.Ubicacion,
                Activo = model.Activo
            };
            _context.Depositos.Add(deposito);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Depósito creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var deposito = await _context.Depositos.FindAsync(id);
        if (deposito == null) return NotFound();

        var model = new DepositoViewModel
        {
            id = deposito.id,
            Nombre = deposito.Nombre,
            Ubicacion = deposito.Ubicacion,
            Activo = deposito.Activo
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, DepositoViewModel model)
    {
        if (id != model.id) return NotFound();

        if (ModelState.IsValid)
        {
            var deposito = await _context.Depositos.FindAsync(id);
            if (deposito == null) return NotFound();

            deposito.Nombre = model.Nombre;
            deposito.Ubicacion = model.Ubicacion;
            deposito.Activo = model.Activo;
            deposito.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Ok"] = "Depósito actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var deposito = await _context.Depositos.FindAsync(id);
        if (deposito == null) return NotFound();

        var model = new DepositoViewModel
        {
            id = deposito.id,
            Nombre = deposito.Nombre,
            Ubicacion = deposito.Ubicacion,
            Activo = deposito.Activo
        };
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var deposito = await _context.Depositos.Include(d => d.Stocks).FirstOrDefaultAsync(d => d.id == id);
        if (deposito != null)
        {
            if (deposito.Stocks.Any(s => s.Cantidad > 0))
            {
                TempData["Error"] = $"No se puede eliminar el depósito '{deposito.Nombre}' porque tiene stock asociado.";
                return RedirectToAction(nameof(Index));
            }
            _context.Depositos.Remove(deposito);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Depósito eliminado con éxito.";
        }
        return RedirectToAction(nameof(Index));
    }
}
