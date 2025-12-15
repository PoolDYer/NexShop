-- ============================================
-- SCRIPT DE VERIFICACIÓN DE BD NEXSHOP
-- Base de Datos: NexShopDb
-- Servidor: ADMINISTRATOR\MSSQLSERVER2025
-- ============================================

-- Usar la base de datos
USE NexShopDb;
GO

-- ============================================
-- 1. VERIFICAR TABLAS CREADAS
-- ============================================

PRINT '=== VERIFICAR TABLAS ==='
PRINT ''
SELECT 'Total Tablas Creadas: ' + CAST(COUNT(*) AS VARCHAR(10)) 
FROM information_schema.tables 
WHERE table_type = 'BASE TABLE';

PRINT ''
PRINT 'Lista de tablas:'
SELECT TABLE_NAME 
FROM information_schema.tables 
WHERE table_type = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- ============================================
-- 2. VERIFICAR COLUMNAS POR TABLA
-- ============================================

PRINT ''
PRINT '=== ESTRUCTURA DE TABLAS PRINCIPALES ==='
PRINT ''

-- Tabla AspNetUsers
PRINT 'Tabla: AspNetUsers'
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM information_schema.columns 
WHERE TABLE_NAME = 'AspNetUsers'
ORDER BY ORDINAL_POSITION;

PRINT ''
PRINT 'Tabla: Productos'
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM information_schema.columns 
WHERE TABLE_NAME = 'Productos'
ORDER BY ORDINAL_POSITION;

PRINT ''
PRINT 'Tabla: Órdenes'
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM information_schema.columns 
WHERE TABLE_NAME = 'Ordenes'
ORDER BY ORDINAL_POSITION;

-- ============================================
-- 3. VERIFICAR ÍNDICES
-- ============================================

PRINT ''
PRINT '=== ÍNDICES CREADOS ==='
PRINT 'Total de índices: ' + CAST(COUNT(*) AS VARCHAR(10))
FROM sys.indexes 
WHERE object_id > 100;

PRINT ''
PRINT 'Índices por tabla:'
SELECT 
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType
FROM sys.indexes i
WHERE object_id > 100
ORDER BY OBJECT_NAME(i.object_id);

-- ============================================
-- 4. VERIFICAR CLAVES FORÁNEAS
-- ============================================

PRINT ''
PRINT '=== RELACIONES (FOREIGN KEYS) ==='
SELECT 
    fk.name AS FK_Name,
    OBJECT_NAME(fk.parent_object_id) AS Parent_Table,
    COL_NAME(fk.parent_object_id, fk.parent_column_id) AS Parent_Column,
    OBJECT_NAME(fk.referenced_object_id) AS Referenced_Table,
    COL_NAME(fk.referenced_object_id, fk.referenced_column_id) AS Referenced_Column
FROM sys.foreign_keys fk
ORDER BY OBJECT_NAME(fk.parent_object_id);

-- ============================================
-- 5. CONTAR REGISTROS EN TABLAS
-- ============================================

PRINT ''
PRINT '=== CONTEO DE REGISTROS ==='

DECLARE @TableName NVARCHAR(MAX)
DECLARE @SQL NVARCHAR(MAX)

DECLARE TableCursor CURSOR FOR
SELECT TABLE_NAME 
FROM information_schema.tables 
WHERE table_type = 'BASE TABLE' AND TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME

OPEN TableCursor

FETCH NEXT FROM TableCursor INTO @TableName
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @SQL = 'SELECT ''' + @TableName + ''' AS TableName, COUNT(*) AS TotalRows FROM [' + @TableName + ']'
    EXEC sp_executesql @SQL
    FETCH NEXT FROM TableCursor INTO @TableName
END

CLOSE TableCursor
DEALLOCATE TableCursor

-- ============================================
-- 6. VERIFICAR USUARIOS CREADOS
-- ============================================

PRINT ''
PRINT '=== USUARIOS EN EL SISTEMA ==='
SELECT 
    Id,
    Email,
    UserName,
    NombreCompleto,
    TipoUsuario,
    EstaActivo,
    FechaCreacion
FROM AspNetUsers
ORDER BY FechaCreacion;

-- ============================================
-- 7. VERIFICAR ROLES CREADOS
-- ============================================

PRINT ''
PRINT '=== ROLES EN EL SISTEMA ==='
SELECT 
    Id,
    Name AS RoleName,
    NormalizedName
FROM AspNetRoles
ORDER BY Name;

-- ============================================
-- 8. VERIFICAR RELACIÓN USUARIO-ROLES
-- ============================================

PRINT ''
PRINT '=== ASIGNACIÓN DE ROLES A USUARIOS ==='
SELECT 
    u.Email,
    u.NombreCompleto,
    r.Name AS RoleName
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
ORDER BY u.Email;

-- ============================================
-- 9. VERIFICAR CATEGORÍAS
-- ============================================

PRINT ''
PRINT '=== CATEGORÍAS ==='
SELECT 
    CategoriaId,
    Nombre,
    Descripcion,
    EstaActiva,
    FechaCreacion
FROM Categorias
ORDER BY Nombre;

-- ============================================
-- 10. INFORMACIÓN DE LA BASE DE DATOS
-- ============================================

PRINT ''
PRINT '=== INFORMACIÓN DE LA BASE DE DATOS ==='
SELECT 
    DB_NAME() AS DatabaseName,
    @@SERVERNAME AS ServerName,
    GETUTCDATE() AS CurrentDateTime

-- ============================================
-- RESUMEN FINAL
-- ============================================

PRINT ''
PRINT '======================================='
PRINT 'VERIFICACIÓN COMPLETADA'
PRINT '======================================='
PRINT 'Base de Datos: NexShopDb'
PRINT 'Servidor: ADMINISTRATOR\MSSQLSERVER2025'
PRINT 'Fecha: ' + CONVERT(VARCHAR, GETUTCDATE(), 121)
PRINT '======================================='
