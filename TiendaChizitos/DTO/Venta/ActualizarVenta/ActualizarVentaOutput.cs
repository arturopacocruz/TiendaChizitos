using System;

namespace TiendaChizitos.DTO.Venta.ActualizarVenta;

public class ActualizarVentaOutput
{
    public Guid Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public Guid? ClienteId { get; set; }
}