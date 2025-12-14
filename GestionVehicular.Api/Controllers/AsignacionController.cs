using GestionVehicular.Api.SwaggerExamples;
using GestionVehicular.Core;
using GestionVehicular.Core.Dtos;
using GestionVehicular.Infrastructure;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;

namespace GestionVehicular.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsignacionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AsignacionController> _logger;

        public AsignacionController(AppDbContext context, ILogger<AsignacionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // CREATE
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(AsignacionDto), typeof(AsignacionRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AsignacionResponseExample))]
        public IActionResult CrearAsignacion([FromBody] AsignacionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            // Validar que el vehículo exista y esté disponible
            var vehiculo = _context.Vehiculos.Find(dto.VehiculoId);
            if (vehiculo == null)
                return NotFound("Vehículo no encontrado");

            if (vehiculo.Estado != "Disponible")
                return Conflict("El vehículo no está disponible para asignación");

            // Validar que el conductor exista
            var conductor = _context.Conductores.Find(dto.ConductorId);
            if (conductor == null)
                return NotFound("Conductor no encontrado");

            // Validar que las fechas sean correctas
            if (dto.FechaInicio >= dto.FechaFin)
                return Conflict("La fecha de inicio debe ser menor que la fecha de fin");


            var existeAsignacion = _context.Asignaciones.Any(a =>
                a.VehiculoId == dto.VehiculoId &&
                a.ConductorId == dto.ConductorId &&
                a.FechaInicio == dto.FechaInicio &&
                a.FechaFin == dto.FechaFin);

            if (existeAsignacion)
                return Conflict("Ya existe una asignacion con esos datos");

            // Validar que el conductor no tenga otra asignación en ese rango
            var conductorAsignado = _context.Asignaciones.Any(a =>
                a.ConductorId == dto.ConductorId &&
                a.FechaFin >= dto.FechaInicio &&
                a.FechaInicio <= dto.FechaFin);

            if (conductorAsignado)
                return Conflict("El conductor ya tiene una asignación activa en ese rango de fechas");

            var usuario = User?.Identity?.Name ?? "Sistema";
            var timestamp = DateTime.UtcNow;

            try
            {
                _context.Database.ExecuteSqlRaw(
                    "EXEC spCrearAsignacion @VehiculoId, @ConductorId, @FechaInicio, @FechaFin",
                    new SqlParameter("@VehiculoId", dto.VehiculoId),
                    new SqlParameter("@ConductorId", dto.ConductorId),
                    new SqlParameter("@FechaInicio", dto.FechaInicio),
                    new SqlParameter("@FechaFin", dto.FechaFin)
                );

                _logger.LogInformation("AUDITORIA: Usuario={Usuario}, Accion=CrearAsignacion, VehiculoId={VehiculoId}, ConductorId={ConductorId}, Fecha={Fecha}",
                    usuario, dto.VehiculoId, dto.ConductorId, timestamp);

                return Ok("Asignacion creada correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: Usuario={Usuario}, Accion=CrearAsignacion, VehiculoId={VehiculoId}, ConductorId={ConductorId}, Fecha={Fecha}",
                    usuario, dto.VehiculoId, dto.ConductorId, timestamp);

                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }

        // READ ALL
        [HttpGet]
        public IActionResult ObtenerAsignaciones()
        {
            try
            {

                var asignaciones = _context.AsignacionesView
                .FromSqlRaw("EXEC spObtenerAsignaciones")
                .ToList();

                return Ok(asignaciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: ObtenerAsignaciones");
                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }

        // READ BY ID
        [HttpGet("{id}")]
        public IActionResult ObtenerAsignacionPorId(int id)
        {
            try
            {

                var asignacion = _context.AsignacionesView
                .FromSqlRaw("EXEC spObtenerAsignacionPorId @Id", new SqlParameter("@Id", id))
                .AsEnumerable()
                .Select(a => new AsignacionDto
                {
                    Id = a.Id,
                    VehiculoId = a.VehiculoId,
                    ConductorId = a.ConductorId,
                    FechaInicio = a.FechaInicio,
                    FechaFin = a.FechaFin,
                    VehiculoMatricula = a.VehiculoMatricula,
                    ConductorNombre = a.ConductorNombre
                })
                .FirstOrDefault();


                return asignacion == null
                    ? NotFound("Asignacion no encontrada")
                    : Ok(asignacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: ObtenerAsignacionPorId Id={Id}", id);
                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }

        // UPDATE
        [HttpPut("{id}")]
        public IActionResult ActualizarAsignacion(int id, [FromBody] AsignacionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            var asignacion = _context.Asignaciones.Find(id);
            if (asignacion == null)
                return NotFound("Asignacion no encontrada");

            var vehiculo = _context.Vehiculos.Find(dto.VehiculoId);
            if (vehiculo == null)
                return NotFound("Vehículo no encontrado");

           if (vehiculo.Estado != "Disponible" && vehiculo.Id != asignacion.VehiculoId)
                return Conflict("El vehículo no está disponible para asignación");

            var conductor = _context.Conductores.Find(dto.ConductorId);
            if (conductor == null)
                return NotFound("Conductor no encontrado");

            // Validar que el conductor no tenga otra asignación activa en el rango
            var conductorAsignado = _context.Asignaciones.Any(a =>
                a.ConductorId == dto.ConductorId &&
                a.Id != id && // excluir la asignación actual
                a.FechaFin >= dto.FechaInicio &&
                a.FechaInicio <= dto.FechaFin);

            if (conductorAsignado)
                return Conflict("El conductor ya tiene una asignación activa en ese rango de fechas");



            try
            {

                _context.Database.ExecuteSqlRaw(
                    "EXEC spActualizarAsignacion @Id, @VehiculoId, @ConductorId, @FechaInicio, @FechaFin",
                    new SqlParameter("@Id", id),
                    new SqlParameter("@VehiculoId", dto.VehiculoId),
                    new SqlParameter("@ConductorId", dto.ConductorId),
                    new SqlParameter("@FechaInicio", dto.FechaInicio),
                    new SqlParameter("@FechaFin", dto.FechaFin)
                );

                _logger.LogInformation("AUDITORIA: Usuario={Usuario}, Accion=ActualizarAsignacion, Id={Id}, Fecha={Fecha}",
                    User?.Identity?.Name ?? "Sistema", id, DateTime.UtcNow);

                return Ok("Asignacion actualizada correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: ActualizarAsignacion Id={Id}", id);
                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        public IActionResult EliminarAsignacion(int id)
        {
            try
            {
                var asignacion = _context.Asignaciones.Find(id);
                if (asignacion == null)
                    return NotFound("Asignacion no encontrada");


                _context.Database.ExecuteSqlRaw(
                    "EXEC spEliminarAsignacion @Id",
                    new SqlParameter("@Id", id)
                );

                // Liberar vehículo se quito
                if (asignacion.Vehiculo != null)
                {
                    asignacion.Vehiculo.Estado = "Disponible";
                    _context.Vehiculos.Update(asignacion.Vehiculo);
                }

                _context.SaveChanges();

                _logger.LogInformation("AUDITORIA: Usuario={Usuario}, Accion=EliminarAsignacion, Id={Id}, Fecha={Fecha}",
                    User?.Identity?.Name ?? "Sistema", id, DateTime.UtcNow);

                return Ok("Asignacion eliminada correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: EliminarAsignacion Id={Id}", id);
                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }
    }
}