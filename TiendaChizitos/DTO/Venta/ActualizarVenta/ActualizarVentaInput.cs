using System;

namespace TiendaChizitos.DTO.Venta.ActualizarVenta;

public class ActualizarVentaInput
{
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public Guid? ClienteId { get; set; }
}