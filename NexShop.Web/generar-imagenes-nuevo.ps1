#!/usr/bin/env pwsh

# Script definitivo para generar 100 imágenes PNG usando .NET

$projectPath = "E:\Proyectos Visual\NexShop\NexShop.Web"
$imagePath = "$projectPath\wwwroot\imagenes\productos"

Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?  ?? GENERADOR DE IMÁGENES PNG - NEXSHOP                  ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Crear carpeta
if (-not (Test-Path $imagePath)) {
    New-Item -ItemType Directory -Path $imagePath -Force | Out-Null
    Write-Host "? Carpeta creada: $imagePath" -ForegroundColor Green
}

Write-Host ""
Write-Host "?? Generando código C# para crear imágenes..." -ForegroundColor Yellow

# Crear archivo temporal .csx
$csharpScript = @"
using System.Drawing;
using System.Drawing.Imaging;

var imagePath = "$imagePath";

// Colores para degradados
var colors = new[]
{
    new[] { Color.FromArgb(52, 152, 219), Color.FromArgb(41, 128, 185) },     // Azul
    new[] { Color.FromArgb(46, 204, 113), Color.FromArgb(39, 174, 96) },      // Verde
    new[] { Color.FromArgb(231, 76, 60), Color.FromArgb(192, 57, 43) },       // Rojo
    new[] { Color.FromArgb(241, 196, 15), Color.FromArgb(230, 126, 34) },     // Amarillo
    new[] { Color.FromArgb(155, 89, 182), Color.FromArgb(142, 68, 173) },     // Púrpura
    new[] { Color.FromArgb(26, 188, 156), Color.FromArgb(22, 160, 133) },     // Turquesa
    new[] { Color.FromArgb(149, 165, 166), Color.FromArgb(127, 140, 141) }    // Gris
};

for (int i = 1; i <= 100; i++)
{
    var filePath = Path.Combine(imagePath, $"producto_{i}.png");
    
    // Crear bitmap
    using (var bitmap = new Bitmap(400, 400))
    using (var graphics = Graphics.FromImage(bitmap))
    {
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        
        // Seleccionar colores
        var colorPair = colors[(i - 1) % colors.Length];
        
        // Crear gradiente
        var rect = new Rectangle(0, 0, 400, 400);
        using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
            rect, colorPair[0], colorPair[1], 45f))
        {
            graphics.FillRectangle(brush, rect);
        }
        
        // Dibujar texto
        using (var font = new Font("Arial", 48, FontStyle.Bold))
        using (var whiteBrush = new SolidBrush(Color.White))
        {
            var textSize = graphics.MeasureString($"Producto {i}", font);
            var x = (400 - textSize.Width) / 2;
            var y = (400 - textSize.Height) / 2 - 40;
            
            graphics.DrawString($"Producto {i}", font, whiteBrush, x, y);
        }
        
        // Dibujar dimensión
        using (var font = new Font("Arial", 14))
        using (var whiteBrush = new SolidBrush(Color.White))
        {
            var textSize = graphics.MeasureString("400x400 px", font);
            var x = (400 - textSize.Width) / 2;
            var y = (400 - textSize.Height) / 2 + 60;
            
            graphics.DrawString("400x400 px", font, whiteBrush, x, y);
        }
        
        // Guardar
        bitmap.Save(filePath, ImageFormat.Png);
    }
    
    if (i % 10 == 0)
        Console.WriteLine($"? Generadas {i}/100 imágenes...");
}

Console.WriteLine($"? ¡100 imágenes PNG generadas exitosamente!");
"@

# Guardar script
$scriptPath = "$projectPath\generar-imagenes.csx"
$csharpScript | Out-File -FilePath $scriptPath -Encoding UTF8

Write-Host "? Script C# guardado" -ForegroundColor Green
Write-Host ""
Write-Host "? Ejecutando generador..." -ForegroundColor Yellow

# Ejecutar script
try {
    $output = & dotnet script $scriptPath 2>&1
    Write-Host $output -ForegroundColor Green
    Write-Host ""
    Write-Host "? Imágenes generadas correctamente" -ForegroundColor Green
}
catch {
    Write-Host "? Error al ejecutar script" -ForegroundColor Red
    Write-Host "Intentando alternativa..." -ForegroundColor Yellow
    
    # Alternativa: usar PowerShell puro
    Write-Host "?? Generando imágenes con PowerShell..." -ForegroundColor Cyan
    
    [reflection.assembly]::LoadWithPartialName("System.Drawing") | Out-Null
    
    for ($i = 1; $i -le 100; $i++) {
        $filePath = "$imagePath\producto_$i.png"
        
        if (Test-Path $filePath) {
            Write-Host "  ??  Imagen $i ya existe" -ForegroundColor Gray
            continue
        }
        
        try {
            $bitmap = New-Object System.Drawing.Bitmap(400, 400)
            $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
            
            $colors = @(
                @([System.Drawing.Color]::FromArgb(52, 152, 219), [System.Drawing.Color]::FromArgb(41, 128, 185)),
                @([System.Drawing.Color]::FromArgb(46, 204, 113), [System.Drawing.Color]::FromArgb(39, 174, 96)),
                @([System.Drawing.Color]::FromArgb(231, 76, 60), [System.Drawing.Color]::FromArgb(192, 57, 43)),
                @([System.Drawing.Color]::FromArgb(241, 196, 15), [System.Drawing.Color]::FromArgb(230, 126, 34)),
                @([System.Drawing.Color]::FromArgb(155, 89, 182), [System.Drawing.Color]::FromArgb(142, 68, 173)),
                @([System.Drawing.Color]::FromArgb(26, 188, 156), [System.Drawing.Color]::FromArgb(22, 160, 133))
            )
            
            $colorPair = $colors[($i - 1) % $colors.Count]
            $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
                (New-Object System.Drawing.Rectangle(0, 0, 400, 400)),
                $colorPair[0],
                $colorPair[1],
                45
            )
            
            $graphics.FillRectangle($brush, 0, 0, 400, 400)
            
            $font = New-Object System.Drawing.Font("Arial", 48, [System.Drawing.FontStyle]::Bold)
            $brush2 = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
            $graphics.DrawString("Producto $i", $font, $brush2, 50, 150)
            
            $bitmap.Save($filePath, [System.Drawing.Imaging.ImageFormat]::Png)
            
            $graphics.Dispose()
            $bitmap.Dispose()
            
            Write-Host "  ? Imagen $i/100" -ForegroundColor Green
        }
        catch {
            Write-Host "  ? Error en imagen $i" -ForegroundColor Red
        }
    }
}

# Limpiar
Remove-Item $scriptPath -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "? GENERACIÓN COMPLETADA" -ForegroundColor Green
Write-Host ""
Write-Host "?? Imágenes guardadas en:" -ForegroundColor Cyan
Write-Host "   $imagePath" -ForegroundColor Gray
Write-Host ""
Write-Host "Próximos pasos:" -ForegroundColor Yellow
Write-Host "  1. Limpia la BD: Remove-Item '*.db*' -Force" -ForegroundColor Yellow
Write-Host "  2. Actualiza: dotnet ef database update" -ForegroundColor Yellow
Write-Host "  3. Ejecuta: dotnet run" -ForegroundColor Yellow
