using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaChizitos.Data;
using TiendaChizitos.Entidades;
using TiendaChizitos.DTO.Cliente; // Asegúrate de importar tus DTOs

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
            // Validación de fecha
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
                Nombre = clienteDto.Nombre,
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
                FechaNacimiento = nuevoCliente.FechaNacimiento
            };

            return CreatedAtAction(nameof(GetCliente), new { id = respuesta.Id }, respuesta);
        }

        // PUT: api/clientes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(Guid id, [FromBody] Cliente cliente)
        {
            if (id == Guid.Empty)
                return BadRequest("El ID del cliente es inválido.");

            if (id != cliente.Id)
                return BadRequest("El ID no coincide.");

            if (cliente.FechaNacimiento.Date > DateTime.UtcNow.Date)
                return BadRequest("La fecha de nacimiento no puede ser futura.");

            var existing = await _contexto.Clientes.FindAsync(id);
            if (existing == null)
                return NotFound();

            var extensionNormalizada = AdecuarExtension(cliente.Extension);

            var existeCliente = await _contexto.Clientes.AnyAsync(x =>
                x.Id != id &&
                x.Ci == cliente.Ci &&
                ((x.Extension ?? string.Empty).Trim().ToUpper() == (extensionNormalizada ?? string.Empty)));

            if (existeCliente)
                return Conflict("Ya existe otro cliente con el mismo CI y extensión.");

            existing.Nombre = cliente.Nombre;
            existing.Ci = cliente.Ci;
            existing.Extension = extensionNormalizada;
            existing.FechaNacimiento = cliente.FechaNacimiento;
            existing.EsFrecuente = cliente.EsFrecuente;
            existing.PorcentajeDescuento = cliente.PorcentajeDescuento;

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