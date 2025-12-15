using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexShop.Web.Models
{
    /// <summary>
    /// Entidad Producto con propiedades de Stock, Precio y Estado
    /// </summary>
    public class Producto
    {
        /// <summary>
        /// Identificador único del producto
        /// </summary>
        public int ProductoId { get; set; }

        /// <summary>
        /// Nombre del producto
        /// </summary>
        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(200, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 200 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción detallada del producto
        /// </summary>
        [Required(ErrorMessage = "La descripción del producto es requerida")]
        [StringLength(2000, MinimumLength = 10,
            ErrorMessage = "La descripción debe tener entre 10 y 2000 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>
        /// Precio del producto
        /// </summary>
        [Required(ErrorMessage = "El precio es requerido")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
        public decimal Precio { get; set; }

        /// <summary>
        /// Cantidad de stock disponible del producto
        /// </summary>
        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        /// <summary>
        /// Cantidad mínima de stock para alertas
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo")]
        public int StockMinimo { get; set; } = 0;

        /// <summary>
        /// Estado del producto: "Disponible", "Agotado", "Descontinuado"
        /// </summary>
        [Required(ErrorMessage = "El estado del producto es requerido")]
        [StringLength(30, ErrorMessage = "El estado no puede exceder 30 caracteres")]
        public string Estado { get; set; } = "Disponible";

        /// <summary>
        /// SKU (Código de unidad de existencia) único del producto
        /// </summary>
        [StringLength(50, ErrorMessage = "El SKU no puede exceder 50 caracteres")]
        public string? SKU { get; set; }

        /// <summary>
        /// Calificación promedio del producto (1-5 estrellas)
        /// </summary>
        [Range(0, 5, ErrorMessage = "La calificación debe estar entre 0 y 5")]
        public decimal? CalificacionPromedio { get; set; }

        /// <summary>
        /// Número de reseñas del producto
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "El número de reseñas no puede ser negativo")]
        public int NumeroResenas { get; set; } = 0;

        /// <summary>
        /// Número de veces que se ha visualizado el producto
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "El número de visualizaciones no puede ser negativo")]
        public int NumeroVisualizaciones { get; set; } = 0;

        /// <summary>
        /// Fecha de creación del producto
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización del producto
        /// </summary>
        public DateTime? FechaActualizacion { get; set; }

        // Claves foráneas

        /// <summary>
        /// Identificador de la categoría a la que pertenece el producto
        /// </summary>
        [Required(ErrorMessage = "La categoría es requerida")]
        public int CategoriaId { get; set; }

        /// <summary>
        /// Identificador del vendedor (usuario) que ofrece el producto
        /// </summary>
        [Required(ErrorMessage = "El vendedor es requerido")]
        public string VendedorId { get; set; } = string.Empty;

        // Relaciones de navegación

        /// <summary>
        /// Navegación a la categoría del producto
        /// </summary>
        [ForeignKey(nameof(CategoriaId))]
        public virtual Categoria? Categoria { get; set; }

        /// <summary>
        /// Navegación al usuario vendedor del producto
        /// </summary>
        [ForeignKey(nameof(VendedorId))]
        public virtual Usuario? Vendedor { get; set; }

        /// <summary>
        /// Colección de multimedia (fotos/videos) del producto
        /// </summary>
        public ICollection<Multimedia> Multimedia { get; set; } = new List<Multimedia>();

        /// <summary>
        /// Colección de detalles de órdenes que incluyen este producto
        /// </summary>
        public ICollection<OrdenDetalle> DetallesOrdenes { get; set; } = new List<OrdenDetalle>();
    }
}
