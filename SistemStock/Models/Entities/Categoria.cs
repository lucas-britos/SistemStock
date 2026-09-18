using System.ComponentModel.DataAnnotations;

namespace SistemStock.Models.Entities
{
    public class Categoria : BaseEntity
    {
        [Required, StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Descripcion { get; set; }

        // Navegación
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
