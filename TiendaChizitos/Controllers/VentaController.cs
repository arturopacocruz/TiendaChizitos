using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaChizitos.Data;
using TiendaChizitos.Entidades;
using TiendaComida.DTO.Venta.GenerarVenta;

namespace TiendaChizitos.Controllers
{
    public class VentasController : BaseApiController
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
        public async Task<ActionResult<GenerarVentaOutput>> CreateVenta([FromBody] GenerarVentaInput input)
        {
            if (input.Detalle == null || !input.Detalle.Any())
                return BadRequest("La venta debe tener al menos un producto.");

            // VALIDAR CLIENTE
            var cliente = await _contexto.Clientes
                .FirstOrDefaultAsync(c =>
                    c.Ci == input.ci);

            if (cliente == null)
                return BadRequest("El cliente no existe. No se puede realizar la venta.");

            var venta = new Venta
            {
                Id = Guid.NewGuid(),
                Fecha = DateTime.Now,
                ClienteId = cliente.Id,
                DetalleVentas = new List<DetalleVenta>()
            };

            decimal total = 0;

            foreach (var detalle in input.Detalle)
            {
                var producto = await _contexto.Productos
                    .FindAsync(detalle.ProductoId);

                if (producto == null)
                    return BadRequest(
                        $"Producto con ID {detalle.ProductoId} no existe."
                    );

                if (producto.Stock < detalle.Cantidad)
                    return BadRequest(
                        $"Stock insuficiente para producto {detalle.ProductoId}. Disponible: {producto.Stock}"
                    );

                // descontar stock
                producto.Stock -= detalle.Cantidad;

                // subtotal línea
                decimal subtotal = producto.Precio * detalle.Cantidad;

                // acumular total venta
                total += subtotal;

                venta.DetalleVentas.Add(new DetalleVenta
                {
                    Id = Guid.NewGuid(),
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,

                    // guardar precio unitario
                    Precio = producto.Precio
                });
            }

            venta.Total = total;

            _contexto.Ventas.Add(venta);

            await _contexto.SaveChangesAsync();

            var output = new GenerarVentaOutput
            {
                VentaId = venta.Id,
                Fecha = venta.Fecha
            };

            return CreatedAtAction(
                nameof(GetVenta),
                new { id = venta.Id },
                output
            );
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