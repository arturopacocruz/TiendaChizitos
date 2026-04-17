using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaChizitos.Data;
using TiendaChizitos.Entidades;

namespace TiendaChizitos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _contexto;

        public ProductosController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<ICollection<Producto>>> GetProductos()
        {
            return Ok(await _contexto.Productos.ToListAsync());
        }

        // GET: api/productos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(Guid id)
        {
            var producto = await _contexto.Productos.FindAsync(id);

            if (producto == null)
                return NotFound();

            return Ok(producto);
        }

        // POST: api/productos
        [HttpPost]
        public async Task<ActionResult<Producto>> CreateProducto([FromBody] Producto producto)
        {
            producto.Id = Guid.NewGuid();

            _contexto.Productos.Add(producto);
            await _contexto.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
        }

        // PUT: api/productos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(Guid id, [FromBody] Producto producto)
        {
            if (id != producto.Id)
                return BadRequest();

            var existing = await _contexto.Productos.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Nombre = producto.Nombre;
            existing.Precio = producto.Precio;
            existing.Stock = producto.Stock;
            existing.CategoriaId = producto.CategoriaId;

            existing.EsVigente = producto.EsVigente;

            await _contexto.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/productos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(Guid id)
        {
            var producto = await _contexto.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            _contexto.Productos.Remove(producto);
            await _contexto.SaveChangesAsync();

            return NoContent();
        }
    }
}