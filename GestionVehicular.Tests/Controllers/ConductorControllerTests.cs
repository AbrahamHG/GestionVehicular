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

public class ConductorControllerTests
{
    private AppDbContext CrearDbContextSqlServer()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=VehiculosTestDb;Trusted_Connection=True;MultipleActiveResultSets=true;Connection Timeout=60;")
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void CrearConductor_DeberiaRetornarOkConMensaje()
    {
        var context = CrearDbContextSqlServer();
        context.Database.ExecuteSqlRaw("DELETE FROM Conductor");

        var logger = new Mock<ILogger<ConductorController>>();
        var controller = new ConductorController(context, logger.Object);

        var dto = new ConductorDto
        {
            NombreCompleto = "Juan Pérez",
            NumeroLicencia = "LIC001",
            Contacto = "555-1234"
        };

        var result = controller.CrearConductor(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Conductor creado correctamente", okResult.Value);
    }

    [Fact]
    public void CrearConductor_NumeroLicenciaDuplicada_DeberiaRetornarConflict()
    {
        var context = CrearDbContextSqlServer();
        context.Database.ExecuteSqlRaw("DELETE FROM Conductor");

        var logger = new Mock<ILogger<ConductorController>>();
        var controller = new ConductorController(context, logger.Object);

        controller.CrearConductor(new ConductorDto
        {
            NombreCompleto = "Pedro López",
            NumeroLicencia = "LIC002",
            Contacto = "555-5678"
        });

        var result = controller.CrearConductor(new ConductorDto
        {
            NombreCompleto = "Pedro López",
            NumeroLicencia = "LIC002",
            Contacto = "555-5678"
        });

        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Contains("Ya existe un conductor con ese numero de licencia", conflictResult.Value.ToString());
    }

    [Fact]
    public void ObtenerConductores_DeberiaRetornarLista()
    {
        var context = CrearDbContextSqlServer();
        context.Database.ExecuteSqlRaw("DELETE FROM Conductor");

        var logger = new Mock<ILogger<ConductorController>>();
        var controller = new ConductorController(context, logger.Object);

        controller.CrearConductor(new ConductorDto
        {
            NombreCompleto = "María García",
            NumeroLicencia = "LIC003",
            Contacto = "555-9876"
        });

        var result = controller.ObtenerConductores();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var lista = Assert.IsAssignableFrom<IEnumerable<ConductorDto>>(okResult.Value);

        Assert.Single(lista);
        Assert.Equal("LIC003", lista.First().NumeroLicencia);
    }
}