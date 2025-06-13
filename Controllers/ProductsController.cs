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

        /// <summary>
        /// Lista todos los productos.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetAll()
        {
            var items = await _db.Productos.ToListAsync();
            return Ok(items);
        }

        /// <summary>
        /// Obtiene un producto por su Id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Producto>> GetById(int id)
        {
            var producto = await _db.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();
            return Ok(producto);
        }

        /// <summary>
        /// Crea un nuevo producto (el Id siempre lo asigna la BD).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Producto>> Create([FromBody] Producto nuevo)
        {
            if (nuevo == null)
                return BadRequest();

            // Forzamos que EF genere un nuevo Id
            nuevo.Id = 0;

            _db.Productos.Add(nuevo);
            await _db.SaveChangesAsync();

            // Devolvemos 201 Created con la ruta al recurso
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }


        /// <summary>
        /// Actualiza un producto existente.
        /// </summary>
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

        /// <summary>
        /// Elimina un producto.
        /// </summary>
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

        /// <summary>
        /// Busca productos por nombre, categoría y/o rango de precio.
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Producto>>> Search(
            [FromQuery] string? name,
            [FromQuery] string? category,
            [FromQuery] decimal? minPrecio,
            [FromQuery] decimal? maxPrecio)
        {
            var query = _db.Productos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, $"%{name}%"));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p =>
                    p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

            if (minPrecio.HasValue)
                query = query.Where(p => p.Precio >= minPrecio.Value);

            if (maxPrecio.HasValue)
                query = query.Where(p => p.Precio <= maxPrecio.Value);

            var resultados = await query.ToListAsync();
            return Ok(resultados);
        }
    }
}
