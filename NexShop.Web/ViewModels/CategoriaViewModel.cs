using System.ComponentModel.DataAnnotations;

namespace NexShop.Web.ViewModels
{
    /// <summary>
    /// ViewModel para listar categorías en el formato esperado por las vistas
    /// </summary>
    public class CategoriaListViewModel
    {
        public int CategoriaId { get; set; }

        [Display(Name = "Categoría")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Icono")]
        public string? IconoUrl { get; set; }

        [Display(Name = "Activa")]
        public bool EstaActiva { get; set; }
    }

    /// <summary>
    /// ViewModel para crear/editar categorías
    /// </summary>
    public class CategoriaEditViewModel
    {
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es requerido")]
        [StringLength(100, MinimumLength = 3, 
            ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        [Display(Name = "Nombre de la Categoría")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [StringLength(255, ErrorMessage = "La URL del icono no puede exceder 255 caracteres")]
        [Url(ErrorMessage = "Ingrese una URL válida")]
        [Display(Name = "URL del Icono")]
        public string? IconoUrl { get; set; }

        [Display(Name = "Activa")]
        public bool EstaActiva { get; set; } = true;
    }

    /// <summary>
    /// ViewModel para seleccionar categoría en dropdown
    /// </summary>
    public class CategoriaSelectViewModel
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
