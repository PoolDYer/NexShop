using NexShop.Web.Models;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Interfaz para manejar operaciones de carga y administración de archivos multimedia
    /// </summary>
    public interface IMultimediaService
    {
        /// <summary>
        /// Carga un archivo multimedia individual
        /// </summary>
        /// <param name="archivo">Archivo a cargar</param>
        /// <param name="productoId">ID del producto asociado</param>
        /// <param name="esPrincipal">Indica si es la imagen principal</param>
        /// <returns>Resultado de la carga</returns>
        Task<ResultadoCargaArchivo> CargarArchivoAsync(IFormFile archivo, int productoId, bool esPrincipal = false);

        /// <summary>
        /// Carga múltiples archivos multimedia simultáneamente
        /// </summary>
        /// <param name="archivos">Lista de archivos a cargar</param>
        /// <param name="productoId">ID del producto asociado</param>
        /// <returns>Resultado detallado de la carga múltiple</returns>
        Task<ResultadoCargaMultiple> CargarArchivosMultiplesAsync(List<IFormFile> archivos, int productoId);

        /// <summary>
        /// Valida un archivo antes de cargarlo
        /// </summary>
        /// <param name="archivo">Archivo a validar</param>
        /// <returns>Resultado de validación</returns>
        ResultadoOperacion ValidarArchivo(IFormFile archivo);

        /// <summary>
        /// Obtiene el tipo de multimedia basado en el tipo MIME
        /// </summary>
        /// <param name="tipoMime">Tipo MIME del archivo</param>
        /// <returns>Tipo de multimedia ("Foto", "Video", etc.)</returns>
        string ObtenerTipoMultimedia(string tipoMime);

        /// <summary>
        /// Elimina un archivo multimedia
        /// </summary>
        /// <param name="multimediaId">ID del multimedia a eliminar</param>
        /// <returns>Resultado de la eliminación</returns>
        Task<ResultadoOperacion> EliminarMultimediaAsync(int multimediaId);

        /// <summary>
        /// Elimina todos los archivos de un producto
        /// </summary>
        /// <param name="productoId">ID del producto</param>
        /// <returns>Resultado de la operación</returns>
        Task<ResultadoOperacion> EliminarMultimediaProductoAsync(int productoId);

        /// <summary>
        /// Reemplaza un archivo multimedia existente
        /// </summary>
        /// <param name="multimediaId">ID del multimedia a reemplazar</param>
        /// <param name="archivoNuevo">Nuevo archivo</param>
        /// <returns>Resultado del reemplazo</returns>
        Task<ResultadoCargaArchivo> ReemplazarMultimediaAsync(int multimediaId, IFormFile archivoNuevo);

        /// <summary>
        /// Obtiene todos los archivos multimedia de un producto
        /// </summary>
        /// <param name="productoId">ID del producto</param>
        /// <returns>Lista de multimedia</returns>
        Task<List<Multimedia>> ObtenerMultimediaProductoAsync(int productoId);

        /// <summary>
        /// Establece un archivo como imagen principal
        /// </summary>
        /// <param name="multimediaId">ID del multimedia</param>
        /// <param name="productoId">ID del producto</param>
        /// <returns>Resultado de la operación</returns>
        Task<ResultadoOperacion> EstablecerComoPrincipalAsync(int multimediaId, int productoId);

        /// <summary>
        /// Actualiza el orden de visualización de archivos
        /// </summary>
        /// <param name="actualizaciones">Diccionario de MultimediaId -> Nuevo Orden</param>
        /// <returns>Resultado de la operación</returns>
        Task<ResultadoOperacion> ActualizarOrdenMultimediaAsync(Dictionary<int, int> actualizaciones);
    }
}
