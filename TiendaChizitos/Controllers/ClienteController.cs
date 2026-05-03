using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaChizitos.Data;
using TiendaChizitos.Entidades;
using TiendaChizitos.DTO.Cliente;
using TiendaChizitos.DTO.Cliente.ActualizarCliente;

namespace TiendaChizitos.Controllers
{
    public class ClientesController : BaseApiController
    {
        private readonly AppDbContext _contexto;

        public ClientesController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<ICollection<Cliente>>> GetClientes()
        {
            return Ok(await _contexto.Clientes.AsNoTracking().ToListAsync());
        }

        // QUERY POR NOMBRE O CI
        // api/clientes/Buscar?nombre=mariana
        // api/clientes/Buscar?ci=1234567
        [HttpGet("Buscar")]
        public async Task<ActionResult<ICollection<Cliente>>> BuscarClientes(
            [FromQuery] string? nombre,
            [FromQuery] int? ci
        )
        {
            var query = _contexto.Clientes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                var termino = nombre.Trim().ToLower();
                query = query.Where(cliente => cliente.Nombre.ToLower().Contains(termino));
            }

            if (ci.HasValue)
            {
                query = query.Where(cliente => cliente.Ci == ci.Value);
            }

            return Ok(await query.ToListAsync());
        }

        // GET: api/clientes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("El ID del cliente es inválido.");

            var cliente = await _contexto.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult<AñadirClienteOUT>> CreateCliente([FromBody] AñadirClienteIN clienteDto)
        {
            if (clienteDto.Ci <= 0)
                return BadRequest("El CI del cliente debe ser un número válido.");

            if (string.IsNullOrWhiteSpace(clienteDto.Nombre))
                return BadRequest("El nombre del cliente es obligatorio.");

            if (clienteDto.Nombre.Length > 50)
                return BadRequest("El nombre del cliente no puede tener más de 50 caracteres.");

            if (clienteDto.FechaNacimiento.Date > DateTime.UtcNow.Date)
                return BadRequest("La fecha de nacimiento no puede ser futura.");

            // Normalizar extensión
            var extensionNormalizada = AdecuarExtension(clienteDto.Extension);

            // Validar duplicados (CI + Extensión)
            var existeCliente = await _contexto.Clientes.AnyAsync(x =>
                x.Ci == clienteDto.Ci &&
                ((x.Extension ?? string.Empty).Trim().ToUpper() == (extensionNormalizada ?? string.Empty)));

            if (existeCliente)
                return Conflict("Ya existe un cliente con el mismo CI y extensión.");

            // Mapeo de DTO a Entidad
            var nuevoCliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Ci = clienteDto.Ci,
                Nombre = clienteDto.Nombre.Trim(),
                Extension = extensionNormalizada,
                FechaNacimiento = clienteDto.FechaNacimiento
            };

            // Persistencia
            _contexto.Clientes.Add(nuevoCliente);
            await _contexto.SaveChangesAsync();

            // Mapeo de Entidad a DTO de salida
            var respuesta = new AñadirClienteOUT
            {
                Id = nuevoCliente.Id,
                Ci = nuevoCliente.Ci,
                Nombre = nuevoCliente.Nombre,
                Extension = nuevoCliente.Extension,
                FechaNacimiento = nuevoCliente.FechaNacimiento,
                EsFrecuente = nuevoCliente.EsFrecuente,
                PorcentajeDescuento = nuevoCliente.PorcentajeDescuento
            };

            return CreatedAtAction(nameof(GetCliente), new { id = respuesta.Id }, respuesta);
        }

        // PUT: api/clientes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(Guid id, [FromBody] ActualizarClienteIN clienteDto)
        {
            if (id == Guid.Empty)
                return BadRequest("El ID del cliente es inválido.");

            if (clienteDto.Ci <= 0)
                return BadRequest("El CI del cliente debe ser un número válido.");

            if (string.IsNullOrWhiteSpace(clienteDto.Nombre))
                return BadRequest("El nombre del cliente es obligatorio.");

            if (clienteDto.Nombre.Length > 50)
                return BadRequest("El nombre del cliente no puede tener más de 50 caracteres.");

            if (clienteDto.FechaNacimiento.Date > DateTime.UtcNow.Date)
                return BadRequest("La fecha de nacimiento no puede ser futura.");

            var existing = await _contexto.Clientes.FindAsync(id);
            if (existing == null)
                return NotFound();

            var extensionNormalizada = AdecuarExtension(clienteDto.Extension);

            var existeCliente = await _contexto.Clientes.AnyAsync(x =>
                x.Id != id &&
                x.Ci == clienteDto.Ci &&
                ((x.Extension ?? string.Empty).Trim().ToUpper() == (extensionNormalizada ?? string.Empty)));

            if (existeCliente)
                return Conflict("Ya existe otro cliente con el mismo CI y extensión.");

            existing.Nombre = clienteDto.Nombre.Trim();
            existing.Ci = clienteDto.Ci;
            existing.Extension = extensionNormalizada;
            existing.FechaNacimiento = clienteDto.FechaNacimiento;

            await _contexto.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/clientes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("El ID del cliente es inválido.");

            var cliente = await _contexto.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            var tieneVentas = await _contexto.Ventas.AnyAsync(v => v.ClienteId == id);
            if (tieneVentas)
                return Conflict("No se puede eliminar un cliente que tiene ventas registradas.");

            _contexto.Clientes.Remove(cliente);
            await _contexto.SaveChangesAsync();

            return NoContent();
        }

        private static string? AdecuarExtension(string? extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
                return null;

            return extension.Trim().ToUpperInvariant();
        }
    }
}