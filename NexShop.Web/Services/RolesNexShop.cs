namespace NexShop.Web.Services
{
    /// <summary>
    /// Clase con constantes para los roles de la aplicación
    /// Centraliza los nombres de roles para evitar errores de tipeo
    /// </summary>
    public static class RolesNexShop
    {
        /// <summary>
        /// Rol de administrador - acceso completo a todas las funciones
        /// </summary>
        public const string Admin = "Admin";

        /// <summary>
        /// Rol de vendedor - puede crear y gestionar productos
        /// </summary>
        public const string Vendedor = "Vendedor";

        /// <summary>
        /// Rol de comprador - puede buscar y comprar productos
        /// </summary>
        public const string Comprador = "Comprador";

        /// <summary>
        /// Array con todos los roles disponibles
        /// </summary>
        public static readonly string[] TodosLosRoles = { Admin, Vendedor, Comprador };

        /// <summary>
        /// Verifica si un rol es válido
        /// </summary>
        public static bool EsRolValido(string rol)
        {
            return TodosLosRoles.Contains(rol);
        }

        /// <summary>
        /// Obtiene descripción legible del rol
        /// </summary>
        public static string ObtenerDescripcion(string rol)
        {
            return rol switch
            {
                Admin => "Administrador - Acceso completo",
                Vendedor => "Vendedor - Crear y gestionar productos",
                Comprador => "Comprador - Búsqueda y compra",
                _ => "Rol desconocido"
            };
        }
    }

    /// <summary>
    /// Clase con constantes para permisos específicos
    /// Facilita la creación de políticas de autorización granulares
    /// </summary>
    public static class PermisosNexShop
    {
        /// <summary>
        /// Permiso para crear productos - solo vendedores
        /// </summary>
        public const string CrearProducto = "CrearProducto";

        /// <summary>
        /// Permiso para editar productos - solo dueño o admin
        /// </summary>
        public const string EditarProducto = "EditarProducto";

        /// <summary>
        /// Permiso para eliminar productos - solo dueño o admin
        /// </summary>
        public const string EliminarProducto = "EliminarProducto";

        /// <summary>
        /// Permiso para gestionar categorías - solo admin
        /// </summary>
        public const string GestionarCategorias = "GestionarCategorias";

        /// <summary>
        /// Permiso para ver estadísticas - solo admin y vendedor
        /// </summary>
        public const string VerEstadisticas = "VerEstadisticas";

        /// <summary>
        /// Permiso para gestionar usuarios - solo admin
        /// </summary>
        public const string GestionarUsuarios = "GestionarUsuarios";

        /// <summary>
        /// Permiso para cargar multimedia - solo vendedor
        /// </summary>
        public const string CargarMultimedia = "CargarMultimedia";
    }
}
