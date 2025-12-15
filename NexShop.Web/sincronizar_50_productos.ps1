# Script para sincronizar carpetas con los 50 productos reales

$basePath = "E:\Proyectos Visual\NexShop\NexShop.Web\wwwroot\uploads\productos"

Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?  SINCRONIZAR CARPETAS CON 50 PRODUCTOS REALES                 ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Lista de 50 productos reales de la BD
$productosReales = @(
    @{id=1; nombre="Smartphone XYZ Pro"},
    @{id=2; nombre="Laptop Gaming 15`""},
    @{id=3; nombre="Tablet 10 pulgadas"},
    @{id=4; nombre="Auriculares Bluetooth"},
    @{id=5; nombre="Monitor 4K 27`""},
    @{id=6; nombre="Teclado Mecánico RGB"},
    @{id=7; nombre="Ratón Gaming Inalámbrico"},
    @{id=8; nombre="Webcam 4K"},
    @{id=9; nombre="Cargador Rápido 100W"},
    @{id=10; nombre="Powerbank 30000mAh"},
    @{id=11; nombre="Camiseta Básica Blanca"},
    @{id=12; nombre="Jeans Azul Oscuro"},
    @{id=13; nombre="Sudadera con Capucha"},
    @{id=14; nombre="Chaqueta de Cuero"},
    @{id=15; nombre="Pantalones Deportivos"},
    @{id=16; nombre="Calcetines Pack de 5"},
    @{id=17; nombre="Gorro de Lana"},
    @{id=18; nombre="Bufanda Larga"},
    @{id=19; nombre="Zapatos Deportivos"},
    @{id=20; nombre="Cinturón de Cuero"},
    @{id=21; nombre="Juego de Sábanas 100% Algodón"},
    @{id=22; nombre="Almohada de Espuma"},
    @{id=23; nombre="Edredón Nórdico"},
    @{id=24; nombre="Cortinas Blackout"},
    @{id=25; nombre="Lámpara de Escritorio LED"},
    @{id=26; nombre="Espejo de Pared Grande"},
    @{id=27; nombre="Alfombra Persa"},
    @{id=28; nombre="Juego de Toallas 6 Piezas"},
    @{id=29; nombre="Tapete de Baño"},
    @{id=30; nombre="Plantas Decorativas Artificiales"},
    @{id=31; nombre="Balón de Fútbol Profesional"},
    @{id=32; nombre="Raqueta de Tenis"},
    @{id=33; nombre="Pelota de Baloncesto"},
    @{id=34; nombre="Mancuernas Ajustables"},
    @{id=35; nombre="Colchoneta de Yoga"},
    @{id=36; nombre="Banda Elástica Resistencia"},
    @{id=37; nombre="Botella de Agua 1L"},
    @{id=38; nombre="Guantes de Boxeo"},
    @{id=39; nombre="Cinta Métrica Flexible"},
    @{id=40; nombre="Uniforme Deportivo"},
    @{id=41; nombre="El Quijote - Cervantes"},
    @{id=42; nombre="1984 - George Orwell"},
    @{id=43; nombre="Cien Años de Soledad - García Márquez"},
    @{id=44; nombre="Hábitos Atómicos - James Clear"},
    @{id=45; nombre="El Alquimista - Paulo Coelho"},
    @{id=46; nombre="Sapiens - Yuval Noah Harari"},
    @{id=47; nombre="El Poder del Ahora - Eckhart Tolle"},
    @{id=48; nombre="La Revolución de los Creativos"},
    @{id=49; nombre="Mindfulness para Principiantes"},
    @{id=50; nombre="El Juego Infinito - Simon Sinek"}
)

Write-Host "? Productos reales cargados: $($productosReales.Count)" -ForegroundColor Green
Write-Host ""

# Obtener carpetas actuales
$carpetasActuales = Get-ChildItem -Path $basePath -Directory | Select-Object -ExpandProperty Name | Sort-Object

Write-Host "?? Carpetas actuales en el sistema: $($carpetasActuales.Count)" -ForegroundColor Yellow
Write-Host ""

# Crear diccionario de productos
$productosDict = @{}
foreach ($prod in $productosReales) {
    $productosDict[$prod.id] = $prod.nombre
}

# Clasificar carpetas
$carpetasValidas = @()
$carpetasARenombrar = @()
$carpetasAEliminar = @()

foreach ($carpeta in $carpetasActuales) {
    $parts = $carpeta -split "_", 2
    
    if ($parts[0] -match "^\d+$") {
        $id = [int]$parts[0]
        
        if ($productosDict.ContainsKey($id)) {
            # Producto existe
            $nombreEsperado = "$id`_$($productosDict[$id])"
            
            if ($carpeta -eq $nombreEsperado) {
                $carpetasValidas += $carpeta
            } else {
                $carpetasARenombrar += @{
                    actual = $carpeta
                    esperado = $nombreEsperado
                    id = $id
                }
            }
        } else {
            # Producto no existe
            $carpetasAEliminar += $carpeta
        }
    } else {
        # Formato inválido
        $carpetasAEliminar += $carpeta
    }
}

# Resumen
Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "RESUMEN DE CAMBIOS NECESARIOS:" -ForegroundColor Green
Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

Write-Host "? Carpetas válidas (correctas):  $($carpetasValidas.Count)" -ForegroundColor Green
Write-Host "??  Carpetas a renombrar:          $($carpetasARenombrar.Count)" -ForegroundColor Yellow
Write-Host "???  Carpetas a eliminar:           $($carpetasAEliminar.Count)" -ForegroundColor Red
Write-Host ""

if ($carpetasARenombrar.Count -gt 0) {
    Write-Host "EJEMPLOS DE RENOMBRAMIENTO:" -ForegroundColor Yellow
    foreach ($item in $carpetasARenombrar | Select-Object -First 5) {
        Write-Host "   $($item.actual)" -ForegroundColor Yellow
        Write-Host "   ? $($item.esperado)" -ForegroundColor Green
        Write-Host ""
    }
    if ($carpetasARenombrar.Count -gt 5) {
        Write-Host "   ... y $($carpetasARenombrar.Count - 5) más" -ForegroundColor Yellow
        Write-Host ""
    }
}

if ($carpetasAEliminar.Count -gt 0) {
    Write-Host "EJEMPLOS DE ELIMINACIÓN:" -ForegroundColor Red
    foreach ($carpeta in $carpetasAEliminar | Select-Object -First 10) {
        Write-Host "   ? $carpeta"
    }
    if ($carpetasAEliminar.Count -gt 10) {
        Write-Host "   ... y $($carpetasAEliminar.Count - 10) más"
    }
    Write-Host ""
}

Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green

# Preguntar confirmación
Write-Host ""
Write-Host "??  ADVERTENCIA: Se $($carpetasARenombrar.Count) carpetas y se eliminarán $($carpetasAEliminar.Count)" -ForegroundColor Yellow
Write-Host ""

$respuesta = Read-Host "¿Deseas proceder? (s/n)"

if ($respuesta -eq "s") {
    Write-Host ""
    Write-Host "Aplicando cambios..." -ForegroundColor Yellow
    Write-Host ""
    
    # Eliminar carpetas innecesarias
    $eliminadas = 0
    foreach ($carpeta in $carpetasAEliminar) {
        try {
            $ruta = Join-Path $basePath $carpeta
            Remove-Item $ruta -Recurse -Force
            Write-Host "???  Eliminada: $carpeta" -ForegroundColor Red
            $eliminadas++
        }
        catch {
            Write-Host "? Error al eliminar $carpeta`: $_" -ForegroundColor DarkRed
        }
    }
    
    # Renombrar carpetas con nombres incorrectos
    $renombradas = 0
    foreach ($item in $carpetasARenombrar) {
        try {
            $rutaActual = Join-Path $basePath $item.actual
            $rutaNueva = Join-Path $basePath $item.esperado
            Rename-Item $rutaActual $rutaNueva
            Write-Host "??  Renombrada: $($item.actual) ? $($item.esperado)" -ForegroundColor Yellow
            $renombradas++
        }
        catch {
            Write-Host "? Error al renombrar $($item.actual)`: $_" -ForegroundColor DarkRed
        }
    }
    
    # Crear carpetas faltantes
    $creadas = 0
    foreach ($prod in $productosReales) {
        $carpetaEsperada = "$($prod.id)`_$($prod.nombre)"
        if (-not (Test-Path (Join-Path $basePath $carpetaEsperada))) {
            try {
                $ruta = Join-Path $basePath $carpetaEsperada
                New-Item -Path $ruta -ItemType Directory | Out-Null
                Write-Host "?? Creada: $carpetaEsperada" -ForegroundColor Green
                $creadas++
            }
            catch {
                Write-Host "? Error al crear $carpetaEsperada`: $_" -ForegroundColor DarkRed
            }
        }
    }
    
    Write-Host ""
    Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green
    Write-Host "? CAMBIOS COMPLETADOS:" -ForegroundColor Green
    Write-Host "   • Eliminadas: $eliminadas" -ForegroundColor Green
    Write-Host "   • Renombradas: $renombradas" -ForegroundColor Green
    Write-Host "   • Creadas: $creadas" -ForegroundColor Green
    Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green
    
    # Verificar resultado final
    Write-Host ""
    $final = Get-ChildItem -Path $basePath -Directory | Select-Object -ExpandProperty Name | Sort-Object
    Write-Host "Carpetas finales: $($final.Count)" -ForegroundColor Green
    
} else {
    Write-Host ""
    Write-Host "Operación cancelada" -ForegroundColor Yellow
}
