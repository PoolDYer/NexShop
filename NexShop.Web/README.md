# ?? GUÍA COMPLETA - APLICACIÓN NEXSHOP

**Versión:** 1.0  
**Fecha:** 2025-11-27  
**Estado:** ? LISTO PARA PRODUCCIÓN

---

## ?? TABLA DE CONTENIDOS

1. [Inicio Rápido](#inicio-rápido)
2. [Requisitos del Sistema](#requisitos-del-sistema)
3. [Instalación y Configuración](#instalación-y-configuración)
4. [Ejecución de la Aplicación](#ejecución-de-la-aplicación)
5. [Pruebas](#pruebas)
6. [Solución de Problemas](#solución-de-problemas)
7. [Arquitectura](#arquitectura)
8. [Base de Datos](#base-de-datos)

---

## ? INICIO RÁPIDO

### Opción 1: Script PowerShell (Recomendado)

```powershell
# Verificar que todo esté correctamente
.\verificar-nexshop.ps1

# Ejecutar la aplicación
.\ejecutar-nexshop.ps1
```

### Opción 2: Comando Manual

```bash
cd "E:\Proyectos Visual\NexShop\NexShop.Web"
dotnet run
```

### Resultado Esperado

```
Now listening on: http://localhost:5217
Application started. Press Ctrl+C to shut down.
```

---

## ??? REQUISITOS DEL SISTEMA

### Software Requerido

- ? **.NET 8.0+**
  ```powershell
  dotnet --version
  ```

- ? **SQL Server 2022+ o SQL Server Express**
  - Instancia: `ADMINISTRATOR\MSSQLSERVER2025`
  - Usuario: `sa`
  - Contraseña: `123456`

- ? **Visual Studio 2022 o VS Code**

- ? **Entity Framework Core CLI** (para migraciones)
  ```powershell
  dotnet ef --version
  ```

### Hardware Recomendado

- RAM: 4GB mínimo, 8GB recomendado
- Disco: 500MB espacio libre
- CPU: Procesador moderno (Intel i5 o equivalente)

---

## ?? INSTALACIÓN Y CONFIGURACIÓN

### Paso 1: Clonar/Obtener el Proyecto

```bash
cd E:\Proyectos Visual\NexShop
# Ya deberías tener el proyecto aquí
```

### Paso 2: Verificar Configuración

**Archivo:** `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=ADMINISTRATOR\\MSSQLSERVER2025;Database=NexShopDb;User Id=sa;Password=123456;MultipleActiveResultSets=true;Encrypt=true;TrustServerCertificate=true"
  }
}
```

### Paso 3: Restaurar Paquetes NuGet

```powershell
cd "E:\Proyectos Visual\NexShop\NexShop.Web"
dotnet restore
```

### Paso 4: Crear Base de Datos (Automático)

La BD se crea automáticamente al ejecutar `dotnet run`

O manualmente:
```powershell
dotnet ef database update
```

### Paso 5: Verificar Compilación

```powershell
dotnet build
```

---

## ?? EJECUCIÓN DE LA APLICACIÓN

### Método 1: PowerShell Script

```powershell
cd "E:\Proyectos Visual\NexShop\NexShop.Web"
.\ejecutar-nexshop.ps1
```

### Método 2: Línea de Comandos

```powershell
cd "E:\Proyectos Visual\NexShop\NexShop.Web"
dotnet run
```

### Método 3: Visual Studio

1. Abre `NexShop.Web.sln`
2. Presiona `F5` (Debug) o `Ctrl+F5` (Sin Debug)

### Método 4: Visual Studio Code

```powershell
cd "E:\Proyectos Visual\NexShop\NexShop.Web"
code .
# Presiona F5 para ejecutar
```

---

## ? PRUEBAS

### 1. Verificar que la Aplicación Inicia

```
? Ver en navegador: http://localhost:5217
? Debe mostrar página de inicio de NexShop
```

### 2. Probar Login

**URL:** `http://localhost:5217/Identity/Account/Login`

```
Credencial Admin:
- Email: admin@nexshop.com
- Contraseña: Admin@123456

Resultado esperado:
? Inicio de sesión exitoso
? Redirección a página principal
? Navbar muestra nombre del usuario
```

### 3. Probar Registro

**URL:** `http://localhost:5217/Identity/Account/Register`

```
Nuevo Usuario:
- Nombre: Test User
- Email: test@example.com
- Contraseña: TestPassword@123456
- Confirmar: TestPassword@123456
- Tipo: Comprador

Resultado esperado:
? Cuenta creada
? Sesión iniciada automáticamente
```

### 4. Probar Categorías

**URL:** `http://localhost:5217/Categorias`

```
NOTA: Solo Admin puede acceder

Prueba 1 - Crear:
- Nombre: Tech Category
- Descripción: Productos tecnológicos
- Icono: bi bi-laptop

Resultado esperado:
? Categoría visible en la lista

Prueba 2 - Editar:
? Botón Editar funciona
? Cambios se guardan

Prueba 3 - Eliminar:
? Confirmación aparece
? Categoría se elimina
```

### 5. Probar Productos

**URL:** `http://localhost:5217/Productos`

```
? Ver listado de productos
? Ver detalles de producto
? Si eres vendedor: crear producto
```

### 6. Probar Carrito

```
? Agregar producto al carrito
? Badge del carrito actualiza
? Ver carrito: /Carrito
```

---

## ?? SOLUCIÓN DE PROBLEMAS

### Error: "Cannot connect to SQL Server"

**Causa:** SQL Server no está corriendo o credenciales incorrectas

**Solución:**

```powershell
# Verificar que SQL Server está corriendo
Get-Service MSSQLSERVER2025

# Si no está corriendo, iniciar:
Start-Service MSSQLSERVER2025

# Verificar conexión manualmente con SQL Server Management Studio
# Server: ADMINISTRATOR\MSSQLSERVER2025
# Authentication: SQL Server
# Login: sa
# Password: 123456
```

### Error: "Port 5217 is already in use"

**Causa:** Otra aplicación está usando el puerto

**Solución:**

```powershell
# Encontrar qué proceso usa el puerto 5217
netstat -ano | findstr :5217

# Matar el proceso
taskkill /PID <PID> /F

# O cambiar el puerto en Program.cs:
# builder.WebHost.UseUrls("http://localhost:5218");
```

### Error: "Database 'NexShopDb' does not exist"

**Causa:** BD no ha sido creada

**Solución:**

```powershell
cd "E:\Proyectos Visual\NexShop\NexShop.Web"
dotnet ef database update --verbose
```

### Error: "Migration InitialCreate not applied"

**Causa:** Migraciones no se han aplicado

**Solución:**

```powershell
# Listar migraciones
dotnet ef migrations list

# Aplicar todas las migraciones
dotnet ef database update

# O si necesitas limpiar y recrear:
dotnet ef database drop
dotnet ef database update
```

### Error: "Login/Register no está accesible"

**Causa:** Páginas Razor Pages no se encuentran

**Solución:**

1. Verificar que existe: `Areas/Identity/Pages/Account/Login.cshtml`
2. Verificar que existe: `Areas/Identity/Pages/_ViewImports.cshtml`
3. Recompilar: `dotnet build`

### Error: "403 Forbidden en Categorías"

**Causa:** No tienes rol de Admin

**Solución:**

```
1. Inicia sesión como admin@nexshop.com
2. O verifica que el usuario tiene rol Admin en BD:

SELECT * FROM AspNetUserRoles 
WHERE UserId = (SELECT Id FROM AspNetUsers WHERE Email = 'email@test.com')
```

### Error: "Contraseña no cumple requisitos"

**Requisitos de Contraseña:**

```
? Mínimo 8 caracteres
? Al menos 1 MAYÚSCULA
? Al menos 1 minúscula
? Al menos 1 número
? Al menos 1 carácter especial (@$!%*?&)

Ejemplo válido: Test@Pass123
Ejemplo inválido: test123 (sin mayúscula, sin especial)
```

---

## ??? ARQUITECTURA

### Estructura del Proyecto

```
NexShop.Web/
??? Models/                    # Entidades de base de datos
?   ??? Usuario.cs
?   ??? Producto.cs
?   ??? Categoria.cs
?   ??? Orden.cs
?   ??? Pregunta.cs
?   ??? Respuesta.cs
?   ??? NexShopContext.cs
??? Controllers/              # Controladores MVC
?   ??? CategoriasController.cs
?   ??? ProductosController.cs
?   ??? CarritoController.cs
?   ??? PreguntasController.cs
??? Views/                    # Vistas Razor
?   ??? Categorias/
?   ??? Productos/
?   ??? Carrito/
?   ??? Shared/
??? Areas/                    # Áreas (Identity, etc.)
?   ??? Identity/
?       ??? Pages/
?           ??? Account/
??? Services/                 # Lógica de negocio
?   ??? ProductoService.cs
?   ??? PreguntaService.cs
?   ??? MultimediaService.cs
??? ViewModels/              # DTOs para vistas
??? Migrations/              # Migraciones EF Core
??? wwwroot/                 # Archivos estáticos
```

### Flujo de Aplicación

```
Usuario
   ?
[Navegador] http://localhost:5217
   ?
[IIS Express / Kestrel]
   ?
[Program.cs] - Configura servicios
   ?
[Middleware] - Autenticación, Autorización
   ?
[Controlador] - Procesa la solicitud
   ?
[Servicio] - Lógica de negocio
   ?
[DbContext] - Acceso a datos
   ?
[SQL Server] - Base de datos
   ?
[Respuesta HTML/JSON]
```

---

## ?? BASE DE DATOS

### Conexión

```
Servidor: ADMINISTRATOR\MSSQLSERVER2025
Base de Datos: NexShopDb
Usuario: sa
Contraseña: 123456
```

### Tablas Principales

```
AspNetUsers              - Usuarios del sistema
AspNetRoles             - Roles (Admin, Vendedor, Comprador)
AspNetUserRoles         - Asignación de roles
Categorias              - Categorías de productos
Productos               - Catálogo de productos
Multimedia              - Imágenes y videos
Ordenes                 - Órdenes de compra
OrdenDetalles           - Detalles de órdenes
Preguntas               - Q&A - Preguntas
Respuestas              - Q&A - Respuestas
Calificaciones          - Reputación de vendedor
```

### Datos Iniciales

**Roles:**
- Admin
- Vendedor
- Comprador

**Usuarios:**
- admin@nexshop.com (Admin)
- vendedor@nexshop.com (Vendedor)
- comprador@nexshop.com (Comprador)

**Categorías:**
- Electrónica
- Ropa
- Hogar
- Deportes
- Libros

---

## ?? SEGURIDAD

### Implementado

- ? ASP.NET Core Identity
- ? Hashing de contraseñas
- ? CSRF Protection
- ? Roles y Autorización
- ? HTTPS
- ? Input Validation
- ? SQL Injection Protection (EF Core)
- ? XSS Protection

### Credenciales

```
Contraseñas de prueba:
- Admin@123456
- Vendedor@123456
- Comprador@123456

NOTA: Cambiar en producción
```

---

## ?? COMANDOS ÚTILES

### Compilación

```powershell
# Compilar proyecto
dotnet build

# Compilar modo release
dotnet build -c Release

# Compilar verbose
dotnet build --verbosity detailed
```

### Ejecución

```powershell
# Ejecutar
dotnet run

# Ejecutar en release
dotnet run -c Release

# Ejecutar con puertos específicos
dotnet run --urls "http://localhost:5300"
```

### Entity Framework

```powershell
# Crear migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones
dotnet ef database update

# Ver migraciones
dotnet ef migrations list

# Revertir migración
dotnet ef migrations remove

# Actualizar base de datos
dotnet ef database update --verbose
```

### Limpiar

```powershell
# Limpiar compilaciones
dotnet clean

# Eliminar carpeta bin
Remove-Item -Recurse bin

# Eliminar carpeta obj
Remove-Item -Recurse obj
```

---

## ?? CONTACTO Y SOPORTE

Si encuentras problemas:

1. **Verifica los logs** en la consola
2. **Revisa el archivo** `CORRECCION_ERRORES_REPORTE.md`
3. **Consulta la BD** usando SQL Server Management Studio
4. **Prueba la compilación** con `dotnet build`

---

## ? CARACTERÍSTICAS PRINCIPALES

- ? Sistema de autenticación completo
- ? Gestión de categorías (Admin)
- ? Catálogo de productos
- ? Carrito de compras
- ? Sistema de órdenes
- ? Sistema de Q&A (Preguntas y Respuestas)
- ? Termómetro de reputación
- ? Gestión de multimedia
- ? Roles y autorizaciones
- ? Interfaz responsive
- ? Bootstrap 5

---

## ?? ¡LISTO PARA USAR!

Tu aplicación NexShop está completamente configurada y lista para ser utilizada.

**¡Que disfrutes desarrollando!** ??

---

*Documentación generada automáticamente*  
*Última actualización: 2025-11-27*
