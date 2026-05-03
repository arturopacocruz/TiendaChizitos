using System;

namespace TiendaChizitos.DTO.Producto.AgregarProducto;

public class AgregarProductoInput
{
    public required string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public Guid? CategoriaId { get; set; }
    public bool EsVigente { get; set; } = true;
}