# ===================================================
# SCRIPT PARA SUBIR NEXSHOP A GITHUB AUTOMÁTICAMENTE
# ===================================================
# Uso: .\subir-a-github.ps1

Write-Host "??????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?  ?? SUBIR NEXSHOP A GITHUB - AUTOMÁTICO   ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Verificar que estamos en la carpeta correcta
if (-not (Test-Path "NexShop.Web\NexShop.Web.csproj")) {
    Write-Host "? Error: No estás en la carpeta raíz de NexShop" -ForegroundColor Red
    Write-Host "   Ejecuta este script desde: E:\Proyectos Visual\NexShop" -ForegroundColor Yellow
    exit 1
}

# Verificar Git
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Host "? Error: Git no está instalado o no está en el PATH" -ForegroundColor Red
    exit 1
}

Write-Host "? Verificaciones previas completadas" -ForegroundColor Green
Write-Host ""

# Solicitar datos
Write-Host "?? CONFIGURACIÓN REQUERIDA:" -ForegroundColor Yellow
$usuario = Read-Host "   Tu usuario de GitHub"
$token = Read-Host "   Tu Personal Access Token (o contraseña)" -AsSecureString

# Convertir SecureString a plain text (solo para este script)
$tokenPlain = [System.Net.NetworkCredential]::new("", $token).Password

Write-Host ""

# Verificar si ya existe repositorio git
if (Test-Path ".git") {
    Write-Host "??  Ya existe un repositorio .git" -ForegroundColor Yellow
    $respuesta = Read-Host "   ¿Deseas continuar? (s/n)"
    if ($respuesta -ne "s") {
        exit 0
    }
} else {
    Write-Host "?? Inicializando repositorio Git..." -ForegroundColor Cyan
    git init
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Error al inicializar git" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host "?? Agregando archivos al repositorio..." -ForegroundColor Cyan
git add .
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Error al agregar archivos" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "?? Estado actual:" -ForegroundColor Cyan
git status

Write-Host ""
$respuesta = Read-Host "   ¿Deseas continuar con el commit? (s/n)"
if ($respuesta -ne "s") {
    Write-Host "??  Operación cancelada" -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "?? Escribiendo mensaje de commit..." -ForegroundColor Cyan
$mensaje = Read-Host "   Mensaje de commit (ej: 'Initial commit: NexShop platform')"

git commit -m $mensaje
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Error al hacer commit" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "?? Configurando repositorio remoto..." -ForegroundColor Cyan

# Verificar si ya existe origin
$remoteUrl = git remote get-url origin 2>$null
if ($remoteUrl) {
    Write-Host "   Repositorio remoto encontrado: $remoteUrl" -ForegroundColor Green
    $respuesta = Read-Host "   ¿Cambiar repositorio remoto? (s/n)"
    if ($respuesta -eq "s") {
        git remote remove origin
    } else {
        Write-Host ""
        Write-Host "??  Usando repositorio existente" -ForegroundColor Yellow
    }
}

if (-not (git remote get-url origin 2>$null)) {
    $repoUrl = "https://${usuario}:${tokenPlain}@github.com/${usuario}/NexShop.git"
    Write-Host "   Agregando: https://github.com/${usuario}/NexShop.git" -ForegroundColor Green
    git remote add origin $repoUrl
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Error al configurar repositorio remoto" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host "Cambiando rama principal a 'main'..." -ForegroundColor Cyan
git branch -M main
if ($LASTEXITCODE -ne 0) {
    Write-Host "??  Advertencia: No se pudo cambiar a rama 'main'" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "?? Subiendo cambios a GitHub..." -ForegroundColor Cyan
git push -u origin main
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Error al subir cambios" -ForegroundColor Red
    Write-Host "   Intenta nuevamente o verifica tu token de acceso" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "??????????????????????????????????????????????" -ForegroundColor Green
Write-Host "?  ? ¡ÉXITO! NEXSHOP ESTÁ EN GITHUB        ?" -ForegroundColor Green
Write-Host "??????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""
Write-Host "?? Tu repositorio:" -ForegroundColor Cyan
Write-Host "   https://github.com/$usuario/NexShop" -ForegroundColor Green
Write-Host ""
Write-Host "?? Próximos pasos:" -ForegroundColor Yellow
Write-Host "   1. Visita tu repositorio en GitHub" -ForegroundColor White
Write-Host "   2. Añade una descripción al repositorio" -ForegroundColor White
Write-Host "   3. Añade temas: aspnetcore, ecommerce, c-sharp" -ForegroundColor White
Write-Host "   4. Activa GitHub Pages para documentación (opcional)" -ForegroundColor White
Write-Host ""
Write-Host "? ¡Listo para colaborar!" -ForegroundColor Cyan
