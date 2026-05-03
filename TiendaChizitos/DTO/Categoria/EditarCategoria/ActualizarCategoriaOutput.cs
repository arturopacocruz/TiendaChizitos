using System;

namespace TiendaChizitos.DTO.Categoria.EditarCategoria;

public class ActualizarCategoriaOutput
{
    public Guid Id { get; set; }
    public required string Nombre { get; set; }
}