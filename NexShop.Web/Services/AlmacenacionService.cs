using NexShop.Web.Models;
using Microsoft.Extensions.Options;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Interfaz para abstracción de almacenamiento (local o Azure)
    /// </summary>
    public interface IAlmacenamiento
    {
        /// <summary>
        /// Sube un archivo al almacenamiento
        /// </summary>
        Task<(bool exito, string url, string nombreArchivo)> SubirArchivoAsync(
            Stream stream, string nombreArchivo, string tipoMime);

        /// <summary>
        /// Elimina un archivo del almacenamiento
        /// </summary>
        Task<bool> EliminarArchivoAsync(string nombreArchivo);

        /// <summary>
        /// Obtiene la URL de acceso a un archivo
        /// </summary>
        string ObtenerUrlAcceso(string nombreArchivo);
    }

    /// <summary>
    /// Implementación de almacenamiento local en wwwroot
    /// </summary>
    public class AlmacenacionLocal : IAlmacenamiento
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IOptions<MultimediaOptions> _options;
        private readonly ILogger<AlmacenacionLocal> _logger;
        private readonly MultimediaOptions _config;

        public AlmacenacionLocal(IWebHostEnvironment environment, IOptions<MultimediaOptions> options,
            ILogger<AlmacenacionLocal> logger)
        {
            _environment = environment;
            _options = options;
            _logger = logger;
            _config = options.Value;

            // Inicializar ruta absoluta
            if (string.IsNullOrEmpty(_config.RutaAbsoluta))
            {
                _config.RutaAbsoluta = Path.Combine(_environment.WebRootPath, _config.RutaAlmacenamiento);
            }
        }

        public async Task<(bool exito, string url, string nombreArchivo)> SubirArchivoAsync(
            Stream stream, string nombreArchivo, string tipoMime)
        {
            try
            {
                // Crear directorio si no existe
                if (!Directory.Exists(_config.RutaAbsoluta))
                {
                    Directory.CreateDirectory(_config.RutaAbsoluta);
                }

                var rutaCompleta = Path.Combine(_config.RutaAbsoluta!, nombreArchivo);

                using (var fileStream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await stream.CopyToAsync(fileStream);
                }

                var urlAcceso = $"{_config.PrefijoDatosUrl}/{nombreArchivo}";
                _logger.LogInformation("Archivo guardado localmente: {NombreArchivo}", nombreArchivo);

                return (true, urlAcceso, nombreArchivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar archivo localmente: {NombreArchivo}", nombreArchivo);
                return (false, string.Empty, nombreArchivo);
            }
        }

        public async Task<bool> EliminarArchivoAsync(string nombreArchivo)
        {
            try
            {
                var rutaCompleta = Path.Combine(_config.RutaAbsoluta!, nombreArchivo);
                
                if (File.Exists(rutaCompleta))
                {
                    File.Delete(rutaCompleta);
                    _logger.LogInformation("Archivo eliminado localmente: {NombreArchivo}", nombreArchivo);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar archivo localmente: {NombreArchivo}", nombreArchivo);
                return false;
            }
        }

        public string ObtenerUrlAcceso(string nombreArchivo)
        {
            return $"{_config.PrefijoDatosUrl}/{nombreArchivo}";
        }
    }

    /// <summary>
    /// Implementación de almacenamiento en Azure Blob Storage (simulada para demostración)
    /// En producción, se usaría Azure.Storage.Blobs
    /// </summary>
    public class AlmacenacionAzure : IAlmacenamiento
    {
        private readonly IOptions<MultimediaOptions> _options;
        private readonly ILogger<AlmacenacionAzure> _logger;
        private readonly MultimediaOptions _config;

        public AlmacenacionAzure(IOptions<MultimediaOptions> options, ILogger<AlmacenacionAzure> logger)
        {
            _options = options;
            _logger = logger;
            _config = options.Value;
        }

        public async Task<(bool exito, string url, string nombreArchivo)> SubirArchivoAsync(
            Stream stream, string nombreArchivo, string tipoMime)
        {
            try
            {
                // Simulación de subida a Azure Blob Storage
                // En producción, usar: Azure.Storage.Blobs.BlobClient
                
                _logger.LogInformation(
                    "Simulando subida a Azure Blob. Contenedor: {Contenedor}, Archivo: {NombreArchivo}",
                    _config.NombreContenedorAzure, nombreArchivo);

                // En un escenario real:
                // var blobContainerClient = new BlobContainerClient(new Uri(...), new DefaultAzureCredential());
                // await blobContainerClient.UploadBlobAsync(nombreArchivo, stream, overwrite: true);

                var urlAzure = $"https://nexshop.blob.core.windows.net/{_config.NombreContenedorAzure}/{nombreArchivo}";
                
                _logger.LogInformation("Archivo subido a Azure (simulado): {NombreArchivo}", nombreArchivo);
                return (true, urlAzure, nombreArchivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir archivo a Azure: {NombreArchivo}", nombreArchivo);
                return (false, string.Empty, nombreArchivo);
            }
        }

        public async Task<bool> EliminarArchivoAsync(string nombreArchivo)
        {
            try
            {
                // Simulación de eliminación en Azure
                _logger.LogInformation(
                    "Simulando eliminación en Azure Blob. Archivo: {NombreArchivo}", nombreArchivo);

                // En un escenario real:
                // var blobClient = new BlobClient(new Uri(...), new DefaultAzureCredential());
                // await blobClient.DeleteAsync();

                _logger.LogInformation("Archivo eliminado de Azure (simulado): {NombreArchivo}", nombreArchivo);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar archivo de Azure: {NombreArchivo}", nombreArchivo);
                return false;
            }
        }

        public string ObtenerUrlAcceso(string nombreArchivo)
        {
            return $"https://nexshop.blob.core.windows.net/{_config.NombreContenedorAzure}/{nombreArchivo}";
        }
    }
}
