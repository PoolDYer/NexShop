# Reorganizar carpetas según productos de BD

$dbPath = "E:\Proyectos Visual\NexShop\NexShop.Web\NexShop.db"
$basePath = "E:\Proyectos Visual\NexShop\NexShop.Web\wwwroot\uploads\productos"

Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?  REORGANIZAR CARPETAS - SINCRONIZAR CON BD                    ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Función para leer productos de SQLite
function Get-ProductosFromDB {
    param([string]$dbPath)
    
    # Crear tabla temporal con productos
    $productos = @()
    
    # Usar sqlite3 CLI si está disponible
    $sqlite3 = Get-Command sqlite3 -ErrorAction SilentlyContinue
    
    if ($sqlite3) {
        Write-Host "? usando sqlite3..." -ForegroundColor Green
        $query = "SELECT ProductoId, Nombre FROM Productos ORDER BY ProductoId;"
        $output = & sqlite3 $dbPath $query 2>&1
        
        foreach ($line in $output) {
            if ($line -and $line -match "^(\d+)\|(.+)$") {
                $id = [int]$matches[1]
                $nombre = $matches[2]
                $productos += @{id=$id; nombre=$nombre}
            }
        }
    } else {
        Write-Host "??  sqlite3 no encontrado" -ForegroundColor Yellow
        Write-Host "Usando lista de ejemplo..." -ForegroundColor Yellow
        
        # Fallback: hardcodear algunos productos conocidos
        $productos = @(
            @{id=49; nombre="Mindfulness para Principiantes"}
        )
    }
    
    return $productos
}

# Leer productos
Write-Host "?? Leyendo productos de la BD..." -ForegroundColor Yellow

$productos = Get-ProductosFromDB $dbPath

Write-Host "? Se encontraron $($productos.Count) productos" -ForegroundColor Green
Write-Host ""

# Mostrar primeros y últimos
if ($productos.Count -gt 0) {
    Write-Host "PRIMEROS 5 PRODUCTOS:" -ForegroundColor Cyan
    $productos | Select-Object -First 5 | ForEach-Object {
        Write-Host "  [$($_.id)] $($_.nombre)"
    }
    
    if ($productos.Count -gt 5) {
        Write-Host "  ..."
        Write-Host ""
        Write-Host "ÚLTIMOS 5 PRODUCTOS:" -ForegroundColor Cyan
        $productos | Select-Object -Last 5 | ForEach-Object {
            Write-Host "  [$($_.id)] $($_.nombre)"
        }
    }
}

Write-Host ""

# Obtener carpetas actuales
$carpetasActuales = Get-ChildItem -Path $basePath -Directory | Select-Object -ExpandProperty Name | Sort-Object

Write-Host "?? Carpetas actuales: $($carpetasActuales.Count)" -ForegroundColor Yellow
Write-Host ""

# Analizar
Write-Host "?? Analizando..." -ForegroundColor Yellow
Write-Host ""

$carpetasAEliminar = @()
$carpetasACrear = @()
$carpetasARenombrar = @()

$productosDict = @{}
foreach ($prod in $productos) {
    $productosDict[$prod.id] = $prod.nombre
}

# Verificar cada carpeta
foreach ($carpeta in $carpetasActuales) {
    $parts = $carpeta -split "_", 2
    
    if ($parts[0] -match "^\d+$") {
        $id = [int]$parts[0]
        
        if ($productosDict.ContainsKey($id)) {
            # Producto existe, verificar nombre
            $nombreEsperado = "$id`_$($productosDict[$id])"
            if ($carpeta -ne $nombreEsperado) {
                $carpetasARenombrar += @{
                    actual = $carpeta
                    id = $id
                    esperado = $nombreEsperado
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

# Encontrar productos sin carpeta
foreach ($prod in $productos) {
    $carpetaEsperada = "$($prod.id)`_$($prod.nombre)"
    $existe = $carpetasActuales -contains $carpetaEsperada
    
    if (-not $existe) {
        $carpetasACrear += $prod
    }
}

# Mostrar resumen
Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "RESUMEN DE ACCIONES:" -ForegroundColor Green
Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

Write-Host "?? Carpetas a crear:       $($carpetasACrear.Count)" -ForegroundColor Green
Write-Host "??  Carpetas a renombrar:   $($carpetasARenombrar.Count)" -ForegroundColor Yellow
Write-Host "???  Carpetas a eliminar:    $($carpetasAEliminar.Count)" -ForegroundColor Red
Write-Host ""

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

if ($carpetasARenombrar.Count -gt 0) {
    Write-Host "??  CARPETAS A RENOMBRAR:" -ForegroundColor Yellow
    foreach ($item in $carpetasARenombrar | Select-Object -First 10) {
        Write-Host "   $($item.actual) ? $($item.esperado)"
    }
    if ($carpetasARenombrar.Count -gt 10) {
        Write-Host "   ... y $($carpetasARenombrar.Count - 10) más"
    }
    Write-Host ""
}

if ($carpetasACrear.Count -gt 0) {
    Write-Host "?? CARPETAS A CREAR:" -ForegroundColor Green
    foreach ($item in $carpetasACrear | Select-Object -First 10) {
        Write-Host "   ? $($item.id)`_$($item.nombre)"
    }
    if ($carpetasACrear.Count -gt 10) {
        Write-Host "   ... y $($carpetasACrear.Count - 10) más"
    }
    Write-Host ""
}

Write-Host "???????????????????????????????????????????????????????????????" -ForegroundColor Green
