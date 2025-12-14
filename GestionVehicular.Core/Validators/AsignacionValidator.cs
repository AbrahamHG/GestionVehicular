using FluentValidation;
using GestionVehicular.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionVehicular.Core.Validators
{
    public class AsignacionValidator : AbstractValidator<AsignacionDto>
    {
        public AsignacionValidator()
        {
            RuleFor(a => a.VehiculoId).GreaterThan(0);
            RuleFor(a => a.ConductorId).GreaterThan(0);

            RuleFor(a => a.FechaInicio)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("La fecha de inicio no puede ser futura.");

            RuleFor(a => a.FechaFin)
                .GreaterThan(a => a.FechaInicio)
                .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.");
        }

    }

}
