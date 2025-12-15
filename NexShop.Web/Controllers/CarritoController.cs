using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexShop.Web.Models;
using NexShop.Web.ViewModels;
using System.Text.Json;

namespace NexShop.Web.Controllers
{
    /// <summary>
    /// Controlador para gestionar el carrito de compras del usuario
    /// El carrito se almacena en sesión para usuarios no autenticados
    /// Acceso anónimo al carrito (permite compras sin login)
    /// </summary>
    public class CarritoController : Controller
    {
        private readonly NexShopContext _context;
        private readonly ILogger<CarritoController> _logger;
        private const string CARRITO_SESSION_KEY = "Carrito";

        public CarritoController(NexShopContext context, ILogger<CarritoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el carrito actual de la sesión
        /// </summary>
        private List<CarritoItemViewModel> ObtenerCarrito()
        {
            var sesion = HttpContext.Session.GetString(CARRITO_SESSION_KEY);
            
            if (string.IsNullOrEmpty(sesion))
            {
                return new List<CarritoItemViewModel>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<CarritoItemViewModel>>(sesion) ?? new List<CarritoItemViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al deserializar carrito de sesión");
                return new List<CarritoItemViewModel>();
            }
        }

        /// <summary>
        /// Guarda el carrito en la sesión
        /// </summary>
        private void GuardarCarrito(List<CarritoItemViewModel> carrito)
        {
            try
            {
                HttpContext.Session.SetString(CARRITO_SESSION_KEY, JsonSerializer.Serialize(carrito));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al serializar carrito a sesión");
            }
        }

        /// <summary>
        /// Muestra el carrito de compras - ACCESO ANÓNIMO PERMITIDO
        /// GET: /Carrito/Index
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            try
            {
                var carrito = ObtenerCarrito();

                // Actualizar información de stock disponible
                foreach (var item in carrito)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);
                    if (producto != null)
                    {
                        item.StockDisponible = producto.Stock;
                        item.Precio = producto.Precio;
                    }
                }

                var viewModel = new CarritoViewModel
                {
                    Articulos = carrito
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el carrito");
                TempData["Error"] = "Error al cargar el carrito.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Agrega un producto al carrito - ACCESO ANÓNIMO PERMITIDO
        /// POST: /Carrito/Agregar
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Agregar(int productoId, int cantidad = 1)
        {
            try
            {
                if (cantidad < 1)
                {
                    return BadRequest("La cantidad debe ser mayor a 0");
                }

                var producto = await _context.Productos
                    .Include(p => p.Multimedia)
                    .FirstOrDefaultAsync(p => p.ProductoId == productoId);

                if (producto == null)
                {
                    return NotFound("El producto no existe");
                }

                if (producto.Stock < cantidad || producto.Estado != "Disponible")
                {
                    TempData["Error"] = "El producto no tiene stock suficiente o no está disponible.";
                    return RedirectToAction("Details", "Productos", new { id = productoId });
                }

                var carrito = ObtenerCarrito();

                // Verificar si el producto ya está en el carrito
                var itemExistente = carrito.FirstOrDefault(i => i.ProductoId == productoId);

                if (itemExistente != null)
                {
                    // Validar que no se exceda el stock
                    if (itemExistente.Cantidad + cantidad > producto.Stock)
                    {
                        TempData["Error"] = $"No hay suficiente stock. Disponibles: {producto.Stock}";
                        return RedirectToAction("Details", "Productos", new { id = productoId });
                    }

                    itemExistente.Cantidad += cantidad;
                }
                else
                {
                    // Agregar nuevo artículo al carrito
                    carrito.Add(new CarritoItemViewModel
                    {
                        ProductoId = productoId,
                        ProductoNombre = producto.Nombre,
                        Precio = producto.Precio,
                        Cantidad = cantidad,
                        ImagenUrl = producto.Multimedia.FirstOrDefault(m => m.EsPrincipal)?.Url,
                        StockDisponible = producto.Stock
                    });
                }

                GuardarCarrito(carrito);

                _logger.LogInformation("Producto agregado al carrito. ProductoId: {ProductoId}, Cantidad: {Cantidad}", 
                    productoId, cantidad);

                TempData["Success"] = $"{producto.Nombre} agregado al carrito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar producto al carrito. ProductoId: {ProductoId}", productoId);
                TempData["Error"] = "Error al agregar el producto al carrito.";
                return RedirectToAction("Details", "Productos", new { id = productoId });
            }
        }

        /// <summary>
        /// Actualiza la cantidad de un artículo - ACCESO ANÓNIMO PERMITIDO
        /// POST: /Carrito/ActualizarCantidad
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ActualizarCantidad(int productoId, int cantidad)
        {
            try
            {
                if (cantidad < 1)
                {
                    return BadRequest("La cantidad debe ser mayor a 0");
                }

                var producto = await _context.Productos.FindAsync(productoId);

                if (producto == null)
                {
                    return NotFound("El producto no existe");
                }

                if (cantidad > producto.Stock)
                {
                    TempData["Error"] = $"No hay suficiente stock. Disponibles: {producto.Stock}";
                    return RedirectToAction(nameof(Index));
                }

                var carrito = ObtenerCarrito();
                var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);

                if (item != null)
                {
                    item.Cantidad = cantidad;
                    GuardarCarrito(carrito);

                    _logger.LogInformation("Cantidad actualizada en carrito. ProductoId: {ProductoId}, Nueva cantidad: {Cantidad}", 
                        productoId, cantidad);

                    TempData["Success"] = "Cantidad actualizada.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cantidad en carrito. ProductoId: {ProductoId}", productoId);
                TempData["Error"] = "Error al actualizar la cantidad.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Elimina un producto del carrito - ACCESO ANÓNIMO PERMITIDO
        /// POST: /Carrito/Eliminar
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult Eliminar(int productoId)
        {
            try
            {
                var carrito = ObtenerCarrito();
                var item = carrito.FirstOrDefault(i => i.ProductoId == productoId);

                if (item != null)
                {
                    carrito.Remove(item);
                    GuardarCarrito(carrito);

                    _logger.LogInformation("Producto eliminado del carrito. ProductoId: {ProductoId}", productoId);

                    TempData["Success"] = "Producto eliminado del carrito.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto del carrito. ProductoId: {ProductoId}", productoId);
                TempData["Error"] = "Error al eliminar el producto del carrito.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Vacía el carrito completamente - ACCESO ANÓNIMO PERMITIDO
        /// POST: /Carrito/Vaciar
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult Vaciar()
        {
            try
            {
                HttpContext.Session.Remove(CARRITO_SESSION_KEY);

                _logger.LogInformation("Carrito vaciado");

                TempData["Success"] = "Carrito vaciado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al vaciar el carrito");
                TempData["Error"] = "Error al vaciar el carrito.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Obtiene el número de artículos en el carrito - ACCESO ANÓNIMO PERMITIDO
        /// GET: /Carrito/ObtenerCantidad
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ObtenerCantidad()
        {
            try
            {
                var carrito = ObtenerCarrito();
                var totalArticulos = carrito.Sum(a => a.Cantidad);
                return Json(new { totalArticulos });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cantidad de artículos en carrito");
                return Json(new { totalArticulos = 0 });
            }
        }
    }
}
