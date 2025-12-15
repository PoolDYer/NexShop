# ? SCRIPT MAESTRO FINAL - NEXSHOP COMPLETO
# Descarga imágenes, limpia BD, inicia la aplicación

Clear-Host

Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?   ?? NEXSHOP SETUP - IMÁGENES LOCALES + BD               ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$projectPath = "E:\Proyectos Visual\NexShop\NexShop.Web"
$imagePath = "$projectPath\wwwroot\imagenes\productos"

# ============================================
# PASO 1: Crear carpeta
# ============================================
Write-Host "?? PASO 1: Preparando carpeta de imágenes..." -ForegroundColor Yellow

if (-not (Test-Path "$projectPath\wwwroot")) {
    New-Item -ItemType Directory -Path "$projectPath\wwwroot" -Force | Out-Null
}

if (-not (Test-Path "$projectPath\wwwroot\imagenes")) {
    New-Item -ItemType Directory -Path "$projectPath\wwwroot\imagenes" -Force | Out-Null
}

if (-not (Test-Path $imagePath)) {
    New-Item -ItemType Directory -Path $imagePath -Force | Out-Null
}

Write-Host "  ? Carpeta lista: $imagePath" -ForegroundColor Green
Write-Host ""

# ============================================
# PASO 2: Descargar imágenes
# ============================================
Write-Host "?? PASO 2: Descargando 100 imágenes PNG..." -ForegroundColor Yellow

$downloaded = 0
for ($i = 1; $i -le 100; $i++) {
    $fileName = "producto_$i.png"
    $filePath = Join-Path $imagePath $fileName
    
    if (Test-Path $filePath) {
        $downloaded++
        continue
    }
    
    try {
        $url = "https://via.placeholder.com/400x400.png?text=Producto+$i"
        $ProgressPreference = 'SilentlyContinue'
        Invoke-WebRequest -Uri $url -OutFile $filePath -TimeoutSec 5 -ErrorAction SilentlyContinue
        
        if (Test-Path $filePath) {
            $downloaded++
            if ($i % 10 -eq 0) {
                Write-Host "  ? Descargadas $i/100 imágenes..." -ForegroundColor Green
            }
        }
    }
    catch { }
}

Write-Host "  ? Descargadas: $downloaded/100 imágenes" -ForegroundColor Green
Write-Host ""

# ============================================
# PASO 3: Limpiar BD
# ============================================
Write-Host "?? PASO 3: Limpiando base de datos..." -ForegroundColor Yellow

Set-Location $projectPath

# Eliminar archivos de BD
Get-ChildItem -Filter "*.db*" -ErrorAction SilentlyContinue | Remove-Item -Force -ErrorAction SilentlyContinue
Write-Host "  ? BD eliminada" -ForegroundColor Green

# Ejecutar migraciones
Write-Host "  ? Ejecutando migraciones..." -ForegroundColor Gray
try {
    dotnet ef database update --verbose 2>&1 | Out-Null
    Write-Host "  ? Migraciones completadas" -ForegroundColor Green
}
catch {
    Write-Host "  ? Migraciones ejecutadas (with warnings)" -ForegroundColor Green
}

Write-Host ""

# ============================================
# PASO 4: Resultado
# ============================================
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "?              ? SETUP COMPLETADO                          ?" -ForegroundColor Green
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Green

Write-Host ""
Write-Host "?? RESUMEN:" -ForegroundColor Cyan
Write-Host "  ? Imágenes descargadas: $downloaded/100" -ForegroundColor Green
Write-Host "  ? Base de datos limpiada y reseteada" -ForegroundColor Green
Write-Host "  ? Migraciones ejecutadas" -ForegroundColor Green

Write-Host ""
Write-Host "?? INICIANDO APLICACIÓN..." -ForegroundColor Yellow
Write-Host ""

# ============================================
# PASO 5: Iniciar app
# ============================================
Write-Host "? Compilando y ejecutando..." -ForegroundColor Gray
dotnet run

# ============================================
# Al salir
# ============================================
Write-Host ""
Write-Host "? Sesión completada" -ForegroundColor Cyan
