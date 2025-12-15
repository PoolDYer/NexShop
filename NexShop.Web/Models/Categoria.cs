using System.ComponentModel.DataAnnotations;

namespace NexShop.Web.Models
{
    /// <summary>
    /// Entidad Categoria para clasificar productos
    /// </summary>
    public class Categoria
    {
        /// <summary>
        /// Identificador único de la categoría
        /// </summary>
        public int CategoriaId { get; set; }

        /// <summary>
        /// Nombre de la categoría
        /// </summary>
        [Required(ErrorMessage = "El nombre de la categoría es requerido")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción detallada de la categoría
        /// </summary>
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        /// <summary>
        /// URL o ruta del icono de la categoría
        /// </summary>
        [StringLength(255, ErrorMessage = "La URL del icono no puede exceder 255 caracteres")]
        public string? IconoUrl { get; set; }

        /// <summary>
        /// Indica si la categoría está activa
        /// </summary>
        public bool EstaActiva { get; set; } = true;

        /// <summary>
        /// Fecha de creación de la categoría
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización de la categoría
        /// </summary>
        public DateTime? FechaActualizacion { get; set; }

        // Relaciones de navegación

        /// <summary>
        /// Colección de productos pertenecientes a esta categoría
        /// </summary>
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
