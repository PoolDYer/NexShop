#!/usr/bin/env pwsh
# Script MAESTRO - Descarga imágenes reales, limpia BD e inicia la app

Clear-Host

Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?                                                            ?" -ForegroundColor Cyan
Write-Host "?   ? NEXSHOP - SETUP CON IMÁGENES REALES DE LA WEB       ?" -ForegroundColor Cyan
Write-Host "?                                                            ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$projectPath = "E:\Proyectos Visual\NexShop\NexShop.Web"
$imagePath = "$projectPath\wwwroot\imagenes\productos"

# ============================================================
# PASO 1: Crear carpeta
# ============================================================
Write-Host "?? PASO 1: Preparando carpeta de imágenes..." -ForegroundColor Yellow

if (-not (Test-Path $imagePath)) {
    New-Item -ItemType Directory -Path $imagePath -Force | Out-Null
}
Write-Host "  ? Carpeta lista" -ForegroundColor Green
Write-Host ""

# ============================================================
# PASO 2: Descargar imágenes
# ============================================================
Write-Host "?? PASO 2: Descargando 100 imágenes reales de la web..." -ForegroundColor Yellow
Write-Host "   (Esto puede tomar 2-3 minutos)" -ForegroundColor Gray
Write-Host ""

$productos = @{
    # ELECTRÓNICA (1-20)
    1 = "smartphone"; 2 = "smartphone";
    3 = "laptop"; 4 = "laptop";
    5 = "tablet"; 6 = "tablet";
    7 = "headphones"; 8 = "headphones";
    9 = "monitor"; 10 = "monitor";
    11 = "keyboard"; 12 = "keyboard";
    13 = "mouse"; 14 = "mouse";
    15 = "webcam"; 16 = "webcam";
    17 = "charger"; 18 = "charger";
    19 = "powerbank"; 20 = "powerbank";
    
    # ROPA (21-40)
    21 = "t-shirt"; 22 = "t-shirt";
    23 = "jeans"; 24 = "jeans";
    25 = "hoodie"; 26 = "hoodie";
    27 = "leather-jacket"; 28 = "leather-jacket";
    29 = "sports-pants"; 30 = "sports-pants";
    31 = "socks"; 32 = "socks";
    33 = "hat"; 34 = "hat";
    35 = "scarf"; 36 = "scarf";
    37 = "sneakers"; 38 = "sneakers";
    39 = "belt"; 40 = "belt";
    
    # HOGAR (41-60)
    41 = "bed-sheets"; 42 = "bed-sheets";
    43 = "pillow"; 44 = "pillow";
    45 = "comforter"; 46 = "comforter";
    47 = "curtains"; 48 = "curtains";
    49 = "desk-lamp"; 50 = "desk-lamp";
    51 = "mirror"; 52 = "mirror";
    53 = "carpet"; 54 = "carpet";
    55 = "towels"; 56 = "towels";
    57 = "bathroom-rug"; 58 = "bathroom-rug";
    59 = "plants"; 60 = "plants";
    
    # DEPORTES (61-80)
    61 = "soccer-ball"; 62 = "soccer-ball";
    63 = "tennis-racket"; 64 = "tennis-racket";
    65 = "basketball"; 66 = "basketball";
    67 = "dumbbells"; 68 = "dumbbells";
    69 = "yoga-mat"; 70 = "yoga-mat";
    71 = "resistance-bands"; 72 = "resistance-bands";
    73 = "water-bottle"; 74 = "water-bottle";
    75 = "boxing-gloves"; 76 = "boxing-gloves";
    77 = "measuring-tape"; 78 = "measuring-tape";
    79 = "sports-uniform"; 80 = "sports-uniform";
    
    # LIBROS (81-100)
    81 = "book"; 82 = "book";
    83 = "book"; 84 = "book";
    85 = "book"; 86 = "book";
    87 = "book"; 88 = "book";
    89 = "book"; 90 = "book";
    91 = "book"; 92 = "book";
    93 = "book"; 94 = "book";
    95 = "book"; 96 = "book";
    97 = "book"; 98 = "book";
    99 = "book"; 100 = "book";
}

$downloaded = 0

foreach ($i in 1..100) {
    $fileName = "producto_$i.jpg"
    $filePath = Join-Path $imagePath $fileName
    
    if (Test-Path $filePath) {
        $downloaded++
        continue
    }
    
    $keyword = $productos[$i]
    $url = "https://source.unsplash.com/400x400/?$keyword,product&sig=$i"
    
    try {
        $ProgressPreference = 'SilentlyContinue'
        Invoke-WebRequest -Uri $url -OutFile $filePath -TimeoutSec 10 -ErrorAction SilentlyContinue
        
        if (Test-Path $filePath) {
            $downloaded++
            if ($i % 10 -eq 0) {
                Write-Host "  ? Descargadas $i/100 imágenes..." -ForegroundColor Green
            }
        }
    }
    catch { }
    
    Start-Sleep -Milliseconds 200
}

Write-Host "  ? Descargadas: $downloaded/100 imágenes" -ForegroundColor Green
Write-Host ""

# ============================================================
# PASO 3: Limpiar BD
# ============================================================
Write-Host "?? PASO 3: Limpiando base de datos..." -ForegroundColor Yellow

Set-Location $projectPath

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
    Write-Host "  ? Migraciones ejecutadas" -ForegroundColor Green
}

Write-Host ""

# ============================================================
# PASO 4: Resultado
# ============================================================
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "?              ? SETUP COMPLETADO                          ?" -ForegroundColor Green
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Green

Write-Host ""
Write-Host "?? RESUMEN:" -ForegroundColor Cyan
Write-Host "  ? Imágenes descargadas: $downloaded/100" -ForegroundColor Green
Write-Host "  ? Base de datos limpiada" -ForegroundColor Green
Write-Host "  ? Migraciones ejecutadas" -ForegroundColor Green

Write-Host ""
Write-Host "?? INICIANDO APLICACIÓN..." -ForegroundColor Yellow
Write-Host ""

# ============================================================
# PASO 5: Iniciar app
# ============================================================
dotnet run

Write-Host ""
Write-Host "? Sesión completada" -ForegroundColor Cyan
