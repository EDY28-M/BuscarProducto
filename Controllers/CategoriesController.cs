using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuscarProducto.Data;
using BuscarProducto.Model;

namespace BuscarProducto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CategoriesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetAll()
        {
            var items = await _db.Categorias.ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Categoria>> GetById(int id)
        {
            var categoria = await _db.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound();
            return Ok(categoria);
        }

        [HttpPost]
        public async Task<ActionResult<Categoria>> Create([FromBody] Categoria nuevo)
        {
            if (nuevo == null)
                return BadRequest();

            nuevo.Id = 0;
            _db.Categorias.Add(nuevo);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Categoria actualizado)
        {
            if (actualizado == null || id != actualizado.Id)
                return BadRequest();

            var existe = await _db.Categorias.AnyAsync(c => c.Id == id);
            if (!existe)
                return NotFound();

            _db.Entry(actualizado).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _db.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound();

            _db.Categorias.Remove(categoria);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
