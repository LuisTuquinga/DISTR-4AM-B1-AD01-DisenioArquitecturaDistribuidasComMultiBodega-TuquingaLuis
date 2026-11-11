using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogoApi.Models
{
    [Table("Producto")]
    public class Producto
    {
        [Key]
        [Column("IdProducto")]
        public int IdProducto { get; set; }

        [Column("IdCategoria")]
        public int IdCategoria { get; set; }

        [Column("IdMarca")]
        public int IdMarca { get; set; }

        [Required]
        [StringLength(150)]
        [Column("Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50)]
        [Column("SKU")]
        public string SKU { get; set; } = string.Empty;

        [Column("Precio", TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }
    }
}
