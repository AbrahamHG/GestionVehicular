using System;

namespace GestionVehicular.Core.Entities
{
    public class AsignacionView
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public string VehiculoMatricula { get; set; } = string.Empty;
        public int ConductorId { get; set; }
        public string ConductorNombre { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}