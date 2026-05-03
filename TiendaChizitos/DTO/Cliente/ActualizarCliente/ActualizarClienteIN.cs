using System;

namespace TiendaChizitos.DTO.Cliente.ActualizarCliente;

public class ActualizarClienteIN
{
    public int Ci { get; set; }
    public string? Extension { get; set; }
    public required string Nombre { get; set; }
    public DateTime FechaNacimiento { get; set; }
}