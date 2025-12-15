using System.ComponentModel.DataAnnotations;
using NexShop.Web.Models;

namespace NexShop.Web.ViewModels
{
    /// <summary>
    /// ViewModel para crear una pregunta
    /// </summary>
    public class PreguntaCreateViewModel
    {
        [Required(ErrorMessage = "La pregunta es requerida")]
        [StringLength(500, MinimumLength = 5, 
            ErrorMessage = "La pregunta debe tener entre 5 y 500 caracteres")]
        [Display(Name = "Tu Pregunta")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
        [Display(Name = "Detalles adicionales (opcional)")]
        public string? Descripcion { get; set; }

        public int ProductoId { get; set; }
    }

    /// <summary>
    /// ViewModel para mostrar una pregunta en la lista
    /// </summary>
    public class PreguntaListViewModel
    {
        public int PreguntaId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public int ProductoId { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public int NumeroRespuestas { get; set; }
        public int VotosUtiles { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<RespuestaListViewModel> Respuestas { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para crear una respuesta
    /// </summary>
    public class RespuestaCreateViewModel
    {
        [Required(ErrorMessage = "La respuesta es requerida")]
        [StringLength(2000, MinimumLength = 5,
            ErrorMessage = "La respuesta debe tener entre 5 y 2000 caracteres")]
        [Display(Name = "Respuesta")]
        public string Contenido { get; set; } = string.Empty;

        public int PreguntaId { get; set; }
    }

    /// <summary>
    /// ViewModel para mostrar una respuesta
    /// </summary>
    public class RespuestaListViewModel
    {
        public int RespuestaId { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public bool EsRespuestaOficial { get; set; }
        public int VotosUtiles { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    /// <summary>
    /// ViewModel para mostrar el resumen de Q&A en detalle de producto
    /// </summary>
    public class PreguntasResumenViewModel
    {
        public int ProductoId { get; set; }
        public int TotalPreguntas { get; set; }
        public int PreguntasPendientes { get; set; }
        public int PreguntasRespondidas { get; set; }
        public List<PreguntaListViewModel> Preguntas { get; set; } = new();
        public PreguntaCreateViewModel FormularioNuevaPregunta { get; set; } = new();
    }
}
