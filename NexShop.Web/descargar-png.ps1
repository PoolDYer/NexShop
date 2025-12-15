# Script para descargar 100 imágenes PNG simples

$projectPath = "E:\Proyectos Visual\NexShop\NexShop.Web"
$imagePath = "$projectPath\wwwroot\imagenes\productos"

# Crear carpeta
if (-not (Test-Path $imagePath)) {
    New-Item -ItemType Directory -Path $imagePath -Force | Out-Null
    Write-Host "? Carpeta creada: $imagePath" -ForegroundColor Green
}

Write-Host ""
Write-Host "?? Descargando 100 imágenes PNG..." -ForegroundColor Cyan
Write-Host "   Esto toma 30 segundos aproximadamente..." -ForegroundColor Gray

$downloaded = 0

for ($i = 1; $i -le 100; $i++) {
    $fileName = "producto_$i.png"
    $filePath = Join-Path $imagePath $fileName
    
    # Skip if already exists
    if (Test-Path $filePath) {
        $downloaded++
        Write-Host "  ??  $fileName (ya existe)" -ForegroundColor Gray
        continue
    }
    
    try {
        # URL de imagen simple
        $url = "https://via.placeholder.com/400x400.png?text=Producto+$i"
        
        $ProgressPreference = 'SilentlyContinue'
        Invoke-WebRequest -Uri $url -OutFile $filePath -TimeoutSec 5 -ErrorAction SilentlyContinue
        
        if (Test-Path $filePath) {
            $downloaded++
            Write-Host "  ? $fileName" -ForegroundColor Green
        }
        else {
            Write-Host "  ??  $fileName (error al descargar)" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "  ? $fileName (no se pudo descargar)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "?? Resultado:" -ForegroundColor Cyan
Write-Host "  ? Imágenes descargadas: $downloaded/100" -ForegroundColor Green
Write-Host "  ?? Ubicación: $imagePath" -ForegroundColor Gray
Write-Host ""

if ($downloaded -ge 100) {
    Write-Host "? ¡100 imágenes listas!" -ForegroundColor Green
}
else {
    Write-Host "??  Se descargaron $downloaded de 100 imágenes" -ForegroundColor Yellow
    Write-Host "   Algunas pueden no haber descargado correctamente" -ForegroundColor Yellow
}
