# ?? VER IMÁGENES ESPECÍFICAS DE TODOS LOS PRODUCTOS

## ? **SOLUCIÓN RÁPIDA (2 PASOS)**

### **PASO 1: Ejecutar Script SQL**

1. **Abre SQL Server Management Studio**
2. **Conéctate a tu servidor local**
3. **Abre el archivo**: `ACTUALIZAR_URLS_IMAGENES.sql`
4. **Ejecuta el script** (F5 o botón Execute)

**¿Qué hace?**
- Actualiza las URLs de las 50 imágenes principales
- Cambia de imágenes aleatorias a imágenes específicas
- **NO borra datos**, solo actualiza URLs

**Resultado esperado:**
```
? URLs actualizadas: 50
```

---

### **PASO 2: Iniciar Aplicación**

En una terminal PowerShell:

```powershell
cd "E:\Proyectos Visual\NexShop\NexShop.Web"
dotnet run
```

**Espera a ver:**
```
Now listening on: http://localhost:5217
```

---

### **PASO 3: Ver Resultados**

Abre tu navegador en:
```
http://localhost:5217/Productos
```

---

## ?? **LO QUE VERÁS**

### **ELECTRÓNICA** (Productos 1-10)
- ? **Smartphone XYZ Pro** ? Imagen de smartphone
- ? **Laptop Gaming 15"** ? Laptop gaming
- ? **Tablet 10 pulgadas** ? Tablet
- ? **Auriculares Bluetooth** ? Auriculares inalámbricos
- ? **Monitor 4K 27"** ? Monitor gaming
- ? **Teclado Mecánico RGB** ? Teclado con RGB
- ? **Ratón Gaming** ? Mouse gaming
- ? **Webcam 4K** ? Cámara web
- ? **Cargador Rápido** ? Cargador USB
- ? **Powerbank** ? Batería externa

### **ROPA** (Productos 11-20)
- ? **Camiseta Básica Blanca** ? Camiseta blanca
- ? **Jeans Azul Oscuro** ? Jeans azules
- ? **Sudadera con Capucha** ? Hoodie
- ? **Chaqueta de Cuero** ? Chaqueta marrón
- ? **Pantalones Deportivos** ? Pants deportivos
- ? **Calcetines Pack de 5** ? Calcetines
- ? **Gorro de Lana** ? Beanie
- ? **Bufanda Larga** ? Bufanda
- ? **Zapatos Deportivos** ? Zapatillas running ?
- ? **Cinturón de Cuero** ? Cinturón de cuero ?

### **HOGAR** (Productos 21-30)
- ? **Juego de Sábanas** ? Sábanas de cama
- ? **Almohada de Espuma** ? Almohada
- ? **Edredón Nórdico** ? Edredón
- ? **Cortinas Blackout** ? Cortinas
- ? **Lámpara de Escritorio** ? Lámpara LED
- ? **Espejo de Pared** ? Espejo grande
- ? **Alfombra Persa** ? Alfombra tradicional
- ? **Juego de Toallas** ? Toallas
- ? **Tapete de Baño** ? Alfombrilla
- ? **Plantas Decorativas** ? Plantas artificiales

### **DEPORTES** (Productos 31-40)
- ? **Balón de Fútbol** ? Balón FIFA
- ? **Raqueta de Tenis** ? Raqueta profesional
- ? **Pelota de Baloncesto** ? Balón NBA
- ? **Mancuernas Ajustables** ? Pesas/dumbbells
- ? **Colchoneta de Yoga** ? Yoga mat
- ? **Banda Elástica Resistencia** ? Banda de ejercicio ?
- ? **Botella de Agua 1L** ? Botella deportiva
- ? **Guantes de Boxeo** ? Guantes rojos ?
- ? **Cinta Métrica** ? Cinta métrica
- ? **Uniforme Deportivo** ? Ropa deportiva

### **LIBROS** (Productos 41-50)
- ? **El Quijote** ? Libros clásicos
- ? **1984** ? Libro distópico
- ? **Cien Años de Soledad** ? Literatura
- ? **Hábitos Atómicos** ? Libro autoayuda
- ? **El Alquimista** ? Libro inspiracional
- ? **Sapiens** ? Libro historia
- ? **El Poder del Ahora** ? Mindfulness
- ? **La Revolución de los Creativos** ? Creatividad
- ? **Mindfulness para Principiantes** ? Meditación
- ? **El Juego Infinito** ? Liderazgo

---

## ??? **SI ALGO NO FUNCIONA**

### ? **"No puedo ejecutar el script SQL"**
**Solución alternativa:**
Ejecuta el script PowerShell que lo hace automáticamente:
```powershell
.\REGENERAR_IMAGENES_AHORA.ps1
```
Elige opción 1 para regenerar todo.

### ? **"Las imágenes no cargan"**
**Causa:** Unsplash bloqueado temporalmente
**Solución:** Espera 10 segundos y refresca (F5)

### ? **"Sigo viendo imágenes aleatorias"**
**Causa:** Script SQL no se ejecutó correctamente
**Solución:** Verifica que el script se ejecutó exitosamente y vio "URLs actualizadas: 50"

---

## ? **RESULTADO FINAL**

**Todos los 50 productos mostrarán imágenes específicas:**

```
Antes:
Cinturón de Cuero ? Ciudad de Nueva York ?
Banda Elástica ? Montañas ?

Después:
Cinturón de Cuero ? Cinturón real ?
Banda Elástica ? Banda de resistencia ?
```

---

## ?? **VERIFICACIÓN**

Para verificar que todo funciona, busca estos productos:

1. **Producto #20**: Cinturón de Cuero ? Debe mostrar cinturón
2. **Producto #19**: Zapatos Deportivos ? Debe mostrar zapatillas
3. **Producto #36**: Banda Elástica ? Debe mostrar banda
4. **Producto #38**: Guantes de Boxeo ? Debe mostrar guantes

Si estos 4 muestran imágenes correctas: **¡ÉXITO!** ??

---

**¿Listo para empezar?**

1. ? Ejecuta `ACTUALIZAR_URLS_IMAGENES.sql`
2. ? `dotnet run`
3. ? Abre `http://localhost:5217/Productos`

**¡Verás las imágenes específicas de todos los productos!** ??