using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApi.Models
{
    [Table("Existencia")]
    public class Existencia
    {
        [Key]
        [Column("IdExistencia")]
        public int IdExistencia { get; set; }

        [Column("IdProducto")]
        public int IdProducto { get; set; }

        [Column("IdBodega")]
        public int IdBodega { get; set; }

        [Column("Stock")]
        public int Stock { get; set; }
    }
}
