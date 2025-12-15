# Script alternativo para crear carpetas - Versión .NET/C#
# Ejecutar desde PowerShell en la carpeta NexShop.Web

Write-Host "?????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?  CREAR CARPETAS PARA TODOS LOS PRODUCTOS                     ?" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Ubicaciones
$dbPath = "NexShop.db"
$uploadsPath = ".\wwwroot\uploads\productos"

# Verificar BD
if (-not (Test-Path $dbPath)) {
    Write-Host "? Base de datos no encontrada: $dbPath" -ForegroundColor Red
    Write-Host "?? Ubicación actual: $(Get-Location)" -ForegroundColor Yellow
    exit 1
}

Write-Host "? Base de datos encontrada" -ForegroundColor Green

# Crear carpeta base
if (-not (Test-Path $uploadsPath)) {
    New-Item -Path $uploadsPath -ItemType Directory -Force | Out-Null
    Write-Host "? Carpeta base creada: $uploadsPath" -ForegroundColor Green
}

Write-Host "? Carpeta base verificada" -ForegroundColor Green
Write-Host ""

# Crear script C# inline para leer BD SQLite
$csharpCode = @"
using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.IO;

public class ProductoCreador
{
    public static int Main(string[] args)
    {
        try
        {
            string dbPath = args[0];
            string carpetaBase = args[1];
            
            List<(int id, string nombre)> productos = new List<(int, string)>();
            
            // Conectar a la BD
            using (var conexion = new SQLiteConnection($"Data Source={dbPath}"))
            {
                conexion.Open();
                
                using (var comando = conexion.CreateCommand())
                {
                    comando.CommandText = "SELECT ProductoId, Nombre FROM Productos ORDER BY ProductoId";
                    
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string nombre = reader.GetString(1);
                            productos.Add((id, nombre));
                        }
                    }
                }
            }
            
            // Crear carpetas
            int creadas = 0;
            int existentes = 0;
            
            foreach (var (id, nombre) in productos)
            {
                // Sanitizar nombre
                string nombreSanitizado = nombre;
                foreach (char c in Path.GetInvalidFileNameChars())
                {
                    nombreSanitizado = nombreSanitizado.Replace(c, '_');
                }
                
                // Crear ruta
                string carpeta = Path.Combine(carpetaBase, $"{id}_{nombreSanitizado}");
                
                // Crear si no existe
                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                    Console.WriteLine($"? [{id}] {nombre}");
                    creadas++;
                }
                else
                {
                    Console.WriteLine($"??  [{id}] {nombre} (ya existe)");
                    existentes++;
                }
            }
            
            Console.WriteLine("");
            Console.WriteLine($"?? RESUMEN:");
            Console.WriteLine($"  • Creadas: {creadas}");
            Console.WriteLine($"  • Existentes: {existentes}");
            Console.WriteLine($"  • Total: {productos.Count}");
            
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? ERROR: {ex.Message}");
            return 1;
        }
    }
}
"@

# Compilar y ejecutar
Write-Host "?? Leyendo productos y creando carpetas..." -ForegroundColor Yellow
Write-Host ""

try {
    # Crear archivo C# temporal
    $csharpFile = "temp_crear_carpetas.cs"
    Set-Content -Path $csharpFile -Value $csharpCode
    
    # Compilar con csc
    $csc = "C:\Program Files\Microsoft Visual Studio\2022\*\MSBuild\Current\Bin\Roslyn\csc.exe"
    $cscPath = Get-Item $csc -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty FullName
    
    if ($cscPath) {
        & $cscPath /target:exe /out:temp_crear_carpetas.exe $csharpFile -reference:"System.Data.SQLite.dll" 2>&1 | Out-Null
        
        if (Test-Path "temp_crear_carpetas.exe") {
            & .\temp_crear_carpetas.exe $dbPath $uploadsPath
            Remove-Item "temp_crear_carpetas.exe" -Force -ErrorAction SilentlyContinue
        }
    } else {
        Write-Host "??  No se encontró compilador C#, usando alternativa..." -ForegroundColor Yellow
        
        # Alternativa: crear carpetas manualmente basadas en patrones
        Write-Host ""
        Write-Host "?? Para crear las carpetas manualmente:" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "1. Abre Visual Studio" -ForegroundColor White
        Write-Host "2. Ve a: Solution Explorer > wwwroot > uploads > productos" -ForegroundColor White
        Write-Host "3. Crea carpetas con este formato:" -ForegroundColor White
        Write-Host "   1_NombreDelProducto1" -ForegroundColor Yellow
        Write-Host "   2_NombreDelProducto2" -ForegroundColor Yellow
        Write-Host "   3_NombreDelProducto3" -ForegroundColor Yellow
        Write-Host "   ... etc" -ForegroundColor Yellow
        Write-Host ""
    }
    
    Remove-Item $csharpFile -Force -ErrorAction SilentlyContinue
}
catch {
    Write-Host "??  Error en ejecución: $_" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "? Proceso completado" -ForegroundColor Green
Write-Host ""
Write-Host "?? Carpetas en: $uploadsPath" -ForegroundColor Cyan
