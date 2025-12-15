using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexShop.Web.Models
{
    /// <summary>
    /// Entidad Respuesta para el sistema de Q&A
    /// </summary>
    public class Respuesta
    {
        /// <summary>
        /// Identificador único de la respuesta
        /// </summary>
        public int RespuestaId { get; set; }

        /// <summary>
        /// Texto de la respuesta
        /// </summary>
        [Required(ErrorMessage = "La respuesta es requerida")]
        [StringLength(2000, MinimumLength = 5,
            ErrorMessage = "La respuesta debe tener entre 5 y 2000 caracteres")]
        public string Contenido { get; set; } = string.Empty;

        /// <summary>
        /// Identificador de la pregunta a la que corresponde
        /// </summary>
        [Required(ErrorMessage = "La pregunta es requerida")]
        public int PreguntaId { get; set; }

        /// <summary>
        /// Identificador del usuario que responde (típicamente el vendedor)
        /// </summary>
        [Required(ErrorMessage = "El usuario es requerido")]
        public string UsuarioId { get; set; } = string.Empty;

        /// <summary>
        /// Indica si es la respuesta oficial del vendedor
        /// </summary>
        public bool EsRespuestaOficial { get; set; } = false;

        /// <summary>
        /// Calificación de la respuesta (votos útiles)
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "La calificación no puede ser negativa")]
        public int VotosUtiles { get; set; } = 0;

        /// <summary>
        /// Fecha de creación de la respuesta
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? FechaActualizacion { get; set; }

        // Claves foráneas y relaciones

        [ForeignKey(nameof(PreguntaId))]
        public virtual Pregunta? Pregunta { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }
    }
}
