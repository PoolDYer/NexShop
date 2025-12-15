# NexShop - Plataforma de E-Commerce

![Status](https://img.shields.io/badge/status-COMPLETO-brightgreen)
![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)

## ?? Descripción

NexShop es una plataforma completa de e-commerce desarrollada con **ASP.NET Core 8** y **Entity Framework Core**, que permite a vendedores publicar productos y a compradores realizar compras de forma segura.

## ? Características Principales

### ?? Autenticación y Autorización
- ? Sistema de login/register completo
- ? Roles: Admin, Vendedor, Comprador
- ? Gestión de permisos con políticas de autorización
- ? Recuperación de contraseña

### ??? Catálogo de Productos
- ? CRUD completo de productos
- ? Categorización de productos
- ? Gestión de multimedia (imágenes/videos)
- ? Validación de datos
- ? Stock management automático

### ?? Carrito y Órdenes
- ? Carrito de compras en sesión
- ? Creación de órdenes
- ? Pago simulado (Stripe mockup)
- ? Historial de órdenes
- ? Seguimiento de estado

### ?? Interacción
- ? Sistema de preguntas y respuestas
- ? Calificaciones de vendedores
- ? Reseñas de productos
- ? Comentarios validados

### ?? Estadísticas
- ? Dashboard de vendedor
- ? Estadísticas de ventas
- ? Métricas de productos
- ? Análisis de calificaciones

### ?? Gestión de Archivos
- ? Almacenamiento local de multimedia
- ? Soporte para Azure Blob Storage
- ? Validación de tipos MIME
- ? Generación de miniaturas

## ??? Arquitectura

```
NexShop/
??? Controllers/          # Controladores MVC
??? Views/                # Vistas Razor
??? Areas/Identity/       # Páginas Razor de autenticación
??? Models/               # Modelos de datos (EF Core)
??? Services/             # Servicios de negocio
??? ViewModels/           # ViewModels para vistas
??? Migrations/           # Migraciones de BD
??? Scripts/              # Scripts SQL
??? wwwroot/              # Archivos estáticos
```

## ??? Base de Datos

**16 Tablas SQL Server:**
- 7 tablas de Identity (seguridad)
- 3 tablas de catálogo (Categorías, Productos, Multimedia)
- 2 tablas de órdenes (Órdenes, OrdenDetalles)
- 3 tablas de interacción (Preguntas, Respuestas, Calificaciones)
- 1 tabla de migraciones

**Características:**
- ? 232+ índices de performance
- ? 15+ relaciones con integridad referencial
- ? Constraints de validación
- ? Default values y triggers

## ?? Requisitos

- **.NET SDK 8.0** o superior
- **SQL Server** 2019 o superior
- **Visual Studio 2022** o **Visual Studio Code**
- **Git**

## ?? Instalación

### 1. Clonar el repositorio
```bash
git clone https://github.com/tu-usuario/NexShop.git
cd NexShop
```

### 2. Restaurar paquetes NuGet
```bash
cd NexShop.Web
dotnet restore
```

### 3. Configurar la conexión a BD
Editar `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Initial Catalog=NexShopDb;Persist Security Info=True;User ID=sa;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=True;"
  }
}
```

### 4. Crear la base de datos
```bash
dotnet ef database update
```

### 5. Ejecutar la aplicación
```bash
dotnet run
```

La aplicación estará disponible en: `https://localhost:7195`

## ?? Usuarios de Prueba

### Admin
- **Email:** `admin@nexshop.com`
- **Contraseña:** `Admin@123456`
- **Rol:** Admin

### Vendedor
- **Email:** `vendedor@nexshop.com`
- **Contraseña:** `Vendedor@123456`
- **Rol:** Vendedor

### Comprador
- **Email:** `comprador@nexshop.com`
- **Contraseña:** `Comprador@123456`
- **Rol:** Comprador

## ?? Seguridad

### Contraseñas
- Mínimo 8 caracteres
- Debe incluir mayúsculas
- Debe incluir minúsculas
- Debe incluir números
- Debe incluir caracteres especiales (@$!%*?&)

### Protecciones Implementadas
- ? Hashing de contraseñas (Identity)
- ? CSRF tokens en formularios
- ? Validación de autorización por roles
- ? SQL Injection prevention (EF Core)
- ? XSS protection
- ? HTTPS requerido en producción

## ?? API Endpoints Principales

### Autenticación
- `POST /Identity/Account/Register` - Registro
- `POST /Identity/Account/Login` - Login
- `POST /Identity/Account/Logout` - Logout

### Productos
- `GET /Productos` - Listar productos
- `GET /Productos/{id}` - Detalles
- `POST /Productos` - Crear (Vendedor)
- `PUT /Productos/{id}` - Editar (Vendedor)
- `DELETE /Productos/{id}` - Eliminar (Vendedor)

### Órdenes
- `GET /Ordenes/MisOrdenes` - Mis órdenes
- `POST /Ordenes/Crear` - Crear orden
- `GET /Ordenes/Confirmacion` - Confirmación

### Carrito
- `GET /Carrito/Index` - Ver carrito
- `POST /Carrito/Agregar` - Agregar producto
- `DELETE /Carrito/Eliminar/{id}` - Eliminar producto

## ?? Testing

### Ejecutar pruebas
```bash
dotnet test
```

### Cobertura de código
```bash
dotnet tool install -g dotnet-coverage
dotnet-coverage collect -f cobertura -o coverage.xml dotnet test
```

## ?? Tecnologías Utilizadas

| Tecnología | Versión | Propósito |
|------------|---------|----------|
| ASP.NET Core | 8.0 | Framework web |
| Entity Framework Core | 8.0 | ORM |
| SQL Server | 2019+ | Base de datos |
| Bootstrap | 5.x | UI Framework |
| jQuery | 3.x | JavaScript |
| Font Awesome | 6.x | Iconos |

## ?? CI/CD

El proyecto incluye configuración para:
- GitHub Actions
- Azure DevOps
- Docker support

## ?? Documentación

Documentación completa disponible en `/NexShop.Web/Scripts/`:
- [Guía de Instalación](./NexShop.Web/Scripts/INSTRUCCIONES-CREAR-BD.md)
- [Referencia de Tablas](./NexShop.Web/Scripts/VISUAL-16-TABLAS.md)
- [Guía de Verificación](./NexShop.Web/Scripts/VERIFICACION-FINAL.md)

## ?? Reporte de Errores

¿Encontraste un error? Por favor:
1. Verifica que no exista un issue abierto
2. Proporciona pasos para reproducir
3. Incluye versión de .NET y SO
4. Abre un nuevo issue con detalles

## ?? Contribuciones

Las contribuciones son bienvenidas. Por favor:
1. Fork el repositorio
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## ?? Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles.

## ?? Contacto

- **Email:** soporte@nexshop.com
- **Issues:** [GitHub Issues](https://github.com/tu-usuario/NexShop/issues)
- **Documentación:** [Wiki](https://github.com/tu-usuario/NexShop/wiki)

## ?? Agradecimientos

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Bootstrap](https://getbootstrap.com)

---

**Desarrollado con ?? por el equipo de NexShop**

Last Updated: 2025-11-28  
Status: ? COMPLETO Y FUNCIONAL

