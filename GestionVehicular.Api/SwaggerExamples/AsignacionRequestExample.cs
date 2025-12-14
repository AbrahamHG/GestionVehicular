using GestionVehicular.Core.Dtos;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace GestionVehicular.Api.SwaggerExamples
{
    public class AsignacionRequestExample : IExamplesProvider<AsignacionDto>
    {
        public AsignacionDto GetExamples()
        {
            return new AsignacionDto
            {
                VehiculoId = 1,
                ConductorId = 2,
                FechaInicio = new DateTime(2025, 1, 10),
                FechaFin = new DateTime(2025, 1, 20)
            };
        }
    }
}