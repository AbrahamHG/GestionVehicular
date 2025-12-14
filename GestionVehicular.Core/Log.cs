using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionVehicular.Core
{
    public class Log
    {
        public int Id { get; set; }
        public string Accion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public int? UsuarioId { get; set; }
        public string? RealizadoPor { get; set; }  
        public string? Cambios { get; set; }
    }

}
