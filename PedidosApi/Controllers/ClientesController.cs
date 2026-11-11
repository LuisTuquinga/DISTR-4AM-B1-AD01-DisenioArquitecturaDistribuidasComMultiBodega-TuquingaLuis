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
    public class ClientesController : ControllerBase
    {
        private readonly PedidosDBContext _context;

        public ClientesController(PedidosDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return Ok(await _context.Clientes.AsNoTracking().ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
                return NotFound(new { mensaje = $"El cliente con ID {id} no existe." });

            return Ok(cliente);
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> CrearCliente(Cliente cliente)
        {
            if (cliente == null)
                return BadRequest(new { mensaje = "Los datos del cliente son obligatorios." });

            if (string.IsNullOrWhiteSpace(cliente.Nombres))
                return BadRequest(new { mensaje = "El nombre del cliente es obligatorio." });

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.IdCliente }, cliente);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarCliente(int id, Cliente cliente)
        {
            if (id <= 0 || id != cliente.IdCliente)
                return BadRequest(new { mensaje = "El ID de la ruta no coincide con el de la entidad." });

            if (string.IsNullOrWhiteSpace(cliente.Nombres))
                return BadRequest(new { mensaje = "El nombre del cliente es obligatorio." });

            var existe = await _context.Clientes.AnyAsync(c => c.IdCliente == id);
            if (!existe)
                return NotFound(new { mensaje = $"El cliente con ID {id} no existe." });

            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BorrarCliente(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound(new { mensaje = $"El cliente con ID {id} no existe." });

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
