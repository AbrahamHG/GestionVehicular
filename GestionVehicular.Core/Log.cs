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
        public int? UsuarioId { get; set; }   // opcional si manejas usuarios
        public string? RealizadoPor { get; set; }  // IP o usuario técnico
        public string? Cambios { get; set; }
    }

}
