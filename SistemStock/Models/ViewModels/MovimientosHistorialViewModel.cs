using Microsoft.AspNetCore.Mvc.Rendering;
using SistemStock.Models.Entities;
using SistemStock.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SistemStock.Models.ViewModels
{
    public class MovimientosHistorialViewModel
    {
        public List<Movimiento> Movimientos { get; set; } = new();

        // Filtros
        [Display(Name = "Tipo")]
        public TipoMovimiento? TipoFiltro { get; set; }

        [Display(Name = "Producto")]
        public int? ProductoIdFiltro { get; set; }

        [Display(Name = "Depósito")]
        public int? DepositoIdFiltro { get; set; }

        [Display(Name = "Desde")]
        [DataType(DataType.Date)]
        public DateTime? FechaDesde { get; set; }

        [Display(Name = "Hasta")]
        [DataType(DataType.Date)]
        public DateTime? FechaHasta { get; set; }

        // Combos para filtros
        public List<SelectListItem> Productos { get; set; } = new();
        public List<SelectListItem> Depositos { get; set; } = new();
    }
}
