using System.ComponentModel.DataAnnotations;

namespace NexShop.Web.ViewModels
{
    /// <summary>
    /// ViewModel para crear una reseña de producto
    /// </summary>
    public class CrearReseñaViewModel
    {
        public int ProductoId { get; set; }

        [Display(Name = "Calificación General")]
        [Range(1, 5, ErrorMessage = "Debe seleccionar una calificación entre 1 y 5 estrellas")]
        public int CalificacionGeneral { get; set; }

        [Display(Name = "Calificación de Atención")]
        [Range(1, 5, ErrorMessage = "Debe seleccionar una calificación")]
        public int CalificacionAtencion { get; set; }

        [Display(Name = "Calificación de Envío")]
        [Range(1, 5, ErrorMessage = "Debe seleccionar una calificación")]
        public int CalificacionEnvio { get; set; }

        [Display(Name = "Comentario")]
        [StringLength(500, MinimumLength = 10, 
            ErrorMessage = "El comentario debe tener entre 10 y 500 caracteres")]
        [Required(ErrorMessage = "El comentario es requerido")]
        public string Comentario { get; set; } = string.Empty;

        [Display(Name = "Título de la Reseña")]
        [StringLength(100, MinimumLength = 5,
            ErrorMessage = "El título debe tener entre 5 y 100 caracteres")]
        [Required(ErrorMessage = "El título es requerido")]
        public string Titulo { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para mostrar una reseña
    /// </summary>
    public class ReseñaProductoDto
    {
        public int CalificacionId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public int Puntaje { get; set; }
        public string? Comentario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? CalificacionAtencion { get; set; }
        public int? CalificacionEnvio { get; set; }
        public string? Titulo { get; set; }
        public bool EsDelUsuarioActual { get; set; }
    }

    /// <summary>
    /// Estadísticas de reseñas del producto
    /// </summary>
    public class EstadisticasReseñasDto
    {
        public int TotalResenas { get; set; }
        public decimal CalificacionPromedio { get; set; }
        public int CalificacionesAtencion { get; set; }
        public int CalificacionesEnvio { get; set; }
        public Dictionary<int, int> DistribucionEstrellas { get; set; } = new();
        public int PorcentajeRecomendaciones { get; set; }
    }
}
