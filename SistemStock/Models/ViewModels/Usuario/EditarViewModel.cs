using System.ComponentModel.DataAnnotations;

namespace SistemStock.Models.ViewModels.Usuario
{
    public class EditarViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un rol")]
        public string Rol { get; set; } = string.Empty;

        public bool Activo { get; set; }
    }
}
