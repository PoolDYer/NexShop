# NEXSHOP - LOGIN Y REGISTRO CORREGIDOS
# Script para ejecutar la aplicación

Write-Host ""
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?     NEXSHOP - CORRECCION DE LOGIN Y REGISTRO FINALIZADA   ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$projectPath = "E:\Proyectos Visual\NexShop\NexShop.Web"
Set-Location $projectPath

Write-Host "? CORRECCIONES APLICADAS:" -ForegroundColor Green
Write-Host "   • Login redirige a Home correctamente" -ForegroundColor Cyan
Write-Host "   • Registro NO inicia sesion automaticamente" -ForegroundColor Cyan
Write-Host "   • Usuario debe hacer login despues de registrarse" -ForegroundColor Cyan
Write-Host "   • Compilacion limpia sin errores" -ForegroundColor Cyan
Write-Host ""

Write-Host "?? PRUEBAS RECOMENDADAS:" -ForegroundColor Yellow
Write-Host ""
Write-Host "  1. LOGIN CORRECTO:" -ForegroundColor White
Write-Host "     URL: http://localhost:5217/Identity/Account/Login" -ForegroundColor Gray
Write-Host "     Email: admin@nexshop.com" -ForegroundColor Gray
Write-Host "     Contrasena: Admin@123456" -ForegroundColor Gray
Write-Host "     Resultado esperado: Redirige a Home" -ForegroundColor Gray
Write-Host ""
Write-Host "  2. REGISTRO:" -ForegroundColor White
Write-Host "     URL: http://localhost:5217/Identity/Account/Register" -ForegroundColor Gray
Write-Host "     Resultado esperado: Redirige a Login con mensaje" -ForegroundColor Gray
Write-Host ""
Write-Host "  3. LOGIN NUEVO USUARIO:" -ForegroundColor White
Write-Host "     Usar credenciales del registro" -ForegroundColor Gray
Write-Host "     Resultado esperado: Redirige a Home" -ForegroundColor Gray
Write-Host ""

Write-Host "?? ACCESOS:" -ForegroundColor Yellow
Write-Host "   Home:     http://localhost:5217" -ForegroundColor White
Write-Host "   Login:    http://localhost:5217/Identity/Account/Login" -ForegroundColor White
Write-Host "   Register: http://localhost:5217/Identity/Account/Register" -ForegroundColor White
Write-Host ""

Write-Host "?? Iniciando aplicacion..." -ForegroundColor Green
Write-Host ""

dotnet run --no-build
