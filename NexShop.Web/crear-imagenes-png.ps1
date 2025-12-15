# Script rápido para crear imágenes placeholder PNG locales

$projectPath = "E:\Proyectos Visual\NexShop\NexShop.Web"
$imagePath = "$projectPath\wwwroot\imagenes\productos"

# Crear carpeta
if (-not (Test-Path $imagePath)) {
    New-Item -ItemType Directory -Path $imagePath -Force | Out-Null
}

Write-Host "?? Creando 100 imágenes placeholder..." -ForegroundColor Cyan

# Crear imágenes PNG simples usando System.Drawing
$assembly = [reflection.Assembly]::LoadWithPartialName("System.Drawing")

for ($i = 1; $i -le 100; $i++) {
    $filePath = "$imagePath\producto_$i.png"
    
    # Crear bitmap 400x400
    $bitmap = New-Object System.Drawing.Bitmap(400, 400)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    
    # Colores aleatorios
    $colors = @(
        [System.Drawing.Color]::FromArgb(52, 152, 219),    # Azul
        [System.Drawing.Color]::FromArgb(46, 204, 113),    # Verde
        [System.Drawing.Color]::FromArgb(231, 76, 60),     # Rojo
        [System.Drawing.Color]::FromArgb(241, 196, 15),    # Amarillo
        [System.Drawing.Color]::FromArgb(155, 89, 182),    # Púrpura
        [System.Drawing.Color]::FromArgb(26, 188, 156),    # Turquesa
        [System.Drawing.Color]::FromArgb(230, 126, 34)     # Naranja
    )
    
    $randomColor = $colors[($i - 1) % $colors.Count]
    
    # Llenar con color
    $brush = New-Object System.Drawing.SolidBrush($randomColor)
    $graphics.FillRectangle($brush, 0, 0, 400, 400)
    
    # Agregar texto
    $font = New-Object System.Drawing.Font("Arial", 32, [System.Drawing.FontStyle]::Bold)
    $textBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
    $stringFormat = New-Object System.Drawing.StringFormat
    $stringFormat.Alignment = [System.Drawing.StringAlignment]::Center
    $stringFormat.LineAlignment = [System.Drawing.StringAlignment]::Center
    
    $graphics.DrawString("Producto $i", $font, $textBrush, 200, 180, $stringFormat)
    $graphics.DrawString("400x400", $font, $textBrush, 200, 240, $stringFormat)
    
    # Guardar
    $bitmap.Save($filePath, [System.Drawing.Imaging.ImageFormat]::Png)
    
    $graphics.Dispose()
    $bitmap.Dispose()
    $brush.Dispose()
    $textBrush.Dispose()
    
    Write-Host "  ? Imagen $i/100 creada" -ForegroundColor Green
}

Write-Host ""
Write-Host "? 100 imágenes PNG creadas exitosamente" -ForegroundColor Green
Write-Host "   Ubicación: $imagePath" -ForegroundColor Gray
