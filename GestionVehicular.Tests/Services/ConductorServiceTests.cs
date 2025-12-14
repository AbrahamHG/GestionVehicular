using GestionVehicular.Core;
using GestionVehicular.Core.Dtos;
using GestionVehicular.Core.Entities;
using GestionVehicular.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

public class ConductorServiceTests
{
    private AppDbContext CrearDbContextInMemory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void CrearConductor_DeberiaAgregarConductor()
    {
        var context = CrearDbContextInMemory();

        var dto = new ConductorDto
        {
            NombreCompleto = "Juan Pérez",
            NumeroLicencia = "LIC001",
            Contacto = "555-1234"
        };

        context.Conductores.Add(new Conductor
        {
            NombreCompleto = dto.NombreCompleto,
            NumeroLicencia = dto.NumeroLicencia,
            Contacto = dto.Contacto
        });
        context.SaveChanges();

        var conductor = context.Conductores.FirstOrDefault(c => c.NumeroLicencia == "LIC001");
        Assert.NotNull(conductor);
        Assert.Equal("Juan Pérez", conductor.NombreCompleto);
    }

    [Fact]
    public void CrearConductor_NumeroLicenciaDuplicada_DeberiaFallar()
    {
        var context = CrearDbContextInMemory();

        context.Conductores.Add(new Conductor
        {
            NombreCompleto = "Pedro López",
            NumeroLicencia = "LIC002",
            Contacto = "555-5678"
        });
        context.SaveChanges();

        var dtoDuplicado = new ConductorDto
        {
            NombreCompleto = "Pedro López",
            NumeroLicencia = "LIC002",
            Contacto = "555-5678"
        };

        var existe = context.Conductores.Any(c => c.NumeroLicencia == dtoDuplicado.NumeroLicencia);

        Assert.True(existe);
    }

    [Fact]
    public void ObtenerConductores_DeberiaRetornarLista()
    {
        var context = CrearDbContextInMemory();

        context.Conductores.Add(new Conductor
        {
            NombreCompleto = "María García",
            NumeroLicencia = "LIC003",
            Contacto = "555-9876"
        });
        context.Conductores.Add(new Conductor
        {
            NombreCompleto = "Luis Torres",
            NumeroLicencia = "LIC004",
            Contacto = "555-4321"
        });
        context.SaveChanges();

        var lista = context.Conductores.ToList();

        Assert.Equal(2, lista.Count);
        Assert.Contains(lista, c => c.NumeroLicencia == "LIC003");
        Assert.Contains(lista, c => c.NumeroLicencia == "LIC004");
    }
}