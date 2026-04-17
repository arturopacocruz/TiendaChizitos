using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaChizitos.Data;
using TiendaChizitos.Entidades;

namespace TiendaChizitos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _contexto;

        public VentasController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        // GET: api/ventas
        [HttpGet]
        public async Task<ActionResult<ICollection<Venta>>> GetVentas()
        {
            return Ok(await _contexto.Ventas.ToListAsync());
        }

        // GET: api/ventas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Venta>> GetVenta(Guid id)
        {
            var venta = await _contexto.Ventas.FindAsync(id);

            if (venta == null)
                return NotFound();

            return Ok(venta);
        }

        // POST: api/ventas
        [HttpPost]
        public async Task<ActionResult<Venta>> CreateVenta([FromBody] Venta venta)
        {
            venta.Id = Guid.NewGuid();
            venta.Fecha = DateTime.Now;

            _contexto.Ventas.Add(venta);
            await _contexto.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVenta), new { id = venta.Id }, venta);
        }

        // PUT: api/ventas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVenta(Guid id, [FromBody] Venta venta)
        {
            if (id != venta.Id)
                return BadRequest();

            var existing = await _contexto.Ventas.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Fecha = venta.Fecha;
            existing.Total = venta.Total;
            existing.ClienteId = venta.ClienteId;

            await _contexto.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ventas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(Guid id)
        {
            var venta = await _contexto.Ventas.FindAsync(id);
            if (venta == null)
                return NotFound();

            _contexto.Ventas.Remove(venta);
            await _contexto.SaveChangesAsync();

            return NoContent();
        }
    }
}