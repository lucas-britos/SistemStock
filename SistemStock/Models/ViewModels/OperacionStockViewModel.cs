using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SistemStock.Models.ViewModels
{
    public class OperacionStockViewModel
    {
        [Display(Name = "Producto")]
        [Required(ErrorMessage = "Debe seleccionar un producto.")]
        public int? idProducto { get; set; }
        public List<SelectListItem>? Productos { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un Deposito.")]
        [Display(Name = "Depósito")]
        public int? idDepositoOrigen { get; set; }
        public List<SelectListItem>? Depositos { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }

        [Display(Name = "Proveedor")]
        public int? idProveedor { get; set; }
        public List<SelectListItem>? Proveedores { get; set; }

        [Display(Name = "Cliente")]
        public int? idCliente { get; set; }
        public List<SelectListItem>? Clientes { get; set; }

        [StringLength(300)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }
    }
}
