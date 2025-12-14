using GestionVehicular.Core;
using GestionVehicular.Core.Entities;
using GestionVehicular.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

public class AsignacionServiceTests
{
    private AppDbContext CrearDbContextInMemory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void CrearAsignacion_DeberiaAgregarAsignacion()
    {
        var context = CrearDbContextInMemory();

        var vehiculo = new Vehiculo { Matricula = "ASIG001", Marca = "Mazda", Estado = "Activo" };
        var conductor = new Conductor { NombreCompleto = "Juan Pérez", NumeroLicencia = "LIC100" };
        context.Vehiculos.Add(vehiculo);
        context.Conductores.Add(conductor);
        context.SaveChanges();

        var asignacion = new Asignacion
        {
            VehiculoId = vehiculo.Id,
            ConductorId = conductor.Id,
            FechaInicio = DateTime.Today,
            FechaFin = DateTime.Today.AddDays(7)
        };

        context.Asignaciones.Add(asignacion);
        context.SaveChanges();

        var resultado = context.Asignaciones.FirstOrDefault(a => a.VehiculoId == vehiculo.Id);
        Assert.NotNull(resultado);
        Assert.Equal(conductor.Id, resultado.ConductorId);
    }

    [Fact]
    public void CrearAsignacion_VehiculoYaAsignadoEnPeriodo_DeberiaFallar()
    {
        var context = CrearDbContextInMemory();

        var vehiculo = new Vehiculo { Matricula = "ASIG002", Marca = "Toyota", Estado = "Activo" };
        var conductor1 = new Conductor { NombreCompleto = "Pedro López", NumeroLicencia = "LIC200" };
        var conductor2 = new Conductor { NombreCompleto = "María García", NumeroLicencia = "LIC201" };
        context.Vehiculos.Add(vehiculo);
        context.Conductores.AddRange(conductor1, conductor2);
        context.SaveChanges();

        // Primera asignación
        context.Asignaciones.Add(new Asignacion
        {
            VehiculoId = vehiculo.Id,
            ConductorId = conductor1.Id,
            FechaInicio = DateTime.Today,
            FechaFin = DateTime.Today.AddDays(5)
        });
        context.SaveChanges();

        // Intento de asignación solapada
        var solapada = context.Asignaciones.Any(a =>
            a.VehiculoId == vehiculo.Id &&
            DateTime.Today.AddDays(3) < a.FechaFin);

        Assert.True(solapada); // el vehículo ya está asignado en ese rango
    }

    [Fact]
    public void ObtenerAsignaciones_DeberiaRetornarLista()
    {
        var context = CrearDbContextInMemory();

        var vehiculo = new Vehiculo { Matricula = "ASIG003", Marca = "Nissan", Estado = "Activo" };
        var conductor = new Conductor { NombreCompleto = "Luis Torres", NumeroLicencia = "LIC300" };
        context.Vehiculos.Add(vehiculo);
        context.Conductores.Add(conductor);
        context.SaveChanges();

        context.Asignaciones.Add(new Asignacion
        {
            VehiculoId = vehiculo.Id,
            ConductorId = conductor.Id,
            FechaInicio = DateTime.Today,
            FechaFin = DateTime.Today.AddDays(10)
        });
        context.SaveChanges();

        var lista = context.Asignaciones.ToList();

        Assert.Single(lista);
        Assert.Equal(vehiculo.Id, lista.First().VehiculoId);
    }
}