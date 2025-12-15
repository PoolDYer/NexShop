# ?? Sistema de Gestión de Stock por Unidad - Documentación

## ? IMPLEMENTACIÓN COMPLETADA

Se ha implementado un **sistema robusto de gestión de stock por unidad individual** que reduce automáticamente el inventario tras cada compra exitosa.

---

## ?? Concepto Fundamental

### La Lógica del Stock

Cada producto es una **mercancía individual** con cantidad de unidades:

```
Ejemplo: Power Bank 3000
?? Stock Inicial: 100 unidades
?? Usuario A compra: 1 unidad ? Stock = 99
?? Usuario B compra: 2 unidades ? Stock = 97
?? Usuario C compra: 97 unidades ? Stock = 0 (AGOTADO)
?? Estado: "Agotado" ??
```

**Cada unidad es independiente pero el stock es acumulado**:
- No hay diferenciación entre "Laptop modelo A" y "Laptop modelo B"
- Solo importa el stock total disponible del producto
- Cada compra reduce el stock en la cantidad solicitada

---

## ?? Cómo Funciona

### Flujo Completo

```
1. Usuario ve producto con Stock = 100 ?
   ?? Estado: "Disponible"
   ?? Indicador: 100%

2. Usuario agrega 5 unidades al carrito
   ?? Validación: ¿Stock >= 5? SÍ ?
   ?? Se permite agregar

3. Usuario procede al pago
   ?? Validación: ¿Stock >= 5? SÍ ?
   ?? Se muestra formulario

4. Simulador procesa pago (2-4 segundos)
   ?? Pago EXITOSO (95%)
   ?  ?? Continuar
   ?? Pago RECHAZADO (5%)
      ?? Volver al carrito (Stock no cambia)

5. PAGO EXITOSO - REDUCIR STOCK
   ?? Stock anterior: 100
   ?? Unidades compradas: 5
   ?? Stock nuevo: 95 ? AUTOMÁTICO
   ?? Estado: "Disponible" (sigue disponible)
   ?? FechaActualizacion: [Timestamp actual]

6. Confirmación mostrada al usuario
   ?? Número de orden: ORD-XXX-12345
   ?? Unidades compradas: 5
   ?? Stock actualizado: 95
   ?? ¡Compra exitosa!
```

---

## ?? Casos de Uso Detallados

### Caso 1: Compra Normal (Stock Suficiente)

```
ANTES:
Producto.Stock = 100
Producto.Estado = "Disponible"

Usuario compra 10 unidades

DESPUÉS:
Producto.Stock = 90 ? Reducido
Producto.Estado = "Disponible" ?
Orden creada con estado "Confirmada"
```

### Caso 2: Último Stock (Agotado después de compra)

```
ANTES:
Producto.Stock = 5
Producto.Estado = "Disponible"

Usuario compra 5 unidades

DESPUÉS:
Producto.Stock = 0 ? Reducido a cero
Producto.Estado = "Agotado" ??
En la página: "PRODUCTO AGOTADO"
Botón: "Agregar al Carrito" ? DESHABILITADO
```

### Caso 3: Stock Insuficiente

```
ANTES:
Producto.Stock = 3
Producto.Estado = "Disponible"

Usuario intenta comprar 5 unidades

RESULTADO:
? Error: "Stock insuficiente. Disponibles: 3 unidades, solicitadas: 5"
Stock NO cambia (sigue en 3)
Orden NO se crea
Usuario vuelve al carrito
```

### Caso 4: Stock Bajo (Advertencia)

```
ANTES:
Producto.Stock = 8
Producto.Estado = "Disponible"

Usuario ve producto

MOSTRADO:
?? "Quedan pocas unidades!" - Badge naranja
?? Indicador visual: 8%
"Compra rápido antes de que se agote"
```

### Caso 5: Pago Rechazado (Stock No Cambia)

```
ANTES:
Producto.Stock = 100

Usuario intenta compra por $500
?
Simulador rechaza pago (5% probabilidad)

DESPUÉS:
Producto.Stock = 100 ? SIN CAMBIOS
Orden NO se crea
Usuario ve: "Pago rechazado, intenta de nuevo"
```

---

## ??? Servicios Implementados

### IStockService

```csharp
public interface IStockService
{
    // Valida disponibilidad
    bool HaySufficientStock(Producto p, int cantidad);
    
    // Valida y devuelve detalles
    Task<ResultadoStock> ValidarStockAsync(Producto p, int cantidad);
    
    // REDUCE stock (compra exitosa)
    Task<ResultadoStock> ReducirStockAsync(Producto p, int cantidad);
    
    // REVIERTE stock (compra cancelada)
    Task<ResultadoStock> RevertirStockAsync(Producto p, int cantidad);
    
    // Mensaje de estado
    string ObtenerMensajeEstado(Producto p);
    
    // Porcentaje visual
    int ObtenerPorcentajeDisponibilidad(Producto p);
}
```

---

## ?? ResultadoStock

Resultado detallado de cada operación:

```csharp
public class ResultadoStock
{
    public bool EsExitoso { get; set; }           // ¿Fue exitosa?
    public string Mensaje { get; set; }           // Descripción
    public int StockAnterior { get; set; }        // Antes
    public int StockNuevo { get; set; }           // Después
    public int UnidadesProcessadas { get; set; }  // Cantidad procesada
    public string EstadoAnterior { get; set; }    // Estado anterior
    public string EstadoNuevo { get; set; }       // Estado nuevo
}
```

---

## ?? Validaciones Implementadas

### 1. Validación en Carrito

```
? Antes de agregar al carrito:
   - Verifica que Stock >= Cantidad
   - Si falla: Muestra error y no agrega

? En el carrito:
   - Máximo = Stock disponible
   - Input type="number" max="@Stock"
   - Si intenta exceder: Advertencia visual
```

### 2. Validación en Checkout

```
? Al mostrar formulario:
   - Verifica que TODOS los productos sigan disponibles
   - Si cambió stock: Retorna al carrito con error

? Al procesar pago:
   - Valida stock de CADA producto nuevamente
   - Si es insuficiente: Revierte transacción
```

### 3. Validación en Reducción de Stock

```
? Usa transacciones ACID:
   - Si algo falla: Rollback automático
   - Stock nunca se reduce sin pago exitoso
   - Orden se crea SOLO si pago es exitoso
```

---

## ?? Indicadores Visuales

### En Página de Producto (Details)

```
? DISPONIBLE:
   Badge verde: "? Disponible"
   Indicador: ???????????????????? 100%
   
?? POCAS UNIDADES:
   Badge naranja: "?? Quedan 5 unidades"
   Indicador: ???????????????????? 5%
   
?? AGOTADO:
   Badge rojo: "?? Producto Agotado"
   Botón "Agregar": DESHABILITADO
   No permite agregar al carrito
```

### En Carrito

```
Por cada producto:
?? Stock disponible: @StockDisponible
?? Cantidad solicitada: @Cantidad
?? Max en input: @StockDisponible
?? Advertencia si es máximo: "?? Límite de stock alcanzado"
```

### En Confirmación

```
? COMPRA EXITOSA
?? Stock anterior: 100
?? Unidades compradas: 5
?? Stock nuevo: 95
?? "Restante: 95 unidades"
```

---

## ?? Tabla de Estados

| Stock | Estado | Mostrado | Acción |
|-------|--------|----------|--------|
| 0 | Agotado | ?? Agotado | Botón disabled |
| 1-9 | Disponible | ?? Pocas | Aviso de prisa |
| 10+ | Disponible | ? Disponible | Botón activo |

---

## ?? Logging Detallado

Todas las operaciones se registran:

```
[INFO] Stock validado exitosamente para Laptop. Stock: 100
[INFO] Stock reducido exitosamente. Producto: Laptop, Anterior: 100, Nuevo: 95
[INFO] Stock reducido para producto 1. Nuevo stock: 95
[WARNING] Stock insuficiente para Mouse. Solicitado: 5, Disponible: 2
[ERROR] Intento de reducir más stock del disponible
```

---

## ?? Ejemplos de Testing

### Test 1: Compra Exitosa

```bash
1. Producto: Laptop, Stock = 100
2. Usuario compra 1 unidad
3. Pago procesado ?
4. Stock = 99 ?
5. Orden creada ?
```

### Test 2: Agotamiento

```bash
1. Producto: Mouse, Stock = 3
2. Usuario A compra 2 ? Stock = 1
3. Usuario B compra 1 ? Stock = 0
4. Usuario C intenta comprar ? ? Agotado
```

### Test 3: Stock Bajo

```bash
1. Producto: Power Bank, Stock = 5
2. Usuario ve página
3. Muestra: "?? Quedan 5 unidades"
4. Indicador: 5%
5. Botón en página de producto es rojo/naranja
```

### Test 4: Transacción Rollback

```bash
1. Producto: Tablet, Stock = 50
2. Usuario intenta pagar
3. Pago rechazado (5%)
4. Stock sigue = 50 ?
5. Orden NO se crea ?
```

---

## ?? Características Avanzadas

### 1. Reversión de Stock

Si una orden se cancela:

```
ANTES:
Orden "Confirmada" con 5 Tablets comprados
Stock del Tablet = 45

CANCELACIÓN:
Stock += 5
Stock nuevo = 50 ? REVERTIDO

Si estaba "Agotado" ? Cambia a "Disponible"
```

### 2. Stock Mínimo

Alerta si stock cae por debajo del mínimo:

```
StockMinimo = 10
Stock actual = 5

?? "Stock bajo (5 unidades)"
```

### 3. Porcentaje de Disponibilidad

Para UI:

```csharp
0-10%    ? Rojo (Máx alerta)
11-49%   ? Naranja (Advertencia)
50%+     ? Verde (Disponible)
100%+    ? Verde máximo
```

---

## ?? Experiencia del Usuario

### Flujo Completo

```
1. Usuario navega productos
   ?
2. Ve "Stock: 100 unidades" ?
   ?
3. Agrega 5 al carrito
   ?
4. Carrito muestra "x5, Max: 100"
   ?
5. Procede al pago
   ?
6. Llena formulario
   ?
7. Espera 2-4 segundos (pago)
   ?
8. ? ÉXITO
   ?  ?? Confirmación
   ?  ?? "Compra exitosa"
   ?  ?? "Stock reducido: 95 unidades"
   ?  ?? Número de orden
   ?? ? FALLO
      ?? "Pago rechazado"
      ?? "Stock sin cambios"
      ?? Botón: Reintentar
```

---

## ?? Configuración

### En Program.cs

```csharp
// Registro automático
builder.Services.AddScoped<IStockService, StockService>();
```

### En OrdenesController

```csharp
// Inyección en constructor
private readonly IStockService _stockService;

public OrdenesController(..., IStockService stockService)
{
    _stockService = stockService;
}
```

---

## ?? Soporte

### Problemas Comunes

**? Stock no disminuye**
- ? Verificar que pago fue exitoso (Estado = "Confirmada")
- ? Verificar logs: "Stock reducido para producto"

**? Dice "Agotado" pero hay stock**
- ? Verificar: Stock <= 0 o Estado = "Agotado"
- ? Ejecutar query: `SELECT Stock, Estado FROM Productos`

**? No permite comprar el máximo**
- ? Verificar max del input = Stock disponible
- ? Actualizar página (F5)

---

## ?? SQL Queries Útiles

### Ver estado actual

```sql
SELECT ProductoId, Nombre, Stock, Estado
FROM Productos
ORDER BY Stock ASC
```

### Ver cambios recientes

```sql
SELECT ProductoId, Nombre, Stock, FechaActualizacion
FROM Productos
WHERE FechaActualizacion > DATEADD(HOUR, -1, GETUTCDATE())
ORDER BY FechaActualizacion DESC
```

### Ver productos agotados

```sql
SELECT * FROM Productos WHERE Estado = 'Agotado' OR Stock = 0
```

### Ver historial de compras

```sql
SELECT o.NumeroOrden, od.Cantidad, p.Nombre, p.Stock
FROM Ordenes o
JOIN OrdenDetalles od ON o.OrdenId = od.OrdenId
JOIN Productos p ON od.ProductoId = p.ProductoId
WHERE o.Estado = 'Confirmada'
ORDER BY o.FechaCreacion DESC
```

---

## ?? Resumen

| Aspecto | Implementado |
|---------|-------------|
| **Reducción de stock** | ? Automática por unidad |
| **Validación** | ? Triple validación |
| **Transacciones** | ? ACID completas |
| **Reversión** | ? En cancelaciones |
| **Indicadores visuales** | ? Barras, badges, iconos |
| **Logging** | ? Completo |
| **Pago integrado** | ? Solo reduce si éxito |
| **Estado "Agotado"** | ? Automático cuando = 0 |

---

**? Sistema de stock 100% funcional y robusto**
