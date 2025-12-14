using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionVehicular.Core
{
    public class Vehiculo
    {
        public int Id { get; set; }
        public string Matricula { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public ICollection<Asignacion> Asignaciones { get; set; }


    }

}
