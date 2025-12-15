# Script para descargar imágenes locales
# Descarga 100 imágenes y las organiza en wwwroot/imagenes/productos

$projectPath = "E:\Proyectos Visual\NexShop\NexShop.Web"
$imagePath = "$projectPath\wwwroot\imagenes\productos"

# Crear carpeta si no existe
if (-not (Test-Path $imagePath)) {
    New-Item -ItemType Directory -Path $imagePath -Force | Out-Null
    Write-Host "? Carpeta creada: $imagePath" -ForegroundColor Green
}

Write-Host ""
Write-Host "?? Descargando 100 imágenes..." -ForegroundColor Cyan
Write-Host "Esto puede tomar 1-2 minutos..." -ForegroundColor Yellow

$downloaded = 0
$failed = 0

# Descargar 100 imágenes
for ($i = 1; $i -le 100; $i++) {
    $fileName = "producto_$i.jpg"
    $filePath = Join-Path $imagePath $fileName
    
    # URLs aleatorias de servicios confiables
    $urls = @(
        "https://picsum.photos/400/400?random=$i",
        "https://loremflickr.com/400/400?random=$i",
        "https://source.unsplash.com/400x400/?product&random=$i"
    )
    
    $success = $false
    
    foreach ($url in $urls) {
        try {
            Invoke-WebRequest -Uri $url -OutFile $filePath -TimeoutSec 10 -ErrorAction SilentlyContinue
            
            if (Test-Path $filePath) {
                $success = $true
                $downloaded++
                Write-Host "  $i. ? $fileName" -ForegroundColor Green
                break
            }
        }
        catch {
            # Intentar siguiente URL
        }
    }
    
    if (-not $success) {
        $failed++
        Write-Host "  $i. ? Error descargando" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "?? Resultado:" -ForegroundColor Cyan
Write-Host "  ? Descargadas: $downloaded" -ForegroundColor Green
Write-Host "  ? Fallidas: $failed" -ForegroundColor Yellow
Write-Host ""
Write-Host "? Imágenes guardadas en: $imagePath" -ForegroundColor Green
