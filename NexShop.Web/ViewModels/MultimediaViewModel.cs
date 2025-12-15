using System.ComponentModel.DataAnnotations;

namespace NexShop.Web.ViewModels
{
    /// <summary>
    /// ViewModel para mostrar archivos multimedia
    /// </summary>
    public class MultimediaListViewModel
    {
        public int MultimediaId { get; set; }

        [Display(Name = "Tipo")]
        public string Tipo { get; set; } = string.Empty;

        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "URL")]
        public string Url { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Principal")]
        public bool EsPrincipal { get; set; }

        [Display(Name = "Orden")]
        public int Orden { get; set; }

        [Display(Name = "Tamaño (KB)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long TamanoKB => TamanoBytes / 1024;

        public long TamanoBytes { get; set; }

        [Display(Name = "Tipo MIME")]
        public string? TipoMime { get; set; }
    }

    /// <summary>
    /// ViewModel para subir/crear multimedia
    /// </summary>
    public class MultimediaCreateViewModel
    {
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un archivo")]
        [Display(Name = "Archivo")]
        public IFormFile? Archivo { get; set; }

        [StringLength(255, ErrorMessage = "El nombre no puede exceder 255 caracteres")]
        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Establecer como principal")]
        public bool EsPrincipal { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El orden debe ser 0 o mayor")]
        [Display(Name = "Orden de Visualización")]
        public int Orden { get; set; } = 0;
    }
}
