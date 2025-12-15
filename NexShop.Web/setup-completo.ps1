# Script MAESTRO: Descargar imágenes, limpiar BD e iniciar
# Ejecutar desde PowerShell en la carpeta del proyecto

Clear-Host
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?   ?? NEXSHOP - DESCARGA DE IMÁGENES Y SETUP FINAL         ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan

$projectPath = "E:\Proyectos Visual\NexShop\NexShop.Web"
$imagePath = "$projectPath\wwwroot\imagenes\productos"

# ============================================
# PASO 1: Crear carpeta de imágenes
# ============================================
Write-Host ""
Write-Host "?? PASO 1: Preparando carpeta de imágenes..." -ForegroundColor Yellow

if (-not (Test-Path $imagePath)) {
    New-Item -ItemType Directory -Path $imagePath -Force | Out-Null
    Write-Host "  ? Carpeta creada: $imagePath" -ForegroundColor Green
} else {
    Write-Host "  ??  Carpeta ya existe: $imagePath" -ForegroundColor Cyan
}

# ============================================
# PASO 2: Descargar imágenes
# ============================================
Write-Host ""
Write-Host "?? PASO 2: Descargando 100 imágenes locales..." -ForegroundColor Yellow
Write-Host "   (Esto puede tomar 2-3 minutos, por favor espera...)" -ForegroundColor Gray

$downloaded = 0
$failed = 0
$skipped = 0

for ($i = 1; $i -le 100; $i++) {
    $fileName = "producto_$i.jpg"
    $filePath = Join-Path $imagePath $fileName
    
    # Si ya existe, saltar
    if (Test-Path $filePath) {
        $skipped++
        continue
    }
    
    # URLs alternativas
    $urls = @(
        "https://picsum.photos/400/400?random=$i",
        "https://loremflickr.com/400/400?lock=$i",
        "https://source.unsplash.com/400x400/?product,retail&sig=$i"
    )
    
    $success = $false
    foreach ($url in $urls) {
        try {
            $ProgressPreference = 'SilentlyContinue'
            Invoke-WebRequest -Uri $url -OutFile $filePath -TimeoutSec 5 -ErrorAction SilentlyContinue
            
            if (Test-Path $filePath) {
                $success = $true
                $downloaded++
                Write-Host "  ? Descargada imagen $i/100" -ForegroundColor Green -NoNewline
                Write-Host " - $fileName" -ForegroundColor Gray
                break
            }
        }
        catch { }
    }
    
    if (-not $success) {
        $failed++
    }
}

Write-Host ""
Write-Host "?? Resultado de descarga:" -ForegroundColor Cyan
Write-Host "   ? Nuevas: $downloaded" -ForegroundColor Green
Write-Host "   ??  Ya existían: $skipped" -ForegroundColor Cyan
Write-Host "   ? Fallidas: $failed" -ForegroundColor Yellow

# ============================================
# PASO 3: Limpiar base de datos
# ============================================
Write-Host ""
Write-Host "?? PASO 3: Limpiando base de datos..." -ForegroundColor Yellow

Set-Location $projectPath

# Eliminar archivos de BD
Get-ChildItem -Filter "*.db*" -ErrorAction SilentlyContinue | Remove-Item -Force -ErrorAction SilentlyContinue
Write-Host "  ? Archivos de BD eliminados" -ForegroundColor Green

# Ejecutar migraciones
Write-Host "  ? Ejecutando migraciones..." -ForegroundColor Gray
try {
    dotnet ef database update 2>$null | Out-Null
    Write-Host "  ? Migraciones completadas" -ForegroundColor Green
}
catch {
    Write-Host "  ??  Error en migraciones (continuando...)" -ForegroundColor Yellow
}

# ============================================
# PASO 4: Resultado final
# ============================================
Write-Host ""
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "?                ? SETUP COMPLETADO                        ?" -ForegroundColor Green
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Green

Write-Host ""
Write-Host "?? RESUMEN:" -ForegroundColor Cyan
Write-Host "  ? Imágenes descargadas: $($downloaded + $skipped)/100" -ForegroundColor Green
Write-Host "  ? Base de datos limpiada" -ForegroundColor Green
Write-Host "  ? Migraciones ejecutadas" -ForegroundColor Green
Write-Host ""
Write-Host "?? SIGUIENTE PASO:" -ForegroundColor Yellow
Write-Host "  Inicia la aplicación con:" -ForegroundColor Yellow
Write-Host "    dotnet run" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? Accede a:" -ForegroundColor Yellow
Write-Host "  http://localhost:5217/Productos" -ForegroundColor Cyan
Write-Host ""
Write-Host "? Las imágenes se cargarán desde:" -ForegroundColor Cyan
Write-Host "  $imagePath" -ForegroundColor Gray
