# ================================================
# SCRIPT AUTOMÁTICO - ACTIVAR IMÁGENES ESPECÍFICAS
# ================================================
# Este script limpia la BD y reinicia la app automáticamente

Write-Host ""
Write-Host "?? ACTIVANDO IMÁGENES ULTRA-ESPECÍFICAS..." -ForegroundColor Cyan
Write-Host ""

# Ruta del proyecto
$projectPath = "E:\Proyectos Visual\NexShop\NexShop.Web"
Set-Location $projectPath

Write-Host "?? Ubicación: $projectPath" -ForegroundColor Yellow
Write-Host ""

# Paso 1: Limpiar multimedia virtual
Write-Host "???  PASO 1: Limpiando imágenes virtuales anteriores..." -ForegroundColor Green

$sqlScript = @"
USE [NexShopDB]
GO

-- Eliminar multimedia virtual
DELETE FROM Multimedia 
WHERE Descripcion LIKE '%generada automáticamente%'
   OR Descripcion LIKE '%Imagen generada%'
   OR NombreArchivo LIKE 'virtual_%'
   OR NombreArchivo LIKE 'temática_%'
   OR NombreArchivo LIKE 'producto_%'
   OR Nombre LIKE 'Imagen Virtual%'
   OR Nombre LIKE 'Imagen Temática%'
GO

-- Verificar
SELECT COUNT(*) as ProductosSinImagenes
FROM Productos p
LEFT JOIN Multimedia m ON p.ProductoId = m.ProductoId
WHERE m.MultimediaId IS NULL
GO
"@

# Guardar script SQL temporal
$tempSqlFile = "$env:TEMP\limpiar_multimedia.sql"
$sqlScript | Out-File -FilePath $tempSqlFile -Encoding UTF8

# Ejecutar script SQL
Write-Host "   Ejecutando limpieza en base de datos..." -ForegroundColor Gray

try {
    # Intentar con diferentes métodos de conexión
    $connectionString = "Server=localhost;Database=NexShopDB;Trusted_Connection=True;TrustServerCertificate=True"
    
    # Usar sqlcmd si está disponible
    $sqlcmdPath = "sqlcmd"
    if (Get-Command sqlcmd -ErrorAction SilentlyContinue) {
        & sqlcmd -S "localhost" -d "NexShopDB" -E -i $tempSqlFile
        Write-Host "   ? Base de datos limpiada exitosamente" -ForegroundColor Green
    } else {
        Write-Host "   ??  sqlcmd no encontrado, continuando..." -ForegroundColor Yellow
        Write-Host "   La limpieza se hará al iniciar la aplicación" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ??  No se pudo conectar a BD ahora, se limpiará al iniciar app" -ForegroundColor Yellow
}

Write-Host ""

# Paso 2: Reiniciar aplicación
Write-Host "?? PASO 2: Iniciando aplicación con imágenes específicas..." -ForegroundColor Green
Write-Host ""
Write-Host "   La aplicación generará automáticamente:" -ForegroundColor Cyan
Write-Host "   • 50 productos con imágenes específicas" -ForegroundColor White
Write-Host "   • Cinturón de Cuero ? leather-belt" -ForegroundColor White
Write-Host "   • Zapatos Deportivos ? running-shoes" -ForegroundColor White
Write-Host "   • Banda Elástica ? resistance-band" -ForegroundColor White
Write-Host "   • Guantes de Boxeo ? boxing-gloves" -ForegroundColor White
Write-Host "   • Y 46 productos más..." -ForegroundColor White
Write-Host ""

Write-Host "? Espera 30 segundos para la sincronización..." -ForegroundColor Yellow
Write-Host ""

# Iniciar aplicación
Write-Host "??  Iniciando dotnet run..." -ForegroundColor Cyan
Write-Host ""
Write-Host "?? Después de ver 'Now listening on: http://localhost:5217'" -ForegroundColor Magenta
Write-Host "   Abre el navegador en:" -ForegroundColor Magenta
Write-Host "   http://localhost:5217/Productos" -ForegroundColor White
Write-Host ""
Write-Host "?? Verifica que cada producto muestre su imagen específica" -ForegroundColor Green
Write-Host ""

# Ejecutar aplicación
dotnet run