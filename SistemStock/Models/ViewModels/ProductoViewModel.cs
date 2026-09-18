using Microsoft.AspNetCore.Mvc.Rendering;
using SistemStock.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SistemStock.Models.ViewModels
{
    public class ProductoViewModel
    {
        public int id { get; set; }

        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        [Display(Name = "Código")]
        public string? Codigo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        [Display(Name = "Nombre del Producto")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Máximo 300 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Categoría")]
        public int? idCategoria { get; set; }
        public string? CategoriaNombre { get; set; }
        public List<SelectListItem>? Categorias { get; set; }

        [Required]
        [Display(Name = "Unidad de Medida")]
        public UnidadMedida UnidadDeMedida { get; set; }

        [Required(ErrorMessage = "El precio de costo es obligatorio.")]
        [Range(0, 99999999.99, ErrorMessage = "El precio debe ser mayor o igual a 0.")]
        [Display(Name = "Precio de Costo")]
        public decimal PrecioCosto { get; set; }

        [Required(ErrorMessage = "El precio de venta es obligatorio.")]
        [Range(0, 99999999.99, ErrorMessage = "El precio debe ser mayor o igual a 0.")]
        [Display(Name = "Precio de Venta")]
        public decimal PrecioVenta { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo debe ser un número positivo.")]
        [Display(Name = "Stock Mínimo")]
        public int StockMinimo { get; set; } = 5;

        [Display(Name = "Stock Total")]
        public int StockTotal { get; set; }

        [Display(Name = "Bajo Stock Mínimo")]
        public bool BajoStockMinimo { get; set; }

        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Display(Name = "Fecha de Actualización")]
        public DateTime? UpdatedAt { get; set; }
    }
}
