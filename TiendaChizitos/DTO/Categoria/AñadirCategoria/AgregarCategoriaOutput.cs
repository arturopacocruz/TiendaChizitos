using System;

namespace TiendaChizitos.DTO.Categoria.AñadirCategoria;

public class AgregarCategoriaOutput
{
    public Guid Id { get; set; }
    public required string Nombre { get; set; }
}