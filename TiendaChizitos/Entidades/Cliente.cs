using System;

namespace TiendaChizitos.Entidades;

public class Cliente
{
    public Guid Id { get; set; }
    public int Ci { get; set; }
    public string? Extension { get; set; }
    public required string Nombre { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public bool EsFrecuente { get; set; } = false;
    public decimal PorcentajeDescuento { get; set; } = 0;

    // Navegación
    public ICollection<Venta> Ventas { get; set;} = new List<Venta>();
}