using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemStock.Models.Entities;
using SistemStock.Models.Enum;

[Authorize]
public class ReporteController : Controller
{
    private readonly AppDbContext _context;

    public ReporteController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> DescargarExcel(
        DateTime? desde, DateTime? hasta, TipoMovimiento? tipo)
    {
        var movimientos = await FiltrarMovimientosAsync(desde, hasta, tipo);

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Movimientos");

        var titulo = ws.Cell(1, 1);
        titulo.Value = "Reporte de Movimientos de Stock";
        titulo.Style.Font.Bold = true;
        titulo.Style.Font.FontSize = 16;
        ws.Range(1, 1, 1, 8).Merge();

        string[] headers = { "ID", "Fecha", "Tipo", "Producto",
            "Depósito Origen", "Depósito Destino", "Cantidad", "Usuario" };
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(2, i + 1);
            cell.Value = headers[i];
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E75B6");
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Font.Bold = true;
        }

        int fila = 3;
        foreach (var m in movimientos)
        {
            ws.Cell(fila, 1).Value = m.id;
            ws.Cell(fila, 2).Value = m.Fecha.ToString("dd/MM/yyyy HH:mm");
            ws.Cell(fila, 3).Value = m.Tipo.ToString();
            ws.Cell(fila, 4).Value = m.Producto?.Nombre ?? "";
            ws.Cell(fila, 5).Value = m.DepositoOrigen?.Nombre ?? "";
            ws.Cell(fila, 6).Value = m.DepositoDestino?.Nombre ?? "—";
            ws.Cell(fila, 7).Value = m.Cantidad;
            ws.Cell(fila, 8).Value = m.UsuarioRegistro;

            var color = m.Tipo == TipoMovimiento.Entrada ? XLColor.FromHtml("#D6F0DC")
                      : m.Tipo == TipoMovimiento.Salida ? XLColor.FromHtml("#FFEBEE")
                      : XLColor.FromHtml("#FFF9C4");
            ws.Row(fila).Style.Fill.BackgroundColor = color;
            fila++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        wb.SaveAs(stream);
        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"movimientos_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    public async Task<IActionResult> DescargarPdf(
        DateTime? desde, DateTime? hasta, TipoMovimiento? tipo)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var movimientos = await FiltrarMovimientosAsync(desde, hasta, tipo);

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);

                page.Header().Text("Reporte de Movimientos de Stock")
                    .Bold().FontSize(18).FontColor("#1F4E79");

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(40);
                        cols.ConstantColumn(110);
                        cols.ConstantColumn(100);
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(2);
                        cols.ConstantColumn(60);
                        cols.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        void HCell(string txt) => header.Cell()
                            .Background("#2E75B6").Padding(5)
                            .Text(txt).Bold().FontColor(Colors.White);
                        HCell("ID"); HCell("Fecha"); HCell("Tipo");
                        HCell("Producto"); HCell("Depósito");
                        HCell("Cant."); HCell("Usuario");
                    });

                    bool alt = false;
                    foreach (var m in movimientos)
                    {
                        string bg = alt ? "#F2F2F2" : "#FFFFFF"; alt = !alt;
                        void DCell(string txt) => table.Cell()
                            .Background(bg).Padding(4).Text(txt).FontSize(9);
                        DCell(m.id.ToString());
                        DCell(m.Fecha.ToString("dd/MM/yy HH:mm"));
                        DCell(m.Tipo.ToString());
                        DCell(m.Producto?.Nombre ?? "");
                        DCell(m.DepositoOrigen?.Nombre ?? "");
                        DCell(m.Cantidad.ToString());
                        DCell(m.UsuarioRegistro?.ToString() ?? "");
                    }
                });

                page.Footer().AlignRight()
                    .Text(t =>
                    {
                        t.Span("Página ");
                        t.CurrentPageNumber();
                        t.Span(" de ");
                        t.TotalPages();
                    });
            });
        });

        var bytes = pdf.GeneratePdf();
        return File(bytes, "application/pdf",
            $"movimientos_{DateTime.Now:yyyyMMdd}.pdf");
    }

    private async Task<List<Movimiento>> FiltrarMovimientosAsync(
        DateTime? desde, DateTime? hasta, TipoMovimiento? tipo)
    {
        var query = _context.Movimientos
            .Include(m => m.Producto)
            .Include(m => m.DepositoOrigen)
            .Include(m => m.DepositoDestino)
            .Include(m => m.Proveedor)
            .Include(m => m.Cliente)
            .AsQueryable();

        if (desde.HasValue) query = query.Where(m => m.Fecha >= desde.Value);
        if (hasta.HasValue) query = query.Where(m => m.Fecha <= hasta.Value.AddDays(1));
        if (tipo.HasValue) query = query.Where(m => m.Tipo == tipo.Value);

        return await query.OrderByDescending(m => m.Fecha).ToListAsync();
    }
}
