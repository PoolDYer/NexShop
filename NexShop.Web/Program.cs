using NexShop.Web.Models;
using NexShop.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Agregar configuración de DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'");

// Usar SQL Server en lugar de SQLite
builder.Services.AddDbContext<NexShopContext>(options =>
    options.UseSqlServer(connectionString));

// Agregar servicios de Identity
builder.Services.AddDefaultIdentity<Usuario>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<NexShopContext>();

// Configurar opciones de Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
});

// Agregar servicios personalizados
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ISeederService, SeederService>();
builder.Services.AddScoped<IPreguntaService, PreguntaService>();
builder.Services.AddScoped<IImagenGeneratorService, ImagenGeneratorService>();
builder.Services.AddScoped<IPagoService, PagoSimuladoService>();
builder.Services.AddScoped<IStockService, StockService>();

// Agregar servicio de imágenes virtuales
builder.Services.AddScoped<IImagenVirtualService, ImagenVirtualService>();

// Agregar servicios de multimedia
builder.Services.Configure<MultimediaOptions>(builder.Configuration.GetSection(MultimediaOptions.SectionName));
builder.Services.AddScoped<IMultimediaService, MultimediaService>();
builder.Services.AddValidacionMultimedia();

// Agregar servicio de sincronización de imágenes
builder.Services.AddScoped<ISincronizacionImagenesService, SincronizacionImagenesService>();
builder.Services.AddScoped<ISincronizacionMultimediaService, SincronizacionMultimediaService>();

// Registrar servicio de almacenamiento (Local por defecto)
// Para cambiar a Azure, descomenta la siguiente línea y comenta la del almacenamiento local
var multimediaConfig = builder.Configuration.GetSection(MultimediaOptions.SectionName).Get<MultimediaOptions>();
if (multimediaConfig?.UsarAzureBlob == true)
{
    builder.Services.AddScoped<IAlmacenamiento, AlmacenacionAzure>();
}
else
{
    builder.Services.AddScoped<IAlmacenamiento, AlmacenacionLocal>();
}

// Agregar servicios de autorización personalizada
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutorizacionPolicies();

// Agregar sesión para carrito de compras
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Crear base de datos y aplicar migraciones
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NexShopContext>();
    dbContext.Database.EnsureCreated();
}

// Inicializar directorio de multimedia
var env = app.Services.GetRequiredService<IWebHostEnvironment>();
var multimediaOptions = app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<MultimediaOptions>>().Value;

if (string.IsNullOrEmpty(multimediaOptions.RutaAbsoluta))
{
    multimediaOptions.RutaAbsoluta = Path.Combine(env.WebRootPath, multimediaOptions.RutaAlmacenamiento);
}

if (!Directory.Exists(multimediaOptions.RutaAbsoluta))
{
    Directory.CreateDirectory(multimediaOptions.RutaAbsoluta);
}

// Inicializar datos y roles
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ISeederService>();
    await seeder.InitializeAsync();
}

// Generar imágenes PNG si no existen
using (var scope = app.Services.CreateScope())
{
    var imagenGenerator = scope.ServiceProvider.GetRequiredService<IImagenGeneratorService>();
    await imagenGenerator.GenerarImagenesAsync();
}

// Sincronizar imágenes virtuales al inicio
using (var scope = app.Services.CreateScope())
{
    var imagenVirtualService = scope.ServiceProvider.GetRequiredService<IImagenVirtualService>();
    await imagenVirtualService.SincronizarImagenesVirtualesAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Usar middleware de sesión
app.UseSession();

// Usar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Manejo de rutas para errores de autorización
app.UseStatusCodePagesWithRedirects("/Error/{0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
