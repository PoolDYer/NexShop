using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexShop.Web.Models;
using NexShop.Web.Services;
using NexShop.Web.ViewModels;

namespace NexShop.Web.Controllers
{
    /// <summary>
    /// Controlador para gestionar operaciones CRUD de productos
    /// Acceso anónimo a Index y Details
    /// Acceso autorizado solo para vendedores en Create, Edit, Delete
    /// </summary>
    public class ProductosController : Controller
    {
        private readonly NexShopContext _context;
        private readonly ILogger<ProductosController> _logger;
        private readonly IMultimediaService _multimediaService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductosController(NexShopContext context, ILogger<ProductosController> logger, 
            IMultimediaService multimediaService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _logger = logger;
            _multimediaService = multimediaService;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Obtiene la lista de todos los productos - ACCESO ANÓNIMO
        /// GET: /Productos
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? categoriaId, string? busqueda, int pagina = 1)
        {
            try
            {
                const int productosPerPage = 12;
                var query = _context.Productos
                    .Include(p => p.Categoria)
                    .Include(p => p.Multimedia)
                    .Where(p => p.Categoria != null && p.Categoria.EstaActiva);

                // Filtrar por categoría si se proporciona
                if (categoriaId.HasValue && categoriaId > 0)
                {
                    query = query.Where(p => p.CategoriaId == categoriaId);
                }

                // Buscar por nombre o descripción
                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    query = query.Where(p =>
                        p.Nombre.Contains(busqueda) ||
                        p.Descripcion.Contains(busqueda) ||
                        p.SKU!.Contains(busqueda));
                }

                // Paginar resultados
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
                    ImagenPrincipal = p.Multimedia.OrderBy(m => m.Orden).FirstOrDefault()?.Url
                }).ToList();

                ViewBag.Categorias = await _context.Categorias
                    .Where(c => c.EstaActiva)
                    .Select(c => new CategoriaSelectViewModel
                    {
                        CategoriaId = c.CategoriaId,
                        Nombre = c.Nombre
                    })
                    .ToListAsync();

                ViewBag.Busqueda = busqueda;
                ViewBag.CategoriaId = categoriaId;
                ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalProductos / productosPerPage);
                ViewBag.PaginaActual = pagina;

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de productos");
                TempData["Error"] = "Error al cargar los productos. Intente nuevamente.";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Obtiene los detalles de un producto específico - ACCESO ANÓNIMO
        /// GET: /Productos/Details/5
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var producto = await _context.Productos
                    .Include(p => p.Categoria)
                    .Include(p => p.Vendedor)
                    .Include(p => p.Multimedia)
                    .FirstOrDefaultAsync(p => p.ProductoId == id);

                if (producto == null)
                {
                    return NotFound();
                }

                // Incrementar el contador de visualizaciones
                producto.NumeroVisualizaciones++;
                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();

                // Crear ViewModel sin incluir reseñas (por ahora)
                var viewModel = new ProductoDetailViewModel
                {
                    ProductoId = producto.ProductoId,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Precio = producto.Precio,
                    Stock = producto.Stock,
                    StockMinimo = producto.StockMinimo,
                    Estado = producto.Estado,
                    SKU = producto.SKU,
                    CategoriaId = producto.CategoriaId,
                    CategoriaNombre = producto.Categoria?.Nombre ?? "Sin categoría",
                    VendedorId = producto.VendedorId,
                    VendedorNombre = producto.Vendedor?.NombreCompleto ?? "Desconocido",
                    CalificacionPromedio = producto.CalificacionPromedio,
                    NumeroResenas = producto.NumeroResenas,
                    NumeroVisualizaciones = producto.NumeroVisualizaciones,
                    FechaCreacion = producto.FechaCreacion,
                    Multimedia = producto.Multimedia
                        .Where(m => m.EstaActivo)
                        .OrderBy(m => m.Orden)
                        .Select(m => new MultimediaListViewModel
                        {
                            MultimediaId = m.MultimediaId,
                            Tipo = m.Tipo,
                            Nombre = m.Nombre,
                            Url = m.Url,
                            Descripcion = m.Descripcion,
                            EsPrincipal = m.EsPrincipal,
                            Orden = m.Orden,
                            TamanoBytes = m.TamanoBytes
                        })
                        .ToList(),
                    Resenas = new List<ReseñaProductoDto>(),
                    EstadisticasResenas = new EstadisticasReseñasDto { TotalResenas = 0 },
                    UsuarioActualId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    UsuarioYaReseno = false
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles del producto con ID: {ProductoId}", id);
                TempData["Error"] = "Error al cargar los detalles del producto.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Muestra el formulario para crear un nuevo producto - SOLO VENDEDORES
        /// GET: /Productos/Create
        /// </summary>
        [Authorize(Roles = "Vendedor,Admin")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var viewModel = new ProductoCreateViewModel
                {
                    Categorias = await _context.Categorias
                        .Where(c => c.EstaActiva)
                        .Select(c => new CategoriaSelectViewModel
                        {
                            CategoriaId = c.CategoriaId,
                            Nombre = c.Nombre
                        })
                        .ToListAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mostrar el formulario de crear producto");
                TempData["Error"] = "Error al cargar el formulario.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Crea un nuevo producto - SOLO VENDEDORES
        /// POST: /Productos/Create
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor,Admin")]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Precio,Stock,StockMinimo,SKU,CategoriaId")] ProductoCreateViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    viewModel.Categorias = await _context.Categorias
                        .Where(c => c.EstaActiva)
                        .Select(c => new CategoriaSelectViewModel
                        {
                            CategoriaId = c.CategoriaId,
                            Nombre = c.Nombre
                        })
                        .ToListAsync();

                    return View(viewModel);
                }

                // Validar que la categoría existe
                var categoriaExists = await _context.Categorias
                    .AnyAsync(c => c.CategoriaId == viewModel.CategoriaId && c.EstaActiva);

                if (!categoriaExists)
                {
                    ModelState.AddModelError("CategoriaId", "La categoría seleccionada no es válida.");
                    viewModel.Categorias = await _context.Categorias
                        .Where(c => c.EstaActiva)
                        .Select(c => new CategoriaSelectViewModel
                        {
                            CategoriaId = c.CategoriaId,
                            Nombre = c.Nombre
                        })
                        .ToListAsync();

                    return View(viewModel);
                }

                var producto = new Producto
                {
                    Nombre = viewModel.Nombre,
                    Descripcion = viewModel.Descripcion,
                    Precio = viewModel.Precio,
                    Stock = viewModel.Stock,
                    StockMinimo = viewModel.StockMinimo,
                    SKU = viewModel.SKU,
                    CategoriaId = viewModel.CategoriaId,
                    VendedorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
                    Estado = "Disponible",
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Add(producto);
                await _context.SaveChangesAsync();

                // ?? Crear carpeta automáticamente para el nuevo producto
                try
                {
                    var carpetaPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "productos", $"{producto.ProductoId}_{viewModel.Nombre}");
                    if (!Directory.Exists(carpetaPath))
                    {
                        Directory.CreateDirectory(carpetaPath);
                        _logger.LogInformation("Carpeta creada automáticamente para nuevo producto. ProductoId: {ProductoId}, Ruta: {Ruta}", 
                            producto.ProductoId, carpetaPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear carpeta para producto. ProductoId: {ProductoId}", producto.ProductoId);
                    // Continuar aunque no se cree la carpeta
                }

                _logger.LogInformation("Producto creado con éxito. ProductoId: {ProductoId}", producto.ProductoId);
                TempData["Success"] = "Producto creado exitosamente.";

                return RedirectToAction(nameof(Details), new { id = producto.ProductoId });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al crear producto");
                TempData["Error"] = "Error al guardar el producto en la base de datos.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                TempData["Error"] = "Error al crear el producto.";
            }

            viewModel.Categorias = await _context.Categorias
                .Where(c => c.EstaActiva)
                .Select(c => new CategoriaSelectViewModel
                {
                    CategoriaId = c.CategoriaId,
                    Nombre = c.Nombre
                })
                .ToListAsync();

            return View(viewModel);
        }

        /// <summary>
        /// Muestra el formulario para editar un producto existente - SOLO PROPIETARIO O ADMIN
        /// GET: /Productos/Edit/5
        /// </summary>
        [Authorize(Roles = "Vendedor,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var producto = await _context.Productos.FindAsync(id);

                if (producto == null)
                {
                    return NotFound();
                }

                // Verificar que el usuario es el propietario del producto
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (producto.VendedorId != userId)
                {
                    return Forbid();
                }

                var viewModel = new ProductoEditViewModel
                {
                    ProductoId = producto.ProductoId,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Precio = producto.Precio,
                    Stock = producto.Stock,
                    StockMinimo = producto.StockMinimo,
                    Estado = producto.Estado,
                    SKU = producto.SKU,
                    CategoriaId = producto.CategoriaId,
                    Categorias = await _context.Categorias
                        .Where(c => c.EstaActiva)
                        .Select(c => new CategoriaSelectViewModel
                        {
                            CategoriaId = c.CategoriaId,
                            Nombre = c.Nombre
                        })
                        .ToListAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mostrar formulario de edición para ProductoId: {ProductoId}", id);
                TempData["Error"] = "Error al cargar el formulario de edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Actualiza un producto existente - SOLO PROPIETARIO O ADMIN
        /// POST: /Productos/Edit/5
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor,Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductoId,Nombre,Descripcion,Precio,Stock,StockMinimo,Estado,SKU,CategoriaId")] ProductoEditViewModel viewModel)
        {
            if (id != viewModel.ProductoId)
            {
                return NotFound();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    viewModel.Categorias = await _context.Categorias
                        .Where(c => c.EstaActiva)
                        .Select(c => new CategoriaSelectViewModel
                        {
                            CategoriaId = c.CategoriaId,
                            Nombre = c.Nombre
                        })
                        .ToListAsync();

                    return View(viewModel);
                }

                var producto = await _context.Productos.FindAsync(id);

                if (producto == null)
                {
                    return NotFound();
                }

                // Verificar autorización
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (producto.VendedorId != userId)
                {
                    return Forbid();
                }

                // Validar categoría
                var categoriaExists = await _context.Categorias
                    .AnyAsync(c => c.CategoriaId == viewModel.CategoriaId && c.EstaActiva);

                if (!categoriaExists)
                {
                    ModelState.AddModelError("CategoriaId", "La categoría seleccionada no es válida.");
                    viewModel.Categorias = await _context.Categorias
                        .Where(c => c.EstaActiva)
                        .Select(c => new CategoriaSelectViewModel
                        {
                            CategoriaId = c.CategoriaId,
                            Nombre = c.Nombre
                        })
                        .ToListAsync();

                    return View(viewModel);
                }

                // Actualizar propiedades
                producto.Nombre = viewModel.Nombre;
                producto.Descripcion = viewModel.Descripcion;
                producto.Precio = viewModel.Precio;
                producto.Stock = viewModel.Stock;
                producto.StockMinimo = viewModel.StockMinimo;
                producto.Estado = viewModel.Estado;
                producto.SKU = viewModel.SKU;
                producto.CategoriaId = viewModel.CategoriaId;
                producto.FechaActualizacion = DateTime.UtcNow;

                _context.Update(producto);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Producto actualizado con éxito. ProductoId: {ProductoId}", id);
                TempData["Success"] = "Producto actualizado exitosamente.";

                return RedirectToAction(nameof(Details), new { id = producto.ProductoId });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al actualizar ProductoId: {ProductoId}", id);
                TempData["Error"] = "Error al actualizar el producto en la base de datos.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar ProductoId: {ProductoId}", id);
                TempData["Error"] = "Error al actualizar el producto.";
            }

            viewModel.Categorias = await _context.Categorias
                .Where(c => c.EstaActiva)
                .Select(c => new CategoriaSelectViewModel
                {
                    CategoriaId = c.CategoriaId,
                    Nombre = c.Nombre
                })
                .ToListAsync();

            return View(viewModel);
        }

        /// <summary>
        /// Muestra la confirmación de eliminación de un producto - SOLO PROPIETARIO O ADMIN
        /// GET: /Productos/Delete/5
        /// </summary>
        [Authorize(Roles = "Vendedor,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var producto = await _context.Productos
                    .Include(p => p.Categoria)
                    .FirstOrDefaultAsync(p => p.ProductoId == id);

                if (producto == null)
                {
                    return NotFound();
                }

                // Verificar autorización
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (producto.VendedorId != userId)
                {
                    return Forbid();
                }

                var viewModel = new ProductoDetailViewModel
                {
                    ProductoId = producto.ProductoId,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Precio = producto.Precio,
                    Stock = producto.Stock,
                    Estado = producto.Estado,
                    CategoriaNombre = producto.Categoria?.Nombre ?? "Sin categoría"
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mostrar confirmación de eliminación para ProductoId: {ProductoId}", id);
                TempData["Error"] = "Error al cargar la confirmación de eliminación.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Elimina un producto confirmado - SOLO PROPIETARIO O ADMIN
        /// POST: /Productos/Delete/5
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor,Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);

                if (producto == null)
                {
                    return NotFound();
                }

                // Verificar autorización
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (producto.VendedorId != userId)
                {
                    return Forbid();
                }

                // ?? Eliminar carpeta del producto
                try
                {
                    var carpetaPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "productos", $"{id}_{producto.Nombre}");
                    if (Directory.Exists(carpetaPath))
                    {
                        Directory.Delete(carpetaPath, true);
                        _logger.LogInformation("Carpeta del producto eliminada. ProductoId: {ProductoId}, Ruta: {Ruta}", id, carpetaPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al eliminar carpeta del producto. ProductoId: {ProductoId}", id);
                    // Continuar aunque no se elimine la carpeta
                }

                // Eliminar multimedia asociada
                var multimedia = await _context.Multimedia
                    .Where(m => m.ProductoId == id)
                    .ToListAsync();
                
                _context.Multimedia.RemoveRange(multimedia);

                // Eliminar detalles de órdenes
                var detallesOrdenes = await _context.OrdenDetalles
                    .Where(od => od.ProductoId == id)
                    .ToListAsync();
                
                _context.OrdenDetalles.RemoveRange(detallesOrdenes);

                // Eliminar producto
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Producto eliminado con éxito. ProductoId: {ProductoId}", id);
                TempData["Success"] = "Producto eliminado exitosamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al eliminar ProductoId: {ProductoId}", id);
                TempData["Error"] = "No se puede eliminar el producto porque está asociado a órdenes.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ProductoId: {ProductoId}", id);
                TempData["Error"] = "Error al eliminar el producto.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Obtiene todos los productos para sincronización de carpetas (JSON)
        /// GET: /Productos/ObtenerTodos
        /// </summary>
        [AllowAnonymous]
        [Route("api/productos/todos")]
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var productos = await _context.Productos
                    .OrderBy(p => p.ProductoId)
                    .Select(p => new { p.ProductoId, p.Nombre })
                    .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos para sincronización");
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
