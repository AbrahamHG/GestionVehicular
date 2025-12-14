using GestionVehicular.Core;
using GestionVehicular.Core.Entities;
using Xunit;

public class VehiculoTests
{
    [Fact]
    public void Vehiculo_DeberiaInicializarCorrectamente()
    {
        var vehiculo = new Vehiculo
        {
            Matricula = "ABC123",
            Marca = "Toyota",
            Modelo = "Corolla",
            Anio = 2020,
            Tipo = "Sedán",
            Estado = "Activo"
        };

        Assert.Equal("Toyota", vehiculo.Marca);
        Assert.Equal(2020, vehiculo.Anio);
    }
}