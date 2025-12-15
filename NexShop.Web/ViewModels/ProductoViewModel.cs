using System.ComponentModel.DataAnnotations;

namespace NexShop.Web.ViewModels
{
    /// <summary>
    /// ViewModel para listar productos con información esencial
    /// </summary>
    public class ProductoListViewModel
    {
        public int ProductoId { get; set; }

        [Display(Name = "Producto")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Precio")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Precio { get; set; }

        [Display(Name = "Stock")]
        public int Stock { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "Categoría")]
        public string CategoriaNombre { get; set; } = string.Empty;

        [Display(Name = "Calificación")]
        public decimal? CalificacionPromedio { get; set; }

        [Display(Name = "Reseñas")]
        public int NumeroResenas { get; set; }

        [Display(Name = "Visualizaciones")]
        public int NumeroVisualizaciones { get; set; }

        [Display(Name = "Imagen Principal")]
        public string? ImagenPrincipal { get; set; }
    }

    /// <summary>
    /// ViewModel detallado para mostrar un producto individual
    /// </summary>
    public class ProductoDetailViewModel
    {
        public int ProductoId { get; set; }

        [Display(Name = "Producto")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Precio")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Precio { get; set; }

        [Display(Name = "Stock Disponible")]
        public int Stock { get; set; }

        [Display(Name = "Stock Mínimo")]
        public int StockMinimo { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "SKU")]
        public string? SKU { get; set; }

        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        public string CategoriaNombre { get; set; } = string.Empty;

        [Display(Name = "Vendedor")]
        public string VendedorId { get; set; } = string.Empty;

        public string VendedorNombre { get; set; } = string.Empty;

        [Display(Name = "Calificación")]
        [DisplayFormat(DataFormatString = "{0:N1}/5")]
        public decimal? CalificacionPromedio { get; set; }

        [Display(Name = "Reseñas")]
        public int NumeroResenas { get; set; }

        [Display(Name = "Visualizaciones")]
        public int NumeroVisualizaciones { get; set; }

        [Display(Name = "Creado")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime FechaCreacion { get; set; }

        public List<MultimediaListViewModel> Multimedia { get; set; } = new();

        public bool StockDisponible => Stock > 0 && Estado == "Disponible";

        /// <summary>
        /// Lista de reseñas del producto
        /// </summary>
        public List<ReseñaProductoDto> Resenas { get; set; } = new();

        /// <summary>
        /// Estadísticas de las reseñas
        /// </summary>
        public EstadisticasReseñasDto? EstadisticasResenas { get; set; }

        /// <summary>
        /// ID del usuario actual para verificar si puede escribir reseña
        /// </summary>
        public string? UsuarioActualId { get; set; }

        /// <summary>
        /// Indica si el usuario actual ya escribió una reseña
        /// </summary>
        public bool UsuarioYaReseno { get; set; }
    }

    /// <summary>
    /// ViewModel para crear un nuevo producto
    /// </summary>
    public class ProductoCreateViewModel
    {
        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(200, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 200 caracteres")]
        [Display(Name = "Nombre del Producto")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(2000, MinimumLength = 10,
            ErrorMessage = "La descripción debe tener entre 10 y 2000 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        [Display(Name = "Stock")]
        public int Stock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo")]
        [Display(Name = "Stock Mínimo")]
        public int StockMinimo { get; set; } = 0;

        [StringLength(50, ErrorMessage = "El SKU no puede exceder 50 caracteres")]
        [Display(Name = "SKU (Código de Producto)")]
        public string? SKU { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        public List<CategoriaSelectViewModel> Categorias { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para editar un producto existente
    /// </summary>
    public class ProductoEditViewModel
    {
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(200, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 200 caracteres")]
        [Display(Name = "Nombre del Producto")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(2000, MinimumLength = 10,
            ErrorMessage = "La descripción debe tener entre 10 y 2000 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        [Display(Name = "Stock")]
        public int Stock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo")]
        [Display(Name = "Stock Mínimo")]
        public int StockMinimo { get; set; } = 0;

        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(30, ErrorMessage = "El estado no puede exceder 30 caracteres")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Disponible";

        [StringLength(50, ErrorMessage = "El SKU no puede exceder 50 caracteres")]
        [Display(Name = "SKU (Código de Producto)")]
        public string? SKU { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        public List<CategoriaSelectViewModel> Categorias { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para agregar producto al carrito
    /// </summary>
    public class AgregarAlCarritoViewModel
    {
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; } = 1;
    }
}
