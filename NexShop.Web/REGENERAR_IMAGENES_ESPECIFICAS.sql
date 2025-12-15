-- =====================================
-- SCRIPT PARA REGENERAR IMÁGENES ESPECÍFICAS
-- =====================================
-- Este script limpia todas las imágenes virtuales y fuerza
-- la regeneración con términos ultra-específicos

USE [NexShopDB]
GO

PRINT '?? INICIANDO LIMPIEZA DE IMÁGENES VIRTUALES...'
PRINT ''

-- Mostrar estadísticas antes de limpieza
PRINT '?? ESTADÍSTICAS ANTES:'
SELECT 
    'Total Multimedia' as Tipo,
    COUNT(*) as Cantidad
FROM Multimedia

UNION ALL

SELECT 
    'Multimedia Virtual' as Tipo,
    COUNT(*) as Cantidad
FROM Multimedia 
WHERE Descripcion LIKE '%generada automáticamente%'
   OR NombreArchivo LIKE 'virtual_%'
   OR NombreArchivo LIKE 'temática_%'
   OR NombreArchivo LIKE 'producto_%'

PRINT ''
PRINT '??? ELIMINANDO IMÁGENES VIRTUALES...'

-- Eliminar TODAS las imágenes virtuales/temáticas
DELETE FROM Multimedia 
WHERE Descripcion LIKE '%generada automáticamente%'
   OR Descripcion LIKE '%Imagen generada%'
   OR NombreArchivo LIKE 'virtual_%'
   OR NombreArchivo LIKE 'temática_%'
   OR NombreArchivo LIKE 'producto_%'
   OR Nombre LIKE 'Imagen Virtual%'
   OR Nombre LIKE 'Imagen Temática%'

PRINT '? Imágenes virtuales eliminadas'
PRINT ''

-- Mostrar estadísticas después
PRINT '?? ESTADÍSTICAS DESPUÉS:'
SELECT 
    'Total Multimedia Restante' as Tipo,
    COUNT(*) as Cantidad
FROM Multimedia

PRINT ''
PRINT '?? PRODUCTOS SIN IMÁGENES (listos para regenerar):'
SELECT 
    p.ProductoId as ID,
    LEFT(p.Nombre, 40) as Producto,
    c.Nombre as Categoria,
    COUNT(m.MultimediaId) as ImagenesActuales
FROM Productos p
LEFT JOIN Multimedia m ON p.ProductoId = m.ProductoId
INNER JOIN Categorias c ON p.CategoriaId = c.CategoriaId
GROUP BY p.ProductoId, p.Nombre, c.Nombre
HAVING COUNT(m.MultimediaId) = 0
ORDER BY p.ProductoId

PRINT ''
PRINT '? LIMPIEZA COMPLETADA'
PRINT ''
PRINT '?? PRÓXIMOS PASOS:'
PRINT '1. Reinicia la aplicación: dotnet run'
PRINT '2. El sistema generará imágenes específicas automáticamente'
PRINT '3. Espera a que termine la sincronización inicial'
PRINT '4. Abre http://localhost:5217/Productos'
PRINT ''
PRINT '?? CADA PRODUCTO MOSTRARÁ:'
PRINT '   • Smartphone ? Imagen de smartphone real'
PRINT '   • Cinturón de Cuero ? Imagen de cinturón de cuero'
PRINT '   • Zapatos Deportivos ? Imagen de zapatos running'
PRINT '   • Botella de Agua ? Imagen de botella deportiva'
PRINT '   • etc...'

GO