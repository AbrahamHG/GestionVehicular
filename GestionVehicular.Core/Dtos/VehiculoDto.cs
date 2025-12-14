using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionVehicular.Core.Dtos
{
    public class VehiculoDto
    {
        public int Id { get; set; }
        public string Matricula { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Anio { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
    }


}
