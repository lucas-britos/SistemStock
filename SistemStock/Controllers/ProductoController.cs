using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemStock.Models.Entities;
using SistemStock.Models.ViewModels;

public class ProductoController : Controller
{
    private readonly AppDbContext _context;

    public ProductoController(AppDbContext context)
    {
        _context = context;
    }

    // GET: PRODUCTO
    public async Task<IActionResult> Index(string buscar, int? buscarCategoria)
    {
        var productos = _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Stocks)
            .AsQueryable();

        if (!string.IsNullOrEmpty(buscar))
            productos = productos.Where(p => p.Nombre.Contains(buscar) || (p.Codigo != null && p.Codigo.Contains(buscar)));

        if (buscarCategoria.HasValue)
            productos = productos.Where(p => p.idCategoria == buscarCategoria.Value);

        ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "id", "Nombre");

        var listaProductos = await productos.ToListAsync();

        var listaViewModels = listaProductos.Select(p => new ProductoViewModel
        {
            id = p.id,
            Codigo = p.Codigo,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            PrecioCosto = p.PrecioCosto,
            PrecioVenta = p.PrecioVenta,
            StockMinimo = p.StockMinimo,
            StockTotal = p.StockTotal,
            BajoStockMinimo = p.BajoStockMinimo,
            CategoriaNombre = p.Categoria?.Nombre,
            UnidadDeMedida = p.UnidadDeMedida
        }).ToList();

        return View(listaViewModels);
    }

    // GET: PRODUCTO/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var producto = await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Stocks).ThenInclude(s => s.Deposito)
            .FirstOrDefaultAsync(m => m.id == id);

        if (producto == null) return NotFound();

        var model = new ProductoViewModel
        {
            id = producto.id,
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            PrecioCosto = producto.PrecioCosto,
            PrecioVenta = producto.PrecioVenta,
            StockMinimo = producto.StockMinimo,
            StockTotal = producto.StockTotal,
            BajoStockMinimo = producto.BajoStockMinimo,
            CategoriaNombre = producto.Categoria?.Nombre,
            UnidadDeMedida = producto.UnidadDeMedida,
            Activo = producto.Activo,
            CreatedAt = producto.CreatedAt,
            UpdatedAt = producto.UpdatedAt,
        };

        return View(model);
    }

    // GET: PRODUCTO/Create
    public async Task<IActionResult> Create()
    {
        var model = new ProductoViewModel
        {
            Categorias = await ObtenerCategorias()
        };
        return View(model);
    }

    private async Task<List<SelectListItem>> ObtenerCategorias(int? idSeleccionado = null)
    {
        var categorias = await _context.Categorias
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();

        return new SelectList(categorias, "id", "Nombre", idSeleccionado).ToList();
    }

    // POST: PRODUCTO/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductoViewModel model)
    {
        if (ModelState.IsValid)
        {
            bool existeProducto = await _context.Productos
                .AnyAsync(c => (c.Codigo ?? "").ToLower() == (model.Codigo ?? "").ToLower()
                    && c.Nombre.ToLower() == model.Nombre.ToLower());

            if (existeProducto)
            {
                ModelState.AddModelError("Nombre", "Ya existe un producto con ese codigo y nombre.");
                model.Categorias = await ObtenerCategorias(model.idCategoria);
                return View(model);
            }

            var producto = new Producto
            {
                Codigo = model.Codigo,
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                PrecioCosto = model.PrecioCosto,
                PrecioVenta = model.PrecioVenta,
                StockMinimo = model.StockMinimo,
                idCategoria = model.idCategoria!.Value,
                UnidadDeMedida = model.UnidadDeMedida
            };

            _context.Add(producto);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Producto creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        model.Categorias = await ObtenerCategorias(model.idCategoria);
        return View(model);
    }

    // GET: PRODUCTO/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var producto = await _context.Productos.FindAsync(id);
        if (producto == null) return NotFound();

        var model = new ProductoViewModel
        {
            id = producto.id,
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            PrecioCosto = producto.PrecioCosto,
            PrecioVenta = producto.PrecioVenta,
            StockMinimo = producto.StockMinimo,
            idCategoria = producto.idCategoria,
            UnidadDeMedida = producto.UnidadDeMedida,
            Activo = producto.Activo
        };

        model.Categorias = await ObtenerCategorias(producto.idCategoria);
        return View(model);
    }

    // POST: PRODUCTO/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, ProductoViewModel model)
    {
        if (id != model.id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null) return NotFound();

                producto.Codigo = model.Codigo;
                producto.Nombre = model.Nombre;
                producto.Descripcion = model.Descripcion;
                producto.PrecioCosto = model.PrecioCosto;
                producto.PrecioVenta = model.PrecioVenta;
                producto.StockMinimo = model.StockMinimo;
                producto.idCategoria = model.idCategoria!.Value;
                producto.UnidadDeMedida = model.UnidadDeMedida;
                producto.Activo = model.Activo;
                producto.UpdatedAt = DateTime.UtcNow;

                _context.Update(producto);
                await _context.SaveChangesAsync();
                TempData["Ok"] = "Producto actualizado correctamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(model.id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        model.Categorias = await ObtenerCategorias(model.idCategoria);
        return View(model);
    }

    // GET: PRODUCTO/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var producto = await _context.Productos.Include(p => p.Categoria).FirstOrDefaultAsync(m => m.id == id);
        if (producto == null) return NotFound();

        var model = new ProductoViewModel
        {
            id = producto.id,
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            PrecioCosto = producto.PrecioCosto,
            PrecioVenta = producto.PrecioVenta,
            StockMinimo = producto.StockMinimo,
            idCategoria = producto.idCategoria,
            CategoriaNombre = producto.Categoria?.Nombre,
            UnidadDeMedida = producto.UnidadDeMedida
        };

        return View(model);
    }

    // POST: PRODUCTO/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var producto = await _context.Productos.Include(c => c.Movimientos).FirstOrDefaultAsync(c => c.id == id);
        if (producto != null)
        {
            if (producto.Movimientos.Any())
            {
                TempData["Error"] = $"No se puede eliminar el producto '{producto.Nombre}' porque tiene movimientos asociados.";
                return RedirectToAction(nameof(Index));
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Producto eliminado con éxito.";
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ProductoExists(int? id)
    {
        return _context.Productos.Any(e => e.id == id);
    }
}
