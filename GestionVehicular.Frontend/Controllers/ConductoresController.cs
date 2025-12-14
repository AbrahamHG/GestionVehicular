using GestionVehicular.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace GestionVehicular.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConductoresController : ControllerBase
    {
        // Datos simulados por ahora
        private static readonly List<ConductorDto> _conductores = new()
        {
            new ConductorDto { NombreCompleto = "Juan Pérez", NumeroLicencia = "ABC123", Contacto = "juan@mail.com" },
            new ConductorDto { NombreCompleto = "María López", NumeroLicencia = "XYZ789", Contacto = "maria@mail.com" }
        };

        // GET api/conductores
        [HttpGet]
        public ActionResult<IEnumerable<ConductorDto>> Get()
        {
            return Ok(_conductores);
        }

        // GET api/conductores/1
        [HttpGet("{index}")]
        public ActionResult<ConductorDto> GetByIndex(int index)
        {
            if (index < 0 || index >= _conductores.Count) return NotFound();
            return Ok(_conductores[index]);
        }

        // POST api/conductores
        [HttpPost]
        public ActionResult<ConductorDto> Create(ConductorDto nuevo)
        {
            _conductores.Add(nuevo);
            // devolvemos el índice como "id" simulado
            var index = _conductores.Count - 1;
            return CreatedAtAction(nameof(GetByIndex), new { index }, nuevo);
        }

        // PUT api/conductores/1
        [HttpPut("{index}")]
        public IActionResult Update(int index, ConductorDto actualizado)
        {
            if (index < 0 || index >= _conductores.Count) return NotFound();

            _conductores[index].NombreCompleto = actualizado.NombreCompleto;
            _conductores[index].NumeroLicencia = actualizado.NumeroLicencia;
            _conductores[index].Contacto = actualizado.Contacto;
            return NoContent();
        }

        // DELETE api/conductores/1
        [HttpDelete("{index}")]
        public IActionResult Delete(int index)
        {
            if (index < 0 || index >= _conductores.Count) return NotFound();

            _conductores.RemoveAt(index);
            return NoContent();
        }
    }
}