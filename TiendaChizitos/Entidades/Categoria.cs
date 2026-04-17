using System;
using System.Text.Json.Serialization;

namespace TiendaChizitos.Entidades;

public class Categoria
{
    public Guid Id { get; set; }
    public required string Nombre { get; set; }

    // Navegación
    [JsonIgnore]
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}