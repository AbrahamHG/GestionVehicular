using Swashbuckle.AspNetCore.Filters;

namespace GestionVehicular.Api.SwaggerExamples
{
    public class ConductorResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                message = "Conductor creado correctamente"
            };
        }
    }
}