using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionVehicular.Core.Dtos
{
    public class ConductorDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string NumeroLicencia { get; set; }
        public string? Contacto { get; set; }

    }

}
