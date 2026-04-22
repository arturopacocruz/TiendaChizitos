using System;

namespace TiendaComida.DTO.Venta.GenerarVenta;

public class GenerarVentaOutput
{
    public Guid VentaId { get; set; }
    public Guid Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }

    public ClienteVentaOutput Cliente { get; set; } = null!;

    public List<DetalleVentaOutput> Detalle { get; set; } = new();
}

public class ClienteVentaOutput
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public int Ci { get; set; }
    public string? Extension { get; set; }
    public bool EsFrecuente { get; set; }
    public decimal PorcentajeDescuento { get; set; }
}

public class DetalleVentaOutput
{
    public Guid ProductoId { get; set; }
    public string NombreProducto { get; set; } = null!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal SubTotal { get; set; }
}