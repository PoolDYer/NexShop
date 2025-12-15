using Microsoft.AspNetCore.Mvc;
using NexShop.Web.Services;

namespace NexShop.Web.Controllers
{
    /// <summary>
    /// Controller para gestionar imágenes de productos
    /// Proporciona endpoints para obtener rutas de imágenes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ImagenesController : ControllerBase
    {
        private readonly ISincronizacionImagenesService _sincronizacionImagenesService;
        private readonly ISincronizacionMultimediaService _sincronizacionMultimediaService;
        private readonly ILogger<ImagenesController> _logger;

        public ImagenesController(
            ISincronizacionImagenesService sincronizacionImagenesService,
            ISincronizacionMultimediaService sincronizacionMultimediaService,
            ILogger<ImagenesController> logger)
        {
            _sincronizacionImagenesService = sincronizacionImagenesService;
            _sincronizacionMultimediaService = sincronizacionMultimediaService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las imágenes de un producto
        /// GET: /api/imagenes/producto/5
        /// </summary>
        [HttpGet("producto/{productoId}")]
        public async Task<IActionResult> ObtenerImagenesProducto(int productoId)
        {
            try
            {
                var imagenes = await _sincronizacionImagenesService.ObtenerImagenesProducto(productoId);

                if (!imagenes.Any())
                {
                    _logger.LogWarning("No se encontraron imágenes para el producto: {ProductoId}", productoId);
                    return NotFound(new { mensaje = "No hay imágenes disponibles para este producto" });
                }

                // Construir URLs relativas
                var imagenesConRuta = imagenes.Select(img => new
                {
                    nombre = img,
                    url = _sincronizacionImagenesService.ObtenerRutaImagenProducto(productoId, img)
                }).ToList();

                return Ok(imagenesConRuta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo imágenes del producto: {ProductoId}", productoId);
                return BadRequest(new { error = "Error al obtener imágenes" });
            }
        }

        /// <summary>
        /// Obtiene una imagen específica de un producto
        /// GET: /api/imagenes/producto/5/imagen/foto.jpg
        /// </summary>
        [HttpGet("producto/{productoId}/imagen/{nombreArchivo}")]
        public IActionResult ObtenerImagenProducto(int productoId, string nombreArchivo)
        {
            try
            {
                var ruta = _sincronizacionImagenesService.ObtenerRutaImagenProducto(productoId, nombreArchivo);

                if (string.IsNullOrEmpty(ruta))
                {
                    return NotFound(new { error = "Producto no encontrado" });
                }

                return Ok(new { url = ruta });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo imagen del producto: {ProductoId}, Archivo: {Archivo}", productoId, nombreArchivo);
                return BadRequest(new { error = "Error al obtener imagen" });
            }
        }

        /// <summary>
        /// Sincroniza todas las imágenes desde las carpetas de productos
        /// POST: /api/imagenes/sincronizar
        /// </summary>
        [HttpPost("sincronizar")]
        public async Task<IActionResult> SincronizarImagenes()
        {
            try
            {
                _logger.LogInformation("Iniciando sincronización de imágenes...");

                var resultado = await _sincronizacionImagenesService.SincronizarTodasLasImagenes();

                if (resultado.Exitoso)
                {
                    _logger.LogInformation("Sincronización completada: {Resultado}", resultado);
                    return Ok(new
                    {
                        mensaje = "Sincronización completada exitosamente",
                        carpetasProcesadas = resultado.CarpetasProcesadas,
                        imagenesCopiadas = resultado.ImagenesthGraciasCopiadasExitosamente,
                        errores = resultado.Errores
                    });
                }
                else
                {
                    _logger.LogWarning("Sincronización con errores: {Resultado}", resultado);
                    return BadRequest(new
                    {
                        mensaje = "Sincronización completada con errores",
                        carpetasProcesadas = resultado.CarpetasProcesadas,
                        imagenesCopiadas = resultado.ImagenesthGraciasCopiadasExitosamente,
                        errores = resultado.Errores
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la sincronización de imágenes");
                return StatusCode(500, new { error = "Error interno del servidor", mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Sincroniza multimedia en la BD desde imágenes del sistema de archivos
        /// POST: /api/imagenes/sincronizar-multimedia
        /// </summary>
        [HttpPost("sincronizar-multimedia")]
        public async Task<IActionResult> SincronizarMultimedia()
        {
            try
            {
                _logger.LogInformation("Iniciando sincronización de multimedia en BD...");

                var resultado = await _sincronizacionMultimediaService.SincronizarTodasLasImagenes();

                if (resultado.Exitoso)
                {
                    _logger.LogInformation("Sincronización de multimedia completada: {Resultado}", resultado);
                    return Ok(new
                    {
                        mensaje = "Multimedia sincronizada exitosamente",
                        productosActualizados = resultado.ProductosActualizados,
                        multimediaAgregada = resultado.MultimediaAgregada,
                        errores = resultado.Errores
                    });
                }
                else
                {
                    _logger.LogWarning("Sincronización de multimedia con errores: {Resultado}", resultado);
                    return BadRequest(new
                    {
                        mensaje = "Sincronización completada con errores",
                        productosActualizados = resultado.ProductosActualizados,
                        multimediaAgregada = resultado.MultimediaAgregada,
                        errores = resultado.Errores
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la sincronización de multimedia");
                return StatusCode(500, new { error = "Error interno del servidor", mensaje = ex.Message });
            }
        }
    }
}
