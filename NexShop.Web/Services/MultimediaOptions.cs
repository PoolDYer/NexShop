using System.Collections.Generic;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Opciones configurables para carga de archivos multimedia
    /// Se vincula desde appsettings.json
    /// </summary>
    public class MultimediaOptions
    {
        public const string SectionName = "Multimedia";

        /// <summary>
        /// Tamaño máximo permitido por archivo en MB
        /// </summary>
        public int TamanoMaximoMB { get; set; } = 50;

        /// <summary>
        /// Tamaño máximo permitido por archivo en bytes
        /// </summary>
        public long TamanoMaximoBytes => TamanoMaximoMB * 1024 * 1024;

        /// <summary>
        /// Tipos MIME permitidos para imágenes
        /// </summary>
        public List<string> TiposMimeImagenesPermitidos { get; set; } = new()
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/webp",
            "image/svg+xml"
        };

        /// <summary>
        /// Tipos MIME permitidos para videos
        /// </summary>
        public List<string> TiposMimeVideosPermitidos { get; set; } = new()
        {
            "video/mp4",
            "video/webm",
            "video/ogg",
            "video/quicktime"
        };

        /// <summary>
        /// Extensiones de archivo permitidas para imágenes
        /// </summary>
        public List<string> ExtensionesImagenesPermitidas { get; set; } = new()
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg"
        };

        /// <summary>
        /// Extensiones de archivo permitidas para videos
        /// </summary>
        public List<string> ExtensionesVideosPermitidas { get; set; } = new()
        {
            ".mp4", ".webm", ".ogv", ".mov"
        };

        /// <summary>
        /// Ruta relativa en wwwroot para almacenar archivos
        /// </summary>
        public string RutaAlmacenamiento { get; set; } = "uploads/multimedia";

        /// <summary>
        /// Ruta absoluta en el servidor (se calcula al iniciar)
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public string? RutaAbsoluta { get; set; }

        /// <summary>
        /// Prefijo de URL para acceder a archivos
        /// </summary>
        public string PrefijoDatosUrl { get; set; } = "/uploads/multimedia";

        /// <summary>
        /// Habilitar almacenamiento en Azure Blob Storage
        /// </summary>
        public bool UsarAzureBlob { get; set; } = false;

        /// <summary>
        /// Cadena de conexión para Azure Blob Storage
        /// </summary>
        public string? CadenaConexionAzure { get; set; }

        /// <summary>
        /// Nombre del contenedor en Azure Blob Storage
        /// </summary>
        public string? NombreContenedorAzure { get; set; } = "multimedia";

        /// <summary>
        /// Permitir imágenes duplicadas (misma extensión hash)
        /// </summary>
        public bool PermitirDuplicados { get; set; } = false;

        /// <summary>
        /// Generar miniaturas para imágenes
        /// </summary>
        public bool GenerarMiniaturas { get; set; } = true;

        /// <summary>
        /// Tamaño de miniatura en píxeles
        /// </summary>
        public int TamanoMiniaturaPx { get; set; } = 300;
    }

    /// <summary>
    /// Resultado de una operación de carga de archivo
    /// </summary>
    public class ResultadoCargaArchivo
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string? NombreArchivo { get; set; }
        public string? Url { get; set; }
        public long TamanoBytes { get; set; }
        public string? TipoMime { get; set; }
        public string? CodigoError { get; set; }

        public static ResultadoCargaArchivo Success(string nombreArchivo, string url, long tamanoByte, string tipoMime)
        {
            return new ResultadoCargaArchivo
            {
                Exito = true,
                Mensaje = "Archivo cargado exitosamente",
                NombreArchivo = nombreArchivo,
                Url = url,
                TamanoBytes = tamanoByte,
                TipoMime = tipoMime
            };
        }

        public static ResultadoCargaArchivo Error(string mensaje, string? codigoError = null)
        {
            return new ResultadoCargaArchivo
            {
                Exito = false,
                Mensaje = mensaje,
                CodigoError = codigoError ?? "ERROR_CARGA"
            };
        }
    }

    /// <summary>
    /// Resultado de operación de carga múltiple
    /// </summary>
    public class ResultadoCargaMultiple
    {
        public bool ExitoTotal { get; set; }
        public int TotalProcesados { get; set; }
        public int TotalExitosos { get; set; }
        public int TotalErrores { get; set; }
        public List<ResultadoCargaArchivo> Resultados { get; set; } = new();
        public string Mensaje { get; set; } = string.Empty;

        public static ResultadoCargaMultiple Crear(List<ResultadoCargaArchivo> resultados)
        {
            var exitosos = resultados.Count(r => r.Exito);
            var errores = resultados.Count(r => !r.Exito);
            
            return new ResultadoCargaMultiple
            {
                ExitoTotal = errores == 0,
                TotalProcesados = resultados.Count,
                TotalExitosos = exitosos,
                TotalErrores = errores,
                Resultados = resultados,
                Mensaje = $"Se procesaron {resultados.Count} archivos: {exitosos} exitosos, {errores} errores"
            };
        }
    }
}
