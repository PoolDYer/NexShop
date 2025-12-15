using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexShop.Web.Models
{
    /// <summary>
    /// Entidad Pregunta para el sistema de Q&A de productos
    /// </summary>
    public class Pregunta
    {
        /// <summary>
        /// Identificador único de la pregunta
        /// </summary>
        public int PreguntaId { get; set; }

        /// <summary>
        /// Texto de la pregunta
        /// </summary>
        [Required(ErrorMessage = "La pregunta es requerida")]
        [StringLength(500, MinimumLength = 5,
            ErrorMessage = "La pregunta debe tener entre 5 y 500 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descripción detallada de la pregunta (opcional)
        /// </summary>
        [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
        public string? Descripcion { get; set; }

        /// <summary>
        /// Identificador del producto al que corresponde la pregunta
        /// </summary>
        [Required(ErrorMessage = "El producto es requerido")]
        public int ProductoId { get; set; }

        /// <summary>
        /// Identificador del usuario que hizo la pregunta
        /// </summary>
        [Required(ErrorMessage = "El usuario es requerido")]
        public string UsuarioId { get; set; } = string.Empty;

        /// <summary>
        /// Estado de la pregunta: "Pendiente", "Respondida", "Cerrada"
        /// </summary>
        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(20, ErrorMessage = "El estado no puede exceder 20 caracteres")]
        public string Estado { get; set; } = "Pendiente";

        /// <summary>
        /// Número de respuestas a esta pregunta
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "El número de respuestas no puede ser negativo")]
        public int NumeroRespuestas { get; set; } = 0;

        /// <summary>
        /// Calificación útil de la pregunta (votos positivos)
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "La calificación no puede ser negativa")]
        public int VotosUtiles { get; set; } = 0;

        /// <summary>
        /// Fecha de creación de la pregunta
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? FechaActualizacion { get; set; }

        // Claves foráneas y relaciones

        [ForeignKey(nameof(ProductoId))]
        public virtual Producto? Producto { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }

        /// <summary>
        /// Colección de respuestas a esta pregunta
        /// </summary>
        public ICollection<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
    }
}
