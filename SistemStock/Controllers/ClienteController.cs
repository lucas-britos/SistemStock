using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemStock.Models.Entities;
using SistemStock.Models.ViewModels;

public class ClienteController : Controller
{
    private readonly AppDbContext _context;

    public ClienteController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var clientes = await _context.Clientes.ToListAsync();
        var model = clientes.Select(c => new ClienteViewModel
        {
            id = c.id,
            Nombre = c.Nombre,
            Documento = c.Documento,
            Email = c.Email,
            Telefono = c.Telefono,
            Activo = c.Activo
        }).ToList();
        return View(model);
    }

    public IActionResult Create() => View(new ClienteViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClienteViewModel model)
    {
        if (ModelState.IsValid)
        {
            var cliente = new Cliente
            {
                Nombre = model.Nombre,
                Documento = model.Documento,
                Email = model.Email,
                Telefono = model.Telefono,
                Activo = model.Activo
            };
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Cliente creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null) return NotFound();

        var model = new ClienteViewModel
        {
            id = cliente.id,
            Nombre = cliente.Nombre,
            Documento = cliente.Documento,
            Email = cliente.Email,
            Telefono = cliente.Telefono,
            Activo = cliente.Activo
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, ClienteViewModel model)
    {
        if (id != model.id) return NotFound();

        if (ModelState.IsValid)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            cliente.Nombre = model.Nombre;
            cliente.Documento = model.Documento;
            cliente.Email = model.Email;
            cliente.Telefono = model.Telefono;
            cliente.Activo = model.Activo;
            cliente.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Ok"] = "Cliente actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null) return NotFound();

        var model = new ClienteViewModel
        {
            id = cliente.id,
            Nombre = cliente.Nombre,
            Documento = cliente.Documento,
            Email = cliente.Email,
            Telefono = cliente.Telefono,
            Activo = cliente.Activo
        };
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var cliente = await _context.Clientes.Include(c => c.Movimientos).FirstOrDefaultAsync(c => c.id == id);
        if (cliente != null)
        {
            if (cliente.Movimientos.Any())
            {
                TempData["Error"] = $"No se puede eliminar el cliente '{cliente.Nombre}' porque tiene movimientos asociados.";
                return RedirectToAction(nameof(Index));
            }
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Cliente eliminado con éxito.";
        }
        return RedirectToAction(nameof(Index));
    }
}
