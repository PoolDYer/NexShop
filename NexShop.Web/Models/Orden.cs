using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexShop.Web.Models
{
    /// <summary>
    /// Entidad Orden que representa una compra realizada por un usuario
    /// </summary>
    public class Orden
    {
        /// <summary>
        /// Identificador único de la orden
        /// </summary>
        public int OrdenId { get; set; }

        /// <summary>
        /// Número de orden único para referencia del cliente
        /// </summary>
        [Required(ErrorMessage = "El número de orden es requerido")]
        [StringLength(50, ErrorMessage = "El número de orden no puede exceder 50 caracteres")]
        public string NumeroOrden { get; set; } = string.Empty;

        /// <summary>
        /// Monto total de la orden
        /// </summary>
        [Required(ErrorMessage = "El monto total es requerido")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, 999999.99, ErrorMessage = "El monto debe estar entre 0.01 y 999999.99")]
        public decimal MontoTotal { get; set; }

        /// <summary>
        /// Monto del impuesto
        /// </summary>
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, 999999.99, ErrorMessage = "El impuesto no puede ser negativo")]
        public decimal Impuesto { get; set; } = 0;

        /// <summary>
        /// Monto de envío
        /// </summary>
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, 999999.99, ErrorMessage = "El envío no puede ser negativo")]
        public decimal MontoEnvio { get; set; } = 0;

        /// <summary>
        /// Descuento aplicado a la orden
        /// </summary>
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, 999999.99, ErrorMessage = "El descuento no puede ser negativo")]
        public decimal Descuento { get; set; } = 0;

        /// <summary>
        /// Estado de la orden: "Pendiente", "Confirmada", "En Envío", "Entregada", "Cancelada"
        /// </summary>
        [Required(ErrorMessage = "El estado de la orden es requerido")]
        [StringLength(30, ErrorMessage = "El estado no puede exceder 30 caracteres")]
        public string Estado { get; set; } = "Pendiente";

        /// <summary>
        /// Método de pago utilizado
        /// </summary>
        [StringLength(50, ErrorMessage = "El método de pago no puede exceder 50 caracteres")]
        public string? MetodoPago { get; set; }

        /// <summary>
        /// Dirección de entrega de la orden
        /// </summary>
        [Required(ErrorMessage = "La dirección de entrega es requerida")]
        [StringLength(255, ErrorMessage = "La dirección no puede exceder 255 caracteres")]
        public string DireccionEntrega { get; set; } = string.Empty;

        /// <summary>
        /// Notas o comentarios adicionales sobre la orden
        /// </summary>
        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        public string? Notas { get; set; }

        /// <summary>
        /// Fecha en que se creó la orden
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de confirmación de la orden
        /// </summary>
        public DateTime? FechaConfirmacion { get; set; }

        /// <summary>
        /// Fecha de envío de la orden
        /// </summary>
        public DateTime? FechaEnvio { get; set; }

        /// <summary>
        /// Fecha de entrega de la orden
        /// </summary>
        public DateTime? FechaEntrega { get; set; }

        /// <summary>
        /// Fecha de cancelación de la orden (si aplica)
        /// </summary>
        public DateTime? FechaCancelacion { get; set; }

        // Claves foráneas

        /// <summary>
        /// Identificador del comprador (usuario)
        /// </summary>
        [Required(ErrorMessage = "El comprador es requerido")]
        public string CompradorId { get; set; } = string.Empty;

        // Relaciones de navegación

        /// <summary>
        /// Navegación al usuario comprador de la orden
        /// </summary>
        [ForeignKey(nameof(CompradorId))]
        public virtual Usuario? Comprador { get; set; }

        /// <summary>
        /// Colección de detalles de la orden (productos incluidos)
        /// </summary>
        public ICollection<OrdenDetalle> Detalles { get; set; } = new List<OrdenDetalle>();
    }

    /// <summary>
    /// Entidad OrdenDetalle que representa los productos incluidos en una orden
    /// </summary>
    public class OrdenDetalle
    {
        /// <summary>
        /// Identificador único del detalle de orden
        /// </summary>
        public int OrdenDetalleId { get; set; }

        /// <summary>
        /// Cantidad del producto en esta línea de orden
        /// </summary>
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        /// <summary>
        /// Precio unitario del producto en el momento de la orden
        /// </summary>
        [Required(ErrorMessage = "El precio unitario es requerido")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
        public decimal PrecioUnitario { get; set; }

        /// <summary>
        /// Subtotal de la línea (Cantidad * PrecioUnitario)
        /// </summary>
        [Required(ErrorMessage = "El subtotal es requerido")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, 999999.99, ErrorMessage = "El subtotal debe estar entre 0.01 y 999999.99")]
        public decimal Subtotal { get; set; }

        // Claves foráneas

        /// <summary>
        /// Identificador de la orden a la que pertenece este detalle
        /// </summary>
        [Required(ErrorMessage = "La orden es requerida")]
        public int OrdenId { get; set; }

        /// <summary>
        /// Identificador del producto en este detalle
        /// </summary>
        [Required(ErrorMessage = "El producto es requerido")]
        public int ProductoId { get; set; }

        // Relaciones de navegación

        /// <summary>
        /// Navegación a la orden de este detalle
        /// </summary>
        [ForeignKey(nameof(OrdenId))]
        public virtual Orden? Orden { get; set; }

        /// <summary>
        /// Navegación al producto de este detalle
        /// </summary>
        [ForeignKey(nameof(ProductoId))]
        public virtual Producto? Producto { get; set; }
    }
}
