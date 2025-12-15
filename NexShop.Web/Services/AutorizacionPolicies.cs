using Microsoft.AspNetCore.Authorization;
using NexShop.Web.Models;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Política personalizada para verificar si un usuario es el propietario de un producto
    /// </summary>
    public class VendedorPropietarioRequirement : IAuthorizationRequirement
    {
    }

    /// <summary>
    /// Manejador de la política VendedorPropietario
    /// </summary>
    public class VendedorPropietarioHandler : AuthorizationHandler<VendedorPropietarioRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NexShopContext _context;
        private readonly ILogger<VendedorPropietarioHandler> _logger;

        public VendedorPropietarioHandler(IHttpContextAccessor httpContextAccessor, NexShopContext context,
            ILogger<VendedorPropietarioHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            VendedorPropietarioRequirement requirement)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return;
                }

                // Obtener el ID del usuario actual
                var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return;
                }

                // Obtener el ID del producto desde la ruta
                var routeData = httpContext.GetRouteData();
                if (routeData == null || !routeData.Values.TryGetValue("id", out var productIdObj))
                {
                    return;
                }

                if (!int.TryParse(productIdObj?.ToString(), out var productoId))
                {
                    return;
                }

                // Verificar si el usuario es propietario del producto
                var producto = await _context.Productos.FindAsync(productoId);

                if (producto != null && producto.VendedorId == userId)
                {
                    context.Succeed(requirement);
                    _logger.LogInformation(
                        "Autorización exitosa - Usuario {UserId} es propietario del producto {ProductoId}",
                        userId, productoId);
                }
                else
                {
                    _logger.LogWarning(
                        "Autorización denegada - Usuario {UserId} no es propietario del producto {ProductoId}",
                        userId, productoId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en VendedorPropietarioHandler");
            }
        }
    }

    /// <summary>
    /// Política para verificar si el usuario es Admin o Vendedor propietario
    /// </summary>
    public class AdminOVendedorPropietarioRequirement : IAuthorizationRequirement
    {
    }

    /// <summary>
    /// Manejador para Admin o Vendedor propietario
    /// </summary>
    public class AdminOVendedorPropietarioHandler : AuthorizationHandler<AdminOVendedorPropietarioRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NexShopContext _context;
        private readonly ILogger<AdminOVendedorPropietarioHandler> _logger;

        public AdminOVendedorPropietarioHandler(IHttpContextAccessor httpContextAccessor, NexShopContext context,
            ILogger<AdminOVendedorPropietarioHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AdminOVendedorPropietarioRequirement requirement)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return;
                }

                var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return;
                }

                // Si es Admin, permitir
                if (context.User.IsInRole(RolesNexShop.Admin))
                {
                    context.Succeed(requirement);
                    _logger.LogInformation("Autorización exitosa - Usuario {UserId} es Admin", userId);
                    return;
                }

                // Si no es vendedor, denegar
                if (!context.User.IsInRole(RolesNexShop.Vendedor))
                {
                    return;
                }

                // Si es vendedor, verificar que sea propietario
                var routeData = httpContext.GetRouteData();
                if (routeData == null || !routeData.Values.TryGetValue("id", out var productIdObj))
                {
                    return;
                }

                if (!int.TryParse(productIdObj?.ToString(), out var productoId))
                {
                    return;
                }

                var producto = await _context.Productos.FindAsync(productoId);

                if (producto != null && producto.VendedorId == userId)
                {
                    context.Succeed(requirement);
                    _logger.LogInformation(
                        "Autorización exitosa - Vendedor {UserId} es propietario del producto {ProductoId}",
                        userId, productoId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en AdminOVendedorPropietarioHandler");
            }
        }
    }

    /// <summary>
    /// Extensión para registrar las políticas personalizadas
    /// </summary>
    public static class AutorizacionPoliciesExtension
    {
        public static IServiceCollection AddAutorizacionPolicies(this IServiceCollection services)
        {
            services.AddAuthorizationBuilder()
                .AddPolicy("VendedorPropietario", policy =>
                {
                    policy.Requirements.Add(new VendedorPropietarioRequirement());
                })
                .AddPolicy("AdminOVendedorPropietario", policy =>
                {
                    policy.Requirements.Add(new AdminOVendedorPropietarioRequirement());
                });

            services.AddScoped<IAuthorizationHandler, VendedorPropietarioHandler>();
            services.AddScoped<IAuthorizationHandler, AdminOVendedorPropietarioHandler>();

            return services;
        }
    }
}
