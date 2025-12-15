-- ============================================
-- SCRIPT DE CREACIÓN - BASE DE DATOS NEXSHOP
-- ============================================
-- Servidor: ADMINISTRADOR\POOL
-- Usuario: sa
-- Contraseña: 123456
-- Base de Datos: NexShopDb
-- ============================================

-- 1. CREAR BASE DE DATOS
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'NexShopDb')
BEGIN
    CREATE DATABASE NexShopDb
    COLLATE SQL_Latin1_General_CP1_CI_AS
END
GO

USE NexShopDb
GO

-- 2. CREAR TABLA AspNetRoles
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetRoles')
BEGIN
    CREATE TABLE [dbo].[AspNetRoles](
        [Id] [nvarchar](450) NOT NULL,
        [Name] [nvarchar](256) NULL,
        [NormalizedName] [nvarchar](256) NULL,
        [ConcurrencyStamp] [nvarchar](max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- 3. CREAR TABLA AspNetUsers
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUsers')
BEGIN
    CREATE TABLE [dbo].[AspNetUsers](
        [Id] [nvarchar](450) NOT NULL,
        [UserName] [nvarchar](256) NULL,
        [NormalizedUserName] [nvarchar](256) NULL,
        [Email] [nvarchar](256) NULL,
        [NormalizedEmail] [nvarchar](256) NULL,
        [EmailConfirmed] [bit] NOT NULL,
        [PasswordHash] [nvarchar](max) NULL,
        [SecurityStamp] [nvarchar](max) NULL,
        [ConcurrencyStamp] [nvarchar](max) NULL,
        [PhoneNumber] [nvarchar](max) NULL,
        [PhoneNumberConfirmed] [bit] NOT NULL,
        [TwoFactorEnabled] [bit] NOT NULL,
        [LockoutEnd] [datetimeoffset](7) NULL,
        [LockoutEnabled] [bit] NOT NULL,
        [AccessFailedCount] [int] NOT NULL,
        [NombreCompleto] [nvarchar](150) NOT NULL,
        [TipoUsuario] [nvarchar](20) NOT NULL DEFAULT 'Comprador',
        [Descripcion] [nvarchar](500) NULL,
        [Direccion] [nvarchar](255) NULL,
        [CalificacionPromedio] [decimal](3,2) NULL,
        [EstaActivo] [bit] NOT NULL DEFAULT 1,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- 4. CREAR TABLA Categorias
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categorias')
BEGIN
    CREATE TABLE [dbo].[Categorias](
        [CategoriaId] [int] IDENTITY(1,1) NOT NULL,
        [Nombre] [nvarchar](100) NOT NULL,
        [Descripcion] [nvarchar](500) NULL,
        [IconoUrl] [nvarchar](255) NULL,
        [EstaActiva] [bit] NOT NULL DEFAULT 1,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Categorias] PRIMARY KEY CLUSTERED ([CategoriaId] ASC),
        CONSTRAINT [UQ_Categorias_Nombre] UNIQUE NONCLUSTERED ([Nombre] ASC)
    )
END
GO

-- 5. CREAR TABLA Productos
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Productos')
BEGIN
    CREATE TABLE [dbo].[Productos](
        [ProductoId] [int] IDENTITY(1,1) NOT NULL,
        [Nombre] [nvarchar](200) NOT NULL,
        [Descripcion] [nvarchar](2000) NOT NULL,
        [Precio] [decimal](10,2) NOT NULL,
        [Stock] [int] NOT NULL DEFAULT 0,
        [StockMinimo] [int] NOT NULL DEFAULT 0,
        [Estado] [nvarchar](30) NOT NULL DEFAULT 'Disponible',
        [SKU] [nvarchar](50) NULL,
        [CalificacionPromedio] [decimal](3,2) NULL,
        [NumeroResenas] [int] NOT NULL DEFAULT 0,
        [NumeroVisualizaciones] [int] NOT NULL DEFAULT 0,
        [CategoriaId] [int] NOT NULL,
        [VendedorId] [nvarchar](450) NOT NULL,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        [FechaActualizacion] [datetime2](7) NULL,
        CONSTRAINT [PK_Productos] PRIMARY KEY CLUSTERED ([ProductoId] ASC),
        CONSTRAINT [UQ_Productos_SKU] UNIQUE NONCLUSTERED ([SKU] ASC),
        CONSTRAINT [FK_Productos_Categorias] FOREIGN KEY([CategoriaId]) REFERENCES [dbo].[Categorias] ([CategoriaId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Productos_Vendedores] FOREIGN KEY([VendedorId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION
    )
END
GO

-- 6. CREAR TABLA Multimedia
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Multimedia')
BEGIN
    CREATE TABLE [dbo].[Multimedia](
        [MultimediaId] [int] IDENTITY(1,1) NOT NULL,
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
        CONSTRAINT [PK_Multimedia] PRIMARY KEY CLUSTERED ([MultimediaId] ASC),
        CONSTRAINT [FK_Multimedia_Productos] FOREIGN KEY([ProductoId]) REFERENCES [dbo].[Productos] ([ProductoId) ON DELETE CASCADE
    )
END
GO

-- 7. CREAR TABLA Ordenes
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Ordenes')
BEGIN
    CREATE TABLE [dbo].[Ordenes](
        [OrdenId] [int] IDENTITY(1,1) NOT NULL,
        [NumeroOrden] [nvarchar](50) NOT NULL,
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
        CONSTRAINT [PK_Ordenes] PRIMARY KEY CLUSTERED ([OrdenId] ASC),
        CONSTRAINT [UQ_Ordenes_NumeroOrden] UNIQUE NONCLUSTERED ([NumeroOrden] ASC),
        CONSTRAINT [FK_Ordenes_Compradores] FOREIGN KEY([CompradorId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION
    )
END
GO

-- 8. CREAR TABLA OrdenDetalles
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrdenDetalles')
BEGIN
    CREATE TABLE [dbo].[OrdenDetalles](
        [OrdenDetalleId] [int] IDENTITY(1,1) NOT NULL,
        [OrdenId] [int] NOT NULL,
        [ProductoId] [int] NOT NULL,
        [Cantidad] [int] NOT NULL,
        [PrecioUnitario] [decimal](10,2) NOT NULL,
        [Subtotal] [decimal](10,2) NOT NULL,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_OrdenDetalles] PRIMARY KEY CLUSTERED ([OrdenDetalleId] ASC),
        CONSTRAINT [FK_OrdenDetalles_Ordenes] FOREIGN KEY([OrdenId]) REFERENCES [dbo].[Ordenes] ([OrdenId]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrdenDetalles_Productos] FOREIGN KEY([ProductoId]) REFERENCES [dbo].[Productos] ([ProductoId]) ON DELETE NO ACTION
    )
END
GO

-- 9. CREAR TABLA Preguntas
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Preguntas')
BEGIN
    CREATE TABLE [dbo].[Preguntas](
        [PreguntaId] [int] IDENTITY(1,1) NOT NULL,
        [ProductoId] [int] NOT NULL,
        [UsuarioId] [nvarchar](450) NOT NULL,
        [Titulo] [nvarchar](500) NOT NULL,
        [Descripcion] [nvarchar](2000) NULL,
        [Estado] [nvarchar](20) NOT NULL DEFAULT 'Pendiente',
        [VotosUtiles] [int] NOT NULL DEFAULT 0,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Preguntas] PRIMARY KEY CLUSTERED ([PreguntaId] ASC),
        CONSTRAINT [FK_Preguntas_Productos] FOREIGN KEY([ProductoId]) REFERENCES [dbo].[Productos] ([ProductoId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Preguntas_Usuarios] FOREIGN KEY([UsuarioId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION
    )
END
GO

-- 10. CREAR TABLA Respuestas
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Respuestas')
BEGIN
    CREATE TABLE [dbo].[Respuestas](
        [RespuestaId] [int] IDENTITY(1,1) NOT NULL,
        [PreguntaId] [int] NOT NULL,
        [UsuarioId] [nvarchar](450) NOT NULL,
        [Contenido] [nvarchar](2000) NOT NULL,
        [VotosUtiles] [int] NOT NULL DEFAULT 0,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Respuestas] PRIMARY KEY CLUSTERED ([RespuestaId] ASC),
        CONSTRAINT [FK_Respuestas_Preguntas] FOREIGN KEY([PreguntaId]) REFERENCES [dbo].[Preguntas] ([PreguntaId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Respuestas_Usuarios] FOREIGN KEY([UsuarioId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION
    )
END
GO

-- 11. CREAR TABLA Calificaciones
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Calificaciones')
BEGIN
    CREATE TABLE [dbo].[Calificaciones](
        [CalificacionId] [int] IDENTITY(1,1) NOT NULL,
        [VendedorId] [nvarchar](450) NOT NULL,
        [UsuarioId] [nvarchar](450) NOT NULL,
        [OrdenId] [int] NULL,
        [Puntaje] [int] NOT NULL,
        [Comentario] [nvarchar](500) NULL,
        [Tipo] [nvarchar](30) NOT NULL DEFAULT 'Comprador',
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Calificaciones] PRIMARY KEY CLUSTERED ([CalificacionId] ASC),
        CONSTRAINT [FK_Calificaciones_Vendedores] FOREIGN KEY([VendedorId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Calificaciones_Usuarios] FOREIGN KEY([UsuarioId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Calificaciones_Ordenes] FOREIGN KEY([OrdenId]) REFERENCES [dbo].[Ordenes] ([OrdenId]) ON DELETE SET NULL
    )
END
GO

-- 12. CREAR TABLA AspNetUserRoles
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserRoles')
BEGIN
    CREATE TABLE [dbo].[AspNetUserRoles](
        [UserId] [nvarchar](450) NOT NULL,
        [RoleId] [nvarchar](450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
        CONSTRAINT [FK_AspNetUserRoles_Roles] FOREIGN KEY([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_Users] FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    )
END
GO

-- 13. CREAR ÍNDICES PARA PERFORMANCE
CREATE NONCLUSTERED INDEX [IX_Productos_CategoriaId] ON [dbo].[Productos] ([CategoriaId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Productos_VendedorId] ON [dbo].[Productos] ([VendedorId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Productos_Estado] ON [dbo].[Productos] ([Estado] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Multimedia_ProductoId] ON [dbo].[Multimedia] ([ProductoId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Ordenes_CompradorId] ON [dbo].[Ordenes] ([CompradorId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Ordenes_Estado] ON [dbo].[Ordenes] ([Estado] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Ordenes_FechaCreacion] ON [dbo].[Ordenes] ([FechaCreacion] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Preguntas_ProductoId] ON [dbo].[Preguntas] ([ProductoId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Preguntas_UsuarioId] ON [dbo].[Preguntas] ([UsuarioId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Preguntas_Estado] ON [dbo].[Preguntas] ([Estado] ASC)
GO

-- 14. INSERTAR ROLES
IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE Name = 'Admin')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName) 
    VALUES (NEWID(), 'Admin', 'ADMIN')
END
GO

IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE Name = 'Vendedor')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName) 
    VALUES (NEWID(), 'Vendedor', 'VENDEDOR')
END
GO

IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE Name = 'Comprador')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName) 
    VALUES (NEWID(), 'Comprador', 'COMPRADOR')
END
GO

-- 15. INSERTAR CATEGORÍAS
IF NOT EXISTS (SELECT * FROM Categorias WHERE Nombre = 'Electrónica')
BEGIN
    INSERT INTO Categorias (Nombre, Descripcion, IconoUrl) 
    VALUES ('Electrónica', 'Productos electrónicos y tecnología', 'bi bi-laptop')
END
GO

IF NOT EXISTS (SELECT * FROM Categorias WHERE Nombre = 'Ropa')
BEGIN
    INSERT INTO Categorias (Nombre, Descripcion, IconoUrl) 
    VALUES ('Ropa', 'Prendas de vestir y accesorios', 'bi bi-bag')
END
GO

IF NOT EXISTS (SELECT * FROM Categorias WHERE Nombre = 'Hogar')
BEGIN
    INSERT INTO Categorias (Nombre, Descripcion, IconoUrl) 
    VALUES ('Hogar', 'Artículos para el hogar', 'bi bi-house')
END
GO

IF NOT EXISTS (SELECT * FROM Categorias WHERE Nombre = 'Deportes')
BEGIN
    INSERT INTO Categorias (Nombre, Descripcion, IconoUrl) 
    VALUES ('Deportes', 'Artículos deportivos y fitness', 'bi bi-dribbble')
END
GO

IF NOT EXISTS (SELECT * FROM Categorias WHERE Nombre = 'Libros')
BEGIN
    INSERT INTO Categorias (Nombre, Descripcion, IconoUrl) 
    VALUES ('Libros', 'Libros y material de lectura', 'bi bi-book')
END
GO

-- ============================================
-- ¡BASE DE DATOS CREADA EXITOSAMENTE!
-- ============================================
-- Ahora puedes ejecutar desde Visual Studio:
-- dotnet ef database update
-- ============================================

PRINT 'Base de datos NexShopDb creada exitosamente'
PRINT 'Tablas: 11'
PRINT 'Índices: 10'
PRINT 'Roles: 3'
PRINT 'Categorías: 5'
GO
