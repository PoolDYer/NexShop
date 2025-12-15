using Microsoft.EntityFrameworkCore;
using NexShop.Web.Models;
using NexShop.Web.ViewModels;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Interfaz para el servicio de preguntas y respuestas
    /// </summary>
    public interface IPreguntaService
    {
        /// <summary>
        /// Crear una nueva pregunta
        /// </summary>
        Task<ResultadoOperacion> CrearPreguntaAsync(PreguntaCreateViewModel viewModel, string usuarioId);

        /// <summary>
        /// Obtener preguntas de un producto
        /// </summary>
        Task<List<PreguntaListViewModel>> ObtenerPreguntasProductoAsync(int productoId);

        /// <summary>
        /// Obtener una pregunta específica
        /// </summary>
        Task<PreguntaListViewModel?> ObtenerPreguntaAsync(int preguntaId);

        /// <summary>
        /// Crear una respuesta a una pregunta
        /// </summary>
        Task<ResultadoOperacion> CrearRespuestaAsync(RespuestaCreateViewModel viewModel, string usuarioId);

        /// <summary>
        /// Obtener respuestas de una pregunta
        /// </summary>
        Task<List<RespuestaListViewModel>> ObtenerRespuestasAsync(int preguntaId);

        /// <summary>
        /// Marcar pregunta como respondida
        /// </summary>
        Task<ResultadoOperacion> ActualizarEstadoPreguntaAsync(int preguntaId, string nuevoEstado);

        /// <summary>
        /// Incrementar votos útiles de una pregunta
        /// </summary>
        Task<ResultadoOperacion> VotarUtilPreguntaAsync(int preguntaId);

        /// <summary>
        /// Incrementar votos útiles de una respuesta
        /// </summary>
        Task<ResultadoOperacion> VotarUtilRespuestaAsync(int respuestaId);
    }

    /// <summary>
    /// Implementación del servicio de preguntas y respuestas
    /// </summary>
    public class PreguntaService : IPreguntaService
    {
        private readonly NexShopContext _context;
        private readonly ILogger<PreguntaService> _logger;

        public PreguntaService(NexShopContext context, ILogger<PreguntaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Crear una nueva pregunta
        /// </summary>
        public async Task<ResultadoOperacion> CrearPreguntaAsync(PreguntaCreateViewModel viewModel, string usuarioId)
        {
            try
            {
                // Validar que el producto existe
                var producto = await _context.Productos.FindAsync(viewModel.ProductoId);
                if (producto == null)
                {
                    return ResultadoOperacion.Error("El producto no existe");
                }

                // Crear la pregunta
                var pregunta = new Pregunta
                {
                    Titulo = viewModel.Titulo,
                    Descripcion = viewModel.Descripcion,
                    ProductoId = viewModel.ProductoId,
                    UsuarioId = usuarioId,
                    Estado = "Pendiente",
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Preguntas.Add(pregunta);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Pregunta creada exitosamente. PreguntaId: {PreguntaId}, ProductoId: {ProductoId}, UsuarioId: {UsuarioId}",
                    pregunta.PreguntaId, viewModel.ProductoId, usuarioId);

                return ResultadoOperacion.Success("Pregunta publicada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear pregunta");
                return ResultadoOperacion.Error("Error al crear la pregunta");
            }
        }

        /// <summary>
        /// Obtener preguntas de un producto
        /// </summary>
        public async Task<List<PreguntaListViewModel>> ObtenerPreguntasProductoAsync(int productoId)
        {
            try
            {
                var preguntas = await _context.Preguntas
                    .Where(p => p.ProductoId == productoId)
                    .Include(p => p.Usuario)
                    .Include(p => p.Respuestas)
                    .ThenInclude(r => r.Usuario)
                    .OrderByDescending(p => p.FechaCreacion)
                    .ToListAsync();

                return preguntas.Select(p => MapearPregunta(p)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener preguntas del producto. ProductoId: {ProductoId}", productoId);
                return new List<PreguntaListViewModel>();
            }
        }

        /// <summary>
        /// Obtener una pregunta específica
        /// </summary>
        public async Task<PreguntaListViewModel?> ObtenerPreguntaAsync(int preguntaId)
        {
            try
            {
                var pregunta = await _context.Preguntas
                    .Include(p => p.Usuario)
                    .Include(p => p.Respuestas)
                    .ThenInclude(r => r.Usuario)
                    .FirstOrDefaultAsync(p => p.PreguntaId == preguntaId);

                return pregunta != null ? MapearPregunta(pregunta) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pregunta. PreguntaId: {PreguntaId}", preguntaId);
                return null;
            }
        }

        /// <summary>
        /// Crear una respuesta a una pregunta
        /// </summary>
        public async Task<ResultadoOperacion> CrearRespuestaAsync(RespuestaCreateViewModel viewModel, string usuarioId)
        {
            try
            {
                // Validar que la pregunta existe
                var pregunta = await _context.Preguntas.FindAsync(viewModel.PreguntaId);
                if (pregunta == null)
                {
                    return ResultadoOperacion.Error("La pregunta no existe");
                }

                // Crear la respuesta
                var respuesta = new Respuesta
                {
                    Contenido = viewModel.Contenido,
                    PreguntaId = viewModel.PreguntaId,
                    UsuarioId = usuarioId,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Respuestas.Add(respuesta);

                // Actualizar contador de respuestas
                pregunta.NumeroRespuestas++;

                // Si hay respuesta, cambiar estado a "Respondida"
                if (pregunta.Estado == "Pendiente")
                {
                    pregunta.Estado = "Respondida";
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Respuesta creada exitosamente. RespuestaId: {RespuestaId}, PreguntaId: {PreguntaId}",
                    respuesta.RespuestaId, viewModel.PreguntaId);

                return ResultadoOperacion.Success("Respuesta publicada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear respuesta. PreguntaId: {PreguntaId}", viewModel.PreguntaId);
                return ResultadoOperacion.Error("Error al crear la respuesta");
            }
        }

        /// <summary>
        /// Obtener respuestas de una pregunta
        /// </summary>
        public async Task<List<RespuestaListViewModel>> ObtenerRespuestasAsync(int preguntaId)
        {
            try
            {
                var respuestas = await _context.Respuestas
                    .Where(r => r.PreguntaId == preguntaId)
                    .Include(r => r.Usuario)
                    .OrderByDescending(r => r.EsRespuestaOficial)
                    .ThenByDescending(r => r.VotosUtiles)
                    .ThenBy(r => r.FechaCreacion)
                    .ToListAsync();

                return respuestas.Select(r => MapearRespuesta(r)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener respuestas. PreguntaId: {PreguntaId}", preguntaId);
                return new List<RespuestaListViewModel>();
            }
        }

        /// <summary>
        /// Marcar pregunta como respondida
        /// </summary>
        public async Task<ResultadoOperacion> ActualizarEstadoPreguntaAsync(int preguntaId, string nuevoEstado)
        {
            try
            {
                var pregunta = await _context.Preguntas.FindAsync(preguntaId);
                if (pregunta == null)
                {
                    return ResultadoOperacion.Error("La pregunta no existe");
                }

                pregunta.Estado = nuevoEstado;
                pregunta.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Estado de pregunta actualizado. PreguntaId: {PreguntaId}, NuevoEstado: {NuevoEstado}",
                    preguntaId, nuevoEstado);

                return ResultadoOperacion.Success("Pregunta actualizada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar pregunta. PreguntaId: {PreguntaId}", preguntaId);
                return ResultadoOperacion.Error("Error al actualizar la pregunta");
            }
        }

        /// <summary>
        /// Incrementar votos útiles de una pregunta
        /// </summary>
        public async Task<ResultadoOperacion> VotarUtilPreguntaAsync(int preguntaId)
        {
            try
            {
                var pregunta = await _context.Preguntas.FindAsync(preguntaId);
                if (pregunta == null)
                {
                    return ResultadoOperacion.Error("La pregunta no existe");
                }

                pregunta.VotosUtiles++;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Voto útil agregado a pregunta. PreguntaId: {PreguntaId}", preguntaId);

                return ResultadoOperacion.Success("Voto registrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al votar pregunta. PreguntaId: {PreguntaId}", preguntaId);
                return ResultadoOperacion.Error("Error al registrar voto");
            }
        }

        /// <summary>
        /// Incrementar votos útiles de una respuesta
        /// </summary>
        public async Task<ResultadoOperacion> VotarUtilRespuestaAsync(int respuestaId)
        {
            try
            {
                var respuesta = await _context.Respuestas.FindAsync(respuestaId);
                if (respuesta == null)
                {
                    return ResultadoOperacion.Error("La respuesta no existe");
                }

                respuesta.VotosUtiles++;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Voto útil agregado a respuesta. RespuestaId: {RespuestaId}", respuestaId);

                return ResultadoOperacion.Success("Voto registrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al votar respuesta. RespuestaId: {RespuestaId}", respuestaId);
                return ResultadoOperacion.Error("Error al registrar voto");
            }
        }

        /// <summary>
        /// Mapear Pregunta a PreguntaListViewModel
        /// </summary>
        private PreguntaListViewModel MapearPregunta(Pregunta pregunta)
        {
            return new PreguntaListViewModel
            {
                PreguntaId = pregunta.PreguntaId,
                Titulo = pregunta.Titulo,
                Descripcion = pregunta.Descripcion,
                UsuarioNombre = pregunta.Usuario?.NombreCompleto ?? "Anónimo",
                UsuarioId = pregunta.UsuarioId,
                ProductoId = pregunta.ProductoId,
                Estado = pregunta.Estado,
                NumeroRespuestas = pregunta.NumeroRespuestas,
                VotosUtiles = pregunta.VotosUtiles,
                FechaCreacion = pregunta.FechaCreacion,
                Respuestas = pregunta.Respuestas
                    .OrderByDescending(r => r.EsRespuestaOficial)
                    .ThenByDescending(r => r.VotosUtiles)
                    .Select(r => MapearRespuesta(r))
                    .ToList()
            };
        }

        /// <summary>
        /// Mapear Respuesta a RespuestaListViewModel
        /// </summary>
        private RespuestaListViewModel MapearRespuesta(Respuesta respuesta)
        {
            return new RespuestaListViewModel
            {
                RespuestaId = respuesta.RespuestaId,
                Contenido = respuesta.Contenido,
                UsuarioNombre = respuesta.Usuario?.NombreCompleto ?? "Anónimo",
                UsuarioId = respuesta.UsuarioId,
                EsRespuestaOficial = respuesta.EsRespuestaOficial,
                VotosUtiles = respuesta.VotosUtiles,
                FechaCreacion = respuesta.FechaCreacion
            };
        }
    }
}
