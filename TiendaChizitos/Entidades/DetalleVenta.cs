using System;

namespace TiendaChizitos.Entidades;

public class DetalleVenta
{
    public Guid Id { get; set; }

    public Guid VentaId { get; set; }
    public Guid ProductoId { get; set; }

    public int Cantidad { get; set; }
    public decimal Precio { get; set; }

    // Navegación
    public Venta Venta { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}