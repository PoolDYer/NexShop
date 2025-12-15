@echo off
REM ============================================
REM SCRIPT PARA CREAR BASE DE DATOS NEXSHOP
REM ============================================
REM Servidor: ADMINISTRADOR\POOL
REM Usuario: sa
REM Contraseña: 123456
REM ============================================

SETLOCAL ENABLEDELAYEDEXPANSION

echo.
echo ================================
echo CREANDO BASE DE DATOS NEXSHOP
echo ================================
echo.

set "SERVER=ADMINISTRADOR\POOL"
set "USER=sa"
set "PASSWORD=123456"
set "SQLFILE=E:\Proyectos Visual\NexShop\NexShop.Web\Scripts\01-CrearBaseDatos.sql"

REM Verificar que SQL Server está activo
echo [1/4] Verificando SQL Server...
tasklist | find /i "sqlservr" >nul
if errorlevel 1 (
    echo ERROR: SQL Server no está ejecutándose
    echo Inicia el servicio ADMINISTRADOR\POOL
    pause
    exit /b 1
)
echo ? SQL Server está activo

REM Ejecutar script SQL
echo.
echo [2/4] Ejecutando script SQL...
sqlcmd -S %SERVER% -U %USER% -P %PASSWORD% -i "%SQLFILE%"

if errorlevel 1 (
    echo ERROR: Fallo al ejecutar el script SQL
    pause
    exit /b 1
)

REM Verificar base de datos
echo.
echo [3/4] Verificando base de datos...
sqlcmd -S %SERVER% -U %USER% -P %PASSWORD% -d NexShopDb -Q "SELECT COUNT(*) as TablesCreated FROM INFORMATION_SCHEMA.TABLES"

if errorlevel 1 (
    echo ERROR: No se pudo conectar a la base de datos
    pause
    exit /b 1
)

echo.
echo [4/4] ? ¡Base de datos creada exitosamente!
echo.
echo ================================
echo ? ÉXITO - BD LISTA PARA USAR
echo ================================
echo.
echo Base de Datos: NexShopDb
echo Servidor: %SERVER%
echo Usuario: %USER%
echo.
echo Ahora ejecuta: dotnet run
echo.

pause
