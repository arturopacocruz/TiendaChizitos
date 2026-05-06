using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaChizitos.Data;
using TiendaChizitos.Entidades;
using TiendaChizitos.DTO.Categoria.AñadirCategoria;
using TiendaChizitos.DTO.Categoria.ConsultaCategoria;
using TiendaChizitos.DTO.Categoria.EditarCategoria;

namespace TiendaChizitos.Controllers
{
    public class CategoriasController : BaseApiController
    {
        private readonly AppDbContext _contexto;

        public CategoriasController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        // GET: api/categorias
        [HttpGet("listarCategorias")]
        public async Task<ActionResult<ICollection<Categoria>>> GetCategorias()
        {
            return Ok(await _contexto.Categorias.AsNoTracking().ToListAsync());
        }

        // GET: api/categorias/Buscar?nombre=papas
        [HttpGet("BuscarCategoria")]
        public async Task<ActionResult<ICollection<CategoriaConListaProductosOutput>>> BuscarCategorias([FromQuery] string? nombre)
        {
            var query = _contexto.Categorias
                .AsNoTracking()
                .Include(c => c.Productos)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                var termino = nombre.Trim().ToLower();
                query = query.Where(categoria => categoria.Nombre.ToLower().Contains(termino));
            }

            var lista = await query
                .Select(categoria => new CategoriaConListaProductosOutput
                {
                    Id = categoria.Id,
                    Nombre = categoria.Nombre,
                    Productos = categoria.Productos.Select(p => new ProductoOutput
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Precio = p.Precio,
                        Stock = p.Stock,
                        EsVigente = p.EsVigente
                    }).ToList()
                })
                .ToListAsync();

            return Ok(lista);
        }

        // GET: api/categorias/Resumen
        [HttpGet("ResumenCategoria")]
        public async Task<ActionResult<ICollection<CategoriaConProductosOutput>>> GetCategoriasConConteo()
        {
            var lista = await _contexto.Categorias
                .Select(categoria => new CategoriaConProductosOutput
                {
                    Id = categoria.Id,
                    Nombre = categoria.Nombre,
                    TotalProductos = categoria.Productos.Count
                })
                .ToListAsync();

            return Ok(lista);
        }

        // GET: api/categorias/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("El ID de la categoría es inválido.");

            var categoria = await _contexto.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound();

            return Ok(categoria);
        }

        // POST: api/categorias
        [HttpPost]
        public async Task<ActionResult<AgregarCategoriaOutput>> CreateCategoria([FromBody] AgregarCategoriaInput categoriaDto)
        {
            if (string.IsNullOrWhiteSpace(categoriaDto.Nombre))
                return BadRequest("El nombre de la categoría es obligatorio.");

            if (categoriaDto.Nombre.Length > 50)
                return BadRequest("El nombre de la categoría no puede tener más de 50 caracteres.");

            var nombreNormalizado = categoriaDto.Nombre.Trim();
            var existeCategoria = await _contexto.Categorias.AnyAsync(c => c.Nombre == nombreNormalizado);
            if (existeCategoria)
                return Conflict("Ya existe una categoría con el mismo nombre.");

            var categoria = new Categoria
            {
                Id = Guid.NewGuid(),
                Nombre = nombreNormalizado
            };

            _contexto.Categorias.Add(categoria);
            await _contexto.SaveChangesAsync();

            var respuesta = new AgregarCategoriaOutput
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre
            };

            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, respuesta);
        }

        // PUT: api/categorias/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoria(Guid id, [FromBody] ActualizarCategoriaInput categoriaDto)
        {
            if (id == Guid.Empty)
                return BadRequest("El ID de la categoría es inválido.");

            if (string.IsNullOrWhiteSpace(categoriaDto.Nombre))
                return BadRequest("El nombre de la categoría es obligatorio.");

            if (categoriaDto.Nombre.Length > 50)
                return BadRequest("El nombre de la categoría no puede tener más de 50 caracteres.");

            var categoriaExistente = await _contexto.Categorias.FindAsync(id);
            if (categoriaExistente == null)
                return NotFound();

            var nombreNormalizado = categoriaDto.Nombre.Trim();
            var existeCategoria = await _contexto.Categorias.AnyAsync(c =>
                c.Id != id && c.Nombre == nombreNormalizado);

            if (existeCategoria)
                return Conflict("Ya existe otra categoría con el mismo nombre.");

            categoriaExistente.Nombre = nombreNormalizado;
            await _contexto.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/categorias/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("El ID de la categoría es inválido.");

            var categoria = await _contexto.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound();

            var tieneProductos = await _contexto.Productos.AnyAsync(p => p.CategoriaId == id);
            if (tieneProductos)
                return Conflict("No se puede eliminar una categoría que tiene productos asociados.");

            _contexto.Categorias.Remove(categoria);
            await _contexto.SaveChangesAsync();

            return NoContent();
        }
    }
}