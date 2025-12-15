using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexShop.Web.Models
{
    /// <summary>
    /// Entidad Multimedia para fotos, videos y otros archivos multimedia asociados a productos
    /// </summary>
    public class Multimedia
    {
        /// <summary>
        /// Identificador único del archivo multimedia
        /// </summary>
        public int MultimediaId { get; set; }

        /// <summary>
        /// Tipo de multimedia: "Foto", "Video", "Documento"
        /// </summary>
        [Required(ErrorMessage = "El tipo de multimedia es requerido")]
        [StringLength(30, ErrorMessage = "El tipo no puede exceder 30 caracteres")]
        public string Tipo { get; set; } = "Foto";

        /// <summary>
        /// Nombre o título del archivo multimedia
        /// </summary>
        [Required(ErrorMessage = "El nombre del archivo es requerido")]
        [StringLength(255, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 255 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// URL o ruta del archivo multimedia
        /// </summary>
        [Required(ErrorMessage = "La URL del archivo es requerida")]
        [StringLength(500, ErrorMessage = "La URL no puede exceder 500 caracteres")]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del archivo en el servidor
        /// </summary>
        [StringLength(255, ErrorMessage = "El nombre del archivo no puede exceder 255 caracteres")]
        public string? NombreArchivo { get; set; }

        /// <summary>
        /// Descripción o alt-text del archivo multimedia
        /// </summary>
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        /// <summary>
        /// Tamaño del archivo en bytes
        /// </summary>
        public long TamanoBytes { get; set; }

        /// <summary>
        /// Tipo MIME del archivo (ej: image/jpeg, video/mp4)
        /// </summary>
        [StringLength(50, ErrorMessage = "El tipo MIME no puede exceder 50 caracteres")]
        public string? TipoMime { get; set; }

        /// <summary>
        /// Orden de visualización del multimedia para el producto
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "El orden no puede ser negativo")]
        public int Orden { get; set; } = 0;

        /// <summary>
        /// Indica si este es el multimedia principal del producto
        /// </summary>
        public bool EsPrincipal { get; set; } = false;

        /// <summary>
        /// Indica si el multimedia está activo/visible
        /// </summary>
        public bool EstaActivo { get; set; } = true;

        /// <summary>
        /// Fecha de carga del archivo multimedia
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización del registro
        /// </summary>
        public DateTime? FechaActualizacion { get; set; }

        // Claves foráneas

        /// <summary>
        /// Identificador del producto al que pertenece este multimedia
        /// </summary>
        [Required(ErrorMessage = "El producto es requerido")]
        public int ProductoId { get; set; }

        // Relaciones de navegación

        /// <summary>
        /// Navegación al producto propietario de este multimedia
        /// </summary>
        [ForeignKey(nameof(ProductoId))]
        public virtual Producto? Producto { get; set; }
    }
}
