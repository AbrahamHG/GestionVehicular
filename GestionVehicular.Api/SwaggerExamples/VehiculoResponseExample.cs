using Swashbuckle.AspNetCore.Filters;

namespace GestionVehicular.Api.SwaggerExamples
{
    public class VehiculoResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                message = "Vehiculo creado correctamente"
            };
        }
    }
}