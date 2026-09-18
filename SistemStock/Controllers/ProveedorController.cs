using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemStock.Models.Entities;
using SistemStock.Models.ViewModels;

public class ProveedorController : Controller
{
    private readonly AppDbContext _context;

    public ProveedorController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var proveedores = await _context.Proveedores.ToListAsync();
        var model = proveedores.Select(p => new ProveedorViewModel
        {
            id = p.id,
            Nombre = p.Nombre,
            Documento = p.Documento,
            Email = p.Email,
            Telefono = p.Telefono,
            Activo = p.Activo
        }).ToList();
        return View(model);
    }

    public IActionResult Create() => View(new ProveedorViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProveedorViewModel model)
    {
        if (ModelState.IsValid)
        {
            var proveedor = new Proveedor
            {
                Nombre = model.Nombre,
                Documento = model.Documento,
                Email = model.Email,
                Telefono = model.Telefono,
                Activo = model.Activo
            };
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Proveedor creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor == null) return NotFound();

        var model = new ProveedorViewModel
        {
            id = proveedor.id,
            Nombre = proveedor.Nombre,
            Documento = proveedor.Documento,
            Email = proveedor.Email,
            Telefono = proveedor.Telefono,
            Activo = proveedor.Activo
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, ProveedorViewModel model)
    {
        if (id != model.id) return NotFound();

        if (ModelState.IsValid)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();

            proveedor.Nombre = model.Nombre;
            proveedor.Documento = model.Documento;
            proveedor.Email = model.Email;
            proveedor.Telefono = model.Telefono;
            proveedor.Activo = model.Activo;
            proveedor.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Ok"] = "Proveedor actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor == null) return NotFound();

        var model = new ProveedorViewModel
        {
            id = proveedor.id,
            Nombre = proveedor.Nombre,
            Documento = proveedor.Documento,
            Email = proveedor.Email,
            Telefono = proveedor.Telefono,
            Activo = proveedor.Activo
        };
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var proveedor = await _context.Proveedores.Include(p => p.Movimientos).FirstOrDefaultAsync(p => p.id == id);
        if (proveedor != null)
        {
            if (proveedor.Movimientos.Any())
            {
                TempData["Error"] = $"No se puede eliminar el proveedor '{proveedor.Nombre}' porque tiene movimientos asociados.";
                return RedirectToAction(nameof(Index));
            }
            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Proveedor eliminado con éxito.";
        }
        return RedirectToAction(nameof(Index));
    }
}
