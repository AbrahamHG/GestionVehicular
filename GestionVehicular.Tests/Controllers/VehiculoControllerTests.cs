using GestionVehicular.Api.Controllers;
using GestionVehicular.Core.Dtos;
using GestionVehicular.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class VehiculoControllerTests
{
    private AppDbContext CrearDbContextSqlServer()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=VehiculosTestDb;Trusted_Connection=True;MultipleActiveResultSets=true;Connection Timeout=60;")
            .Options;


        return new AppDbContext(options);
    }

    [Fact]
    public void CrearVehiculo_DeberiaRetornarOkConMensaje()
    {
        // Arrange
        var context = CrearDbContextSqlServer();
        context.Database.ExecuteSqlRaw("DELETE FROM Asignacion");
        context.Database.ExecuteSqlRaw("DELETE FROM Conductor"); // limpiar tabla
        context.Database.ExecuteSqlRaw("DELETE FROM Vehiculo"); // limpiar tabla

        var logger = new Mock<ILogger<VehiculoController>>();
        var controller = new VehiculoController(context, logger.Object);

        var dto = new VehiculoDto
        {
            Matricula = "TEST001",
            Marca = "Honda",
            Modelo = "Civic",
            Anio = 2022,
            Tipo = "Sedán"
        };

        // Act
        var result = controller.CrearVehiculo(dto);

        // Assert
        var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal("Vehiculo creado correctamente", objectResult.Value);
    }

    [Fact]
    public void CrearVehiculo_MatriculaDuplicada_DeberiaRetornarConflict()
    {
        // Arrange
        var context = CrearDbContextSqlServer();
        context.Database.ExecuteSqlRaw("DELETE FROM Asignacion");
        context.Database.ExecuteSqlRaw("DELETE FROM Conductor"); // limpiar tabla
        context.Database.ExecuteSqlRaw("DELETE FROM Vehiculo"); // limpiar tabla

        var logger = new Mock<ILogger<VehiculoController>>();
        var controller = new VehiculoController(context, logger.Object);

        // Crear primer vehículo usando el controlador
        controller.CrearVehiculo(new VehiculoDto
        {
            Matricula = "TEST002",
            Marca = "Toyota",
            Modelo = "Corolla",
            Anio = 2020,
            Tipo = "Sedán"
        });

        // Act: intentar crear duplicado
        var result = controller.CrearVehiculo(new VehiculoDto
        {
            Matricula = "TEST002",
            Marca = "Toyota",
            Modelo = "Corolla",
            Anio = 2020,
            Tipo = "Sedán"
        });

        // Assert
        var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
        Assert.Equal(409, objectResult.StatusCode);
        Assert.Equal("Ya existe un vehículo con esa matricula", objectResult.Value);

    }

    [Fact]
    public void ObtenerVehiculos_DeberiaRetornarLista()
    {
        // Arrange
        var context = CrearDbContextSqlServer();
        context.Database.ExecuteSqlRaw("DELETE FROM Asignacion");
        context.Database.ExecuteSqlRaw("DELETE FROM Conductor"); // limpiar tabla
        context.Database.ExecuteSqlRaw("DELETE FROM Vehiculo"); // limpiar tabla

        var logger = new Mock<ILogger<VehiculoController>>();
        var controller = new VehiculoController(context, logger.Object);

        // Crear vehículo usando el controlador
        controller.CrearVehiculo(new VehiculoDto
        {
            Matricula = "TEST003",
            Marca = "Nissan",
            Modelo = "Sentra",
            Anio = 2021,
            Tipo = "Sedán"
        });

        // Act
        var result = controller.ObtenerVehiculos();

        // Assert
        var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
        Assert.Equal(200, objectResult.StatusCode);

        var lista = Assert.IsAssignableFrom<IEnumerable<VehiculoDto>>(objectResult.Value);
        Assert.Single(lista);
    }
}