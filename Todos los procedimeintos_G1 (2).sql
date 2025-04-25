-- Procedimientos de TechSolutionsC
-- Fecha 27/03/2025

USE TechSolutionsC_DB;

-- =============================================
--                  Perfil
-- =============================================

-- SQL aparte

-- =============================================
--                  Casos
-- =============================================

CREATE PROCEDURE [dbo].[sp_AgregarCaso]
    @Titulo VARCHAR(255),
    @Descripcion TEXT,
    @Imagen VARCHAR(255) = NULL,
    @ID_Usuario BIGINT
AS
BEGIN
    SET NOCOUNT ON;
        INSERT INTO [dbo].[Caso] (
            [Titulo],
            [Descripcion],
            [Estado],
            [Imagen],
            [Fecha_Ingreso],
            [ID_Usuario]
        ) VALUES (
            @Titulo,
            @Descripcion,
            'A', -- 'A' para Activo
            @Imagen,
            GETDATE(),
            @ID_Usuario
        );
        
        -- Retornar el ID del caso insertado
        SELECT SCOPE_IDENTITY() AS ID_Caso;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerCasoPorId]
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.ID_Caso,
        c.Titulo,
        c.Descripcion,
        c.Estado,
        c.Imagen,
        c.Fecha_Ingreso,
        c.ID_Usuario,
        u.Nombre_Usuario,
        u.Apellidos,
        u.Email
    FROM 
        [dbo].[Caso] c
    LEFT JOIN 
        [dbo].[Usuario] u ON c.ID_Usuario = u.ID_Usuario
    WHERE 
        c.ID_Caso = @Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerCasosPorUsuario]
    @IdUsuario BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.ID_Caso,
        c.Titulo,
        c.Descripcion,
        c.Estado,
        c.Imagen,
        c.Fecha_Ingreso,
        c.ID_Usuario,
        u.Nombre_Usuario,
        u.Apellidos,
        u.Email
    FROM 
        [dbo].[Caso] c
    LEFT JOIN 
        [dbo].[Usuario] u ON c.ID_Usuario = u.ID_Usuario
    WHERE 
        c.ID_Usuario = @IdUsuario
    ORDER BY 
        c.Fecha_Ingreso DESC;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerTodosLosCasos]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.ID_Caso,
        c.Titulo,
        c.Descripcion,
        c.Estado,
        c.Imagen,
        c.Fecha_Ingreso,
        c.ID_Usuario,
        u.Nombre_Usuario,
        u.Apellidos,
        u.Email
    FROM 
        [dbo].[Caso] c
    LEFT JOIN 
        [dbo].[Usuario] u ON c.ID_Usuario = u.ID_Usuario
    ORDER BY 
        c.Fecha_Ingreso ASC; -- ASC para ordenar del m�s antiguo al m�s reciente
END
GO

-- =============================================
--                  Art�culo
-- =============================================

-- Agregar un art�culo
CREATE OR ALTER PROCEDURE [dbo].[sp_AgregarArticulo]
    @Nombre VARCHAR(255),
    @Precio DECIMAL(18,2),
    @ID_Marca BIGINT,
    @ID_Tipo BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        INSERT INTO [dbo].[Articulo] (
            [Nombre],
            [Precio],
            [ID_Marca],
            [ID_Tipo]
        ) VALUES (
            @Nombre,
            @Precio,
            @ID_Marca,
            @ID_Tipo
        );
        
        -- Retornar el ID del art�culo insertado
        SELECT SCOPE_IDENTITY() AS ID_Articulo;
    END TRY
    BEGIN CATCH
        
		-- Retornar error si ocurre
        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
GO

-- Actualizar un art�culo
CREATE OR ALTER PROCEDURE [dbo].[sp_ActualizarArticulo]
    @ID_Articulo BIGINT,
    @Nombre VARCHAR(255),
    @Precio DECIMAL(18,2),
    @ID_Marca BIGINT,
    @ID_Tipo BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        UPDATE [dbo].[Articulo]
        SET 
            [Nombre] = @Nombre,
            [Precio] = @Precio,
            [ID_Marca] = @ID_Marca,
            [ID_Tipo] = @ID_Tipo
        WHERE 
            [ID_Articulo] = @ID_Articulo;
            
        -- Retornar 1 si se actualiz� correctamente
        SELECT @@ROWCOUNT AS Resultado;
    END TRY
    BEGIN CATCH
        
		-- Retornar error si ocurre
        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
GO

-- Eliminar un art�culo
CREATE OR ALTER PROCEDURE [dbo].[sp_EliminarArticulo]
    @ID_Articulo BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        DELETE FROM [dbo].[Articulo]
        WHERE [ID_Articulo] = @ID_Articulo;
        
        -- Retornar 1 si se elimin� correctamente
        SELECT @@ROWCOUNT AS Resultado;
    END TRY
    BEGIN CATCH
        
		-- Retornar error si ocurre
        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
GO

-- Obtener un art�culo por ID
CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerArticuloPorId]
    @ID_Articulo BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            a.[ID_Articulo],
            a.[Nombre],
            a.[Precio],
            a.[ID_Marca],
            m.[Nombre_Marca] AS NombreMarca,
            a.[ID_Tipo],
            t.[Tipo_Articulo] AS NombreTipo
        FROM 
            [dbo].[Articulo] a
        LEFT JOIN 
            [dbo].[Marca] m ON a.[ID_Marca] = m.[ID_Marca]
        LEFT JOIN 
            [dbo].[Tipo] t ON a.[ID_Tipo] = t.[ID_Tipo]
        WHERE 
            a.[ID_Articulo] = @ID_Articulo;
    END TRY
    BEGIN CATCH
        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
GO


-- =============================================
--                  Inventario
-- =============================================

-- Agregar en el Inventario
CREATE OR ALTER PROCEDURE [dbo].[sp_AgregarInventario]
    @N_Lote VARCHAR(100),
    @Cantidad INT,
    @ID_Articulo BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        
		-- Verificar que el art�culo exista
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Articulo] WHERE [ID_Articulo] = @ID_Articulo)
        BEGIN
            SELECT -1 AS ID_Inventario, 'Error: El art�culo especificado no existe' AS Mensaje;
            RETURN;
        END
        
        -- Insertar el nuevo registro de inventario
        INSERT INTO [dbo].[Inventario] (
            [N_Lote],
            [Cantidad],
            [ID_Articulo]
        ) VALUES (
            @N_Lote,
            @Cantidad,
            @ID_Articulo
        );
        
        -- Retornar el ID del inventario insertado
        SELECT SCOPE_IDENTITY() AS ID_Inventario, 'Registro agregado correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        
		-- Retornar error si ocurre
        SELECT 
            -1 AS ID_Inventario,
            ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- Obtener el Inventario por ID
CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerInventarioPorId]
    @ID_Inventario BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            i.[ID_Inventario],
            i.[N_Lote],
            i.[Cantidad],
            i.[ID_Articulo],
            a.[Nombre] AS NombreArticulo,
            a.[Precio],
            m.[Nombre_Marca] AS Marca,
            t.[Tipo_Articulo] AS Tipo
        FROM 
            [dbo].[Inventario] i
        INNER JOIN 
            [dbo].[Articulo] a ON i.[ID_Articulo] = a.[ID_Articulo]
        LEFT JOIN 
            [dbo].[Marca] m ON a.[ID_Marca] = m.[ID_Marca]
        LEFT JOIN 
            [dbo].[Tipo] t ON a.[ID_Tipo] = t.[ID_Tipo]
        WHERE 
            i.[ID_Inventario] = @ID_Inventario;
            
        -- Si no se encontr� el registro
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'No se encontr� el registro de inventario con el ID especificado' AS Mensaje;
        END
    END TRY
    BEGIN CATCH
        
		-- Retornar error si ocurre
        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerInventarioDisponible]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            i.[ID_Inventario],
            i.[N_Lote],
            i.[Cantidad],
            i.[ID_Articulo],
            a.[Nombre] AS NombreArticulo,
            a.[Precio],
            m.[Nombre_Marca] AS Marca,
            t.[Tipo_Articulo] AS Tipo
        FROM 
            [dbo].[Inventario] i
        INNER JOIN 
            [dbo].[Articulo] a ON i.[ID_Articulo] = a.[ID_Articulo]
        LEFT JOIN 
            [dbo].[Marca] m ON a.[ID_Marca] = m.[ID_Marca]
        LEFT JOIN 
            [dbo].[Tipo] t ON a.[ID_Tipo] = t.[ID_Tipo]
        WHERE 
            i.[Cantidad] >= 1
        ORDER BY
            a.[Nombre], i.[N_Lote];
    END TRY
    BEGIN CATCH
        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerTodoElInventario]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            i.[ID_Inventario],
            i.[N_Lote],
            i.[Cantidad],
            i.[ID_Articulo],
            a.[Nombre] AS NombreArticulo,
            a.[Precio],
            m.[Nombre_Marca] AS Marca,
            t.[Tipo_Articulo] AS Tipo
        FROM 
            [dbo].[Inventario] i
        INNER JOIN 
            [dbo].[Articulo] a ON i.[ID_Articulo] = a.[ID_Articulo]
        LEFT JOIN 
            [dbo].[Marca] m ON a.[ID_Marca] = m.[ID_Marca]
        LEFT JOIN 
            [dbo].[Tipo] t ON a.[ID_Tipo] = t.[ID_Tipo]
        ORDER BY
            a.[Nombre], i.[N_Lote];
    END TRY
    BEGIN CATCH
        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_ActualizarCantidadInventario]
    @ID_Inventario BIGINT,
    @Cantidad INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Verificar que el inventario exista
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Inventario] WHERE [ID_Inventario] = @ID_Inventario)
        BEGIN
            SELECT 
                -1 AS Resultado,
                'Error: El registro de inventario especificado no existe' AS Mensaje;
            RETURN;
        END
        
        -- Actualizar la cantidad
        UPDATE [dbo].[Inventario]
        SET 
            [Cantidad] = @Cantidad
        WHERE 
            [ID_Inventario] = @ID_Inventario;
            
        -- Retornar resultado
        IF @@ROWCOUNT > 0
            SELECT 
                1 AS Resultado,
                'Cantidad de inventario actualizada correctamente' AS Mensaje;
        ELSE
            SELECT 
                0 AS Resultado,
                'No se pudo actualizar la cantidad de inventario' AS Mensaje;
    END TRY
    BEGIN CATCH
        SELECT 
            -1 AS Resultado,
            ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- =============================================
--                 Caso Atendido
-- =============================================

-- Agregar el Caso Atendido
CREATE OR ALTER PROCEDURE [dbo].[sp_AgregarCasoAtendido]
    @ID_Caso BIGINT,
    @ID_Usuario BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        
        -- Verificar que el caso exista
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Caso] WHERE [ID_Caso] = @ID_Caso)
        BEGIN
            SELECT 
                -1 AS ID_CasoAtendido, 
                'Error: El caso especificado no existe' AS Mensaje;
            RETURN;
        END
        
        -- Verificar que el usuario exista
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuario] WHERE [ID_Usuario] = @ID_Usuario)
        BEGIN
            SELECT 
                -1 AS ID_CasoAtendido, 
                'Error: El usuario especificado no existe' AS Mensaje;
            RETURN;
        END
        
        -- Insertar el nuevo caso atendido
        INSERT INTO [dbo].[Caso_Atendido] (
            [Fecha_Atencion],
            [ID_Caso],
            [ID_Usuario]
        ) VALUES (
            GETDATE(), -- Fecha actual
            @ID_Caso,
            @ID_Usuario
        );
        
        -- Retornar el ID del caso atendido insertado
        SELECT 
            SCOPE_IDENTITY() AS ID_CasoAtendido, 
            'Caso atendido registrado correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        -- Retornar error si ocurre
        SELECT 
            -1 AS ID_CasoAtendido,
            ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- Actualizar el Caso Atendido
CREATE OR ALTER PROCEDURE [dbo].[sp_ActualizarCasoAtendido]
    @ID_CasoAtendido BIGINT,
    @Fecha_Finalizado DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        
		-- Actualizar solo la fecha de finalizaci�n
        UPDATE [dbo].[Caso_Atendido]
        SET 
            [Fecha_Finalizado] = ISNULL(@Fecha_Finalizado, [Fecha_Finalizado])
        WHERE 
            [ID_CasoAtendido] = @ID_CasoAtendido;
            
        -- Retornar resultado
        IF @@ROWCOUNT > 0
            SELECT 
                @ID_CasoAtendido AS ID_CasoAtendido,
                'Caso atendido actualizado correctamente' AS Mensaje;
        ELSE
            SELECT 
                -1 AS ID_CasoAtendido,
                'Error: No se encontr� el caso atendido especificado' AS Mensaje;
    END TRY
    BEGIN CATCH
        -- Retornar error si ocurre
        SELECT 
            -1 AS ID_CasoAtendido,
            ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- Obtener el Caso Atendido por ID
CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerCasoAtendidoPorId]
    @ID_CasoAtendido BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            ca.[ID_CasoAtendido],
            ca.[Fecha_Atencion],
            ca.[Fecha_Finalizado],
            ca.[ID_Caso],
            c.[Titulo] AS TituloCaso,
            c.[Descripcion] AS DescripcionCaso,
            c.[Estado] AS EstadoCaso,
            DATEDIFF(DAY, ca.[Fecha_Atencion], ISNULL(ca.[Fecha_Finalizado], GETDATE())) AS DiasTranscurridos
        FROM 
            [dbo].[Caso_Atendido] ca
        INNER JOIN 
            [dbo].[Caso] c ON ca.[ID_Caso] = c.[ID_Caso]
        WHERE 
            ca.[ID_CasoAtendido] = @ID_CasoAtendido;
            
        -- Si no se encontr� el registro
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'No se encontr� el caso atendido con el ID especificado' AS Mensaje;
        END
    END TRY
    BEGIN CATCH
        -- Retornar error si ocurre
        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
GO


-- =============================================
--             Inventario Utilizado
-- =============================================

-- Agregar el Inventario Utilizado
CREATE OR ALTER PROCEDURE [dbo].[sp_GestionarInventarioUtilizado]
    @ID_Inventario BIGINT,
    @ID_CasoAtendido BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CantidadActual INT, @ExisteRegistro BIT = 0, @Resultado INT = 0;
    
    BEGIN TRY
        -- Verificar que el inventario existe y tiene cantidad suficiente
        SELECT @CantidadActual = Cantidad 
        FROM [dbo].[Inventario] 
        WHERE ID_Inventario = @ID_Inventario;
        
        IF @CantidadActual IS NULL
        BEGIN
            SELECT -1 AS Resultado, 'Error: El inventario especificado no existe' AS Mensaje;
            RETURN;
        END
        
        IF @CantidadActual < 1
        BEGIN
            SELECT -1 AS Resultado, 'Error: No hay suficiente cantidad en el inventario' AS Mensaje;
            RETURN;
        END
        
        -- Verificar si ya existe un registro para este inventario y caso
        IF EXISTS (
            SELECT 1 FROM [dbo].[Inventario_Utilizado] 
            WHERE ID_Inventario = @ID_Inventario AND ID_CasoAtendido = @ID_CasoAtendido
        )
        BEGIN
            SET @ExisteRegistro = 1;
            
            -- Actualizar cantidad existente (+1)
            UPDATE [dbo].[Inventario_Utilizado]
            SET Cantidad = Cantidad + 1,
                Fecha = GETDATE()
            WHERE ID_Inventario = @ID_Inventario 
            AND ID_CasoAtendido = @ID_CasoAtendido;
            
            SET @Resultado = 1;
        END
        ELSE
        BEGIN
            -- Crear nuevo registro
            INSERT INTO [dbo].[Inventario_Utilizado] (
                Fecha,
                Cantidad,
                ID_Inventario,
                ID_CasoAtendido
            ) VALUES (
                GETDATE(),
                1,
                @ID_Inventario,
                @ID_CasoAtendido
            );
            
            SET @Resultado = SCOPE_IDENTITY();
        END
        
        -- Actualizar inventario (restar 1)
        UPDATE [dbo].[Inventario]
        SET Cantidad = Cantidad - 1
        WHERE ID_Inventario = @ID_Inventario;
        
        -- Retornar resultado
        IF @ExisteRegistro = 1
            SELECT @Resultado AS Resultado, 'Cantidad actualizada correctamente' AS Mensaje;
        ELSE
            SELECT @Resultado AS Resultado, 'Registro creado correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        SELECT -1 AS Resultado, ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- Eliminar el Inventario Utilizado
CREATE OR ALTER PROCEDURE [dbo].[sp_EliminarInventarioUtilizado]
    @ID_InventarioUtilizado BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        DECLARE @Cantidad INT, @ID_Inventario BIGINT;
        
        -- Obtener datos para restaurar stock
        SELECT 
            @Cantidad = [Cantidad],
            @ID_Inventario = [ID_Inventario]
        FROM [dbo].[Inventario_Utilizado]
        WHERE [ID_InventarioUtilizado] = @ID_InventarioUtilizado;
        
        IF @ID_Inventario IS NULL
        BEGIN
            SELECT 0 AS Resultado, 'Error: Registro no encontrado' AS Mensaje;
            RETURN;
        END
        
        -- Eliminar registro
        DELETE FROM [dbo].[Inventario_Utilizado]
        WHERE [ID_InventarioUtilizado] = @ID_InventarioUtilizado;
        
        -- Restaurar stock
        UPDATE [dbo].[Inventario]
        SET [Cantidad] = [Cantidad] + @Cantidad
        WHERE [ID_Inventario] = @ID_Inventario;
        
        SELECT @@ROWCOUNT AS Resultado, 'Registro eliminado correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        SELECT -1 AS Resultado, ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- Obtener Inventario Utilizado por ID
CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerInventarioUtilizado]
    @ID_CasoAtendido BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            iu.[ID_InventarioUtilizado],
            iu.[Fecha],
            iu.[Cantidad],
            iu.[ID_Inventario],
            i.[N_Lote],
            a.[Nombre] AS NombreArticulo,
            a.[Precio],
            iu.[ID_CasoAtendido],
            ca.[Fecha_Atencion],
            c.[Titulo] AS TituloCaso
        FROM 
            [dbo].[Inventario_Utilizado] iu
        INNER JOIN 
            [dbo].[Inventario] i ON iu.[ID_Inventario] = i.[ID_Inventario]
        INNER JOIN 
            [dbo].[Articulo] a ON i.[ID_Articulo] = a.[ID_Articulo]
        INNER JOIN 
            [dbo].[Caso_Atendido] ca ON iu.[ID_CasoAtendido] = ca.[ID_CasoAtendido]
        INNER JOIN 
            [dbo].[Caso] c ON ca.[ID_Caso] = c.[ID_Caso]
        WHERE 
            iu.[ID_CasoAtendido] = @ID_CasoAtendido
        ORDER BY 
            iu.[Fecha] DESC;
            
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'No se encontraron registros de inventario utilizado para este caso' AS Mensaje;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS MensajeError;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_RestarInventarioUtilizado]
    @ID_CasoAtendido BIGINT,
    @ID_Inventario BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CantidadActual INT, @ID_InventarioUtilizado BIGINT, @Resultado INT = 0;
    
    BEGIN TRY
        -- Verificar que exista el registro de inventario utilizado para este caso e inventario
        SELECT 
            @ID_InventarioUtilizado = ID_InventarioUtilizado,
            @CantidadActual = Cantidad
        FROM [dbo].[Inventario_Utilizado]
        WHERE ID_CasoAtendido = @ID_CasoAtendido 
          AND ID_Inventario = @ID_Inventario;
        
        IF @CantidadActual IS NULL
        BEGIN
            SELECT -1 AS Resultado, 'Error: No existe un registro de este inventario utilizado en el caso especificado' AS Mensaje;
            RETURN;
        END
        
        -- Si la cantidad es 1, eliminar el registro
        IF @CantidadActual = 1
        BEGIN
            DELETE FROM [dbo].[Inventario_Utilizado]
            WHERE ID_InventarioUtilizado = @ID_InventarioUtilizado;
            
            SET @Resultado = 0; -- Indica que se elimin�
        END
        ELSE
        BEGIN
            -- Restar 1 a la cantidad
            UPDATE [dbo].[Inventario_Utilizado]
            SET Cantidad = Cantidad - 1,
                Fecha = GETDATE()
            WHERE ID_InventarioUtilizado = @ID_InventarioUtilizado;
            
            SET @Resultado = @CantidadActual - 1; -- Nueva cantidad
        END
        
        -- Actualizar inventario principal (sumar 1)
        UPDATE [dbo].[Inventario]
        SET Cantidad = Cantidad + 1
        WHERE ID_Inventario = @ID_Inventario;
        
        -- Verificar si la actualizaci�n fue exitosa
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT -1 AS Resultado, 'Error: No se pudo actualizar el inventario principal' AS Mensaje;
            RETURN;
        END
        
        -- Retornar resultado
        IF @Resultado = 0
            SELECT 0 AS Resultado, 'Registro de inventario utilizado eliminado correctamente' AS Mensaje;
        ELSE
            SELECT @Resultado AS Resultado, 'Cantidad de inventario utilizado actualizada correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        SELECT -1 AS Resultado, ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- =============================================
--				     Factura
-- =============================================

-- Agregar o Generar una Factura
CREATE OR ALTER PROCEDURE [dbo].[sp_AgregarFactura]
    @ID_CasoAtendido BIGINT,
    @Comentario VARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validar que el caso atendido exista
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Caso_Atendido] WHERE [ID_CasoAtendido] = @ID_CasoAtendido)
        BEGIN
            SELECT -1 AS ID_Factura, 'Error: El caso atendido especificado no existe' AS Mensaje;
            RETURN;
        END
        
        -- Validar que no exista ya una factura para este caso
        IF EXISTS (SELECT 1 FROM [dbo].[Factura] WHERE [ID_CasoAtendido] = @ID_CasoAtendido)
        BEGIN
            SELECT -1 AS ID_Factura, 'Error: Ya existe una factura para este caso atendido' AS Mensaje;
            RETURN;
        END
        
        -- Insertar la nueva factura
        INSERT INTO [dbo].[Factura] (
            [Fecha],
            [Estado],
            [Comentario],
            [ID_CasoAtendido]
        ) VALUES (
            GETDATE(), -- Fecha actual
            1,        -- Estado activo (1 = true)
            @Comentario,
            @ID_CasoAtendido
        );
        
        -- Retornar el ID de la factura generada
        SELECT SCOPE_IDENTITY() AS ID_Factura, 'Factura generada correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        SELECT -1 AS ID_Factura, ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- Obtener la Factura por ID
CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerFacturaPorId]
    @ID_Factura BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            f.[ID_Factura],
            f.[Fecha],
            f.[Estado],
            f.[Comentario],
            f.[ID_CasoAtendido],
            ca.[Fecha_Atencion],
            ca.[Fecha_Finalizado],
            c.[ID_Caso],
            c.[Titulo] AS TituloCaso,
            c.[Descripcion] AS DescripcionCaso,
            
			-- Calcular d�as de atenci�n (si est� finalizado)
            CASE 
                WHEN ca.[Fecha_Finalizado] IS NOT NULL THEN 
                    DATEDIFF(DAY, ca.[Fecha_Atencion], ca.[Fecha_Finalizado])
                ELSE 
                    DATEDIFF(DAY, ca.[Fecha_Atencion], GETDATE())
            END AS DiasAtencion,
           
			-- Obtener informaci�n del cliente (asumiendo que existe en tabla Caso)
            u.[Nombre_Usuario] + ' ' + u.[Apellidos] AS Cliente,
            u.[Email] AS EmailCliente
        FROM 
            [dbo].[Factura] f
        INNER JOIN 
            [dbo].[Caso_Atendido] ca ON f.[ID_CasoAtendido] = ca.[ID_CasoAtendido]
        INNER JOIN 
            [dbo].[Caso] c ON ca.[ID_Caso] = c.[ID_Caso]
        LEFT JOIN 
            [dbo].[Usuario] u ON c.[ID_Usuario] = u.[ID_Usuario]
        WHERE 
            f.[ID_Factura] = @ID_Factura;
            
        -- Si no se encontr� la factura
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'No se encontr� la factura con el ID especificado' AS Mensaje;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS MensajeError;
    END CATCH
END
GO

-- Obtener la Factura por el Caso Atendido
CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerFacturaPorCasoAtendido]
    @ID_CasoAtendido BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            f.[ID_Factura],
            f.[Fecha],
            CASE 
                WHEN f.[Estado] = 1 THEN 'Activa'
                ELSE 'Inactiva'
            END AS EstadoDescripcion,
            f.[Comentario],
            f.[ID_CasoAtendido],
           
			-- Informaci�n del caso
            c.[ID_Caso],
            c.[Titulo] AS TituloCaso,
            c.[Descripcion] AS DescripcionCaso,
            
			-- Informaci�n del cliente (asumiendo estructura de tablas relacionadas)
            u.[Nombre_Usuario] + ' ' + u.[Apellidos] AS Cliente,
            u.[Email] AS EmailCliente,
            
			-- Detalles de atenci�n
            ca.[Fecha_Atencion],
            ca.[Fecha_Finalizado],
            DATEDIFF(DAY, ca.[Fecha_Atencion], ISNULL(ca.[Fecha_Finalizado], GETDATE())) AS DiasAtencion,
            
			-- Total de art�culos utilizados (suma de cantidades)
            (SELECT SUM(iu.[Cantidad]) 
             FROM [dbo].[Inventario_Utilizado] iu 
             WHERE iu.[ID_CasoAtendido] = @ID_CasoAtendido) AS TotalArticulosUtilizados
        FROM 
            [dbo].[Factura] f
        INNER JOIN 
            [dbo].[Caso_Atendido] ca ON f.[ID_CasoAtendido] = ca.[ID_CasoAtendido]
        INNER JOIN 
            [dbo].[Caso] c ON ca.[ID_Caso] = c.[ID_Caso]
        LEFT JOIN 
            [dbo].[Usuario] u ON c.[ID_Usuario] = u.[ID_Usuario]
        WHERE 
            f.[ID_CasoAtendido] = @ID_CasoAtendido;
            
        -- Si no se encontr� la factura
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'No se encontr� factura para el caso atendido especificado' AS Mensaje;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS MensajeError;
    END CATCH
END
GO


-- =============================================
--				     Genero
-- =============================================

-- Obtener el Genero por ID
CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerGeneroPorId]
    @ID_Genero BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            [ID_Genero],
            [Tipo_Genero]
        FROM 
            [dbo].[Genero]
        WHERE 
            [ID_Genero] = @ID_Genero;
            
        -- Si no se encontr� el g�nero
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'No se encontr� el g�nero con el ID especificado' AS Mensaje;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS MensajeError;
    END CATCH
END
GO


-- =============================================
--				     Marca
-- =============================================

-- Agregar una Marca
CREATE OR ALTER PROCEDURE [dbo].[sp_AgregarMarca]
    @Nombre_Marca VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validar que el nombre no est� vac�o
        IF LEN(TRIM(@Nombre_Marca)) = 0
        BEGIN
            SELECT -1 AS ID_Marca, 'Error: El nombre de la marca no puede estar vac�o' AS Mensaje;
            RETURN;
        END
        
        -- Validar que no exista una marca con el mismo nombre
        IF EXISTS (SELECT 1 FROM [dbo].[Marca] WHERE [Nombre_Marca] = @Nombre_Marca)
        BEGIN
            SELECT -1 AS ID_Marca, 'Error: Ya existe una marca con este nombre' AS Mensaje;
            RETURN;
        END
        
        -- Insertar la nueva marca
        INSERT INTO [dbo].[Marca] (
            [Nombre_Marca]
        ) VALUES (
            @Nombre_Marca
        );
        
        -- Retornar el ID de la marca insertada
        SELECT SCOPE_IDENTITY() AS ID_Marca, 'Marca agregada correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        SELECT -1 AS ID_Marca, ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- Actualizar la Marca
CREATE OR ALTER PROCEDURE [dbo].[sp_ActualizarMarca]
    @ID_Marca BIGINT,
    @Nombre_Marca VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validar que el nombre no est� vac�o
        IF LEN(TRIM(@Nombre_Marca)) = 0
        BEGIN
            SELECT -1 AS Resultado, 'Error: El nombre de la marca no puede estar vac�o' AS Mensaje;
            RETURN;
        END
        
        -- Validar que no exista otra marca con el mismo nombre
        IF EXISTS (SELECT 1 FROM [dbo].[Marca] WHERE [Nombre_Marca] = @Nombre_Marca AND [ID_Marca] <> @ID_Marca)
        BEGIN
            SELECT -1 AS Resultado, 'Error: Ya existe otra marca con este nombre' AS Mensaje;
            RETURN;
        END
        
        -- Actualizar la marca
        UPDATE [dbo].[Marca]
        SET 
            [Nombre_Marca] = @Nombre_Marca
        WHERE 
            [ID_Marca] = @ID_Marca;
            
        -- Retornar resultado
        SELECT @@ROWCOUNT AS Resultado, 'Marca actualizada correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        SELECT -1 AS Resultado, ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- Eliminar la Marca
CREATE OR ALTER PROCEDURE [dbo].[sp_EliminarMarca]
    @ID_Marca BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validar que la marca no est� siendo usada en art�culos
        IF EXISTS (SELECT 1 FROM [dbo].[Articulo] WHERE [ID_Marca] = @ID_Marca)
        BEGIN
            SELECT 0 AS Resultado, 'Error: No se puede eliminar la marca porque est� asociada a art�culos' AS Mensaje;
            RETURN;
        END
        
        -- Eliminar la marca
        DELETE FROM [dbo].[Marca]
        WHERE [ID_Marca] = @ID_Marca;
        
        -- Retornar resultado
        SELECT @@ROWCOUNT AS Resultado, 'Marca eliminada correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        SELECT -1 AS Resultado, ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- Obtener la Marca por ID
CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerMarcaPorId]
    @ID_Marca BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            [ID_Marca],
            [Nombre_Marca]
        FROM 
            [dbo].[Marca]
        WHERE 
            [ID_Marca] = @ID_Marca;
            
        -- Si no se encontr� la marca
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'No se encontr� la marca con el ID especificado' AS Mensaje;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS MensajeError;
    END CATCH
END
GO


-- =============================================
--				     Tipo
-- =============================================

-- Agregar el Tipo de Articulo
CREATE OR ALTER PROCEDURE [dbo].[sp_AgregarTipo]
    @Tipo_Articulo VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validar que el tipo no est� vac�o
        IF LEN(TRIM(@Tipo_Articulo)) = 0
        BEGIN
            SELECT -1 AS ID_Tipo, 'Error: El tipo de art�culo no puede estar vac�o' AS Mensaje;
            RETURN;
        END
        
        -- Validar que no exista un tipo con el mismo nombre
        IF EXISTS (SELECT 1 FROM [dbo].[Tipo] WHERE [Tipo_Articulo] = @Tipo_Articulo)
        BEGIN
            SELECT -1 AS ID_Tipo, 'Error: Ya existe un tipo con este nombre' AS Mensaje;
            RETURN;
        END
        
        -- Insertar el nuevo tipo
        INSERT INTO [dbo].[Tipo] (
            [Tipo_Articulo]
        ) VALUES (
            @Tipo_Articulo
        );
        
        -- Retornar el ID del tipo insertado
        SELECT SCOPE_IDENTITY() AS ID_Tipo, 'Tipo agregado correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        SELECT -1 AS ID_Tipo, ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- Actualizar el Tipo de Articulo
CREATE OR ALTER PROCEDURE [dbo].[sp_ActualizarTipo]
    @ID_Tipo BIGINT,
    @Tipo_Articulo VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validar que el tipo no est� vac�o
        IF LEN(TRIM(@Tipo_Articulo)) = 0
        BEGIN
            SELECT -1 AS Resultado, 'Error: El tipo de art�culo no puede estar vac�o' AS Mensaje;
            RETURN;
        END
        
        -- Validar que no exista otro tipo con el mismo nombre
        IF EXISTS (SELECT 1 FROM [dbo].[Tipo] WHERE [Tipo_Articulo] = @Tipo_Articulo AND [ID_Tipo] <> @ID_Tipo)
        BEGIN
            SELECT -1 AS Resultado, 'Error: Ya existe otro tipo con este nombre' AS Mensaje;
            RETURN;
        END
        
        -- Actualizar el tipo
        UPDATE [dbo].[Tipo]
        SET 
            [Tipo_Articulo] = @Tipo_Articulo
        WHERE 
            [ID_Tipo] = @ID_Tipo;
            
        -- Retornar resultado
        SELECT @@ROWCOUNT AS Resultado, 'Tipo actualizado correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        SELECT -1 AS Resultado, ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- Eliminar el Tipo de Articulo
CREATE OR ALTER PROCEDURE [dbo].[sp_EliminarTipo]
    @ID_Tipo BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validar que el tipo no est� siendo usado en art�culos
        IF EXISTS (SELECT 1 FROM [dbo].[Articulo] WHERE [ID_Tipo] = @ID_Tipo)
        BEGIN
            SELECT 0 AS Resultado, 'Error: No se puede eliminar el tipo porque est� asociado a art�culos' AS Mensaje;
            RETURN;
        END
        
        -- Eliminar el tipo
        DELETE FROM [dbo].[Tipo]
        WHERE [ID_Tipo] = @ID_Tipo;
        
        -- Retornar resultado
        SELECT @@ROWCOUNT AS Resultado, 'Tipo eliminado correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        SELECT -1 AS Resultado, ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- Obtener el Tipo de Articulo por ID
CREATE OR ALTER PROCEDURE [dbo].[sp_ObtenerTipoPorId]
    @ID_Tipo BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            [ID_Tipo],
            [Tipo_Articulo]
        FROM 
            [dbo].[Tipo]
        WHERE 
            [ID_Tipo] = @ID_Tipo;
            
        -- Si no se encontr� el tipo
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'No se encontr� el tipo con el ID especificado' AS Mensaje;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS MensajeError;
    END CATCH
END
GO

-- =============================================
--				     Errores
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[sp_RegistrarError]
    @Mensaje VARCHAR(255),
    @StackTrace VARCHAR(50) = NULL,  -- Cambiado a VARCHAR(50) para coincidir con la tabla
    @IdUsuario BIGINT,               -- Quitado el valor por defecto ya que es NOT NULL en la tabla
    @Origen VARCHAR(100) = NULL      -- Cambiado a VARCHAR(100) para coincidir con la tabla
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validar que el mensaje no est� vac�o
        IF LEN(TRIM(@Mensaje)) = 0
        BEGIN
            RETURN; -- Salir silenciosamente si no hay mensaje
        END
        
        -- Validar longitud m�xima del StackTrace
        IF @StackTrace IS NOT NULL AND LEN(@StackTrace) > 50
        BEGIN
            SET @StackTrace = LEFT(@StackTrace, 50); -- Truncar si excede el l�mite
        END
        
        -- Validar longitud m�xima del Origen
        IF @Origen IS NOT NULL AND LEN(@Origen) > 100
        BEGIN
            SET @Origen = LEFT(@Origen, 100); -- Truncar si excede el l�mite
        END
        
        INSERT INTO [dbo].[Errores] (
            [Fecha],
            [Mensaje],
            [StackTrace],
            [ID_Usuario],  -- Nombre de columna corregido para coincidir con la tabla
            [Origen]
        ) VALUES (
            GETDATE(),
            @Mensaje,
            @StackTrace,
            @IdUsuario,
            @Origen
        );
    END TRY
    BEGIN CATCH
        -- Registrar error en el log de SQL Server si falla
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

CREATE PROCEDURE [dbo].[ValidarUsuarioCorreo]
@Email varchar(100)
AS
BEGIN
	
	SELECT U.ID_Usuario,
		   U.Nombre_Usuario,
		   U.Apellidos,
		   U.Email
	FROM dbo.Usuario U
	WHERE Email = @Email
END

CREATE PROCEDURE [dbo].[ObtenerCasosEnEstadoEntrantes]
AS
BEGIN
	
	SELECT 
        c.ID_Caso,
        c.Titulo,
        c.Descripcion,
        c.Estado,
        c.Imagen,
        c.Fecha_Ingreso,
        c.ID_Usuario,
        u.Nombre_Usuario,
        u.Apellidos,
        u.Email
    FROM 
        [dbo].[Caso] c
    LEFT JOIN 
        [dbo].[Usuario] u ON c.ID_Usuario = u.ID_Usuario
    WHERE 
        Fecha_Ingreso IS NULL
END

CREATE PROCEDURE [dbo].[ObtenerCasosEnEstadoPendiente]
AS
BEGIN
    SELECT 
        c.ID_Caso,
        c.Titulo,
        c.Descripcion,
        c.Estado,
        c.Imagen,
        c.Fecha_Ingreso,
        c.ID_Usuario,
        u.Nombre_Usuario,
        u.Apellidos,
        u.Email
    FROM 
        [dbo].[Caso] c
    LEFT JOIN 
        [dbo].[Usuario] u ON c.ID_Usuario = u.ID_Usuario
    WHERE 
        c.Fecha_Ingreso IS NOT NULL
        AND NOT EXISTS (
            SELECT 1
            FROM [dbo].[Caso_Atendido] ca
            WHERE ca.ID_Caso = c.ID_Caso
        )
END

Execute sp_ObtenerTodosLosCasos

CREATE PROCEDURE [dbo].[ObtenerCasosEnEstadoPendientePorId]
@ID_Usuario
AS
BEGIN
    SELECT 
        c.ID_Caso,
        c.Titulo,
        c.Descripcion,
        c.Estado,
        c.Imagen,
        c.Fecha_Ingreso,
        c.ID_Usuario,
        u.Nombre_Usuario,
        u.Apellidos,
        u.Email
    FROM 
        [dbo].[Caso] c
    LEFT JOIN 
        [dbo].[Usuario] u ON c.ID_Usuario = u.ID_Usuario
    WHERE 
        c.Fecha_Ingreso IS NOT NULL
        AND NOT EXISTS (
            SELECT 1
            FROM [dbo].[Caso_Atendido] ca
            WHERE ca.ID_Caso = c.ID_Caso
        )
		AND ID_Usuario = @ID_Usuario
END --Cambiar

CREATE PROCEDURE [dbo].[ActualizarContrasenna]
@Email varchar(100),
@contrasenna varchar(50)
as
begin

	UPDATE Usuario
	set Contrasenna = @contrasenna
	where Email = @Email
END


ALTER   PROCEDURE [dbo].[sp_ObtenerCasosPorUsuario]
    @IdUsuario BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.ID_Caso,
        c.Titulo,
        c.Descripcion,
        c.Estado,
        c.Imagen,
        c.Fecha_Ingreso,
        c.ID_Usuario,
        u.Nombre_Usuario,
        u.Apellidos,
        u.Email
    FROM 
        [dbo].[Caso] c
    LEFT JOIN 
        [dbo].[Usuario] u ON c.ID_Usuario = u.ID_Usuario
    WHERE 
        c.ID_Usuario = @IdUsuario
    ORDER BY 
        c.Fecha_Ingreso DESC;
END

CREATE PROCEDURE [dbo].[EditarCasoPendiente]
@idCaso varchar(100)
as
begin

	UPDATE Caso
	set Fecha_Ingreso = GETDATE()
	where ID_Caso = @idCaso
END

CREATE  PROCEDURE [dbo].[sp_ObtenerArticulos]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            a.[ID_Articulo],
            a.[Nombre],
            a.[Precio],
            a.[ID_Marca],
            m.[Nombre_Marca] AS NombreMarca,
            a.[ID_Tipo],
            t.[Tipo_Articulo] AS NombreTipo
        FROM 
            [dbo].[Articulo] a
        LEFT JOIN 
            [dbo].[Marca] m ON a.[ID_Marca] = m.[ID_Marca]
        LEFT JOIN 
            [dbo].[Tipo] t ON a.[ID_Tipo] = t.[ID_Tipo]
    END TRY
    BEGIN CATCH
        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END

EXECUTE [sp_ObtenerArticulos]