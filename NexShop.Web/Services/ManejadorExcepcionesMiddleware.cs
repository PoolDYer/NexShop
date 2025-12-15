using System.Net;
using System.Text.Json;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Middleware global para capturar y manejar excepciones no controladas
    /// </summary>
    public class ManejadorExcepcionesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ManejadorExcepcionesMiddleware> _logger;

        public ManejadorExcepcionesMiddleware(RequestDelegate next, ILogger<ManejadorExcepcionesMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción no controlada: {Message}", ex.Message);
                await ManejarExcepcionAsync(context, ex);
            }
        }

        private static Task ManejarExcepcionAsync(HttpContext context, Exception excepcion)
        {
            context.Response.ContentType = "application/json";

            var respuesta = new
            {
                mensaje = "Ocurrió un error inesperado",
                codigoError = "ERROR_INESPERADO",
                detalles = excepcion.Message
            };

            switch (excepcion)
            {
                case ArgumentException argEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    respuesta = new
                    {
                        mensaje = "Argumento inválido",
                        codigoError = "ARGUMENTO_INVALIDO",
                        detalles = argEx.Message
                    };
                    break;

                case UnauthorizedAccessException uaEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    respuesta = new
                    {
                        mensaje = "Acceso denegado",
                        codigoError = "ACCESO_DENEGADO",
                        detalles = uaEx.Message
                    };
                    break;

                case InvalidOperationException ioEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    respuesta = new
                    {
                        mensaje = "Operación inválida",
                        codigoError = "OPERACION_INVALIDA",
                        detalles = ioEx.Message
                    };
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    respuesta = new
                    {
                        mensaje = "Ocurrió un error inesperado",
                        codigoError = "ERROR_INESPERADO",
                        detalles = excepcion.Message
                    };
                    break;
            }

            var json = JsonSerializer.Serialize(respuesta);
            return context.Response.WriteAsync(json);
        }
    }

    /// <summary>
    /// Extensión para registrar el middleware de manejo de excepciones
    /// </summary>
    public static class ManejadorExcepcionesExtension
    {
        public static IApplicationBuilder UsarManejadorExcepciones(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ManejadorExcepcionesMiddleware>();
        }
    }
}
