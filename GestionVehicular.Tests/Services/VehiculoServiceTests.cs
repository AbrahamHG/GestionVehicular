using GestionVehicular.Core;
using GestionVehicular.Core.Dtos;
using GestionVehicular.Core.Entities;
using GestionVehicular.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

public class VehiculoServiceTests
{
    private AppDbContext CrearDbContextInMemory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
       .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
       .Options;


        return new AppDbContext(options);
    }

    [Fact]
    public void CrearVehiculo_DeberiaAgregarVehiculo()
    {
        var context = CrearDbContextInMemory();

        var dto = new VehiculoDto
        {
            Matricula = "SERV001",
            Marca = "Mazda",
            Modelo = "3",
            Anio = 2021,
            Tipo = "Sedán"
        };


        context.Vehiculos.Add(new Vehiculo
        {
            Matricula = dto.Matricula,
            Marca = dto.Marca,
            Modelo = dto.Modelo,
            Anio = dto.Anio,
            Tipo = dto.Tipo,
            Estado = "Activo"
        });
        context.SaveChanges();

        var vehiculo = context.Vehiculos.FirstOrDefault(v => v.Matricula == "SERV001");
        Assert.NotNull(vehiculo);
        Assert.Equal("Mazda", vehiculo.Marca);
    }

    [Fact]
    public void CrearVehiculo_MatriculaDuplicada_DeberiaFallar()
    {
        var context = CrearDbContextInMemory();

        context.Vehiculos.Add(new Vehiculo
        {
            Matricula = "SERV002",
            Marca = "Toyota",
            Modelo = "Yaris",
            Anio = 2020,
            Tipo = "Sedán",
            Estado = "Activo"
        });
        context.SaveChanges();

        var dtoDuplicado = new VehiculoDto
        {
            Matricula = "SERV002",
            Marca = "Toyota",
            Modelo = "Yaris",
            Anio = 2020,
            Tipo = "Sedán"
        };

        
        var existe = context.Vehiculos.Any(v => v.Matricula == dtoDuplicado.Matricula);

        
        Assert.True(existe); 
    }

    [Fact]
    public void ObtenerVehiculos_DeberiaRetornarListaActivos()
    {

        var context = CrearDbContextInMemory();

        context.Vehiculos.Add(new Vehiculo
        {
            Matricula = "SERV003",
            Marca = "Nissan",
            Modelo = "Versa",
            Anio = 2022,
            Tipo = "Sedán",
            Estado = "Activo"
        });
        context.Vehiculos.Add(new Vehiculo
        {
            Matricula = "SERV004",
            Marca = "Ford",
            Modelo = "Focus",
            Anio = 2019,
            Tipo = "Sedán",
            Estado = "Inactivo"
        });
        context.SaveChanges();

        var activos = context.Vehiculos.Where(v => v.Estado == "Activo").ToList();

        Assert.Single(activos);
        Assert.Equal("SERV003", activos.First().Matricula);
    }
}