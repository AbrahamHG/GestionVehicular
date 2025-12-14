using FluentValidation;
using GestionVehicular.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionVehicular.Core.Validators
{
    public class ConductorValidator : AbstractValidator<ConductorDto>
    {
        public ConductorValidator()
        {
            RuleFor(c => c.NombreCompleto)
            .NotEmpty().WithMessage("El Nombre es obligatorio")
                .MaximumLength(100);

            RuleFor(c => c.NumeroLicencia)
                .NotEmpty().WithMessage("La licencia es obligatorio")
                .MaximumLength(30);

            RuleFor(c => c.Contacto)
                .NotEmpty().WithMessage("El Contacto es obligatorio")
                .MaximumLength(100);
        }


    }

}
