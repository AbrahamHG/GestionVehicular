CREATE DATABASE GestionVehicular;
GO
USE GestionVehicular;
GO

-- =========================
-- TABLAS
-- =========================

-- Vehiculo
CREATE TABLE dbo.Vehiculo (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  Matricula VARCHAR(20) NOT NULL,
  Marca VARCHAR(50) NOT NULL,
  Modelo VARCHAR(50) NOT NULL,
  Anio INT NOT NULL,
  Tipo VARCHAR(30) NOT NULL,
  Estado VARCHAR(20) NOT NULL DEFAULT 'Disponible'
);
CREATE UNIQUE INDEX UX_Vehiculo_Matricula ON dbo.Vehiculo(Matricula);

-- Conductor
CREATE TABLE dbo.Conductor (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  NombreCompleto VARCHAR(100) NOT NULL,
  NumeroLicencia VARCHAR(30) NOT NULL,
  Contacto VARCHAR(100) NULL
);
CREATE UNIQUE INDEX UX_Conductor_NumeroLicencia ON dbo.Conductor(NumeroLicencia);

-- Asignacion
CREATE TABLE dbo.Asignacion (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  VehiculoId INT NOT NULL,
  ConductorId INT NOT NULL,
  FechaInicio DATETIME2(0) NOT NULL,
  FechaFin DATETIME2(0) NOT NULL,
  CONSTRAINT FK_Asignacion_Vehiculo FOREIGN KEY (VehiculoId) REFERENCES dbo.Vehiculo(Id),
  CONSTRAINT FK_Asignacion_Conductor FOREIGN KEY (ConductorId) REFERENCES dbo.Conductor(Id),
  CONSTRAINT CK_Asignacion_Fechas CHECK (FechaInicio < FechaFin)
);
-- Evitar duplicados exactos
CREATE UNIQUE INDEX UX_Asignacion_Unica
ON dbo.Asignacion(VehiculoId, ConductorId, FechaInicio, FechaFin);

-- Índices adicionales para rendimiento
CREATE INDEX IX_Asignacion_FechaInicio ON dbo.Asignacion(FechaInicio);
CREATE INDEX IX_Asignacion_FechaFin ON dbo.Asignacion(FechaFin);
CREATE INDEX IX_Vehiculo_Estado ON dbo.Vehiculo(Estado);

-- =========================
-- STORED PROCEDURES
-- =========================

-- Vehiculo CRUD
CREATE PROCEDURE spCrearVehiculo
  @Matricula VARCHAR(20),
  @Marca VARCHAR(50),
  @Modelo VARCHAR(50),
  @Anio INT,
  @Tipo VARCHAR(30)
AS
BEGIN
  INSERT INTO dbo.Vehiculo (Matricula, Marca, Modelo, Anio, Tipo, Estado)
  VALUES (@Matricula, @Marca, @Modelo, @Anio, @Tipo, 'Disponible');
END
GO

CREATE PROCEDURE spActualizarVehiculo
  @Id INT,
  @Marca VARCHAR(50),
  @Modelo VARCHAR(50),
  @Anio INT,
  @Tipo VARCHAR(30),
  @Estado VARCHAR(20)
AS
BEGIN
  UPDATE dbo.Vehiculo
  SET Marca=@Marca, Modelo=@Modelo, Anio=@Anio, Tipo=@Tipo, Estado=@Estado
  WHERE Id=@Id;
END
GO

CREATE PROCEDURE spEliminarVehiculo
  @Id INT
AS
BEGIN
  DELETE FROM dbo.Vehiculo WHERE Id=@Id;
END
GO

-- Conductor CRUD
CREATE PROCEDURE spCrearConductor
  @NombreCompleto VARCHAR(100),
  @NumeroLicencia VARCHAR(30),
  @Contacto VARCHAR(100)
AS
BEGIN
  INSERT INTO dbo.Conductor (NombreCompleto, NumeroLicencia, Contacto)
  VALUES (@NombreCompleto, @NumeroLicencia, @Contacto);
END
GO

CREATE PROCEDURE spActualizarConductor
  @Id INT,
  @NombreCompleto VARCHAR(100),
  @NumeroLicencia VARCHAR(30),
  @Contacto VARCHAR(100)
AS
BEGIN
  UPDATE dbo.Conductor
  SET NombreCompleto=@NombreCompleto, NumeroLicencia=@NumeroLicencia, Contacto=@Contacto
  WHERE Id=@Id;
END
GO

CREATE PROCEDURE spEliminarConductor
  @Id INT
AS
BEGIN
  DELETE FROM dbo.Conductor WHERE Id=@Id;
END
GO

CREATE PROCEDURE spObtenerConductores
AS
BEGIN
    SELECT Id, NombreCompleto, NumeroLicencia, Contacto
    FROM Conductor;
END
GO


CREATE PROCEDURE spObtenerConductorPorId
    @Id INT
AS
BEGIN
    SELECT Id, NombreCompleto, NumeroLicencia, Contacto
    FROM Conductor
    WHERE Id = @Id;
END
GO


-- Asignacion CRUD
CREATE PROCEDURE spCrearAsignacion
  @VehiculoId INT,
  @ConductorId INT,
  @FechaInicio DATETIME2(0),
  @FechaFin DATETIME2(0)
AS
BEGIN
  -- Validar que el vehículo esté disponible
  IF EXISTS (SELECT 1 FROM dbo.Vehiculo WHERE Id=@VehiculoId AND Estado='Asignado')
  BEGIN
    RAISERROR('El vehículo ya está asignado', 16, 1);
    RETURN;
  END

  -- Validar que el conductor no tenga otra asignación activa
  IF EXISTS (
    SELECT 1 FROM dbo.Asignacion
    WHERE ConductorId=@ConductorId
      AND FechaFin >= @FechaInicio
      AND FechaInicio <= @FechaFin
  )
  BEGIN
    RAISERROR('El conductor ya tiene una asignación activa en ese rango', 16, 1);
    RETURN;
  END

  INSERT INTO dbo.Asignacion (VehiculoId, ConductorId, FechaInicio, FechaFin)
  VALUES (@VehiculoId, @ConductorId, @FechaInicio, @FechaFin);

  UPDATE dbo.Vehiculo SET Estado='Asignado' WHERE Id=@VehiculoId;
END
GO

CREATE PROCEDURE spEliminarAsignacion
  @Id INT
AS
BEGIN
  DECLARE @VehiculoId INT;
  SELECT @VehiculoId = VehiculoId FROM dbo.Asignacion WHERE Id=@Id;

  DELETE FROM dbo.Asignacion WHERE Id=@Id;

  IF @VehiculoId IS NOT NULL
    UPDATE dbo.Vehiculo SET Estado='Disponible' WHERE Id=@VehiculoId;
END
GO

CREATE PROCEDURE spActualizarAsignacion
  @Id INT,
  @VehiculoId INT,
  @ConductorId INT,
  @FechaInicio DATETIME2(0),
  @FechaFin DATETIME2(0)
AS
BEGIN
  -- Validar que las fechas sean correctas
  IF (@FechaInicio >= @FechaFin)
  BEGIN
    RAISERROR('La fecha de inicio debe ser menor que la fecha de fin', 16, 1);
    RETURN;
  END

  -- Validar que el vehículo no esté asignado en el mismo rango (excepto esta asignación)
  IF EXISTS (
    SELECT 1 FROM dbo.Asignacion
    WHERE VehiculoId = @VehiculoId
      AND Id <> @Id
      AND FechaFin >= @FechaInicio
      AND FechaInicio <= @FechaFin
  )
  BEGIN
    RAISERROR('El vehículo ya está asignado en ese rango de fechas', 16, 1);
    RETURN;
  END

  -- Validar que el conductor no tenga otra asignación en el mismo rango (excepto esta asignación)
  IF EXISTS (
    SELECT 1 FROM dbo.Asignacion
    WHERE ConductorId = @ConductorId
      AND Id <> @Id
      AND FechaFin >= @FechaInicio
      AND FechaInicio <= @FechaFin
  )
  BEGIN
    RAISERROR('El conductor ya tiene una asignación en ese rango de fechas', 16, 1);
    RETURN;
  END

  -- Actualizar la asignación
  UPDATE dbo.Asignacion
  SET VehiculoId = @VehiculoId,
      ConductorId = @ConductorId,
      FechaInicio = @FechaInicio,
      FechaFin = @FechaFin
  WHERE Id = @Id;
END
GO

CREATE PROCEDURE spObtenerAsignaciones
AS
BEGIN
    SELECT 
        a.Id,
        a.VehiculoId,
        v.Matricula AS VehiculoMatricula,
        a.ConductorId,
        c.NombreCompleto AS ConductorNombre,
        a.FechaInicio,
        a.FechaFin
    FROM dbo.Asignacion a
    INNER JOIN dbo.Vehiculo v ON a.VehiculoId = v.Id
    INNER JOIN dbo.Conductor c ON a.ConductorId = c.Id
    ORDER BY a.FechaInicio DESC;
END
GO

CREATE PROCEDURE spObtenerAsignacionPorId
    @Id INT
AS
BEGIN
    SELECT 
        a.Id,
        a.VehiculoId,
        a.ConductorId,
        a.FechaInicio,
        a.FechaFin,
        v.Matricula AS VehiculoMatricula,
        c.NombreCompleto AS ConductorNombre
    FROM Asignacion a
    INNER JOIN Vehiculo v ON a.VehiculoId = v.Id
    INNER JOIN Conductor c ON a.ConductorId = c.Id
    WHERE a.Id = @Id;
END
GO



CREATE PROCEDURE spObtenerVehiculos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        v.Id,
        v.Matricula,
        v.Marca,
        v.Modelo,
        v.Anio,
        v.Tipo,
        v.Estado
    FROM Vehiculo v;
END
GO

CREATE PROCEDURE spObtenerVehiculoPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        v.Id,
        v.Matricula,
        v.Marca,
        v.Modelo,
        v.Anio,
        v.Tipo,
        v.Estado
    FROM Vehiculo v
    WHERE v.Id = @Id;
END
GO


