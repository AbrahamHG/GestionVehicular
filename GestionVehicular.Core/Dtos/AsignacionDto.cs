using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionVehicular.Core.Dtos
{
    public class AsignacionDto
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public int ConductorId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        // Estas propiedades deben existir si las usas en el frontend
        public string ConductorNombre { get; set; } = string.Empty;
        public string VehiculoMatricula { get; set; } = string.Empty;


    }

}
