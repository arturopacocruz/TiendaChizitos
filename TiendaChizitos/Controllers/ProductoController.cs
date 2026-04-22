using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaChizitos.Data;
using TiendaChizitos.DTO.Producto.AgregarProducto;
using TiendaChizitos.Entidades;

namespace TiendaChizitos.Controllers
{
    public class ProductosController : BaseApiController
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
            var productos = await _contexto.Productos.ToListAsync();
            return Ok(productos);
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
        public async Task<ActionResult<AgregarProductoOutput>> CreateProducto([FromBody] AgregarProductoInput producto)
        {
            var entrada = new Producto
            {
                Nombre = producto.Nombre,
                Precio = producto.precio,
                Stock = producto.stock
            };

            
            entrada.Id = Guid.NewGuid();
            // El producto es vigente si tiene stock disponible
            entrada.EsVigente = entrada.Stock > 0;;

            _contexto.Productos.Add(entrada);
            await _contexto.SaveChangesAsync();

            var salida = new AgregarProductoOutput
            {
                Id = entrada.Id,
                Nombre = entrada.Nombre,
                precio = entrada.Precio,
                stock = entrada.Stock
            };

            return CreatedAtAction(nameof(GetProducto), new { id = salida.Id }, salida);
        }

        // PUT: api/productos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(Guid id, [FromBody] Producto producto)
        {
            if (id != producto.Id)
                return BadRequest("El ID no coincide.");

            var existing = await _contexto.Productos.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Nombre = producto.Nombre;
            existing.Precio = producto.Precio;
            existing.Stock = producto.Stock;


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