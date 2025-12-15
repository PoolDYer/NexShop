using System.ComponentModel.DataAnnotations;

namespace NexShop.Web.ViewModels
{
    /// <summary>
    /// ViewModel para un artículo en el carrito de compras
    /// </summary>
    public class CarritoItemViewModel
    {
        public int ProductoId { get; set; }

        [Display(Name = "Producto")]
        public string ProductoNombre { get; set; } = string.Empty;

        [Display(Name = "Precio")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Precio { get; set; }

        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Subtotal => Precio * Cantidad;

        [Display(Name = "Imagen")]
        public string? ImagenUrl { get; set; }

        public int StockDisponible { get; set; }
    }

    /// <summary>
    /// ViewModel del carrito de compras completo
    /// </summary>
    public class CarritoViewModel
    {
        [Display(Name = "Artículos")]
        public List<CarritoItemViewModel> Articulos { get; set; } = new();

        [Display(Name = "Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Subtotal => Articulos.Sum(a => a.Subtotal);

        [Display(Name = "Impuesto (16%)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Impuesto => Math.Round(Subtotal * 0.16m, 2);

        [Display(Name = "Envío")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Envio { get; set; } = 0;

        [Display(Name = "Descuento")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Descuento { get; set; } = 0;

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Total => Subtotal + Impuesto + Envio - Descuento;

        public int TotalArticulos => Articulos.Sum(a => a.Cantidad);

        public bool EstaVacio => !Articulos.Any();
    }

    /// <summary>
    /// ViewModel detallado para un detalles de orden
    /// </summary>
    public class OrdenDetalleListViewModel
    {
        public int OrdenDetalleId { get; set; }

        [Display(Name = "Producto")]
        public string ProductoNombre { get; set; } = string.Empty;

        [Display(Name = "Precio Unitario")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal PrecioUnitario { get; set; }

        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Subtotal { get; set; }
    }

    /// <summary>
    /// ViewModel para listar órdenes del usuario
    /// </summary>
    public class OrdenListViewModel
    {
        public int OrdenId { get; set; }

        [Display(Name = "Número de Orden")]
        public string NumeroOrden { get; set; } = string.Empty;

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal MontoTotal { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "Artículos")]
        public int TotalArticulos { get; set; }
    }

    /// <summary>
    /// ViewModel detallado para ver los detalles de una orden
    /// </summary>
    public class OrdenDetailViewModel
    {
        public int OrdenId { get; set; }

        [Display(Name = "Número de Orden")]
        public string NumeroOrden { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "Fecha de Orden")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Confirmada")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? FechaConfirmacion { get; set; }

        [Display(Name = "Enviada")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? FechaEnvio { get; set; }

        [Display(Name = "Entregada")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? FechaEntrega { get; set; }

        [Display(Name = "Dirección de Entrega")]
        public string DireccionEntrega { get; set; } = string.Empty;

        [Display(Name = "Método de Pago")]
        public string? MetodoPago { get; set; }

        [Display(Name = "Notas")]
        public string? Notas { get; set; }

        [Display(Name = "Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Subtotal { get; set; }

        [Display(Name = "Impuesto")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Impuesto { get; set; }

        [Display(Name = "Envío")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal MontoEnvio { get; set; }

        [Display(Name = "Descuento")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Descuento { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal MontoTotal { get; set; }

        [Display(Name = "Detalles")]
        public List<OrdenDetalleListViewModel> Detalles { get; set; } = new();

        public int TotalArticulos => Detalles.Sum(d => d.Cantidad);
    }

    /// <summary>
    /// ViewModel para crear una orden desde el carrito (Checkout)
    /// </summary>
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "La dirección es requerida")]
        [StringLength(255, MinimumLength = 10,
            ErrorMessage = "La dirección debe tener entre 10 y 255 caracteres")]
        [Display(Name = "Dirección de Entrega")]
        public string DireccionEntrega { get; set; } = string.Empty;

        [Required(ErrorMessage = "El método de pago es requerido")]
        [StringLength(50, ErrorMessage = "El método de pago no puede exceder 50 caracteres")]
        [Display(Name = "Método de Pago")]
        public string MetodoPago { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        [Display(Name = "Notas Adicionales")]
        public string? Notas { get; set; }

        [Display(Name = "He leído y acepto los términos y condiciones")]
        public bool AceptaTerminos { get; set; }

        // Información del carrito (solo lectura)
        [Display(Name = "Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Subtotal { get; set; }

        [Display(Name = "Impuesto")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Impuesto { get; set; }

        [Display(Name = "Envío")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal MontoEnvio { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Total { get; set; }

        public List<CarritoItemViewModel> ArticulosCarrito { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para confirmar orden (resumen final)
    /// </summary>
    public class ConfirmacionOrdenViewModel
    {
        public int OrdenId { get; set; }

        [Display(Name = "Número de Orden")]
        public string NumeroOrden { get; set; } = string.Empty;

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal MontoTotal { get; set; }

        [Display(Name = "Dirección de Entrega")]
        public string DireccionEntrega { get; set; } = string.Empty;

        [Display(Name = "Mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        public List<OrdenDetalleListViewModel> Detalles { get; set; } = new();
    }
}
