using System.ComponentModel.DataAnnotations;

namespace SistemStock.Models.ViewModels
{
    public class ClienteViewModel
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio"), StringLength(120, ErrorMessage = "Máximo 120 caracteres")]
        [Display(Name = "Nombre / Razón Social")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "Máximo 30 caracteres")]
        public string? Documento { get; set; }

        [EmailAddress, StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string? Email { get; set; }

        [Display(Name = "Teléfono")]
        [Phone, StringLength(30, ErrorMessage = "Máximo 30 caracteres")]
        public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;
    }
}
