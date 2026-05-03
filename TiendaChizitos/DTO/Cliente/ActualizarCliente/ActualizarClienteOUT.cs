using System;

namespace TiendaChizitos.DTO.Cliente.ActualizarCliente;

public class ActualizarClienteOUT
{
    public Guid Id { get; set; }
    public int Ci { get; set; }
    public string? Extension { get; set; }
    public required string Nombre { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public bool EsFrecuente { get; set; }
    public decimal PorcentajeDescuento { get; set; }
}