using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionVehicular.Core
{
    public class Asignacion
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public int ConductorId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        // Relaciones de navegación
        public Vehiculo Vehiculo { get; set; } = default!;
        public Conductor Conductor { get; set; } = default!;
    }

}
