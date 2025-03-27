SELECT * FROM Usuario WHERE ID_Usuario = 15; -- (Usa un ID válido)
SELECT * FROM Usuario WHERE Email = 'ldiaz@gmail.com' AND ID_Usuario != 15;
EXEC ActualizarPerfil 
    @ID_Usuario = 15,
    @Nombre_Usuario = 'Lucho',
    @Apellidos = 'Diaz',
    @Telefono = '99999999',
    @Direccion = 'Colombia',
    @Email = 'ldiaz@gmail.com',
    @Contrasenna = '12345678',
    @ID_Genero = 1;


	USE TechSolutionsC_DB;

	ALTER PROCEDURE [dbo].[ActualizarPerfil]
    @ID_Usuario bigint,
    @Nombre_Usuario varchar(255),
    @Apellidos varchar(255),
    @Telefono varchar(50) = NULL,
    @Direccion varchar(255) = NULL,
    @Email varchar(100),
    @Contrasenna varchar(255) = NULL,
    @ID_Genero bigint
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE ID_Usuario = @ID_Usuario)
    BEGIN
        RAISERROR('El usuario no existe.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM dbo.Usuario WHERE Email = @Email AND ID_Usuario != @ID_Usuario)
    BEGIN
        RAISERROR('El correo electrónico ya está registrado por otro usuario.', 16, 1);
        RETURN;
    END

    UPDATE dbo.Usuario
    SET Nombre_Usuario = @Nombre_Usuario,
        Apellidos = @Apellidos,
        Telefono = @Telefono,
        Direccion = @Direccion,
        Email = @Email,
        ID_Genero = @ID_Genero,
        Contrasenna = ISNULL(@Contrasenna, Contrasenna)
    WHERE ID_Usuario = @ID_Usuario;

    -- Verificar si se actualizó algo
    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('No se realizaron cambios en la actualización.', 16, 1);
        RETURN;
    END
END
GO
-----------------------------------------------------------------------------------------------------------------


ALTER PROCEDURE [dbo].[ActualizarPerfil]
    @ID_Usuario BIGINT,
    @Nombre_Usuario VARCHAR(255),
    @Apellidos VARCHAR(255),
    @Telefono VARCHAR(50) = NULL,
    @Direccion VARCHAR(255) = NULL,
    @Email VARCHAR(100),
    @Contrasenna VARCHAR(255) = NULL,
    @ID_Genero BIGINT,
    @Resultado INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Inicializar la variable de resultado en 0 (sin cambios)
    SET @Resultado = 0;

    -- Verificar si el usuario existe
    IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE ID_Usuario = @ID_Usuario)
    BEGIN
        SET @Resultado = -1; -- Usuario no encontrado
        RETURN;
    END

    -- Verificar si el correo ya está registrado por otro usuario
    IF EXISTS (SELECT 1 FROM dbo.Usuario WHERE Email = @Email AND ID_Usuario != @ID_Usuario)
    BEGIN
        SET @Resultado = -2; -- Correo ya en uso
        RETURN;
    END

    -- Actualizar datos del usuario
    UPDATE dbo.Usuario
    SET Nombre_Usuario = @Nombre_Usuario,
        Apellidos = @Apellidos,
        Telefono = @Telefono,
        Direccion = @Direccion,
        Email = @Email,
        ID_Genero = @ID_Genero,
        Contrasenna = ISNULL(@Contrasenna, Contrasenna)
    WHERE ID_Usuario = @ID_Usuario;

    -- Verificar si realmente se modificó algo
    IF @@ROWCOUNT > 0
    BEGIN
        SET @Resultado = 1; -- Actualización exitosa
    END
    ELSE
    BEGIN
        SET @Resultado = -3; -- No se realizaron cambios
    END
END
GO

