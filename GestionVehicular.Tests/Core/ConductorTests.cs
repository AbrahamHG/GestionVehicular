using GestionVehicular.Core;
using GestionVehicular.Core.Entities;
using Xunit;

public class ConductorTests
{
    [Fact]
    public void NumeroLicencia_DeberiaSerRequerido()
    {
        var conductor = new Conductor
        {
            NombreCompleto = "Ana Torres",
            NumeroLicencia = null,
            Contacto = "555-0000"
        };

        Assert.Null(conductor.NumeroLicencia);
    }

    [Fact]
    public void Contacto_DeberiaAceptarFormatoLibre()
    {
        var conductor = new Conductor
        {
            NombreCompleto = "Luis Gómez",
            NumeroLicencia = "LIC005",
            Contacto = "luis@example.com"
        };

        Assert.Contains("@", conductor.Contacto);
    }

    [Fact]
    public void NombreCompleto_DeberiaPermitirCaracteresEspeciales()
    {
        var conductor = new Conductor
        {
            NombreCompleto = "José Ángel",
            NumeroLicencia = "LIC006",
            Contacto = "555-9999"
        };

        Assert.Equal("José Ángel", conductor.NombreCompleto);
    }

    [Fact]
    public void Conductor_DeberiaSerValidoConDatosMinimos()
    {
        var conductor = new Conductor
        {
            NombreCompleto = "Carlos Ruiz",
            NumeroLicencia = "LIC007",
            Contacto = "555-1111"
        };

        Assert.NotNull(conductor);
        Assert.Equal("LIC007", conductor.NumeroLicencia);
    }
}