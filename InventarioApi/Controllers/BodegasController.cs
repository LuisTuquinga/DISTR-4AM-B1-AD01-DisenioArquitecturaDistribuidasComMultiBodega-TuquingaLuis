using InventarioApi.Data;
using InventarioApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventarioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BodegasController : ControllerBase
    {
        private readonly InventarioDBContext _context;

        public BodegasController(InventarioDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bodega>>> GetBodegas()
        {
            return Ok(await _context.Bodegas.AsNoTracking().ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Bodega>> GetBodega(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var bodega = await _context.Bodegas
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.IdBodega == id);

            if (bodega == null)
                return NotFound(new { mensaje = $"La bodega con ID {id} no existe." });

            return Ok(bodega);
        }

        [HttpPost]
        public async Task<ActionResult<Bodega>> CrearBodega(Bodega bodega)
        {
            if (bodega == null)
                return BadRequest(new { mensaje = "Los datos de la bodega son obligatorios." });

            if (string.IsNullOrWhiteSpace(bodega.Nombre))
                return BadRequest(new { mensaje = "El nombre de la bodega es obligatorio." });

            _context.Bodegas.Add(bodega);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBodega), new { id = bodega.IdBodega }, bodega);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarBodega(int id, Bodega bodega)
        {
            if (id <= 0 || id != bodega.IdBodega)
                return BadRequest(new { mensaje = "El ID de la ruta no coincide con el de la entidad." });

            if (string.IsNullOrWhiteSpace(bodega.Nombre))
                return BadRequest(new { mensaje = "El nombre de la bodega es obligatorio." });

            var existe = await _context.Bodegas.AnyAsync(b => b.IdBodega == id);
            if (!existe)
                return NotFound(new { mensaje = $"La bodega con ID {id} no existe." });

            _context.Entry(bodega).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BorrarBodega(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var bodega = await _context.Bodegas.FindAsync(id);
            if (bodega == null)
                return NotFound(new { mensaje = $"La bodega con ID {id} no existe." });

            _context.Bodegas.Remove(bodega);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
