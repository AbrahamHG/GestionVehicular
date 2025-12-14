using Swashbuckle.AspNetCore.Filters;

namespace GestionVehicular.Api.SwaggerExamples
{
    public class AsignacionResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                message = "Asignacion creada correctamente"
            };
        }
    }
}