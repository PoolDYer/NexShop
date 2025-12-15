@echo off
REM ============================================
REM SCRIPT PARA CREAR TODAS LAS TABLAS NEXSHOP
REM ============================================
REM Servidor: ADMINISTRATOR\POOL
REM BD: NexShopDb
REM Usuario: sa
REM Contraseña: 123456
REM ============================================

SETLOCAL ENABLEDELAYEDEXPANSION

echo.
echo ================================
echo CREANDO TODAS LAS TABLAS NEXSHOP
echo ================================
echo.

set "SERVER=ADMINISTRATOR\POOL"
set "DATABASE=NexShopDb"
set "USER=sa"
set "PASSWORD=123456"
set "SQLFILE=E:\Proyectos Visual\NexShop\NexShop.Web\Scripts\02-CrearTablas-Completo.sql"

REM Verificar que SQL Server está activo
echo [1/4] Verificando SQL Server...
tasklist | find /i "sqlservr" >nul
if errorlevel 1 (
    echo ERROR: SQL Server no está ejecutándose
    echo Inicia el servicio ADMINISTRATOR\POOL
    echo.
    pause
    exit /b 1
)
echo ? SQL Server está activo

REM Ejecutar script SQL
echo.
echo [2/4] Ejecutando script SQL completo...
echo.
sqlcmd -S %SERVER% -d %DATABASE% -U %USER% -P %PASSWORD% -i "%SQLFILE%"

if errorlevel 1 (
    echo.
    echo ERROR: Fallo al ejecutar el script SQL
    echo.
    pause
    exit /b 1
)

REM Verificar tablas
echo.
echo [3/4] Verificando tablas creadas...
sqlcmd -S %SERVER% -d %DATABASE% -U %USER% -P %PASSWORD% -Q "SELECT 'Tablas: ' + CAST(COUNT(*) AS VARCHAR(10)) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo'"

if errorlevel 1 (
    echo.
    echo ERROR: No se pudo conectar a la base de datos
    echo.
    pause
    exit /b 1
)

echo.
echo [4/4] ? ¡ÉXITO! Tablas creadas completamente
echo.
echo ================================
echo ? BASE DE DATOS COMPLETAMENTE LISTA
echo ================================
echo.
echo Servidor: %SERVER%
echo BD: %DATABASE%
echo Usuario: %USER%
echo.
echo Próximo paso: dotnet run
echo.

pause
