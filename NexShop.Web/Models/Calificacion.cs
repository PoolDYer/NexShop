using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexShop.Web.Models
{
    /// <summary>
    /// Entidad Calificacion para rastrear la reputación del vendedor
    /// </summary>
    public class Calificacion
    {
        /// <summary>
        /// Identificador único de la calificación
        /// </summary>
        public int CalificacionId { get; set; }

        /// <summary>
        /// Puntaje de la calificación (1-5 estrellas)
        /// </summary>
        [Required(ErrorMessage = "El puntaje es requerido")]
        [Range(1, 5, ErrorMessage = "El puntaje debe estar entre 1 y 5")]
        public int Puntaje { get; set; }

        /// <summary>
        /// Comentario sobre la calificación
        /// </summary>
        [StringLength(500, ErrorMessage = "El comentario no puede exceder 500 caracteres")]
        public string? Comentario { get; set; }

        /// <summary>
        /// Tipo de calificación: "Comprador", "Vendedor", "Servicio", "Producto"
        /// </summary>
        [Required(ErrorMessage = "El tipo es requerido")]
        [StringLength(30, ErrorMessage = "El tipo no puede exceder 30 caracteres")]
        public string Tipo { get; set; } = "Comprador";

        /// <summary>
        /// Identificador del producto siendo calificado (para reseñas de producto)
        /// </summary>
        public int? ProductoId { get; set; }

        /// <summary>
        /// Identificador de la orden asociada (si aplica)
        /// </summary>
        public int? OrdenId { get; set; }

        /// <summary>
        /// Identificador del vendedor siendo calificado
        /// </summary>
        [Required(ErrorMessage = "El vendedor es requerido")]
        public string VendedorId { get; set; } = string.Empty;

        /// <summary>
        /// Identificador del usuario que califica
        /// </summary>
        [Required(ErrorMessage = "El usuario es requerido")]
        public string UsuarioId { get; set; } = string.Empty;

        /// <summary>
        /// Título de la reseña (para reseñas de producto)
        /// </summary>
        [StringLength(100, ErrorMessage = "El título no puede exceder 100 caracteres")]
        public string? Titulo { get; set; }

        /// <summary>
        /// Calificación de la atención del vendedor (1-5)
        /// </summary>
        public int? CalificacionAtencion { get; set; }

        /// <summary>
        /// Calificación del envío (1-5)
        /// </summary>
        public int? CalificacionEnvio { get; set; }

        /// <summary>
        /// Fecha de la calificación
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Claves foráneas y relaciones

        [ForeignKey(nameof(VendedorId))]
        public virtual Usuario? Vendedor { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey(nameof(ProductoId))]
        public virtual Producto? Producto { get; set; }

        [ForeignKey(nameof(OrdenId))]
        public virtual Orden? Orden { get; set; }
    }

    /// <summary>
    /// Vista o DTO para estadísticas de reputación del vendedor
    /// </summary>
    public class EstadisticasReputacion
    {
        public string VendedorId { get; set; } = string.Empty;
        public int TotalCalificaciones { get; set; }
        public decimal CalificacionPromedio { get; set; }
        public int CalificacionesPositivas { get; set; } // 4-5 estrellas
        public int CalificacionesNeutras { get; set; }   // 3 estrellas
        public int CalificacionesNegativas { get; set; } // 1-2 estrellas
        public decimal PorcentajePositivas { get; set; }
        public decimal PorcentajeNegativas { get; set; }

        /// <summary>
        /// Calcula un índice de reputación (0-100)
        /// </summary>
        public int IndiceReputacion
        {
            get
            {
                if (TotalCalificaciones == 0) return 50; // Neutral si no hay calificaciones

                // Fórmula: (puntaje promedio / 5) * 100
                return (int)Math.Round((CalificacionPromedio / 5m) * 100);
            }
        }

        /// <summary>
        /// Obtiene el nivel de reputación como texto
        /// </summary>
        public string NivelReputacion
        {
            get
            {
                return IndiceReputacion switch
                {
                    >= 90 => "Excelente",
                    >= 75 => "Muy Buena",
                    >= 60 => "Buena",
                    >= 45 => "Regular",
                    >= 30 => "Pobre",
                    _ => "Muy Pobre"
                };
            }
        }

        /// <summary>
        /// Obtiene el color para el termómetro de reputación
        /// </summary>
        public string ColorReputacion
        {
            get
            {
                return IndiceReputacion switch
                {
                    >= 90 => "#2ea44f",      // Verde oscuro
                    >= 75 => "#4CAF50",      // Verde
                    >= 60 => "#FFC107",      // Amarillo
                    >= 45 => "#FF9800",      // Naranja
                    >= 30 => "#FF5722",      // Rojo naranja
                    _ => "#F44336"           // Rojo oscuro
                };
            }
        }

        /// <summary>
        /// Obtiene un emoji representativo
        /// </summary>
        public string EmojiReputacion
        {
            get
            {
                return IndiceReputacion switch
                {
                    >= 90 => "??",
                    >= 75 => "??",
                    >= 60 => "??",
                    >= 45 => "??",
                    >= 30 => "??",
                    _ => "??"
                };
            }
        }
    }
}
