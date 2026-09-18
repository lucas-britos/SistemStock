using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemStock.Models.Entities
{
    public class StockDeposito : BaseEntity
    {
        [Required]
        [ForeignKey("Producto")]
        public int idProducto { get; set; }
        public Producto? Producto { get; set; }

        [Required]
        [ForeignKey("Deposito")]
        public int idDeposito { get; set; }
        public Deposito? Deposito { get; set; }

        [Range(0, int.MaxValue)]
        public int Cantidad { get; set; } = 0;
    }
}
