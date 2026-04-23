using System.Text.Json.Serialization;

namespace TiendaComida.DTO.Venta.GenerarVenta;

public class GenerarVentaOutput
{
    public DateTime Fecha { get; set; }

    public decimal Total { get; set; }

    public ClienteVentaOutput Cliente { get; set; } = null!;

    public List<DetalleVentaOutput> Detalle { get; set; } = new();
}

public class ClienteVentaOutput
{
    public string Nombre { get; set; } = null!;

    public int Ci { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Extension { get; set; }

    public decimal PorcentajeDescuento { get; set; }
}

public class DetalleVentaOutput
{
    public string Nombre { get; set; } = null!;

    public int Cantidad { get; set; }

    public decimal Precio { get; set; }

    public decimal Subtotal { get; set; }
}