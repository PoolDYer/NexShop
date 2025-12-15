# ??? Sistema de Imágenes Responsivo - Guía Completa

## ? Lo que se ha implementado

### 1. **Servicio de Sincronización de Imágenes** (`SincronizacionImagenesService`)
- Lee imágenes desde carpetas de productos en `/wwwroot/uploads/productos/`
- Copia imágenes automáticamente a `/wwwroot/imagenes/productos/`
- Proporciona métodos para obtener rutas de imágenes

### 2. **Servicio de Sincronización de Multimedia** (`SincronizacionMultimediaService`)
- Crea registros automáticos de `Multimedia` en la BD
- Asocia imágenes con productos
- Marca la primera imagen como principal

### 3. **Controller de Imágenes** (`ImagenesController`)
- **GET `/api/imagenes/producto/{id}`** - Obtiene todas las imágenes de un producto
- **GET `/api/imagenes/producto/{id}/imagen/{archivo}`** - Obtiene URL de una imagen específica
- **POST `/api/imagenes/sincronizar`** - Sincroniza archivos desde carpetas
- **POST `/api/imagenes/sincronizar-multimedia`** - Sincroniza multimedia en BD

### 4. **CSS Responsive** (`galeria-productos-responsive.css`)

#### Características:
- ? **Aspect Ratio 1:1** - Las imágenes mantienen proporciones sin distorsión
- ? **object-fit: contain** - Las imágenes se escalan sin corte
- ? **Responsive Breakpoints** - Adaptado para:
  - Desktop (1200px+)
  - Tablets (768px - 1199px)
  - Mobile (576px - 767px)
  - Mobile pequeño (360px - 575px)

#### Componentes Responsivos:
- Carrusel de imágenes principal
- Miniaturas scrolleables
- Grid de productos
- Soporte para modo oscuro
- Accesibilidad mejorada

### 5. **Views Actualizados**

#### `Views/Productos/Details.cshtml`
- Galería con carrusel responsivo
- Miniaturas interactivas
- Imágenes lazy-loaded
- Fallback si no hay imágenes

#### `Views/Productos/Index.cshtml`
- Grid responsivo de productos
- Imágenes sin distorsión
- Badges de estado y envío
- Información de stock

---

## ?? Cómo Usar

### Paso 1: Copiar Imágenes a Carpetas
```
E:\Proyectos Visual\NexShop\NexShop.Web\wwwroot\uploads\productos\
??? 1_Smartphone XYZ Pro\
?   ??? imagen1.jpg
?   ??? imagen2.png
?   ??? imagen3.webp
??? 2_Laptop Gaming 15\
?   ??? laptop.jpg
??? ...
```

### Paso 2: Ejecutar Script de Sincronización
```powershell
# En PowerShell
powershell -ExecutionPolicy Bypass -File ".\sincronizar-imagenes.ps1"
```

El script:
1. ? Crea estructura de carpetas en `/wwwroot/imagenes/productos/`
2. ? Copia todas las imágenes de origen a destino
3. ? Verifica si la app está corriendo
4. ? Sincroniza multimedia en BD (si app está corriendo)

### Paso 3: Ejecutar la Aplicación
```bash
cd "E:\Proyectos Visual\NexShop\NexShop.Web"
dotnet run
```

### Paso 4: Verificar en el Navegador
```
http://localhost:5217/Productos
```

- ? Ver listado de productos con imágenes
- ? Hacer click en "Ver Detalle" para ver galería
- ? Usar controles del carrusel

---

## ?? Endpoints API

### Obtener Imágenes de un Producto
```
GET /api/imagenes/producto/1
```

**Respuesta:**
```json
[
    {
        "nombre": "imagen1.jpg",
        "url": "/imagenes/productos/1_Smartphone XYZ Pro/imagen1.jpg"
    },
    {
        "nombre": "imagen2.png",
        "url": "/imagenes/productos/1_Smartphone XYZ Pro/imagen2.png"
    }
]
```

### Sincronizar Archivos
```
POST /api/imagenes/sincronizar
```

**Respuesta:**
```json
{
    "mensaje": "Sincronización completada exitosamente",
    "carpetasProcesadas": 50,
    "imagenesCopiadas": 150,
    "errores": 0
}
```

### Sincronizar Multimedia en BD
```
POST /api/imagenes/sincronizar-multimedia
```

**Respuesta:**
```json
{
    "mensaje": "Multimedia sincronizada exitosamente",
    "productosActualizados": 50,
    "multimediaAgregada": 150,
    "errores": 0
}
```

---

## ?? Responsividad - Breakpoints

### Desktop (1200px+)
- Grid: 3-4 columnas
- Imagen: 250px x 250px
- Thumbnails: 60px x 60px

### Tablet (768px - 1199px)
- Grid: 2-3 columnas
- Imagen: 200px x 200px
- Thumbnails: 50px x 50px

### Mobile (576px - 767px)
- Grid: 1-2 columnas
- Imagen: 150px x 150px
- Thumbnails: 45px x 45px

### Mobile Pequeño (360px - 575px)
- Grid: 2 columnas
- Imagen: adaptable
- Thumbnails: 40px x 40px

---

## ?? Características CSS

### Aspect Ratio
```css
aspect-ratio: 1 / 1;  /* Siempre cuadrado */
object-fit: contain;   /* Sin distorsión */
object-position: center; /* Centrado */
```

### Responsive
```css
@media (max-width: 768px) {
    .productos-grid {
        grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    }
}
```

### Lazy Loading
```html
<img src="..." loading="lazy" decoding="async">
```

### Accesibilidad
- ARIA labels
- Focus states
- Reduced motion support
- Alt text descriptivo

---

## ?? Estructura de Archivos

```
NexShop.Web/
??? Services/
?   ??? SincronizacionImagenesService.cs    ? Sincroniza archivos
?   ??? SincronizacionMultimediaService.cs  ? Sincroniza BD
??? Controllers/
?   ??? ImagenesController.cs               ? API endpoints
??? wwwroot/
?   ??? css/
?   ?   ??? galeria-productos-responsive.css ? Estilos
?   ??? imagenes/
?   ?   ??? productos/                       ? Imágenes sincronizadas
?   ?       ??? 1_Smartphone XYZ Pro/
?   ?       ??? 2_Laptop Gaming 15/
?   ?       ??? ...
?   ??? uploads/
?       ??? productos/                       ? Origen (lectura)
?           ??? 1_Smartphone XYZ Pro/
?           ??? 2_Laptop Gaming 15/
?           ??? ...
??? Views/
    ??? Productos/
        ??? Details.cshtml                   ? Galería detallada
        ??? Index.cshtml                     ? Grid de productos
```

---

## ?? Configuración en Program.cs

```csharp
// Servicios de imagen agregados
builder.Services.AddScoped<ISincronizacionImagenesService, SincronizacionImagenesService>();
builder.Services.AddScoped<ISincronizacionMultimediaService, SincronizacionMultimediaService>();
```

---

## ? Ventajas del Sistema

### Performance
- ? Lazy loading de imágenes
- ? Copias locales (no externa CDN)
- ? Async/await en servicios

### UX
- ? Imágenes responsivas en todos los dispositivos
- ? Sin distorsión de imágenes
- ? Carrusel interactivo
- ? Miniaturas destacadas

### Mantenibilidad
- ? Sincronización automática
- ? API centralizada
- ? Logging completo
- ? Errores manejados

### Accesibilidad
- ? Alt text descriptivo
- ? ARIA labels
- ? Soporte modo oscuro
- ? Reduced motion

---

## ?? Troubleshooting

### Las imágenes no aparecen
1. Verifica que las imágenes estén en `/wwwroot/uploads/productos/`
2. Ejecuta el script de sincronización
3. Verifica permisos de lectura/escritura
4. Comprueba la consola para errores

### Errores de sincronización
1. Verifica que la app esté corriendo en http://localhost:5217
2. Revisa los logs en la consola
3. Intenta acceder directamente a `/api/imagenes/producto/1`

### Imágenes distorsionadas
- El CSS ya incluye `object-fit: contain` y `aspect-ratio: 1`
- No deberían ocurrir distorsiones
- Si ocurren, limpia caché del navegador (Ctrl+F5)

---

## ?? Documentación de Servicios

### ISincronizacionImagenesService

```csharp
// Sincronizar todas las imágenes
var resultado = await _sincronizacionImagenesService.SincronizarTodasLasImagenes();

// Obtener imágenes de un producto
var imagenes = await _sincronizacionImagenesService.ObtenerImagenesProducto(1);

// Obtener URL relativa
var url = _sincronizacionImagenesService.ObtenerRutaImagenProducto(1, "foto.jpg");
// Retorna: /imagenes/productos/1_Nombre/foto.jpg
```

### ISincronizacionMultimediaService

```csharp
// Sincronizar toda la multimedia
var resultado = await _sincronizacionMultimediaService.SincronizarTodasLasImagenes();

// Sincronizar un producto específico
var cantidad = await _sincronizacionMultimediaService.SincronizarImagenesProducto(1);
```

---

## ?? Próximos Pasos Recomendados

1. **Agregar imágenes reales** a las carpetas de productos
2. **Ejecutar script** de sincronización
3. **Probar responsividad** en diferentes dispositivos
4. **Optimizar imágenes** para web (WebP, compresión)
5. **Agregar galería de zoom** (opcional)

---

**Última actualización:** 28/11/2025  
**Estado:** ? **100% COMPLETADO**
