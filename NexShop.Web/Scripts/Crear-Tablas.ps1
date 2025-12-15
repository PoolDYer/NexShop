# ============================================
# SCRIPT POWERSHELL - CREAR TODAS LAS TABLAS
# ============================================
# Servidor: ADMINISTRATOR\POOL
# BD: NexShopDb
# Usuario: sa
# Contraseña: 123456
# ============================================

Write-Host ""
Write-Host "================================" -ForegroundColor Green
Write-Host "CREANDO TODAS LAS TABLAS NEXSHOP" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green
Write-Host ""

# Configuración
$sqlScriptPath = "E:\Proyectos Visual\NexShop\NexShop.Web\Scripts\02-CrearTablas-Completo.sql"
$serverInstance = "ADMINISTRATOR\POOL"
$database = "NexShopDb"
$username = "sa"
$password = "123456"

# Verificar que el archivo SQL existe
if (-Not (Test-Path $sqlScriptPath)) {
    Write-Host "ERROR: No se encontró el archivo SQL en: $sqlScriptPath" -ForegroundColor Red
    exit 1
}

try {
    Write-Host "[1/4] Conectando a SQL Server: $serverInstance\$database..." -ForegroundColor Yellow
    
    # Leer el contenido del archivo SQL
    $sqlScript = Get-Content $sqlScriptPath -Raw
    
    Write-Host "[2/4] Ejecutando script SQL completo..." -ForegroundColor Yellow
    Write-Host ""
    
    # Ejecutar el script SQL
    Invoke-SqlCmd -ServerInstance $serverInstance `
                  -Database $database `
                  -Username $username `
                  -Password $password `
                  -Query $sqlScript `
                  -ErrorAction Stop
    
    Write-Host ""
    Write-Host "[3/4] Verificando tablas creadas..." -ForegroundColor Yellow
    
    # Verificar tablas
    $verifyQuery = @"
    SELECT 
        (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo') as [Tablas],
        (SELECT COUNT(*) FROM sys.indexes WHERE object_id > 0 AND name LIKE 'IX_%') as [Índices],
        (SELECT COUNT(*) FROM AspNetRoles) as [Roles],
        (SELECT COUNT(*) FROM Categorias) as [Categorías]
"@
    
    $result = Invoke-SqlCmd -ServerInstance $serverInstance `
                            -Database $database `
                            -Username $username `
                            -Password $password `
                            -Query $verifyQuery `
                            -ErrorAction Stop
    
    $tablas = $result.Tablas
    $indices = $result.Índices
    $roles = $result.Roles
    $categorias = $result.Categorías
    
    Write-Host ""
    Write-Host "[4/4] ? ¡ÉXITO! Resumen de creación:" -ForegroundColor Green
    Write-Host ""
    Write-Host "  ? Tablas creadas: $tablas" -ForegroundColor Cyan
    Write-Host "  ? Índices creados: $indices" -ForegroundColor Cyan
    Write-Host "  ? Roles insertados: $roles" -ForegroundColor Cyan
    Write-Host "  ? Categorías creadas: $categorias" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "================================" -ForegroundColor Green
    Write-Host "? BASE DE DATOS COMPLETAMENTE LISTA" -ForegroundColor Green
    Write-Host "================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Servidor: $serverInstance" -ForegroundColor Cyan
    Write-Host "BD: $database" -ForegroundColor Cyan
    Write-Host "Usuario: $username" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Próximo paso: dotnet run" -ForegroundColor Yellow
    Write-Host ""
    
}
catch {
    Write-Host ""
    Write-Host "ERROR al crear las tablas:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    exit 1
}
