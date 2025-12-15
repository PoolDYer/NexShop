using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Servicio para sincronizar imágenes de productos desde carpetas a wwwroot
    /// </summary>
    public interface ISincronizacionImagenesService
    {
        /// <summary>
        /// Sincroniza imágenes de todas las carpetas de productos
        /// </summary>
        Task<SincronizacionResultado> SincronizarTodasLasImagenes();

        /// <summary>
        /// Sincroniza imágenes de un producto específico
        /// </summary>
        Task<List<string>> ObtenerImagenesProducto(int productoId);

        /// <summary>
        /// Obtiene la ruta relativa de una imagen de producto
        /// </summary>
        string ObtenerRutaImagenProducto(int productoId, string nombreArchivo);
    }

    public class SincronizacionImagenesService : ISincronizacionImagenesService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<SincronizacionImagenesService> _logger;

        // Rutas base
        private readonly string _carpetasProductosOrigen; // E:\...\uploads\productos
        private readonly string _imagenesdestino;          // wwwroot/imagenes/productos

        public SincronizacionImagenesService(IWebHostEnvironment webHostEnvironment, ILogger<SincronizacionImagenesService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;

            // Carpeta origen (carpetas de productos con imágenes)
            _carpetasProductosOrigen = Path.Combine(
                Path.GetDirectoryName(_webHostEnvironment.WebRootPath)?.Replace("wwwroot", "") ?? "",
                "..", "..", // Subir dos niveles desde wwwroot
                "uploads", "productos"
            );

            // Carpeta destino (wwwroot/imagenes/productos)
            _imagenesdestino = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes", "productos");

            _logger.LogInformation("Ruta origen (uploads/productos): {Origen}", _carpetasProductosOrigen);
            _logger.LogInformation("Ruta destino (wwwroot/imagenes/productos): {Destino}", _imagenesdestino);
        }

        public async Task<SincronizacionResultado> SincronizarTodasLasImagenes()
        {
            var resultado = new SincronizacionResultado();

            try
            {
                // Crear carpeta destino si no existe
                if (!Directory.Exists(_imagenesdestino))
                {
                    Directory.CreateDirectory(_imagenesdestino);
                    _logger.LogInformation("Carpeta de destino creada: {Destino}", _imagenesdestino);
                }

                // Obtener todas las carpetas de productos
                var carpetasProductos = Directory.GetDirectories(_carpetasProductosOrigen);
                _logger.LogInformation("Se encontraron {Cantidad} carpetas de productos", carpetasProductos.Length);

                foreach (var carpetaProducto in carpetasProductos)
                {
                    try
                    {
                        var nombreCarpeta = Path.GetFileName(carpetaProducto);
                        _logger.LogInformation("Procesando carpeta: {Carpeta}", nombreCarpeta);

                        // Obtener imágenes de la carpeta
                        var archivos = Directory.GetFiles(carpetaProducto)
                            .Where(f => esImagenValida(f))
                            .ToList();

                        if (archivos.Any())
                        {
                            // Crear subcarpeta para el producto
                            var carpetaDestino = Path.Combine(_imagenesdestino, nombreCarpeta);
                            if (!Directory.Exists(carpetaDestino))
                            {
                                Directory.CreateDirectory(carpetaDestino);
                            }

                            // Copiar imágenes
                            foreach (var archivo in archivos)
                            {
                                try
                                {
                                    var nombreArchivo = Path.GetFileName(archivo);
                                    var rutaDestino = Path.Combine(carpetaDestino, nombreArchivo);

                                    // Copiar solo si no existe o si es más reciente
                                    if (!File.Exists(rutaDestino) || File.GetLastWriteTime(archivo) > File.GetLastWriteTime(rutaDestino))
                                    {
                                        File.Copy(archivo, rutaDestino, true);
                                        resultado.ImagenesthGraciasCopiadasExitosamente++;
                                        _logger.LogInformation("Imagen copiada: {Archivo}", nombreArchivo);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    resultado.Errores++;
                                    _logger.LogError(ex, "Error al copiar imagen: {Archivo}", archivo);
                                }
                            }

                            resultado.CarpetasProcesadas++;
                        }
                    }
                    catch (Exception ex)
                    {
                        resultado.Errores++;
                        _logger.LogError(ex, "Error procesando carpeta: {Carpeta}", carpetaProducto);
                    }
                }

                resultado.Exitoso = resultado.Errores == 0;
                return resultado;
            }
            catch (Exception ex)
            {
                resultado.Exitoso = false;
                resultado.Errores++;
                _logger.LogError(ex, "Error en sincronización de imágenes");
                throw;
            }
        }

        public async Task<List<string>> ObtenerImagenesProducto(int productoId)
        {
            var imagenes = new List<string>();

            try
            {
                // Buscar carpetas que comiencen con el ID del producto
                var carpetaProducto = Directory.GetDirectories(_carpetasProductosOrigen)
                    .FirstOrDefault(d => Path.GetFileName(d).StartsWith($"{productoId}_"));

                if (carpetaProducto == null)
                {
                    _logger.LogWarning("No se encontró carpeta para producto ID: {ProductoId}", productoId);
                    return imagenes;
                }

                // Obtener imágenes de la carpeta
                var archivos = Directory.GetFiles(carpetaProducto)
                    .Where(f => esImagenValida(f))
                    .OrderBy(f => Path.GetFileName(f))
                    .ToList();

                foreach (var archivo in archivos)
                {
                    imagenes.Add(Path.GetFileName(archivo));
                }

                return imagenes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo imágenes para producto: {ProductoId}", productoId);
                return imagenes;
            }
        }

        public string ObtenerRutaImagenProducto(int productoId, string nombreArchivo)
        {
            // Construir ruta relativa: /imagenes/productos/{id_nombre}/{archivo}
            var carpetaProducto = Directory.GetDirectories(_carpetasProductosOrigen)
                .Select(d => Path.GetFileName(d))
                .FirstOrDefault(d => d.StartsWith($"{productoId}_"));

            if (string.IsNullOrEmpty(carpetaProducto))
            {
                _logger.LogWarning("No se encontró carpeta para producto: {ProductoId}", productoId);
                return null;
            }

            return $"/imagenes/productos/{carpetaProducto}/{nombreArchivo}";
        }

        private bool esImagenValida(string rutaArchivo)
        {
            var extensionesValidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(rutaArchivo).ToLower();
            return extensionesValidas.Contains(extension);
        }
    }

    /// <summary>
    /// Resultado de la sincronización de imágenes
    /// </summary>
    public class SincronizacionResultado
    {
        public bool Exitoso { get; set; }
        public int CarpetasProcesadas { get; set; }
        public int ImagenesthGraciasCopiadasExitosamente { get; set; }
        public int Errores { get; set; }
        public string Mensaje { get; set; }

        public override string ToString()
        {
            return $"Carpetas: {CarpetasProcesadas}, Imágenes: {ImagenesthGraciasCopiadasExitosamente}, Errores: {Errores}";
        }
    }
}
