using Microsoft.EntityFrameworkCore;
using NexShop.Web.Models;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Servicio para manejar operaciones comunes de productos
    /// Implementa lógica reutilizable para controllers
    /// </summary>
    public interface IProductoService
    {
        Task<ResultadoOperacion<Producto>> ObtenerProductoAsync(int id);
        Task<ResultadoOperacion<List<Producto>>> ObtenerProductosPorCategoriaAsync(int categoriaId);
        Task<ResultadoOperacion> CrearProductoAsync(Producto producto);
        Task<ResultadoOperacion> ActualizarProductoAsync(Producto producto);
        Task<ResultadoOperacion> EliminarProductoAsync(int id);
        Task<ResultadoOperacion> DecrementarStockAsync(int productoId, int cantidad);
        Task<ResultadoOperacion> IncrementarStockAsync(int productoId, int cantidad);
    }

    public class ProductoService : IProductoService
    {
        private readonly NexShopContext _context;
        private readonly ILogger<ProductoService> _logger;

        public ProductoService(NexShopContext context, ILogger<ProductoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResultadoOperacion<Producto>> ObtenerProductoAsync(int id)
        {
            try
            {
                var producto = await _context.Productos
                    .Include(p => p.Categoria)
                    .Include(p => p.Multimedia)
                    .FirstOrDefaultAsync(p => p.ProductoId == id);

                if (producto == null)
                {
                    return ResultadoOperacion<Producto>.Error("Producto no encontrado", "PRODUCTO_NO_ENCONTRADO");
                }

                return ResultadoOperacion<Producto>.Success("Producto obtenido correctamente", producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto con ID: {ProductoId}", id);
                return ResultadoOperacion<Producto>.Error("Error al obtener el producto", "ERROR_BD");
            }
        }

        public async Task<ResultadoOperacion<List<Producto>>> ObtenerProductosPorCategoriaAsync(int categoriaId)
        {
            try
            {
                var productos = await _context.Productos
                    .Where(p => p.CategoriaId == categoriaId && p.Categoria!.EstaActiva)
                    .Include(p => p.Multimedia)
                    .ToListAsync();

                return ResultadoOperacion<List<Producto>>.Success("Productos obtenidos correctamente", productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos de categoría: {CategoriaId}", categoriaId);
                return ResultadoOperacion<List<Producto>>.Error("Error al obtener productos", "ERROR_BD");
            }
        }

        public async Task<ResultadoOperacion> CrearProductoAsync(Producto producto)
        {
            try
            {
                if (producto == null)
                {
                    return ResultadoOperacion.Error("El producto no puede ser nulo", "PRODUCTO_NULO");
                }

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Producto creado: {ProductoId}", producto.ProductoId);
                return ResultadoOperacion.Success("Producto creado exitosamente");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al crear producto");
                return ResultadoOperacion.Error("Error al guardar el producto", "ERROR_BD");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear producto");
                return ResultadoOperacion.Error("Error inesperado al crear el producto", "ERROR_INESPERADO");
            }
        }

        public async Task<ResultadoOperacion> ActualizarProductoAsync(Producto producto)
        {
            try
            {
                if (producto == null)
                {
                    return ResultadoOperacion.Error("El producto no puede ser nulo", "PRODUCTO_NULO");
                }

                var productoExistente = await _context.Productos.FindAsync(producto.ProductoId);
                if (productoExistente == null)
                {
                    return ResultadoOperacion.Error("Producto no encontrado", "PRODUCTO_NO_ENCONTRADO");
                }

                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Producto actualizado: {ProductoId}", producto.ProductoId);
                return ResultadoOperacion.Success("Producto actualizado exitosamente");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al actualizar producto");
                return ResultadoOperacion.Error("Error al actualizar el producto", "ERROR_BD");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar producto");
                return ResultadoOperacion.Error("Error inesperado al actualizar el producto", "ERROR_INESPERADO");
            }
        }

        public async Task<ResultadoOperacion> EliminarProductoAsync(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                {
                    return ResultadoOperacion.Error("Producto no encontrado", "PRODUCTO_NO_ENCONTRADO");
                }

                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Producto eliminado: {ProductoId}", id);
                return ResultadoOperacion.Success("Producto eliminado exitosamente");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al eliminar producto");
                return ResultadoOperacion.Error("No se puede eliminar el producto porque está asociado a órdenes", "RESTRICCION_FK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar producto");
                return ResultadoOperacion.Error("Error inesperado al eliminar el producto", "ERROR_INESPERADO");
            }
        }

        public async Task<ResultadoOperacion> DecrementarStockAsync(int productoId, int cantidad)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(productoId);
                if (producto == null)
                {
                    return ResultadoOperacion.Error("Producto no encontrado", "PRODUCTO_NO_ENCONTRADO");
                }

                if (producto.Stock < cantidad)
                {
                    return ResultadoOperacion.Error($"Stock insuficiente. Disponibles: {producto.Stock}", "STOCK_INSUFICIENTE");
                }

                producto.Stock -= cantidad;

                if (producto.Stock <= producto.StockMinimo)
                {
                    producto.Estado = "Agotado";
                }

                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();

                return ResultadoOperacion.Success("Stock decrementado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al decrementar stock del producto: {ProductoId}", productoId);
                return ResultadoOperacion.Error("Error al actualizar el stock", "ERROR_BD");
            }
        }

        public async Task<ResultadoOperacion> IncrementarStockAsync(int productoId, int cantidad)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(productoId);
                if (producto == null)
                {
                    return ResultadoOperacion.Error("Producto no encontrado", "PRODUCTO_NO_ENCONTRADO");
                }

                producto.Stock += cantidad;

                if (producto.Stock > producto.StockMinimo && producto.Estado == "Agotado")
                {
                    producto.Estado = "Disponible";
                }

                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();

                return ResultadoOperacion.Success("Stock incrementado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al incrementar stock del producto: {ProductoId}", productoId);
                return ResultadoOperacion.Error("Error al actualizar el stock", "ERROR_BD");
            }
        }
    }
}
