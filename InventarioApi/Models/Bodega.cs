using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApi.Models
{
    [Table("Bodega")]
    public class Bodega
    {
        [Key]
        [Column("IdBodega")]
        public int IdBodega { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        [Column("Ciudad")]
        public string Ciudad { get; set; } = string.Empty;

        [StringLength(200)]
        [Column("Direccion")]
        public string Direccion { get; set; } = string.Empty;
    }
}
