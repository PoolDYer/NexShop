@echo off
REM NEXSHOP - LOGIN Y REGISTRO CORREGIDOS
REM Ejecutar aplicacion

color 0A
cls

echo.
echo ====================================================================
echo.
echo     NEXSHOP - CORRECCION DE LOGIN Y REGISTRO
echo.
echo ====================================================================
echo.

echo.
echo CORRECCIONES APLICADAS:
echo.
echo   * Login ahora redirige a Home correctamente
echo   * Registro NO inicia sesion automaticamente
echo   * Usuario debe hacer login despues de registrarse
echo   * Compilacion limpia sin errores
echo.

echo.
echo PRUEBAS RECOMENDADAS:
echo.
echo   1. LOGIN CORRECTO:
echo      - Email: admin@nexshop.com
echo      - Contrasena: Admin@123456
echo      - Resultado: Redirige a Home
echo.
echo   2. REGISTRO:
echo      - Crear nueva cuenta
echo      - Resultado: Redirige a Login
echo.
echo   3. LOGIN NUEVO USUARIO:
echo      - Usar credenciales de registro
echo      - Resultado: Redirige a Home
echo.

echo.
echo Iniciando aplicacion en: http://localhost:5217
echo.

cd /d "E:\Proyectos Visual\NexShop\NexShop.Web"
dotnet run --no-build

pause
