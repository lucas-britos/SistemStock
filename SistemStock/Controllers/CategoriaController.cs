using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemStock.Models.Entities;
using SistemStock.Models.ViewModels;

public class CategoriaController : Controller
{
    private readonly AppDbContext _context;

    public CategoriaController(AppDbContext context)
    {
        _context = context;
    }

    // GET: CATEGORIA
    public async Task<IActionResult> Index()
    {
        var categorias = await _context.Categorias.Include(c => c.Productos).ToListAsync();

        var listaViewModels = categorias.Select(c => new CategoriaViewModel
        {
            id = c.id,
            Nombre = c.Nombre,
            Descripcion = c.Descripcion,
            Activo = c.Activo,
            CantidadProductos = c.Productos.Count
        }).ToList();

        return View(listaViewModels);
    }

    // GET: CATEGORIA/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var categoria = await _context.Categorias.Include(c => c.Productos).FirstOrDefaultAsync(c => c.id == id);
        if (categoria == null) return NotFound();

        var model = new CategoriaViewModel
        {
            id = categoria.id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion,
            Activo = categoria.Activo,
            CreatedAt = categoria.CreatedAt,
            UpdatedAt = categoria.UpdatedAt,
            CantidadProductos = categoria.Productos.Count
        };

        return View(model);
    }

    // GET: CATEGORIA/Create
    public IActionResult Create()
    {
        return View(new CategoriaViewModel());
    }

    // POST: CATEGORIA/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoriaViewModel model)
    {
        if (ModelState.IsValid)
        {
            bool existeCategoria = await _context.Categorias
                .AnyAsync(c => c.Nombre.ToLower() == model.Nombre.ToLower());

            if (existeCategoria)
            {
                ModelState.AddModelError("Nombre", "Ya existe una categoría con este nombre.");
                return View(model);
            }

            var nuevaCategoria = new Categoria
            {
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                Activo = model.Activo
            };

            _context.Categorias.Add(nuevaCategoria);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "¡La categoría se ha creado con éxito!";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: CATEGORIA/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null) return NotFound();

        var model = new CategoriaViewModel
        {
            id = categoria.id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion,
            Activo = categoria.Activo
        };

        return View(model);
    }

    // POST: CATEGORIA/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, CategoriaViewModel model)
    {
        if (id != model.id) return NotFound();

        if (ModelState.IsValid)
        {
            bool existeCategoria = await _context.Categorias
                .AnyAsync(c => c.Nombre.ToLower() == model.Nombre.ToLower() && c.id != model.id);

            if (existeCategoria)
            {
                ModelState.AddModelError("Nombre", "Ya existe una categoría con este nombre.");
                return View(model);
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            categoria.Nombre = model.Nombre;
            categoria.Descripcion = model.Descripcion;
            categoria.Activo = model.Activo;
            categoria.UpdatedAt = DateTime.UtcNow;

            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "¡La categoría se ha modificado con éxito!";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: CATEGORIA/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null) return NotFound();

        var model = new CategoriaViewModel
        {
            id = categoria.id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion,
            Activo = categoria.Activo
        };

        return View(model);
    }

    // POST: CATEGORIA/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var categoria = await _context.Categorias.Include(c => c.Productos).FirstOrDefaultAsync(c => c.id == id);
        if (categoria != null)
        {
            if (categoria.Productos.Any())
            {
                TempData["Error"] = $"No se puede eliminar la categoría '{categoria.Nombre}' porque tiene productos asociados.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Categoría eliminada con éxito.";
        }
        return RedirectToAction(nameof(Index));
    }

    private bool CategoriaExists(int? id)
    {
        return _context.Categorias.Any(e => e.id == id);
    }
}
