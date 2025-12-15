# SCRIPT FINAL - Crear carpetas sin dependencias
# Ejecutar desde: E:\Proyectos Visual\NexShop\NexShop.Web

Write-Host ""
Write-Host "?????????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?                                                                   ?" -ForegroundColor Cyan
Write-Host "?   ? CREAR CARPETAS PARA TODOS LOS PRODUCTOS EXISTENTES ?       ?" -ForegroundColor Cyan
Write-Host "?                                                                   ?" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Configuración
$projectPath = Get-Location
$wwwrootPath = Join-Path $projectPath "wwwroot"
$uploadsPath = Join-Path $wwwrootPath "uploads"
$productosPath = Join-Path $uploadsPath "productos"

Write-Host "?? Verificando ubicación..." -ForegroundColor Yellow
Write-Host "   Ubicación actual: $projectPath" -ForegroundColor Gray
Write-Host ""

# Verificar que estamos en la carpeta correcta
if (-not (Test-Path "NexShop.Web.csproj")) {
    Write-Host "? ERROR: No estás en la carpeta NexShop.Web" -ForegroundColor Red
    Write-Host ""
    Write-Host "Solución:" -ForegroundColor Yellow
    Write-Host "  1. Abre PowerShell" -ForegroundColor White
    Write-Host "  2. Navega a: cd E:\Proyectos Visual\NexShop\NexShop.Web" -ForegroundColor White
    Write-Host "  3. Luego ejecuta: .\crear-carpetas-automatico.ps1" -ForegroundColor White
    Write-Host ""
    exit 1
}

Write-Host "? Ubicación verificada: NexShop.Web" -ForegroundColor Green
Write-Host ""

# Crear estructura de carpetas
Write-Host "?? Creando estructura de carpetas..." -ForegroundColor Yellow

if (-not (Test-Path $uploadsPath)) {
    New-Item -Path $uploadsPath -ItemType Directory -Force | Out-Null
    Write-Host "   ? /uploads creada" -ForegroundColor Green
} else {
    Write-Host "   ? /uploads existente" -ForegroundColor Green
}

if (-not (Test-Path $productosPath)) {
    New-Item -Path $productosPath -ItemType Directory -Force | Out-Null
    Write-Host "   ? /uploads/productos creada" -ForegroundColor Green
} else {
    Write-Host "   ? /uploads/productos existente" -ForegroundColor Green
}

Write-Host ""

# Crear carpetas de productos
# Estos son nombres de ejemplo - en producción vendría de la BD
Write-Host "?? Creando carpetas de productos..." -ForegroundColor Cyan
Write-Host ""

$productos = @(
    "1_Power_Bank_3000_mAh",
    "2_Cable_USB_Tipo_C",
    "3_Audífonos_Bluetooth_Inalámbricos",
    "4_Funda_Protectora_Teléfono",
    "5_Protector_Pantalla_Cristal",
    "6_Batería_Externa_Portátil",
    "7_Cargador_Rápido_65W",
    "8_Micrófono_USB_Condensador",
    "9_Hub_USB_Adaptador",
    "10_Adaptador_HDMI_4K"
)

$carpetasCreadas = 0
$carpetasExistentes = 0

foreach ($nombreCarpeta in $productos) {
    $rutaCarpeta = Join-Path $productosPath $nombreCarpeta
    
    if (-not (Test-Path $rutaCarpeta)) {
        New-Item -Path $rutaCarpeta -ItemType Directory -Force | Out-Null
        Write-Host "   ? Creada: $nombreCarpeta" -ForegroundColor Green
        $carpetasCreadas++
    } else {
        Write-Host "   ??  Existe: $nombreCarpeta" -ForegroundColor Yellow
        $carpetasExistentes++
    }
}

Write-Host ""
Write-Host "?? RESUMEN:" -ForegroundColor Cyan
Write-Host "   • Carpetas creadas: $carpetasCreadas" -ForegroundColor Green
Write-Host "   • Carpetas existentes: $carpetasExistentes" -ForegroundColor Yellow
Write-Host "   • Total: $($carpetasCreadas + $carpetasExistentes)" -ForegroundColor Cyan
Write-Host ""

# Mostrar estructura
Write-Host "?? Estructura de carpetas creada:" -ForegroundColor Cyan
Write-Host ""
Write-Host "   wwwroot/" -ForegroundColor White
Write-Host "   ??? uploads/" -ForegroundColor White
Write-Host "       ??? productos/" -ForegroundColor White

$carpetas = Get-ChildItem $productosPath -Directory | Select-Object -First 5
foreach ($carpeta in $carpetas) {
    Write-Host "           ??? $($carpeta.Name)/" -ForegroundColor Cyan
}

$totalCarpetas = (Get-ChildItem $productosPath -Directory).Count
if ($totalCarpetas -gt 5) {
    Write-Host "           ??? ... y $($totalCarpetas - 5) más" -ForegroundColor Gray
}

Write-Host ""
Write-Host "? COMPLETADO" -ForegroundColor Green
Write-Host ""

# Instrucciones
Write-Host "?? PRÓXIMOS PASOS:" -ForegroundColor Green
Write-Host ""
Write-Host "1??  DESCARGAR IMÁGENES" -ForegroundColor White
Write-Host "   • Abre Google Imágenes: https://images.google.com" -ForegroundColor Gray
Write-Host "   • Busca: 'power bank 3000 mah'" -ForegroundColor Gray
Write-Host "   • Descarga 2-3 imágenes por producto" -ForegroundColor Gray
Write-Host ""

Write-Host "2??  AGREGAR IMÁGENES A LAS CARPETAS" -ForegroundColor White
Write-Host "   Opción A (Fácil en Visual Studio):" -ForegroundColor Gray
Write-Host "   • Abre Visual Studio" -ForegroundColor Gray
Write-Host "   • Solution Explorer > wwwroot > uploads > productos" -ForegroundColor Gray
Write-Host "   • Click derecho en una carpeta > Add > Existing Item" -ForegroundColor Gray
Write-Host "   • Selecciona las imágenes descargadas" -ForegroundColor Gray
Write-Host ""
Write-Host "   Opción B (Explorador de Windows):" -ForegroundColor Gray
Write-Host "   • Abre: $productosPath" -ForegroundColor Gray
Write-Host "   • Arrastra las imágenes descargadas a cada carpeta" -ForegroundColor Gray
Write-Host ""

Write-Host "3??  VERIFICAR EN LA APLICACIÓN" -ForegroundColor White
Write-Host "   • Ejecuta: dotnet run" -ForegroundColor Gray
Write-Host "   • Abre: http://localhost:5217/Productos" -ForegroundColor Gray
Write-Host "   • Haz click en un producto para ver la galería" -ForegroundColor Gray
Write-Host ""

Write-Host "?? Ruta de carpetas:" -ForegroundColor Yellow
Write-Host "   $productosPath" -ForegroundColor White
Write-Host ""

# Abrir explorador si lo desea
$respuesta = Read-Host "¿Deseas abrir la carpeta de productos en el Explorador? (S/N)"
if ($respuesta -eq "S" -or $respuesta -eq "s") {
    explorer $productosPath
}

Write-Host ""
Write-Host "? ¡Listo! Las carpetas están creadas. Ahora descarga imágenes. ?" -ForegroundColor Green
Write-Host ""
