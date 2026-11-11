using CatalogoApi.Data;
using CatalogoApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CatalogoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly CatalogoDBContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductosController(CatalogoDBContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            var productos = await _context.Productos.AsNoTracking().ToListAsync();
            return Ok(productos);
        }

        // GET: api/productos/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var producto = await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
                return NotFound(new { mensaje = $"El producto con ID {id} no existe." });

            return Ok(producto);
        }

        // POST: api/productos
        [HttpPost]
        public async Task<ActionResult<Producto>> CrearProducto(Producto producto)
        {
            if (producto == null)
                return BadRequest(new { mensaje = "Los datos del producto son obligatorios." });

            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return BadRequest(new { mensaje = "El nombre del producto no puede estar vacío." });

            if (producto.Precio < 0)
                return BadRequest(new { mensaje = "El precio del producto no puede ser negativo." });

            // Validar que la categoría y marca existan
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.IdCategoria == producto.IdCategoria);
            if (!categoriaExiste)
                return BadRequest(new { mensaje = $"La categoría con ID {producto.IdCategoria} no existe." });

            var marcaExiste = await _context.Marcas.AnyAsync(m => m.IdMarca == producto.IdMarca);
            if (!marcaExiste)
                return BadRequest(new { mensaje = $"La marca con ID {producto.IdMarca} no existe." });

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Retorna 201 Created con cabecera Location
            return CreatedAtAction(nameof(GetProducto), new { id = producto.IdProducto }, producto);
        }

        // PUT: api/productos/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarProducto(int id, Producto producto)
        {
            if (id <= 0 || id != producto.IdProducto)
                return BadRequest(new { mensaje = "El ID de la ruta no coincide con el ID del producto enviado." });

            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return BadRequest(new { mensaje = "El nombre del producto no puede estar vacío." });

            if (producto.Precio < 0)
                return BadRequest(new { mensaje = "El precio del producto no puede ser negativo." });

            var productoExistente = await _context.Productos.FindAsync(id);
            if (productoExistente == null)
                return NotFound(new { mensaje = $"No se puede actualizar porque el producto con ID {id} no existe." });

            productoExistente.Nombre = producto.Nombre;
            productoExistente.SKU = producto.SKU;
            productoExistente.Precio = producto.Precio;
            productoExistente.Estado = producto.Estado;
            productoExistente.IdCategoria = producto.IdCategoria;
            productoExistente.IdMarca = producto.IdMarca;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/productos/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BorrarProducto(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound(new { mensaje = $"No se puede eliminar porque el producto con ID {id} no existe." });

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        // comunicacion entre Catalogo.Api -> Inventario.Api

        [HttpGet("{id:int}/con-inventario")]
        public async Task<IActionResult> GetProductoInventario(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID del producto debe ser mayor a 0." });

            var producto = await _context.Productos.AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
                return NotFound(new { mensaje = "El producto no existe." });

            var cliente = _httpClientFactory.CreateClient("InventarioApi");

            try
            {
                using var respuesta = await cliente.GetAsync($"api/existencias/por-producto/{id}");

                if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return Ok(new
                    {
                        producto,
                        inventario = (object?)null,
                        mensaje = "El producto existe, pero no tiene inventario registrado."
                    });
                }

                if (!respuesta.IsSuccessStatusCode)
                {
                    return StatusCode(StatusCodes.Status502BadGateway, new
                    {
                        mensaje = "Inventario.Api respondió con un error HTTP."
                    });
                }

                var inventario = await respuesta.Content.ReadFromJsonAsync<object>();

                if (inventario is null)
                {
                    return StatusCode(StatusCodes.Status502BadGateway, new
                    {
                        mensaje = "Inventario.Api devolvió una respuesta vacía."
                    });
                }

                return Ok(new { producto, inventario });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    mensaje = "No se pudo comunicar con el microservicio de Inventario (Inventario.Api no está disponible o apagado).",
                    detalle = ex.Message
                });
            }
        }
    }
}
