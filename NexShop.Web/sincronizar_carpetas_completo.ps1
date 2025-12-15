# Leer productos de la BD y reorganizar carpetas

$dbPath = "E:\Proyectos Visual\NexShop\NexShop.Web\NexShop.db"
$basePath = "E:\Proyectos Visual\NexShop\NexShop.Web\wwwroot\uploads\productos"

Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?  REORGANIZAR CARPETAS - SINCRONIZAR CON PRODUCTOS REALES      ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Verificar sqlite3
$sqlite3Path = & where.exe sqlite3 2>$null
if (-not $sqlite3Path) {
    Write-Host "??  sqlite3 no está en PATH. Intentando con ubicaciones comunes..." -ForegroundColor Yellow
    $posiblesSQLite = @(
        "C:\sqlite3\sqlite3.exe",
        "C:\Program Files\sqlite3\sqlite3.exe",
        "C:\Program Files (x86)\sqlite3\sqlite3.exe"
    )
    
    foreach ($ruta in $posiblesSQLite) {
        if (Test-Path $ruta) {
            $sqlite3Path = $ruta
            break
        }
    }
}

# Leer productos de la BD
$productos = @()

if ($sqlite3Path) {
    Write-Host "? Usando sqlite3: $sqlite3Path" -ForegroundColor Green
    
    try {
        $query = "SELECT ProductoId, Nombre FROM Productos ORDER BY ProductoId"
        $output = & $sqlite3Path -separator "|" $dbPath $query 2>&1
        
        foreach ($line in $output) {
            if ($line -and $line.Trim() -and $line -match "^(\d+)\|(.+)$") {
                $id = [int]$matches[1]
                $nombre = $matches[2].Trim()
                $productos += @{id=$id; nombre=$nombre}
            }
        }
        
        Write-Host "? Se leyeron $($productos.Count) productos de la BD" -ForegroundColor Green
    }
    catch {
        Write-Host "? Error al consultar BD: $_" -ForegroundColor Red
    }
} else {
    Write-Host "? sqlite3 no encontrado. Instalarlo desde: https://www.sqlite.org/download.html" -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternativamente, usa el endpoint API (requiere tener la app corriendo):" -ForegroundColor Yellow
    Write-Host "  1. dotnet run" -ForegroundColor Yellow
    Write-Host "  2. http://localhost:5217/api/productos/todos" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
if ($productos.Count -eq 0) {
    Write-Host "? No se encontraron productos en la BD" -ForegroundColor Red
    exit 1
}

# Mostrar productos
Write-Host "PRODUCTOS EN LA BD:" -ForegroundColor Cyan
foreach ($prod in $productos) {
    Write-Host "  [$($prod.id):000] $($prod.nombre)"
}
Write-Host ""

# Obtener carpetas actuales
$carpetasActuales = Get-ChildItem -Path $basePath -Directory | Select-Object -ExpandProperty Name | Sort-Object

Write-Host "?? Carpetas actuales: $($carpetasActuales.Count)" -ForegroundColor Yellow
Write-Host ""

# Crear diccionario de productos por ID
$productosDict = @{}
foreach ($prod in $productos) {
    $productosDict[$prod.id] = $prod.nombre
}

# Analizar carpetas
$carpetasAEliminar = @()
$carpetasARenombrar = @()
$carpetasCorrectas = @()

Write-Host "?? Analizando carpetas..." -ForegroundColor Yellow
Write-Host ""

foreach ($carpeta in $carpetasActuales) {
    $parts = $carpeta -split "_", 2
    
    if ($parts[0] -match "^\d+$") {
        $id = [int]$parts[0]
        
        if ($productosDict.ContainsKey($id)) {
            # Producto existe
            $nombreEsperado = "$id`_$($productosDict[$id])"
            
            if ($carpeta -eq $nombreEsperado) {
                # Correcta
                $carpetasCorrectas += $carpeta
            } else {
                # Necesita renombrar
                $carpetasARenombrar += @{
                    actual = $carpeta
                    esperado = $nombreEsperado
                    id = $id
                }
            }
        } else {
            # Producto no existe - eliminar
            $carpetasAEliminar += $carpeta
        }
    } else {
        # Formato inválido - eliminar
        $carpetasAEliminar += $carpeta
    }
}

# Resumen
Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "RESUMEN DE CAMBIOS:" -ForegroundColor Green
Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

Write-Host "? Carpetas correctas:      $($carpetasCorrectas.Count)" -ForegroundColor Green
Write-Host "??  Carpetas a renombrar:    $($carpetasARenombrar.Count)" -ForegroundColor Yellow
Write-Host "???  Carpetas a eliminar:     $($carpetasAEliminar.Count)" -ForegroundColor Red
Write-Host ""

if ($carpetasCorrectas.Count -gt 0) {
    Write-Host "? CARPETAS CORRECTAS:" -ForegroundColor Green
    foreach ($carpeta in $carpetasCorrectas | Select-Object -First 5) {
        Write-Host "   ? $carpeta"
    }
    if ($carpetasCorrectas.Count -gt 5) {
        Write-Host "   ... y $($carpetasCorrectas.Count - 5) más"
    }
    Write-Host ""
}

if ($carpetasARenombrar.Count -gt 0) {
    Write-Host "??  CARPETAS A RENOMBRAR:" -ForegroundColor Yellow
    foreach ($item in $carpetasARenombrar | Select-Object -First 5) {
        Write-Host "   $($item.actual)"
        Write-Host "   ? $($item.esperado)"
        Write-Host ""
    }
    if ($carpetasARenombrar.Count -gt 5) {
        Write-Host "   ... y $($carpetasARenombrar.Count - 5) más"
        Write-Host ""
    }
}

if ($carpetasAEliminar.Count -gt 0) {
    Write-Host "???  CARPETAS A ELIMINAR:" -ForegroundColor Red
    foreach ($carpeta in $carpetasAEliminar | Select-Object -First 10) {
        Write-Host "   ? $carpeta"
    }
    if ($carpetasAEliminar.Count -gt 10) {
        Write-Host "   ... y $($carpetasAEliminar.Count - 10) más"
    }
    Write-Host ""
}

Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green

# Preguntar si continuar
Write-Host ""
Write-Host "??  ADVERTENCIA: Esto eliminará $($carpetasAEliminar.Count) carpetas" -ForegroundColor Yellow
$respuesta = Read-Host "¿Deseas continuar? (s/n)"

if ($respuesta -eq "s") {
    Write-Host ""
    Write-Host "Aplicando cambios..." -ForegroundColor Yellow
    Write-Host ""
    
    # Eliminar carpetas
    $eliminadas = 0
    foreach ($carpeta in $carpetasAEliminar) {
        try {
            $ruta = Join-Path $basePath $carpeta
            Remove-Item $ruta -Recurse -Force
            Write-Host "???  Eliminada: $carpeta" -ForegroundColor Red
            $eliminadas++
        }
        catch {
            Write-Host "? Error al eliminar $carpeta`: $_" -ForegroundColor Red
        }
    }
    
    # Renombrar carpetas
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
            Write-Host "? Error al renombrar $($item.actual)`: $_" -ForegroundColor Red
        }
    }
    
    Write-Host ""
    Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green
    Write-Host "? CAMBIOS APLICADOS:" -ForegroundColor Green
    Write-Host "   • Eliminadas: $eliminadas" -ForegroundColor Green
    Write-Host "   • Renombradas: $renombradas" -ForegroundColor Green
    Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "Operación cancelada" -ForegroundColor Yellow
}
