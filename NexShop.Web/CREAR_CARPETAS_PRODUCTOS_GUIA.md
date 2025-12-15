# ?? CREAR CARPETAS PARA TODOS LOS PRODUCTOS

## ?? OBJETIVO

Crear una carpeta para CADA producto existente en la BD con estructura:

```
wwwroot/
??? uploads/
?   ??? productos/
?       ??? 1_Producto1/
?       ??? 2_Producto2/
?       ??? 3_Producto3/
?       ??? ... etc
```

---

## ?? OPCIONES

### OPCIÓN 1: Script PowerShell (RECOMENDADO) ?

```powershell
# Abre PowerShell en la carpeta NexShop.Web
cd "E:\Proyectos Visual\NexShop\NexShop.Web"

# Ejecuta el script
.\crear-todas-carpetas.ps1
```

**Qué hace:**
- ? Lee todos los productos de la BD
- ? Crea una carpeta para cada uno
- ? Muestra progreso en tiempo real
- ? Resumen final

**Si quieres ver los productos sin crearlos:**
```powershell
.\crear-todas-carpetas.ps1 -List
```

**Si quieres limpiar y recrear:**
```powershell
.\crear-todas-carpetas.ps1 -Clean
```

---

### OPCIÓN 2: Hacerlo Manualmente en Visual Studio ?

```
1. Abre NexShop.Web en Visual Studio

2. Ve al Solution Explorer

3. Navega a:
   NexShop.Web
   ??? wwwroot
       ??? uploads
           ??? productos

4. Para cada producto, crea una carpeta:
   Botón derecho > Add > New Folder
   
   Nombre del formato: {ProductoId}_{NombreProducto}
   
   Ejemplos:
   - 1_Power_Bank_3000_mAh
   - 2_Cable_USB_C
   - 3_Audífonos_Bluetooth
   - etc...

5. Listo! Ahora puedes descargar imágenes y colocar en cada carpeta
```

---

### OPCIÓN 3: Crear Manualmente en Explorador de Windows ?

```
1. Abre Explorador de Windows

2. Ve a:
   E:\Proyectos Visual\NexShop\NexShop.Web\wwwroot\uploads\productos

3. Crea carpetas con los nombres de los productos

4. Descarga imágenes y colócalas en cada carpeta

5. Listo!
```

---

## ?? EJEMPLO

Después de ejecutar el script, verás:

```
??????????????????????????????????????????????????????????????????
?     CREAR CARPETAS PARA TODOS LOS PRODUCTOS EXISTENTES        ?
??????????????????????????????????????????????????????????????????

? Base de datos: NexShop.db
? Carpeta verificada: .\wwwroot\uploads\productos

?? Leyendo productos...
? Se encontraron 3 producto(s)

?? Creando carpetas:

  ? [1] Power Bank 3000 mAh
  ? [2] Cable USB Tipo C
  ? [3] Audífonos Bluetooth

?? RESUMEN:
  • Carpetas creadas: 3
  • Carpetas existentes: 0
  • Total: 3

? COMPLETADO

?? Ubicación: E:\Proyectos Visual\NexShop\NexShop.Web\wwwroot\uploads\productos

?? Ahora puedes:
   1. Abrir en Visual Studio: wwwroot > uploads > productos
   2. Descargar imágenes de internet
   3. Colocar las imágenes en sus carpetas
   4. O crear productos nuevos y subir multimedia desde la app
```

---

## ??? DESCARGAR IMÁGENES

Después de crear las carpetas, descarga imágenes:

### Desde Google Images:
```
1. Abre Google Imágenes
2. Busca: "producto nombre" + "producto foto"
3. Descarga las imágenes
4. Colócalas en las carpetas correspondientes
```

### Desde sitios gratuitos:
- unsplash.com
- pexels.com
- pixabay.com
- freepik.com

---

## ?? ESTRUCTURA FINAL

Una vez descargues imágenes, verás esto en Visual Studio:

```
Solution Explorer
??? NexShop.Web
    ??? wwwroot
        ??? uploads
            ??? productos
                ??? 1_Power_Bank_3000_mAh
                ?   ??? power_bank_1.jpg
                ?   ??? power_bank_2.jpg
                ?   ??? power_bank_video.mp4
                ?
                ??? 2_Cable_USB_C
                ?   ??? cable_1.jpg
                ?   ??? cable_2.png
                ?   ??? cable_3.jpg
                ?
                ??? 3_Audífonos_Bluetooth
                    ??? headphones_1.jpg
                    ??? headphones_2.jpg
                    ??? demo_video.mp4
```

---

## ? VERIFICACIÓN

Para verificar que todo está bien:

```powershell
# Ver carpetas creadas
Get-ChildItem ".\wwwroot\uploads\productos" -Directory

# Ver archivos en cada carpeta
Get-ChildItem ".\wwwroot\uploads\productos" -Recurse

# Contar total de archivos
(Get-ChildItem ".\wwwroot\uploads\productos" -Recurse -File).Count
```

---

## ?? COMPARACIÓN

### SIN carpetas:
```
? Todos los archivos en un lugar
? Difícil de organizar
? Confusión con nombres de archivos
? Mantenimiento complicado
```

### CON carpetas:
```
? Cada producto en su carpeta
? Fácil de encontrar archivos
? Nombres claros y organizados
? Mantenimiento simple
? Escalable para muchos productos
```

---

## ?? PRÓXIMOS PASOS

1. ? Ejecuta el script o crea carpetas manualmente
2. ? Descarga imágenes de internet
3. ? Coloca imágenes en cada carpeta
4. ? La aplicación las cargará automáticamente
5. ? O sube multimedia desde la app en Productos > Details

---

## ?? TIPS

```
• Los nombres de carpeta deben seguir: {ProductoId}_{Nombre}
• Usa guiones o guiones bajos, no espacios
• Mantén los nombres cortos y descriptivos
• Usa formatos comunes para imágenes: JPG, PNG
• Para videos: MP4, WebM
```

---

## ? PROBLEMAS

### "No se crean las carpetas"
- ? Verifica que estés en la carpeta NexShop.Web
- ? Ejecuta PowerShell como Administrador
- ? Verifica permisos en wwwroot

### "No veo las carpetas en Visual Studio"
- ? Haz click derecho en Solution Explorer > Refresh
- ? O presiona F5

### "Las imágenes no aparecen en la app"
- ? Verifica que los nombres de carpeta sean correctos
- ? Verifica que las imágenes estén en la carpeta correcta
- ? Recarga la página en el navegador (Ctrl+Shift+R)

---

**¡Listo para agregar imágenes a tus productos! ??**
