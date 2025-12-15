# Script automático para sincronizar carpetas - con sanitización de nombres

$basePath = "E:\Proyectos Visual\NexShop\NexShop.Web\wwwroot\uploads\productos"

Write-Host "Sincronizando carpetas con 50 productos..." -ForegroundColor Green

$productosReales = @{
    1 = "Smartphone XYZ Pro"
    2 = "Laptop Gaming 15"
    3 = "Tablet 10 pulgadas"
    4 = "Auriculares Bluetooth"
    5 = "Monitor 4K 27"
    6 = "Teclado Mecánico RGB"
    7 = "Ratón Gaming Inalámbrico"
    8 = "Webcam 4K"
    9 = "Cargador Rápido 100W"
    10 = "Powerbank 30000mAh"
    11 = "Camiseta Básica Blanca"
    12 = "Jeans Azul Oscuro"
    13 = "Sudadera con Capucha"
    14 = "Chaqueta de Cuero"
    15 = "Pantalones Deportivos"
    16 = "Calcetines Pack de 5"
    17 = "Gorro de Lana"
    18 = "Bufanda Larga"
    19 = "Zapatos Deportivos"
    20 = "Cinturón de Cuero"
    21 = "Juego de Sábanas 100 por ciento Algodón"
    22 = "Almohada de Espuma"
    23 = "Edredón Nórdico"
    24 = "Cortinas Blackout"
    25 = "Lámpara de Escritorio LED"
    26 = "Espejo de Pared Grande"
    27 = "Alfombra Persa"
    28 = "Juego de Toallas 6 Piezas"
    29 = "Tapete de Baño"
    30 = "Plantas Decorativas Artificiales"
    31 = "Balón de Fútbol Profesional"
    32 = "Raqueta de Tenis"
    33 = "Pelota de Baloncesto"
    34 = "Mancuernas Ajustables"
    35 = "Colchoneta de Yoga"
    36 = "Banda Elástica Resistencia"
    37 = "Botella de Agua 1L"
    38 = "Guantes de Boxeo"
    39 = "Cinta Métrica Flexible"
    40 = "Uniforme Deportivo"
    41 = "El Quijote - Cervantes"
    42 = "1984 - George Orwell"
    43 = "Cien Años de Soledad - García Márquez"
    44 = "Hábitos Atómicos - James Clear"
    45 = "El Alquimista - Paulo Coelho"
    46 = "Sapiens - Yuval Noah Harari"
    47 = "El Poder del Ahora - Eckhart Tolle"
    48 = "La Revolución de los Creativos"
    49 = "Mindfulness para Principiantes"
    50 = "El Juego Infinito - Simon Sinek"
}

# Función para sanitizar nombres (quitar caracteres inválidos)
function Sanitizar-Nombre {
    param([string]$nombre)
    $invalidos = @('"', '<', '>', '|', '?', '*')
    foreach ($char in $invalidos) {
        $nombre = $nombre.Replace($char, "")
    }
    return $nombre.Trim()
}

# Obtener carpetas actuales
$carpetas = Get-ChildItem -Path $basePath -Directory | Select-Object -ExpandProperty Name

# Eliminar todas las carpetas
Write-Host "Eliminando todas las carpetas actuales..." -ForegroundColor Yellow
$eliminadas = 0
foreach ($carpeta in $carpetas) {
    try {
        $ruta = Join-Path $basePath $carpeta
        Remove-Item $ruta -Recurse -Force
        $eliminadas++
    } catch {}
}
Write-Host "Eliminadas: $eliminadas" -ForegroundColor Red

# Crear las 50 carpetas correctas
Write-Host "Creando 50 carpetas con nombres correctos..." -ForegroundColor Green
$creadas = 0

foreach ($id in 1..50) {
    $nombreBruto = $productosReales[$id]
    $nombreSanitizado = Sanitizar-Nombre $nombreBruto
    $carpetaEsperada = "$id`_$nombreSanitizado"
    $rutaEsperada = Join-Path $basePath $carpetaEsperada
    
    try {
        if (-not (Test-Path $rutaEsperada)) {
            New-Item -Path $rutaEsperada -ItemType Directory -Force | Out-Null
            $creadas++
        }
    } catch {
        Write-Host "Error creando $carpetaEsperada`: $_" -ForegroundColor DarkRed
    }
}

Write-Host "Creadas: $creadas carpetas" -ForegroundColor Green

# Verificar
$final = (Get-ChildItem -Path $basePath -Directory).Count
Write-Host ""
Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "Carpetas finales: $final / 50" -ForegroundColor Cyan
Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green

if ($final -eq 50) {
    Write-Host ""
    Write-Host "? SINCRONIZACIÓN EXITOSA - 50 CARPETAS CREADAS" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "??  Resultado: $final carpetas (esperadas 50)" -ForegroundColor Yellow
}
