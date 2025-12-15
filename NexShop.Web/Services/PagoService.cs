using NexShop.Web.Models;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Resultado de procesamiento de pago
    /// </summary>
    public class ResultadoPago
    {
        /// <summary>
        /// Indica si el pago fue exitoso
        /// </summary>
        public bool EsExitoso { get; set; }

        /// <summary>
        /// Mensaje descriptivo del resultado
        /// </summary>
        public string Mensaje { get; set; } = string.Empty;

        /// <summary>
        /// ID de transacción del pago
        /// </summary>
        public string IdTransaccion { get; set; } = string.Empty;

        /// <summary>
        /// Código de respuesta del procesador de pagos
        /// </summary>
        public string CodigoRespuesta { get; set; } = string.Empty;

        /// <summary>
        /// Hora exacta del procesamiento
        /// </summary>
        public DateTime FechaProcesamiento { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Servicio que simula el procesamiento de pagos
    /// Para propósitos de demostración y pruebas
    /// </summary>
    public interface IPagoService
    {
        /// <summary>
        /// Procesa un pago simulado
        /// </summary>
        /// <param name="orden">La orden a procesar</param>
        /// <param name="metodoPago">Método de pago seleccionado</param>
        /// <returns>Resultado del procesamiento del pago</returns>
        Task<ResultadoPago> ProcesarPagoAsync(Orden orden, string metodoPago);

        /// <summary>
        /// Verifica el estado de un pago
        /// </summary>
        /// <param name="idTransaccion">ID de la transacción</param>
        /// <returns>Resultado del pago</returns>
        Task<ResultadoPago> VerificarPagoAsync(string idTransaccion);
    }

    /// <summary>
    /// Implementación del servicio de pago simulado
    /// </summary>
    public class PagoSimuladoService : IPagoService
    {
        private readonly ILogger<PagoSimuladoService> _logger;
        private readonly Random _random = new Random();

        public PagoSimuladoService(ILogger<PagoSimuladoService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Procesa un pago simulado
        /// Simula un procesamiento real con:
        /// - Retardo de 2-4 segundos
        /// - 95% de éxito, 5% de fallo
        /// - Generación de ID de transacción único
        /// </summary>
        public async Task<ResultadoPago> ProcesarPagoAsync(Orden orden, string metodoPago)
        {
            try
            {
                _logger.LogInformation("Iniciando procesamiento de pago simulado. OrdenId: {OrdenId}, Monto: {Monto}, Método: {Metodo}",
                    orden.OrdenId, orden.MontoTotal, metodoPago);

                // Simular tiempo de procesamiento (2-4 segundos)
                var tiempoEspera = _random.Next(2000, 4000);
                await Task.Delay(tiempoEspera);

                // Simular tasa de éxito del 95%
                var esExitoso = _random.Next(0, 100) < 95;

                var resultado = new ResultadoPago
                {
                    EsExitoso = esExitoso,
                    IdTransaccion = GenerarIdTransaccion(),
                    CodigoRespuesta = esExitoso ? "00" : "99",
                    FechaProcesamiento = DateTime.UtcNow
                };

                if (esExitoso)
                {
                    resultado.Mensaje = $"Pago procesado exitosamente por ${orden.MontoTotal:F2} mediante {metodoPago}";

                    _logger.LogInformation("Pago procesado exitosamente. OrdenId: {OrdenId}, IdTransaccion: {IdTransaccion}",
                        orden.OrdenId, resultado.IdTransaccion);
                }
                else
                {
                    resultado.Mensaje = "El pago fue rechazado. Por favor, verifica tu información de pago e intenta nuevamente.";

                    _logger.LogWarning("Pago rechazado. OrdenId: {OrdenId}, Método: {Metodo}",
                        orden.OrdenId, metodoPago);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el procesamiento de pago. OrdenId: {OrdenId}",
                    orden.OrdenId);

                return new ResultadoPago
                {
                    EsExitoso = false,
                    Mensaje = "Ocurrió un error al procesar el pago. Por favor, intenta más tarde.",
                    CodigoRespuesta = "ERR"
                };
            }
        }

        /// <summary>
        /// Verifica el estado de un pago (método de demostración)
        /// </summary>
        public async Task<ResultadoPago> VerificarPagoAsync(string idTransaccion)
        {
            try
            {
                _logger.LogInformation("Verificando estado de pago. IdTransaccion: {IdTransaccion}",
                    idTransaccion);

                // Simular consulta
                await Task.Delay(500);

                return new ResultadoPago
                {
                    EsExitoso = true,
                    Mensaje = "Pago confirmado",
                    IdTransaccion = idTransaccion,
                    CodigoRespuesta = "00",
                    FechaProcesamiento = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar pago. IdTransaccion: {IdTransaccion}",
                    idTransaccion);

                throw;
            }
        }

        /// <summary>
        /// Genera un ID de transacción único
        /// Formato: TXN-[timestamp]-[aleatorio]
        /// </summary>
        private string GenerarIdTransaccion()
        {
            var timestamp = DateTime.UtcNow.Ticks.ToString("X");
            var random = _random.Next(100000, 999999);
            return $"TXN-{timestamp}-{random}";
        }
    }
}
