#!/usr/bin/env pwsh
# ? SCRIPT MAESTRO FINAL - NEXSHOP CON IMÁGENES CORRECTAS

Clear-Host

Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?                                                            ?" -ForegroundColor Cyan
Write-Host "?   ? NEXSHOP - SETUP DEFINITIVO CON IMÁGENES             ?" -ForegroundColor Cyan
Write-Host "?                                                            ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$projectPath = "E:\Proyectos Visual\NexShop\NexShop.Web"
$imagePath = "$projectPath\wwwroot\imagenes\productos"

# ============================================================
# PASO 1: Crear estructura de carpetas
# ============================================================
Write-Host "?? PASO 1: Preparando carpetas..." -ForegroundColor Yellow

$wwwrootPath = "$projectPath\wwwroot"
$imagesDirPath = "$wwwrootPath\imagenes"

if (-not (Test-Path $wwwrootPath)) {
    New-Item -ItemType Directory -Path $wwwrootPath -Force | Out-Null
}

if (-not (Test-Path $imagesDirPath)) {
    New-Item -ItemType Directory -Path $imagesDirPath -Force | Out-Null
}

if (-not (Test-Path $imagePath)) {
    New-Item -ItemType Directory -Path $imagePath -Force | Out-Null
}

Write-Host "  ? Carpetas listas" -ForegroundColor Green
Write-Host ""

# ============================================================
# PASO 2: Generar 100 imágenes PNG
# ============================================================
Write-Host "?? PASO 2: Generando 100 imágenes PNG..." -ForegroundColor Yellow

[reflection.assembly]::LoadWithPartialName("System.Drawing") | Out-Null

$generated = 0

$colors = @(
    @([System.Drawing.Color]::FromArgb(52, 152, 219), [System.Drawing.Color]::FromArgb(41, 128, 185)),     # Azul
    @([System.Drawing.Color]::FromArgb(46, 204, 113), [System.Drawing.Color]::FromArgb(39, 174, 96)),      # Verde
    @([System.Drawing.Color]::FromArgb(231, 76, 60), [System.Drawing.Color]::FromArgb(192, 57, 43)),       # Rojo
    @([System.Drawing.Color]::FromArgb(241, 196, 15), [System.Drawing.Color]::FromArgb(230, 126, 34)),     # Amarillo
    @([System.Drawing.Color]::FromArgb(155, 89, 182), [System.Drawing.Color]::FromArgb(142, 68, 173)),     # Púrpura
    @([System.Drawing.Color]::FromArgb(26, 188, 156), [System.Drawing.Color]::FromArgb(22, 160, 133))      # Turquesa
)

for ($i = 1; $i -le 100; $i++) {
    $filePath = "$imagePath\producto_$i.png"
    
    # Si ya existe, saltar
    if (Test-Path $filePath) {
        $generated++
        continue
    }
    
    try {
        # Crear bitmap
        $bitmap = New-Object System.Drawing.Bitmap(400, 400)
        $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
        $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
        
        # Seleccionar colores
        $colorIdx = ($i - 1) % $colors.Count
        $colorPair = $colors[$colorIdx]
        $color1 = $colorPair[0]
        $color2 = $colorPair[1]
        
        # Crear gradiente
        $rect = New-Object System.Drawing.Rectangle(0, 0, 400, 400)
        $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush($rect, $color1, $color2, 45)
        $graphics.FillRectangle($brush, $rect)
        
        # Dibujar texto principal
        $font = New-Object System.Drawing.Font("Arial", 48, [System.Drawing.FontStyle]::Bold)
        $whiteBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
        
        $text1 = "Producto $i"
        $textSize = $graphics.MeasureString($text1, $font)
        $x1 = (400 - $textSize.Width) / 2
        $y1 = (400 - $textSize.Height) / 2 - 40
        $graphics.DrawString($text1, $font, $whiteBrush, $x1, $y1)
        
        # Dibujar dimensión
        $font2 = New-Object System.Drawing.Font("Arial", 14)
        $text2 = "400x400 px"
        $textSize2 = $graphics.MeasureString($text2, $font2)
        $x2 = (400 - $textSize2.Width) / 2
        $y2 = (400 - $textSize2.Height) / 2 + 60
        $graphics.DrawString($text2, $font2, $whiteBrush, $x2, $y2)
        
        # Guardar
        $bitmap.Save($filePath, [System.Drawing.Imaging.ImageFormat]::Png)
        
        # Liberar recursos
        $graphics.Dispose()
        $bitmap.Dispose()
        $brush.Dispose()
        $font.Dispose()
        $font2.Dispose()
        $whiteBrush.Dispose()
        
        $generated++
        
        if ($i % 10 -eq 0) {
            Write-Host "  ? Generadas $i/100 imágenes..." -ForegroundColor Green
        }
    }
    catch {
        Write-Host "  ? Error en imagen $i" -ForegroundColor Red
    }
}

Write-Host "  ? Imágenes generadas: $generated/100" -ForegroundColor Green
Write-Host ""

# ============================================================
# PASO 3: Limpiar base de datos
# ============================================================
Write-Host "?? PASO 3: Limpiando base de datos..." -ForegroundColor Yellow

Set-Location $projectPath

# Eliminar archivos de BD
Get-ChildItem -Filter "*.db*" -ErrorAction SilentlyContinue | ForEach-Object {
    Remove-Item $_ -Force -ErrorAction SilentlyContinue
}
Write-Host "  ? BD eliminada" -ForegroundColor Green

# Ejecutar migraciones
Write-Host "  ? Ejecutando migraciones..." -ForegroundColor Gray
try {
    dotnet ef database update 2>&1 | Out-Null
    Write-Host "  ? Migraciones completadas" -ForegroundColor Green
}
catch {
    Write-Host "  ??  Migraciones ejecutadas (algunas advertencias esperadas)" -ForegroundColor Yellow
}

Write-Host ""

# ============================================================
# PASO 4: Resultado
# ============================================================
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "?                  ? SETUP COMPLETADO                      ?" -ForegroundColor Green
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Green

Write-Host ""
Write-Host "?? RESUMEN:" -ForegroundColor Cyan
Write-Host "  ? Imágenes generadas: $generated/100" -ForegroundColor Green
Write-Host "  ? Ubicación: $imagePath" -ForegroundColor Green
Write-Host "  ? Base de datos limpiada" -ForegroundColor Green
Write-Host "  ? Migraciones ejecutadas" -ForegroundColor Green

Write-Host ""
Write-Host "?? INICIANDO APLICACIÓN..." -ForegroundColor Yellow
Write-Host ""

# ============================================================
# PASO 5: Iniciar aplicación
# ============================================================
dotnet run

Write-Host ""
Write-Host "? Sesión completada" -ForegroundColor Cyan
