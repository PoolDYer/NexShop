using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexShop.Web.Models;
using NexShop.Web.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace NexShop.Web.Controllers
{
    /// <summary>
    /// ViewModel para edición de perfil de usuario
    /// </summary>
    public class EditarPerfilViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Nombre Completo")]
        [Required(ErrorMessage = "El nombre completo es requerido")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Dirección")]
        [StringLength(255, ErrorMessage = "La dirección no puede exceder 255 caracteres")]
        public string? Direccion { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        [Display(Name = "Tipo de Usuario")]
        public string TipoUsuario { get; set; } = string.Empty;
    }

    public class CambiarContraseñaViewModel
    {
        [Display(Name = "Contraseña Actual")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "La contraseña actual es requerida")]
        public string ContraseñaActual { get; set; } = string.Empty;

        [Display(Name = "Nueva Contraseña")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener mínimo 6 caracteres")]
        public string NuevaContraseña { get; set; } = string.Empty;

        [Display(Name = "Confirmar Contraseña")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [Compare("NuevaContraseña", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContraseña { get; set; } = string.Empty;
    }

    /// <summary>
    /// Controlador para gestionar el perfil y datos del usuario
    /// </summary>
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly NexShopContext _context;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, 
            NexShopContext context, ILogger<UsuariosController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Muestra el perfil del usuario autenticado
        /// GET: /Usuarios/Perfil
        /// </summary>
        public async Task<IActionResult> Perfil()
        {
            try
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var viewModel = new EditarPerfilViewModel
                {
                    Id = usuario.Id,
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email ?? string.Empty,
                    PhoneNumber = usuario.PhoneNumber,
                    Direccion = usuario.Direccion,
                    Descripcion = usuario.Descripcion,
                    TipoUsuario = usuario.TipoUsuario
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener perfil del usuario");
                TempData["Error"] = "Error al cargar el perfil.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Muestra el formulario para editar el perfil
        /// GET: /Usuarios/Editar
        /// </summary>
        public async Task<IActionResult> Editar()
        {
            try
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var viewModel = new EditarPerfilViewModel
                {
                    Id = usuario.Id,
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email ?? string.Empty,
                    PhoneNumber = usuario.PhoneNumber,
                    Direccion = usuario.Direccion,
                    Descripcion = usuario.Descripcion,
                    TipoUsuario = usuario.TipoUsuario
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mostrar formulario de edición");
                TempData["Error"] = "Error al cargar el formulario.";
                return RedirectToAction(nameof(Perfil));
            }
        }

        /// <summary>
        /// Actualiza el perfil del usuario
        /// POST: /Usuarios/Editar
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([Bind("NombreCompleto,PhoneNumber,Direccion,Descripcion")] EditarPerfilViewModel viewModel)
        {
            try
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (!ModelState.IsValid)
                {
                    viewModel.Email = usuario.Email ?? string.Empty;
                    viewModel.TipoUsuario = usuario.TipoUsuario;
                    return View(viewModel);
                }

                usuario.NombreCompleto = viewModel.NombreCompleto;
                usuario.PhoneNumber = viewModel.PhoneNumber;
                usuario.Direccion = viewModel.Direccion;
                usuario.Descripcion = viewModel.Descripcion;
                usuario.FechaActualizacion = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(usuario);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Perfil actualizado para usuario: {UserId}", usuario.Id);
                    TempData["Success"] = "Perfil actualizado exitosamente.";
                    return RedirectToAction(nameof(Perfil));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar perfil");
                TempData["Error"] = "Error al actualizar el perfil.";
            }

            var usr = await _userManager.GetUserAsync(User);
            viewModel.Email = usr?.Email ?? string.Empty;
            viewModel.TipoUsuario = usr?.TipoUsuario ?? string.Empty;
            return View(viewModel);
        }

        /// <summary>
        /// Muestra el formulario para cambiar contraseña
        /// GET: /Usuarios/CambiarContraseña
        /// </summary>
        public IActionResult CambiarContraseña()
        {
            return View(new CambiarContraseñaViewModel());
        }

        /// <summary>
        /// Procesa el cambio de contraseña
        /// POST: /Usuarios/CambiarContraseña
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarContraseña(CambiarContraseñaViewModel viewModel)
        {
            try
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                var result = await _userManager.ChangePasswordAsync(usuario, 
                    viewModel.ContraseñaActual, viewModel.NuevaContraseña);

                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(usuario);
                    _logger.LogInformation("Contraseña cambiada para usuario: {UserId}", usuario.Id);
                    TempData["Success"] = "Contraseña cambiada exitosamente.";
                    return RedirectToAction(nameof(Perfil));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña");
                TempData["Error"] = "Error al cambiar la contraseña.";
            }

            return View(viewModel);
        }

        /// <summary>
        /// Obtiene los productos vendidos por el usuario (si es vendedor) - SOLO VENDEDORES
        /// GET: /Usuarios/MisProductos
        /// </summary>
        [Authorize(Roles = "Vendedor,Admin")]
        public async Task<IActionResult> MisProductos(int pagina = 1)
        {
            try
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null || usuario.TipoUsuario != "Vendedor")
                {
                    return Forbid();
                }

                const int productosPerPage = 12;
                var query = _context.Productos
                    .Include(p => p.Categoria)
                    .Include(p => p.Multimedia)
                    .Where(p => p.VendedorId == usuario.Id);

                var totalProductos = await query.CountAsync();
                var productos = await query
                    .OrderByDescending(p => p.FechaCreacion)
                    .Skip((pagina - 1) * productosPerPage)
                    .Take(productosPerPage)
                    .ToListAsync();

                var viewModels = productos.Select(p => new ProductoListViewModel
                {
                    ProductoId = p.ProductoId,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    Estado = p.Estado,
                    CategoriaNombre = p.Categoria?.Nombre ?? "Sin categoría",
                    CalificacionPromedio = p.CalificacionPromedio,
                    NumeroResenas = p.NumeroResenas,
                    NumeroVisualizaciones = p.NumeroVisualizaciones,
                    ImagenPrincipal = p.Multimedia.FirstOrDefault(m => m.EsPrincipal)?.Url
                }).ToList();

                ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalProductos / productosPerPage);
                ViewBag.PaginaActual = pagina;
                ViewBag.UsuarioId = usuario.Id;

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos del vendedor");
                TempData["Error"] = "Error al cargar los productos.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Obtiene las estadísticas de venta del vendedor - SOLO VENDEDORES
        /// GET: /Usuarios/Estadisticas
        /// </summary>
        [Authorize(Roles = "Vendedor,Admin")]
        public async Task<IActionResult> Estadisticas()
        {
            try
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null || usuario.TipoUsuario != "Vendedor")
                {
                    return Forbid();
                }

                // Obtener todos los productos del vendedor
                var productosVendedor = await _context.Productos
                    .Where(p => p.VendedorId == usuario.Id)
                    .ToListAsync();

                // Obtener todas las órdenes relacionadas con los productos del vendedor
                var ordenesRelacionadas = await _context.OrdenDetalles
                    .Include(od => od.Orden)
                    .Where(od => productosVendedor.Select(p => p.ProductoId).Contains(od.ProductoId))
                    .ToListAsync();

                // Calcular métricas principales
                var totalVentas = ordenesRelacionadas.Sum(od => od.Subtotal);
                var totalOrdenes = ordenesRelacionadas.GroupBy(od => od.OrdenId).Count();
                var totalUnidades = ordenesRelacionadas.Sum(od => od.Cantidad);
                var ticketPromedio = totalOrdenes > 0 ? totalVentas / totalOrdenes : 0;
                var calificacionPromedio = productosVendedor.Any() 
                    ? productosVendedor.Average(p => p.CalificacionPromedio ?? 0m)
                    : 0m;

                // Estadísticas de este mes
                var ahora = DateTime.UtcNow;
                var primerDiaDelMes = new DateTime(ahora.Year, ahora.Month, 1);
                var ventasEsteMes = ordenesRelacionadas
                    .Where(od => od.Orden != null && od.Orden.FechaCreacion >= primerDiaDelMes)
                    .ToList();

                // Productos más vendidos
                var productosMasVendidos = ordenesRelacionadas
                    .GroupBy(od => od.ProductoId)
                    .Select(g => new
                    {
                        ProductoId = g.Key,
                        UnidadesVendidas = g.Sum(od => od.Cantidad),
                        VentasTotal = g.Sum(od => od.Subtotal)
                    })
                    .OrderByDescending(x => x.UnidadesVendidas)
                    .Take(5)
                    .ToList();

                // Mapear productos más vendidos con detalles
                var productosMasVendidosDetalles = new List<ProductoTopDto>();
                int posicion = 1;
                foreach (var item in productosMasVendidos)
                {
                    var producto = productosVendedor.FirstOrDefault(p => p.ProductoId == item.ProductoId);
                    if (producto != null)
                    {
                        productosMasVendidosDetalles.Add(new ProductoTopDto
                        {
                            Posicion = posicion++,
                            Nombre = producto.Nombre,
                            Valor = item.UnidadesVendidas,
                            Metrica = "unidades"
                        });
                    }
                }

                // Productos mejor calificados
                var productosMejorCalificados = productosVendedor
                    .Where(p => p.CalificacionPromedio.HasValue && p.CalificacionPromedio.Value > 0)
                    .OrderByDescending(p => p.CalificacionPromedio)
                    .Take(5)
                    .Select((p, index) => new ProductoTopDto
                    {
                        Posicion = index + 1,
                        Nombre = p.Nombre,
                        Valor = (decimal)(p.CalificacionPromedio ?? 0),
                        Metrica = "estrellas"
                    })
                    .ToList();

                // Productos más visualizados
                var productosMasVisualizados = productosVendedor
                    .OrderByDescending(p => p.NumeroVisualizaciones)
                    .Take(5)
                    .Select((p, index) => new ProductoTopDto
                    {
                        Posicion = index + 1,
                        Nombre = p.Nombre,
                        Valor = p.NumeroVisualizaciones,
                        Metrica = "vistas"
                    })
                    .ToList();

                // Calcular primeras y últimas ventas
                var primerVenta = ordenesRelacionadas
                    .OrderBy(od => od.Orden?.FechaCreacion)
                    .FirstOrDefault()?.Orden?.FechaCreacion ?? DateTime.UtcNow;
                var ultimaVenta = ordenesRelacionadas
                    .OrderByDescending(od => od.Orden?.FechaCreacion)
                    .FirstOrDefault()?.Orden?.FechaCreacion ?? DateTime.UtcNow;

                // Días como comerciante
                var diasComerciante = (int)(DateTime.UtcNow - usuario.FechaCreacion).TotalDays;
                var ventasPromedioPorDia = diasComerciante > 0 ? (double)totalVentas / diasComerciante : 0;

                // Clientes únicos
                var clientesUnicos = ordenesRelacionadas
                    .Select(od => od.Orden?.CompradorId)
                    .Distinct()
                    .Count();

                // Tasa de conversión promedio
                var visitasTotales = productosVendedor.Sum(p => p.NumeroVisualizaciones);
                var tasaConversionPromedio = visitasTotales > 0 
                    ? (double)totalUnidades / visitasTotales * 100 
                    : 0;

                // Crear ViewModel
                var viewModel = new EstadisticasVendedorViewModel
                {
                    VendedorId = usuario.Id,
                    VendedorNombre = usuario.NombreCompleto,
                    VendedorEmail = usuario.Email,
                    TotalProductos = productosVendedor.Count,
                    ProductosActivos = productosVendedor.Count(p => p.Estado == "Disponible"),
                    ProductosAgotados = productosVendedor.Count(p => p.Stock == 0 || p.Estado == "Agotado"),
                    TotalVentas = totalVentas,
                    TotalOrdenes = totalOrdenes,
                    TotalUnidadesVendidas = totalUnidades,
                    TicketPromedio = ticketPromedio,
                    CalificacionPromedio = (double)calificacionPromedio,
                    VisitasTotales = visitasTotales,
                    TasaConversionPromedio = tasaConversionPromedio,
                    VisitasEsteMes = productosVendedor
                        .Sum(p => p.NumeroVisualizaciones), // Se podría mejorar con fecha
                    VentasEsteMes = ventasEsteMes.Count,
                    VentasEsteMesMonto = ventasEsteMes.Sum(od => od.Subtotal),
                    PrimerVenta = primerVenta,
                    UltimaVenta = ultimaVenta,
                    DiasComoComerciante = diasComerciante,
                    VentasPromedioPorDia = ventasPromedioPorDia,
                    ResenasTotal = productosVendedor.Sum(p => p.NumeroResenas),
                    IngresoPromedioPorProducto = productosVendedor.Count > 0 
                        ? totalVentas / productosVendedor.Count 
                        : 0,
                    StockTotalValor = productosVendedor.Sum(p => (decimal)p.Stock * p.Precio),
                    ClientesUnicos = clientesUnicos,
                    Top5MasVendidos = productosMasVendidosDetalles,
                    Top5MejorCalificados = productosMejorCalificados,
                    ProductosMasVendidos = new List<ProductoEstadisticaDto>()
                };

                _logger.LogInformation("Estadísticas generadas para vendedor {VendedorId}", usuario.Id);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de vendedor");
                TempData["Error"] = "Error al cargar las estadísticas.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Obtiene un resumen del dashboard del usuario - TODOS LOS USUARIOS AUTENTICADOS
        /// GET: /Usuarios/Dashboard
        /// </summary>
        [Authorize(Roles = "Comprador,Vendedor,Admin")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                dynamic viewModel;

                if (usuario.TipoUsuario == "Vendedor")
                {
                    var productosVendedor = await _context.Productos
                        .Where(p => p.VendedorId == usuario.Id)
                        .ToListAsync();

                    var ordenesRelacionadas = await _context.OrdenDetalles
                        .Include(od => od.Orden)
                        .Where(od => productosVendedor.Select(p => p.ProductoId).Contains(od.ProductoId))
                        .ToListAsync();

                    viewModel = new
                    {
                        Tipo = "Vendedor",
                        Usuario = usuario,
                        TotalProductos = productosVendedor.Count,
                        TotalVentas = ordenesRelacionadas.Sum(od => od.Subtotal),
                        ProductosActivos = productosVendedor.Count(p => p.Estado == "Disponible")
                    };
                }
                else
                {
                    var ordenes = await _context.Ordenes
                        .Where(o => o.CompradorId == usuario.Id)
                        .CountAsync();

                    var gastosTotal = await _context.Ordenes
                        .Where(o => o.CompradorId == usuario.Id)
                        .SumAsync(o => o.MontoTotal);

                    viewModel = new
                    {
                        Tipo = "Comprador",
                        Usuario = usuario,
                        TotalOrdenes = ordenes,
                        GastosTotal = gastosTotal
                    };
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener dashboard");
                TempData["Error"] = "Error al cargar el dashboard.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Obtiene las ordenes del usuario comprador - SOLO COMPRADORES
        /// GET: /Usuarios/MisOrdenes
        /// </summary>
        [Authorize(Roles = "Comprador,Admin")]
        public async Task<IActionResult> MisOrdenes(int pagina = 1)
        {
            try
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                const int ordenesPerPage = 10;
                
                // Query mejorada con includes
                var query = _context.Ordenes
                    .Include(o => o.Detalles)
                    .Where(o => o.CompradorId == usuario.Id)
                    .AsNoTracking(); // No trackear para mejor performance en lectura

                var totalOrdenes = await query.CountAsync();
                var ordenes = await query
                    .OrderByDescending(o => o.FechaCreacion)
                    .Skip((pagina - 1) * ordenesPerPage)
                    .Take(ordenesPerPage)
                    .ToListAsync();

                if (!ordenes.Any() && pagina > 1)
                {
                    // Si no hay órdenes en la página solicitada, ir a la última
                    var ultimaPagina = (int)Math.Ceiling((double)totalOrdenes / ordenesPerPage);
                    return RedirectToAction("MisOrdenes", new { pagina = ultimaPagina });
                }

                ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalOrdenes / ordenesPerPage);
                ViewBag.PaginaActual = pagina;
                ViewBag.TotalOrdenes = totalOrdenes;

                _logger.LogInformation("Usuario {UserId} accedió a sus órdenes (página {Pagina})", usuario.Id, pagina);
                return View(ordenes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ordenes del usuario");
                TempData["Error"] = "Error al cargar las órdenes. Por favor, intenta nuevamente.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Muestra la pagina de configuracion del usuario
        /// GET: /Usuarios/Configuracion
        /// </summary>
        public async Task<IActionResult> Configuracion()
        {
            try
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var viewModel = new EditarPerfilViewModel
                {
                    Id = usuario.Id,
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email ?? string.Empty,
                    PhoneNumber = usuario.PhoneNumber,
                    Direccion = usuario.Direccion,
                    Descripcion = usuario.Descripcion,
                    TipoUsuario = usuario.TipoUsuario
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar configuracion");
                TempData["Error"] = "Error al cargar la configuracion.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Actualiza la configuracion del usuario
        /// POST: /Usuarios/Configuracion
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Configuracion([Bind("Id,NombreCompleto,PhoneNumber,Direccion,Descripcion")] EditarPerfilViewModel viewModel)
        {
            try
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Validar que el ID del modelo coincida con el usuario autenticado
                if (viewModel.Id != usuario.Id)
                {
                    ModelState.AddModelError(string.Empty, "Acceso no autorizado");
                    return Forbid();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Validación fallida para configuración del usuario {UserId}", usuario.Id);
                    viewModel.Email = usuario.Email ?? string.Empty;
                    viewModel.TipoUsuario = usuario.TipoUsuario;
                    return View(viewModel);
                }

                usuario.NombreCompleto = viewModel.NombreCompleto;
                usuario.PhoneNumber = viewModel.PhoneNumber;
                usuario.Direccion = viewModel.Direccion;
                usuario.Descripcion = viewModel.Descripcion;
                usuario.FechaActualizacion = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(usuario);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Configuracion actualizada para usuario: {UserId}", usuario.Id);
                    TempData["Success"] = "Tu configuración ha sido actualizada exitosamente.";
                    return RedirectToAction(nameof(Configuracion));
                }
                else
                {
                    _logger.LogError("Error al actualizar configuración del usuario {UserId}", usuario.Id);
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        _logger.LogError("Error de Identity: {Error}", error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar configuracion del usuario");
                TempData["Error"] = "Error al actualizar la configuración. Por favor, intenta nuevamente.";
            }

            var usr = await _userManager.GetUserAsync(User);
            viewModel.Email = usr?.Email ?? string.Empty;
            viewModel.TipoUsuario = usr?.TipoUsuario ?? string.Empty;
            viewModel.Id = usr?.Id ?? string.Empty;
            return View(viewModel);
        }

        /// <summary>
        /// Obtiene el detalle completo de una orden específica
        /// GET: /Usuarios/DetallesOrden/{id}
        /// </summary>
        [Authorize(Roles = "Comprador,Admin")]
        public async Task<IActionResult> DetallesOrden(int id)
        {
            try
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var orden = await _context.Ordenes
                    .Include(o => o.Detalles)
                    .FirstOrDefaultAsync(o => o.OrdenId == id && o.CompradorId == usuario.Id);

                if (orden == null)
                {
                    _logger.LogWarning("Intento de acceso no autorizado a orden {OrdenId} por usuario {UserId}", id, usuario.Id);
                    return NotFound();
                }

                _logger.LogInformation("Usuario {UserId} accedió a orden {OrdenId}", usuario.Id, id);
                return View(orden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de la orden {OrdenId}", id);
                TempData["Error"] = "Error al cargar los detalles de la orden.";
                return RedirectToAction("MisOrdenes");
            }
        }
    }
}
