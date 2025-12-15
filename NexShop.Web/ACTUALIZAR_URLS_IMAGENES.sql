-- ================================================
-- ACTUALIZAR URLS DE IMÁGENES A ESPECÍFICAS
-- ================================================
-- Este script actualiza las URLs de las imágenes principales
-- para que muestren imágenes específicas de cada producto

USE [NexShopDB]
GO

PRINT '?? ACTUALIZANDO URLS DE IMÁGENES A ESPECÍFICAS...'
PRINT ''

-- Mostrar URLs actuales
PRINT '?? URLs ACTUALES (primeros 10):'
SELECT TOP 10
    m.MultimediaId,
    p.ProductoId,
    LEFT(p.Nombre, 30) as Producto,
    LEFT(m.Url, 60) as UrlActual
FROM Multimedia m
INNER JOIN Productos p ON m.ProductoId = p.ProductoId
WHERE m.EsPrincipal = 1
ORDER BY p.ProductoId

PRINT ''
PRINT '?? ACTUALIZANDO URLS...'
PRINT ''

-- Actualizar URLs a imágenes específicas
UPDATE Multimedia SET
    Url = CASE 
        -- === ELECTRÓNICA ===
        WHEN ProductoId = 1 THEN 'https://source.unsplash.com/400x400/?smartphone&sig=1'
        WHEN ProductoId = 2 THEN 'https://source.unsplash.com/400x400/?gaming-laptop&sig=2'
        WHEN ProductoId = 3 THEN 'https://source.unsplash.com/400x400/?tablet&sig=3'
        WHEN ProductoId = 4 THEN 'https://source.unsplash.com/400x400/?wireless-headphones&sig=4'
        WHEN ProductoId = 5 THEN 'https://source.unsplash.com/400x400/?4k-monitor&sig=5'
        WHEN ProductoId = 6 THEN 'https://source.unsplash.com/400x400/?mechanical-keyboard&sig=6'
        WHEN ProductoId = 7 THEN 'https://source.unsplash.com/400x400/?gaming-mouse&sig=7'
        WHEN ProductoId = 8 THEN 'https://source.unsplash.com/400x400/?webcam&sig=8'
        WHEN ProductoId = 9 THEN 'https://source.unsplash.com/400x400/?usb-charger&sig=9'
        WHEN ProductoId = 10 THEN 'https://source.unsplash.com/400x400/?powerbank&sig=10'
        
        -- === ROPA ===
        WHEN ProductoId = 11 THEN 'https://source.unsplash.com/400x400/?white-tshirt&sig=11'
        WHEN ProductoId = 12 THEN 'https://source.unsplash.com/400x400/?blue-jeans&sig=12'
        WHEN ProductoId = 13 THEN 'https://source.unsplash.com/400x400/?hoodie&sig=13'
        WHEN ProductoId = 14 THEN 'https://source.unsplash.com/400x400/?leather-jacket&sig=14'
        WHEN ProductoId = 15 THEN 'https://source.unsplash.com/400x400/?sweatpants&sig=15'
        WHEN ProductoId = 16 THEN 'https://source.unsplash.com/400x400/?socks&sig=16'
        WHEN ProductoId = 17 THEN 'https://source.unsplash.com/400x400/?beanie&sig=17'
        WHEN ProductoId = 18 THEN 'https://source.unsplash.com/400x400/?scarf&sig=18'
        WHEN ProductoId = 19 THEN 'https://source.unsplash.com/400x400/?running-shoes&sig=19'
        WHEN ProductoId = 20 THEN 'https://source.unsplash.com/400x400/?leather-belt&sig=20'
        
        -- === HOGAR ===
        WHEN ProductoId = 21 THEN 'https://source.unsplash.com/400x400/?bed-sheets&sig=21'
        WHEN ProductoId = 22 THEN 'https://source.unsplash.com/400x400/?pillow&sig=22'
        WHEN ProductoId = 23 THEN 'https://source.unsplash.com/400x400/?comforter&sig=23'
        WHEN ProductoId = 24 THEN 'https://source.unsplash.com/400x400/?curtains&sig=24'
        WHEN ProductoId = 25 THEN 'https://source.unsplash.com/400x400/?desk-lamp&sig=25'
        WHEN ProductoId = 26 THEN 'https://source.unsplash.com/400x400/?wall-mirror&sig=26'
        WHEN ProductoId = 27 THEN 'https://source.unsplash.com/400x400/?persian-rug&sig=27'
        WHEN ProductoId = 28 THEN 'https://source.unsplash.com/400x400/?towels&sig=28'
        WHEN ProductoId = 29 THEN 'https://source.unsplash.com/400x400/?bath-mat&sig=29'
        WHEN ProductoId = 30 THEN 'https://source.unsplash.com/400x400/?artificial-plants&sig=30'
        
        -- === DEPORTES ===
        WHEN ProductoId = 31 THEN 'https://source.unsplash.com/400x400/?soccer-ball&sig=31'
        WHEN ProductoId = 32 THEN 'https://source.unsplash.com/400x400/?tennis-racket&sig=32'
        WHEN ProductoId = 33 THEN 'https://source.unsplash.com/400x400/?basketball&sig=33'
        WHEN ProductoId = 34 THEN 'https://source.unsplash.com/400x400/?dumbbells&sig=34'
        WHEN ProductoId = 35 THEN 'https://source.unsplash.com/400x400/?yoga-mat&sig=35'
        WHEN ProductoId = 36 THEN 'https://source.unsplash.com/400x400/?resistance-band&sig=36'
        WHEN ProductoId = 37 THEN 'https://source.unsplash.com/400x400/?water-bottle&sig=37'
        WHEN ProductoId = 38 THEN 'https://source.unsplash.com/400x400/?boxing-gloves&sig=38'
        WHEN ProductoId = 39 THEN 'https://source.unsplash.com/400x400/?measuring-tape&sig=39'
        WHEN ProductoId = 40 THEN 'https://source.unsplash.com/400x400/?sports-uniform&sig=40'
        
        -- === LIBROS ===
        WHEN ProductoId = 41 THEN 'https://source.unsplash.com/400x400/?classic-books&sig=41'
        WHEN ProductoId = 42 THEN 'https://source.unsplash.com/400x400/?dystopian-book&sig=42'
        WHEN ProductoId = 43 THEN 'https://source.unsplash.com/400x400/?literature-book&sig=43'
        WHEN ProductoId = 44 THEN 'https://source.unsplash.com/400x400/?self-help-book&sig=44'
        WHEN ProductoId = 45 THEN 'https://source.unsplash.com/400x400/?inspirational-book&sig=45'
        WHEN ProductoId = 46 THEN 'https://source.unsplash.com/400x400/?history-book&sig=46'
        WHEN ProductoId = 47 THEN 'https://source.unsplash.com/400x400/?mindfulness-book&sig=47'
        WHEN ProductoId = 48 THEN 'https://source.unsplash.com/400x400/?creativity-book&sig=48'
        WHEN ProductoId = 49 THEN 'https://source.unsplash.com/400x400/?meditation-book&sig=49'
        WHEN ProductoId = 50 THEN 'https://source.unsplash.com/400x400/?leadership-book&sig=50'
        
        ELSE Url
    END,
    FechaActualizacion = GETDATE()
WHERE EsPrincipal = 1
  AND ProductoId BETWEEN 1 AND 50

PRINT '? URLs actualizadas: ' + CAST(@@ROWCOUNT AS VARCHAR(10))
PRINT ''

-- Mostrar URLs nuevas
PRINT '?? URLs NUEVAS (primeros 10):'
SELECT TOP 10
    m.MultimediaId,
    p.ProductoId,
    LEFT(p.Nombre, 30) as Producto,
    LEFT(m.Url, 60) as UrlNueva
FROM Multimedia m
INNER JOIN Productos p ON m.ProductoId = p.ProductoId
WHERE m.EsPrincipal = 1
ORDER BY p.ProductoId

PRINT ''
PRINT '? ACTUALIZACIÓN COMPLETADA'
PRINT ''
PRINT '?? PRÓXIMOS PASOS:'
PRINT '1. Inicia o reinicia la aplicación: dotnet run'
PRINT '2. Abre: http://localhost:5217/Productos'
PRINT '3. Verifica que cada producto muestre su imagen específica'
PRINT ''
PRINT '?? EJEMPLOS DE LO QUE VERÁS:'
PRINT '   • Cinturón de Cuero ? Imagen de cinturón'
PRINT '   • Zapatos Deportivos ? Zapatillas running'
PRINT '   • Banda Elástica ? Banda de resistencia'
PRINT '   • Guantes de Boxeo ? Guantes rojos'
PRINT ''

GO