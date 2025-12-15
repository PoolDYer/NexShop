#!/usr/bin/env powershell

<#
.SYNOPSIS
Script para crear carpetas de productos existentes en NexShop

.DESCRIPTION
Lee todos los productos de la BD y crea carpetas en:
wwwroot/uploads/productos/{ProductoId}_{NombreProducto}/

.EXAMPLE
.\crear-todas-carpetas.ps1

.NOTES
Debe ejecutarse desde la carpeta NexShop.Web
#>

param(
    [switch]$Clean,
    [switch]$List,
    [switch]$Help
)

if ($Help) {
    Write-Host @"
??????????????????????????????????????????????????????????????????
?     CREAR CARPETAS PARA TODOS LOS PRODUCTOS EXISTENTES        ?
??????????????????????????????????????????????????????????????????

OPCIONES:
  -List   : Listar productos sin crear carpetas
  -Clean  : Limpiar carpetas existentes (antes de crear nuevas)
  -Help   : Mostrar esta ayuda

EJEMPLOS:
  .\crear-todas-carpetas.ps1           # Crear carpetas
  .\crear-todas-carpetas.ps1 -List     # Listar productos
  .\crear-todas-carpetas.ps1 -Clean    # Limpiar y recrear

UBICACIÓN:
  Ejecutar desde: E:\Proyectos Visual\NexShop\NexShop.Web
"@
    exit 0
}

Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?     CREAR CARPETAS PARA TODOS LOS PRODUCTOS EXISTENTES        ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Verificar ubicación
if (-not (Test-Path "NexShop.Web.csproj")) {
    Write-Host "? ERROR: No estás en la carpeta correcta" -ForegroundColor Red
    Write-Host "?? Debes estar en: NexShop.Web" -ForegroundColor Yellow
    Write-Host "?? Estás en: $(Get-Location)" -ForegroundColor Yellow
    exit 1
}

# Paths
$dbFile = "NexShop.db"
$uploadsDir = ".\wwwroot\uploads\productos"

# Verificar BD
if (-not (Test-Path $dbFile)) {
    Write-Host "? Base de datos no encontrada: $dbFile" -ForegroundColor Red
    exit 1
}

Write-Host "? Base de datos: $dbFile" -ForegroundColor Green
Write-Host ""

# Crear carpeta base
$uploadsBase = ".\wwwroot\uploads"
if (-not (Test-Path $uploadsBase)) {
    New-Item -Path $uploadsBase -ItemType Directory -Force | Out-Null
}

if (-not (Test-Path $uploadsDir)) {
    New-Item -Path $uploadsDir -ItemType Directory -Force | Out-Null
    Write-Host "? Carpeta creada: $uploadsDir" -ForegroundColor Green
} else {
    Write-Host "? Carpeta verificada: $uploadsDir" -ForegroundColor Green
}

Write-Host ""

# Leer productos de la BD usando dotnet ef (si está disponible)
Write-Host "?? Leyendo productos..." -ForegroundColor Yellow

# Método 1: Usar dotnet user-secrets y crear un pequeño programa
$readProductosCode = @"
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NexShop.Web.Models;

var context = new NexShopContext();
var productos = context.Productos.OrderBy(p => p.ProductoId).ToList();

foreach (var p in productos)
{
    Console.WriteLine($"{p.ProductoId}|{p.Nombre}");
}
"@

# Método 2: Usar .NET para leer directamente
try {
    # Cargar assemblies necesarios
    [System.Reflection.Assembly]::LoadFrom(".\bin\Debug\net8.0\NexShop.Web.dll") | Out-Null
    [System.Reflection.Assembly]::LoadFrom(".\bin\Debug\net8.0\Microsoft.EntityFrameworkCore.dll") | Out-Null
} catch {
    # Si no está compilado, compilar primero
    Write-Host "??  Compilando proyecto..." -ForegroundColor Yellow
    dotnet build | Out-Null
    Write-Host "? Compilación completada" -ForegroundColor Green
    Write-Host ""
}

# Leer manualmente de la BD usando SQL
try {
    # Crear conexión SQLite
    $assembly = [System.Reflection.Assembly]::LoadFrom(".\bin\Debug\net8.0\System.Data.SQLite.dll")
    $sqlite_connection = New-Object System.Data.SQLite.SQLiteConnection
    $sqlite_connection.ConnectionString = "Data Source=$dbFile"
    $sqlite_connection.Open()
    
    $sqlite_cmd = $sqlite_connection.CreateCommand()
    $sqlite_cmd.CommandText = "SELECT ProductoId, Nombre FROM Productos ORDER BY ProductoId;"
    
    $reader = $sqlite_cmd.ExecuteReader()
    
    $productos = @()
    while ($reader.Read()) {
        $productos += @{
            ProductoId = $reader.GetInt32(0)
            Nombre = $reader.GetString(1)
        }
    }
    
    $reader.Close()
    $sqlite_connection.Close()
    
    if ($List) {
        Write-Host "?? PRODUCTOS EN LA BASE DE DATOS:" -ForegroundColor Cyan
        Write-Host ""
        foreach ($p in $productos) {
            Write-Host "  [$($p.ProductoId)] $($p.Nombre)" -ForegroundColor White
        }
        Write-Host ""
        Write-Host "Total: $($productos.Count) producto(s)" -ForegroundColor Green
        exit 0
    }
    
    Write-Host "? Se encontraron $($productos.Count) producto(s)" -ForegroundColor Green
    Write-Host ""
    
    if ($Clean) {
        Write-Host "???  Limpiando carpetas existentes..." -ForegroundColor Yellow
        Get-ChildItem $uploadsDir -Directory | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "? Carpetas limpias" -ForegroundColor Green
        Write-Host ""
    }
    
    Write-Host "?? Creando carpetas:" -ForegroundColor Cyan
    Write-Host ""
    
    $creadas = 0
    $existentes = 0
    
    foreach ($p in $productos) {
        $id = $p.ProductoId
        $nombre = $p.Nombre
        
        # Sanitizar nombre
        $nombreSanitizado = $nombre
        [System.IO.Path]::GetInvalidFileNameChars() | ForEach-Object {
            $nombreSanitizado = $nombreSanitizado -replace [regex]::Escape($_), "_"
        }
        
        # Ruta
        $carpeta = Join-Path $uploadsDir "${id}_${nombreSanitizado}"
        
        # Crear
        if (-not (Test-Path $carpeta)) {
            New-Item -Path $carpeta -ItemType Directory -Force | Out-Null
            Write-Host "  ? [$id] $nombre" -ForegroundColor Green
            $creadas++
        } else {
            Write-Host "  ??  [$id] $nombre (ya existe)" -ForegroundColor Yellow
            $existentes++
        }
    }
    
    Write-Host ""
    Write-Host "?? RESUMEN:" -ForegroundColor Cyan
    Write-Host "  • Carpetas creadas: $creadas" -ForegroundColor Green
    Write-Host "  • Carpetas existentes: $existentes" -ForegroundColor Yellow
    Write-Host "  • Total: $($productos.Count)" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Host "? COMPLETADO" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? Ubicación: $(Get-Item $uploadsDir).FullName" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "?? Ahora puedes:" -ForegroundColor Green
    Write-Host "   1. Abrir en Visual Studio: wwwroot > uploads > productos" -ForegroundColor White
    Write-Host "   2. Descargar imágenes de internet" -ForegroundColor White
    Write-Host "   3. Colocar las imágenes en sus carpetas" -ForegroundColor White
    Write-Host "   4. O crear productos nuevos y subir multimedia desde la app" -ForegroundColor White
    Write-Host ""
    
}
catch {
    Write-Host "? ERROR: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "?? SOLUCIÓN ALTERNATIVA:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Crea manualmente las carpetas en Visual Studio:" -ForegroundColor White
    Write-Host ""
    Write-Host "1. Abre Solution Explorer" -ForegroundColor White
    Write-Host "2. Ve a: NexShop.Web > wwwroot > uploads > productos" -ForegroundColor White
    Write-Host "3. Crea nuevas carpetas:" -ForegroundColor White
    Write-Host "   Click derecho > Add > New Folder" -ForegroundColor White
    Write-Host ""
    Write-Host "4. Nombra las carpetas así:" -ForegroundColor White
    Write-Host "   1_Nombre del Producto 1" -ForegroundColor Yellow
    Write-Host "   2_Nombre del Producto 2" -ForegroundColor Yellow
    Write-Host "   3_Nombre del Producto 3" -ForegroundColor Yellow
    Write-Host "   ... etc" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}
