-- ============================================================
-- SCRIPT: Resetear Stock del Producto "Juego Infinito - Simon Sinek"
-- ============================================================

-- 1. Buscar el producto
SELECT ProductoId, Nombre, Stock, Estado, Precio 
FROM Productos 
WHERE Nombre LIKE '%juego infinito%' OR Nombre LIKE '%simon sinek%'

-- 2. Resetear el stock a 15 (reemplaza [ProductoId] con el ID del producto)
UPDATE Productos
SET 
    Stock = 15,
    Estado = 'Disponible',
    FechaActualizacion = GETUTCDATE()
WHERE Nombre LIKE '%juego infinito%' OR Nombre LIKE '%simon sinek%'

-- 3. Verificar que se actualizó correctamente
SELECT ProductoId, Nombre, Stock, Estado, FechaActualizacion 
FROM Productos 
WHERE Nombre LIKE '%juego infinito%' OR Nombre LIKE '%simon sinek%'

-- ============================================================
-- Si quieres ser más específico, usa esta query:
-- ============================================================

-- Encontrar el ID exacto
DECLARE @ProductoId INT
SELECT @ProductoId = ProductoId 
FROM Productos 
WHERE Nombre LIKE '%juego infinito%' OR Nombre LIKE '%simon sinek%'

-- Actualizar
IF @ProductoId IS NOT NULL
BEGIN
    UPDATE Productos
    SET 
        Stock = 15,
        Estado = 'Disponible',
        FechaActualizacion = GETUTCDATE()
    WHERE ProductoId = @ProductoId
    
    SELECT 'Stock actualizado a 15 unidades' AS Resultado, 
           @ProductoId AS ProductoId,
           (SELECT Nombre FROM Productos WHERE ProductoId = @ProductoId) AS Producto
END
ELSE
BEGIN
    SELECT 'Producto no encontrado' AS Resultado
END
