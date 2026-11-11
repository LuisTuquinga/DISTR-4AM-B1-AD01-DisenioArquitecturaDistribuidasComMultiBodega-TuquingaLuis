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
    public class ExistenciasController : ControllerBase
    {
        private readonly InventarioDBContext _context;

        public ExistenciasController(InventarioDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Existencia>>> GetExistencias()
        {
            return Ok(await _context.Existencias.AsNoTracking().ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Existencia>> GetExistencia(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var existencia = await _context.Existencias
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdExistencia == id);

            if (existencia == null)
                return NotFound(new { mensaje = $"La existencia con ID {id} no existe." });

            return Ok(existencia);
        }


        // =========================================================================
        // Endpoint consultado por otros microservicios (por ejemplo Catalogo.Api)
        // GET: api/existencias/por-producto/1
        // =========================================================================
        [HttpGet("por-producto/{idProducto:int}")]
        public async Task<ActionResult<Existencia>> GetExistenciaPorProducto(int idProducto)
        {
            if (idProducto <= 0)
                return BadRequest(new { mensaje = "El IdProducto debe ser mayor a 0." });

            var existencia = await _context.Existencias
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdProducto == idProducto);

            if (existencia == null)
                return NotFound(new { mensaje = $"No existen registros de stock para el producto con ID {idProducto}." });

            return Ok(existencia);
        }

        [HttpPost]
        public async Task<ActionResult<Existencia>> CrearExistencia(Existencia existencia)
        {
            if (existencia == null)
                return BadRequest(new { mensaje = "Los datos de existencia son obligatorios." });

            if (existencia.IdProducto <= 0)
                return BadRequest(new { mensaje = "El IdProducto debe ser válido y mayor a 0." });

            if (existencia.Stock < 0)
                return BadRequest(new { mensaje = "El stock no puede ser negativo." });

            var bodegaExiste = await _context.Bodegas.AnyAsync(b => b.IdBodega == existencia.IdBodega);
            if (!bodegaExiste)
                return BadRequest(new { mensaje = $"La bodega con ID {existencia.IdBodega} no existe." });

            _context.Existencias.Add(existencia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExistencia), new { id = existencia.IdExistencia }, existencia);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarExistencia(int id, Existencia existencia)
        {
            if (id <= 0 || id != existencia.IdExistencia)
                return BadRequest(new { mensaje = "El ID de la ruta no coincide con el de la entidad." });

            if (existencia.Stock < 0)
                return BadRequest(new { mensaje = "El stock no puede ser negativo." });

            var existe = await _context.Existencias.AnyAsync(e => e.IdExistencia == id);
            if (!existe)
                return NotFound(new { mensaje = $"La existencia con ID {id} no existe." });

            _context.Entry(existencia).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BorrarExistencia(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var existencia = await _context.Existencias.FindAsync(id);
            if (existencia == null)
                return NotFound(new { mensaje = $"La existencia con ID {id} no existe." });

            _context.Existencias.Remove(existencia);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
