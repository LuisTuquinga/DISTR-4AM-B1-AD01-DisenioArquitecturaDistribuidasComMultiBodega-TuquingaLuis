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
    public class CategoriasController : ControllerBase
    {
        private readonly CatalogoDBContext _context;

        public CategoriasController(CatalogoDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            return Ok(await _context.Categorias.AsNoTracking().ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var categoria = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCategoria == id);

            if (categoria == null)
                return NotFound(new { mensaje = $"La categoría con ID {id} no existe." });

            return Ok(categoria);
        }

        [HttpPost]
        public async Task<ActionResult<Categoria>> CrearCategoria(Categoria categoria)
        {
            if (categoria == null)
                return BadRequest(new { mensaje = "Los datos de la categoría son obligatorios." });

            if (string.IsNullOrWhiteSpace(categoria.Nombre))
                return BadRequest(new { mensaje = "El nombre de la categoría es obligatorio." });

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.IdCategoria }, categoria);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarCategoria(int id, Categoria categoria)
        {
            if (id <= 0 || id != categoria.IdCategoria)
                return BadRequest(new { mensaje = "El ID de la ruta no coincide con el de la entidad." });

            if (string.IsNullOrWhiteSpace(categoria.Nombre))
                return BadRequest(new { mensaje = "El nombre de la categoría es obligatorio." });

            var existe = await _context.Categorias.AnyAsync(c => c.IdCategoria == id);
            if (!existe)
                return NotFound(new { mensaje = $"La categoría con ID {id} no existe." });

            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BorrarCategoria(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "El ID debe ser mayor a 0." });

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound(new { mensaje = $"La categoría con ID {id} no existe." });

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
