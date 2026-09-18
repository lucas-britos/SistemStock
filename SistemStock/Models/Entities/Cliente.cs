using System.ComponentModel.DataAnnotations;

namespace SistemStock.Models.Entities
{
    public class Cliente : BaseEntity
    {
        [Required, StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        public string? Contacto { get; set; }

        [StringLength(30)]
        public string? Documento { get; set; }

        [Phone, StringLength(30)]
        public string? Telefono { get; set; }

        [EmailAddress, StringLength(100)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(300)]
        public string? Observaciones { get; set; }

        public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
    }
}
