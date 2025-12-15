-- Script para limpiar multimedia y forzar regeneración de imágenes temáticas
-- Este script eliminará todas las imágenes virtuales existentes para permitir 
-- que el sistema genere nuevas imágenes temáticas más específicas

USE [NexShopDB]
GO

-- Mostrar estadísticas actuales
SELECT 
    'Multimedia Total' as Tipo,
    COUNT(*) as Cantidad
FROM Multimedia
UNION ALL
SELECT 
    'Multimedia Virtual' as Tipo,
    COUNT(*) as Cantidad
FROM Multimedia 
WHERE NombreArchivo LIKE 'virtual_%' OR NombreArchivo LIKE 'temática_%'

-- Eliminar toda la multimedia virtual/temática existente
DELETE FROM Multimedia 
WHERE NombreArchivo LIKE 'virtual_%' 
   OR NombreArchivo LIKE 'temática_%'
   OR Descripcion LIKE '%generada automáticamente%'

-- Mostrar estadísticas después de la limpieza
SELECT 
    'Multimedia después de limpieza' as Tipo,
    COUNT(*) as Cantidad
FROM Multimedia

-- Mostrar productos sin imágenes (que necesitan regeneración)
SELECT 
    p.ProductoId,
    p.Nombre,
    c.Nombre as Categoria,
    COUNT(m.MultimediaId) as ImagenesActuales
FROM Productos p
LEFT JOIN Multimedia m ON p.ProductoId = m.ProductoId
LEFT JOIN Categorias c ON p.CategoriaId = c.CategoriaId
GROUP BY p.ProductoId, p.Nombre, c.Nombre
HAVING COUNT(m.MultimediaId) = 0
ORDER BY p.ProductoId

PRINT '? Limpieza completada. Reinicia la aplicación para generar imágenes temáticas.'