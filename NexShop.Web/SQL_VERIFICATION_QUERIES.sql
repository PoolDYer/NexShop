-- ============================================================
-- QUERIES PARA VERIFICAR EL SISTEMA DE PAGO Y STOCK
-- ============================================================

-- NOTA: Reemplaza los [VALORES] con datos reales de tu BD

-- ============================================================
-- 1. VER ÓRDENES CREADAS RECIENTEMENTE
-- ============================================================

-- Todas las órdenes del último día
SELECT 
    o.OrdenId,
    o.NumeroOrden,
    o.CompradorId,
    o.MontoTotal,
    o.Estado,
    o.MetodoPago,
    o.DireccionEntrega,
    o.FechaCreacion
FROM Ordenes o
WHERE o.FechaCreacion > DATEADD(DAY, -1, GETUTCDATE())
ORDER BY o.FechaCreacion DESC;

-- Órdenes confirmadas (pago exitoso)
SELECT 
    o.OrdenId,
    o.NumeroOrden,
    o.MontoTotal,
    COUNT(od.OrdenDetalleId) AS TotalProductos,
    o.FechaCreacion
FROM Ordenes o
LEFT JOIN OrdenDetalles od ON o.OrdenId = od.OrdenId
WHERE o.Estado = 'Confirmada'
GROUP BY o.OrdenId, o.NumeroOrden, o.MontoTotal, o.FechaCreacion
ORDER BY o.FechaCreacion DESC;

-- ============================================================
-- 2. VER DETALLES DE UNA ORDEN ESPECÍFICA
-- ============================================================

-- Detalles de orden (reemplaza [ORDEN_ID] con el ID real)
SELECT 
    od.OrdenDetalleId,
    od.ProductoId,
    p.Nombre AS ProductoNombre,
    od.Cantidad,
    od.PrecioUnitario,
    od.Subtotal,
    (od.Cantidad * od.PrecioUnitario) AS Total
FROM OrdenDetalles od
JOIN Productos p ON od.ProductoId = p.ProductoId
WHERE od.OrdenId = [ORDEN_ID]
ORDER BY od.OrdenDetalleId;

-- ============================================================
-- 3. VERIFICAR CAMBIOS DE STOCK
-- ============================================================

-- Stock actual de todos los productos
SELECT 
    p.ProductoId,
    p.Nombre,
    p.Precio,
    p.Stock,
    p.StockMinimo,
    p.Estado,
    p.FechaActualizacion
FROM Productos p
ORDER BY p.FechaActualizacion DESC;

-- Stock de productos comprados recientemente
SELECT 
    p.ProductoId,
    p.Nombre,
    p.Stock,
    p.StockMinimo,
    p.Estado,
    p.FechaActualizacion,
    COUNT(od.OrdenDetalleId) AS VecesComprado,
    SUM(od.Cantidad) AS TotalVendido
FROM Productos p
LEFT JOIN OrdenDetalles od ON p.ProductoId = od.ProductoId
LEFT JOIN Ordenes o ON od.OrdenId = o.OrdenId
    AND o.FechaCreacion > DATEADD(DAY, -1, GETUTCDATE())
WHERE p.Stock < 100  -- Productos con stock bajo
GROUP BY p.ProductoId, p.Nombre, p.Stock, p.StockMinimo, 
         p.Estado, p.FechaActualizacion
ORDER BY p.FechaActualizacion DESC;

-- ============================================================
-- 4. ANÁLISIS DE VENTAS
-- ============================================================

-- Ingresos por fecha
SELECT 
    CAST(o.FechaCreacion AS DATE) AS Fecha,
    COUNT(o.OrdenId) AS TotalOrdenes,
    COUNT(DISTINCT o.CompradorId) AS ClientesUnicos,
    SUM(o.MontoTotal) AS IngresoTotal,
    AVG(o.MontoTotal) AS PromedioPorOrden,
    SUM(o.Impuesto) AS TotalImpuesto
FROM Ordenes o
WHERE o.Estado = 'Confirmada'
GROUP BY CAST(o.FechaCreacion AS DATE)
ORDER BY Fecha DESC;

-- Productos más vendidos
SELECT TOP 10
    p.ProductoId,
    p.Nombre,
    SUM(od.Cantidad) AS CantidadVendida,
    SUM(od.Subtotal) AS IngresoTotal,
    COUNT(DISTINCT od.OrdenId) AS OrdenesConEsteProducto
FROM OrdenDetalles od
JOIN Productos p ON od.ProductoId = p.ProductoId
JOIN Ordenes o ON od.OrdenId = o.OrdenId
WHERE o.Estado = 'Confirmada'
GROUP BY p.ProductoId, p.Nombre
ORDER BY CantidadVendida DESC;

-- ============================================================
-- 5. PROBLEMAS Y ALERTAS
-- ============================================================

-- Productos con stock bajo (menor al mínimo)
SELECT 
    p.ProductoId,
    p.Nombre,
    p.Stock,
    p.StockMinimo,
    p.Estado,
    (p.StockMinimo - p.Stock) AS UnidadesFaltantes
FROM Productos p
WHERE p.Stock <= p.StockMinimo
ORDER BY UnidadesFaltantes DESC;

-- Productos agotados
SELECT 
    p.ProductoId,
    p.Nombre,
    p.Stock,
    p.FechaActualizacion
FROM Productos p
WHERE p.Estado = 'Agotado' OR p.Stock = 0
ORDER BY p.FechaActualizacion DESC;

-- Órdenes sin confirmar (estado pendiente)
SELECT 
    o.OrdenId,
    o.NumeroOrden,
    o.MontoTotal,
    o.FechaCreacion,
    DATEDIFF(MINUTE, o.FechaCreacion, GETUTCDATE()) AS MinutosDesdeCreacion
FROM Ordenes o
WHERE o.Estado = 'Pendiente'
ORDER BY o.FechaCreacion ASC;

-- ============================================================
-- 6. AUDITORÍA - HISTORIAL DE CAMBIOS DE STOCK
-- ============================================================

-- Ver cuánto stock cambió por producto (comparar antes/después)
-- Nota: Esta query es un ejemplo. En producción, usar tabla de auditoría

-- Productos que tuvieron cambios recientemente
SELECT 
    p.ProductoId,
    p.Nombre,
    p.Stock AS StockActual,
    p.FechaActualizacion,
    COUNT(od.OrdenDetalleId) AS OrdenesRecientes,
    SUM(od.Cantidad) AS UnidadesVendidasRecientes
FROM Productos p
LEFT JOIN OrdenDetalles od ON p.ProductoId = od.ProductoId
LEFT JOIN Ordenes o ON od.OrdenId = o.OrdenId 
    AND o.FechaCreacion > DATEADD(HOUR, -1, GETUTCDATE())
WHERE p.FechaActualizacion > DATEADD(HOUR, -1, GETUTCDATE())
GROUP BY p.ProductoId, p.Nombre, p.Stock, p.FechaActualizacion
ORDER BY p.FechaActualizacion DESC;

-- ============================================================
-- 7. REPORTE GENERAL DEL SISTEMA
-- ============================================================

-- Resumen del estado actual
SELECT 
    'Total Productos' AS Metrica,
    CAST(COUNT(*) AS NVARCHAR(50)) AS Valor
FROM Productos

UNION ALL

SELECT 
    'Productos Disponibles' AS Metrica,
    CAST(COUNT(*) AS NVARCHAR(50)) AS Valor
FROM Productos
WHERE Estado = 'Disponible' AND Stock > 0

UNION ALL

SELECT 
    'Productos Agotados' AS Metrica,
    CAST(COUNT(*) AS NVARCHAR(50)) AS Valor
FROM Productos
WHERE Estado = 'Agotado' OR Stock = 0

UNION ALL

SELECT 
    'Total Órdenes' AS Metrica,
    CAST(COUNT(*) AS NVARCHAR(50)) AS Valor
FROM Ordenes

UNION ALL

SELECT 
    'Órdenes Confirmadas' AS Metrica,
    CAST(COUNT(*) AS NVARCHAR(50)) AS Valor
FROM Ordenes
WHERE Estado = 'Confirmada'

UNION ALL

SELECT 
    'Ingreso Total' AS Metrica,
    CAST(CAST(SUM(MontoTotal) AS DECIMAL(10,2)) AS NVARCHAR(50))
FROM Ordenes
WHERE Estado = 'Confirmada';

-- ============================================================
-- 8. CORRECCIONES Y MANTENIMIENTO
-- ============================================================

-- Si necesitas revertir stock manualmente (USAR CON CUIDADO)
-- UPDATE Productos 
-- SET Stock = Stock + [CANTIDAD],
--     FechaActualizacion = GETUTCDATE()
-- WHERE ProductoId = [PRODUCTO_ID];

-- Actualizar estado de productos basado en stock
UPDATE Productos
SET Estado = CASE 
    WHEN Stock <= StockMinimo THEN 'Agotado'
    WHEN Stock > StockMinimo THEN 'Disponible'
    ELSE Estado
END,
FechaActualizacion = GETUTCDATE()
WHERE Estado != CASE 
    WHEN Stock <= StockMinimo THEN 'Agotado'
    WHEN Stock > StockMinimo THEN 'Disponible'
    ELSE Estado
END;

-- ============================================================
-- NOTAS IMPORTANTES
-- ============================================================
-- 
-- 1. Reemplaza [VALORES] con datos reales
-- 2. Las fechas están en UTC (GETUTCDATE())
-- 3. Los ingresos incluyen impuesto (MontoTotal)
-- 4. Stock se reduce SOLO si Estado = 'Confirmada'
-- 5. Usa transacciones para cambios manuales
-- 6. Realiza backup antes de modificaciones
--
-- ============================================================
