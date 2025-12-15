# Script de verificación para NexShop
# Uso: .\verificar-nexshop.ps1

Write-Host "?????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?        VERIFICACIÓN DE APLICACIÓN NEXSHOP             ?" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Variables
$projectPath = "E:\Proyectos Visual\NexShop\NexShop.Web"
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

# 1. VERIFICAR RUTA
Write-Host "1??  Verificando ruta del proyecto..." -ForegroundColor Yellow
if (Test-Path $projectPath) {
    Write-Host "? Ruta válida: $projectPath" -ForegroundColor Green
} else {
    Write-Host "? Ruta no encontrada: $projectPath" -ForegroundColor Red
    exit
}
Write-Host ""

# 2. VERIFICAR ARCHIVOS CRÍTICOS
Write-Host "2??  Verificando archivos críticos..." -ForegroundColor Yellow
$archivos = @(
    "Program.cs",
    "appsettings.json",
    "NexShop.Web.csproj",
    "Models/NexShopContext.cs",
    "Migrations/20251127171255_InitialCreate.cs"
)

foreach ($archivo in $archivos) {
    $ruta = Join-Path $projectPath $archivo
    if (Test-Path $ruta) {
        Write-Host "? $archivo" -ForegroundColor Green
    } else {
        Write-Host "? Falta: $archivo" -ForegroundColor Red
    }
}
Write-Host ""

# 3. LIMPIAR Y COMPILAR
Write-Host "3??  Compilando proyecto..." -ForegroundColor Yellow
Set-Location $projectPath
Write-Host "  Ejecutando: dotnet build" -ForegroundColor Cyan

$buildResult = dotnet build 2>&1
$buildSuccess = $LASTEXITCODE -eq 0

if ($buildSuccess) {
    Write-Host "? Compilación exitosa" -ForegroundColor Green
} else {
    Write-Host "? Error en compilación" -ForegroundColor Red
    Write-Host $buildResult
    exit
}
Write-Host ""

# 4. VERIFICAR CARPETAS DE VISTAS
Write-Host "4??  Verificando vistas..." -ForegroundColor Yellow
$vistas = @(
    "Views/Categorias/Index.cshtml",
    "Views/Categorias/Create.cshtml",
    "Views/Categorias/Edit.cshtml",
    "Views/Categorias/Delete.cshtml",
    "Areas/Identity/Pages/Account/Login.cshtml",
    "Areas/Identity/Pages/Account/Register.cshtml"
)

foreach ($vista in $vistas) {
    $ruta = Join-Path $projectPath $vista
    if (Test-Path $ruta) {
        Write-Host "? $vista" -ForegroundColor Green
    } else {
        Write-Host "? Falta: $vista" -ForegroundColor Red
    }
}
Write-Host ""

# 5. VERIFICAR BASE DE DATOS
Write-Host "5??  Verificando base de datos..." -ForegroundColor Yellow
Write-Host "  Verificando credenciales SQL Server..." -ForegroundColor Cyan
$connString = "Server=ADMINISTRATOR\MSSQLSERVER2025;Database=master;User Id=sa;Password=123456;Encrypt=true;TrustServerCertificate=true;"

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = $connString
    $connection.Open()
    Write-Host "? Conexión a SQL Server exitosa" -ForegroundColor Green
    $connection.Close()
} catch {
    Write-Host "? Error conectando a SQL Server: $_" -ForegroundColor Red
    Write-Host "  Asegúrate que:" -ForegroundColor Yellow
    Write-Host "    - SQL Server MSSQLSERVER2025 está corriendo" -ForegroundColor Yellow
    Write-Host "    - Las credenciales son correctas" -ForegroundColor Yellow
}
Write-Host ""

# 6. VERIFICAR MODELOS
Write-Host "6??  Verificando modelos..." -ForegroundColor Yellow
$modelos = @(
    "Models/Usuario.cs",
    "Models/Producto.cs",
    "Models/Categoria.cs",
    "Models/Pregunta.cs",
    "Models/Respuesta.cs",
    "Models/Calificacion.cs"
)

foreach ($modelo in $modelos) {
    $ruta = Join-Path $projectPath $modelo
    if (Test-Path $ruta) {
        Write-Host "? $modelo" -ForegroundColor Green
    } else {
        Write-Host "? Falta: $modelo" -ForegroundColor Red
    }
}
Write-Host ""

# 7. VERIFICAR CONTROLADORES
Write-Host "7??  Verificando controladores..." -ForegroundColor Yellow
$controladores = @(
    "Controllers/CategoriasController.cs",
    "Controllers/ProductosController.cs",
    "Controllers/CarritoController.cs",
    "Controllers/PreguntasController.cs"
)

foreach ($controlador in $controladores) {
    $ruta = Join-Path $projectPath $controlador
    if (Test-Path $ruta) {
        Write-Host "? $controlador" -ForegroundColor Green
    } else {
        Write-Host "? Falta: $controlador" -ForegroundColor Red
    }
}
Write-Host ""

# 8. INFORMACIÓN DE CREDENCIALES
Write-Host "8??  Credenciales de prueba..." -ForegroundColor Yellow
Write-Host "  ?? Admin:" -ForegroundColor Cyan
Write-Host "     Email: admin@nexshop.com" -ForegroundColor White
Write-Host "     Contraseña: Admin@123456" -ForegroundColor White
Write-Host "  ?? Vendedor:" -ForegroundColor Cyan
Write-Host "     Email: vendedor@nexshop.com" -ForegroundColor White
Write-Host "     Contraseña: Vendedor@123456" -ForegroundColor White
Write-Host "  ?? Comprador:" -ForegroundColor Cyan
Write-Host "     Email: comprador@nexshop.com" -ForegroundColor White
Write-Host "     Contraseña: Comprador@123456" -ForegroundColor White
Write-Host ""

# 9. INFORMACIÓN DE ACCESO
Write-Host "9??  Accesos a la aplicación..." -ForegroundColor Yellow
Write-Host "  ?? Principal:" -ForegroundColor Cyan
Write-Host "     http://localhost:5217" -ForegroundColor White
Write-Host "  ?? Login:" -ForegroundColor Cyan
Write-Host "     http://localhost:5217/Identity/Account/Login" -ForegroundColor White
Write-Host "  ?? Registro:" -ForegroundColor Cyan
Write-Host "     http://localhost:5217/Identity/Account/Register" -ForegroundColor White
Write-Host "  ?? Categorías (Admin):" -ForegroundColor Cyan
Write-Host "     http://localhost:5217/Categorias" -ForegroundColor White
Write-Host ""

# 10. RESUMEN FINAL
Write-Host "?????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "?   ? VERIFICACIÓN COMPLETADA                           ?" -ForegroundColor Green
Write-Host "?????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

Write-Host "?? Para ejecutar la aplicación:" -ForegroundColor Cyan
Write-Host "   cd '$projectPath'" -ForegroundColor White
Write-Host "   dotnet run" -ForegroundColor White
Write-Host ""

Write-Host "?? Información del sistema:" -ForegroundColor Yellow
Write-Host "   Timestamp: $timestamp" -ForegroundColor White
Write-Host "   PowerShell: $($PSVersionTable.PSVersion)" -ForegroundColor White
Write-Host "   .NET: $(dotnet --version)" -ForegroundColor White
Write-Host ""

Write-Host "? ¡La aplicación está lista para usar!" -ForegroundColor Green
