namespace TiendaChizitos.DTO.Venta.ListarVentas;

public class ListarVentasOutput
{
    public Guid Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public ClienteListarVentasOutput Cliente { get; set; } = null!;
    public List<DetalleListarVentasOutput> Detalle { get; set; } = new();
}

public class ClienteListarVentasOutput
{
    public string Nombre { get; set; } = string.Empty;
    public int Ci { get; set; }
    public string? Extension { get; set; }
    public decimal PorcentajeDescuento { get; set; }
}

public class DetalleListarVentasOutput
{
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }
    public decimal Subtotal { get; set; }
}