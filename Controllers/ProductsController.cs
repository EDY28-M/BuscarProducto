using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuscarProducto.Data;
using BuscarProducto.Model;

namespace BuscarProducto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProductsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetAll()
        {
            var items = await _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Producto>> GetById(int id)
        {
            var producto = await _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (producto == null)
                return NotFound();
            return Ok(producto);
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> Create([FromBody] Producto nuevo)
        {
            if (nuevo == null)
                return BadRequest();

            nuevo.Id = 0;
            _db.Productos.Add(nuevo);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Producto actualizado)
        {
            if (actualizado == null || id != actualizado.Id)
                return BadRequest();

            var existe = await _db.Productos.AnyAsync(p => p.Id == id);
            if (!existe)
                return NotFound();

            _db.Entry(actualizado).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _db.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            _db.Productos.Remove(producto);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Producto>>> Search(
            [FromQuery] string? nombre,
            [FromQuery] string? categoria,
            [FromQuery] string? marca,
            [FromQuery] decimal? minPrecio,
            [FromQuery] decimal? maxPrecio)
        {
            var query = _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(p => EF.Functions.Like(p.Nombre, $"%{nombre}%"));

            if (!string.IsNullOrWhiteSpace(categoria))
                query = query.Where(p => EF.Functions.Like(p.Categoria.Nombre, $"%{categoria}%"));

            if (!string.IsNullOrWhiteSpace(marca))
                query = query.Where(p => p.Marca != null && EF.Functions.Like(p.Marca.Nombre, $"%{marca}%"));

            if (minPrecio.HasValue)
                query = query.Where(p => p.Precio >= minPrecio.Value);

            if (maxPrecio.HasValue)
                query = query.Where(p => p.Precio <= maxPrecio.Value);

            var resultados = await query.ToListAsync();
            return Ok(resultados);
        }
    }
}
