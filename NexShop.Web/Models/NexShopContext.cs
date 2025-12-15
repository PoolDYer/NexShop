using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NexShop.Web.Models
{
    /// <summary>
    /// DbContext principal para la aplicación NexShop
    /// Configura todas las entidades y sus relaciones de navegación
    /// </summary>
    public class NexShopContext : IdentityDbContext<Usuario>
    {
        /// <summary>
        /// Constructor del DbContext que recibe las opciones de configuración
        /// </summary>
        /// <param name="options">Opciones de configuración del DbContext</param>
        public NexShopContext(DbContextOptions<NexShopContext> options) : base(options)
        {
        }

        // DbSets para cada entidad

        /// <summary>
        /// DbSet para las categorías de productos
        /// </summary>
        public DbSet<Categoria> Categorias { get; set; }

        /// <summary>
        /// DbSet para los productos
        /// </summary>
        public DbSet<Producto> Productos { get; set; }

        /// <summary>
        /// DbSet para las órdenes
        /// </summary>
        public DbSet<Orden> Ordenes { get; set; }

        /// <summary>
        /// DbSet para los detalles de las órdenes
        /// </summary>
        public DbSet<OrdenDetalle> OrdenDetalles { get; set; }

        /// <summary>
        /// DbSet para los archivos multimedia
        /// </summary>
        public DbSet<Multimedia> Multimedia { get; set; }

        /// <summary>
        /// DbSet para las preguntas
        /// </summary>
        public DbSet<Pregunta> Preguntas { get; set; }

        /// <summary>
        /// DbSet para las respuestas
        /// </summary>
        public DbSet<Respuesta> Respuestas { get; set; }

        /// <summary>
        /// DbSet para las calificaciones
        /// </summary>
        public DbSet<Calificacion> Calificaciones { get; set; }

        /// <summary>
        /// Configura el modelo de datos y las relaciones entre entidades
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad Usuario
            ConfigurarUsuario(modelBuilder);

            // Configuración de la entidad Categoria
            ConfigurarCategoria(modelBuilder);

            // Configuración de la entidad Producto
            ConfigurarProducto(modelBuilder);

            // Configuración de la entidad Orden
            ConfigurarOrden(modelBuilder);

            // Configuración de la entidad OrdenDetalle
            ConfigurarOrdenDetalle(modelBuilder);

            // Configuración de la entidad Multimedia
            ConfigurarMultimedia(modelBuilder);

            // Configuración de la entidad Pregunta
            ConfigurarPregunta(modelBuilder);

            // Configuración de la entidad Respuesta
            ConfigurarRespuesta(modelBuilder);

            // Configuración de la entidad Calificacion
            ConfigurarCalificacion(modelBuilder);
        }

        /// <summary>
        /// Configura la entidad Usuario con índices y propiedades especiales
        /// </summary>
        private void ConfigurarUsuario(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.NombreCompleto)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.TipoUsuario)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Comprador");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500);

                entity.Property(e => e.Direccion)
                    .HasMaxLength(255);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(e => e.CalificacionPromedio)
                    .HasColumnType("decimal(3,2)");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Índices
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.TipoUsuario);
                entity.HasIndex(e => e.EstaActivo);

                // Relaciones
                entity.HasMany(e => e.Ordenes)
                    .WithOne(o => o.Comprador)
                    .HasForeignKey(o => o.CompradorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.ProductosVendidos)
                    .WithOne(p => p.Vendedor)
                    .HasForeignKey(p => p.VendedorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        /// <summary>
        /// Configura la entidad Categoria
        /// </summary>
        private void ConfigurarCategoria(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.CategoriaId);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500);

                entity.Property(e => e.IconoUrl)
                    .HasMaxLength(255);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Índices
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.HasIndex(e => e.EstaActiva);

                // Relaciones
                entity.HasMany(e => e.Productos)
                    .WithOne(p => p.Categoria)
                    .HasForeignKey(p => p.CategoriaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        /// <summary>
        /// Configura la entidad Producto
        /// </summary>
        private void ConfigurarProducto(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.ProductoId);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Precio)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Stock)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasDefaultValue("Disponible");

                entity.Property(e => e.SKU)
                    .HasMaxLength(50);

                entity.Property(e => e.CalificacionPromedio)
                    .HasColumnType("decimal(3,2)");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Configurar relaciones de clave foránea explícitamente
                entity.HasOne(e => e.Categoria)
                    .WithMany(c => c.Productos)
                    .HasForeignKey(e => e.CategoriaId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Vendedor)
                    .WithMany(u => u.ProductosVendidos)
                    .HasForeignKey(e => e.VendedorId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.Nombre);
                entity.HasIndex(e => e.CategoriaId);
                entity.HasIndex(e => e.VendedorId);
                entity.HasIndex(e => e.Estado);
                entity.HasIndex(e => e.SKU).IsUnique();

                // Relaciones
                entity.HasMany(e => e.Multimedia)
                    .WithOne(m => m.Producto)
                    .HasForeignKey(m => m.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.DetallesOrdenes)
                    .WithOne(od => od.Producto)
                    .HasForeignKey(od => od.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        /// <summary>
        /// Configura la entidad Orden
        /// </summary>
        private void ConfigurarOrden(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Orden>(entity =>
            {
                entity.HasKey(e => e.OrdenId);

                entity.Property(e => e.NumeroOrden)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MontoTotal)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Impuesto)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValue(0);

                entity.Property(e => e.MontoEnvio)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValue(0);

                entity.Property(e => e.Descuento)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValue(0);

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasDefaultValue("Pendiente");

                entity.Property(e => e.MetodoPago)
                    .HasMaxLength(50);

                entity.Property(e => e.DireccionEntrega)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Notas)
                    .HasMaxLength(500);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Configurar relaciones de clave foránea
                entity.HasOne(e => e.Comprador)
                    .WithMany(u => u.Ordenes)
                    .HasForeignKey(e => e.CompradorId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.NumeroOrden).IsUnique();
                entity.HasIndex(e => e.CompradorId);
                entity.HasIndex(e => e.Estado);
                entity.HasIndex(e => e.FechaCreacion);

                // Relaciones
                entity.HasMany(e => e.Detalles)
                    .WithOne(od => od.Orden)
                    .HasForeignKey(od => od.OrdenId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        /// <summary>
        /// Configura la entidad OrdenDetalle
        /// </summary>
        private void ConfigurarOrdenDetalle(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrdenDetalle>(entity =>
            {
                entity.HasKey(e => e.OrdenDetalleId);

                entity.Property(e => e.Cantidad)
                    .IsRequired();

                entity.Property(e => e.PrecioUnitario)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Subtotal)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

                // Configurar relaciones de clave foránea
                entity.HasOne(e => e.Orden)
                    .WithMany(o => o.Detalles)
                    .HasForeignKey(e => e.OrdenId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.DetallesOrdenes)
                    .HasForeignKey(e => e.ProductoId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.OrdenId);
                entity.HasIndex(e => e.ProductoId);
            });
        }

        /// <summary>
        /// Configura la entidad Multimedia
        /// </summary>
        private void ConfigurarMultimedia(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Multimedia>(entity =>
            {
                entity.HasKey(e => e.MultimediaId);

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.NombreArchivo)
                    .HasMaxLength(255);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500);

                entity.Property(e => e.TipoMime)
                    .HasMaxLength(50);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Configurar relaciones de clave foránea
                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.Multimedia)
                    .HasForeignKey(e => e.ProductoId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices
                entity.HasIndex(e => e.ProductoId);
                entity.HasIndex(e => e.Tipo);
                entity.HasIndex(e => e.EstaActivo);
            });
        }

        /// <summary>
        /// Configura la entidad Pregunta
        /// </summary>
        private void ConfigurarPregunta(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pregunta>(entity =>
            {
                entity.HasKey(e => e.PreguntaId);

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(2000);

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Pendiente");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Configurar relaciones
                entity.HasOne(e => e.Producto)
                    .WithMany()
                    .HasForeignKey(e => e.ProductoId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.ProductoId);
                entity.HasIndex(e => e.UsuarioId);
                entity.HasIndex(e => e.Estado);
                entity.HasIndex(e => e.FechaCreacion);

                // Relaciones
                entity.HasMany(e => e.Respuestas)
                    .WithOne(r => r.Pregunta)
                    .HasForeignKey(r => r.PreguntaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        /// <summary>
        /// Configura la entidad Respuesta
        /// </summary>
        private void ConfigurarRespuesta(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Respuesta>(entity =>
            {
                entity.HasKey(e => e.RespuestaId);

                entity.Property(e => e.Contenido)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Configurar relaciones
                entity.HasOne(e => e.Pregunta)
                    .WithMany(p => p.Respuestas)
                    .HasForeignKey(e => e.PreguntaId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.PreguntaId);
                entity.HasIndex(e => e.UsuarioId);
                entity.HasIndex(e => e.FechaCreacion);
            });
        }

        /// <summary>
        /// Configura la entidad Calificacion
        /// </summary>
        private void ConfigurarCalificacion(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Calificacion>(entity =>
            {
                entity.HasKey(e => e.CalificacionId);

                entity.Property(e => e.Puntaje)
                    .IsRequired();

                entity.Property(e => e.Comentario)
                    .HasMaxLength(500);

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasDefaultValue("Comprador");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Configurar relaciones
                entity.HasOne(e => e.Vendedor)
                    .WithMany()
                    .HasForeignKey(e => e.VendedorId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Orden)
                    .WithMany()
                    .HasForeignKey(e => e.OrdenId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Índices
                entity.HasIndex(e => e.VendedorId);
                entity.HasIndex(e => e.UsuarioId);
                entity.HasIndex(e => e.Tipo);
                entity.HasIndex(e => e.FechaCreacion);
            });
        }
    }
}
