using System;

namespace TiendaChizitos.DTO.Categoria.ConsultaCategoria;

public class CategoriaConListaProductosOutput
{
    public Guid Id { get; set; }
    public required string Nombre { get; set; }
    public required ICollection<ProductoOutput> Productos { get; set; }
}

public class ProductoOutput
{
    public Guid Id { get; set; }
    public required string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public bool EsVigente { get; set; }
}