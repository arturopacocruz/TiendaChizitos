using System;

namespace TiendaChizitos.Entidades;

public class Categoria
{
    public Guid Id { get; set; }
    public required string Nombre { get; set; }

    // Navegación
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}