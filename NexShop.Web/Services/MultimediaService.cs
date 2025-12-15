using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NexShop.Web.Models;
using System.Security.Cryptography;
using System.Text;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Servicio de gestión de archivos multimedia con almacenamiento en wwwroot
    /// Soporta validación, carga asincrónica y gestión en base de datos
    /// </summary>
    public class MultimediaService : IMultimediaService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IOptions<MultimediaOptions> _options;
        private readonly NexShopContext _context;
        private readonly ILogger<MultimediaService> _logger;
        private readonly MultimediaOptions _config;

        public MultimediaService(IWebHostEnvironment environment, IOptions<MultimediaOptions> options,
            NexShopContext context, ILogger<MultimediaService> logger)
        {
            _environment = environment;
            _options = options;
            _context = context;
            _logger = logger;
            _config = options.Value;

            // Inicializar rutas absolutas
            if (string.IsNullOrEmpty(_config.RutaAbsoluta))
            {
                _config.RutaAbsoluta = Path.Combine(_environment.WebRootPath, _config.RutaAlmacenamiento);
            }
        }

        /// <summary>
        /// Carga un archivo multimedia individual
        /// </summary>
        public async Task<ResultadoCargaArchivo> CargarArchivoAsync(IFormFile archivo, int productoId, bool esPrincipal = false)
        {
            try
            {
                // Validar archivo
                var validacion = ValidarArchivo(archivo);
                if (!validacion.Exito)
                {
                    return ResultadoCargaArchivo.Error(validacion.Mensaje, validacion.CodigoError);
                }

                // Verificar que el producto existe
                var producto = await _context.Productos.FindAsync(productoId);
                if (producto == null)
                {
                    return ResultadoCargaArchivo.Error("El producto no existe", "PRODUCTO_NO_ENCONTRADO");
                }

                // Crear directorio si no existe
                if (!Directory.Exists(_config.RutaAbsoluta))
                {
                    Directory.CreateDirectory(_config.RutaAbsoluta);
                }

                // Generar nombre único para el archivo
                var nombreArchivo = GenerarNombreArchivoUnico(archivo);
                var rutaCompleta = Path.Combine(_config.RutaAbsoluta, nombreArchivo);

                // Guardar archivo físicamente
                using (var fileStream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await archivo.CopyToAsync(fileStream);
                }

                // Obtener información del archivo guardado
                var infoArchivo = new FileInfo(rutaCompleta);
                var tipoMultimedia = ObtenerTipoMultimedia(archivo.ContentType ?? string.Empty);
                var urlAcceso = $"{_config.PrefijoDatosUrl}/{nombreArchivo}";

                // Si es la primera imagen o se indica como principal, marcar como principal
                var tieneImagenPrincipal = await _context.Multimedia
                    .AnyAsync(m => m.ProductoId == productoId && m.EsPrincipal);

                if (!tieneImagenPrincipal && tipoMultimedia == "Foto")
                {
                    esPrincipal = true;
                }

                // Crear registro en base de datos
                var multimedia = new Multimedia
                {
                    ProductoId = productoId,
                    Nombre = Path.GetFileNameWithoutExtension(archivo.FileName),
                    NombreArchivo = nombreArchivo,
                    Url = urlAcceso,
                    TipoMime = archivo.ContentType,
                    Tipo = tipoMultimedia,
                    TamanoBytes = infoArchivo.Length,
                    EsPrincipal = esPrincipal,
                    EstaActivo = true,
                    FechaCreacion = DateTime.UtcNow,
                    Orden = await _context.Multimedia
                        .Where(m => m.ProductoId == productoId)
                        .MaxAsync(m => (int?)m.Orden) + 1 ?? 0
                };

                _context.Multimedia.Add(multimedia);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Archivo multimedia cargado exitosamente. MultimediaId: {MultimediaId}, ProductoId: {ProductoId}, Archivo: {NombreArchivo}",
                    multimedia.MultimediaId, productoId, nombreArchivo);

                return ResultadoCargaArchivo.Success(nombreArchivo, urlAcceso, infoArchivo.Length, archivo.ContentType ?? string.Empty);
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "Error de I/O al cargar archivo para ProductoId: {ProductoId}", productoId);
                return ResultadoCargaArchivo.Error("Error al guardar el archivo en el servidor", "ERROR_IO");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al cargar multimedia para ProductoId: {ProductoId}", productoId);
                return ResultadoCargaArchivo.Error("Error al registrar el archivo en la base de datos", "ERROR_BD");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al cargar archivo para ProductoId: {ProductoId}", productoId);
                return ResultadoCargaArchivo.Error("Error inesperado al cargar el archivo", "ERROR_INESPERADO");
            }
        }

        /// <summary>
        /// Carga múltiples archivos de manera concurrente
        /// </summary>
        public async Task<ResultadoCargaMultiple> CargarArchivosMultiplesAsync(List<IFormFile> archivos, int productoId)
        {
            if (!archivos.Any())
            {
                return ResultadoCargaMultiple.Crear(new List<ResultadoCargaArchivo>
                {
                    ResultadoCargaArchivo.Error("No se proporcionaron archivos", "SIN_ARCHIVOS")
                });
            }

            try
            {
                var resultados = new List<ResultadoCargaArchivo>();

                // Procesar archivos de forma secuencial para mantener orden
                // (evita problemas de concurrencia en asignación de orden)
                foreach (var archivo in archivos)
                {
                    var resultado = await CargarArchivoAsync(archivo, productoId, esPrincipal: false);
                    resultados.Add(resultado);
                }

                return ResultadoCargaMultiple.Crear(resultados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar múltiples archivos para ProductoId: {ProductoId}", productoId);
                return ResultadoCargaMultiple.Crear(new List<ResultadoCargaArchivo>
                {
                    ResultadoCargaArchivo.Error("Error al procesar múltiples archivos", "ERROR_MULTIPLES")
                });
            }
        }

        /// <summary>
        /// Valida un archivo
        /// </summary>
        public ResultadoOperacion ValidarArchivo(IFormFile archivo)
        {
            // Validación nula
            if (archivo == null || archivo.Length == 0)
            {
                return ResultadoOperacion.Error("El archivo está vacío", "ARCHIVO_VACIO");
            }

            // Validación de tamaño
            if (archivo.Length > _config.TamanoMaximoBytes)
            {
                return ResultadoOperacion.Error(
                    $"El archivo excede el tamaño máximo de {_config.TamanoMaximoMB} MB",
                    "ARCHIVO_DEMASIADO_GRANDE");
            }

            // Validación de extensión
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension))
            {
                return ResultadoOperacion.Error("El archivo no tiene extensión", "SIN_EXTENSION");
            }

            var tipoMime = archivo.ContentType?.ToLowerInvariant() ?? string.Empty;
            var esImagen = _config.TiposMimeImagenesPermitidos.Contains(tipoMime);
            var esVideo = _config.TiposMimeVideosPermitidos.Contains(tipoMime);

            if (!esImagen && !esVideo)
            {
                return ResultadoOperacion.Error(
                    $"Tipo de archivo no permitido: {tipoMime}. Permitidos: imágenes (JPEG, PNG, GIF, WebP) y videos (MP4, WebM)",
                    "TIPO_ARCHIVO_NO_PERMITIDO");
            }

            // Validación de extensión según tipo MIME
            var extensionesPermitidas = esImagen 
                ? _config.ExtensionesImagenesPermitidas 
                : _config.ExtensionesVideosPermitidas;

            if (!extensionesPermitidas.Contains(extension))
            {
                return ResultadoOperacion.Error(
                    $"Extensión de archivo no permitida: {extension}",
                    "EXTENSION_NO_PERMITIDA");
            }

            // Validación de contenido (verificar firma del archivo)
            if (!ValidarFirmaArchivo(archivo))
            {
                return ResultadoOperacion.Error(
                    "La firma del archivo no coincide con su tipo MIME",
                    "FIRMA_INVALIDA");
            }

            return ResultadoOperacion.Success("Archivo válido");
        }

        /// <summary>
        /// Obtiene el tipo de multimedia
        /// </summary>
        public string ObtenerTipoMultimedia(string tipoMime)
        {
            tipoMime = tipoMime?.ToLowerInvariant() ?? string.Empty;

            if (_config.TiposMimeImagenesPermitidos.Contains(tipoMime))
                return "Foto";

            if (_config.TiposMimeVideosPermitidos.Contains(tipoMime))
                return "Video";

            return "Documento";
        }

        /// <summary>
        /// Elimina un archivo multimedia
        /// </summary>
        public async Task<ResultadoOperacion> EliminarMultimediaAsync(int multimediaId)
        {
            try
            {
                var multimedia = await _context.Multimedia.FindAsync(multimediaId);

                if (multimedia == null)
                {
                    return ResultadoOperacion.Error("Archivo multimedia no encontrado", "MULTIMEDIA_NO_ENCONTRADO");
                }

                // Eliminar archivo físico
                if (!string.IsNullOrEmpty(multimedia.NombreArchivo))
                {
                    var rutaArchivo = Path.Combine(_config.RutaAbsoluta!, multimedia.NombreArchivo);
                    if (File.Exists(rutaArchivo))
                    {
                        File.Delete(rutaArchivo);
                    }
                }

                // Si era la imagen principal, marcar la siguiente como principal
                if (multimedia.EsPrincipal && multimedia.Tipo == "Foto")
                {
                    var siguienteImagenPrincipal = await _context.Multimedia
                        .Where(m => m.ProductoId == multimedia.ProductoId &&
                                   m.Tipo == "Foto" &&
                                   m.MultimediaId != multimediaId)
                        .OrderBy(m => m.Orden)
                        .FirstOrDefaultAsync();

                    if (siguienteImagenPrincipal != null)
                    {
                        siguienteImagenPrincipal.EsPrincipal = true;
                        _context.Multimedia.Update(siguienteImagenPrincipal);
                    }
                }

                _context.Multimedia.Remove(multimedia);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Multimedia eliminado exitosamente. MultimediaId: {MultimediaId}", multimediaId);
                return ResultadoOperacion.Success("Archivo eliminado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar multimedia. MultimediaId: {MultimediaId}", multimediaId);
                return ResultadoOperacion.Error("Error al eliminar el archivo", "ERROR_ELIMINAR");
            }
        }

        /// <summary>
        /// Elimina todos los archivos de un producto
        /// </summary>
        public async Task<ResultadoOperacion> EliminarMultimediaProductoAsync(int productoId)
        {
            try
            {
                var multimedia = await _context.Multimedia
                    .Where(m => m.ProductoId == productoId)
                    .ToListAsync();

                foreach (var archivo in multimedia)
                {
                    if (!string.IsNullOrEmpty(archivo.NombreArchivo))
                    {
                        var rutaArchivo = Path.Combine(_config.RutaAbsoluta!, archivo.NombreArchivo);
                        if (File.Exists(rutaArchivo))
                        {
                            File.Delete(rutaArchivo);
                        }
                    }
                }

                _context.Multimedia.RemoveRange(multimedia);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Multimedia del producto eliminado. ProductoId: {ProductoId}", productoId);
                return ResultadoOperacion.Success("Archivos del producto eliminados");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar multimedia del producto. ProductoId: {ProductoId}", productoId);
                return ResultadoOperacion.Error("Error al eliminar archivos", "ERROR_ELIMINAR");
            }
        }

        /// <summary>
        /// Reemplaza un archivo multimedia existente
        /// </summary>
        public async Task<ResultadoCargaArchivo> ReemplazarMultimediaAsync(int multimediaId, IFormFile archivoNuevo)
        {
            try
            {
                var multimedia = await _context.Multimedia.FindAsync(multimediaId);

                if (multimedia == null)
                {
                    return ResultadoCargaArchivo.Error("Archivo multimedia no encontrado", "MULTIMEDIA_NO_ENCONTRADO");
                }

                // Validar archivo nuevo
                var validacion = ValidarArchivo(archivoNuevo);
                if (!validacion.Exito)
                {
                    return ResultadoCargaArchivo.Error(validacion.Mensaje, validacion.CodigoError);
                }

                // Eliminar archivo anterior
                if (!string.IsNullOrEmpty(multimedia.NombreArchivo))
                {
                    var rutaAnterior = Path.Combine(_config.RutaAbsoluta!, multimedia.NombreArchivo);
                    if (File.Exists(rutaAnterior))
                    {
                        File.Delete(rutaAnterior);
                    }
                }

                // Generar nuevo nombre
                var nombreArchivoNuevo = GenerarNombreArchivoUnico(archivoNuevo);
                var rutaCompleta = Path.Combine(_config.RutaAbsoluta!, nombreArchivoNuevo);

                // Guardar nuevo archivo
                using (var fileStream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await archivoNuevo.CopyToAsync(fileStream);
                }

                var infoArchivo = new FileInfo(rutaCompleta);
                var urlAcceso = $"{_config.PrefijoDatosUrl}/{nombreArchivoNuevo}";

                // Actualizar registro
                multimedia.NombreArchivo = nombreArchivoNuevo;
                multimedia.Url = urlAcceso;
                multimedia.TipoMime = archivoNuevo.ContentType;
                multimedia.Tipo = ObtenerTipoMultimedia(archivoNuevo.ContentType ?? string.Empty);
                multimedia.TamanoBytes = infoArchivo.Length;
                multimedia.FechaActualizacion = DateTime.UtcNow;

                _context.Multimedia.Update(multimedia);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Multimedia reemplazado exitosamente. MultimediaId: {MultimediaId}", multimediaId);
                return ResultadoCargaArchivo.Success(nombreArchivoNuevo, urlAcceso, infoArchivo.Length, archivoNuevo.ContentType ?? string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reemplazar multimedia. MultimediaId: {MultimediaId}", multimediaId);
                return ResultadoCargaArchivo.Error("Error al reemplazar el archivo", "ERROR_REEMPLAZAR");
            }
        }

        /// <summary>
        /// Obtiene todos los archivos de un producto
        /// </summary>
        public async Task<List<Multimedia>> ObtenerMultimediaProductoAsync(int productoId)
        {
            try
            {
                return await _context.Multimedia
                    .Where(m => m.ProductoId == productoId && m.EstaActivo)
                    .OrderBy(m => m.Orden)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener multimedia del producto. ProductoId: {ProductoId}", productoId);
                return new List<Multimedia>();
            }
        }

        /// <summary>
        /// Establece un archivo como imagen principal
        /// </summary>
        public async Task<ResultadoOperacion> EstablecerComoPrincipalAsync(int multimediaId, int productoId)
        {
            try
            {
                var multimedia = await _context.Multimedia.FindAsync(multimediaId);

                if (multimedia == null)
                {
                    return ResultadoOperacion.Error("Archivo multimedia no encontrado", "MULTIMEDIA_NO_ENCONTRADO");
                }

                if (multimedia.ProductoId != productoId)
                {
                    return ResultadoOperacion.Error("El archivo no pertenece a este producto", "PRODUCTO_NO_COINCIDE");
                }

                // Desmarcar la principal anterior
                var anteriorPrincipal = await _context.Multimedia
                    .Where(m => m.ProductoId == productoId && m.EsPrincipal)
                    .FirstOrDefaultAsync();

                if (anteriorPrincipal != null)
                {
                    anteriorPrincipal.EsPrincipal = false;
                    _context.Multimedia.Update(anteriorPrincipal);
                }

                // Marcar como principal
                multimedia.EsPrincipal = true;
                _context.Multimedia.Update(multimedia);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Archivo establecido como principal. MultimediaId: {MultimediaId}", multimediaId);
                return ResultadoOperacion.Success("Archivo establecido como principal");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al establecer multimedia como principal. MultimediaId: {MultimediaId}", multimediaId);
                return ResultadoOperacion.Error("Error al establecer como principal", "ERROR_PRINCIPAL");
            }
        }

        /// <summary>
        /// Actualiza el orden de visualización
        /// </summary>
        public async Task<ResultadoOperacion> ActualizarOrdenMultimediaAsync(Dictionary<int, int> actualizaciones)
        {
            try
            {
                var multimediaIds = actualizaciones.Keys.ToList();
                var multimedia = await _context.Multimedia
                    .Where(m => multimediaIds.Contains(m.MultimediaId))
                    .ToListAsync();

                foreach (var archivo in multimedia)
                {
                    if (actualizaciones.TryGetValue(archivo.MultimediaId, out var nuevoOrden))
                    {
                        archivo.Orden = nuevoOrden;
                        _context.Multimedia.Update(archivo);
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Orden de multimedia actualizado. Total: {Total}", actualizaciones.Count);
                return ResultadoOperacion.Success("Orden actualizado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar orden de multimedia");
                return ResultadoOperacion.Error("Error al actualizar el orden", "ERROR_ORDEN");
            }
        }

        /// <summary>
        /// Genera un nombre de archivo único basado en hash y timestamp
        /// </summary>
        private string GenerarNombreArchivoUnico(IFormFile archivo)
        {
            var timestamp = DateTime.UtcNow.Ticks.ToString("X8");
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            var nombreSinExtension = Path.GetFileNameWithoutExtension(archivo.FileName);

            // Generar hash para evitar duplicados
            using (var md5 = MD5.Create())
            {
                var nombreOriginal = $"{nombreSinExtension}{timestamp}";
                var bytes = Encoding.UTF8.GetBytes(nombreOriginal);
                var hash = md5.ComputeHash(bytes);
                var hashString = BitConverter.ToString(hash).Replace("-", "").Substring(0, 8);

                return $"{hashString}_{timestamp}{extension}";
            }
        }

        /// <summary>
        /// Valida la firma del archivo para evitar spoofing de tipos MIME
        /// </summary>
        private bool ValidarFirmaArchivo(IFormFile archivo)
        {
            try
            {
                // Firmas hexadecimales de archivos comunes
                var firmas = new Dictionary<string, byte[]>
                {
                    // JPEG
                    { "image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF } },
                    // PNG
                    { "image/png", new byte[] { 0x89, 0x50, 0x4E, 0x47 } },
                    // GIF
                    { "image/gif", new byte[] { 0x47, 0x49, 0x46 } },
                    // WebP
                    { "image/webp", new byte[] { 0x52, 0x49, 0x46, 0x46 } },
                    // MP4
                    { "video/mp4", new byte[] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70 } },
                };

                var tipoMime = archivo.ContentType?.ToLowerInvariant() ?? string.Empty;

                if (!firmas.TryGetValue(tipoMime, out var firmaEsperada))
                {
                    // No hay firma registrada, permitir
                    return true;
                }

                // Leer los primeros bytes del archivo
                using (var stream = archivo.OpenReadStream())
                {
                    var buffer = new byte[firmaEsperada.Length];
                    var bytesLeidos = stream.Read(buffer, 0, buffer.Length);

                    if (bytesLeidos < firmaEsperada.Length)
                    {
                        return false;
                    }

                    return buffer.Take(firmaEsperada.Length).SequenceEqual(firmaEsperada);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al validar firma de archivo");
                return true; // Permitir si no se puede validar
            }
        }
    }
}
