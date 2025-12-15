# Script para sincronizar imágenes de productos
# Ejecuta la sincronización de archivos y multimedia en la BD

Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?  SINCRONIZAR IMÁGENES DE PRODUCTOS A WWWROOT                  ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Rutas
$rutaProjecto = "E:\Proyectos Visual\NexShop\NexShop.Web"
$rutaUploadsCarpetas = "E:\Proyectos Visual\NexShop\NexShop.Web\wwwroot\uploads\productos"
$rutaImagenesDestino = "E:\Proyectos Visual\NexShop\NexShop.Web\wwwroot\imagenes\productos"
$urlApi = "http://localhost:5217"

Write-Host "Rutas detectadas:" -ForegroundColor Yellow
Write-Host "  Proyecto: $rutaProjecto" -ForegroundColor Gray
Write-Host "  Origen (uploads): $rutaUploadsCarpetas" -ForegroundColor Gray
Write-Host "  Destino (imagenes): $rutaImagenesDestino" -ForegroundColor Gray
Write-Host ""

# Crear carpeta destino si no existe
if (-not (Test-Path $rutaImagenesDestino)) {
    Write-Host "?? Creando carpeta de destino..." -ForegroundColor Yellow
    New-Item -Path $rutaImagenesDestino -ItemType Directory -Force | Out-Null
    Write-Host "? Carpeta creada" -ForegroundColor Green
}

# Copiar imágenes
Write-Host ""
Write-Host "?? Copiando imágenes desde carpetas de productos..." -ForegroundColor Yellow

$carpetasProductos = Get-ChildItem -Path $rutaUploadsCarpetas -Directory
$totalCarpetas = $carpetasProductos.Count
$copiadasExitosamente = 0
$errores = 0

foreach ($carpeta in $carpetasProductos) {
    $nombreCarpeta = $carpeta.Name
    $rutaOrigen = $carpeta.FullName
    $rutaDestino = Join-Path $rutaImagenesDestino $nombreCarpeta
    
    # Crear subcarpeta en destino
    if (-not (Test-Path $rutaDestino)) {
        New-Item -Path $rutaDestino -ItemType Directory -Force | Out-Null
    }
    
    # Copiar imágenes
    $imagenes = Get-ChildItem -Path $rutaOrigen -File -Include "*.jpg", "*.jpeg", "*.png", "*.gif", "*.webp" -ErrorAction SilentlyContinue
    
    foreach ($imagen in $imagenes) {
        try {
            $rutaImagenDestino = Join-Path $rutaDestino $imagen.Name
            Copy-Item -Path $imagen.FullName -Destination $rutaImagenDestino -Force
            $copiadasExitosamente++
        } catch {
            $errores++
            Write-Host "? Error copiando: $($imagen.Name)" -ForegroundColor Red
        }
    }
}

Write-Host "? Imágenes copiadas: $copiadasExitosamente" -ForegroundColor Green
if ($errores -gt 0) {
    Write-Host "? Errores: $errores" -ForegroundColor Red
}
Write-Host ""

# Verificar si la aplicación está corriendo
Write-Host "?? Verificando aplicación..." -ForegroundColor Yellow

$appRunning = $false
try {
    $response = Invoke-WebRequest -Uri "$urlApi/api/imagenes/producto/1" -ErrorAction SilentlyContinue
    $appRunning = $response.StatusCode -eq 200 -or $response.StatusCode -eq 404
} catch {
    $appRunning = $false
}

if ($appRunning) {
    Write-Host "? Aplicación detectada en $urlApi" -ForegroundColor Green
    Write-Host ""
    
    # Sincronizar multimedia en BD
    Write-Host "?? Sincronizando multimedia en BD..." -ForegroundColor Yellow
    try {
        $response = Invoke-WebRequest -Uri "$urlApi/api/imagenes/sincronizar-multimedia" -Method POST -ErrorAction SilentlyContinue
        
        if ($response.StatusCode -eq 200) {
            $resultado = $response.Content | ConvertFrom-Json
            Write-Host "? Sincronización de multimedia completada" -ForegroundColor Green
            Write-Host "   • Productos actualizados: $($resultado.productosActualizados)" -ForegroundColor Green
            Write-Host "   • Multimedia agregada: $($resultado.multimediaAgregada)" -ForegroundColor Green
            if ($resultado.errores -gt 0) {
                Write-Host "   • Errores: $($resultado.errores)" -ForegroundColor Yellow
            }
        }
    } catch {
        Write-Host "? Error al sincronizar multimedia: $_" -ForegroundColor Red
    }
} else {
    Write-Host "??  Aplicación no detectada en $urlApi" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Para sincronizar multimedia en BD, ejecuta:" -ForegroundColor Yellow
    Write-Host "  1. Abre una terminal en el proyecto" -ForegroundColor Yellow
    Write-Host "  2. Ejecuta: dotnet run" -ForegroundColor Yellow
    Write-Host "  3. La sincronización se hará automáticamente" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "O usa Postman para hacer una request POST a:" -ForegroundColor Yellow
    Write-Host "  POST $urlApi/api/imagenes/sincronizar-multimedia" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "? SINCRONIZACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

# Verificar carpetas creadas
$carpetasCreadas = (Get-ChildItem -Path $rutaImagenesDestino -Directory -ErrorAction SilentlyContinue).Count
Write-Host "Resumen:" -ForegroundColor Cyan
Write-Host "  • Carpetas de productos en origen: $totalCarpetas" -ForegroundColor Cyan
Write-Host "  • Imágenes copiadas: $copiadasExitosamente" -ForegroundColor Cyan
Write-Host "  • Carpetas en destino: $carpetasCreadas" -ForegroundColor Cyan
Write-Host ""

Write-Host "Próximos pasos:" -ForegroundColor Yellow
Write-Host "  1. Ejecutar la aplicación: dotnet run" -ForegroundColor Yellow
Write-Host "  2. Verificar imágenes en: http://localhost:5217/Productos" -ForegroundColor Yellow
Write-Host "  3. Ver detalles de un producto para ver galería" -ForegroundColor Yellow
Write-Host ""
