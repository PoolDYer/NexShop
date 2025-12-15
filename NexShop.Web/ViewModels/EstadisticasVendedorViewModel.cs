using System.ComponentModel.DataAnnotations;

namespace NexShop.Web.ViewModels
{
    /// <summary>
    /// ViewModel para estadísticas detalladas de ventas del vendedor
    /// </summary>
    public class EstadisticasVendedorViewModel
    {
        /// <summary>
        /// Información del usuario vendedor
        /// </summary>
        public string VendedorId { get; set; } = string.Empty;
        public string VendedorNombre { get; set; } = string.Empty;
        public string? VendedorEmail { get; set; }

        /// <summary>
        /// Métricas Principales
        /// </summary>
        public int TotalProductos { get; set; }
        public int ProductosActivos { get; set; }
        public int ProductosAgotados { get; set; }
        public decimal TotalVentas { get; set; }
        public int TotalOrdenes { get; set; }
        public int TotalUnidadesVendidas { get; set; }
        public decimal TicketPromedio { get; set; }
        public double CalificacionPromedio { get; set; }

        /// <summary>
        /// Estadísticas de Productos
        /// </summary>
        public List<ProductoEstadisticaDto> ProductosMasVendidos { get; set; } = new();
        public List<ProductoEstadisticaDto> ProductosMejorCalificados { get; set; } = new();
        public List<ProductoEstadisticaDto> ProductosMasVisualizados { get; set; } = new();

        /// <summary>
        /// Estadísticas de Visitas
        /// </summary>
        public int VisitasTotales { get; set; }
        public double TasaConversionPromedio { get; set; }
        public int VisitasEsteMes { get; set; }
        public int VentasEsteMes { get; set; }
        public decimal VentasEsteMesMonto { get; set; }

        /// <summary>
        /// Estadísticas de Tiempo
        /// </summary>
        public DateTime PrimerVenta { get; set; }
        public DateTime UltimaVenta { get; set; }
        public int DiasComoComerciante { get; set; }
        public double VentasPromedioPorDia { get; set; }

        /// <summary>
        /// Información adicional
        /// </summary>
        public int ResenasTotal { get; set; }
        public decimal IngresoPromedioPorProducto { get; set; }
        public decimal StockTotalValor { get; set; }
        public int ClientesUnicos { get; set; }

        /// <summary>
        /// Top 5 - Productos
        /// </summary>
        public List<ProductoTopDto> Top5MasVendidos { get; set; } = new();
        public List<ProductoTopDto> Top5MejorCalificados { get; set; } = new();
    }

    /// <summary>
    /// DTO para estadísticas de un producto específico
    /// </summary>
    public class ProductoEstadisticaDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int UnidadesVendidas { get; set; }
        public decimal VentasTotal { get; set; }
        public int Visualizaciones { get; set; }
        public double Calificacion { get; set; }
        public int NumeroResenas { get; set; }
        public double TasaConversion { get; set; }
    }

    /// <summary>
    /// DTO para top de productos
    /// </summary>
    public class ProductoTopDto
    {
        public int Posicion { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string Metrica { get; set; } = string.Empty; // "unidades", "dinero", "calificación", etc.
    }

    /// <summary>
    /// DTO para datos mensuales
    /// </summary>
    public class DatosMenualDto
    {
        public string Mes { get; set; } = string.Empty;
        public int Ventas { get; set; }
        public decimal Monto { get; set; }
        public int UnidadesVendidas { get; set; }
    }
}
