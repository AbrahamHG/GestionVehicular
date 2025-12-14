using GestionVehicular.Core.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace GestionVehicular.Api.SwaggerExamples
{
    public class VehiculoRequestExample : IExamplesProvider<VehiculoDto>
    {
        public VehiculoDto GetExamples()
        {
            return new VehiculoDto
            {
                Matricula = "ABC123",
                Marca = "Toyota",
                Modelo = "Corolla",
                Anio = 2022,
                Tipo = "Sedan",
                Estado = "Activo"
            };
        }
    }
}