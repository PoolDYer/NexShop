# Script para crear carpetas de todos los productos de forma automática
# Ejecutar desde: NexShop.Web

Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?   CREAR CARPETAS PARA TODOS LOS PRODUCTOS AUTOMÁTICAMENTE    ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Ubicación
$carpetaBase = ".\wwwroot\uploads\productos"
$dbFile = "NexShop.db"

# Verificar que estamos en la carpeta correcta
if (-not (Test-Path "NexShop.Web.csproj")) {
    Write-Host "? ERROR: Debes estar en la carpeta NexShop.Web" -ForegroundColor Red
    Write-Host "?? Ubicación actual: $(Get-Location)" -ForegroundColor Yellow
    exit 1
}

Write-Host "? Carpeta correcta verificada" -ForegroundColor Green
Write-Host ""

# Crear carpeta base si no existe
if (-not (Test-Path $carpetaBase)) {
    New-Item -Path $carpetaBase -ItemType Directory -Force | Out-Null
    Write-Host "? Carpeta base creada: $carpetaBase" -ForegroundColor Green
} else {
    Write-Host "? Carpeta base existente: $carpetaBase" -ForegroundColor Green
}

Write-Host ""

# MÉTODO: Crear carpetas basadas en patrón común
# Vamos a leer de la BD usando una combinación de métodos

Write-Host "?? Buscando productos en la base de datos..." -ForegroundColor Yellow
Write-Host ""

# Intentar usar sqlite3 si está disponible
$sqlite3Path = Get-Command sqlite3 -ErrorAction SilentlyContinue

if ($null -ne $sqlite3Path) {
    Write-Host "? sqlite3 encontrado" -ForegroundColor Green
    
    # Ejecutar query
    $query = "SELECT ProductoId, Nombre FROM Productos ORDER BY ProductoId;"
    $output = & sqlite3 $dbFile $query 2>&1
    
    if ($output) {
        $carpetasCreadas = 0
        
        foreach ($line in $output) {
            if ($line -match "^(\d+)\|(.+)$") {
                $productoId = [int]$matches[1]
                $nombre = $matches[2]
                
                # Sanitizar nombre
                $nombreSanitizado = $nombre
                [System.IO.Path]::GetInvalidFileNameChars() | ForEach-Object {
                    $nombreSanitizado = $nombreSanitizado -replace [regex]::Escape($_), "_"
                }
                
                # Crear carpeta
                $carpetaProducto = Join-Path $carpetaBase "${productoId}_${nombreSanitizado}"
                
                if (-not (Test-Path $carpetaProducto)) {
                    New-Item -Path $carpetaProducto -ItemType Directory -Force | Out-Null
                    Write-Host "  ? [$productoId] $nombre" -ForegroundColor Green
                    $carpetasCreadas++
                } else {
                    Write-Host "  ??  [$productoId] $nombre (ya existe)" -ForegroundColor Yellow
                }
            }
        }
        
        Write-Host ""
        Write-Host "? Se crearon $carpetasCreadas carpeta(s)" -ForegroundColor Green
        Write-Host ""
        Write-Host "?? Carpetas en: $carpetaBase" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "? COMPLETADO" -ForegroundColor Green
    }
} else {
    Write-Host "??  sqlite3 no encontrado, creando carpetas de ejemplo..." -ForegroundColor Yellow
    Write-Host ""
    
    # Si no está sqlite3, crear carpetas de ejemplo
    # Basadas en productos típicos de e-commerce
    $productos = @(
        @{ Id = 1; Nombre = "Power_Bank_3000_mAh" },
        @{ Id = 2; Nombre = "Cable_USB_C" },
        @{ Id = 3; Nombre = "Audífonos_Bluetooth" },
        @{ Id = 4; Nombre = "Funda_Teléfono" },
        @{ Id = 5; Nombre = "Protector_Pantalla" }
    )
    
    $carpetasCreadas = 0
    $carpetasExistentes = 0
    
    foreach ($producto in $productos) {
        $carpetaProducto = Join-Path $carpetaBase "$($producto.Id)_$($producto.Nombre)"
        
        if (-not (Test-Path $carpetaProducto)) {
            New-Item -Path $carpetaProducto -ItemType Directory -Force | Out-Null
            Write-Host "  ? [$($producto.Id)] $($producto.Nombre)" -ForegroundColor Green
            $carpetasCreadas++
        } else {
            Write-Host "  ??  [$($producto.Id)] $($producto.Nombre) (ya existe)" -ForegroundColor Yellow
            $carpetasExistentes++
        }
    }
    
    Write-Host ""
    Write-Host "?? RESUMEN:" -ForegroundColor Cyan
    Write-Host "  • Carpetas creadas: $carpetasCreadas" -ForegroundColor Green
    Write-Host "  • Carpetas existentes: $carpetasExistentes" -ForegroundColor Yellow
    Write-Host "  • Total: $($carpetasCreadas + $carpetasExistentes)" -ForegroundColor White
    Write-Host ""
    Write-Host "?? NOTA: Se crearon carpetas de ejemplo" -ForegroundColor Yellow
    Write-Host "   Para crear carpetas con tus productos reales:" -ForegroundColor Yellow
    Write-Host "   1. Instala sqlite3: choco install sqlite" -ForegroundColor White
    Write-Host "   2. Vuelve a ejecutar este script" -ForegroundColor White
    Write-Host ""
}

Write-Host "?? Ubicación de carpetas:" -ForegroundColor Cyan
Write-Host "   $(Get-Item $carpetaBase).FullName" -ForegroundColor White
Write-Host ""

# Mostrar carpetas creadas
Write-Host "?? Carpetas actuales:" -ForegroundColor Cyan
Get-ChildItem $carpetaBase -Directory | ForEach-Object {
    Write-Host "   ? $($_.Name)" -ForegroundColor Green
}

Write-Host ""
Write-Host "? PROCESO COMPLETADO" -ForegroundColor Green
Write-Host ""
Write-Host "?? Siguientes pasos:" -ForegroundColor Cyan
Write-Host "   1. Descarga imágenes de Google Imágenes o sitios gratuitos" -ForegroundColor White
Write-Host "   2. Coloca las imágenes en cada carpeta" -ForegroundColor White
Write-Host "   3. Abre Visual Studio > F5 para refrescar" -ForegroundColor White
Write-Host "   4. Ejecuta: dotnet run" -ForegroundColor White
Write-Host "   5. Ve a Productos para ver las imágenes" -ForegroundColor White
Write-Host ""
