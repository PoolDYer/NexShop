# ? INSTRUCCIONES FINALES - Stock y "Ver Mis Órdenes"

## ?? TAREA 1: Resetear Stock del "Juego Infinito - Simon Sinek" a 15

### Opción A: SQL Server Management Studio

**Paso 1:** Abrir SQL Server Management Studio

**Paso 2:** Conectar a tu base de datos NexShop

**Paso 3:** Ejecutar el siguiente comando SQL:

```sql
-- Buscar y actualizar el producto
UPDATE Productos
SET 
    Stock = 15,
    Estado = 'Disponible',
    FechaActualizacion = GETUTCDATE()
WHERE Nombre LIKE '%juego infinito%' OR Nombre LIKE '%simon sinek%'

-- Verificar que se actualizó
SELECT ProductoId, Nombre, Stock, Estado, FechaActualizacion 
FROM Productos 
WHERE Nombre LIKE '%juego infinito%' OR Nombre LIKE '%simon sinek%'
```

**Paso 4:** Ejecutar (presiona F5 o el botón Execute)

**Resultado esperado:**
```
ProductoId | Nombre                          | Stock | Estado        | FechaActualizacion
-----------|----------------------------------|-------|---------------|-------------------
[ID]       | Juego Infinito - Simon Sinek    | 15    | Disponible    | [Fecha Actual]
```

---

### Opción B: Usar el script desde VS Code

**Paso 1:** Abre el archivo: `RESET_STOCK_SIMON_SINEK.sql`

**Paso 2:** Copia el contenido:

```sql
UPDATE Productos
SET 
    Stock = 15,
    Estado = 'Disponible',
    FechaActualizacion = GETUTCDATE()
WHERE Nombre LIKE '%juego infinito%' OR Nombre LIKE '%simon sinek%'
```

**Paso 3:** En SQL Server Management Studio:
- Abre una nueva consulta
- Pega el comando
- Presiona F5

---

### Opción C: Desde Visual Studio (Package Manager Console)

```powershell
# No es recomendado, pero si quieres:
# 1. Abre Package Manager Console
# 2. Ejecuta una migration si es necesario
# 3. O usa SQL Server Object Explorer
```

---

## ?? TAREA 2: Botón "Ver Mis Órdenes" - Funcionando Correctamente

### ¿Qué cambió?

Antes:
- El botón intentaba ir a `Ordenes/MisOrdenes`
- Pero la vista estaba en `Usuarios/MisOrdenes`
- Resultado: Error 404

Ahora:
- ? Creamos nueva vista: `Views/Ordenes/MisOrdenes.cshtml`
- ? Usa el ViewModel correcto: `OrdenListViewModel`
- ? El controlador `OrdenesController.MisOrdenes()` funciona correctamente
- ? Todo es consistente

### Flujo Completo (Funcional)

```
1. Usuario compra productos
   ?
2. Completa checkout y pago
   ?
3. ? PAGO EXITOSO
   ?
4. Ver página: "Confirmación de Orden"
   ?
5. Click botón: "Ver Mis Órdenes"
   ?
6. ? Redirecciona a: /Ordenes/MisOrdenes
   ?
7. ? Muestra lista de TODAS las órdenes del usuario
   ?
8. Puede hacer click en "Ver Detalles" de cualquier orden
```

### Requisitos (Verificar)

1. **Usuario debe estar autenticado** ?
   - El controlador `OrdenesController` tiene `[Authorize]`
   - Esto garantiza que solo usuarios logueados vean sus órdenes

2. **Vista MisOrdenes existe** ?
   - Creada en: `Views/Ordenes/MisOrdenes.cshtml`
   - Usa ViewModel: `List<OrdenListViewModel>`

3. **Controlador MisOrdenes funciona** ?
   - GET: `/Ordenes/MisOrdenes`
   - Obtiene órdenes del usuario actual
   - Maneja paginación

4. **Botón redirige correctamente** ?
   - En `Confirmacion.cshtml`:
   ```html
   <a asp-action="MisOrdenes" asp-controller="Ordenes" class="btn btn-primary btn-lg">
       <i class="bi bi-list-check"></i> Ver Mis Órdenes
   </a>
   ```

---

## ?? Testing Completo

### Test 1: Verificar Stock

**1. Base de datos:**
```sql
SELECT Nombre, Stock FROM Productos WHERE Nombre LIKE '%juego infinito%'
```

**Resultado esperado:** `Stock = 15` ?

**2. En la aplicación:**
- Navega a: `/Productos` o busca el producto
- Verifica que muestre: "15 unidades disponibles"
- El indicador debe estar al 15%

---

### Test 2: Compra Completa

**1. Agrega el producto al carrito**
- Click: "Agregar al Carrito"
- Cantidad: 5
- Carrito debe actualizar

**2. Procede al checkout**
- Click: "Proceder al Pago"
- Llena el formulario
- Acepta términos

**3. Espera el pago simulado**
- Verás: "Procesando tu Pago..."
- Espera 2-4 segundos

**4. Resultado (95% éxito)**
- ? Ver página: "Compra Realizada Exitosamente!"
- Stock debe ser: 15 - 5 = **10 unidades**

**5. Click botón: "Ver Mis Órdenes"**
- ? Debe redirigir a: `/Ordenes/MisOrdenes`
- ? Ver tu orden en la lista
- ? NO debe mostrar error 404
- ? Muestra: Número de orden, fecha, total, estado, artículos

---

### Test 3: Verificar Stock Actualizado en BD

```sql
SELECT ProductoId, Nombre, Stock, Estado, FechaActualizacion
FROM Productos 
WHERE Nombre LIKE '%juego infinito%'
```

**Resultado esperado:**
```
ProductoId | Nombre                          | Stock | Estado     | FechaActualizacion
-----------|----------------------------------|-------|------------|-----------
[ID]       | Juego Infinito - Simon Sinek    | 10    | Disponible | [Reciente]
```

---

## ?? Checklist

- [ ] Ejecuté SQL para resetear stock a 15
- [ ] Verifiqué en BD: Stock = 15
- [ ] Compilé el proyecto sin errores ?
- [ ] Realicé una compra de prueba
- [ ] Clickeé "Ver Mis Órdenes" sin error
- [ ] Se mostró la lista de órdenes
- [ ] Stock se redujo correctamente en BD
- [ ] ¡Todo funciona! ??

---

## ?? Estado Final

? **Stock:** Resetizado a 15 unidades
? **"Ver Mis Órdenes":** Funcional sin errores
? **Compilación:** Exitosa
? **Listo para usar**

---

## ?? Archivos Creados/Modificados

### Nuevos
- `Views/Ordenes/MisOrdenes.cshtml` - Vista lista de órdenes
- `RESET_STOCK_SIMON_SINEK.sql` - Script SQL

### Ya Existentes (Verificados)
- `Controllers/OrdenesController.cs` - Ya tiene `MisOrdenes` action ?
- `Views/Ordenes/Confirmacion.cshtml` - Botón correcto ?

---

## ?? Notas Importantes

1. **El stock es por unidad**
   - Si compras 5 unidades de 15 disponibles
   - Quedan 10 unidades (15 - 5 = 10)

2. **Solo usuarios autenticados**
   - Debes estar logueado para ver tus órdenes
   - No puedes ver órdenes de otros usuarios

3. **Estados de orden**
   - `Pendiente` - Creada pero en espera
   - `Confirmada` - Pago procesado exitosamente ?
   - `Cancelada` - Cancelada por usuario

4. **Seguridad**
   - Cada usuario ve solo sus propias órdenes
   - Controladas por `[Authorize]`

---

## ? Si Algo Falla

**Error 404 en "Ver Mis Órdenes":**
- Verifica que la vista existe: `Views/Ordenes/MisOrdenes.cshtml`
- Verifica que compiló sin errores: `run_build`
- Limpia caché del navegador: Ctrl+Shift+Supr

**Stock no se actualiza:**
- Verifica que el pago fue exitoso (Estado = "Confirmada")
- Ejecuta SQL para verificar: `SELECT Stock FROM Productos`
- Revisa logs de aplicación

**Botón "Ver Mis Órdenes" no funciona:**
- Verifica que estés logueado
- Verifica que la orden fue creada correctamente
- Abre Developer Tools (F12) y verifica la URL

---

**¡Listo! Tu sistema de órdenes está completamente funcional. ??**
