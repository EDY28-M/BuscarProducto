using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscarProducto.Model
{
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Precio { get; set; }
        public string? Category { get; set; }
    }
}
