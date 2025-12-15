using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexShop.Web.Models;

namespace NexShop.Web.Controllers
{
    /// <summary>
    /// Controlador para manejar errores de autorización
    /// </summary>
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Muestra la página de acceso denegado (403 Forbidden)
        /// GET: /Error/AccesoDenegado
        /// </summary>
        [HttpGet("~/Error/AccesoDenegado")]
        public IActionResult AccesoDenegado()
        {
            _logger.LogWarning("Usuario intentó acceder a un recurso sin permisos");
            return View();
        }

        /// <summary>
        /// Muestra la página de no autorizado (401 Unauthorized)
        /// GET: /Error/NoAutorizado
        /// </summary>
        [HttpGet("~/Error/NoAutorizado")]
        public IActionResult NoAutorizado()
        {
            _logger.LogInformation("Usuario no autenticado intentó acceder a un recurso protegido");
            return View();
        }

        /// <summary>
        /// Maneja errores generales (404, 500, etc.)
        /// GET: /Error/{statusCode}
        /// </summary>
        [HttpGet("~/Error/{statusCode?}")]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode == null)
            {
                statusCode = HttpContext.Response.StatusCode;
            }

            _logger.LogError("Error HTTP {StatusCode}", statusCode);

            var mensajeError = statusCode switch
            {
                400 => "Solicitud inválida",
                401 => "No autorizado",
                403 => "Acceso denegado",
                404 => "Página no encontrada",
                500 => "Error interno del servidor",
                503 => "Servicio no disponible",
                _ => "Ha ocurrido un error"
            };

            ViewBag.StatusCode = statusCode;
            ViewBag.Mensaje = mensajeError;

            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }
    }
}
