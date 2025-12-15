using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NexShop.Web.Models;
using System.Web;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Servicio para inicializar roles y datos por defecto en la base de datos
    /// Se ejecuta al iniciar la aplicación para garantizar que los roles existan
    /// </summary>
    public interface ISeederService
    {
        /// <summary>
        /// Inicializa los roles y datos de prueba
        /// </summary>
        Task InitializeAsync();
    }

    public class SeederService : ISeederService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly NexShopContext _context;
        private readonly ILogger<SeederService> _logger;

        public SeederService(RoleManager<IdentityRole> roleManager, UserManager<Usuario> userManager,
            NexShopContext context, ILogger<SeederService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Inicializa roles y datos por defecto
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                // Inicializar roles
                await InitializarRolesAsync();

                // Inicializar categorías
                await InitializarCategoriasAsync();

                // Inicializar usuarios de prueba
                await InitializarUsuariosAsync();

                // Inicializar productos (10 por categoría) con imágenes (2 por producto)
                await InitializarProductosAsync();

                _logger.LogInformation("Inicialización de datos completada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la inicialización de datos");
                throw;
            }
        }

        /// <summary>
        /// Crea los roles si no existen
        /// </summary>
        private async Task InitializarRolesAsync()
        {
            var rolesACrear = new[]
            {
                RolesNexShop.Admin,
                RolesNexShop.Vendedor,
                RolesNexShop.Comprador
            };

            foreach (var roleName in rolesACrear)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Rol creado: {RoleName}", roleName);
                    }
                    else
                    {
                        _logger.LogWarning("Error al crear rol {RoleName}: {Errors}",
                            roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    _logger.LogDebug("Rol ya existe: {RoleName}", roleName);
                }
            }
        }

        /// <summary>
        /// Crea categorías por defecto si no existen
        /// </summary>
        private async Task InitializarCategoriasAsync()
        {
            if (await _context.Categorias.AnyAsync())
            {
                _logger.LogDebug("Las categorías ya existen");
                return;
            }

            var categorias = new[]
            {
                new Categoria
                {
                    Nombre = "Electrónica",
                    Descripcion = "Productos electrónicos y dispositivos tecnológicos",
                    EstaActiva = true,
                    FechaCreacion = DateTime.UtcNow
                },
                new Categoria
                {
                    Nombre = "Ropa",
                    Descripcion = "Prendas de vestir y accesorios",
                    EstaActiva = true,
                    FechaCreacion = DateTime.UtcNow
                },
                new Categoria
                {
                    Nombre = "Hogar",
                    Descripcion = "Artículos para el hogar y decoración",
                    EstaActiva = true,
                    FechaCreacion = DateTime.UtcNow
                },
                new Categoria
                {
                    Nombre = "Deportes",
                    Descripcion = "Equipos y artículos deportivos",
                    EstaActiva = true,
                    FechaCreacion = DateTime.UtcNow
                },
                new Categoria
                {
                    Nombre = "Libros",
                    Descripcion = "Libros y material de lectura",
                    EstaActiva = true,
                    FechaCreacion = DateTime.UtcNow
                }
            };

            _context.Categorias.AddRange(categorias);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Se crearon {Count} categorías por defecto", categorias.Length);
        }

        /// <summary>
        /// Crea usuarios de prueba con diferentes roles
        /// </summary>
        private async Task InitializarUsuariosAsync()
        {
            // Usuario Administrador
            var adminEmail = "admin@nexshop.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new Usuario
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    NombreCompleto = "Administrador NexShop",
                    TipoUsuario = RolesNexShop.Admin,
                    EstaActivo = true,
                    FechaCreacion = DateTime.UtcNow
                };

                var resultAdmin = await _userManager.CreateAsync(adminUser, "Admin@123456");

                if (resultAdmin.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, RolesNexShop.Admin);
                    _logger.LogInformation("Usuario administrador creado: {Email}", adminEmail);
                }
                else
                {
                    _logger.LogWarning("Error al crear usuario administrador: {Errors}",
                        string.Join(", ", resultAdmin.Errors.Select(e => e.Description)));
                }
            }

            // Usuario Vendedor
            var vendedorEmail = "vendedor@nexshop.com";
            var vendedorUser = await _userManager.FindByEmailAsync(vendedorEmail);

            if (vendedorUser == null)
            {
                vendedorUser = new Usuario
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "vendedor",
                    Email = vendedorEmail,
                    EmailConfirmed = true,
                    NombreCompleto = "Vendedor Ejemplo",
                    TipoUsuario = RolesNexShop.Vendedor,
                    PhoneNumber = "+34912345678",
                    Descripcion = "Vendedor de productos de calidad",
                    EstaActivo = true,
                    FechaCreacion = DateTime.UtcNow
                };

                var resultVendedor = await _userManager.CreateAsync(vendedorUser, "Vendedor@123456");

                if (resultVendedor.Succeeded)
                {
                    await _userManager.AddToRoleAsync(vendedorUser, RolesNexShop.Vendedor);
                    _logger.LogInformation("Usuario vendedor creado: {Email}", vendedorEmail);
                }
                else
                {
                    _logger.LogWarning("Error al crear usuario vendedor: {Errors}",
                        string.Join(", ", resultVendedor.Errors.Select(e => e.Description)));
                }
            }

            // Usuario Comprador
            var compradorEmail = "comprador@nexshop.com";
            var compradorUser = await _userManager.FindByEmailAsync(compradorEmail);

            if (compradorUser == null)
            {
                compradorUser = new Usuario
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "comprador",
                    Email = compradorEmail,
                    EmailConfirmed = true,
                    NombreCompleto = "Comprador Ejemplo",
                    TipoUsuario = RolesNexShop.Comprador,
                    PhoneNumber = "+34987654321",
                    Direccion = "Calle Principal 123, Madrid",
                    EstaActivo = true,
                    FechaCreacion = DateTime.UtcNow
                };

                var resultComprador = await _userManager.CreateAsync(compradorUser, "Comprador@123456");

                if (resultComprador.Succeeded)
                {
                    await _userManager.AddToRoleAsync(compradorUser, RolesNexShop.Comprador);
                    _logger.LogInformation("Usuario comprador creado: {Email}", compradorEmail);
                }
                else
                {
                    _logger.LogWarning("Error al crear usuario comprador: {Errors}",
                        string.Join(", ", resultComprador.Errors.Select(e => e.Description)));
                }
            }
        }

        /// <summary>
        /// Genera URLs de imágenes placeholder con formato correcto
        /// </summary>
        private List<string> GenerarUrlsImagenes(int numeroImagen)
        {
            // Usar rutas locales guardadas en wwwroot (imágenes reales descargadas)
            var urls = new List<string>
            {
                $"/imagenes/productos/producto_{(numeroImagen * 2) - 1}.jpg",
                $"/imagenes/productos/producto_{(numeroImagen * 2)}.jpg"
            };
            return urls;
        }

        /// <summary>
        /// Crea 10 productos por categoría con 2 imágenes cada uno
        /// </summary>
        private async Task InitializarProductosAsync()
        {
            if (await _context.Productos.AnyAsync())
            {
                _logger.LogDebug("Los productos ya existen");
                return;
            }

            var vendedor = await _userManager.FindByEmailAsync("vendedor@nexshop.com");
            var categorias = await _context.Categorias.ToListAsync();
            var multimedia = new List<Multimedia>();

            var productos = new List<Producto>();
            int productoIndex = 1;

            // ELECTRÓNICA (10 productos)
            var electronica = categorias.First(c => c.Nombre == "Electrónica");
            var productosElectronica = new[]
            {
                ("Smartphone XYZ Pro", "Smartphone con pantalla AMOLED 6.7\" y procesador último modelo", 899.99m, 25),
                ("Laptop Gaming 15\"", "Laptop con GPU RTX 4060 y procesador i7-13700K", 1299.99m, 10),
                ("Tablet 10 pulgadas", "Tablet con pantalla IPS y 128GB almacenamiento", 349.99m, 15),
                ("Auriculares Bluetooth", "Auriculares inalámbricos con cancelación de ruido", 199.99m, 40),
                ("Monitor 4K 27\"", "Monitor gaming 4K con 144Hz y HDR", 599.99m, 8),
                ("Teclado Mecánico RGB", "Teclado mecánico con switches Cherry MX y iluminación RGB", 149.99m, 20),
                ("Ratón Gaming Inalámbrico", "Ratón ergonómico con sensor 16000 DPI", 89.99m, 30),
                ("Webcam 4K", "Cámara web USB 4K con micrófono integrado", 129.99m, 12),
                ("Cargador Rápido 100W", "Cargador USB-C con carga rápida hasta 100W", 49.99m, 50),
                ("Powerbank 30000mAh", "Batería externa con carga rápida y múltiples puertos", 59.99m, 35)
            };

            foreach (var (nombre, desc, precio, stock) in productosElectronica)
            {
                var prod = new Producto
                {
                    Nombre = nombre,
                    Descripcion = desc,
                    Precio = precio,
                    Stock = stock,
                    Estado = "Disponible",
                    CategoriaId = electronica.CategoriaId,
                    VendedorId = vendedor.Id,
                    FechaCreacion = DateTime.UtcNow
                };
                productos.Add(prod);
            }

            // ROPA (10 productos)
            var ropa = categorias.First(c => c.Nombre == "Ropa");
            var productosRopa = new[]
            {
                ("Camiseta Básica Blanca", "Camiseta de algodón 100% manga corta", 14.99m, 100),
                ("Jeans Azul Oscuro", "Pantalón vaquero slim fit ajustado", 59.99m, 50),
                ("Sudadera con Capucha", "Sudadera de algodón con bolsillos y capucha", 44.99m, 30),
                ("Chaqueta de Cuero", "Chaqueta de cuero auténtico marrón", 199.99m, 15),
                ("Pantalones Deportivos", "Pantalones técnicos para running y entrenamientos", 49.99m, 40),
                ("Calcetines Pack de 5", "Calcetines de algodón antibacteriano", 12.99m, 80),
                ("Gorro de Lana", "Gorro de invierno en varios colores", 24.99m, 25),
                ("Bufanda Larga", "Bufanda de lana para invierno", 34.99m, 20),
                ("Zapatos Deportivos", "Zapatillas running con amortiguación", 129.99m, 18),
                ("Cinturón de Cuero", "Cinturón de cuero marrón con hebilla plateada", 34.99m, 35)
            };

            foreach (var (nombre, desc, precio, stock) in productosRopa)
            {
                var prod = new Producto
                {
                    Nombre = nombre,
                    Descripcion = desc,
                    Precio = precio,
                    Stock = stock,
                    Estado = "Disponible",
                    CategoriaId = ropa.CategoriaId,
                    VendedorId = vendedor.Id,
                    FechaCreacion = DateTime.UtcNow
                };
                productos.Add(prod);
            }

            // HOGAR (10 productos)
            var hogar = categorias.First(c => c.Nombre == "Hogar");
            var productosHogar = new[]
            {
                ("Juego de Sábanas 100% Algodón", "Juego de sábanas para cama de matrimonio", 49.99m, 30),
                ("Almohada de Espuma", "Almohada viscoelástica ergonómica", 39.99m, 25),
                ("Edredón Nórdico", "Edredón relleno de plumón blanco", 79.99m, 15),
                ("Cortinas Blackout", "Cortinas opacas para oscurecer habitaciones", 69.99m, 20),
                ("Lámpara de Escritorio LED", "Lámpara con luz ajustable y puerto USB", 34.99m, 40),
                ("Espejo de Pared Grande", "Espejo decorativo 80x100cm", 89.99m, 10),
                ("Alfombra Persa", "Alfombra tradicional con diseños clásicos", 199.99m, 8),
                ("Juego de Toallas 6 Piezas", "Toallas de microfibra suave y absorbentes", 44.99m, 35),
                ("Tapete de Baño", "Tapete antideslizante para baño", 19.99m, 50),
                ("Plantas Decorativas Artificiales", "Set de 3 plantas artificiales decorativas", 54.99m, 20)
            };

            foreach (var (nombre, desc, precio, stock) in productosHogar)
            {
                var prod = new Producto
                {
                    Nombre = nombre,
                    Descripcion = desc,
                    Precio = precio,
                    Stock = stock,
                    Estado = "Disponible",
                    CategoriaId = hogar.CategoriaId,
                    VendedorId = vendedor.Id,
                    FechaCreacion = DateTime.UtcNow
                };
                productos.Add(prod);
            }

            // DEPORTES (10 productos)
            var deportes = categorias.First(c => c.Nombre == "Deportes");
            var productosDeportes = new[]
            {
                ("Balón de Fútbol Profesional", "Balón FIFA tamaño 5", 34.99m, 25),
                ("Raqueta de Tenis", "Raqueta profesional de fibra de carbono", 149.99m, 12),
                ("Pelota de Baloncesto", "Balón de baloncesto tamaño 7", 29.99m, 20),
                ("Mancuernas Ajustables", "Pesas de 2-20 kg con estuche", 199.99m, 15),
                ("Colchoneta de Yoga", "Tapete antideslizante para yoga y pilates", 24.99m, 40),
                ("Banda Elástica Resistencia", "Set de 5 bandas elásticas diferentes resistencias", 19.99m, 50),
                ("Botella de Agua 1L", "Botella termo de acero inoxidable", 34.99m, 60),
                ("Guantes de Boxeo", "Guantes profesionales 12 oz", 69.99m, 18),
                ("Cinta Métrica Flexible", "Cinta métrica corporal de 1.5m", 9.99m, 100),
                ("Uniforme Deportivo", "Set playera + shorts deportivos", 49.99m, 30)
            };

            foreach (var (nombre, desc, precio, stock) in productosDeportes)
            {
                var prod = new Producto
                {
                    Nombre = nombre,
                    Descripcion = desc,
                    Precio = precio,
                    Stock = stock,
                    Estado = "Disponible",
                    CategoriaId = deportes.CategoriaId,
                    VendedorId = vendedor.Id,
                    FechaCreacion = DateTime.UtcNow
                };
                productos.Add(prod);
            }

            // LIBROS (10 productos)
            var libros = categorias.First(c => c.Nombre == "Libros");
            var productosLibros = new[]
            {
                ("El Quijote - Cervantes", "Novela clásica española edición de bolsillo", 14.99m, 40),
                ("1984 - George Orwell", "Novela distópica clásica", 12.99m, 35),
                ("Cien Años de Soledad - García Márquez", "Novela realismo mágico", 16.99m, 30),
                ("Hábitos Atómicos - James Clear", "Libro de autoayuda sobre hábitos", 18.99m, 25),
                ("El Alquimista - Paulo Coelho", "Novela inspiradora", 13.99m, 45),
                ("Sapiens - Yuval Noah Harari", "Historia resumida de la humanidad", 19.99m, 20),
                ("El Poder del Ahora - Eckhart Tolle", "Libro de desarrollo personal", 17.99m, 22),
                ("La Revolución de los Creativos", "Sobre el futuro de la creatividad", 15.99m, 18),
                ("Mindfulness para Principiantes", "Guía práctica de meditación", 14.99m, 28),
                ("El Juego Infinito - Simon Sinek", "Liderazgo y propósito", 20.99m, 15)
            };

            foreach (var (nombre, desc, precio, stock) in productosLibros)
            {
                var prod = new Producto
                {
                    Nombre = nombre,
                    Descripcion = desc,
                    Precio = precio,
                    Stock = stock,
                    Estado = "Disponible",
                    CategoriaId = libros.CategoriaId,
                    VendedorId = vendedor.Id,
                    FechaCreacion = DateTime.UtcNow
                };
                productos.Add(prod);
            }

            // Guardar todos los productos primero
            _context.Productos.AddRange(productos);
            await _context.SaveChangesAsync();

            // Ahora agregar multimedia (imágenes) a cada producto
            int indiceProducto = 1;
            var listaMultimedia = new List<Multimedia>();

            foreach (var producto in productos)
            {
                // Generar 2 imágenes por producto
                var urls = GenerarUrlsImagenes(indiceProducto);

                // Primera imagen (principal)
                listaMultimedia.Add(new Multimedia
                {
                    ProductoId = producto.ProductoId,
                    Tipo = "Foto",
                    Nombre = $"{producto.Nombre} - Imagen 1",
                    Url = urls[0],
                    NombreArchivo = $"{producto.ProductoId}_1.jpg",
                    Descripcion = $"Imagen principal de {producto.Nombre}",
                    TamanoBytes = 0,
                    TipoMime = "image/jpeg",
                    Orden = 1,
                    EsPrincipal = true,
                    EstaActivo = true,
                    FechaCreacion = DateTime.UtcNow
                });

                // Segunda imagen
                listaMultimedia.Add(new Multimedia
                {
                    ProductoId = producto.ProductoId,
                    Tipo = "Foto",
                    Nombre = $"{producto.Nombre} - Imagen 2",
                    Url = urls[1],
                    NombreArchivo = $"{producto.ProductoId}_2.jpg",
                    Descripcion = $"Imagen secundaria de {producto.Nombre}",
                    TamanoBytes = 0,
                    TipoMime = "image/jpeg",
                    Orden = 2,
                    EsPrincipal = false,
                    EstaActivo = true,
                    FechaCreacion = DateTime.UtcNow
                });

                indiceProducto++;
            }

            _context.Multimedia.AddRange(listaMultimedia);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Se crearon {Count} productos con {MediaCount} imágenes", productos.Count, listaMultimedia.Count);
        }
    }
}
