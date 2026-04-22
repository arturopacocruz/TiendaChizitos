using System;

namespace TiendaChizitos.DTO.Producto.AgregarProducto;

public class AgregarProductoOutput
{
    public Guid Id { get; set; }
    public required string Nombre { get; set; }
    public decimal precio { get; set; }
    public int stock { get; set; }
}