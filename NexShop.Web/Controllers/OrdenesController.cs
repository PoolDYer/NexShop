using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexShop.Web.Models;
using NexShop.Web.Services;
using NexShop.Web.ViewModels;
using System.Text.Json;

namespace NexShop.Web.Controllers
{
    /// <summary>
    /// Controlador para gestionar las órdenes de compra y proceso de checkout
    /// SOLO USUARIOS AUTENTICADOS pueden ver sus órdenes y hacer checkout
    /// </summary>
    [Authorize(Roles = "Comprador,Vendedor,Admin")]
    public class OrdenesController : Controller
    {
        private readonly NexShopContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<OrdenesController> _logger;
        private readonly IPagoService _pagoService;
        private readonly IStockService _stockService;
        private const string CARRITO_SESSION_KEY = "Carrito";

        public OrdenesController(NexShopContext context, UserManager<Usuario> userManager, 
            ILogger<OrdenesController> logger, IPagoService pagoService, IStockService stockService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _pagoService = pagoService;
            _stockService = stockService;
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
                _logger.LogError(ex, "Error al deserializar carrito");
                return new List<CarritoItemViewModel>();
            }
        }

        /// <summary>
        /// Muestra las órdenes del usuario autenticado
        /// GET: /Ordenes/MisOrdenes
        /// </summary>
        public async Task<IActionResult> MisOrdenes(int pagina = 1)
        {
            try
            {
                const int ordenesPerPage = 10;
                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var query = _context.Ordenes
                    .Where(o => o.CompradorId == userId)
                    .OrderByDescending(o => o.FechaCreacion);

                var totalOrdenes = await query.CountAsync();
                var ordenes = await query
                    .Skip((pagina - 1) * ordenesPerPage)
                    .Take(ordenesPerPage)
                    .ToListAsync();

                var viewModels = ordenes.Select(o => new OrdenListViewModel
                {
                    OrdenId = o.OrdenId,
                    NumeroOrden = o.NumeroOrden,
                    FechaCreacion = o.FechaCreacion,
                    MontoTotal = o.MontoTotal,
                    Estado = o.Estado,
                    TotalArticulos = _context.OrdenDetalles
                        .Where(od => od.OrdenId == o.OrdenId)
                        .Sum(od => od.Cantidad)
                }).ToList();

                ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalOrdenes / ordenesPerPage);
                ViewBag.PaginaActual = pagina;

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener órdenes del usuario");
                TempData["Error"] = "Error al cargar las órdenes.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Muestra los detalles de una orden específica
        /// GET: /Ordenes/Detalles/5
        /// </summary>
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var userId = _userManager.GetUserId(User);
                var orden = await _context.Ordenes
                    .Include(o => o.Detalles)
                    .ThenInclude(od => od.Producto)
                    .FirstOrDefaultAsync(o => o.OrdenId == id);

                if (orden == null)
                {
                    return NotFound();
                }

                // Verificar que el usuario es el propietario de la orden
                if (orden.CompradorId != userId)
                {
                    return Forbid();
                }

                var viewModel = new OrdenDetailViewModel
                {
                    OrdenId = orden.OrdenId,
                    NumeroOrden = orden.NumeroOrden,
                    Estado = orden.Estado,
                    FechaCreacion = orden.FechaCreacion,
                    FechaConfirmacion = orden.FechaConfirmacion,
                    FechaEnvio = orden.FechaEnvio,
                    FechaEntrega = orden.FechaEntrega,
                    DireccionEntrega = orden.DireccionEntrega,
                    MetodoPago = orden.MetodoPago,
                    Notas = orden.Notas,
                    Subtotal = orden.MontoTotal - orden.Impuesto - orden.MontoEnvio + orden.Descuento,
                    Impuesto = orden.Impuesto,
                    MontoEnvio = orden.MontoEnvio,
                    Descuento = orden.Descuento,
                    MontoTotal = orden.MontoTotal,
                    Detalles = orden.Detalles.Select(od => new OrdenDetalleListViewModel
                    {
                        OrdenDetalleId = od.OrdenDetalleId,
                        ProductoNombre = od.Producto?.Nombre ?? "Producto eliminado",
                        PrecioUnitario = od.PrecioUnitario,
                        Cantidad = od.Cantidad,
                        Subtotal = od.Subtotal
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de orden. OrdenId: {OrdenId}", id);
                TempData["Error"] = "Error al cargar los detalles de la orden.";
                return RedirectToAction(nameof(MisOrdenes));
            }
        }

        /// <summary>
        /// Muestra el formulario de checkout (confirmación de compra)
        /// GET: /Ordenes/Checkout
        /// </summary>
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var carrito = ObtenerCarrito();

                if (!carrito.Any())
                {
                    TempData["Error"] = "El carrito está vacío.";
                    return RedirectToAction("Index", "Carrito");
                }

                // Validar que todos los productos siguen disponibles
                foreach (var item in carrito)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);
                    if (producto == null || producto.Stock < item.Cantidad || producto.Estado != "Disponible")
                    {
                        TempData["Error"] = "Uno o más productos no están disponibles. Verifique su carrito.";
                        return RedirectToAction("Index", "Carrito");
                    }
                }

                var usuario = await _userManager.GetUserAsync(User);
                
                var subtotal = carrito.Sum(a => a.Subtotal);
                var impuesto = Math.Round(subtotal * 0.16m, 2);
                var envio = 0m; // Aquí podría calcularse según lógica de envío
                var total = subtotal + impuesto + envio;

                var viewModel = new CheckoutViewModel
                {
                    DireccionEntrega = usuario?.Direccion ?? string.Empty,
                    MetodoPago = "Tarjeta de Crédito",
                    Subtotal = subtotal,
                    Impuesto = impuesto,
                    MontoEnvio = envio,
                    Total = total,
                    ArticulosCarrito = carrito
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mostrar formulario de checkout");
                TempData["Error"] = "Error al cargar el formulario de checkout.";
                return RedirectToAction("Index", "Carrito");
            }
        }

        /// <summary>
        /// Procesa la creación de una orden desde el checkout
        /// POST: /Ordenes/Checkout
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var carrito = ObtenerCarrito();
                    viewModel.ArticulosCarrito = carrito;
                    viewModel.Subtotal = carrito.Sum(a => a.Subtotal);
                    viewModel.Impuesto = Math.Round(viewModel.Subtotal * 0.16m, 2);
                    viewModel.Total = viewModel.Subtotal + viewModel.Impuesto + viewModel.MontoEnvio;
                    return View(viewModel);
                }

                if (!viewModel.AceptaTerminos)
                {
                    ModelState.AddModelError("AceptaTerminos", "Debe aceptar los términos y condiciones.");
                    var carrito = ObtenerCarrito();
                    viewModel.ArticulosCarrito = carrito;
                    return View(viewModel);
                }

                var carritoActual = ObtenerCarrito();

                if (!carritoActual.Any())
                {
                    TempData["Error"] = "El carrito está vacío.";
                    return RedirectToAction("Index", "Carrito");
                }

                var userId = _userManager.GetUserId(User);

                // Iniciar transacción para garantizar consistencia
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Crear la orden
                        var orden = new Orden
                        {
                            NumeroOrden = GenerarNumeroOrden(),
                            CompradorId = userId ?? string.Empty,
                            DireccionEntrega = viewModel.DireccionEntrega,
                            MetodoPago = viewModel.MetodoPago,
                            Notas = viewModel.Notas,
                            Estado = "Pendiente",
                            FechaCreacion = DateTime.UtcNow
                        };

                        // Agregar detalles de la orden y validar stock
                        decimal subtotal = 0;

                        foreach (var item in carritoActual)
                        {
                            var producto = await _context.Productos.FindAsync(item.ProductoId);

                            if (producto == null || producto.Stock < item.Cantidad)
                            {
                                await transaction.RollbackAsync();
                                TempData["Error"] = $"Stock insuficiente para {item.ProductoNombre}.";
                                return RedirectToAction("Index", "Carrito");
                            }

                            var detalle = new OrdenDetalle
                            {
                                ProductoId = item.ProductoId,
                                Cantidad = item.Cantidad,
                                PrecioUnitario = item.Precio,
                                Subtotal = item.Subtotal
                            };

                            orden.Detalles.Add(detalle);
                            subtotal += detalle.Subtotal;
                        }

                        // Calcular montos
                        orden.Impuesto = Math.Round(subtotal * 0.16m, 2);
                        orden.MontoEnvio = viewModel.MontoEnvio;
                        orden.Descuento = viewModel.MontoEnvio > 0 ? 0 : 0;
                        orden.MontoTotal = subtotal + orden.Impuesto + orden.MontoEnvio - orden.Descuento;

                        _context.Ordenes.Add(orden);
                        await _context.SaveChangesAsync();

                        // PROCESAR PAGO SIMULADO
                        _logger.LogInformation("Iniciando procesamiento de pago simulado para la orden {NumeroOrden}", orden.NumeroOrden);
                        var resultadoPago = await _pagoService.ProcesarPagoAsync(orden, viewModel.MetodoPago);

                        if (!resultadoPago.EsExitoso)
                        {
                            // Pago rechazado - revertir cambios
                            await transaction.RollbackAsync();
                            TempData["Error"] = resultadoPago.Mensaje;
                            _logger.LogWarning("Pago rechazado para la orden {NumeroOrden}", orden.NumeroOrden);
                            return RedirectToAction("Index", "Carrito");
                        }

                        // PAGO EXITOSO - REDUCIR STOCK USANDO SERVICIO
                        _logger.LogInformation("Pago exitoso. Reduciendo stock para la orden {NumeroOrden}", orden.NumeroOrden);

                        foreach (var item in carritoActual)
                        {
                            var producto = await _context.Productos.FindAsync(item.ProductoId);

                            if (producto != null)
                            {
                                // Usar servicio de stock para reducir
                                var resultadoStock = await _stockService.ReducirStockAsync(producto, item.Cantidad);

                                if (!resultadoStock.EsExitoso)
                                {
                                    _logger.LogError("Error al reducir stock: {Mensaje}", resultadoStock.Mensaje);
                                    await transaction.RollbackAsync();
                                    TempData["Error"] = $"Error al procesar el stock de {item.ProductoNombre}";
                                    return RedirectToAction("Index", "Carrito");
                                }

                                _context.Productos.Update(producto);

                                _logger.LogInformation("Stock reducido exitosamente. Producto: {Nombre}, Anterior: {Anterior}, Nuevo: {Nuevo}",
                                    producto.Nombre, resultadoStock.StockAnterior, resultadoStock.StockNuevo);
                            }
                        }

                        // Confirmar orden con detalles de pago
                        orden.Estado = "Confirmada";
                        orden.FechaConfirmacion = DateTime.UtcNow;
                        _context.Ordenes.Update(orden);

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Orden creada y procesada exitosamente. OrdenId: {OrdenId}, NumeroOrden: {NumeroOrden}, MontoTotal: {MontoTotal}", 
                            orden.OrdenId, orden.NumeroOrden, orden.MontoTotal);

                        // Limpiar carrito
                        HttpContext.Session.Remove(CARRITO_SESSION_KEY);

                        TempData["Success"] = "Tu pago ha sido procesado exitosamente.";
                        return RedirectToAction(nameof(Confirmacion), new { id = orden.OrdenId });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Error durante la transacción de checkout");
                        TempData["Error"] = "Error al procesar la orden. Intente nuevamente.";
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar checkout");
                TempData["Error"] = "Error al crear la orden. Intente nuevamente.";
                return RedirectToAction("Index", "Carrito");
            }
        }

        /// <summary>
        /// Muestra la confirmación de la orden creada
        /// GET: /Ordenes/Confirmacion/5
        /// </summary>
        public async Task<IActionResult> Confirmacion(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var userId = _userManager.GetUserId(User);
                var orden = await _context.Ordenes
                    .Include(o => o.Detalles)
                    .FirstOrDefaultAsync(o => o.OrdenId == id);

                if (orden == null)
                {
                    return NotFound();
                }

                // Verificar que el usuario es el propietario
                if (orden.CompradorId != userId)
                {
                    return Forbid();
                }

                var viewModel = new ConfirmacionOrdenViewModel
                {
                    OrdenId = orden.OrdenId,
                    NumeroOrden = orden.NumeroOrden,
                    FechaCreacion = orden.FechaCreacion,
                    MontoTotal = orden.MontoTotal,
                    DireccionEntrega = orden.DireccionEntrega,
                    Mensaje = "Su orden ha sido creada exitosamente. Recibirá una confirmación por correo electrónico.",
                    Detalles = orden.Detalles.Select(od => new OrdenDetalleListViewModel
                    {
                        OrdenDetalleId = od.OrdenDetalleId,
                        ProductoNombre = "Producto",
                        PrecioUnitario = od.PrecioUnitario,
                        Cantidad = od.Cantidad,
                        Subtotal = od.Subtotal
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mostrar confirmación de orden. OrdenId: {OrdenId}", id);
                TempData["Error"] = "Error al cargar la confirmación.";
                return RedirectToAction(nameof(MisOrdenes));
            }
        }

        /// <summary>
        /// Cancela una orden si aún está en estado "Pendiente"
        /// POST: /Ordenes/Cancelar/5
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var orden = await _context.Ordenes
                    .Include(o => o.Detalles)
                    .FirstOrDefaultAsync(o => o.OrdenId == id);

                if (orden == null)
                {
                    return NotFound();
                }

                // Verificar autorización
                if (orden.CompradorId != userId)
                {
                    return Forbid();
                }

                // Solo se pueden cancelar órdenes pendientes
                if (orden.Estado != "Pendiente")
                {
                    TempData["Error"] = "Solo se pueden cancelar órdenes en estado Pendiente.";
                    return RedirectToAction(nameof(Detalles), new { id });
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Revertir stock de todos los productos
                        foreach (var detalle in orden.Detalles)
                        {
                            var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                            if (producto != null)
                            {
                                producto.Stock += detalle.Cantidad;

                                if (producto.Stock > producto.StockMinimo && producto.Estado == "Agotado")
                                {
                                    producto.Estado = "Disponible";
                                }

                                _context.Productos.Update(producto);
                            }
                        }

                        // Cambiar estado de la orden
                        orden.Estado = "Cancelada";
                        orden.FechaCancelacion = DateTime.UtcNow;
                        _context.Ordenes.Update(orden);

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Orden cancelada. OrdenId: {OrdenId}", id);
                        TempData["Success"] = "Orden cancelada exitosamente.";

                        return RedirectToAction(nameof(MisOrdenes));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar orden. OrdenId: {OrdenId}", id);
                TempData["Error"] = "Error al cancelar la orden.";
                return RedirectToAction(nameof(Detalles), new { id });
            }
        }

        /// <summary>
        /// Genera un número de orden único
        /// </summary>
        private string GenerarNumeroOrden()
        {
            var timestamp = DateTime.UtcNow.Ticks;
            var random = new Random().Next(1000, 9999);
            return $"ORD-{timestamp:X8}-{random}";
        }
    }
}
