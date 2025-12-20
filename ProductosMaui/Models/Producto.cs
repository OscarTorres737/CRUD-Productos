using System.ComponentModel.DataAnnotations;

namespace ProductosMaui.Models
{
    public class Producto
    {

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string SKU { get; set; } = string.Empty;

        [Range(0, 99999999.99)]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaAlta { get; set; }
    }
}
