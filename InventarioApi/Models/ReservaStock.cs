using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApi.Models
{
    [Table("ReservaStock")]
    public class ReservaStock
    {
        [Key]
        [Column("IdReserva")]
        public int IdReserva { get; set; }

        [Column("IdProducto")]
        public int IdProducto { get; set; }

        [Column("IdBodega")]
        public int IdBodega { get; set; }

        [Column("Cantidad")]
        public int Cantidad { get; set; }

        [Required]
        [StringLength(20)]
        [Column("Estado")]
        public string Estado { get; set; } = string.Empty;
    }
}
