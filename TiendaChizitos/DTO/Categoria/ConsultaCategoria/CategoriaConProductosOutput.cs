using System;

namespace TiendaChizitos.DTO.Categoria.ConsultaCategoria;

public class CategoriaConProductosOutput
{
    public Guid Id { get; set; }
    public required string Nombre { get; set; }
    public int TotalProductos { get; set; }
}