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
    public class ReservasStockController : ControllerBase
    {
        private readonly InventarioDBContext _context;

        public ReservasStockController(InventarioDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservaStock>>> GetReservas()
        {
            return Ok(await _context.ReservasStock.AsNoTracking().ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReservaStock>> GetReserva(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var reserva = await _context.ReservasStock
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.IdReserva == id);

            if (reserva == null)
                return NotFound(new { mensaje = $"La reserva con ID {id} no existe." });

            return Ok(reserva);
        }

        [HttpPost]
        public async Task<ActionResult<ReservaStock>> CrearReserva(ReservaStock reserva)
        {
            if (reserva == null)
                return BadRequest(new { mensaje = "Los datos de la reserva son obligatorios." });

            if (reserva.Cantidad <= 0)
                return BadRequest(new { mensaje = "La cantidad a reservar debe ser mayor a 0." });

            var bodegaExiste = await _context.Bodegas.AnyAsync(b => b.IdBodega == reserva.IdBodega);
            if (!bodegaExiste)
                return BadRequest(new { mensaje = $"La bodega con ID {reserva.IdBodega} no existe." });

            if (string.IsNullOrWhiteSpace(reserva.Estado))
                reserva.Estado = "PENDIENTE";

            _context.ReservasStock.Add(reserva);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReserva), new { id = reserva.IdReserva }, reserva);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarReserva(int id, ReservaStock reserva)
        {
            if (id <= 0 || id != reserva.IdReserva)
                return BadRequest(new { mensaje = "El ID de la ruta no coincide con el de la entidad." });

            if (reserva.Cantidad <= 0)
                return BadRequest(new { mensaje = "La cantidad debe ser mayor a 0." });

            var existe = await _context.ReservasStock.AnyAsync(r => r.IdReserva == id);
            if (!existe)
                return NotFound(new { mensaje = $"La reserva con ID {id} no existe." });

            _context.Entry(reserva).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BorrarReserva(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var reserva = await _context.ReservasStock.FindAsync(id);
            if (reserva == null)
                return NotFound(new { mensaje = $"La reserva con ID {id} no existe." });

            _context.ReservasStock.Remove(reserva);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
