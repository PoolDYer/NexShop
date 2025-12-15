# ?? Sistema de Reseñas y Comentarios - Documentación

## ? Implementación Completada

Se ha agregado un sistema completo de reseñas y comentarios en la página de detalles del producto, permitiendo que los compradores califiquen la atención del vendedor, el envío y escriban comentarios.

---

## ?? Características Implementadas

### 1. **Formulario de Reseña**
```
? Título de la Reseña (5-100 caracteres)
? Calificación General (1-5 estrellas)
? Calificación de Atención (1-5 estrellas)
? Calificación de Envío (1-5 estrellas)
? Comentario Detallado (10-500 caracteres)
? Contador de caracteres dinámico
```

### 2. **Interfaz de Estrellas**
```
? Selección interactiva de estrellas
? Colores diferentes por tipo:
   - Amarillo para calificación general
   - Azul para atención
   - Verde para envío
? Hover effects
? Validación en tiempo real
```

### 3. **Visualización de Reseñas**
```
? Nombre del usuario
? Fecha y hora
? Título
? Calificación con estrellas
? Calificaciones secundarias (Atención/Envío)
? Comentario completo
? Indicador "Tu Reseña" si es del usuario actual
```

### 4. **Estadísticas de Reseñas**
```
? Calificación promedio
? Distribución de estrellas (gráfico)
? Total de reseñas
? Porcentajes por cada nivel
```

---

## ?? Archivos Modificados/Creados

### Creados
- ? `ViewModels/CrearReseñaViewModel.cs` - ViewModel para crear reseña
- ? `ViewModels/CrearReseñaViewModel.cs` (DTOs) - ReseñaProductoDto, EstadisticasReseñasDto

### Modificados
- ? `Models/Calificacion.cs` - Agregadas propiedades para reseñas
- ? `ViewModels/ProductoViewModel.cs` - ProductoDetailViewModel con reseñas
- ? `Controllers/PreguntasController.cs` - Acción CrearResena
- ? `Controllers/ProductosController.cs` - Details actualizado con reseñas
- ? `Views/Productos/Details.cshtml` - Sección completa de reseñas

---

## ?? Cambios en Base de Datos

### Tabla `Calificaciones` - Nuevas Columnas

```sql
-- Ya existían:
- CalificacionId (PK)
- Puntaje (1-5)
- Comentario (varchar 500)
- Tipo (varchar 30)
- VendedorId
- UsuarioId
- OrdenId
- FechaCreacion

-- AGREGADOS:
- ProductoId (FK) - Referencia al producto
- Titulo (varchar 100) - Título de la reseña
- CalificacionAtencion (int) - 1-5 estrellas
- CalificacionEnvio (int) - 1-5 estrellas
```

### Relación con Producto

```csharp
[ForeignKey(nameof(ProductoId))]
public virtual Producto? Producto { get; set; }
```

---

## ?? Flujo de Uso

### 1. **Usuario Anonimo**
```
Intenta escribir reseña
   ?
Ve mensaje: "Inicia sesión para escribir una reseña"
   ?
[Enlace a Login]
```

### 2. **Usuario Autenticado (Primera vez)**
```
Accede a Details del producto
   ?
Ve formulario de reseña
   ?
Completa todos los campos
   ?
Click "Enviar Reseña"
   ?
Reseña guardada ?
   ?
Recibe mensaje: "Reseña enviada exitosamente"
```

### 3. **Usuario Autenticado (Ya reseñó)**
```
Accede a Details del producto
   ?
Ve mensaje: "Ya escribiste una reseña para este producto"
   ?
Puede ver su reseña con badge "Tu Reseña"
```

---

## ?? Estructura de Formulario

```html
Título de Reseña: [Campo texto 5-100 caracteres]

Calificación General: [? ? ? ? ?]

Calificación Atención: [?? ?? ?? ?? ??]

Calificación Envío: [? ? ? ? ?]

Comentario: [Área texto 10-500 caracteres]
           [Contador: 0/500]

[ENVIAR RESEÑA]
```

---

## ?? Validaciones

### Frontend
```
? Título: 5-100 caracteres (required)
? Calificación General: 1-5 (required)
? Calificación Atención: 1-5 (required)
? Calificación Envío: 1-5 (required)
? Comentario: 10-500 caracteres (required)
? Contador dinámico de caracteres
? Buttons habilitados solo cuando hay datos
```

### Backend
```
? ModelState válido
? Usuario autenticado
? Producto existe
? Usuario NO ha reseñado ya
? Rango de calificaciones 1-5
? Longitud de comentario 10-500
```

---

## ?? Visualización de Estadísticas

### Resumen de Reseñas
```
???????????????????????????????????????
? 4.5                                 ?
? ? ? ? ? ?                      ?
? (125 reseñas)                       ?
???????????????????????????????????????
```

### Distribución
```
?????  75 reseñas  ???????????????? 60%
?????   30 reseñas  ??????????????? 24%
?????   15 reseñas  ??????????????? 12%
?????   3 reseñas   ??????????????? 2%
?????    2 reseñas   ??????????????? 2%
```

---

## ?? Elementos Visuales

### Tarjeta de Reseña
```
???????????????????????????????????????
? "Excelente calidad"    [Tu Reseña] ?
? Por Juan Pérez el 28/11/2024 16:30 ?
?                                     ?
? ? ? ? ? ? 5.0 / 5.0            ?
?                                     ?
? Atención: ?? ?? ?? ?? ??          ?
? Envío:    ? ? ? ? ?            ?
?                                     ?
? "Producto de excelente calidad,    ?
? vendedor muy atento y entrega      ?
? rápida. Muy recomendado."          ?
???????????????????????????????????????
```

---

## ?? Seguridad

```
? Solo usuarios autenticados pueden escribir
? Un usuario, una reseña por producto
? No se puede editar/eliminar reseña (integridad)
? Los datos se validan en backend
? CSRF token en formulario
? Logs de auditoría
```

---

## ?? Responsividad

```
Desktop (> 1200px):
?? Formulario en 1 columna
?? Estrellas con espacio
?? Reseñas en grid

Tablet (768-1200px):
?? Formulario ajustado
?? Estrellas responsivas
?? Reseñas en 1-2 columnas

Mobile (< 768px):
?? Formulario full-width
?? Estrellas en línea
?? Reseñas apiladas
```

---

## ?? Integración con Otros Sistemas

### Relacionado con
- ? Sistema de Calificaciones (existente)
- ? Sistema de Órdenes (para contexto)
- ? Sistema de Usuarios (para autenticación)
- ? Sistema de Productos (para reseñas)

### Datos Referenciados
```
ReseñaProducto ? Producto (ProductoId)
              ? Usuario (UsuarioId)
              ? Vendedor (VendedorId)
              ? Orden (OrdenId, opcional)
```

---

## ?? ActionResult

### CrearResena Action

```csharp
[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CrearResena(CrearReseñaViewModel viewModel)
{
    // POST: /Preguntas/CrearResena
    // Body: {
    //   productoId: 1,
    //   calificacionGeneral: 5,
    //   calificacionAtencion: 4,
    //   calificacionEnvio: 5,
    //   titulo: "Excelente",
    //   comentario: "Muy buen producto..."
    // }
    
    // Return:
    // Success: { exito: true, mensaje: "Reseña enviada exitosamente" }
    // Error: { error: "Descripción del error" }
}
```

---

## ?? Testing

### Casos de Prueba

1. **Usuario No Autenticado**
   ```
   ? Ver botón "Iniciar sesión"
   ? No puede acceder a formulario
   ```

2. **Usuario Autenticado (Sin reseña)**
   ```
   ? Ver formulario completo
   ? Seleccionar estrellas
   ? Escribir comentario
   ? Enviar reseña
   ? Recibir confirmación
   ```

3. **Usuario Autenticado (Con reseña)**
   ```
   ? Ver mensaje "Ya escribiste"
   ? Ver su reseña con badge
   ? No puede escribir otra
   ```

4. **Validaciones**
   ```
   ? Título requerido
   ? Comentario min 10 caracteres
   ? Calificación 1-5
   ? Mensajes de error claros
   ```

---

## ?? Próximas Mejoras Posibles

```
1. Editar/Eliminar reseña propia
2. Marcar reseña como "útil"
3. Respuestas del vendedor a reseñas
4. Filtrar por calificación
5. Ordenar reseñas (nuevas/antiguas/útiles)
6. Verificación de compra (mostrar badge)
7. Fotos en reseñas
8. Puntos por escribir reseña
```

---

## ?? Endpoint Disponible

```
POST /Preguntas/CrearResena

Parámetros:
- productoId: int (required)
- titulo: string (required, 5-100)
- calificacionGeneral: int (required, 1-5)
- calificacionAtencion: int (required, 1-5)
- calificacionEnvio: int (required, 1-5)
- comentario: string (required, 10-500)

Response:
{
  "exito": true,
  "mensaje": "Reseña enviada exitosamente"
}

Error:
{
  "error": "Ya escribiste una reseña para este producto"
}
```

---

## ?? Notas Importantes

1. **Base de datos**: Requiere migración para agregar columnas
2. **Modelo**: Calificacion.cs ya está actualizado
3. **ViewModels**: Listos para usar
4. **Controllers**: Completos y funcionales
5. **Vistas**: Componente listo en Details.cshtml
6. **Compilación**: ? 0 errores

---

**Estado: ? LISTO PARA PRODUCCIÓN**

Fecha: 28 de Noviembre, 2024
