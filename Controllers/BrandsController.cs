using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuscarProducto.Data;
using BuscarProducto.Model;

namespace BuscarProducto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public BrandsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Marca>>> GetAll()
        {
            var items = await _db.Marcas.ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Marca>> GetById(int id)
        {
            var marca = await _db.Marcas.FindAsync(id);
            if (marca == null)
                return NotFound();
            return Ok(marca);
        }

        [HttpPost]
        public async Task<ActionResult<Marca>> Create([FromBody] Marca nuevo)
        {
            if (nuevo == null)
                return BadRequest();

            nuevo.Id = 0;
            _db.Marcas.Add(nuevo);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Marca actualizado)
        {
            if (actualizado == null || id != actualizado.Id)
                return BadRequest();

            var existe = await _db.Marcas.AnyAsync(m => m.Id == id);
            if (!existe)
                return NotFound();

            _db.Entry(actualizado).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var marca = await _db.Marcas.FindAsync(id);
            if (marca == null)
                return NotFound();

            _db.Marcas.Remove(marca);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
