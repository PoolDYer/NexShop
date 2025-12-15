using System.Net;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Resultado de una operación que contiene información sobre éxito o error
    /// </summary>
    public class ResultadoOperacion
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public string? CodigoError { get; set; }
        public object? Datos { get; set; }

        public static ResultadoOperacion Success(string mensaje = "Operación exitosa", object? datos = null)
        {
            return new ResultadoOperacion
            {
                Exito = true,
                Mensaje = mensaje,
                Datos = datos
            };
        }

        public static ResultadoOperacion Error(string mensaje, string? codigoError = null)
        {
            return new ResultadoOperacion
            {
                Exito = false,
                Mensaje = mensaje,
                CodigoError = codigoError ?? "ERROR_GENERAL"
            };
        }
    }

    /// <summary>
    /// Resultado tipado genérico de una operación
    /// </summary>
    public class ResultadoOperacion<T>
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public string? CodigoError { get; set; }
        public T? Datos { get; set; }

        public static ResultadoOperacion<T> Success(string mensaje = "Operación exitosa", T? datos = default)
        {
            return new ResultadoOperacion<T>
            {
                Exito = true,
                Mensaje = mensaje,
                Datos = datos
            };
        }

        public static ResultadoOperacion<T> Error(string mensaje, string? codigoError = null)
        {
            return new ResultadoOperacion<T>
            {
                Exito = false,
                Mensaje = mensaje,
                CodigoError = codigoError ?? "ERROR_GENERAL"
            };
        }
    }
}
