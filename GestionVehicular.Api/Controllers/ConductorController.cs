using GestionVehicular.Api.SwaggerExamples;
using GestionVehicular.Core;
using GestionVehicular.Core.Dtos;
using GestionVehicular.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;


namespace GestionVehicular.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConductorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ConductorController> _logger;

        public ConductorController(AppDbContext context, ILogger<ConductorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // CREATE
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(ConductorDto), typeof(ConductorRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ConductorResponseExample))]
        public IActionResult CrearConductor([FromBody] ConductorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            var existeLicencia = _context.Conductores.Any(c => c.NumeroLicencia == dto.NumeroLicencia);
            if (existeLicencia)
                return Conflict("Ya existe un conductor con ese numero de licencia");



            var usuario = User?.Identity?.Name ?? "Sistema";
            var timestamp = DateTime.UtcNow;

            try
            {
                _context.Database.ExecuteSqlRaw(
           "EXEC spCrearConductor @NombreCompleto, @NumeroLicencia, @Contacto",
           new SqlParameter("@NombreCompleto", dto.NombreCompleto),
           new SqlParameter("@NumeroLicencia", dto.NumeroLicencia),
           new SqlParameter("@Contacto", dto.Contacto)
       );
                _logger.LogInformation("AUDITORIA: Usuario={Usuario}, Accion=CrearConductor, Nombre={Nombre}, Fecha={Fecha}",
                    usuario, dto.NombreCompleto, timestamp);

                return Ok("Conductor creado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: Usuario={Usuario}, Accion=CrearConductor, Nombre={Nombre}, Fecha={Fecha}",
                    usuario, dto.NombreCompleto, timestamp);

                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }

        // READ ALL
        [HttpGet]
        public IActionResult ObtenerConductores()
        {
            try
            {
                var conductores = _context.Conductores
                   .FromSqlRaw("EXEC spObtenerConductores")
                   .AsEnumerable()
                   .Select(c => new ConductorDto
                   {
                       Id = c.Id,
                       NombreCompleto = c.NombreCompleto,
                       NumeroLicencia = c.NumeroLicencia,
                       Contacto = c.Contacto
                   })
                   .ToList();


                return Ok(conductores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: ObtenerConductores");
                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }

        // READ BY ID
        [HttpGet("{id}")]
        public IActionResult ObtenerConductorPorId(int id)
        {
            try
            {
                var conductor = _context.Conductores
            .FromSqlRaw("EXEC spObtenerConductorPorId @Id", new SqlParameter("@Id", id))
            .AsEnumerable()
            .Select(c => new ConductorDto
            {
                Id = c.Id,
                NombreCompleto = c.NombreCompleto,
                NumeroLicencia = c.NumeroLicencia,
                Contacto = c.Contacto
            })
            .FirstOrDefault();


                return conductor == null
                    ? NotFound("Conductor no encontrado")
                    : Ok(conductor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: ObtenerConductorPorId Id={Id}", id);
                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }

        // UPDATE
        [HttpPut("{id}")]
        public IActionResult ActualizarConductor(int id, [FromBody] ConductorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            var conductor = _context.Conductores.Find(id);
            if (conductor == null)
                return NotFound("Conductor no encontrado");

            var existeLicencia = _context.Conductores.Any(c => c.NumeroLicencia == dto.NumeroLicencia && c.Id != id);
            if (existeLicencia)
                return Conflict("Ya existe un conductor con ese numero de licencia");



            try
            {

                _context.Database.ExecuteSqlRaw(
                     "EXEC spActualizarConductor @Id, @NombreCompleto, @NumeroLicencia, @Contacto",
                     new SqlParameter("@Id", id),
                     new SqlParameter("@NombreCompleto", dto.NombreCompleto),
                     new SqlParameter("@NumeroLicencia", dto.NumeroLicencia),
                     new SqlParameter("@Contacto", dto.Contacto)
                 );

                _logger.LogInformation("AUDITORIA: Usuario={Usuario}, Accion=ActualizarConductor, Id={Id}, Fecha={Fecha}",
                    User?.Identity?.Name ?? "Sistema", id, DateTime.UtcNow);

                return Ok("Conductor actualizado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: ActualizarConductor Id={Id}", id);
                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        public IActionResult EliminarConductor(int id)
        {
            try
            {

                var conductor = _context.Conductores
                    .Include(c => c.Asignaciones)
                    .FirstOrDefault(c => c.Id == id);

                if (conductor == null)
                    return NotFound("Conductor no encontrado");

                if(conductor.Asignaciones.Any())
                    return Conflict("No se puede eliminar el Conductor porque esta asignado a un vehiculo");


                _context.Database.ExecuteSqlRaw(
                    "EXEC spEliminarConductor @Id",
                    new SqlParameter("@Id", id)
                );

                _logger.LogInformation("AUDITORIA: Usuario={Usuario}, Accion=EliminarConductor, Id={Id}, Fecha={Fecha}",
                    User?.Identity?.Name ?? "Sistema", id, DateTime.UtcNow);

                return Ok("Conductor eliminado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: EliminarConductor Id={Id}", id);
                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }
    }
}