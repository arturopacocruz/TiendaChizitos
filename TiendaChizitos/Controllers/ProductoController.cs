using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaChizitos.Data;
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
            return Ok(
                await _contexto.Productos
                    .Include(p => p.Categoria)
                    .ToListAsync()
            );
        }


        // QUERY POR VIGENCIA
        // api/productos/ListarPorVigencia?esVigente=true
        // api/productos/ListarPorVigencia?esVigente=false
        // api/productos/ListarPorVigencia
        [HttpGet("ListarPorVigencia")]
        [ActionName("ListarPorVigencia")]
        public async Task<ActionResult<ICollection<Producto>>> ListarPorVigencia(
            [FromQuery] bool? esVigente
        )
        {
            var query = _contexto.Productos
                .Include(producto => producto.Categoria)
                .AsQueryable();


            // true = solo vigentes
            // false = solo no vigentes
            // null = ambos
            if (esVigente.HasValue)
            {
                query = query.Where(
                    producto => producto.EsVigente == esVigente.Value
                );
            }

            var lista = await query.ToListAsync();

            return Ok(lista);
        }



        // GET: api/productos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(Guid id)
        {
            var producto = await _contexto.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(
                    producto => producto.Id == id
                );

            if (producto == null)
                return NotFound();

            return Ok(producto);
        }



        // POST
        [HttpPost]
        public async Task<ActionResult<Producto>> CreateProducto(
            [FromBody] Producto producto
        )
        {
            producto.Id = Guid.NewGuid();

            _contexto.Productos.Add(producto);

            await _contexto.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProducto),
                new { id = producto.Id },
                producto
            );
        }



        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(
            Guid id,
            [FromBody] Producto producto
        )
        {
            if(id != producto.Id)
                return BadRequest();

            var productoExistente =
                await _contexto.Productos.FindAsync(id);

            if(productoExistente == null)
                return NotFound();

            productoExistente.Nombre = producto.Nombre;
            productoExistente.Precio = producto.Precio;
            productoExistente.Stock = producto.Stock;
            productoExistente.EsVigente = producto.EsVigente;
            productoExistente.CategoriaId = producto.CategoriaId;

            await _contexto.SaveChangesAsync();

            return NoContent();
        }



        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(Guid id)
        {
            var producto =
                await _contexto.Productos.FindAsync(id);

            if(producto == null)
                return NotFound();

            _contexto.Productos.Remove(producto);

            await _contexto.SaveChangesAsync();

            return NoContent();
        }
    }
}