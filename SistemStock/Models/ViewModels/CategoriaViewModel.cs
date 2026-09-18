using System.ComponentModel.DataAnnotations;

namespace SistemStock.Models.ViewModels
{
    public class CategoriaViewModel
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los {1} caracteres.")]
        [Display(Name = "Nombre de la Categoría")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La descripcion no puede superar los {1} caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Display(Name = "Fecha de Actualización")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Cantidad de Productos")]
        public int CantidadProductos { get; set; }
    }
}
