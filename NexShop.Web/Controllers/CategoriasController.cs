using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexShop.Web.Models;
using NexShop.Web.Services;
using NexShop.Web.ViewModels;

namespace NexShop.Web.Controllers
{
    /// <summary>
    /// Controlador para gestionar categorías de productos
    /// SOLO ADMINISTRADORES pueden ver, crear, editar y eliminar categorías
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class CategoriasController : Controller
    {
        private readonly NexShopContext _context;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(NexShopContext context, ILogger<CategoriasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene la lista de todas las categorías - SOLO ADMIN
        /// GET: /Categorias
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var categorias = await _context.Categorias
                    .OrderBy(c => c.Nombre)
                    .ToListAsync();

                var viewModels = categorias.Select(c => new CategoriaListViewModel
                {
                    CategoriaId = c.CategoriaId,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    IconoUrl = c.IconoUrl,
                    EstaActiva = c.EstaActiva
                }).ToList();

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lista de categorías");
                TempData["Error"] = "Error al cargar las categorías";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Muestra el formulario para crear una nueva categoría - SOLO ADMIN
        /// GET: /Categorias/Create
        /// </summary>
        public IActionResult Create()
        {
            return View(new CategoriaEditViewModel());
        }

        /// <summary>
        /// Crea una nueva categoría - SOLO ADMIN
        /// POST: /Categorias/Create
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaEditViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                // Verificar que el nombre no esté duplicado
                var exists = await _context.Categorias
                    .AnyAsync(c => c.Nombre.ToLower() == viewModel.Nombre.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("Nombre", "Ya existe una categoría con este nombre");
                    return View(viewModel);
                }

                var categoria = new Categoria
                {
                    Nombre = viewModel.Nombre,
                    Descripcion = viewModel.Descripcion,
                    IconoUrl = viewModel.IconoUrl,
                    EstaActiva = viewModel.EstaActiva,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Categoría creada por admin. CategoriaId: {CategoriaId}, Nombre: {Nombre}",
                    categoria.CategoriaId, categoria.Nombre);

                TempData["Success"] = "Categoría creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear categoría");
                TempData["Error"] = "Error al crear la categoría";
                return View(viewModel);
            }
        }

        /// <summary>
        /// Muestra el formulario para editar una categoría - SOLO ADMIN
        /// GET: /Categorias/Edit/5
        /// </summary>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var categoria = await _context.Categorias.FindAsync(id);

                if (categoria == null)
                {
                    return NotFound();
                }

                var viewModel = new CategoriaEditViewModel
                {
                    CategoriaId = categoria.CategoriaId,
                    Nombre = categoria.Nombre,
                    Descripcion = categoria.Descripcion,
                    IconoUrl = categoria.IconoUrl,
                    EstaActiva = categoria.EstaActiva
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categoría para editar. CategoriaId: {CategoriaId}", id);
                TempData["Error"] = "Error al cargar la categoría";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Actualiza una categoría - SOLO ADMIN
        /// POST: /Categorias/Edit/5
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoriaEditViewModel viewModel)
        {
            if (id != viewModel.CategoriaId)
            {
                return NotFound();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                var categoria = await _context.Categorias.FindAsync(id);

                if (categoria == null)
                {
                    return NotFound();
                }

                // Verificar nombre duplicado (excluir la categoría actual)
                var exists = await _context.Categorias
                    .AnyAsync(c => c.Nombre.ToLower() == viewModel.Nombre.ToLower()
                        && c.CategoriaId != id);

                if (exists)
                {
                    ModelState.AddModelError("Nombre", "Ya existe otra categoría con este nombre");
                    return View(viewModel);
                }

                categoria.Nombre = viewModel.Nombre;
                categoria.Descripcion = viewModel.Descripcion;
                categoria.IconoUrl = viewModel.IconoUrl;
                categoria.EstaActiva = viewModel.EstaActiva;
                categoria.FechaActualizacion = DateTime.UtcNow;

                _context.Categorias.Update(categoria);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Categoría actualizada por admin. CategoriaId: {CategoriaId}", id);

                TempData["Success"] = "Categoría actualizada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar categoría. CategoriaId: {CategoriaId}", id);
                TempData["Error"] = "Error al actualizar la categoría";
                return View(viewModel);
            }
        }

        /// <summary>
        /// Muestra la confirmación de eliminación - SOLO ADMIN
        /// GET: /Categorias/Delete/5
        /// </summary>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var categoria = await _context.Categorias
                    .Include(c => c.Productos)
                    .FirstOrDefaultAsync(c => c.CategoriaId == id);

                if (categoria == null)
                {
                    return NotFound();
                }

                var viewModel = new CategoriaListViewModel
                {
                    CategoriaId = categoria.CategoriaId,
                    Nombre = categoria.Nombre,
                    Descripcion = categoria.Descripcion,
                    EstaActiva = categoria.EstaActiva
                };

                ViewBag.TotalProductos = categoria.Productos.Count;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mostrar confirmación de eliminación. CategoriaId: {CategoriaId}", id);
                TempData["Error"] = "Error al cargar la confirmación";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Elimina una categoría - SOLO ADMIN
        /// POST: /Categorias/Delete/5
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);

                if (categoria == null)
                {
                    return NotFound();
                }

                // Verificar que no tenga productos
                var tieneProductos = await _context.Productos
                    .AnyAsync(p => p.CategoriaId == id);

                if (tieneProductos)
                {
                    TempData["Error"] = "No se puede eliminar una categoría que tiene productos asociados";
                    return RedirectToAction(nameof(Index));
                }

                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Categoría eliminada por admin. CategoriaId: {CategoriaId}", id);

                TempData["Success"] = "Categoría eliminada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar categoría. CategoriaId: {CategoriaId}", id);
                TempData["Error"] = "Error al eliminar la categoría";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Activa o desactiva una categoría - SOLO ADMIN (AJAX)
        /// POST: /Categorias/ToggleActiva/5
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleActiva(int id)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);

                if (categoria == null)
                {
                    return NotFound();
                }

                categoria.EstaActiva = !categoria.EstaActiva;
                categoria.FechaActualizacion = DateTime.UtcNow;

                _context.Categorias.Update(categoria);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Estado de categoría toggled. CategoriaId: {CategoriaId}, Activa: {EstaActiva}",
                    id, categoria.EstaActiva);

                return Json(new { success = true, estaActiva = categoria.EstaActiva });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de categoría. CategoriaId: {CategoriaId}", id);
                return Json(new { success = false, message = "Error al cambiar el estado" });
            }
        }
    }
}
