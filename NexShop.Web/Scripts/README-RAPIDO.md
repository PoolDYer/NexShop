# ? SCRIPT COMPLETO PARA CREAR BASE DE DATOS - RESUMEN RÁPIDO

## ?? INICIO RÁPIDO (30 segundos)

### OPCIÓN 1: PowerShell (RECOMENDADO)

```powershell
# 1. Abre PowerShell como Admin
# 2. Ejecuta:

cd "E:\Proyectos Visual\NexShop\NexShop.Web"
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
.\Scripts\Crear-BD.ps1

# 3. Listo! La BD se creará automáticamente
```

### OPCIÓN 2: CMD (MÁS SIMPLE)

```cmd
REM 1. Abre CMD como Admin
REM 2. Ejecuta:

cd E:\Proyectos Visual\NexShop\NexShop.Web
Scripts\Crear-BD.bat

REM 3. ¡Presiona una tecla cuando termine!
```

### OPCIÓN 3: SQL Server Management Studio (MANUAL)

```sql
-- 1. Abre SSMS
-- 2. Conecta a: ADMINISTRADOR\POOL (sa / 123456)
-- 3. Abre el archivo: Scripts\01-CrearBaseDatos.sql
-- 4. Presiona F5 para ejecutar
-- 5. ¡Listo!
```

---

## ?? ARCHIVOS GENERADOS

```
E:\Proyectos Visual\NexShop\NexShop.Web\Scripts\
??? 01-CrearBaseDatos.sql          ? Script SQL completo (? 11 tablas)
??? Crear-BD.ps1                   ? Script PowerShell automático
??? Crear-BD.bat                   ? Script Batch automático
??? INSTRUCCIONES-CREAR-BD.md      ? Instrucciones detalladas
```

---

## ?? CREDENCIALES CONFIGURADAS

```
Servidor:    ADMINISTRADOR\POOL
Usuario:     sa
Contraseña:  123456
BD:          NexShopDb
```

**appsettings.json ya actualizado ?**

---

## ? LO QUE CREA EL SCRIPT

### 11 Tablas
- AspNetRoles, AspNetUsers, AspNetUserRoles
- Categorias, Productos, Multimedia
- Ordenes, OrdenDetalles
- Preguntas, Respuestas, Calificaciones

### 10 Índices
- Para búsquedas rápidas de productos, órdenes, preguntas

### 3 Roles
- Admin, Vendedor, Comprador

### 5 Categorías
- Electrónica, Ropa, Hogar, Deportes, Libros

---

## ?? DESPUÉS DE CREAR LA BD

```powershell
# Compilar
dotnet build

# Ejecutar
dotnet run

# Abrir navegador
http://localhost:5217
```

---

## ? ¡LISTO!

Todo está configurado. Solo ejecuta el script y la BD se crea automáticamente.

**Tiempo total:** ?? 30 segundos
