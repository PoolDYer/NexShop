using NexShop.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Servicio para sincronizar imágenes en la BD
    /// Crea registros de Multimedia basados en imágenes del sistema de archivos
    /// </summary>
    public interface ISincronizacionMultimediaService
    {
        /// <summary>
        /// Sincroniza todas las imágenes de carpetas a registros en BD
        /// </summary>
        Task<SincronizacionMultimediaResultado> SincronizarTodasLasImagenes();

        /// <summary>
        /// Sincroniza imágenes de un producto específico
        /// </summary>
        Task<int> SincronizarImagenesProducto(int productoId);
    }

    public class SincronizacionMultimediaService : ISincronizacionMultimediaService
    {
        private readonly NexShopContext _context;
        private readonly ISincronizacionImagenesService _sincronizacionImagenesService;
        private readonly ILogger<SincronizacionMultimediaService> _logger;

        public SincronizacionMultimediaService(
            NexShopContext context,
            ISincronizacionImagenesService sincronizacionImagenesService,
            ILogger<SincronizacionMultimediaService> logger)
        {
            _context = context;
            _sincronizacionImagenesService = sincronizacionImagenesService;
            _logger = logger;
        }

        public async Task<SincronizacionMultimediaResultado> SincronizarTodasLasImagenes()
        {
            var resultado = new SincronizacionMultimediaResultado();

            try
            {
                // Obtener todos los productos
                var productos = await _context.Productos.ToListAsync();
                _logger.LogInformation("Sincronizando multimedia para {Cantidad} productos", productos.Count);

                foreach (var producto in productos)
                {
                    try
                    {
                        var imagenesAgregadas = await SincronizarImagenesProducto(producto.ProductoId);
                        if (imagenesAgregadas > 0)
                        {
                            resultado.ProductosActualizados++;
                            resultado.MultimediaAgregada += imagenesAgregadas;
                        }
                    }
                    catch (Exception ex)
                    {
                        resultado.Errores++;
                        _logger.LogError(ex, "Error sincronizando producto {ProductoId}", producto.ProductoId);
                    }
                }

                resultado.Exitoso = resultado.Errores == 0;
                _logger.LogInformation("Sincronización de multimedia completada: {Resultado}", resultado);

                return resultado;
            }
            catch (Exception ex)
            {
                resultado.Exitoso = false;
                resultado.Errores++;
                _logger.LogError(ex, "Error en sincronización de multimedia");
                throw;
            }
        }

        public async Task<int> SincronizarImagenesProducto(int productoId)
        {
            var imagenesAgregadas = 0;

            try
            {
                // Obtener producto
                var producto = await _context.Productos
                    .Include(p => p.Multimedia)
                    .FirstOrDefaultAsync(p => p.ProductoId == productoId);

                if (producto == null)
                {
                    _logger.LogWarning("Producto no encontrado: {ProductoId}", productoId);
                    return 0;
                }

                // Obtener imágenes del sistema de archivos
                var imagenes = await _sincronizacionImagenesService.ObtenerImagenesProducto(productoId);

                if (!imagenes.Any())
                {
                    _logger.LogDebug("No hay imágenes para el producto {ProductoId}", productoId);
                    return 0;
                }

                // Agregar cada imagen como Multimedia si no existe
                foreach (var (nombreArchivo, indice) in imagenes.Select((img, idx) => (img, idx)))
                {
                    // Verificar si ya existe
                    var existeEnBD = producto.Multimedia.Any(m => m.Url.EndsWith(nombreArchivo));

                    if (!existeEnBD)
                    {
                        var ruta = _sincronizacionImagenesService.ObtenerRutaImagenProducto(productoId, nombreArchivo);

                        var multimedia = new Multimedia
                        {
                            ProductoId = productoId,
                            Nombre = Path.GetFileNameWithoutExtension(nombreArchivo),
                            Url = ruta,
                            Tipo = "Foto",
                            Descripcion = $"Imagen {indice + 1} de {producto.Nombre}",
                            EsPrincipal = indice == 0, // La primera imagen es la principal
                            Orden = indice,
                            EstaActivo = true,
                            FechaCreacion = DateTime.UtcNow,
                            TamanoBytes = 0 // Se podría actualizar después
                        };

                        _context.Multimedia.Add(multimedia);
                        imagenesAgregadas++;
                        _logger.LogInformation("Multimedia agregada: {ProductoId} - {Archivo}", productoId, nombreArchivo);
                    }
                }

                // Guardar cambios
                if (imagenesAgregadas > 0)
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Se agregaron {Cantidad} imágenes para producto {ProductoId}", imagenesAgregadas, productoId);
                }

                return imagenesAgregadas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sincronizando imágenes del producto {ProductoId}", productoId);
                throw;
            }
        }
    }

    /// <summary>
    /// Resultado de la sincronización de multimedia
    /// </summary>
    public class SincronizacionMultimediaResultado
    {
        public bool Exitoso { get; set; }
        public int ProductosActualizados { get; set; }
        public int MultimediaAgregada { get; set; }
        public int Errores { get; set; }

        public override string ToString()
        {
            return $"Productos: {ProductosActualizados}, Multimedia: {MultimediaAgregada}, Errores: {Errores}";
        }
    }
}
