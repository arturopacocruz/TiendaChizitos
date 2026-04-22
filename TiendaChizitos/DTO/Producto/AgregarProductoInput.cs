using System;
using System.Security.Authentication;

namespace TiendaChizitos.DTO.Producto.AgregarProducto;

public class AgregarProductoInput
{
    public required string Nombre { get; set; }
    public decimal precio { get; set; }
    public int stock { get; set; }

}