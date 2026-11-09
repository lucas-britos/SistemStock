namespace SistemStock.Models.ViewModels
{
    public class DashboardViewModel
    {
        public decimal ValorTotalInventario { get; set; }
        public int TotalProductosActivos { get; set; }
        public int TotalProveedoresActivos { get; set; }
        public int TotalClientesActivos { get; set; }
        public int TotalDepositosActivos { get; set; }
        public int AlertasBajoStock { get; set; }
        public int MovimientosHoy { get; set; }
        public decimal ValorVentasHoy { get; set; }

        public List<AlertaReposicion> AlertasReposicion { get; set; } = new();

        public List<StockProducto> TopMayorStock { get; set; } = new();
        public List<StockProducto> TopMenorStock { get; set; } = new();
    }

    public class AlertaReposicion
    {
        public int idProducto { get; set; }
        public string Producto { get; set; } = "";
        public string Categoria { get; set; } = "";
        public decimal StockTotal { get; set; }
        public decimal StockMinimo { get; set; }
        public bool SinStock => StockTotal <= 0;
        public string Estado => SinStock ? "Sin stock" : "Bajo stock";
    }

    public class StockProducto
    {
        public string Producto { get; set; } = "";
        public decimal StockTotal { get; set; }
    }
}
