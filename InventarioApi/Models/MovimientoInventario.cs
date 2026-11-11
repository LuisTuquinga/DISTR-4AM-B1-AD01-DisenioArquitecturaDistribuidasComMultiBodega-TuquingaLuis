using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApi.Models
{
    [Table("MovimientoInventario")]
    public class MovimientoInventario
    {
        [Key]
        [Column("IdMovimiento")]
        public int IdMovimiento { get; set; }

        [Column("IdProducto")]
        public int IdProducto { get; set; }

        [Column("IdBodega")]
        public int IdBodega { get; set; }

        [Required]
        [StringLength(20)]
        [Column("Tipo")]
        public string Tipo { get; set; } = string.Empty;

        [Column("Cantidad")]
        public int Cantidad { get; set; }

        [Column("Fecha")]
        public DateTime Fecha { get; set; }
    }
}
