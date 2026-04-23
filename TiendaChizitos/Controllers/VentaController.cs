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
        public async Task<ActionResult<GenerarVentaOutput>> CreateVenta(
            [FromBody] GenerarVentaInput input)
        {
            if (input.Detalle == null || !input.Detalle.Any())
                return BadRequest(
                    "La venta debe tener al menos un producto."
                );

            // VALIDAR CLIENTE
            var cliente = await _contexto.Clientes
                .FirstOrDefaultAsync(cliente => cliente.Ci == input.ci);

            if (cliente == null)
                return BadRequest(
                    "El cliente no existe. No se puede realizar la venta."
                );

            var venta = new Venta
            {
                Id = Guid.NewGuid(),
                Fecha = DateTime.Now,
                ClienteId = cliente.Id,
                DetalleVentas = new List<DetalleVenta>()
            };

            decimal total = 0;

            foreach (var detalleInput in input.Detalle)
            {
                var producto = await _contexto.Productos
                    .FindAsync(detalleInput.ProductoId);

                if (producto == null)
                    return BadRequest(
                        $"Producto con ID {detalleInput.ProductoId} no existe."
                    );

                if (producto.Stock < detalleInput.Cantidad)
                    return BadRequest(
                        $"Stock insuficiente para producto {detalleInput.ProductoId}. Disponible: {producto.Stock}"
                    );

                // Descontar stock
                producto.Stock -= detalleInput.Cantidad;

                if (producto.Stock == 0)
                {
                    producto.EsVigente = false;
                }

                decimal subtotal =
                    producto.Precio * detalleInput.Cantidad;

                total += subtotal;

                venta.DetalleVentas.Add(new DetalleVenta
                {
                    Id = Guid.NewGuid(),
                    ProductoId = detalleInput.ProductoId,
                    Cantidad = detalleInput.Cantidad,
                    Precio = producto.Precio
                });
            }

            venta.Total = total;

            _contexto.Ventas.Add(venta);

            await _contexto.SaveChangesAsync();

            // Ouput
            var output = new GenerarVentaOutput
            {
                Fecha = venta.Fecha,
                Total = venta.Total,

                Cliente = new ClienteVentaOutput
                {
                    Nombre = cliente.Nombre,
                    Ci = cliente.Ci,
                    Extension = cliente.Extension,
                    PorcentajeDescuento = cliente.PorcentajeDescuento
                },

                Detalle = venta.DetalleVentas
                    .Select(detalleVenta => new DetalleVentaOutput
                    {

                        Nombre = _contexto.Productos
                            .First(producto =>
                                producto.Id == detalleVenta.ProductoId)
                            .Nombre,

                        Cantidad = detalleVenta.Cantidad,

                        Precio = detalleVenta.Precio,

                        Subtotal =
                            detalleVenta.Cantidad *
                            detalleVenta.Precio

                    }).ToList()
            };

            return CreatedAtAction(
                nameof(GetVenta),
                new { id = venta.Id },
                output
            );
        }

        // PUT: api/ventas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVenta(
            Guid id,
            [FromBody] Venta venta)
        {
            if (id != venta.Id)
                return BadRequest();

            var ventaExistente =
                await _contexto.Ventas.FindAsync(id);

            if (ventaExistente == null)
                return NotFound();

            ventaExistente.Fecha = venta.Fecha;
            ventaExistente.Total = venta.Total;
            ventaExistente.ClienteId = venta.ClienteId;

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