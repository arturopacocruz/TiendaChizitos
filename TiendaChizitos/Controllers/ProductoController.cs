using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaChizitos.Data;
using TiendaChizitos.DTO.Producto.AgregarProducto;
using TiendaChizitos.DTO.Producto.EditarProducto;
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

            if (esVigente.HasValue)
            {
                query = query.Where(
                    producto => producto.EsVigente == esVigente.Value
                );
            }

            var lista = await query.ToListAsync();

            return Ok(lista);
        } 

        // QUERY POR NOMBRE
        // api/productos/Buscar?nombre=papas
        [HttpGet("Buscar")]
        public async Task<ActionResult<ICollection<Producto>>> BuscarProductos(
            [FromQuery] string? nombre
        )
        {
            var query = _contexto.Productos
                .Include(producto => producto.Categoria)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                var termino = nombre.Trim().ToLower();

                query = query.Where(producto =>
                    producto.Nombre.ToLower().Contains(termino)
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
        public async Task<ActionResult<AgregarProductoOutput>> CreateProducto(
            [FromBody] AgregarProductoInput productoDto
        )
        {
            if (string.IsNullOrWhiteSpace(productoDto.Nombre))
                return BadRequest("El nombre del producto es obligatorio.");

            if (productoDto.Nombre.Length > 100)
                return BadRequest("El nombre del producto no puede tener más de 100 caracteres.");

            if (productoDto.Precio < 0)
                return BadRequest("El precio del producto no puede ser negativo.");

            if (productoDto.Stock < 0)
                return BadRequest("El stock del producto no puede ser negativo.");

            if (productoDto.CategoriaId.HasValue)
            {
                var categoriaExiste = await _contexto.Categorias
                    .AnyAsync(c => c.Id == productoDto.CategoriaId.Value);

                if (!categoriaExiste)
                    return BadRequest("La categoría especificada no existe.");
            }

            var producto = new Producto
            {
                Id = Guid.NewGuid(),
                Nombre = productoDto.Nombre.Trim(),
                Precio = productoDto.Precio,
                Stock = productoDto.Stock,
                EsVigente = productoDto.EsVigente,
                CategoriaId = productoDto.CategoriaId
            };

            _contexto.Productos.Add(producto);
            await _contexto.SaveChangesAsync();

            var respuesta = new AgregarProductoOutput
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CategoriaId = producto.CategoriaId,
                EsVigente = producto.EsVigente
            };

            return CreatedAtAction(
                nameof(GetProducto),
                new { id = producto.Id },
                respuesta
            );
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(
            Guid id,
            [FromBody] ActualizarProductoInput productoDto
        )
        {
            if (id == Guid.Empty)
                return BadRequest("El ID del producto es inválido.");

            if (string.IsNullOrWhiteSpace(productoDto.Nombre))
                return BadRequest("El nombre del producto es obligatorio.");

            if (productoDto.Nombre.Length > 100)
                return BadRequest("El nombre del producto no puede tener más de 100 caracteres.");

            if (productoDto.Precio < 0)
                return BadRequest("El precio del producto no puede ser negativo.");

            if (productoDto.Stock < 0)
                return BadRequest("El stock del producto no puede ser negativo.");

            if (productoDto.CategoriaId.HasValue)
            {
                var categoriaExiste = await _contexto.Categorias
                    .AnyAsync(c => c.Id == productoDto.CategoriaId.Value);

                if (!categoriaExiste)
                    return BadRequest("La categoría especificada no existe.");
            }

            var productoExistente = await _contexto.Productos.FindAsync(id);

            if (productoExistente == null)
                return NotFound();

            productoExistente.Nombre = productoDto.Nombre.Trim();
            productoExistente.Precio = productoDto.Precio;
            productoExistente.Stock = productoDto.Stock;
            productoExistente.EsVigente = productoDto.EsVigente;
            productoExistente.CategoriaId = productoDto.CategoriaId;

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