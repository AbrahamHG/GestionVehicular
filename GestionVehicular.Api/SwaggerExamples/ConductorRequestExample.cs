using GestionVehicular.Core.Dtos;
using Swashbuckle.AspNetCore.Filters;


namespace GestionVehicular.Api.SwaggerExamples
{
    public class ConductorRequestExample : IExamplesProvider<ConductorDto>
    {
        public ConductorDto GetExamples()
        {
            return new ConductorDto
            {
                NombreCompleto = "Abraham Hernandez Gonzalez",
                NumeroLicencia = "LIC123456",
                Contacto = "abraham@gmail.com"
            };
        }
    }
}