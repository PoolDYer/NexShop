using NexShop.Web.Models;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Resultado de operación de stock
    /// </summary>
    public class ResultadoStock
    {
        /// <summary>
        /// Indica si la operación fue exitosa
        /// </summary>
        public bool EsExitoso { get; set; }

        /// <summary>
        /// Mensaje descriptivo
        /// </summary>
        public string Mensaje { get; set; } = string.Empty;

        /// <summary>
        /// Stock anterior
        /// </summary>
        public int StockAnterior { get; set; }

        /// <summary>
        /// Stock nuevo después de la operación
        /// </summary>
        public int StockNuevo { get; set; }

        /// <summary>
        /// Cantidad de unidades que se procesaron
        /// </summary>
        public int UnidadesProcessadas { get; set; }

        /// <summary>
        /// Estado anterior del producto
        /// </summary>
        public string EstadoAnterior { get; set; } = string.Empty;

        /// <summary>
        /// Estado nuevo del producto
        /// </summary>
        public string EstadoNuevo { get; set; } = string.Empty;
    }

    /// <summary>
    /// Servicio para gestionar el stock de productos
    /// Cada producto representa unidades individuales
    /// Ej: Power Bank 3000 con Stock=100 significa 100 unidades disponibles
    /// </summary>
    public interface IStockService
    {
        /// <summary>
        /// Valida si hay suficiente stock disponible
        /// </summary>
        /// <param name="producto">El producto a validar</param>
        /// <param name="cantidadSolicitada">Cantidad de unidades solicitadas</param>
        /// <returns>True si hay stock suficiente</returns>
        bool HaySufficientStock(Producto producto, int cantidadSolicitada);

        /// <summary>
        /// Valida si hay suficiente stock y devuelve un mensaje detallado
        /// </summary>
        Task<ResultadoStock> ValidarStockAsync(Producto producto, int cantidadSolicitada);

        /// <summary>
        /// Reduce el stock de un producto tras una compra exitosa
        /// </summary>
        /// <param name="producto">El producto a actualizar</param>
        /// <param name="cantidadComprada">Unidades compradas</param>
        /// <returns>Resultado de la operación</returns>
        Task<ResultadoStock> ReducirStockAsync(Producto producto, int cantidadComprada);

        /// <summary>
        /// Revierte el stock tras una compra cancelada
        /// </summary>
        /// <param name="producto">El producto a revertir</param>
        /// <param name="cantidadARevertir">Unidades a revertir</param>
        /// <returns>Resultado de la operación</returns>
        Task<ResultadoStock> RevertirStockAsync(Producto producto, int cantidadARevertir);

        /// <summary>
        /// Obtiene el mensaje de estado del producto
        /// </summary>
        string ObtenerMensajeEstado(Producto producto);

        /// <summary>
        /// Obtiene el porcentaje de disponibilidad del producto
        /// </summary>
        int ObtenerPorcentajeDisponibilidad(Producto producto);
    }

    /// <summary>
    /// Implementación del servicio de gestión de stock
    /// </summary>
    public class StockService : IStockService
    {
        private readonly ILogger<StockService> _logger;

        public StockService(ILogger<StockService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Valida si hay suficiente stock disponible
        /// </summary>
        public bool HaySufficientStock(Producto producto, int cantidadSolicitada)
        {
            if (producto == null)
            {
                _logger.LogWarning("Intento de validar stock en producto nulo");
                return false;
            }

            bool haySuficiente = producto.Stock >= cantidadSolicitada;

            if (!haySuficiente)
            {
                _logger.LogWarning("Stock insuficiente. Producto: {ProductoId}, Solicitado: {Cantidad}, Disponible: {Stock}",
                    producto.ProductoId, cantidadSolicitada, producto.Stock);
            }

            return haySuficiente;
        }

        /// <summary>
        /// Valida si hay suficiente stock y devuelve un mensaje detallado
        /// </summary>
        public async Task<ResultadoStock> ValidarStockAsync(Producto producto, int cantidadSolicitada)
        {
            try
            {
                if (producto == null)
                {
                    return new ResultadoStock
                    {
                        EsExitoso = false,
                        Mensaje = "El producto no existe",
                        StockAnterior = 0,
                        StockNuevo = 0
                    };
                }

                if (cantidadSolicitada <= 0)
                {
                    return new ResultadoStock
                    {
                        EsExitoso = false,
                        Mensaje = "La cantidad solicitada debe ser mayor a 0",
                        StockAnterior = producto.Stock,
                        StockNuevo = producto.Stock
                    };
                }

                // Validar si hay suficiente stock
                if (producto.Stock < cantidadSolicitada)
                {
                    _logger.LogWarning("Stock insuficiente para {ProductoNombre}. Solicitado: {Cantidad}, Disponible: {Stock}",
                        producto.Nombre, cantidadSolicitada, producto.Stock);

                    return new ResultadoStock
                    {
                        EsExitoso = false,
                        Mensaje = $"Stock insuficiente. Disponibles: {producto.Stock} unidades, solicitadas: {cantidadSolicitada}",
                        StockAnterior = producto.Stock,
                        StockNuevo = producto.Stock,
                        UnidadesProcessadas = 0
                    };
                }

                // Hay suficiente stock
                _logger.LogInformation("Stock validado exitosamente para {ProductoNombre}. Stock: {Stock}",
                    producto.Nombre, producto.Stock);

                return new ResultadoStock
                {
                    EsExitoso = true,
                    Mensaje = $"Stock disponible: {producto.Stock} unidades",
                    StockAnterior = producto.Stock,
                    StockNuevo = producto.Stock - cantidadSolicitada,
                    UnidadesProcessadas = cantidadSolicitada
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar stock del producto {ProductoId}", producto?.ProductoId);
                return new ResultadoStock
                {
                    EsExitoso = false,
                    Mensaje = "Error al validar el stock"
                };
            }
        }

        /// <summary>
        /// Reduce el stock de un producto tras una compra exitosa
        /// Cada unidad comprada reduce el stock en 1
        /// Si stock llega a 0, marca producto como "Agotado"
        /// </summary>
        public async Task<ResultadoStock> ReducirStockAsync(Producto producto, int cantidadComprada)
        {
            try
            {
                if (producto == null)
                {
                    return new ResultadoStock
                    {
                        EsExitoso = false,
                        Mensaje = "El producto no existe"
                    };
                }

                // Validar que hay suficiente stock
                if (producto.Stock < cantidadComprada)
                {
                    _logger.LogError("Intento de reducir más stock del disponible. Producto: {ProductoId}, Stock: {Stock}, Solicitado: {Cantidad}",
                        producto.ProductoId, producto.Stock, cantidadComprada);

                    return new ResultadoStock
                    {
                        EsExitoso = false,
                        Mensaje = "No hay suficiente stock para completar la compra",
                        StockAnterior = producto.Stock,
                        StockNuevo = producto.Stock
                    };
                }

                // Guardar valores anteriores
                int stockAnterior = producto.Stock;
                string estadoAnterior = producto.Estado;

                // REDUCIR STOCK: -1 por cada unidad comprada
                producto.Stock -= cantidadComprada;

                // Actualizar estado basado en stock nuevo
                string estadoNuevo = (producto.Stock <= 0) ? "Agotado" : "Disponible";

                if (producto.Stock <= 0)
                {
                    producto.Stock = 0;
                    producto.Estado = "Agotado";
                }
                else if (producto.Stock > 0 && estadoAnterior == "Agotado")
                {
                    // Si volvía a haber stock, cambiar de agotado a disponible
                    producto.Estado = "Disponible";
                }

                producto.FechaActualizacion = DateTime.UtcNow;

                _logger.LogInformation("Stock reducido exitosamente. Producto: {ProductoNombre}, Anterior: {Anterior}, Nuevo: {Nuevo}, Unidades compradas: {Cantidad}, Estado: {Estado}",
                    producto.Nombre, stockAnterior, producto.Stock, cantidadComprada, producto.Estado);

                return new ResultadoStock
                {
                    EsExitoso = true,
                    Mensaje = $"Stock reducido correctamente. {cantidadComprada} unidades compradas. Restante: {producto.Stock}",
                    StockAnterior = stockAnterior,
                    StockNuevo = producto.Stock,
                    UnidadesProcessadas = cantidadComprada,
                    EstadoAnterior = estadoAnterior,
                    EstadoNuevo = producto.Estado
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reducir stock del producto {ProductoId}", producto?.ProductoId);
                return new ResultadoStock
                {
                    EsExitoso = false,
                    Mensaje = "Error al reducir el stock"
                };
            }
        }

        /// <summary>
        /// Revierte el stock tras una compra cancelada
        /// Cada unidad revertida aumenta el stock en 1
        /// </summary>
        public async Task<ResultadoStock> RevertirStockAsync(Producto producto, int cantidadARevertir)
        {
            try
            {
                if (producto == null)
                {
                    return new ResultadoStock
                    {
                        EsExitoso = false,
                        Mensaje = "El producto no existe"
                    };
                }

                int stockAnterior = producto.Stock;
                string estadoAnterior = producto.Estado;

                // REVERTIR STOCK: +1 por cada unidad revertida
                producto.Stock += cantidadARevertir;

                // Actualizar estado
                if (producto.Stock > 0 && producto.Estado == "Agotado")
                {
                    producto.Estado = "Disponible";
                }

                producto.FechaActualizacion = DateTime.UtcNow;

                _logger.LogInformation("Stock revertido exitosamente. Producto: {ProductoNombre}, Anterior: {Anterior}, Nuevo: {Nuevo}, Unidades revertidas: {Cantidad}",
                    producto.Nombre, stockAnterior, producto.Stock, cantidadARevertir);

                return new ResultadoStock
                {
                    EsExitoso = true,
                    Mensaje = $"Stock revertido correctamente. {cantidadARevertir} unidades revertidas. Total: {producto.Stock}",
                    StockAnterior = stockAnterior,
                    StockNuevo = producto.Stock,
                    UnidadesProcessadas = cantidadARevertir,
                    EstadoAnterior = estadoAnterior,
                    EstadoNuevo = producto.Estado
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al revertir stock del producto {ProductoId}", producto?.ProductoId);
                return new ResultadoStock
                {
                    EsExitoso = false,
                    Mensaje = "Error al revertir el stock"
                };
            }
        }

        /// <summary>
        /// Obtiene el mensaje de estado del producto
        /// </summary>
        public string ObtenerMensajeEstado(Producto producto)
        {
            if (producto == null)
                return "Producto no disponible";

            if (producto.Estado == "Agotado" || producto.Stock <= 0)
                return "?? Producto Agotado";

            if (producto.Stock < 10)
                return $"?? Quedan {producto.Stock} unidades";

            if (producto.Stock < producto.StockMinimo)
                return $"?? Stock bajo ({producto.Stock} unidades)";

            return $"? {producto.Stock} unidades disponibles";
        }

        /// <summary>
        /// Obtiene el porcentaje de disponibilidad del producto
        /// </summary>
        public int ObtenerPorcentajeDisponibilidad(Producto producto)
        {
            if (producto == null || producto.Stock <= 0)
                return 0;

            if (producto.Stock > 100)
                return 100;

            return producto.Stock;
        }
    }
}
