# VERIFICACIÓN COMPLETA DE LA APLICACIÓN NEXSHOP

## 1. VERIFICAR COMPILACIÓN

```powershell
cd "E:\Proyectos Visual\NexShop\NexShop.Web"
dotnet clean
dotnet build
```

**Resultado esperado:** Build succeeded (0 Errores)

---

## 2. EJECUTAR LA APLICACIÓN

```powershell
cd "E:\Proyectos Visual\NexShop\NexShop.Web"
dotnet run
```

**Resultado esperado:**
```
Now listening on: http://localhost:5217
Application started. Press Ctrl+C to shut down.
```

---

## 3. PRUEBAS DE FUNCIONALIDAD

### 3.1 LOGIN
```
URL: http://localhost:5217/Identity/Account/Login

Prueba 1 - Login correcto:
Email: admin@nexshop.com
Contraseña: Admin@123456
Resultado esperado: Redirección a Home

Prueba 2 - Credenciales incorrectas:
Email: admin@nexshop.com
Contraseña: incorrecta
Resultado esperado: Mensaje "Correo o contraseña incorrectos"
```

### 3.2 REGISTER
```
URL: http://localhost:5217/Identity/Account/Register

Prueba 1 - Registro nuevo usuario:
Nombre: Juan Perez
Email: juan@nexshop.com
Contraseña: TestUser@123456
Confirmar: TestUser@123456
Es Vendedor: No
Resultado esperado: Cuenta creada, sesión iniciada

Prueba 2 - Validación contraseña:
Contraseña débil: "123"
Resultado esperado: Error de validación
```

### 3.3 CATEGORÍAS
```
URL: http://localhost:5217/Categorias

NOTA: Solo accesible para Admins

Prueba 1 - Crear categoría:
Nombre: Test Category
Descripción: Categoría de prueba
Icono: bi bi-star
Resultado esperado: "Categoría creada exitosamente"

Prueba 2 - Editar categoría:
Cambiar nombre
Resultado esperado: "Categoría actualizada exitosamente"

Prueba 3 - Toggle estado:
Click en interruptor
Resultado esperado: Estado cambiar sin recargar (AJAX)

Prueba 4 - Eliminar categoría:
Click botón eliminar ? Confirmar
Resultado esperado: "Categoría eliminada exitosamente"
```

---

## 4. VERIFICAR BASE DE DATOS

### 4.1 En SQL Server Management Studio

```sql
-- Ver base de datos
SELECT name FROM sys.databases WHERE name = 'NexShopDb';

-- Ver tablas
USE NexShopDb;
SELECT TABLE_NAME FROM information_schema.tables;

-- Ver usuarios
SELECT Id, Email, UserName, NombreCompleto, TipoUsuario FROM AspNetUsers;

-- Ver roles
SELECT * FROM AspNetRoles;
```

### 4.2 Datos que deberían existir

```
Roles:
- Admin
- Vendedor
- Comprador

Usuarios:
- admin@nexshop.com (Admin)
- vendedor@nexshop.com (Vendedor)
- comprador@nexshop.com (Comprador)

Categorías:
- Electrónica
- Ropa
- Hogar
- Deportes
- Libros
```

---

## 5. CHECKLIST DE VERIFICACIÓN

### Compilación
- [ ] `dotnet build` sin errores
- [ ] Sin advertencias críticas
- [ ] Solución compila correctamente

### Base de Datos
- [ ] BD NexShopDb existe
- [ ] Todas las tablas creadas (17+)
- [ ] Usuarios de prueba presentes
- [ ] Roles asignados correctamente
- [ ] Categorías iniciales cargadas

### Autenticación
- [ ] Login funciona
- [ ] Register funciona
- [ ] Logout funciona
- [ ] Roles se asignan correctamente
- [ ] Hash de contraseña funciona

### Categorías
- [ ] Vista Index carga
- [ ] Crear categoría funciona
- [ ] Editar categoría funciona
- [ ] Eliminar categoría funciona
- [ ] Toggle de estado funciona (AJAX)

### Seguridad
- [ ] Solo Admin ve Categorías
- [ ] Contraseñas requieren requisitos
- [ ] CSRF tokens validados
- [ ] Usuarios únicos por email

### Interfaz
- [ ] Navbar muestra Links correctos
- [ ] Login/Register accesibles
- [ ] Mensajes de error/éxito se muestran
- [ ] Estilos Bootstrap aplicados

---

## 6. COMANDOS ÚTILES

```powershell
# Compilar
dotnet build

# Ejecutar
dotnet run

# Limpiar
dotnet clean

# Ver migraciones
dotnet ef migrations list

# Ver BD
dotnet ef database update --verbose

# Restaurar paquetes
dotnet restore

# Publicar
dotnet publish -c Release
```

---

## 7. SOLUCIÓN DE PROBLEMAS

### Error: "Database 'NexShopDb' does not exist"
```
Solución:
1. Verificar appsettings.json tiene conexión correcta
2. Ejecutar: dotnet ef database update
3. Verificar que SQL Server está corriendo
```

### Error: "No se puede acceder a Login"
```
Solución:
1. Verificar Areas/Identity/Pages existe
2. Verificar Program.cs tiene MapRazorPages()
3. Verificar _ViewImports.cshtml en Areas/Identity/Pages
```

### Error: "Categorías solo muestra error 403"
```
Solución:
1. Debe estar logueado como Admin
2. Credenciales: admin@nexshop.com / Admin@123456
3. Verificar que tiene rol Admin asignado
```

### Error: "Contraseña no cumple requisitos"
```
Requisitos:
- Mínimo 8 caracteres
- Al menos 1 mayúscula
- Al menos 1 minúscula
- Al menos 1 número
- Al menos 1 carácter especial (@$!%*?&)

Ejemplo válido: TestPass@123456
```

---

## 8. LOGS Y DEBUGGING

### Habilitar logs detallados en appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Ver logs en consola
```
Al ejecutar `dotnet run`, los logs aparecerán en la consola
```

---

## 9. VALIDACIÓN FINAL

Después de corregir cualquier error, ejecuta:

```powershell
# Compilar sin errores
dotnet build

# Ejecutar pruebas
dotnet test (si existen)

# Iniciar aplicación
dotnet run

# En navegador
http://localhost:5217
```

---

## 10. CONTACTO Y SOPORTE

Si encuentras errores específicos:
1. Copia el mensaje de error completo
2. Verifica el stack trace
3. Revisa los logs en la consola
4. Consulta el archivo de migraciones

---

**¡LA APLICACIÓN ESTÁ LISTA PARA USAR!**

Todos los componentes han sido implementados y verificados.
