# ?? GUÍA PASO A PASO - CREAR CARPETAS Y DESCARGAR IMÁGENES

## ?? OBJETIVO FINAL

```
wwwroot/uploads/productos/
??? 1_Producto_1/
?   ??? imagen1.jpg      ? Descargada
?   ??? imagen2.jpg      ? Descargada
?   ??? video.mp4        ? Descargado (opcional)
?
??? 2_Producto_2/
?   ??? imagen1.jpg      ? Descargada
?   ??? imagen2.jpg      ? Descargada
?
??? 3_Producto_3/
    ??? imagen1.jpg      ? Descargada
    ??? imagen2.jpg      ? Descargada
```

---

## ? PASO 1: CREAR LAS CARPETAS

### Opción A: Con Script (FÁCIL) ?

```
1. Abre PowerShell como Administrador
   Windows + R ? powershell ? Enter

2. Navega a la carpeta
   cd "E:\Proyectos Visual\NexShop\NexShop.Web"

3. Ejecuta el script
   .\crear-todas-carpetas.ps1

4. Espera a que termine (2-5 segundos)

5. ¡Listo! Las carpetas están creadas
```

### Opción B: Visual Studio (MANUAL)

```
1. Abre NexShop.Web en Visual Studio

2. Ve a Solution Explorer
   (Si no está visible: View > Solution Explorer)

3. Expande: NexShop.Web > wwwroot > uploads > productos

4. Para cada producto:
   Click derecho > Add > New Folder
   
   Nombre: {ProductoId}_{NombreProducto}
   
   Ejemplos:
   ? 1_Power_Bank_3000_mAh
   ? 2_Cable_USB_C
   ? 3_Audífonos_Bluetooth

5. ¡Listo!
```

---

## ? PASO 2: DESCARGAR IMÁGENES

### Opción A: Google Imágenes

```
1. Abre Google.com

2. Click en "Imágenes"
   (o ve a: images.google.com)

3. Busca: "nombre del producto"
   Ej: "power bank 3000 mah"

4. Filtra por tamaño o color:
   Herramientas > Tamaño > Grande

5. Selecciona 2-3 imágenes que te gusten

6. Click derecho > Guardar imagen como

7. Guarda en una carpeta temporal
   Ej: C:\Descargas\Imagenes_Productos\

8. Repite para otros productos
```

### Opción B: Sitios Gratuitos

**Unsplash.com:**
```
1. Ve a unsplash.com
2. Busca el producto
3. Click en la imagen > Download
4. Guarda en tu carpeta de descargas
```

**Pexels.com:**
```
1. Ve a pexels.com
2. Busca el producto
3. Click derecho > Descargar
```

**Pixabay.com:**
```
1. Ve a pixabay.com
2. Busca el producto
3. Click "Descargar"
```

---

## ? PASO 3: COPIAR IMÁGENES A LAS CARPETAS

### Método A: Visual Studio (Fácil) ?

```
1. En Visual Studio, expande:
   NexShop.Web > wwwroot > uploads > productos
   > 1_Power_Bank_3000_mAh

2. Click derecho > Add > Existing Item

3. Selecciona las imágenes que descargaste

4. Click "Add"

5. Las imágenes aparecen en la carpeta!

6. Repite para cada producto
```

### Método B: Explorador de Windows

```
1. Abre Explorador de Windows

2. Ve a:
   E:\Proyectos Visual\NexShop\NexShop.Web\wwwroot\uploads\productos

3. Abre una carpeta de producto:
   1_Power_Bank_3000_mAh

4. Arrastra las imágenes descargadas aquí

5. Listo! Se copian automáticamente

6. Repite para cada producto
```

### Método C: Copiar y Pegar

```
1. Abre dos ventanas de Explorador

Ventana 1:
   C:\Descargas\Imagenes_Productos\

Ventana 2:
   E:\Proyectos Visual\NexShop\NexShop.Web\wwwroot\uploads\productos\1_Power_Bank\

2. Arrastra imágenes de Ventana 1 a Ventana 2

3. ¡Listo!
```

---

## ?? VERIFICACIÓN

### En Visual Studio:

```
1. Click derecho en Solution Explorer

2. Selecciona "Refresh" (o F5)

3. Expande las carpetas:
   ? Deberías ver las imágenes en cada carpeta
   ? Los nombres de archivo deben aparecer

Resultado esperado:
Solution Explorer
??? NexShop.Web
    ??? wwwroot
        ??? uploads
            ??? productos
                ??? 1_Power_Bank_3000_mAh
                    ??? imagen1.jpg         ? Descargada ?
                    ??? imagen2.jpg         ? Descargada ?
```

### En la Aplicación:

```
1. Ejecuta la aplicación:
   dotnet run

2. Ve a http://localhost:5217/Productos

3. Haz click en un producto

4. Scroll hasta "Galería de Imágenes"

5. ¡Deberías ver las imágenes que descargaste!
```

---

## ??? EJEMPLO COMPLETO

### Producto: Power Bank 3000 mAh

**Paso 1: Crear carpeta**
```
? Carpeta: /uploads/productos/1_Power_Bank_3000_mAh/
```

**Paso 2: Descargar imágenes**
```
? power_bank_1.jpg (descargada de Google Imágenes)
? power_bank_2.jpg (descargada de unsplash.com)
? demo_video.mp4 (descargado de YouTube)
```

**Paso 3: Copiar a la carpeta**
```
? C:\Descargas\power_bank_1.jpg 
   ? /uploads/productos/1_Power_Bank_3000_mAh/power_bank_1.jpg

? C:\Descargas\power_bank_2.jpg 
   ? /uploads/productos/1_Power_Bank_3000_mAh/power_bank_2.jpg

? C:\Descargas\demo_video.mp4 
   ? /uploads/productos/1_Power_Bank_3000_mAh/demo_video.mp4
```

**Resultado en la app:**
```
Galería de Imágenes:
  [Imagen principal: power_bank_1.jpg]
  
  Thumbnails:
  [power_bank_1] [power_bank_2] [demo_video.mp4]
  
  Descripción:
  "Imagen 1 de 2 videos"
```

---

## ?? TIEMPO ESTIMADO

```
Crear carpetas:        2-5 minutos (script)  / 10-15 minutos (manual)
Descargar imágenes:    5-10 minutos (3 productos x 2-3 imágenes)
Copiar a carpetas:     2-5 minutos (drag & drop)
Verificar en app:      1 minuto

TOTAL:                 10-30 minutos
```

---

## ?? TIPS

? **Nombra las imágenes claramente:**
   - ? imagen1.jpg, imagen2.jpg (confuso)
   - ? product_front.jpg, product_back.jpg (claro)

? **Usa formatos comunes:**
   - Imágenes: JPG, PNG, GIF
   - Videos: MP4, WebM

? **Descarga imágenes de tamaño similar:**
   - Mejor: 400x400 o 800x600
   - Evita: Muy pequeñas (<200x200) o muy grandes (>5MB)

? **Para cada producto, descarga:**
   - Mínimo: 1 imagen (frontal)
   - Mejor: 2-3 imágenes (frontal, lateral, trasero)
   - Videos: 1 video demo (opcional)

---

## ?? PROBLEMAS COMUNES

### "No se crean las carpetas"
```
Solución:
1. Verifica que estés en la carpeta correcta
2. Ejecuta PowerShell como Administrador
3. Intenta el método manual en Visual Studio
```

### "No veo las imágenes en Visual Studio"
```
Solución:
1. Haz click derecho en Solution Explorer
2. Click "Refresh" o presiona F5
3. O cierra y reabre la solución
```

### "Las imágenes no aparecen en la app"
```
Solución:
1. Verifica que estén en la carpeta correcta
2. Recarga la página: Ctrl + Shift + R
3. Borra el cache del navegador
4. Reinicia la aplicación: dotnet run
```

### "No sé cómo descargar imágenes"
```
Alternativa:
1. Crea imágenes de prueba:
   • Usa Paint o Photoshop
   • O usa generadores online (Canva.com)
2. Guarda como JPG
3. Copia a las carpetas
```

---

## ?? RECURSOS

**Sitios para descargar imágenes gratis:**
- unsplash.com
- pexels.com
- pixabay.com
- freepik.com
- unsplash.com

**Editores de imágenes online:**
- canva.com
- pixlr.com
- photopea.com

**Generadores de imágenes:**
- removebg.com (fondo transparente)
- cleanup.pictures (remover objetos)
- upscayl.github.io (aumentar resolución)

---

## ? CHECKLIST FINAL

```
? Abrí PowerShell en NexShop.Web
? Ejecuté el script criar-todas-carpetas.ps1
? Se crearon las carpetas (vi el output)
? Descargué imágenes de Google Imágenes o sitios gratuitos
? Copié las imágenes a cada carpeta
? Actualicé Solution Explorer (Refresh/F5)
? Vi las imágenes en Visual Studio
? Ejecuté la app (dotnet run)
? Fui a Productos > Details
? Vi las imágenes en la galería ?
```

---

**¡FELICIDADES! Ya tienes imágenes en tus productos! ??**
