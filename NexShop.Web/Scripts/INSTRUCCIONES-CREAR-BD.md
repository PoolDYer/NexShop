# ?? INSTRUCCIONES PARA CREAR LA BASE DE DATOS NEXSHOP

**Fecha:** 2025-11-28  
**Servidor:** ADMINISTRADOR\POOL  
**Usuario:** sa  
**Contraseña:** 123456  
**Base de Datos:** NexShopDb  

---

## ?? OPCIÓN 1: USANDO POWERSHELL (RECOMENDADO)

### Paso 1: Abrir PowerShell como Administrador

1. Presiona `Win + X`
2. Selecciona "Windows PowerShell (Admin)" o "Terminal (Admin)"
3. Navega a la carpeta del proyecto:

```powershell
cd "E:\Proyectos Visual\NexShop\NexShop.Web"
```

### Paso 2: Ejecutar el Script

```powershell
# Permitir ejecución de scripts (solo primera vez)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Ejecutar el script
.\Scripts\Crear-BD.ps1
```

### Resultado Esperado

```
================================
CREANDO BASE DE DATOS NEXSHOP
================================

[1/5] Conectando a SQL Server: ADMINISTRADOR\POOL...
[2/5] Ejecutando script SQL...
[3/5] Script SQL ejecutado correctamente
[4/5] Verificando base de datos...
Tablas creadas: 11
[5/5] ¡Base de datos creada exitosamente!

================================
? ÉXITO - BD LISTA PARA USAR
================================

Base de Datos: NexShopDb
Servidor: ADMINISTRADOR\POOL
Usuario: sa

Ahora ejecuta: dotnet run
```

---

## ?? OPCIÓN 2: USANDO BATCH (SIMPLE)

### Paso 1: Abrir CMD como Administrador

1. Presiona `Win + R`
2. Escribe: `cmd`
3. Presiona `Ctrl + Shift + Enter` para abrir como admin
4. Navega al proyecto:

```cmd
cd E:\Proyectos Visual\NexShop\NexShop.Web
```

### Paso 2: Ejecutar el Script

```cmd
Scripts\Crear-BD.bat
```

---

## ?? OPCIÓN 3: SQL SERVER MANAGEMENT STUDIO (MANUAL)

### Paso 1: Abrir SSMS

1. Abre **SQL Server Management Studio**
2. Conecta con:
   - Server: `ADMINISTRADOR\POOL`
   - Authentication: SQL Server Authentication
   - Login: `sa`
   - Password: `123456`

### Paso 2: Ejecutar Script SQL

1. Click en **File** ? **Open** ? **File**
2. Selecciona: `E:\Proyectos Visual\NexShop\NexShop.Web\Scripts\01-CrearBaseDatos.sql`
3. Presiona **Execute** (F5)
4. Verifica que todas las tablas se creen sin errores

---

## ?? OPCIÓN 4: DESDE VISUAL STUDIO (ENTITY FRAMEWORK)

### Paso 1: Abrir Visual Studio

1. Abre `NexShop.Web.sln`
2. Abre la **Package Manager Console**
   - Tools ? NuGet Package Manager ? Package Manager Console

### Paso 2: Ejecutar Migraciones

```powershell
# En la Package Manager Console

# Crear migración
Add-Migration InitialCreate

# Aplicar migración (crea la BD)
Update-Database
```

---

## ? VERIFICAR QUE LA BD FUE CREADA

### Opción A: SQL Server Management Studio

```sql
-- Conectar a ADMINISTRADOR\POOL con usuario sa
-- Ejecutar esta query:

SELECT COUNT(*) as TablesCreated 
FROM NexShopDb.INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'dbo'
```

Resultado esperado: **11** (tablas creadas)

### Opción B: PowerShell

```powershell
sqlcmd -S ADMINISTRADOR\POOL -U sa -P 123456 -d NexShopDb -Q "SELECT COUNT(*) as TablesCreated FROM INFORMATION_SCHEMA.TABLES"
```

---

## ?? CREDENCIALES ACTUALIZADAS EN APPSETTINGS

El archivo `appsettings.json` ya ha sido actualizado con:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=ADMINISTRADOR\\POOL;Database=NexShopDb;User Id=sa;Password=123456;MultipleActiveResultSets=true;Encrypt=true;TrustServerCertificate=true"
  }
}
```

---

## ?? DESPUÉS DE CREAR LA BD

### Paso 1: Compilar el Proyecto

```powershell
cd "E:\Proyectos Visual\NexShop\NexShop.Web"
dotnet build
```

### Paso 2: Ejecutar la Aplicación

```powershell
dotnet run
```

### Paso 3: Abrir en Navegador

```
http://localhost:5217
```

---

## ?? CONTENIDO DE LA BASE DE DATOS

### Tablas Creadas (11 total)

1. **AspNetRoles** - Roles del sistema (Admin, Vendedor, Comprador)
2. **AspNetUsers** - Usuarios registrados
3. **AspNetUserRoles** - Relación usuario-rol
4. **Categorias** - Categorías de productos (5 predefinidas)
5. **Productos** - Catálogo de productos
6. **Multimedia** - Imágenes y videos de productos
7. **Ordenes** - Órdenes de compra
8. **OrdenDetalles** - Items dentro de cada orden
9. **Preguntas** - Preguntas de clientes sobre productos
10. **Respuestas** - Respuestas a preguntas
11. **Calificaciones** - Reseñas y ratings

### Índices Creados (10 total)

```sql
IX_Productos_CategoriaId
IX_Productos_VendedorId
IX_Productos_Estado
IX_Multimedia_ProductoId
IX_Ordenes_CompradorId
IX_Ordenes_Estado
IX_Ordenes_FechaCreacion
IX_Preguntas_ProductoId
IX_Preguntas_UsuarioId
IX_Preguntas_Estado
```

### Roles Insertados (3 total)

- **Admin** - Administrador del sistema
- **Vendedor** - Puede crear y gestionar productos
- **Comprador** - Cliente que compra productos

### Categorías Insertadas (5 total)

- Electrónica
- Ropa
- Hogar
- Deportes
- Libros

---

## ? TROUBLESHOOTING

### Error: "Login failed for user 'sa'"

**Solución:**
```powershell
# Verificar que SQL Server está corriendo
Get-Service | Where-Object {$_.Name -like "*MSSQL*"}

# O inicia SQL Server manualmente
Start-Service MSSQLSERVER
```

### Error: "Network or instance-specific error"

**Solución:**
```powershell
# Verifica que el servidor sea accesible
Test-NetConnection -ComputerName ADMINISTRADOR -Port 1433

# Intenta conectar desde SSMS primero
```

### Error: "File not found"

**Solución:**
```powershell
# Verifica que los archivos existen
Test-Path "E:\Proyectos Visual\NexShop\NexShop.Web\Scripts\01-CrearBaseDatos.sql"
Test-Path "E:\Proyectos Visual\NexShop\NexShop.Web\Scripts\Crear-BD.ps1"
```

---

## ? ¡LISTO!

Tu base de datos **NexShopDb** está completamente configurada y lista para usar.

**Próximo paso:** Ejecuta `dotnet run` y accede a `http://localhost:5217`

---

**Última actualización:** 2025-11-28  
**Versión:** 1.0  
**Estado:** ? LISTO PARA PRODUCCIÓN
