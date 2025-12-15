# Script para crear carpetas de productos y aplicar cambios en la BD
# Ejecutar desde: E:\Proyectos Visual\NexShop\NexShop.Web

param(
    [string]$WebRootPath = ".\wwwroot",
    [string]$ConnectionString = "Data Source=NexShop.db"
)

Write-Host "?????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?  CREAR CARPETAS PARA TODOS LOS PRODUCTOS EXISTENTES          ?" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Verificar que wwwroot existe
if (-not (Test-Path $WebRootPath)) {
    Write-Host "? ERROR: No se encontró $WebRootPath" -ForegroundColor Red
    exit 1
}

Write-Host "? Carpeta wwwroot encontrada: $WebRootPath" -ForegroundColor Green
Write-Host ""

# Crear estructura de carpetas
$uploadsPath = Join-Path $WebRootPath "uploads"
$productosPath = Join-Path $uploadsPath "productos"

# Crear carpetas base si no existen
if (-not (Test-Path $uploadsPath)) {
    New-Item -Path $uploadsPath -ItemType Directory | Out-Null
    Write-Host "? Carpeta creada: /uploads" -ForegroundColor Green
}

if (-not (Test-Path $productosPath)) {
    New-Item -Path $productosPath -ItemType Directory | Out-Null
    Write-Host "? Carpeta creada: /uploads/productos" -ForegroundColor Green
}

Write-Host ""
Write-Host "?? Leyendo productos de la base de datos..." -ForegroundColor Yellow

# Usar .NET para leer la BD
$dbPath = Join-Path (Get-Location) "NexShop.db"

if (-not (Test-Path $dbPath)) {
    Write-Host "? ERROR: Base de datos no encontrada en: $dbPath" -ForegroundColor Red
    Write-Host "?? Directorio actual: $(Get-Location)" -ForegroundColor Yellow
    exit 1
}

Write-Host "? Base de datos encontrada: $dbPath" -ForegroundColor Green
Write-Host ""

# Crear conexión SQLite
Add-Type -Path "$PSScriptRoot\System.Data.SQLite.dll" -ErrorAction SilentlyContinue

try {
    # Usar sqlite3.exe si está disponible
    $sqlite3 = "sqlite3.exe"
    
    # Query para obtener todos los productos
    $query = "SELECT ProductoId, Nombre FROM Productos ORDER BY ProductoId;"
    
    # Ejecutar query
    $output = & $sqlite3 $dbPath $query 2>&1
    
    if ($output -eq $null -or $output -eq "") {
        Write-Host "??  No se encontraron productos o error al leer la BD" -ForegroundColor Yellow
        Write-Host "?? Intenta crear un producto primero desde la aplicación" -ForegroundColor Cyan
        exit 0
    }
    
    $productos = @()
    foreach ($line in $output) {
        if ($line -match "^\d+\|") {
            $parts = $line -split "\|"
            if ($parts.Count -ge 2) {
                $productos += @{
                    ProductoId = [int]$parts[0]
                    Nombre = $parts[1]
                }
            }
        }
    }
    
    if ($productos.Count -eq 0) {
        Write-Host "??  No se encontraron productos" -ForegroundColor Yellow
        exit 0
    }
    
    Write-Host "? Se encontraron $($productos.Count) producto(s)" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? Creando carpetas para cada producto:" -ForegroundColor Cyan
    Write-Host ""
    
    $carpetasCreadas = 0
    $carpetasExistentes = 0
    
    foreach ($producto in $productos) {
        $id = $producto.ProductoId
        $nombre = $producto.Nombre
        
        # Sanitizar nombre (remover caracteres inválidos)
        $nombreSanitizado = $nombre -replace '[<>:"/\\|?*]', '_'
        
        # Crear nombre de carpeta
        $carpetaNombre = "${id}_${nombreSanitizado}"
        $carpetaPath = Join-Path $productosPath $carpetaNombre
        
        # Crear carpeta si no existe
        if (-not (Test-Path $carpetaPath)) {
            New-Item -Path $carpetaPath -ItemType Directory | Out-Null
            Write-Host "  ? [$id] $nombre" -ForegroundColor Green
            $carpetasCreadas++
        } else {
            Write-Host "  ??  [$id] $nombre (ya existe)" -ForegroundColor Yellow
            $carpetasExistentes++
        }
    }
    
    Write-Host ""
    Write-Host "?? RESUMEN:" -ForegroundColor Cyan
    Write-Host "  • Carpetas creadas: $carpetasCreadas" -ForegroundColor Green
    Write-Host "  • Carpetas existentes: $carpetasExistentes" -ForegroundColor Yellow
    Write-Host "  • Total de carpetas: $($carpetasCreadas + $carpetasExistentes)" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Host "? COMPLETADO" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? Ubicación de carpetas:" -ForegroundColor Cyan
    Write-Host "   $productosPath" -ForegroundColor White
    Write-Host ""
    Write-Host "?? Ahora puedes:" -ForegroundColor Cyan
    Write-Host "   1. Ir a Visual Studio File Explorer" -ForegroundColor White
    Write-Host "   2. Navegar a: wwwroot > uploads > productos" -ForegroundColor White
    Write-Host "   3. Descargar imágenes en cada carpeta" -ForegroundColor White
    Write-Host "   4. O subir desde la aplicación en Productos > Details" -ForegroundColor White
    Write-Host ""
    
}
catch {
    Write-Host "? ERROR: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "?? Alternativa: Crear carpetas manualmente" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Abre Visual Studio y:"
    Write-Host "  1. Ve a wwwroot > uploads > productos" -ForegroundColor White
    Write-Host "  2. Crea carpetas con este formato:" -ForegroundColor White
    Write-Host "     {ProductoId}_{NombreProducto}" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}
