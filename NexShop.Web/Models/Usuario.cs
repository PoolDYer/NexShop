using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NexShop.Web.Models
{
    /// <summary>
    /// Entidad Usuario que extiende IdentityUser con roles de Comprador/Vendedor
    /// </summary>
    public class Usuario : IdentityUser
    {
        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        [Required(ErrorMessage = "El nombre completo es requerido")]
        [StringLength(150, MinimumLength = 3, 
            ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres")]
        public string NombreCompleto { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de rol del usuario: "Comprador" o "Vendedor"
        /// </summary>
        [Required(ErrorMessage = "El tipo de usuario es requerido")]
        [StringLength(20, ErrorMessage = "El tipo de usuario no puede exceder 20 caracteres")]
        public string TipoUsuario { get; set; } = "Comprador"; // Valor por defecto

        /// <summary>
        /// Descripción o biografía del usuario (especialmente relevante para vendedores)
        /// </summary>
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        /// <summary>
        /// Dirección del usuario
        /// </summary>
        [StringLength(255, ErrorMessage = "La dirección no puede exceder 255 caracteres")]
        public string? Direccion { get; set; }

        /// <summary>
        /// Número de teléfono del usuario (ya existe en IdentityUser, lo documentamos)
        /// </summary>
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public override string? PhoneNumber { get; set; }

        /// <summary>
        /// Indica si la cuenta del usuario está activa
        /// </summary>
        public bool EstaActivo { get; set; } = true;

        /// <summary>
        /// Fecha de creación del usuario
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización del usuario
        /// </summary>
        public DateTime? FechaActualizacion { get; set; }

        /// <summary>
        /// Calificación promedio del usuario (1-5 estrellas)
        /// </summary>
        [Range(0, 5, ErrorMessage = "La calificación debe estar entre 0 y 5")]
        public decimal? CalificacionPromedio { get; set; }

        // Relaciones de navegación

        /// <summary>
        /// Colección de órdenes del usuario (como comprador)
        /// </summary>
        public ICollection<Orden> Ordenes { get; set; } = new List<Orden>();

        /// <summary>
        /// Colección de productos vendidos por el usuario (si es vendedor)
        /// </summary>
        public ICollection<Producto> ProductosVendidos { get; set; } = new List<Producto>();
    }
}
