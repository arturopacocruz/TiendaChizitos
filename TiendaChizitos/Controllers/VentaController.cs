using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaChizitos.Data;
using TiendaChizitos.DTO.Venta.ActualizarVenta;
using TiendaChizitos.DTO.Venta.ListarVentas;
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
        public async Task<ActionResult<ICollection<ListarVentasOutput>>> GetVentas()
        {
            var ventas = await _contexto.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.DetalleVentas)
                .ThenInclude(d => d.Producto)
                .AsNoTracking()
                .ToListAsync();

            var result = ventas.Select(v => new ListarVentasOutput
            {
                Id = v.Id,
                Fecha = v.Fecha,
                Total = v.Total,
                Cliente = new ClienteListarVentasOutput
                {
                    Nombre = v.Cliente?.Nombre ?? string.Empty,
                    Ci = v.Cliente?.Ci ?? 0,
                    Extension = v.Cliente?.Extension,
                    PorcentajeDescuento = v.Cliente?.PorcentajeDescuento ?? 0m
                },
                Detalle = v.DetalleVentas.Select(d => new DetalleListarVentasOutput
                {
                    Nombre = d.Producto?.Nombre ?? string.Empty,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    Subtotal = Math.Round(d.Cantidad * d.Precio * (1 - (v.Cliente?.PorcentajeDescuento ?? 0m) / 100m), 2)
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // QUERY POR CLIENTE
        // api/ventas/ListarPorCliente?ci=1234567&extension=LP
        [HttpGet("ListarPorCliente")]
        public async Task<ActionResult<ICollection<ListarVentasOutput>>> ListarPorCliente(
            [FromQuery] int? ci,
            [FromQuery] string? extension
        )
        {
            if (!ci.HasValue)
                return BadRequest("Debe proporcionar el CI del cliente.");

            var extensionNormalizada = AdecuarExtension(extension);

            var ventas = await _contexto.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.DetalleVentas)
                .ThenInclude(d => d.Producto)
                .AsQueryable()
                .Where(v =>
                    v.Cliente != null &&
                    v.Cliente.Ci == ci.Value &&
                    (extensionNormalizada == null ||
                     (v.Cliente.Extension ?? string.Empty) == extensionNormalizada)
                )
                .ToListAsync();

            var result = ventas.Select(v => new ListarVentasOutput
            {
                Id = v.Id,
                Fecha = v.Fecha,
                Total = v.Total,
                Cliente = new ClienteListarVentasOutput
                {
                    Nombre = v.Cliente?.Nombre ?? string.Empty,
                    Ci = v.Cliente?.Ci ?? 0,
                    Extension = v.Cliente?.Extension,
                    PorcentajeDescuento = v.Cliente?.PorcentajeDescuento ?? 0m
                },
                Detalle = v.DetalleVentas.Select(d => new DetalleListarVentasOutput
                {
                    Nombre = d.Producto?.Nombre ?? string.Empty,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    Subtotal = Math.Round(d.Cantidad * d.Precio * (1 - (v.Cliente?.PorcentajeDescuento ?? 0m) / 100m), 2)
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // QUERY POR FECHA
        // api/ventas/ListarPorFecha?desde=2026-01-01&hasta=2026-05-03
        [HttpGet("ListarPorFecha")]
        public async Task<ActionResult<ICollection<ListarVentasOutput>>> ListarPorFecha(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta
        )
        {
            if (desde.HasValue && hasta.HasValue && desde.Value > hasta.Value)
                return BadRequest("La fecha 'desde' no puede ser mayor que la fecha 'hasta'.");

            var ventas = await _contexto.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.DetalleVentas)
                .ThenInclude(d => d.Producto)
                .AsQueryable()
                .Where(v =>
                    (!desde.HasValue || v.Fecha.Date >= desde.Value.Date) &&
                    (!hasta.HasValue || v.Fecha.Date <= hasta.Value.Date)
                )
                .ToListAsync();

            var result = ventas.Select(v => new ListarVentasOutput
            {
                Id = v.Id,
                Fecha = v.Fecha,
                Total = v.Total,
                Cliente = new ClienteListarVentasOutput
                {
                    Nombre = v.Cliente?.Nombre ?? string.Empty,
                    Ci = v.Cliente?.Ci ?? 0,
                    Extension = v.Cliente?.Extension,
                    PorcentajeDescuento = v.Cliente?.PorcentajeDescuento ?? 0m
                },
                Detalle = v.DetalleVentas.Select(d => new DetalleListarVentasOutput
                {
                    Nombre = d.Producto?.Nombre ?? string.Empty,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    Subtotal = Math.Round(d.Cantidad * d.Precio * (1 - (v.Cliente?.PorcentajeDescuento ?? 0m) / 100m), 2)
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        // GET: api/ventas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ListarVentasOutput>> GetVenta(Guid id)
        {
            var venta = await _contexto.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.DetalleVentas)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
                return NotFound();

            var result = new ListarVentasOutput
            {
                Id = venta.Id,
                Fecha = venta.Fecha,
                Total = venta.Total,
                Cliente = new ClienteListarVentasOutput
                {
                    Nombre = venta.Cliente?.Nombre ?? string.Empty,
                    Ci = venta.Cliente?.Ci ?? 0,
                    Extension = venta.Cliente?.Extension,
                    PorcentajeDescuento = venta.Cliente?.PorcentajeDescuento ?? 0m
                },
                Detalle = venta.DetalleVentas.Select(d => new DetalleListarVentasOutput
                {
                    Nombre = d.Producto?.Nombre ?? string.Empty,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    Subtotal = Math.Round(d.Cantidad * d.Precio * (1 - (venta.Cliente?.PorcentajeDescuento ?? 0m) / 100m), 2)
                }).ToList()
            };

            return Ok(result);
        }

        // POST: api/ventas
        [HttpPost]
        public async Task<ActionResult<GenerarVentaOutput>> CreateVenta(
            [FromBody] GenerarVentaInput input)
        {
            if (input.ci <= 0)
                return BadRequest("El CI del cliente es obligatorio y debe ser válido.");

            if (input.Detalle == null || !input.Detalle.Any())
                return BadRequest("La venta debe tener al menos un producto.");

            foreach (var detalle in input.Detalle)
            {
                if (detalle.Cantidad <= 0)
                    return BadRequest("La cantidad de cada producto debe ser mayor a cero.");
            }

            var extensionNormalizada = AdecuarExtension(input.extension);

            // VALIDAR CLIENTE
            var cliente = await _contexto.Clientes
                .FirstOrDefaultAsync(cliente =>
                    cliente.Ci == input.ci &&
                    ((cliente.Extension == null && extensionNormalizada == null) ||
                     (cliente.Extension ?? string.Empty) == extensionNormalizada)
                );

            if (cliente == null)
                return BadRequest("El cliente no existe. No se puede realizar la venta.");

            var venta = new Venta
            {
                Id = Guid.NewGuid(),
                Fecha = DateTime.Now,
                ClienteId = cliente.Id,
                DetalleVentas = new List<DetalleVenta>()
            };

            decimal totalSinDescuento = 0;
            var productosCache = new Dictionary<Guid, Producto>();

            foreach (var detalleInput in input.Detalle)
            {
                var producto = await _contexto.Productos.FindAsync(detalleInput.ProductoId);

                if (producto == null)
                    return BadRequest($"Producto con ID {detalleInput.ProductoId} no existe.");

                if (detalleInput.Cantidad <= 0)
                    return BadRequest($"La cantidad para el producto {producto.Nombre} debe ser mayor a cero.");

                if (producto.Stock < detalleInput.Cantidad)
                    return BadRequest($"Stock insuficiente para producto {producto.Nombre}. Disponible: {producto.Stock}");

                producto.Stock -= detalleInput.Cantidad;

                if (producto.Stock == 0)
                    producto.EsVigente = false;

                decimal subtotal = producto.Precio * detalleInput.Cantidad;
                totalSinDescuento += subtotal;

                venta.DetalleVentas.Add(new DetalleVenta
                {
                    Id = Guid.NewGuid(),
                    ProductoId = detalleInput.ProductoId,
                    Cantidad = detalleInput.Cantidad,
                    Precio = producto.Precio
                });

                productosCache[producto.Id] = producto;
            }

            var ventasCliente = await _contexto.Ventas
                .Where(v => v.ClienteId == cliente.Id)
                .Select(v => new
                {
                    Fecha = v.Fecha.Date,
                    TotalBruto = v.DetalleVentas.Sum(d => d.Precio * d.Cantidad)
                })
                .ToListAsync();

            var diasPorVenta = ventasCliente
                .GroupBy(v => v.Fecha)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.TotalBruto));

            var hoy = venta.Fecha.Date;
            if (diasPorVenta.ContainsKey(hoy))
                diasPorVenta[hoy] += totalSinDescuento;
            else
                diasPorVenta[hoy] = totalSinDescuento;

            var diasConCompraMayorADiez = diasPorVenta.Count(x => x.Value > 10m);
            var esFrecuente = cliente.EsFrecuente || diasConCompraMayorADiez >= 5;
            var porcentajeDescuento = esFrecuente ? 5m : 0m;

            venta.Total = Math.Round(totalSinDescuento * (1 - porcentajeDescuento / 100m), 2);

            cliente.EsFrecuente = esFrecuente;
            cliente.PorcentajeDescuento = porcentajeDescuento;

            _contexto.Ventas.Add(venta);
            await _contexto.SaveChangesAsync();

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
                Detalle = venta.DetalleVentas.Select(detalleVenta => new DetalleVentaOutput
                {
                    Nombre = productosCache.TryGetValue(detalleVenta.ProductoId, out var productoDetalle)
                        ? productoDetalle.Nombre
                        : string.Empty,
                    Cantidad = detalleVenta.Cantidad,
                    Precio = detalleVenta.Precio,
                    Subtotal = Math.Round((detalleVenta.Cantidad * detalleVenta.Precio) * (1 - cliente.PorcentajeDescuento / 100m), 2)
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
            [FromBody] ActualizarVentaInput ventaDto)
        {
            if (id == Guid.Empty)
                return BadRequest("El ID de la venta es inválido.");

            if (ventaDto.Total < 0)
                return BadRequest("El total de la venta no puede ser negativo.");

            if (ventaDto.ClienteId.HasValue)
            {
                var clienteExiste = await _contexto.Clientes.AnyAsync(c => c.Id == ventaDto.ClienteId.Value);
                if (!clienteExiste)
                    return BadRequest("El cliente especificado no existe.");
            }

            var ventaExistente = await _contexto.Ventas.FindAsync(id);

            if (ventaExistente == null)
                return NotFound();

            ventaExistente.Fecha = ventaDto.Fecha;
            ventaExistente.Total = ventaDto.Total;
            ventaExistente.ClienteId = ventaDto.ClienteId;

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

        private static string? AdecuarExtension(string? extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
                return null;

            return extension.Trim().ToUpperInvariant();
        }
    }
}