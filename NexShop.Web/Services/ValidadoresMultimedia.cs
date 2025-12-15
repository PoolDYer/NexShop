using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Atributo de validación personalizado para archivos multimedia
    /// Se aplica a propiedades IFormFile para validar tipo MIME y tamaño
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidarMultimediaAttribute : ValidationAttribute
    {
        private readonly bool _permitirImagenes;
        private readonly bool _permitirVideos;

        public ValidarMultimediaAttribute(bool permitirImagenes = true, bool permitirVideos = true)
        {
            _permitirImagenes = permitirImagenes;
            _permitirVideos = permitirVideos;
            ErrorMessage = "El archivo no cumple con los requisitos de multimedia";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || value is not IFormFile archivo)
            {
                return ValidationResult.Success;
            }

            // Obtener opciones desde ServiceProvider
            var options = validationContext.GetService(typeof(IOptions<MultimediaOptions>)) as IOptions<MultimediaOptions>;
            if (options == null)
            {
                return ValidationResult.Success;
            }

            var config = options.Value;
            var tipoMime = archivo.ContentType?.ToLowerInvariant() ?? string.Empty;

            // Validar tipo MIME
            var esImagen = config.TiposMimeImagenesPermitidos.Contains(tipoMime);
            var esVideo = config.TiposMimeVideosPermitidos.Contains(tipoMime);

            if ((!esImagen && !esVideo) || 
                (esImagen && !_permitirImagenes) || 
                (esVideo && !_permitirVideos))
            {
                return new ValidationResult("El tipo de archivo no es permitido");
            }

            // Validar tamaño
            if (archivo.Length > config.TamanoMaximoBytes)
            {
                return new ValidationResult($"El archivo excede el tamaño máximo de {config.TamanoMaximoMB} MB");
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Atributo de validación personalizado para extensiones de archivo
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidarExtensionArchivoAttribute : ValidationAttribute
    {
        private readonly string[] _extensionesPermitidas;

        public ValidarExtensionArchivoAttribute(params string[] extensiones)
        {
            _extensionesPermitidas = extensiones.Select(e => e.StartsWith(".") ? e.ToLowerInvariant() : $".{e.ToLowerInvariant()}").ToArray();
            ErrorMessage = $"Extensión no permitida. Permitidas: {string.Join(", ", _extensionesPermitidas)}";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || value is not IFormFile archivo)
            {
                return ValidationResult.Success;
            }

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

            if (!_extensionesPermitidas.Contains(extension))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Atributo de validación personalizado para tamaño de archivo
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidarTamanoArchivoAttribute : ValidationAttribute
    {
        private readonly int _tamanoMaximoMB;

        public ValidarTamanoArchivoAttribute(int tamanoMaximoMB)
        {
            _tamanoMaximoMB = tamanoMaximoMB;
            ErrorMessage = $"El archivo no debe exceder {tamanoMaximoMB} MB";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || value is not IFormFile archivo)
            {
                return ValidationResult.Success;
            }

            var tamanoMaximoBytes = _tamanoMaximoMB * 1024 * 1024;

            if (archivo.Length > tamanoMaximoBytes)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Validador personalizado para validar lista de archivos
    /// </summary>
    public class ValidadorListaArchivos
    {
        private readonly IOptions<MultimediaOptions> _options;

        public ValidadorListaArchivos(IOptions<MultimediaOptions> options)
        {
            _options = options;
        }

        /// <summary>
        /// Valida una lista de archivos
        /// </summary>
        public List<string> ValidarMultiplesArchivos(List<IFormFile> archivos, int maxArchivos = 10)
        {
            var errores = new List<string>();

            if (!archivos.Any())
            {
                errores.Add("Debe proporcionar al menos un archivo");
                return errores;
            }

            if (archivos.Count > maxArchivos)
            {
                errores.Add($"No puede cargar más de {maxArchivos} archivos simultáneamente");
                return errores;
            }

            var config = _options.Value;
            var tamanioTotalBytes = 0L;

            foreach (var archivo in archivos)
            {
                var archivoErrores = ValidarArchivoIndividual(archivo, config);
                errores.AddRange(archivoErrores);

                tamanioTotalBytes += archivo.Length;
            }

            // Validar tamaño total (ejemplo: 500 MB límite)
            var tamanoTotalMaximoBytes = 500L * 1024 * 1024;
            if (tamanioTotalBytes > tamanoTotalMaximoBytes)
            {
                errores.Add($"El tamaño total de archivos excede 500 MB");
            }

            return errores;
        }

        /// <summary>
        /// Valida un archivo individual
        /// </summary>
        private List<string> ValidarArchivoIndividual(IFormFile archivo, MultimediaOptions config)
        {
            var errores = new List<string>();
            var nombreArchivo = Path.GetFileName(archivo.FileName);

            // Validación de tamaño
            if (archivo.Length == 0)
            {
                errores.Add($"{nombreArchivo}: El archivo está vacío");
                return errores;
            }

            if (archivo.Length > config.TamanoMaximoBytes)
            {
                errores.Add($"{nombreArchivo}: Excede tamaño máximo de {config.TamanoMaximoMB} MB");
            }

            // Validación de tipo MIME
            var tipoMime = archivo.ContentType?.ToLowerInvariant() ?? string.Empty;
            var esImagen = config.TiposMimeImagenesPermitidos.Contains(tipoMime);
            var esVideo = config.TiposMimeVideosPermitidos.Contains(tipoMime);

            if (!esImagen && !esVideo)
            {
                errores.Add($"{nombreArchivo}: Tipo de archivo no permitido ({tipoMime})");
            }

            // Validación de extensión
            var extension = Path.GetExtension(nombreArchivo).ToLowerInvariant();
            var extensionesPermitidas = esImagen 
                ? config.ExtensionesImagenesPermitidas 
                : config.ExtensionesVideosPermitidas;

            if (!extensionesPermitidas.Contains(extension))
            {
                errores.Add($"{nombreArchivo}: Extensión no permitida ({extension})");
            }

            return errores;
        }
    }

    /// <summary>
    /// Extensión para añadir validadores personalizados
    /// </summary>
    public static class ValidacionMultimediaExtensions
    {
        public static IServiceCollection AddValidacionMultimedia(this IServiceCollection services)
        {
            services.AddScoped<ValidadorListaArchivos>();
            return services;
        }
    }
}
