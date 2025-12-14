using GestionVehicular.Core;
using GestionVehicular.Core.Entities;
using System;
using Xunit;

public class AsignacionTests
{
    [Fact]
    public void Asignacion_InstanciaValida_DeberiaTenerDatosCorrectos()
    {
        var asignacion = new Asignacion
        {
            VehiculoId = 1,
            ConductorId = 2,
            FechaInicio = new DateTime(2025, 1, 10),
            FechaFin = new DateTime(2025, 1, 15)
        };

        Assert.Equal(1, asignacion.VehiculoId);
        Assert.Equal(2, asignacion.ConductorId);
        Assert.True(asignacion.FechaInicio < asignacion.FechaFin);
    }

    [Fact]
    public void Asignacion_FechaInicioMayorOIgual_FechaFin_DeberiaSerInvalida()
    {
        var asignacion = new Asignacion
        {
            VehiculoId = 1,
            ConductorId = 2,
            FechaInicio = new DateTime(2025, 1, 20),
            FechaFin = new DateTime(2025, 1, 15)
        };

        Assert.False(asignacion.FechaInicio < asignacion.FechaFin);
    }

    [Fact]
    public void Asignacion_SinVehiculoId_DeberiaTenerValorPorDefecto()
    {
        var asignacion = new Asignacion
        {
            ConductorId = 5,
            FechaInicio = DateTime.Today,
            FechaFin = DateTime.Today.AddDays(3)
        };

        Assert.Equal(0, asignacion.VehiculoId);
    }

    [Fact]
    public void Asignacion_SinConductorId_DeberiaTenerValorPorDefecto()
    {
        var asignacion = new Asignacion
        {
            VehiculoId = 10,
            FechaInicio = DateTime.Today,
            FechaFin = DateTime.Today.AddDays(3)
        };

        Assert.Equal(0, asignacion.ConductorId);
    }
}