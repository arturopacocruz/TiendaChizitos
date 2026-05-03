using System;

namespace TiendaChizitos.DTO.Producto.EditarProducto;

public class ActualizarProductoOutput
{
    public Guid Id { get; set; }
    public required string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public bool EsVigente { get; set; }
    public Guid? CategoriaId { get; set; }
}