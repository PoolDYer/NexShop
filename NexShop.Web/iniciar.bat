@echo off
REM NEXSHOP - APLICACION FINAL LISTA
REM Todas las correcciones aplicadas

color 0A
cls

echo.
echo ====================================================================
echo.
echo     NEXSHOP - APLICACION LISTA PARA USAR
echo.
echo ====================================================================
echo.

echo.
echo Correcciones aplicadas:
echo  * Login redirige correctamente
echo  * Usuario se muestra en navbar
echo  * Carrito muestra contenido
echo  * Encoding corregido
echo  * Compilacion limpia
echo.

echo.
echo Iniciando aplicacion en: http://localhost:5217
echo.

cd /d "E:\Proyectos Visual\NexShop\NexShop.Web"
dotnet run --no-build

pause
