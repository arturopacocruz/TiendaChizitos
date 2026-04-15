using System;

namespace TiendaChizitos.Entidades;

public class Producto
{
    public Guid Id { get; set; }
    public required string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }

    public bool EsVigente { get; set; } = true;

    // FK
    public Guid CategoriaId { get; set; }

    // Navegación
    public Categoria Categoria { get; set; } =null!;
    public ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();
}