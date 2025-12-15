#!/usr/bin/env pwsh
# Script final para ejecutar NexShop con generación de imágenes

Clear-Host

Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?                                                            ?" -ForegroundColor Cyan
Write-Host "?   ? NEXSHOP - GENERADOR AUTOMÁTICO DE IMÁGENES          ?" -ForegroundColor Cyan
Write-Host "?                                                            ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$projectPath = "E:\Proyectos Visual\NexShop\NexShop.Web"

Write-Host "?? Navegando al proyecto..." -ForegroundColor Yellow
Set-Location $projectPath

Write-Host "?? Limpiando base de datos vieja..." -ForegroundColor Yellow
Get-ChildItem -Filter "*.db*" -ErrorAction SilentlyContinue | ForEach-Object {
    Remove-Item $_ -Force -ErrorAction SilentlyContinue
}
Write-Host "? BD limpiada" -ForegroundColor Green

Write-Host ""
Write-Host "?? Iniciando aplicación..." -ForegroundColor Yellow
Write-Host "? La aplicación generará:" -ForegroundColor Gray
Write-Host "   - 50 productos" -ForegroundColor Gray
Write-Host "   - 100 imágenes PNG (400×400)" -ForegroundColor Gray
Write-Host "   - Galería de 2 imágenes por producto" -ForegroundColor Gray
Write-Host ""

dotnet run

Write-Host ""
Write-Host "? Sesión finalizada" -ForegroundColor Cyan
