#!/usr/bin/env pwsh
# Script para descargar imágenes reales de productos desde Unsplash y otras fuentes

$projectPath = "E:\Proyectos Visual\NexShop\NexShop.Web"
$imagePath = "$projectPath\wwwroot\imagenes\productos"

Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?   ?? DESCARGADOR DE IMÁGENES REALES - NEXSHOP            ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Crear carpeta si no existe
if (-not (Test-Path $imagePath)) {
    New-Item -ItemType Directory -Path $imagePath -Force | Out-Null
    Write-Host "? Carpeta creada" -ForegroundColor Green
}

# Diccionario de productos con keywords para búsqueda en Unsplash
$productos = @{
    # ELECTRÓNICA (1-20)
    1 = "smartphone";      2 = "smartphone";
    3 = "laptop";         4 = "laptop";
    5 = "tablet";         6 = "tablet";
    7 = "headphones";     8 = "headphones";
    9 = "monitor";        10 = "monitor";
    11 = "keyboard";      12 = "keyboard";
    13 = "mouse";         14 = "mouse";
    15 = "webcam";        16 = "webcam";
    17 = "charger";       18 = "charger";
    19 = "powerbank";     20 = "powerbank";
    
    # ROPA (21-40)
    21 = "t-shirt";       22 = "t-shirt";
    23 = "jeans";         24 = "jeans";
    25 = "hoodie";        26 = "hoodie";
    27 = "leather-jacket"; 28 = "leather-jacket";
    29 = "sports-pants";  30 = "sports-pants";
    31 = "socks";         32 = "socks";
    33 = "hat";           34 = "hat";
    35 = "scarf";         36 = "scarf";
    37 = "sneakers";      38 = "sneakers";
    39 = "belt";          40 = "belt";
    
    # HOGAR (41-60)
    41 = "bed-sheets";    42 = "bed-sheets";
    43 = "pillow";        44 = "pillow";
    45 = "comforter";     46 = "comforter";
    47 = "curtains";      48 = "curtains";
    49 = "desk-lamp";     50 = "desk-lamp";
    51 = "mirror";        52 = "mirror";
    53 = "carpet";        54 = "carpet";
    55 = "towels";        56 = "towels";
    57 = "bathroom-rug";  58 = "bathroom-rug";
    59 = "plants";        60 = "plants";
    
    # DEPORTES (61-80)
    61 = "soccer-ball";   62 = "soccer-ball";
    63 = "tennis-racket"; 64 = "tennis-racket";
    65 = "basketball";    66 = "basketball";
    67 = "dumbbells";     68 = "dumbbells";
    69 = "yoga-mat";      70 = "yoga-mat";
    71 = "resistance-bands"; 72 = "resistance-bands";
    73 = "water-bottle";  74 = "water-bottle";
    75 = "boxing-gloves"; 76 = "boxing-gloves";
    77 = "measuring-tape"; 78 = "measuring-tape";
    79 = "sports-uniform"; 80 = "sports-uniform";
    
    # LIBROS (81-100)
    81 = "book";          82 = "book";
    83 = "book";          84 = "book";
    85 = "book";          86 = "book";
    87 = "book";          88 = "book";
    89 = "book";          90 = "book";
    91 = "book";          92 = "book";
    93 = "book";          94 = "book";
    95 = "book";          96 = "book";
    97 = "book";          98 = "book";
    99 = "book";          100 = "book";
}

$downloaded = 0
$failed = 0

Write-Host "?? Descargando 100 imágenes reales..." -ForegroundColor Yellow
Write-Host ""

# Descargar imágenes
foreach ($i in 1..100) {
    $fileName = "producto_$i.jpg"
    $filePath = Join-Path $imagePath $fileName
    
    if (Test-Path $filePath) {
        Write-Host "  ??  $i. Ya existe" -ForegroundColor Gray
        $downloaded++
        continue
    }
    
    $keyword = $productos[$i]
    $url = "https://source.unsplash.com/400x400/?$keyword,product&sig=$i"
    
    try {
        $ProgressPreference = 'SilentlyContinue'
        Invoke-WebRequest -Uri $url -OutFile $filePath -TimeoutSec 10 -ErrorAction SilentlyContinue
        
        if (Test-Path $filePath) {
            $fileSize = (Get-Item $filePath).Length / 1KB
            Write-Host "  ? $i. Descargado - $keyword ($([Math]::Round($fileSize))KB)" -ForegroundColor Green
            $downloaded++
        }
        else {
            Write-Host "  ? $i. Error descargando $keyword" -ForegroundColor Red
            $failed++
        }
    }
    catch {
        Write-Host "  ? $i. Error con $keyword" -ForegroundColor Red
        $failed++
    }
    
    # Pequeña pausa para no sobrecargar el servidor
    Start-Sleep -Milliseconds 200
}

Write-Host ""
Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?? RESULTADO:" -ForegroundColor Cyan
Write-Host "  ? Descargadas: $downloaded/100" -ForegroundColor Green
Write-Host "  ? Fallidas: $failed/100" -ForegroundColor Yellow
Write-Host ""
Write-Host "?? Ubicación: $imagePath" -ForegroundColor Gray
Write-Host ""

if ($downloaded -ge 80) {
    Write-Host "? Descarga exitosa - Procede a ejecutar la app" -ForegroundColor Green
}
else {
    Write-Host "??  Solo se descargaron $downloaded imágenes" -ForegroundColor Yellow
    Write-Host "   Intenta nuevamente en unos minutos" -ForegroundColor Yellow
}
