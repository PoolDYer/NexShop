using NexShop.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Servicio para generar imágenes virtuales dinámicas para productos
    /// Utiliza servicios externos como Picsum, Unsplash, o genera imágenes basadas en categorías
    /// </summary>
    public interface IImagenVirtualService
    {
        /// <summary>
        /// Obtiene URL de imagen virtual para un producto específico
        /// </summary>
        string ObtenerImagenVirtual(Producto producto, int width = 400, int height = 400);
        
        /// <summary>
        /// Obtiene múltiples imágenes virtuales para un producto
        /// </summary>
        List<string> ObtenerImagenesVirtuales(Producto producto, int cantidad = 3, int width = 400, int height = 400);
        
        /// <summary>
        /// Sincroniza imágenes virtuales con la base de datos
        /// </summary>
        Task SincronizarImagenesVirtualesAsync();
    }

    public class ImagenVirtualService : IImagenVirtualService
    {
        private readonly NexShopContext _context;
        private readonly ILogger<ImagenVirtualService> _logger;
        
        // ?? MAPEO EXHAUSTIVO - Cada producto a su término exacto
        private readonly Dictionary<string, string> _mapeoProductoExacto = new(StringComparer.OrdinalIgnoreCase)
        {
            // === ELECTRÓNICA (10 productos) ===
            { "Smartphone XYZ Pro", "smartphone" },
            { "Laptop Gaming 15", "gaming-laptop" },
            { "Tablet 10 pulgadas", "tablet" },
            { "Auriculares Bluetooth", "wireless-headphones" },
            { "Monitor 4K 27", "4k-monitor" },
            { "Teclado Mecánico RGB", "mechanical-keyboard" },
            { "Ratón Gaming Inalámbrico", "gaming-mouse" },
            { "Webcam 4K", "webcam" },
            { "Cargador Rápido 100W", "usb-charger" },
            { "Powerbank 30000mAh", "powerbank" },
            
            // === ROPA (10 productos) ===
            { "Camiseta Básica Blanca", "white-tshirt" },
            { "Jeans Azul Oscuro", "blue-jeans" },
            { "Sudadera con Capucha", "hoodie" },
            { "Chaqueta de Cuero", "leather-jacket" },
            { "Pantalones Deportivos", "sweatpants" },
            { "Calcetines Pack de 5", "socks" },
            { "Gorro de Lana", "beanie" },
            { "Bufanda Larga", "scarf" },
            { "Zapatos Deportivos", "running-shoes" },
            { "Cinturón de Cuero", "leather-belt" },
            
            // === HOGAR (10 productos) ===
            { "Juego de Sábanas 100% Algodón", "bed-sheets" },
            { "Almohada de Espuma", "pillow" },
            { "Edredón Nórdico", "comforter" },
            { "Cortinas Blackout", "curtains" },
            { "Lámpara de Escritorio LED", "desk-lamp" },
            { "Espejo de Pared Grande", "wall-mirror" },
            { "Alfombra Persa", "persian-rug" },
            { "Juego de Toallas 6 Piezas", "towels" },
            { "Tapete de Baño", "bath-mat" },
            { "Plantas Decorativas Artificiales", "artificial-plants" },
            
            // === DEPORTES (10 productos) ===
            { "Balón de Fútbol Profesional", "soccer-ball" },
            { "Raqueta de Tenis", "tennis-racket" },
            { "Pelota de Baloncesto", "basketball" },
            { "Mancuernas Ajustables", "dumbbells" },
            { "Colchoneta de Yoga", "yoga-mat" },
            { "Banda Elástica Resistencia", "resistance-band" },
            { "Botella de Agua 1L", "water-bottle" },
            { "Guantes de Boxeo", "boxing-gloves" },
            { "Cinta Métrica Flexible", "measuring-tape" },
            { "Uniforme Deportivo", "sports-uniform" },
            
            // === LIBROS (10 productos) ===
            { "El Quijote - Cervantes", "classic-books" },
            { "1984 - George Orwell", "dystopian-book" },
            { "Cien Años de Soledad - García Márquez", "literature-book" },
            { "Hábitos Atómicos - James Clear", "self-help-book" },
            { "El Alquimista - Paulo Coelho", "inspirational-book" },
            { "Sapiens - Yuval Noah Harari", "history-book" },
            { "El Poder del Ahora - Eckhart Tolle", "mindfulness-book" },
            { "La Revolución de los Creativos", "creativity-book" },
            { "Mindfulness para Principiantes", "meditation-book" },
            { "El Juego Infinito - Simon Sinek", "leadership-book" }
        };

        // ?? Mapeo de palabras clave a términos de búsqueda
        private readonly Dictionary<string, string> _palabrasClaveATermino = new(StringComparer.OrdinalIgnoreCase)
        {
            // Tecnología
            { "smartphone", "smartphone" },
            { "teléfono", "smartphone" },
            { "móvil", "smartphone" },
            { "laptop", "laptop" },
            { "portátil", "laptop" },
            { "computadora", "laptop" },
            { "tablet", "tablet" },
            { "auriculares", "headphones" },
            { "headphones", "headphones" },
            { "monitor", "monitor" },
            { "pantalla", "monitor" },
            { "teclado", "keyboard" },
            { "keyboard", "keyboard" },
            { "ratón", "mouse" },
            { "mouse", "mouse" },
            { "webcam", "webcam" },
            { "cámara", "camera" },
            { "cargador", "charger" },
            { "powerbank", "powerbank" },
            { "batería", "battery" },
            
            // Ropa
            { "camiseta", "t-shirt" },
            { "tshirt", "t-shirt" },
            { "jeans", "jeans" },
            { "vaquero", "jeans" },
            { "sudadera", "hoodie" },
            { "hoodie", "hoodie" },
            { "chaqueta", "jacket" },
            { "jacket", "jacket" },
            { "cuero", "leather" },
            { "pantalones", "pants" },
            { "pants", "pants" },
            { "calcetines", "socks" },
            { "socks", "socks" },
            { "gorro", "beanie" },
            { "beanie", "beanie" },
            { "bufanda", "scarf" },
            { "scarf", "scarf" },
            { "zapatos", "shoes" },
            { "shoes", "shoes" },
            { "deportivos", "sneakers" },
            { "cinturón", "belt" },
            { "belt", "belt" },
            
            // Hogar
            { "sábanas", "bedding" },
            { "sheets", "bedding" },
            { "almohada", "pillow" },
            { "pillow", "pillow" },
            { "edredón", "comforter" },
            { "comforter", "comforter" },
            { "cortinas", "curtains" },
            { "curtains", "curtains" },
            { "lámpara", "lamp" },
            { "lamp", "lamp" },
            { "espejo", "mirror" },
            { "mirror", "mirror" },
            { "alfombra", "rug" },
            { "rug", "rug" },
            { "toallas", "towels" },
            { "towels", "towels" },
            { "tapete", "mat" },
            { "mat", "mat" },
            { "plantas", "plants" },
            { "plants", "plants" },
            
            // Deportes
            { "balón", "ball" },
            { "ball", "ball" },
            { "fútbol", "soccer" },
            { "soccer", "soccer" },
            { "raqueta", "racket" },
            { "racket", "racket" },
            { "tenis", "tennis" },
            { "tennis", "tennis" },
            { "baloncesto", "basketball" },
            { "basketball", "basketball" },
            { "mancuernas", "dumbbells" },
            { "dumbbells", "dumbbells" },
            { "pesas", "weights" },
            { "weights", "weights" },
            { "yoga", "yoga" },
            { "colchoneta", "yoga-mat" },
            { "banda", "resistance-band" },
            { "resistencia", "resistance-band" },
            { "botella", "water-bottle" },
            { "agua", "water" },
            { "guantes", "gloves" },
            { "gloves", "gloves" },
            { "boxeo", "boxing" },
            { "boxing", "boxing" },
            
            // Libros
            { "libro", "book" },
            { "book", "book" },
            { "lectura", "reading" },
            { "reading", "reading" },
            { "novela", "novel" },
            { "novel", "novel" },
            { "quijote", "classic-literature" },
            { "1984", "dystopian" },
            { "garcía márquez", "literature" },
            { "hábitos", "habits" },
            { "alquimista", "inspiration" },
            { "sapiens", "history" },
            { "mindfulness", "meditation" },
            { "meditación", "meditation" },
            { "creatividad", "creativity" },
            { "liderazgo", "leadership" }
        };

        public ImagenVirtualService(NexShopContext context, ILogger<ImagenVirtualService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public string ObtenerImagenVirtual(Producto producto, int width = 400, int height = 400)
        {
            try
            {
                var terminoBusqueda = ObtenerTerminoBusquedaEspecifico(producto);
                
                if (!string.IsNullOrEmpty(terminoBusqueda))
                {
                    // Usar Unsplash con término específico
                    return $"https://source.unsplash.com/{width}x{height}/?{terminoBusqueda}&sig={producto.ProductoId}";
                }
                
                // Fallback: Picsum
                return $"https://picsum.photos/seed/{producto.ProductoId}/{width}/{height}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando imagen para ProductoId: {ProductoId}", producto.ProductoId);
                return $"https://via.placeholder.com/{width}x{height}/6c5ce7/ffffff?text=Producto";
            }
        }

        public List<string> ObtenerImagenesVirtuales(Producto producto, int cantidad = 3, int width = 400, int height = 400)
        {
            var imagenes = new List<string>();
            var termino = ObtenerTerminoBusquedaEspecifico(producto);
            
            try
            {
                for (int i = 0; i < cantidad; i++)
                {
                    if (!string.IsNullOrEmpty(termino))
                    {
                        var url = $"https://source.unsplash.com/{width}x{height}/?{termino}&sig={producto.ProductoId + i}";
                        imagenes.Add(url);
                    }
                    else
                    {
                        var seed = producto.ProductoId * 1000 + i;
                        imagenes.Add($"https://picsum.photos/seed/{seed}/{width}/{height}");
                    }
                }
                
                return imagenes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando imágenes para ProductoId: {ProductoId}", producto.ProductoId);
                
                // Fallback
                for (int i = 0; i < cantidad; i++)
                {
                    imagenes.Add($"https://via.placeholder.com/{width}x{height}/6c5ce7/ffffff?text=Imagen+{i + 1}");
                }
                
                return imagenes;
            }
        }

        /// <summary>
        /// ?? NUEVO: Obtiene el término más específico para un producto
        /// </summary>
        private string ObtenerTerminoBusquedaEspecifico(Producto producto)
        {
            var nombreLimpio = LimpiarNombre(producto.Nombre);
            
            // 1. Buscar coincidencia exacta por nombre
            if (_mapeoProductoExacto.TryGetValue(nombreLimpio, out var terminoExacto))
            {
                return terminoExacto;
            }
            
            // 2. Buscar por palabras clave en el nombre
            var nombreLower = producto.Nombre.ToLower();
            foreach (var kvp in _palabrasClaveATermino)
            {
                if (nombreLower.Contains(kvp.Key.ToLower()))
                {
                    return kvp.Value;
                }
            }
            
            // 3. Fallback por categoría
            if (producto.Categoria != null)
            {
                return producto.Categoria.Nombre.ToLower();
            }
            
            return string.Empty;
        }

        /// <summary>
        /// Limpia el nombre del producto para búsqueda
        /// </summary>
        private string LimpiarNombre(string nombre)
        {
            // Remover comillas y caracteres especiales
            return nombre.Replace("\"", "").Replace("®", "").Replace("™", "").Trim();
        }

        public async Task SincronizarImagenesVirtualesAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando sincronización de imágenes específicas...");
                
                var productosSinImagenes = await _context.Productos
                    .Include(p => p.Multimedia)
                    .Include(p => p.Categoria)
                    .Where(p => !p.Multimedia.Any())
                    .ToListAsync();

                _logger.LogInformation("Productos sin imágenes: {Cantidad}", productosSinImagenes.Count);

                int procesados = 0;

                foreach (var producto in productosSinImagenes)
                {
                    try
                    {
                        var imagenesUrls = ObtenerImagenesVirtuales(producto, 3, 400, 400);
                        var terminoBusqueda = ObtenerTerminoBusquedaEspecifico(producto);
                        
                        for (int i = 0; i < imagenesUrls.Count; i++)
                        {
                            var multimedia = new Multimedia
                            {
                                ProductoId = producto.ProductoId,
                                Tipo = "Foto",
                                Nombre = $"Imagen {i + 1} - {producto.Nombre}",
                                Url = imagenesUrls[i],
                                NombreArchivo = $"producto_{producto.ProductoId}_{i + 1}.jpg",
                                Descripcion = $"Imagen de {producto.Nombre}" + 
                                             (!string.IsNullOrEmpty(terminoBusqueda) ? $" ({terminoBusqueda})" : ""),
                                TamanoBytes = 0,
                                TipoMime = "image/jpeg",
                                Orden = i,
                                EsPrincipal = i == 0,
                                EstaActivo = true,
                                FechaCreacion = DateTime.UtcNow
                            };

                            _context.Multimedia.Add(multimedia);
                        }

                        procesados++;

                        if (procesados % 10 == 0)
                        {
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Procesados {Procesados} productos...", procesados);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error procesando ProductoId: {ProductoId}", producto.ProductoId);
                    }
                }

                if (procesados % 10 != 0)
                {
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Sincronización completada. Procesados: {Procesados}", procesados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante sincronización");
                throw;
            }
        }
    }
}