using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SistemStock.Models.ViewModels
{
    public class TransferenciaViewModel
    {
        [Display(Name = "Producto")]
        [Required(ErrorMessage = "Debe seleccionar un producto.")]
        public int? idProducto { get; set; }
        public List<SelectListItem>? Productos { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un Depósito origen.")]
        [Display(Name = "Depósito Origen")]
        public int? idDepositoOrigen { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un Depósito destino.")]
        [Display(Name = "Depósito Destino")]
        public int? idDepositoDestino { get; set; }

        public List<SelectListItem>? Depositos { get; set; }

        [StringLength(300)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }
    }
}
