using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosApi.Models
{
    [Table("Pedido")]
    public class Pedido
    {
        [Key]
        [Column("IdPedido")]
        public int IdPedido { get; set; }

        [Column("IdCliente")]
        public int IdCliente { get; set; }

        [Column("Fecha")]
        public DateTime Fecha { get; set; }

        [Column("Total", TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Required]
        [StringLength(20)]
        [Column("Estado")]
        public string Estado { get; set; } = string.Empty;
    }
}
