# Script para limpiar la base de datos y recargar con imágenes correctas
# Ejecutar desde PowerShell en la carpeta del proyecto

$projectPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $projectPath

Write-Host "?? Limpiando base de datos..." -ForegroundColor Cyan

# Buscar archivos de BD
$dbFiles = @("*.db", "*.db-shm", "*.db-wal")

foreach ($pattern in $dbFiles) {
    $files = Get-ChildItem -Path $pattern -ErrorAction SilentlyContinue
    if ($files) {
        Remove-Item $files -Force -ErrorAction SilentlyContinue
        Write-Host "  ? Eliminado: $pattern" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "?? Actualizando migraciones..." -ForegroundColor Cyan
dotnet ef database update

Write-Host ""
Write-Host "? Base de datos limpiada y reseteada correctamente" -ForegroundColor Green
Write-Host ""
Write-Host "Ahora inicia la aplicación con:" -ForegroundColor Yellow
Write-Host "  dotnet run" -ForegroundColor Yellow
Write-Host ""
Write-Host "Las imágenes se cargarán con:" -ForegroundColor Cyan
Write-Host "  - 50 productos" -ForegroundColor Cyan
Write-Host "  - 100 imágenes reales" -ForegroundColor Cyan
Write-Host "  - 2 imágenes por producto" -ForegroundColor Cyan
