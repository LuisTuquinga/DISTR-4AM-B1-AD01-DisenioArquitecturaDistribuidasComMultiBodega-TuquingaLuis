using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedidosApi.Data;
using PedidosApi.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PedidosApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly PedidosDBContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public PedidosController(PedidosDBContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            return Ok(await _context.Pedidos.AsNoTracking().ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var pedido = await _context.Pedidos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
                return NotFound(new { mensaje = $"El pedido con ID {id} no existe." });

            return Ok(pedido);
        }



        // =========================================================================
        // COMUNICACIÓN DISTRIBUIDA (Pedidos.Api -> Catalogo.Api)
        // Consulta un pedido con sus detalles y resuelve los productos en Catalogo.Api
        // =========================================================================
        [HttpGet("{id:int}/con-catalogo")]
        public async Task<IActionResult> GetPedidoConCatalogo(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID del pedido debe ser mayor a 0." });

            var pedido = await _context.Pedidos.AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
                return NotFound(new { mensaje = $"El pedido con ID {id} no existe." });

            var detalles = await _context.DetallesPedido
                .Where(d => d.IdPedido == id)
                .AsNoTracking()
                .ToListAsync();

            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCliente == pedido.IdCliente);

            var httpCatalogo = _httpClientFactory.CreateClient("CatalogoApi");
            var itemsEnriquecidos = new List<object>();

            foreach (var det in detalles)
            {
                object? productoInfo = null;
                try
                {
                    var res = await httpCatalogo.GetAsync($"api/productos/{det.IdProducto}");
                    if (res.IsSuccessStatusCode)
                    {
                        productoInfo = await res.Content.ReadFromJsonAsync<object>();
                    }
                }
                catch
                {

                    productoInfo = new { mensaje = "Información del producto no disponible (Catalogo.Api no accesible)." };
                }

                itemsEnriquecidos.Add(new
                {
                    detalle = det,
                    producto = productoInfo
                });
            }

            return Ok(new
            {
                pedido,
                cliente,
                detalles = itemsEnriquecidos
            });
        }

        [HttpPost]
        public async Task<ActionResult<Pedido>> CrearPedido(Pedido pedido)
        {
            if (pedido == null)
                return BadRequest(new { mensaje = "Los datos del pedido son obligatorios." });

            var clienteExiste = await _context.Clientes.AnyAsync(c => c.IdCliente == pedido.IdCliente);
            if (!clienteExiste)
                return BadRequest(new { mensaje = $"El cliente con ID {pedido.IdCliente} no existe." });

            if (pedido.Fecha == default)
                pedido.Fecha = DateTime.Now;

            if (string.IsNullOrWhiteSpace(pedido.Estado))
                pedido.Estado = "PENDIENTE";

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.IdPedido }, pedido);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarPedido(int id, Pedido pedido)
        {
            if (id <= 0 || id != pedido.IdPedido)
                return BadRequest(new { mensaje = "El ID de la ruta no coincide con el de la entidad." });

            var existe = await _context.Pedidos.AnyAsync(p => p.IdPedido == id);
            if (!existe)
                return NotFound(new { mensaje = $"El pedido con ID {id} no existe." });

            _context.Entry(pedido).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BorrarPedido(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound(new { mensaje = $"El pedido con ID {id} no existe." });

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
