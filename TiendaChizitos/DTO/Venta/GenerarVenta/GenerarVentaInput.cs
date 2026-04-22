using System;

namespace TiendaComida.DTO.Venta.GenerarVenta;

public class GenerarVentaInput
{
    public int ci { get; set; }

    public string? extension { get; set; }

    public List<DetalleVentaInput> Detalle { get; set; } = new();
}

public class DetalleVentaInput
{
    public Guid ProductoId { get; set; }
    public int Cantidad { get; set; }
}