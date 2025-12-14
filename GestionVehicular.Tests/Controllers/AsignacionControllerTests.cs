using GestionVehicular.Api.Controllers;
using GestionVehicular.Core;
using GestionVehicular.Core.Dtos;
using GestionVehicular.Core.Entities;
using GestionVehicular.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class AsignacionControllerTests
{
    private AppDbContext CrearDbContextSqlServer()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=VehiculosTestDb;Trusted_Connection=True;MultipleActiveResultSets=true;Connection Timeout=60;")
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void CrearAsignacion_DeberiaRetornarOkConMensaje()
    {
        var context = CrearDbContextSqlServer();
        context.Database.ExecuteSqlRaw("DELETE FROM Asignacion");
        context.Database.ExecuteSqlRaw("DELETE FROM Vehiculo");
        context.Database.ExecuteSqlRaw("DELETE FROM Conductor");

        var logger = new Mock<ILogger<AsignacionController>>();
        var controller = new AsignacionController(context, logger.Object);

        var vehiculo = new Vehiculo { Matricula = "ASIG001", Marca = "Mazda", Modelo = "3", Anio = 2021, Tipo = "Sedán", Estado = "Disponible" };
        var conductor = new Conductor { NombreCompleto = "Juan Pérez", NumeroLicencia = "LIC001", Contacto = "555-1234" };
        context.Vehiculos.Add(vehiculo);
        context.Conductores.Add(conductor);
        context.SaveChanges();

        var asignacionDto = new AsignacionDto
        {
            VehiculoId = vehiculo.Id,
            ConductorId = conductor.Id,
            FechaInicio = DateTime.Today,
            FechaFin = DateTime.Today.AddDays(7)
        };

        var result = controller.CrearAsignacion(asignacionDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Asignacion creada correctamente", okResult.Value);
    }

    [Fact]
    public void CrearAsignacion_ConductorYaAsignado_DeberiaRetornarConflict()
    {
        var context = CrearDbContextSqlServer();
        context.Database.ExecuteSqlRaw("DELETE FROM Asignacion");
        context.Database.ExecuteSqlRaw("DELETE FROM Vehiculo");
        context.Database.ExecuteSqlRaw("DELETE FROM Conductor");

        var logger = new Mock<ILogger<AsignacionController>>();
        var controller = new AsignacionController(context, logger.Object);

        var vehiculo1 = new Vehiculo { Matricula = "ASIG002", Marca = "Toyota", Modelo = "Yaris", Anio = 2020, Tipo = "Sedán", Estado = "Disponible" };
        var vehiculo2 = new Vehiculo { Matricula = "ASIG003", Marca = "Nissan", Modelo = "Versa", Anio = 2021, Tipo = "Sedán", Estado = "Disponible" };
        var conductor = new Conductor { NombreCompleto = "Pedro López", NumeroLicencia = "LIC003", Contacto = "555-5678" };
        context.Vehiculos.AddRange(vehiculo1, vehiculo2);
        context.Conductores.Add(conductor);
        context.SaveChanges();

        controller.CrearAsignacion(new AsignacionDto
        {
            VehiculoId = vehiculo1.Id,
            ConductorId = conductor.Id,
            FechaInicio = DateTime.Today,
            FechaFin = DateTime.Today.AddDays(5)
        });

        var result = controller.CrearAsignacion(new AsignacionDto
        {
            VehiculoId = vehiculo2.Id,
            ConductorId = conductor.Id,
            FechaInicio = DateTime.Today.AddDays(3),
            FechaFin = DateTime.Today.AddDays(7)
        });

        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Contains("El conductor ya tiene una asignación activa", conflictResult.Value.ToString());
    }

    [Fact]
    public void ObtenerAsignaciones_DeberiaRetornarLista()
    {
        var context = CrearDbContextSqlServer();
        context.Database.ExecuteSqlRaw("DELETE FROM Asignacion");
        context.Database.ExecuteSqlRaw("DELETE FROM Vehiculo");
        context.Database.ExecuteSqlRaw("DELETE FROM Conductor");

        var logger = new Mock<ILogger<AsignacionController>>();
        var controller = new AsignacionController(context, logger.Object);

        var vehiculo = new Vehiculo { Matricula = "ASIG004", Marca = "Ford", Modelo = "Focus", Anio = 2019, Tipo = "Sedán", Estado = "Disponible" };
        var conductor = new Conductor { NombreCompleto = "María García", NumeroLicencia = "LIC003", Contacto = "555-9876" };
        context.Vehiculos.Add(vehiculo);
        context.Conductores.Add(conductor);
        context.SaveChanges();

        controller.CrearAsignacion(new AsignacionDto
        {
            VehiculoId = vehiculo.Id,
            ConductorId = conductor.Id,
            FechaInicio = DateTime.Today,
            FechaFin = DateTime.Today.AddDays(10)
        });

        var result = controller.ObtenerAsignaciones();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var lista = Assert.IsAssignableFrom<IEnumerable<AsignacionView>>(okResult.Value);

        Assert.Single(lista);
        Assert.Equal("ASIG004", lista.First().VehiculoMatricula);
    }
}