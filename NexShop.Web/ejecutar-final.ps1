# NEXSHOP - EJECUTAR APLICACION FINAL
# Todas las correcciones aplicadas

Write-Host ""
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?       NEXSHOP - TODAS LAS CORRECCIONES FINALIZADAS         ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$projectPath = "E:\Proyectos Visual\NexShop\NexShop.Web"
Set-Location $projectPath

Write-Host "? Correcciones aplicadas:" -ForegroundColor Green
Write-Host "   • Login redirige correctamente" -ForegroundColor Cyan
Write-Host "   • Usuario se muestra en navbar (boton verde)" -ForegroundColor Cyan
Write-Host "   • Carrito muestra contenido" -ForegroundColor Cyan
Write-Host "   • Encoding de caracteres corregido" -ForegroundColor Cyan
Write-Host "   • Compilacion limpia sin errores" -ForegroundColor Cyan
Write-Host ""

Write-Host "?? Accesos principales:" -ForegroundColor Yellow
Write-Host "   Home:       http://localhost:5217" -ForegroundColor White
Write-Host "   Login:      http://localhost:5217/Identity/Account/Login" -ForegroundColor White
Write-Host "   Carrito:    http://localhost:5217/Carrito" -ForegroundColor White
Write-Host "   Categorias: http://localhost:5217/Categorias" -ForegroundColor White
Write-Host ""

Write-Host "?? Credenciales:" -ForegroundColor Yellow
Write-Host "   Admin: admin@nexshop.com / Admin@123456" -ForegroundColor White
Write-Host ""

Write-Host "?? Iniciando aplicacion..." -ForegroundColor Green
Write-Host ""

dotnet run --no-build
