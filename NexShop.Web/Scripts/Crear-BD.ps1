# ============================================
# SCRIPT PARA CREAR BASE DE DATOS NEXSHOP
# ============================================
# Servidor: ADMINISTRADOR\POOL
# Usuario: sa
# Contraseña: 123456
# ============================================

Write-Host "================================" -ForegroundColor Green
Write-Host "CREANDO BASE DE DATOS NEXSHOP" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green
Write-Host ""

# Ruta del script SQL
$sqlScriptPath = "E:\Proyectos Visual\NexShop\NexShop.Web\Scripts\01-CrearBaseDatos.sql"
$serverInstance = "ADMINISTRADOR\POOL"
$username = "sa"
$password = "123456"

# Verificar que el archivo SQL existe
if (-Not (Test-Path $sqlScriptPath)) {
    Write-Host "ERROR: No se encontró el archivo SQL en: $sqlScriptPath" -ForegroundColor Red
    exit 1
}

# Crear conexión a SQL Server
Write-Host "[1/5] Conectando a SQL Server: $serverInstance..." -ForegroundColor Yellow

try {
    # Leer el contenido del archivo SQL
    $sqlScript = Get-Content $sqlScriptPath -Raw
    
    # Ejecutar el script SQL
    Write-Host "[2/5] Ejecutando script SQL..." -ForegroundColor Yellow
    
    Invoke-SqlCmd -ServerInstance $serverInstance `
                  -Username $username `
                  -Password $password `
                  -Query $sqlScript `
                  -ErrorAction Stop
    
    Write-Host "[3/5] Script SQL ejecutado correctamente" -ForegroundColor Green
    
    # Verificar que la BD fue creada
    Write-Host "[4/5] Verificando base de datos..." -ForegroundColor Yellow
    
    $checkQuery = "SELECT COUNT(*) as TableCount FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_CATALOG = 'NexShopDb'"
    $result = Invoke-SqlCmd -ServerInstance $serverInstance `
                            -Username $username `
                            -Password $password `
                            -Database "NexShopDb" `
                            -Query $checkQuery `
                            -ErrorAction Stop
    
    $tableCount = $result.ItemArray[0]
    Write-Host "Tablas creadas: $tableCount" -ForegroundColor Green
    
    Write-Host "[5/5] ¡Base de datos creada exitosamente!" -ForegroundColor Green
    Write-Host ""
    Write-Host "================================" -ForegroundColor Green
    Write-Host "? ÉXITO - BD LISTA PARA USAR" -ForegroundColor Green
    Write-Host "================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Base de Datos: NexShopDb" -ForegroundColor Cyan
    Write-Host "Servidor: $serverInstance" -ForegroundColor Cyan
    Write-Host "Usuario: $username" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Ahora ejecuta: dotnet run" -ForegroundColor Yellow
    
}
catch {
    Write-Host "ERROR al crear la base de datos:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
