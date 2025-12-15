-- ============================================
-- SCRIPT COMPLETO - BASE DE DATOS NEXSHOP
-- ============================================
-- Servidor: ADMINISTRATOR\POOL
-- BD: NexShopDb
-- Usuario: sa
-- Contraseña: 123456
-- ============================================

USE [NexShopDb]
GO

-- ============================================
-- 1. CREAR TABLA AspNetRoles
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetRoles')
BEGIN
    CREATE TABLE [dbo].[AspNetRoles](
        [Id] [nvarchar](450) NOT NULL PRIMARY KEY,
        [Name] [nvarchar](256) NULL,
        [NormalizedName] [nvarchar](256) NULL,
        [ConcurrencyStamp] [nvarchar](max) NULL
    )
    CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [AspNetRoles]([NormalizedName] ASC) WHERE ([NormalizedName] IS NOT NULL)
    PRINT '? Tabla AspNetRoles creada'
END
GO

-- ============================================
-- 2. CREAR TABLA AspNetUsers (USUARIOS)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
BEGIN
    CREATE TABLE [dbo].[AspNetUsers](
        [Id] [nvarchar](450) NOT NULL PRIMARY KEY,
        [UserName] [nvarchar](256) NULL,
        [NormalizedUserName] [nvarchar](256) NULL,
        [Email] [nvarchar](256) NULL,
        [NormalizedEmail] [nvarchar](256) NULL,
        [EmailConfirmed] [bit] NOT NULL DEFAULT 0,
        [PasswordHash] [nvarchar](max) NULL,
        [SecurityStamp] [nvarchar](max) NULL,
        [ConcurrencyStamp] [nvarchar](max) NULL,
        [PhoneNumber] [nvarchar](max) NULL,
        [PhoneNumberConfirmed] [bit] NOT NULL DEFAULT 0,
        [TwoFactorEnabled] [bit] NOT NULL DEFAULT 0,
        [LockoutEnd] [datetimeoffset](7) NULL,
        [LockoutEnabled] [bit] NOT NULL DEFAULT 1,
        [AccessFailedCount] [int] NOT NULL DEFAULT 0,
        [NombreCompleto] [nvarchar](150) NOT NULL,
        [TipoUsuario] [nvarchar](20) NOT NULL DEFAULT 'Comprador',
        [Descripcion] [nvarchar](500) NULL,
        [Direccion] [nvarchar](255) NULL,
        [CalificacionPromedio] [decimal](3,2) NULL,
        [NumeroResenas] [int] NOT NULL DEFAULT 0,
        [EstaActivo] [bit] NOT NULL DEFAULT 1,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE()
    )
    CREATE UNIQUE NONCLUSTERED INDEX [EmailIndex] ON [AspNetUsers]([NormalizedEmail] ASC) WHERE ([NormalizedEmail] IS NOT NULL)
    CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [AspNetUsers]([NormalizedUserName] ASC) WHERE ([NormalizedUserName] IS NOT NULL)
    CREATE NONCLUSTERED INDEX [IX_TipoUsuario] ON [AspNetUsers]([TipoUsuario] ASC)
    CREATE NONCLUSTERED INDEX [IX_EstaActivo] ON [AspNetUsers]([EstaActivo] ASC)
    PRINT '? Tabla AspNetUsers creada'
END
GO

-- ============================================
-- 3. CREAR TABLA AspNetUserRoles (RELACIÓN USUARIO-ROL)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserRoles')
BEGIN
    CREATE TABLE [dbo].[AspNetUserRoles](
        [UserId] [nvarchar](450) NOT NULL,
        [RoleId] [nvarchar](450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
        CONSTRAINT [FK_AspNetUserRoles_Roles] FOREIGN KEY([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_Users] FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    )
    CREATE NONCLUSTERED INDEX [IX_RoleId] ON [AspNetUserRoles]([RoleId] ASC)
    PRINT '? Tabla AspNetUserRoles creada'
END
GO

-- ============================================
-- 4. CREAR TABLA Categorias
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categorias')
BEGIN
    CREATE TABLE [dbo].[Categorias](
        [CategoriaId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Nombre] [nvarchar](100) NOT NULL UNIQUE,
        [Descripcion] [nvarchar](500) NULL,
        [IconoUrl] [nvarchar](255) NULL,
        [EstaActiva] [bit] NOT NULL DEFAULT 1,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE()
    )
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Nombre] ON [Categorias]([Nombre] ASC)
    CREATE NONCLUSTERED INDEX [IX_EstaActiva] ON [Categorias]([EstaActiva] ASC)
    PRINT '? Tabla Categorias creada'
END
GO

-- ============================================
-- 5. CREAR TABLA Productos
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Productos')
BEGIN
    CREATE TABLE [dbo].[Productos](
        [ProductoId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Nombre] [nvarchar](200) NOT NULL,
        [Descripcion] [nvarchar](2000) NOT NULL,
        [Precio] [decimal](10,2) NOT NULL,
        [Stock] [int] NOT NULL DEFAULT 0,
        [StockMinimo] [int] NOT NULL DEFAULT 0,
        [Estado] [nvarchar](30) NOT NULL DEFAULT 'Disponible',
        [SKU] [nvarchar](50) NULL UNIQUE,
        [CalificacionPromedio] [decimal](3,2) NULL,
        [NumeroResenas] [int] NOT NULL DEFAULT 0,
        [NumeroVisualizaciones] [int] NOT NULL DEFAULT 0,
        [CategoriaId] [int] NOT NULL,
        [VendedorId] [nvarchar](450) NOT NULL,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        [FechaActualizacion] [datetime2](7) NULL,
        CONSTRAINT [FK_Productos_Categorias] FOREIGN KEY([CategoriaId]) REFERENCES [dbo].[Categorias] ([CategoriaId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Productos_Vendedores] FOREIGN KEY([VendedorId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE RESTRICT
    )
    CREATE UNIQUE NONCLUSTERED INDEX [IX_SKU] ON [Productos]([SKU] ASC) WHERE [SKU] IS NOT NULL
    CREATE NONCLUSTERED INDEX [IX_CategoriaId] ON [Productos]([CategoriaId] ASC)
    CREATE NONCLUSTERED INDEX [IX_VendedorId] ON [Productos]([VendedorId] ASC)
    CREATE NONCLUSTERED INDEX [IX_Estado] ON [Productos]([Estado] ASC)
    CREATE NONCLUSTERED INDEX [IX_Nombre] ON [Productos]([Nombre] ASC)
    PRINT '? Tabla Productos creada'
END
GO

-- ============================================
-- 6. CREAR TABLA Multimedia (IMÁGENES Y VIDEOS)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Multimedia')
BEGIN
    CREATE TABLE [dbo].[Multimedia](
        [MultimediaId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [ProductoId] [int] NOT NULL,
        [Tipo] [nvarchar](30) NOT NULL,
        [Nombre] [nvarchar](255) NOT NULL,
        [Url] [nvarchar](500) NOT NULL,
        [NombreArchivo] [nvarchar](255) NULL,
        [Descripcion] [nvarchar](500) NULL,
        [TamanoBytes] [bigint] NULL,
        [TipoMime] [nvarchar](50) NULL,
        [Orden] [int] NOT NULL DEFAULT 0,
        [EsPrincipal] [bit] NOT NULL DEFAULT 0,
        [EstaActivo] [bit] NOT NULL DEFAULT 1,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_Multimedia_Productos] FOREIGN KEY([ProductoId]) REFERENCES [dbo].[Productos] ([ProductoId]) ON DELETE CASCADE
    )
    CREATE NONCLUSTERED INDEX [IX_ProductoId] ON [Multimedia]([ProductoId] ASC)
    CREATE NONCLUSTERED INDEX [IX_Tipo] ON [Multimedia]([Tipo] ASC)
    CREATE NONCLUSTERED INDEX [IX_EstaActivo] ON [Multimedia]([EstaActivo] ASC)
    PRINT '? Tabla Multimedia creada'
END
GO

-- ============================================
-- 7. CREAR TABLA Ordenes (COMPRAS)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Ordenes')
BEGIN
    CREATE TABLE [dbo].[Ordenes](
        [OrdenId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [NumeroOrden] [nvarchar](50) NOT NULL UNIQUE,
        [CompradorId] [nvarchar](450) NOT NULL,
        [MontoTotal] [decimal](10,2) NOT NULL,
        [Impuesto] [decimal](10,2) NOT NULL DEFAULT 0,
        [MontoEnvio] [decimal](10,2) NOT NULL DEFAULT 0,
        [Descuento] [decimal](10,2) NOT NULL DEFAULT 0,
        [Estado] [nvarchar](30) NOT NULL DEFAULT 'Pendiente',
        [MetodoPago] [nvarchar](50) NULL,
        [DireccionEntrega] [nvarchar](255) NOT NULL,
        [Notas] [nvarchar](500) NULL,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        [FechaActualizacion] [datetime2](7) NULL,
        CONSTRAINT [UQ_NumeroOrden] UNIQUE ([NumeroOrden]),
        CONSTRAINT [FK_Ordenes_Compradores] FOREIGN KEY([CompradorId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE RESTRICT
    )
    CREATE UNIQUE NONCLUSTERED INDEX [IX_NumeroOrden] ON [Ordenes]([NumeroOrden] ASC)
    CREATE NONCLUSTERED INDEX [IX_CompradorId] ON [Ordenes]([CompradorId] ASC)
    CREATE NONCLUSTERED INDEX [IX_Estado] ON [Ordenes]([Estado] ASC)
    CREATE NONCLUSTERED INDEX [IX_FechaCreacion] ON [Ordenes]([FechaCreacion] ASC)
    PRINT '? Tabla Ordenes creada'
END
GO

-- ============================================
-- 8. CREAR TABLA OrdenDetalles (ITEMS DE ÓRDENES)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrdenDetalles')
BEGIN
    CREATE TABLE [dbo].[OrdenDetalles](
        [OrdenDetalleId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [OrdenId] [int] NOT NULL,
        [ProductoId] [int] NOT NULL,
        [Cantidad] [int] NOT NULL,
        [PrecioUnitario] [decimal](10,2) NOT NULL,
        [Subtotal] [decimal](10,2) NOT NULL,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_OrdenDetalles_Ordenes] FOREIGN KEY([OrdenId]) REFERENCES [dbo].[Ordenes] ([OrdenId]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrdenDetalles_Productos] FOREIGN KEY([ProductoId]) REFERENCES [dbo].[Productos] ([ProductoId]) ON DELETE RESTRICT
    )
    CREATE NONCLUSTERED INDEX [IX_OrdenId] ON [OrdenDetalles]([OrdenId] ASC)
    CREATE NONCLUSTERED INDEX [IX_ProductoId] ON [OrdenDetalles]([ProductoId] ASC)
    PRINT '? Tabla OrdenDetalles creada'
END
GO

-- ============================================
-- 9. CREAR TABLA Preguntas (Q&A)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Preguntas')
BEGIN
    CREATE TABLE [dbo].[Preguntas](
        [PreguntaId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [ProductoId] [int] NOT NULL,
        [UsuarioId] [nvarchar](450) NOT NULL,
        [Titulo] [nvarchar](500) NOT NULL,
        [Descripcion] [nvarchar](2000) NULL,
        [Estado] [nvarchar](20) NOT NULL DEFAULT 'Pendiente',
        [VotosUtiles] [int] NOT NULL DEFAULT 0,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_Preguntas_Productos] FOREIGN KEY([ProductoId]) REFERENCES [dbo].[Productos] ([ProductoId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Preguntas_Usuarios] FOREIGN KEY([UsuarioId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE RESTRICT
    )
    CREATE NONCLUSTERED INDEX [IX_ProductoId] ON [Preguntas]([ProductoId] ASC)
    CREATE NONCLUSTERED INDEX [IX_UsuarioId] ON [Preguntas]([UsuarioId] ASC)
    CREATE NONCLUSTERED INDEX [IX_Estado] ON [Preguntas]([Estado] ASC)
    CREATE NONCLUSTERED INDEX [IX_FechaCreacion] ON [Preguntas]([FechaCreacion] ASC)
    PRINT '? Tabla Preguntas creada'
END
GO

-- ============================================
-- 10. CREAR TABLA Respuestas (Q&A)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Respuestas')
BEGIN
    CREATE TABLE [dbo].[Respuestas](
        [RespuestaId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [PreguntaId] [int] NOT NULL,
        [UsuarioId] [nvarchar](450) NOT NULL,
        [Contenido] [nvarchar](2000) NOT NULL,
        [VotosUtiles] [int] NOT NULL DEFAULT 0,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_Respuestas_Preguntas] FOREIGN KEY([PreguntaId]) REFERENCES [dbo].[Preguntas] ([PreguntaId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Respuestas_Usuarios] FOREIGN KEY([UsuarioId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE RESTRICT
    )
    CREATE NONCLUSTERED INDEX [IX_PreguntaId] ON [Respuestas]([PreguntaId] ASC)
    CREATE NONCLUSTERED INDEX [IX_UsuarioId] ON [Respuestas]([UsuarioId] ASC)
    CREATE NONCLUSTERED INDEX [IX_FechaCreacion] ON [Respuestas]([FechaCreacion] ASC)
    PRINT '? Tabla Respuestas creada'
END
GO

-- ============================================
-- 11. CREAR TABLA Calificaciones (RESEÑAS)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Calificaciones')
BEGIN
    CREATE TABLE [dbo].[Calificaciones](
        [CalificacionId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [VendedorId] [nvarchar](450) NOT NULL,
        [UsuarioId] [nvarchar](450) NOT NULL,
        [OrdenId] [int] NULL,
        [Puntaje] [int] NOT NULL,
        [Comentario] [nvarchar](500) NULL,
        [Tipo] [nvarchar](30) NOT NULL DEFAULT 'Comprador',
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_Calificaciones_Vendedores] FOREIGN KEY([VendedorId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE RESTRICT,
        CONSTRAINT [FK_Calificaciones_Usuarios] FOREIGN KEY([UsuarioId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE RESTRICT,
        CONSTRAINT [FK_Calificaciones_Ordenes] FOREIGN KEY([OrdenId]) REFERENCES [dbo].[Ordenes] ([OrdenId]) ON DELETE SET NULL
    )
    CREATE NONCLUSTERED INDEX [IX_VendedorId] ON [Calificaciones]([VendedorId] ASC)
    CREATE NONCLUSTERED INDEX [IX_UsuarioId] ON [Calificaciones]([UsuarioId] ASC)
    CREATE NONCLUSTERED INDEX [IX_Tipo] ON [Calificaciones]([Tipo] ASC)
    CREATE NONCLUSTERED INDEX [IX_FechaCreacion] ON [Calificaciones]([FechaCreacion] ASC)
    PRINT '? Tabla Calificaciones creada'
END
GO

-- ============================================
-- INSERTAR ROLES INICIALES
-- ============================================
IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE Name = 'Admin')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) 
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID())
    PRINT '? Rol Admin insertado'
END

IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE Name = 'Vendedor')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) 
    VALUES (NEWID(), 'Vendedor', 'VENDEDOR', NEWID())
    PRINT '? Rol Vendedor insertado'
END

IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE Name = 'Comprador')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) 
    VALUES (NEWID(), 'Comprador', 'COMPRADOR', NEWID())
    PRINT '? Rol Comprador insertado'
END
GO

-- ============================================
-- INSERTAR CATEGORÍAS INICIALES
-- ============================================
IF NOT EXISTS (SELECT * FROM Categorias WHERE Nombre = 'Electrónica')
BEGIN
    INSERT INTO Categorias (Nombre, Descripcion, IconoUrl, EstaActiva) 
    VALUES ('Electrónica', 'Productos electrónicos y tecnología', 'bi bi-laptop', 1)
    PRINT '? Categoría Electrónica insertada'
END

IF NOT EXISTS (SELECT * FROM Categorias WHERE Nombre = 'Ropa')
BEGIN
    INSERT INTO Categorias (Nombre, Descripcion, IconoUrl, EstaActiva) 
    VALUES ('Ropa', 'Prendas de vestir y accesorios', 'bi bi-bag', 1)
    PRINT '? Categoría Ropa insertada'
END

IF NOT EXISTS (SELECT * FROM Categorias WHERE Nombre = 'Hogar')
BEGIN
    INSERT INTO Categorias (Nombre, Descripcion, IconoUrl, EstaActiva) 
    VALUES ('Hogar', 'Artículos para el hogar', 'bi bi-house', 1)
    PRINT '? Categoría Hogar insertada'
END

IF NOT EXISTS (SELECT * FROM Categorias WHERE Nombre = 'Deportes')
BEGIN
    INSERT INTO Categorias (Nombre, Descripcion, IconoUrl, EstaActiva) 
    VALUES ('Deportes', 'Artículos deportivos y fitness', 'bi bi-dribbble', 1)
    PRINT '? Categoría Deportes insertada'
END

IF NOT EXISTS (SELECT * FROM Categorias WHERE Nombre = 'Libros')
BEGIN
    INSERT INTO Categorias (Nombre, Descripcion, IconoUrl, EstaActiva) 
    VALUES ('Libros', 'Libros y material de lectura', 'bi bi-book', 1)
    PRINT '? Categoría Libros insertada'
END
GO

-- ============================================
-- VERIFICACIÓN FINAL
-- ============================================
PRINT ''
PRINT '================================'
PRINT '? BASE DE DATOS COMPLETAMENTE CREADA'
PRINT '================================'
PRINT ''

SELECT 
    COUNT(*) as [Tablas Creadas]
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo'

SELECT 
    COUNT(*) as [Índices Creados]
FROM sys.indexes 
WHERE object_id > 0 AND name LIKE 'IX_%'

SELECT 
    COUNT(*) as [Roles Creados]
FROM AspNetRoles

SELECT 
    COUNT(*) as [Categorías Creadas]
FROM Categorias

PRINT ''
PRINT 'Las tablas y relaciones están listas para usar.'
PRINT 'Próximo paso: dotnet run'
PRINT ''
GO
