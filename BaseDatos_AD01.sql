-- ============================================================
-- Script SQL - AD01: Diseño de Arquitecturas Distribuidas
-- Autor: Luis Tuquinga
-- Descripcion: Creacion de las 3 bases de datos para el
--              sistema de Comercio Multibodega con microservicios
-- ============================================================


-- ============================================================
-- BASE DE DATOS 1: CatalogoDB (Microservicio 1 - Catalogo.Api)
-- Responsabilidad: Administrar informacion comercial de productos
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CatalogoDB')
    CREATE DATABASE CatalogoDB;
GO

USE CatalogoDB;
GO

-- Tabla: Categoria
CREATE TABLE Categoria (
    IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255) NULL
);
GO

-- Tabla: Marca
CREATE TABLE Marca (
    IdMarca    INT IDENTITY(1,1) PRIMARY KEY,
    Nombre     NVARCHAR(100) NOT NULL,
    PaisOrigen NVARCHAR(100) NULL
);
GO

-- Tabla: Producto
CREATE TABLE Producto (
    IdProducto  INT IDENTITY(1,1) PRIMARY KEY,
    IdCategoria INT NOT NULL,
    IdMarca     INT NOT NULL,
    Nombre      NVARCHAR(150) NOT NULL,
    SKU         NVARCHAR(50)  NULL,
    Precio      DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    Estado      BIT           NOT NULL DEFAULT 1,
    CONSTRAINT FK_Producto_Categoria FOREIGN KEY (IdCategoria) REFERENCES Categoria(IdCategoria),
    CONSTRAINT FK_Producto_Marca     FOREIGN KEY (IdMarca)     REFERENCES Marca(IdMarca)
);
GO

-- Datos de prueba CatalogoDB
INSERT INTO Categoria (Nombre, Descripcion) VALUES
    ('Laptops',       'Equipos portatiles de computo'),
    ('Monitores',     'Pantallas y monitores de escritorio'),
    ('Accesorios',    'Teclados, mouses y perifericos');

INSERT INTO Marca (Nombre, PaisOrigen) VALUES
    ('Dell',    'Estados Unidos'),
    ('HP',      'Estados Unidos'),
    ('Samsung', 'Corea del Sur');

INSERT INTO Producto (IdCategoria, IdMarca, Nombre, SKU, Precio, Estado) VALUES
    (1, 1, 'Dell Inspiron 15',     'DELL-INS-15',  799.99, 1),
    (1, 2, 'HP Pavilion 14',       'HP-PAV-14',    649.99, 1),
    (2, 3, 'Samsung Monitor 27"',  'SAM-MON-27',   329.99, 1),
    (3, 1, 'Dell Mouse Inalambrico','DELL-MSE-W1', 29.99,  1);
GO


-- ============================================================
-- BASE DE DATOS 2: InventarioDB (Microservicio 2 - Inventario.Api)
-- Responsabilidad: Gestionar almacenamiento y movimientos fisicos
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'InventarioDB')
    CREATE DATABASE InventarioDB;
GO

USE InventarioDB;
GO

-- Tabla: Bodega
CREATE TABLE Bodega (
    IdBodega   INT IDENTITY(1,1) PRIMARY KEY,
    Nombre     NVARCHAR(100) NOT NULL,
    Ciudad     NVARCHAR(100) NULL,
    Direccion  NVARCHAR(200) NULL
);
GO

-- Tabla: Existencia
-- (IdProducto referencia logica a CatalogoDB.Producto)
CREATE TABLE Existencia (
    IdExistencia INT IDENTITY(1,1) PRIMARY KEY,
    IdProducto   INT NOT NULL,
    IdBodega     INT NOT NULL,
    Stock        INT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Existencia_Bodega FOREIGN KEY (IdBodega) REFERENCES Bodega(IdBodega)
);
GO

-- Tabla: MovimientoInventario
CREATE TABLE MovimientoInventario (
    IdMovimiento INT IDENTITY(1,1) PRIMARY KEY,
    IdProducto   INT           NOT NULL,
    IdBodega     INT           NOT NULL,
    Tipo         NVARCHAR(20)  NOT NULL,  -- 'ENTRADA' o 'SALIDA'
    Cantidad     INT           NOT NULL,
    Fecha        DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Movimiento_Bodega FOREIGN KEY (IdBodega) REFERENCES Bodega(IdBodega)
);
GO

-- Tabla: ReservaStock
CREATE TABLE ReservaStock (
    IdReserva  INT IDENTITY(1,1) PRIMARY KEY,
    IdProducto INT          NOT NULL,
    IdBodega   INT          NOT NULL,
    Cantidad   INT          NOT NULL,
    Estado     NVARCHAR(20) NOT NULL DEFAULT 'PENDIENTE',  -- 'PENDIENTE', 'CONFIRMADA', 'CANCELADA'
    CONSTRAINT FK_Reserva_Bodega FOREIGN KEY (IdBodega) REFERENCES Bodega(IdBodega)
);
GO

-- Datos de prueba InventarioDB
INSERT INTO Bodega (Nombre, Ciudad, Direccion) VALUES
    ('Bodega Central',   'Quito',      'Av. 10 de Agosto N25-30'),
    ('Bodega Norte',     'Quito',      'Av. America N45-10'),
    ('Bodega Guayaquil', 'Guayaquil',  'Av. 9 de Octubre 100');

INSERT INTO Existencia (IdProducto, IdBodega, Stock) VALUES
    (1, 1, 50),
    (1, 2, 20),
    (2, 1, 35),
    (3, 3, 15),
    (4, 1, 100);

INSERT INTO MovimientoInventario (IdProducto, IdBodega, Tipo, Cantidad, Fecha) VALUES
    (1, 1, 'ENTRADA', 50, GETDATE()),
    (2, 1, 'ENTRADA', 35, GETDATE()),
    (1, 2, 'ENTRADA', 20, GETDATE());

INSERT INTO ReservaStock (IdProducto, IdBodega, Cantidad, Estado) VALUES
    (1, 1, 5, 'PENDIENTE'),
    (2, 1, 3, 'CONFIRMADA');
GO


-- ============================================================
-- BASE DE DATOS 3: PedidosDB (Microservicio 3 - Pedidos.Api)
-- Responsabilidad: Registrar y administrar ordenes de compra
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PedidosDB')
    CREATE DATABASE PedidosDB;
GO

USE PedidosDB;
GO

-- Tabla: Cliente
CREATE TABLE Cliente (
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Nombres   NVARCHAR(150) NOT NULL,
    Cedula    NVARCHAR(20)  NULL,
    Correo    NVARCHAR(100) NULL
);
GO

-- Tabla: Pedido
CREATE TABLE Pedido (
    IdPedido  INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente INT           NOT NULL,
    Fecha     DATETIME      NOT NULL DEFAULT GETDATE(),
    Total     DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    Estado    NVARCHAR(20)  NOT NULL DEFAULT 'PENDIENTE',  -- 'PENDIENTE', 'PROCESADO', 'ENTREGADO', 'CANCELADO'
    CONSTRAINT FK_Pedido_Cliente FOREIGN KEY (IdCliente) REFERENCES Cliente(IdCliente)
);
GO

-- Tabla: DetallePedido
-- (IdProducto referencia logica a CatalogoDB.Producto)
CREATE TABLE DetallePedido (
    IdDetalle       INT IDENTITY(1,1) PRIMARY KEY,
    IdPedido        INT           NOT NULL,
    IdProducto      INT           NOT NULL,
    Cantidad        INT           NOT NULL,
    PrecioUnitario  DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Detalle_Pedido FOREIGN KEY (IdPedido) REFERENCES Pedido(IdPedido)
);
GO

-- Datos de prueba PedidosDB
INSERT INTO Cliente (Nombres, Cedula, Correo) VALUES
    ('Juan Carlos Perez',  '1712345678', 'jperez@email.com'),
    ('Maria Lopez',        '0923456789', 'mlopez@email.com'),
    ('Carlos Ramirez',     '1756789012', 'cramirez@email.com');

INSERT INTO Pedido (IdCliente, Fecha, Total, Estado) VALUES
    (1, GETDATE(), 829.98, 'PENDIENTE'),
    (2, GETDATE(), 329.99, 'PROCESADO');

INSERT INTO DetallePedido (IdPedido, IdProducto, Cantidad, PrecioUnitario) VALUES
    (1, 1, 1, 799.99),
    (1, 4, 1, 29.99),
    (2, 3, 1, 329.99);
GO

PRINT 'Bases de datos CatalogoDB, InventarioDB y PedidosDB creadas correctamente.';
