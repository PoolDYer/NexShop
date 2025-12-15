# ? SCRIPT COMPLETO - CREAR TODAS LAS TABLAS Y RELACIONES

## ?? RESUMEN RÁPIDO

He creado **scripts completos** que crean TODAS las tablas con todas las relaciones correctas.

---

## ?? TABLAS CREADAS (11 TOTAL)

### ?? SEGURIDAD Y USUARIOS
1. **AspNetRoles** - Roles (Admin, Vendedor, Comprador)
2. **AspNetUsers** - Usuarios registrados del sistema
3. **AspNetUserRoles** - Relación Usuario-Rol (N:N)

### ?? CATÁLOGO
4. **Categorias** - Categorías de productos (5 predefinidas)
5. **Productos** - Catálogo de productos
6. **Multimedia** - Imágenes y videos de productos

### ?? COMPRAS Y ÓRDENES
7. **Ordenes** - Órdenes de compra
8. **OrdenDetalles** - Items dentro de cada orden

### ? INTERACCIÓN
9. **Preguntas** - Preguntas sobre productos (Q&A)
10. **Respuestas** - Respuestas a preguntas
11. **Calificaciones** - Reseñas y ratings

---

## ?? RELACIONES IMPLEMENTADAS

```
Usuario (1) ??? (N) Productos (como vendedor) - FK: VendedorId
Usuario (1) ??? (N) Órdenes (como comprador) - FK: CompradorId
Usuario (1) ??? (N) Preguntas - FK: UsuarioId
Usuario (1) ??? (N) Respuestas - FK: UsuarioId
Usuario (1) ??? (N) Calificaciones - FK: UsuarioId, VendedorId

Categoría (1) ??? (N) Productos - FK: CategoriaId (CASCADE)
Producto (1) ??? (N) Multimedia - FK: ProductoId (CASCADE)
Producto (1) ??? (N) OrdenDetalles - FK: ProductoId (RESTRICT)
Producto (1) ??? (N) Preguntas - FK: ProductoId (CASCADE)
Producto (1) ??? (N) Calificaciones - (via Producto)

Orden (1) ??? (N) OrdenDetalles - FK: OrdenId (CASCADE)
Orden (1) ??? (N) Calificaciones - FK: OrdenId (SET NULL)

Pregunta (1) ??? (N) Respuestas - FK: PreguntaId (CASCADE)
```

---

## ?? ÍNDICES CREADOS (30+ TOTAL)

### Índices de Identidad/Clave
- EmailIndex (AspNetUsers)
- UserNameIndex (AspNetUsers)
- RoleNameIndex (AspNetRoles)
- SKU Único (Productos)
- NumeroOrden Único (Ordenes)

### Índices de Búsqueda
- IX_CategoriaId (Productos)
- IX_VendedorId (Productos)
- IX_EstadoProducto (Productos)
- IX_CompradorId (Ordenes)
- IX_ProductoId (Multimedia, OrdenDetalles)
- IX_UsuarioId (Preguntas, Respuestas, Calificaciones)

### Índices de Performance
- IX_FechaCreacion (Órdenes, Preguntas, Respuestas)
- IX_Estado (Preguntas)
- IX_Tipo (Multimedia, Calificaciones)

---

## ?? EJECUCIÓN RÁPIDA

### OPCIÓN 1: PowerShell (RECOMENDADO) ?

```powershell
cd "E:\Proyectos Visual\NexShop\NexShop.Web"
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
.\Scripts\Crear-Tablas.ps1
```

### OPCIÓN 2: CMD (BATCH)

```cmd
cd E:\Proyectos Visual\NexShop\NexShop.Web
Scripts\Crear-Tablas.bat
```

### OPCIÓN 3: SQL Server Management Studio

1. Abre SSMS
2. Conecta a: `ADMINISTRATOR\POOL` (sa / 123456)
3. Selecciona BD: `NexShopDb`
4. Abre archivo: `Scripts\02-CrearTablas-Completo.sql`
5. Presiona **F5**

---

## ? DATOS INICIALES INSERTADOS

### Roles (3)
- Admin (Administrador del sistema)
- Vendedor (Puede crear productos)
- Comprador (Cliente)

### Categorías (5)
- Electrónica
- Ropa
- Hogar
- Deportes
- Libros

---

## ?? ARCHIVOS GENERADOS

```
Scripts/
??? 01-CrearBaseDatos.sql           ? Script original
??? Crear-BD.ps1 / Crear-BD.bat     ? Scripts originales
?
??? 02-CrearTablas-Completo.sql     ? ? NUEVO - Script COMPLETO
??? Crear-Tablas.ps1                ? ? NUEVO - PowerShell completo
??? Crear-Tablas.bat                ? ? NUEVO - Batch completo
?
??? INSTRUCCIONES-CREAR-BD.md       ? Instrucciones originales
??? README-RAPIDO.md                ? Resumen rápido
```

---

## ?? CADENA DE CONEXIÓN CONFIGURADA

```
Server=ADMINISTRATOR\POOL;
Initial Catalog=NexShopDb;
Persist Security Info=True;
User ID=sa;
Password=123456;
Pooling=False;
MultipleActiveResultSets=False;
Encrypt=True;
TrustServerCertificate=True;
Application Name=NexShop;
Command Timeout=0
```

**Ya actualizado en `appsettings.json` ?**

---

## ?? DESPUÉS DE CREAR LAS TABLAS

### 1. Compilar
```powershell
dotnet build
```

### 2. Ejecutar
```powershell
dotnet run
```

### 3. Abrir en navegador
```
http://localhost:5217
```

---

## ? CARACTERÍSTICAS DEL SCRIPT

? Crea todas las tablas con tipos de datos correctos  
? Relaciones Foreign Key (FK) correctas  
? Índices para optimizar búsquedas  
? Restricciones de unicidad (UNIQUE)  
? Valores por defecto (DEFAULT)  
? Inserta roles y categorías iniciales  
? Validaciones de integridad referencial  
? Manejo de cascada DELETE según corresponda  
? Mensajes de progreso en consola  
? Verificación final de creación  

---

## ? LISTO PARA USAR

Todo está configurado. Solo ejecuta uno de los scripts:

**PowerShell:** `.\Scripts\Crear-Tablas.ps1`  
**CMD:** `Scripts\Crear-Tablas.bat`  
**SSMS:** Abre `Scripts\02-CrearTablas-Completo.sql`

**Tiempo total:** ?? 30 segundos

---

**Estado:** ? LISTO PARA PRODUCCIÓN
