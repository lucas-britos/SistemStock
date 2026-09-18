using SistemStock.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemStock.Models.Entities
{
    public class Producto : BaseEntity
    {
        [StringLength(50)]
        public string? Codigo { get; set; }

        [Required, StringLength(100)]
        public required string Nombre { get; set; }

        [StringLength(300)]
        public string? Descripcion { get; set; }

        [Required]
        [ForeignKey("Categoria")]
        public int idCategoria { get; set; }
        public virtual Categoria Categoria { get; set; } = null!;

        [Required]
        public UnidadMedida UnidadDeMedida { get; set; }

        [Required]
        [Range(0, 1000000)]
        [DataType(DataType.Currency)]
        public decimal PrecioCosto { get; set; }

        [Required]
        [Range(0, 1000000)]
        [DataType(DataType.Currency)]
        public decimal PrecioVenta { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockMinimo { get; set; } = 5;

        // Relaciones
        public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
        public ICollection<StockDeposito> Stocks { get; set; } = new List<StockDeposito>();

        [NotMapped]
        public int StockTotal => Stocks.Sum(s => s.Cantidad);

        [NotMapped]
        public bool BajoStockMinimo => StockTotal <= StockMinimo;
    }
}
