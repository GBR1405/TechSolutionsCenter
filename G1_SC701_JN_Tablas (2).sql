USE [master]
GO

-- Crear base de datos
CREATE DATABASE [TechSolutionsC_DB]
GO

USE [TechSolutionsC_DB]
GO

-- Crear tabla Marca
CREATE TABLE [dbo].[Marca] (
    [ID_Marca] bigint IDENTITY(1,1) NOT NULL,
    [Nombre_Marca] varchar(100) NOT NULL,
    CONSTRAINT [PK_Marca] PRIMARY KEY CLUSTERED ([ID_Marca] ASC)
) ON [PRIMARY]
GO

-- Crear tabla Tipo
CREATE TABLE [dbo].[Tipo] (
    [ID_Tipo] bigint IDENTITY(1,1) NOT NULL,
    [Tipo_Articulo] varchar(100) NOT NULL,
    CONSTRAINT [PK_Tipo] PRIMARY KEY CLUSTERED ([ID_Tipo] ASC)
) ON [PRIMARY]
GO

-- Crear tabla Art�culo
CREATE TABLE [dbo].[Articulo] (
    [ID_Articulo] bigint IDENTITY(1,1) NOT NULL,
    [Nombre] varchar(255) NOT NULL,
    [Precio] decimal(18,2) NOT NULL,
    [ID_Marca] bigint NOT NULL,
    [ID_Tipo] bigint NOT NULL,
    CONSTRAINT [PK_Articulo] PRIMARY KEY CLUSTERED ([ID_Articulo] ASC),
    CONSTRAINT [FK_Articulo_Marca] FOREIGN KEY([ID_Marca]) REFERENCES [dbo].[Marca]([ID_Marca]),
    CONSTRAINT [FK_Articulo_Tipo] FOREIGN KEY([ID_Tipo]) REFERENCES [dbo].[Tipo]([ID_Tipo])
) ON [PRIMARY]
GO

-- Crear tabla Inventario
CREATE TABLE [dbo].[Inventario] (
    [ID_Inventario] bigint IDENTITY(1,1) NOT NULL,
    [N_Lote] varchar(100) NOT NULL,
    [Cantidad] int NOT NULL,
    [ID_Articulo] bigint NOT NULL,
    CONSTRAINT [PK_Inventario] PRIMARY KEY CLUSTERED ([ID_Inventario] ASC),
    CONSTRAINT [FK_Inventario_Articulo] FOREIGN KEY([ID_Articulo]) REFERENCES [dbo].[Articulo]([ID_Articulo])
) ON [PRIMARY]
GO

-- Crear tabla Caso
CREATE TABLE [dbo].[Caso] (
    [ID_Caso] bigint IDENTITY(1,1) NOT NULL,
    [Titulo] varchar(255) NOT NULL,
    [Descripcion] text NOT NULL,
	[Estado] char NOT NULL,
    [Imagen] VARBINARY(MAX) NULL,
    [Fecha_Ingreso] datetime NULL,
    CONSTRAINT [PK_Caso] PRIMARY KEY CLUSTERED ([ID_Caso] ASC)
) ON [PRIMARY]
GO


-- Crear tabla Caso Atendido
CREATE TABLE [dbo].[Caso_Atendido] (
    [ID_CasoAtendido] bigint IDENTITY(1,1) NOT NULL,
    [Fecha_Atencion] datetime NOT NULL,
    [Fecha_Finalizado] datetime NULL,
    [ID_Caso] bigint NOT NULL,
    CONSTRAINT [PK_Caso_Atendido] PRIMARY KEY CLUSTERED ([ID_CasoAtendido] ASC),
    CONSTRAINT [FK_CasoAtendido_Caso] FOREIGN KEY([ID_Caso]) REFERENCES [dbo].[Caso]([ID_Caso])
) ON [PRIMARY]
GO

-- Crear tabla Inventario Utilizado
CREATE TABLE [dbo].[Inventario_Utilizado] (
    [ID_InventarioUtilizado] bigint IDENTITY(1,1) NOT NULL,
    [Fecha] datetime NOT NULL,
    [Cantidad] int NOT NULL,
    [ID_Inventario] bigint NOT NULL,
    [ID_CasoAtendido] bigint NOT NULL,
    CONSTRAINT [PK_Inventario_Utilizado] PRIMARY KEY CLUSTERED ([ID_InventarioUtilizado] ASC),
    CONSTRAINT [FK_InventarioUtilizado_Inventario] FOREIGN KEY([ID_Inventario]) REFERENCES [dbo].[Inventario]([ID_Inventario]),
    CONSTRAINT [FK_InventarioUtilizado_CasoAtendido] FOREIGN KEY([ID_CasoAtendido]) REFERENCES [dbo].[Caso_Atendido]([ID_CasoAtendido])
) ON [PRIMARY]
GO

-- Crear tabla Factura
CREATE TABLE [dbo].[Factura] (
    [ID_Factura] bigint IDENTITY(1,1) NOT NULL,
    [Fecha] datetime NOT NULL,
    [Estado] bit NOT NULL,
    [Comentario] varchar(500) NULL,
    [ID_CasoAtendido] bigint NOT NULL,
    CONSTRAINT [PK_Factura] PRIMARY KEY CLUSTERED ([ID_Factura] ASC),
    CONSTRAINT [FK_Factura_CasoAtendido] FOREIGN KEY([ID_CasoAtendido]) REFERENCES [dbo].[Caso_Atendido]([ID_CasoAtendido])
) ON [PRIMARY]
GO

-- Crear tabla Rol
CREATE TABLE [dbo].[Rol] (
    [ID_Rol] bigint IDENTITY(1,1) NOT NULL,
    [Tipo_Rol] varchar(100) NOT NULL,
    CONSTRAINT [PK_Rol] PRIMARY KEY CLUSTERED ([ID_Rol] ASC)
) ON [PRIMARY]
GO

-- Crear tabla Genero
CREATE TABLE [dbo].[Genero] (
    [ID_Genero] bigint IDENTITY(1,1) NOT NULL,
    [Tipo_Genero] varchar(50) NOT NULL,
    CONSTRAINT [PK_Genero] PRIMARY KEY CLUSTERED ([ID_Genero] ASC)
) ON [PRIMARY]
GO

-- Crear tabla Usuario
CREATE TABLE [dbo].[Usuario] (
    [ID_Usuario] bigint IDENTITY(1,1) NOT NULL,
    [Nombre_Usuario] varchar(255) NOT NULL,
    [Apellidos] varchar(255) NOT NULL,
    [Telefono] varchar(50) NULL,
    [Direccion] varchar(255) NULL,
    [Email] varchar(100) NOT NULL,
    [Contrasenna] varchar(255) NOT NULL,
    [ID_Genero] bigint NOT NULL,
    [ID_Rol] bigint NOT NULL,
    CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED ([ID_Usuario] ASC),
    CONSTRAINT [FK_Usuario_Genero] FOREIGN KEY([ID_Genero]) REFERENCES [dbo].[Genero]([ID_Genero]),
    CONSTRAINT [FK_Usuario_Rol] FOREIGN KEY([ID_Rol]) REFERENCES [dbo].[Rol]([ID_Rol])
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Errores] (
    [ID_Errror] bigint IDENTITY(1,1) NOT NULL,
    [Fecha] datetime NOT NULL,
    [Mensaje] varchar(255) NOT NULL,
    [StackTrace] varchar(50) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Errores]
ADD [IdUsuario] bigint NULL,  -- Agregar la columna IdUsuario como bigint
    [Origen] varchar(255) NULL;

-- Agregar columna ID_Usuario en la tabla Caso_Atendido
ALTER TABLE [dbo].[Caso_Atendido]
ADD [ID_Usuario] bigint NOT NULL;

-- Agregar la restricci�n de clave for�nea para ID_Usuario
ALTER TABLE [dbo].[Caso_Atendido]
ADD CONSTRAINT [FK_CasoAtendido_Usuario] FOREIGN KEY([ID_Usuario]) REFERENCES [dbo].[Usuario]([ID_Usuario]);


-- Agregar columna ID_Usuario en la tabla Caso
ALTER TABLE [dbo].[Caso]
ADD [ID_Usuario] bigint NOT NULL;

-- Agregar la restricci�n de clave for�nea para ID_Usuario
ALTER TABLE [dbo].[Caso]
ADD CONSTRAINT [FK_Caso_Usuario] FOREIGN KEY([ID_Usuario]) REFERENCES [dbo].[Usuario]([ID_Usuario]);


-- Insertar datos ejemplo
INSERT INTO [dbo].[Genero] ([Tipo_Genero]) VALUES (N'Masculino')
INSERT INTO [dbo].[Genero] ([Tipo_Genero]) VALUES (N'Femenino')

INSERT INTO [dbo].[Rol] ([Tipo_Rol]) VALUES (N'Administrador')
INSERT INTO [dbo].[Rol] ([Tipo_Rol]) VALUES (N'Usuario')
INSERT INTO [dbo].[Rol] ([Tipo_Rol]) VALUES (N'Gestor')
INSERT INTO [dbo].[Rol] ([Tipo_Rol]) VALUES (N'Reparador')


-- Procedimientos de Prueba
CREATE PROCEDURE [dbo].[RegistrarCuenta]
    @Nombre_Usuario varchar(255),
    @Apellidos varchar(255),
    @Telefono varchar(50) = NULL,
    @Direccion varchar(255) = NULL,
    @Email varchar(100),
    @Contrasenna varchar(255),
    @ID_Genero bigint,
    @ID_Rol bigint = NULL
AS
BEGIN
    IF @Nombre_Usuario IS NULL OR @Apellidos IS NULL OR @Email IS NULL OR @Contrasenna IS NULL OR @ID_Genero IS NULL
    BEGIN
        RAISERROR('Todos los campos obligatorios deben tener un valor.', 16, 1)
        RETURN
    END

    IF EXISTS (SELECT 1 FROM dbo.Usuario WHERE Email = @Email)
    BEGIN
        RAISERROR('El correo electr�nico ya est� registrado.', 16, 1)
        RETURN
    END

    -- Asignar el rol "Usuario" si no se proporciona el ID_Rol
    IF @ID_Rol IS NULL
    BEGIN
        SELECT @ID_Rol = ID_Rol FROM dbo.Rol WHERE Tipo_Rol = 'Usuario'
        IF @ID_Rol IS NULL
        BEGIN
            RAISERROR('El rol "Usuario" no se encontr� en la tabla Rol.', 16, 1)
            RETURN
        END
    END

    INSERT INTO dbo.Usuario (Nombre_Usuario, Apellidos, Telefono, Direccion, Email, Contrasenna, ID_Genero, ID_Rol)
    VALUES (@Nombre_Usuario, @Apellidos, @Telefono, @Direccion, @Email, @Contrasenna, @ID_Genero, @ID_Rol)
END
GO


CREATE PROCEDURE [dbo].[IniciarSesion]
    @Email varchar(100),
    @Contrasenna varchar(255)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Email = @Email)
    BEGIN
        SELECT 0 AS Indicador, 'Correo electr�nico no registrado.' AS Mensaje
        RETURN
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Email = @Email AND Contrasenna = @Contrasenna)
    BEGIN
        SELECT 0 AS Indicador, 'Contrase�a incorrecta.' AS Mensaje
        RETURN
    END

    SELECT 1 AS Indicador,
           U.ID_Usuario,
           U.Nombre_Usuario,
           U.Apellidos,
           U.Telefono,
           U.Direccion,
           U.Email,
           U.ID_Genero,
           U.ID_Rol,
           R.Tipo_Rol AS 'NombreRol'
    FROM    dbo.Usuario U
    INNER JOIN dbo.Rol R ON U.ID_Rol = R.ID_Rol
    WHERE   U.Email = @Email AND U.Contrasenna = @Contrasenna
END
GO


-- En caso de ser necesario, elimina los procedures
DROP PROCEDURE IF EXISTS dbo.IniciarSesion;
GO

DROP PROCEDURE IF EXISTS dbo.RegistrarCuenta;
GO
