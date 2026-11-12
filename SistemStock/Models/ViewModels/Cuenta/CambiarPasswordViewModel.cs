using System.ComponentModel.DataAnnotations;

namespace SistemStock.Models.ViewModels.Cuenta
{
    public class CambiarPasswordViewModel
    {
        [Required(ErrorMessage = "Requerido"), DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string PasswordActual { get; set; } = string.Empty;

        [Required(ErrorMessage = "Requerido"), StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NuevaPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Requerido"), DataType(DataType.Password)]
        [Compare(nameof(NuevaPassword), ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}
