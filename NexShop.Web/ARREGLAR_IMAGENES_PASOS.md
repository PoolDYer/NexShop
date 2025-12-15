# ?? ARREGLAR IMÁGENES - PASOS NECESARIOS

## PROBLEMA
Las imágenes están usando URLs que no se cargan bien. Se han corregido pero necesitas limpiar la base de datos.

## SOLUCIÓN RÁPIDA

### OPCIÓN 1: Resetear la base de datos (RECOMENDADO)

```powershell
# 1. Navega a la carpeta del proyecto
cd "E:\Proyectos Visual\NexShop\NexShop.Web"

# 2. Elimina la base de datos actual
Remove-Item "*.db" -Force 2>$null
Remove-Item "*.db-shm" -Force 2>$null
Remove-Item "*.db-wal" -Force 2>$null

# 3. Ejecuta las migraciones (crea la BD nueva)
dotnet ef database update

# 4. Inicia la aplicación (cargará datos nuevos con imágenes correctas)
dotnet run
```

### OPCIÓN 2: Solo limpiar datos de prueba (si quieres mantener otros datos)

```sql
-- Ejecutar en la BD de NexShop

-- Primero, elimina las restricciones
DELETE FROM Multimedia;
DELETE FROM OrdenDetalles;
DELETE FROM Ordenes;
DELETE FROM Carrito;
DELETE FROM Productos;
DELETE FROM Categorias;

-- Los datos volverán a crearse al reiniciar
```

---

## ? CAMBIOS REALIZADOS

### SeederService.cs
```csharp
// ? ANTES: URLs con espacios que no se codificaban
https://via.placeholder.com/400x400?text=Producto+1+v1

// ? DESPUÉS: URLs de imagen real (picsum.photos)
https://picsum.photos/400/400?random=1v1
https://picsum.photos/400/400?random=1v2
```

### ProductosController.cs
```csharp
// ? MEJORADO: Obtiene primera imagen, si no hay fallback
ImagenPrincipal = p.Multimedia.OrderBy(m => m.Orden).FirstOrDefault()?.Url
```

### Index.cshtml (Vista Productos)
```html
<!-- ? MEJORADO: Con fallback a placeholder.com -->
@if (!string.IsNullOrEmpty(producto.ImagenPrincipal))
{
    <img src="@producto.ImagenPrincipal" onerror="...fallback...">
}
else
{
    <img src="https://picsum.photos/400/400?random=...">
}
```

---

## ?? RESULTADO ESPERADO

Después de limpiar y reiniciar:

```
? 50 productos cargados
? 100 imágenes reales cargadas
? Imágenes visibles en la galería
? 2 imágenes por producto
? Sin errores de carga
```

---

## ?? PASOS FINALES

1. **Ejecuta la limpieza** (elige opción 1 o 2)
2. **Reinicia la aplicación**
3. **Abre** `http://localhost:5217/Productos`
4. **Verifica** que las imágenes se cargan correctamente

---

## ?? NOTAS

- picsum.photos es un servicio FREE de imágenes reales aleatorias
- Las imágenes cambiarán cada vez que se carga la página (es el servicio)
- Si quieres imágenes fijas, usa placeholder.com o subidas locales
- Las URLs se guardan en la BD, así que no cambiarán en la aplicación

---

**¡Después de estos pasos, las imágenes deberían funcionar perfectamente!** ?
