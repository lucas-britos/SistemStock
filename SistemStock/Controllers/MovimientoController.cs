using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemStock.Models.Entities;
using SistemStock.Models.Enum;
using SistemStock.Models.ViewModels;

public class MovimientoController : Controller
{
    private readonly AppDbContext _context;

    public MovimientoController(AppDbContext context)
    {
        _context = context;
    }

    // GET: MOVIMIENTO (historial)
    public async Task<IActionResult> Index(TipoMovimiento? tipoFiltro, int? productoIdFiltro, int? depositoIdFiltro, DateTime? fechaDesde, DateTime? fechaHasta)
    {
        var query = _context.Movimientos
            .Include(m => m.Producto)
            .Include(m => m.DepositoOrigen)
            .Include(m => m.DepositoDestino)
            .Include(m => m.Proveedor)
            .Include(m => m.Cliente)
            .AsQueryable();

        if (tipoFiltro.HasValue)
            query = query.Where(m => m.Tipo == tipoFiltro.Value);
        if (productoIdFiltro.HasValue)
            query = query.Where(m => m.idProducto == productoIdFiltro.Value);
        if (depositoIdFiltro.HasValue)
            query = query.Where(m => m.idDepositoOrigen == depositoIdFiltro.Value || m.idDepositoDestino == depositoIdFiltro.Value);
        if (fechaDesde.HasValue)
            query = query.Where(m => m.Fecha >= fechaDesde.Value);
        if (fechaHasta.HasValue)
            query = query.Where(m => m.Fecha <= fechaHasta.Value.AddDays(1).AddTicks(-1));

        var model = new MovimientosHistorialViewModel
        {
            Movimientos = await query.OrderByDescending(m => m.Fecha).ToListAsync(),
            TipoFiltro = tipoFiltro,
            ProductoIdFiltro = productoIdFiltro,
            DepositoIdFiltro = depositoIdFiltro,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            Productos = new SelectList(await _context.Productos.OrderBy(p => p.Nombre).ToListAsync(), "id", "Nombre").ToList(),
            Depositos = new SelectList(await _context.Depositos.OrderBy(d => d.Nombre).ToListAsync(), "id", "Nombre").ToList()
        };

        return View(model);
    }

    private async Task<StockDeposito> ObtenerOCrearStock(int idProducto, int idDeposito)
    {
        var stock = await _context.StockDepositos.FirstOrDefaultAsync(s => s.idProducto == idProducto && s.idDeposito == idDeposito);
        if (stock == null)
        {
            stock = new StockDeposito { idProducto = idProducto, idDeposito = idDeposito, Cantidad = 0 };
            _context.StockDepositos.Add(stock);
        }
        return stock;
    }

    private async Task<OperacionStockViewModel> PrepararOperacionStock(OperacionStockViewModel model, bool esSalida)
    {
        model.Productos = new SelectList(await _context.Productos.OrderBy(p => p.Nombre).ToListAsync(), "id", "Nombre", model.idProducto).ToList();
        model.Depositos = new SelectList(await _context.Depositos.Where(d => d.Activo).OrderBy(d => d.Nombre).ToListAsync(), "id", "Nombre", model.idDepositoOrigen).ToList();

        if (esSalida)
            model.Clientes = new SelectList(await _context.Clientes.Where(c => c.Activo).OrderBy(c => c.Nombre).ToListAsync(), "id", "Nombre", model.idCliente).ToList();
        else
            model.Proveedores = new SelectList(await _context.Proveedores.Where(p => p.Activo).OrderBy(p => p.Nombre).ToListAsync(), "id", "Nombre", model.idProveedor).ToList();

        return model;
    }

    // GET: MOVIMIENTO/Entrada
    public async Task<IActionResult> Entrada()
    {
        var model = await PrepararOperacionStock(new OperacionStockViewModel(), esSalida: false);
        return View(model);
    }

    // POST: MOVIMIENTO/Entrada
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Entrada(OperacionStockViewModel model)
    {
        if (ModelState.IsValid)
        {
            var stock = await ObtenerOCrearStock(model.idProducto!.Value, model.idDepositoOrigen!.Value);
            stock.Cantidad += model.Cantidad;

            var movimiento = new Movimiento
            {
                Tipo = TipoMovimiento.Entrada,
                Cantidad = model.Cantidad,
                Fecha = DateTime.Now,
                Observaciones = model.Observaciones,
                idProducto = model.idProducto.Value,
                idDepositoOrigen = model.idDepositoOrigen,
                idProveedor = model.idProveedor
            };

            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Entrada de mercadería registrada con éxito.";
            return RedirectToAction(nameof(Index));
        }

        model = await PrepararOperacionStock(model, esSalida: false);
        return View(model);
    }

    // GET: MOVIMIENTO/Salida
    public async Task<IActionResult> Salida()
    {
        var model = await PrepararOperacionStock(new OperacionStockViewModel(), esSalida: true);
        return View(model);
    }

    // POST: MOVIMIENTO/Salida
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Salida(OperacionStockViewModel model)
    {
        if (ModelState.IsValid)
        {
            var stock = await ObtenerOCrearStock(model.idProducto!.Value, model.idDepositoOrigen!.Value);

            if (stock.Cantidad < model.Cantidad)
            {
                ModelState.AddModelError("Cantidad", $"Stock insuficiente. Disponible: {stock.Cantidad}.");
                model = await PrepararOperacionStock(model, esSalida: true);
                return View(model);
            }

            stock.Cantidad -= model.Cantidad;

            var movimiento = new Movimiento
            {
                Tipo = TipoMovimiento.Salida,
                Cantidad = model.Cantidad,
                Fecha = DateTime.Now,
                Observaciones = model.Observaciones,
                idProducto = model.idProducto.Value,
                idDepositoOrigen = model.idDepositoOrigen,
                idCliente = model.idCliente
            };

            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Salida de mercadería registrada con éxito.";
            return RedirectToAction(nameof(Index));
        }

        model = await PrepararOperacionStock(model, esSalida: true);
        return View(model);
    }

    // GET: MOVIMIENTO/Transferencia
    public async Task<IActionResult> Transferencia()
    {
        var model = new TransferenciaViewModel
        {
            Productos = new SelectList(await _context.Productos.OrderBy(p => p.Nombre).ToListAsync(), "id", "Nombre").ToList(),
            Depositos = new SelectList(await _context.Depositos.Where(d => d.Activo).OrderBy(d => d.Nombre).ToListAsync(), "id", "Nombre").ToList()
        };
        return View(model);
    }

    // POST: MOVIMIENTO/Transferencia
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Transferencia(TransferenciaViewModel model)
    {
        if (model.idDepositoOrigen == model.idDepositoDestino)
        {
            ModelState.AddModelError("idDepositoDestino", "El depósito destino debe ser distinto al de origen.");
        }

        if (ModelState.IsValid)
        {
            var stockOrigen = await ObtenerOCrearStock(model.idProducto!.Value, model.idDepositoOrigen!.Value);

            if (stockOrigen.Cantidad < model.Cantidad)
            {
                ModelState.AddModelError("Cantidad", $"Stock insuficiente en el depósito origen. Disponible: {stockOrigen.Cantidad}.");
                model.Productos = new SelectList(await _context.Productos.OrderBy(p => p.Nombre).ToListAsync(), "id", "Nombre", model.idProducto).ToList();
                model.Depositos = new SelectList(await _context.Depositos.Where(d => d.Activo).OrderBy(d => d.Nombre).ToListAsync(), "id", "Nombre").ToList();
                return View(model);
            }

            var stockDestino = await ObtenerOCrearStock(model.idProducto.Value, model.idDepositoDestino!.Value);

            stockOrigen.Cantidad -= model.Cantidad;
            stockDestino.Cantidad += model.Cantidad;

            var fecha = DateTime.Now;

            _context.Movimientos.Add(new Movimiento
            {
                Tipo = TipoMovimiento.TransferenciaSalida,
                Cantidad = model.Cantidad,
                Fecha = fecha,
                Observaciones = model.Observaciones,
                idProducto = model.idProducto.Value,
                idDepositoOrigen = model.idDepositoOrigen,
                idDepositoDestino = model.idDepositoDestino
            });

            _context.Movimientos.Add(new Movimiento
            {
                Tipo = TipoMovimiento.TransferenciaEntrada,
                Cantidad = model.Cantidad,
                Fecha = fecha,
                Observaciones = model.Observaciones,
                idProducto = model.idProducto.Value,
                idDepositoOrigen = model.idDepositoOrigen,
                idDepositoDestino = model.idDepositoDestino
            });

            await _context.SaveChangesAsync();

            TempData["Ok"] = "Transferencia entre depósitos registrada con éxito.";
            return RedirectToAction(nameof(Index));
        }

        model.Productos = new SelectList(await _context.Productos.OrderBy(p => p.Nombre).ToListAsync(), "id", "Nombre", model.idProducto).ToList();
        model.Depositos = new SelectList(await _context.Depositos.Where(d => d.Activo).OrderBy(d => d.Nombre).ToListAsync(), "id", "Nombre").ToList();
        return View(model);
    }
}
