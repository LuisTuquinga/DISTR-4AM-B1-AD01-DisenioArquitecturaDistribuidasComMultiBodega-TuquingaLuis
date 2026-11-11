using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedidosApi.Data;
using PedidosApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PedidosApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetallesPedidoController : ControllerBase
    {
        private readonly PedidosDBContext _context;

        public DetallesPedidoController(PedidosDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetallePedido>>> GetDetalles()
        {
            return Ok(await _context.DetallesPedido.AsNoTracking().ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DetallePedido>> GetDetalle(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var detalle = await _context.DetallesPedido
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.IdDetalle == id);

            if (detalle == null)
                return NotFound(new { mensaje = $"El detalle con ID {id} no existe." });

            return Ok(detalle);
        }

        [HttpPost]
        public async Task<ActionResult<DetallePedido>> CrearDetalle(DetallePedido detalle)
        {
            if (detalle == null)
                return BadRequest(new { mensaje = "Los datos del detalle son obligatorios." });

            if (detalle.Cantidad <= 0)
                return BadRequest(new { mensaje = "La cantidad debe ser mayor a 0." });

            if (detalle.PrecioUnitario < 0)
                return BadRequest(new { mensaje = "El precio unitario no puede ser negativo." });

            var pedidoExiste = await _context.Pedidos.AnyAsync(p => p.IdPedido == detalle.IdPedido);
            if (!pedidoExiste)
                return BadRequest(new { mensaje = $"El pedido con ID {detalle.IdPedido} no existe." });

            _context.DetallesPedido.Add(detalle);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDetalle), new { id = detalle.IdDetalle }, detalle);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarDetalle(int id, DetallePedido detalle)
        {
            if (id <= 0 || id != detalle.IdDetalle)
                return BadRequest(new { mensaje = "El ID de la ruta no coincide con el de la entidad." });

            if (detalle.Cantidad <= 0)
                return BadRequest(new { mensaje = "La cantidad debe ser mayor a 0." });

            var existe = await _context.DetallesPedido.AnyAsync(d => d.IdDetalle == id);
            if (!existe)
                return NotFound(new { mensaje = $"El detalle con ID {id} no existe." });

            _context.Entry(detalle).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BorrarDetalle(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var detalle = await _context.DetallesPedido.FindAsync(id);
            if (detalle == null)
                return NotFound(new { mensaje = $"El detalle con ID {id} no existe." });

            _context.DetallesPedido.Remove(detalle);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
