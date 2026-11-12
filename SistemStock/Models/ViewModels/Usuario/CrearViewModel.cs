using System.ComponentModel.DataAnnotations;

namespace SistemStock.Models.ViewModels.Usuario
{
    public class CrearViewModel
    {
        [Required(ErrorMessage = "El Email es obligatorio."), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria."), DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria."), DataType(DataType.Password), Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un rol")]
        public string Rol { get; set; } = string.Empty;
    }
}
