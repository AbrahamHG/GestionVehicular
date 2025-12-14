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
    public class VehiculoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VehiculoController> _logger;

        public VehiculoController(AppDbContext context, ILogger<VehiculoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // CREATE
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(VehiculoDto), typeof(VehiculoRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(VehiculoResponseExample))]

        public IActionResult CrearVehiculo([FromBody] VehiculoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos invalidos");

            var existeMatricula = _context.Vehiculos.Any(v => v.Matricula == dto.Matricula);
            if (existeMatricula)
                return Conflict("Ya existe un vehículo con esa matricula");



            var usuario = User?.Identity?.Name ?? "Sistema";
            var timestamp = DateTime.UtcNow;

            try
            {
                _context.Database.ExecuteSqlRaw(
            "EXEC spCrearVehiculo @Matricula, @Marca, @Modelo, @Anio, @Tipo",
            new SqlParameter("@Matricula", dto.Matricula),
            new SqlParameter("@Marca", dto.Marca),
            new SqlParameter("@Modelo", dto.Modelo),
            new SqlParameter("@Anio", dto.Anio),
            new SqlParameter("@Tipo", dto.Tipo)
        );
                
                _logger.LogInformation("AUDITORIA: Usuario={Usuario}, Accion=CrearVehiculo, Matricula={Matricula}, Fecha={Fecha}",
                    usuario, dto.Matricula, timestamp);

                return Ok("Vehiculo creado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: Usuario={Usuario}, Accion=CrearVehiculo, Matricula={Matricula}, Fecha={Fecha}",
                    usuario, dto.Matricula, timestamp);

                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }

        // READ ALL
        [HttpGet]
        public IActionResult ObtenerVehiculos()
        {
            try
            {
                var vehiculos = _context.Vehiculos
            .FromSqlRaw("EXEC spObtenerVehiculos")
            .AsEnumerable()
            .Select(v => new VehiculoDto
            {
                Id = v.Id,
                Matricula = v.Matricula,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Anio = v.Anio,
                Tipo = v.Tipo,
                Estado = v.Estado
            })
            .ToList();

                return Ok(vehiculos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: ObtenerVehiculos");
                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }

        // READ BY ID
        [HttpGet("{id}")]
        public IActionResult ObtenerVehiculoPorId(int id)
        {
            try
            {
            var vehiculo = _context.Vehiculos
           .FromSqlRaw("EXEC spObtenerVehiculoPorId @Id", new SqlParameter("@Id", id))
           .AsEnumerable()
           .Select(v => new VehiculoDto
           {
               Id = v.Id,
               Matricula = v.Matricula,
               Marca = v.Marca,
               Modelo = v.Modelo,
               Anio = v.Anio,
               Tipo = v.Tipo,
               Estado = v.Estado
           })
           .FirstOrDefault();


                if (vehiculo == null)
                    return NotFound("Vehiculo no encontrado");

                return Ok(vehiculo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: ObtenerVehiculoPorId Id={Id}", id);
                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }

        // READ disponibles
        [HttpGet("disponible")]
        public IActionResult ObtenerVehiculosDisponibles()
        {
            try
            {
                var disponibles = _context.Vehiculos
                    .Where(v => v.Estado == "Disponible")
                    .ToList();

                return Ok(disponibles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: ObtenerVehiculosDisponibles");
                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }

        // UPDATE
        [HttpPut("{id}")]
        public IActionResult ActualizarVehiculo(int id, [FromBody] VehiculoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos invalidos");

            var vehiculo = _context.Vehiculos.Find(id);
            if (vehiculo == null)
                return NotFound("Vehiculo no encontrado");

            var existeMatricula = _context.Vehiculos.Any(v => v.Matricula == dto.Matricula && v.Id != id);
            if (existeMatricula)
                return Conflict("Ya existe un vehiculo con esa matricula");

            try
            {

                _context.Database.ExecuteSqlRaw(
                            "EXEC spActualizarVehiculo @Id, @Marca, @Modelo, @Anio, @Tipo, @Estado",
                            new SqlParameter("@Id", id),
                            new SqlParameter("@Marca", dto.Marca),
                            new SqlParameter("@Modelo", dto.Modelo),
                            new SqlParameter("@Anio", dto.Anio),
                            new SqlParameter("@Tipo", dto.Tipo),
                            new SqlParameter("@Estado", dto.Estado)
                        );

                _logger.LogInformation("AUDITORIA: Usuario={Usuario}, Accion=ActualizarVehiculo, Id={Id}, Fecha={Fecha}",
                    User?.Identity?.Name ?? "Sistema", id, DateTime.UtcNow);

                return Ok("Vehiculo actualizado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: ActualizarVehiculo Id={Id}", id);
                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        public IActionResult EliminarVehiculo(int id)
        {
            try
            {
               var vehiculo = _context.Vehiculos
               .Include(v => v.Asignaciones)
                .FirstOrDefault(v => v.Id == id);

                if (vehiculo == null)
                    return NotFound("Vehiculo no encontrado");

                if (vehiculo.Asignaciones.Any())
                    return Conflict("No se puede eliminar el vehiculo porque esta asignado a un conductor");

                _context.Database.ExecuteSqlRaw(
                "EXEC spEliminarVehiculo @Id",
                 new SqlParameter("@Id", id)
                 );



                _logger.LogInformation("AUDITORIA: Usuario={Usuario}, Accion=EliminarVehiculo, Id={Id}, Fecha={Fecha}",
                    User?.Identity?.Name ?? "Sistema", id, DateTime.UtcNow);

                return Ok("Vehiculo eliminado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR BD: EliminarVehiculo Id={Id}", id);
                return StatusCode(500, "No se pudo completar la operacion, intente mas tarde");
            }
        }
    }
}