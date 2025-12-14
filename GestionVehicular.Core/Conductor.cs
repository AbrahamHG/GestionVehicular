using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionVehicular.Core
{
    public class Conductor
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string NumeroLicencia { get; set; } = string.Empty;
        public string? Contacto { get; set; }

        // Relación con asignaciones
        public ICollection<Asignacion> Asignaciones { get; set; } = new List<Asignacion>();
    }

}
