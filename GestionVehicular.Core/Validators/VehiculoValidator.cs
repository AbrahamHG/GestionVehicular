using GestionVehicular.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace GestionVehicular.Core.Validators
{
    public class VehiculoValidator : AbstractValidator<VehiculoDto>
    {
        public VehiculoValidator()
        {
            RuleFor(v => v.Matricula)
            .NotEmpty().WithMessage("La matrícula es obligatoria")
            .MaximumLength(10);

            RuleFor(v => v.Marca)
                .NotEmpty().WithMessage("La marca es obligatoria");

            RuleFor(v => v.Modelo)
                .NotEmpty().WithMessage("El modelo es obligatorio");

            RuleFor(v => v.Anio)
                .InclusiveBetween(1900, DateTime.Now.Year + 1)
                .WithMessage("El año debe ser válido");

            RuleFor(v => v.Tipo)
                .NotEmpty().WithMessage("El tipo es obligatorio");

            RuleFor(v => v.Estado)
                .NotEmpty().WithMessage("El estado es obligatorio");
        }


    }

}
