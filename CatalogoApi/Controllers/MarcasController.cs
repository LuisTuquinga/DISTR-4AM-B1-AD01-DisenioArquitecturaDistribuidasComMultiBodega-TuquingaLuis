using CatalogoApi.Data;
using CatalogoApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatalogoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarcasController : ControllerBase
    {
        private readonly CatalogoDBContext _context;

        public MarcasController(CatalogoDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Marca>>> GetMarcas()
        {
            return Ok(await _context.Marcas.AsNoTracking().ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Marca>> GetMarca(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var marca = await _context.Marcas
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdMarca == id);

            if (marca == null)
                return NotFound(new { mensaje = $"La marca con ID {id} no existe." });

            return Ok(marca);
        }

        [HttpPost]
        public async Task<ActionResult<Marca>> CrearMarca(Marca marca)
        {
            if (marca == null)
                return BadRequest(new { mensaje = "Los datos de la marca son obligatorios." });

            if (string.IsNullOrWhiteSpace(marca.Nombre))
                return BadRequest(new { mensaje = "El nombre de la marca es obligatorio." });

            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMarca), new { id = marca.IdMarca }, marca);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarMarca(int id, Marca marca)
        {
            if (id <= 0 || id != marca.IdMarca)
                return BadRequest(new { mensaje = "El ID de la ruta no coincide con el de la entidad." });

            if (string.IsNullOrWhiteSpace(marca.Nombre))
                return BadRequest(new { mensaje = "El nombre de la marca es obligatorio." });

            var existe = await _context.Marcas.AnyAsync(m => m.IdMarca == id);
            if (!existe)
                return NotFound(new { mensaje = $"La marca con ID {id} no existe." });

            _context.Entry(marca).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BorrarMarca(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null)
                return NotFound(new { mensaje = $"La marca con ID {id} no existe." });

            _context.Marcas.Remove(marca);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
