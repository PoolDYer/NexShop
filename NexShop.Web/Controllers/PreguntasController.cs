using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NexShop.Web.Models;
using NexShop.Web.Services;
using NexShop.Web.ViewModels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NexShop.Web.Controllers
{
    /// <summary>
    /// Controlador para gestionar preguntas y respuestas
    /// </summary>
    public class PreguntasController : Controller
    {
        private readonly IPreguntaService _preguntaService;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<PreguntasController> _logger;
        private readonly NexShopContext _context;

        public PreguntasController(IPreguntaService preguntaService, UserManager<Usuario> userManager,
            ILogger<PreguntasController> logger, NexShopContext context)
        {
            _preguntaService = preguntaService;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Obtener preguntas de un producto (JSON)
        /// GET: /Preguntas/ObtenerPreguntas/5
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ObtenerPreguntas(int productoId)
        {
            try
            {
                var preguntas = await _preguntaService.ObtenerPreguntasProductoAsync(productoId);
                return PartialView("_PreguntasLista", preguntas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener preguntas. ProductoId: {ProductoId}", productoId);
                return BadRequest("Error al cargar preguntas");
            }
        }

        /// <summary>
        /// Crear una nueva pregunta (AJAX)
        /// POST: /Preguntas/CrearPregunta
        /// </summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPregunta([FromForm] PreguntaCreateViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Datos inválidos" });
                }

                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var resultado = await _preguntaService.CrearPreguntaAsync(viewModel, userId);

                if (!resultado.Exito)
                {
                    return BadRequest(new { error = resultado.Mensaje });
                }

                // Retornar lista actualizada de preguntas
                var preguntas = await _preguntaService.ObtenerPreguntasProductoAsync(viewModel.ProductoId);
                return PartialView("_PreguntasLista", preguntas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear pregunta");
                return BadRequest(new { error = "Error al crear la pregunta" });
            }
        }

        /// <summary>
        /// Crear una respuesta (AJAX)
        /// POST: /Preguntas/CrearRespuesta
        /// </summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearRespuesta([FromForm] RespuestaCreateViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Datos inválidos" });
                }

                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var resultado = await _preguntaService.CrearRespuestaAsync(viewModel, userId);

                if (!resultado.Exito)
                {
                    return BadRequest(new { error = resultado.Mensaje });
                }

                // Retornar respuestas actualizadas
                var respuestas = await _preguntaService.ObtenerRespuestasAsync(viewModel.PreguntaId);
                return PartialView("_RespuestasLista", respuestas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear respuesta");
                return BadRequest(new { error = "Error al crear la respuesta" });
            }
        }

        /// <summary>
        /// Votar pregunta como útil (AJAX)
        /// POST: /Preguntas/VotarPregunta/5
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> VotarPregunta(int preguntaId)
        {
            try
            {
                var resultado = await _preguntaService.VotarUtilPreguntaAsync(preguntaId);

                if (!resultado.Exito)
                {
                    return BadRequest(new { error = resultado.Mensaje });
                }

                var pregunta = await _preguntaService.ObtenerPreguntaAsync(preguntaId);
                return Json(new { exito = true, votosUtiles = pregunta?.VotosUtiles ?? 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al votar pregunta");
                return BadRequest(new { error = "Error al registrar voto" });
            }
        }

        /// <summary>
        /// Votar respuesta como útil (AJAX)
        /// POST: /Preguntas/VotarRespuesta/5
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> VotarRespuesta(int respuestaId)
        {
            try
            {
                var resultado = await _preguntaService.VotarUtilRespuestaAsync(respuestaId);

                if (!resultado.Exito)
                {
                    return BadRequest(new { error = resultado.Mensaje });
                }

                return Json(new { exito = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al votar respuesta");
                return BadRequest(new { error = "Error al registrar voto" });
            }
        }

        /// <summary>
        /// Crear una nueva reseña de producto
        /// POST: /Preguntas/CrearResena
        /// </summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearResena([FromForm] CrearReseñaViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Los datos no son válidos" });
                }

                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Obtener el producto y su vendedor
                var producto = await _context.Productos.FindAsync(viewModel.ProductoId);
                if (producto == null)
                {
                    return BadRequest(new { error = "Producto no encontrado" });
                }

                // Verificar si el usuario ya escribió una reseña para este producto
                var reseñaExistente = _context.Calificaciones
                    .Any(c => c.ProductoId == viewModel.ProductoId 
                            && c.UsuarioId == userId 
                            && c.Tipo == "Producto");

                if (reseñaExistente)
                {
                    return BadRequest(new { error = "Ya escribiste una reseña para este producto" });
                }

                // Crear la calificación
                var calificacion = new Calificacion
                {
                    ProductoId = viewModel.ProductoId,
                    Puntaje = viewModel.CalificacionGeneral,
                    Comentario = viewModel.Comentario,
                    Titulo = viewModel.Titulo,
                    CalificacionAtencion = viewModel.CalificacionAtencion,
                    CalificacionEnvio = viewModel.CalificacionEnvio,
                    Tipo = "Producto",
                    UsuarioId = userId,
                    VendedorId = producto.VendedorId,
                    FechaCreacion = DateTime.UtcNow
                };

                // Guardar en base de datos
                _context.Calificaciones.Add(calificacion);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reseña creada para producto {ProductoId} por usuario {UserId}", 
                    viewModel.ProductoId, userId);

                return Ok(new { exito = true, mensaje = "Reseña enviada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear reseña");
                return BadRequest(new { error = "Error al enviar la reseña" });
            }
        }
    }
}
