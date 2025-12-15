using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexShop.Web.Models;
using NexShop.Web.Services;
using NexShop.Web.ViewModels;

namespace NexShop.Web.Controllers
{
    /// <summary>
    /// Controlador para gestionar carga y administración de archivos multimedia
    /// Soporta carga individual, múltiple y edición de archivos
    /// SOLO VENDEDORES pueden cargar, editar y eliminar archivos
    /// ACCESO ANÓNIMO para consultar multimedia
    /// </summary>
    [Authorize(Roles = "Vendedor,Admin")]
    public class MultimediaController : Controller
    {
        private readonly IMultimediaService _multimediaService;
        private readonly NexShopContext _context;
        private readonly ILogger<MultimediaController> _logger;

        public MultimediaController(IMultimediaService multimediaService, NexShopContext context,
            ILogger<MultimediaController> logger)
        {
            _multimediaService = multimediaService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Carga un archivo multimedia individual desde formulario
        /// POST: /Multimedia/Cargar/{productoId}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Cargar(int productoId, IFormFile archivo)
        {
            try
            {
                // Verificar permisos
                if (!await TienePermisoProductoAsync(productoId))
                {
                    TempData["Error"] = "No tiene permiso para cargar archivos en este producto";
                    return RedirectToAction("Details", "Productos", new { id = productoId });
                }

                if (archivo == null || archivo.Length == 0)
                {
                    TempData["Error"] = "Debe seleccionar un archivo";
                    return RedirectToAction("Details", "Productos", new { id = productoId });
                }

                var resultado = await _multimediaService.CargarArchivoAsync(archivo, productoId);

                if (!resultado.Exito)
                {
                    TempData["Error"] = $"Error al cargar el archivo: {resultado.Mensaje}";
                    return RedirectToAction("Details", "Productos", new { id = productoId });
                }

                _logger.LogInformation(
                    "Archivo cargado exitosamente. ProductoId: {ProductoId}, Archivo: {NombreArchivo}",
                    productoId, resultado.NombreArchivo);

                TempData["Success"] = $"Archivo {archivo.FileName} cargado exitosamente";
                return RedirectToAction("Details", "Productos", new { id = productoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar archivo. ProductoId: {ProductoId}", productoId);
                TempData["Error"] = "Error inesperado al cargar el archivo";
                return RedirectToAction("Details", "Productos", new { id = productoId });
            }
        }

        /// <summary>
        /// Carga múltiples archivos desde formulario
        /// POST: /Multimedia/CargarMultiples/{productoId}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CargarMultiples(int productoId, List<IFormFile> archivos)
        {
            try
            {
                // Verificar permisos
                if (!await TienePermisoProductoAsync(productoId))
                {
                    TempData["Error"] = "No tiene permiso para cargar archivos en este producto";
                    return RedirectToAction("Details", "Productos", new { id = productoId });
                }

                if (!archivos.Any())
                {
                    TempData["Error"] = "Debe seleccionar al menos un archivo";
                    return RedirectToAction("Details", "Productos", new { id = productoId });
                }

                // Limitar cantidad de archivos simultáneos
                if (archivos.Count > 10)
                {
                    TempData["Error"] = "No se pueden cargar más de 10 archivos simultáneamente";
                    return RedirectToAction("Details", "Productos", new { id = productoId });
                }

                var resultado = await _multimediaService.CargarArchivosMultiplesAsync(archivos, productoId);

                if (resultado.ExitoTotal)
                {
                    TempData["Success"] = $"Se cargaron exitosamente {resultado.TotalExitosos} archivos";
                }
                else if (resultado.TotalExitosos > 0)
                {
                    TempData["Warning"] = $"Se cargaron {resultado.TotalExitosos} archivos, {resultado.TotalErrores} tuvieron errores";
                }
                else
                {
                    TempData["Error"] = "No se pudieron cargar los archivos";
                }

                _logger.LogInformation(
                    "Carga múltiple completada. ProductoId: {ProductoId}, Exitosos: {Exitosos}, Errores: {Errores}",
                    productoId, resultado.TotalExitosos, resultado.TotalErrores);

                return RedirectToAction("Details", "Productos", new { id = productoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en carga múltiple. ProductoId: {ProductoId}", productoId);
                TempData["Error"] = "Error inesperado al cargar los archivos";
                return RedirectToAction("Details", "Productos", new { id = productoId });
            }
        }

        /// <summary>
        /// Reemplaza un archivo multimedia existente
        /// POST: /Multimedia/Reemplazar/{multimediaId}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Reemplazar(int multimediaId, IFormFile archivoNuevo)
        {
            try
            {
                var multimedia = await _context.Multimedia.FindAsync(multimediaId);

                if (multimedia == null)
                {
                    return NotFound();
                }

                // Verificar permisos
                if (!await TienePermisoProductoAsync(multimedia.ProductoId))
                {
                    TempData["Error"] = "No tiene permiso para editar este archivo";
                    return RedirectToAction("Details", "Productos", new { id = multimedia.ProductoId });
                }

                if (archivoNuevo == null || archivoNuevo.Length == 0)
                {
                    TempData["Error"] = "Debe proporcionar un nuevo archivo";
                    return RedirectToAction("Details", "Productos", new { id = multimedia.ProductoId });
                }

                var resultado = await _multimediaService.ReemplazarMultimediaAsync(multimediaId, archivoNuevo);

                if (!resultado.Exito)
                {
                    TempData["Error"] = $"Error al reemplazar: {resultado.Mensaje}";
                    return RedirectToAction("Details", "Productos", new { id = multimedia.ProductoId });
                }

                _logger.LogInformation("Archivo reemplazado. MultimediaId: {MultimediaId}", multimediaId);
                TempData["Success"] = "Archivo reemplazado exitosamente";
                return RedirectToAction("Details", "Productos", new { id = multimedia.ProductoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reemplazar archivo. MultimediaId: {MultimediaId}", multimediaId);
                TempData["Error"] = "Error inesperado al reemplazar el archivo";
                return RedirectToAction("Details", "Productos", new { id = 0 });
            }
        }

        /// <summary>
        /// Elimina un archivo multimedia
        /// POST: /Multimedia/Eliminar/{multimediaId}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Eliminar(int multimediaId)
        {
            try
            {
                var multimedia = await _context.Multimedia.FindAsync(multimediaId);

                if (multimedia == null)
                {
                    return NotFound();
                }

                // Verificar permisos
                if (!await TienePermisoProductoAsync(multimedia.ProductoId))
                {
                    TempData["Error"] = "No tiene permiso para eliminar este archivo";
                    return RedirectToAction("Details", "Productos", new { id = multimedia.ProductoId });
                }

                var resultado = await _multimediaService.EliminarMultimediaAsync(multimediaId);

                if (!resultado.Exito)
                {
                    TempData["Error"] = $"Error al eliminar: {resultado.Mensaje}";
                    return RedirectToAction("Details", "Productos", new { id = multimedia.ProductoId });
                }

                _logger.LogInformation("Archivo eliminado. MultimediaId: {MultimediaId}", multimediaId);
                TempData["Success"] = "Archivo eliminado exitosamente";
                return RedirectToAction("Details", "Productos", new { id = multimedia.ProductoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar archivo. MultimediaId: {MultimediaId}", multimediaId);
                TempData["Error"] = "Error inesperado al eliminar el archivo";
                return RedirectToAction("Details", "Productos", new { id = 0 });
            }
        }

        /// <summary>
        /// Establece un archivo como imagen principal
        /// POST: /Multimedia/EstablecerComoPrincipal/{multimediaId}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> EstablecerComoPrincipal(int multimediaId)
        {
            try
            {
                var multimedia = await _context.Multimedia.FindAsync(multimediaId);

                if (multimedia == null)
                {
                    return NotFound();
                }

                // Verificar permisos
                if (!await TienePermisoProductoAsync(multimedia.ProductoId))
                {
                    TempData["Error"] = "No tiene permiso para editar este producto";
                    return RedirectToAction("Details", "Productos", new { id = multimedia.ProductoId });
                }

                var resultado = await _multimediaService.EstablecerComoPrincipalAsync(multimediaId, multimedia.ProductoId);

                if (!resultado.Exito)
                {
                    TempData["Error"] = $"Error: {resultado.Mensaje}";
                    return RedirectToAction("Details", "Productos", new { id = multimedia.ProductoId });
                }

                _logger.LogInformation("Archivo establecido como principal. MultimediaId: {MultimediaId}", multimediaId);
                TempData["Success"] = "Imagen principal actualizada";
                return RedirectToAction("Details", "Productos", new { id = multimedia.ProductoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al establecer como principal. MultimediaId: {MultimediaId}", multimediaId);
                TempData["Error"] = "Error inesperado";
                return RedirectToAction("Details", "Productos", new { id = 0 });
            }
        }

        /// <summary>
        /// Obtiene todos los archivos multimedia de un producto (AJAX) - ACCESO ANÓNIMO
        /// GET: /Multimedia/ObtenerMultimedia/{productoId}
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ObtenerMultimedia(int productoId)
        {
            try
            {
                var multimedia = await _multimediaService.ObtenerMultimediaProductoAsync(productoId);

                var viewModels = multimedia.Select(m => new MultimediaListViewModel
                {
                    MultimediaId = m.MultimediaId,
                    Tipo = m.Tipo,
                    Nombre = m.Nombre,
                    Url = m.Url,
                    Descripcion = m.Descripcion,
                    EsPrincipal = m.EsPrincipal,
                    Orden = m.Orden,
                    TamanoBytes = m.TamanoBytes,
                    TipoMime = m.TipoMime
                }).ToList();

                return Json(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener multimedia. ProductoId: {ProductoId}", productoId);
                return StatusCode(500, new { error = "Error al obtener los archivos" });
            }
        }

        /// <summary>
        /// Valida un archivo sin cargarlo (AJAX) - SOLO VENDEDORES
        /// POST: /Multimedia/Validar
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Vendedor,Admin")]
        public IActionResult Validar(IFormFile archivo)
        {
            try
            {
                if (archivo == null)
                {
                    return BadRequest(new { error = "Debe proporcionar un archivo" });
                }

                var resultado = _multimediaService.ValidarArchivo(archivo);
                return Json(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar archivo");
                return StatusCode(500, new { error = "Error al validar el archivo" });
            }
        }

        /// <summary>
        /// Verifica si el usuario tiene permiso para editar un producto
        /// </summary>
        private async Task<bool> TienePermisoProductoAsync(int productoId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null)
            {
                return false;
            }

            return producto.VendedorId == userId;
        }
    }
}
