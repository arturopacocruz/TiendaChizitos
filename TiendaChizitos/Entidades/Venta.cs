using System;

namespace TiendaChizitos.Entidades;

public class Venta
{
    public Guid Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }

    // FK
    public Guid? ClienteId { get; set; }

    // Navegación
    public Cliente? Cliente { get; set; }
    public ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();
}