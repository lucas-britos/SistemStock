using System.ComponentModel.DataAnnotations;

namespace SistemStock.Models.Entities
{
    public class BaseEntity
    {
        [Key]
        public int id { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
