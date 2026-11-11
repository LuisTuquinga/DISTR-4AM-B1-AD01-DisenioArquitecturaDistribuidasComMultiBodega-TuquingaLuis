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
    public class MovimientosInventarioController : ControllerBase
    {
        private readonly InventarioDBContext _context;

        public MovimientosInventarioController(InventarioDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimientoInventario>>> GetMovimientos()
        {
            return Ok(await _context.MovimientosInventario.AsNoTracking().ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MovimientoInventario>> GetMovimiento(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var movimiento = await _context.MovimientosInventario
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdMovimiento == id);

            if (movimiento == null)
                return NotFound(new { mensaje = $"El movimiento con ID {id} no existe." });

            return Ok(movimiento);
        }

        [HttpPost]
        public async Task<ActionResult<MovimientoInventario>> CrearMovimiento(MovimientoInventario movimiento)
        {
            if (movimiento == null)
                return BadRequest(new { mensaje = "Los datos del movimiento son obligatorios." });

            if (movimiento.Cantidad <= 0)
                return BadRequest(new { mensaje = "La cantidad del movimiento debe ser mayor a 0." });

            if (string.IsNullOrWhiteSpace(movimiento.Tipo))
                return BadRequest(new { mensaje = "El tipo de movimiento es obligatorio (ej. ENTRADA, SALIDA)." });

            var bodegaExiste = await _context.Bodegas.AnyAsync(b => b.IdBodega == movimiento.IdBodega);
            if (!bodegaExiste)
                return BadRequest(new { mensaje = $"La bodega con ID {movimiento.IdBodega} no existe." });

            if (movimiento.Fecha == default)
                movimiento.Fecha = DateTime.Now;

            _context.MovimientosInventario.Add(movimiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovimiento), new { id = movimiento.IdMovimiento }, movimiento);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarMovimiento(int id, MovimientoInventario movimiento)
        {
            if (id <= 0 || id != movimiento.IdMovimiento)
                return BadRequest(new { mensaje = "El ID de la ruta no coincide con el de la entidad." });

            if (movimiento.Cantidad <= 0)
                return BadRequest(new { mensaje = "La cantidad debe ser mayor a 0." });

            var existe = await _context.MovimientosInventario.AnyAsync(m => m.IdMovimiento == id);
            if (!existe)
                return NotFound(new { mensaje = $"El movimiento con ID {id} no existe." });

            _context.Entry(movimiento).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BorrarMovimiento(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var movimiento = await _context.MovimientosInventario.FindAsync(id);
            if (movimiento == null)
                return NotFound(new { mensaje = $"El movimiento con ID {id} no existe." });

            _context.MovimientosInventario.Remove(movimiento);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
