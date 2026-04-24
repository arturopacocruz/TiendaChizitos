using System;
using TiendaChizitos.Data;
using TiendaChizitos.Entidades;

namespace TiendaChizitos.Semilla;

public class Semilla
{
    public static async Task Poblar(AppDbContext contexto)
    {
        if (contexto.Productos.Any())
            return;


        // CATEGORIAS

        var bebidas = new Categoria
        {
            Id = Guid.NewGuid(),
            Nombre = "Bebidas"
        };

        var snacks = new Categoria
        {
            Id = Guid.NewGuid(),
            Nombre = "Snacks"
        };

        var dulces = new Categoria
        {
            Id = Guid.NewGuid(),
            Nombre = "Dulces"
        };

        var lacteos = new Categoria
        {
            Id = Guid.NewGuid(),
            Nombre = "Lácteos"
        };


        var categorias = new List<Categoria>()
        {
            bebidas,
            snacks,
            dulces,
            lacteos
        };

        contexto.Categorias.AddRange(categorias);



        // PRODUCTOS

        var productos = new List<Producto>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Coca Cola 3 Litros",
                Precio = 15,
                Stock = 30,
                EsVigente = true,
                CategoriaId = bebidas.Id
            },

            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Pepsi 2 Litros",
                Precio = 12,
                Stock = 20,
                EsVigente = true,
                CategoriaId = bebidas.Id
            },

            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Fanta",
                Precio = 10,
                Stock = 15,
                EsVigente = true,
                CategoriaId = bebidas.Id
            },

            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Papas Lays",
                Precio = 8,
                Stock = 25,
                EsVigente = true,
                CategoriaId = snacks.Id
            },

            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Chizitos",
                Precio = 5,
                Stock = 40,
                EsVigente = true,
                CategoriaId = snacks.Id
            },

            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Doritos",
                Precio = 9,
                Stock = 18,
                EsVigente = true,
                CategoriaId = snacks.Id
            },

            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Chocolate",
                Precio = 4,
                Stock = 35,
                EsVigente = true,
                CategoriaId = dulces.Id
            },

            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Bon Bon Bum",
                Precio = 2,
                Stock = 60,
                EsVigente = true,
                CategoriaId = dulces.Id
            },

            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Yogurt Pil",
                Precio = 7,
                Stock = 10,
                EsVigente = true,
                CategoriaId = lacteos.Id
            },

            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Leche Pil",
                Precio = 9,
                Stock = 22,
                EsVigente = true,
                CategoriaId = lacteos.Id
            },

            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Queso",
                Precio = 14,
                Stock = 8,
                EsVigente = true,
                CategoriaId = lacteos.Id
            }
        };
        // Clientes
        if (contexto.Clientes.Any()) return ; // Si ya hay datos, no hacemos nada
            var clientes = new List<Cliente>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Ci = 12345678, 
                    Extension = "A1",
                    Nombre = "JUAN PEREZ",
                    FechaNacimiento = new DateTime(1990, 1, 1),
                    EsFrecuente = true,
                    PorcentajeDescuento = 15
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Ci = 87654321, 
                    Extension = "",
                    Nombre = "MARIA GOMEZ",
                    FechaNacimiento = new DateTime(1985, 5, 20),
                    EsFrecuente = false,
                    PorcentajeDescuento = 0
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Ci = 11223344, 
                    Extension = "B2",
                    Nombre = "CARLOS LOPEZ",
                    FechaNacimiento = new DateTime(1995, 3, 15),
                    EsFrecuente = true,
                    PorcentajeDescuento = 10
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Ci = 44332211, 
                    Extension = "C3",
                    Nombre = "ANA MARTINEZ",
                    FechaNacimiento = new DateTime(1988, 7, 30),
                    EsFrecuente = false,
                    PorcentajeDescuento = 0
                },
            };
        contexto.Clientes.AddRange(clientes);
        contexto.Productos.AddRange(productos);

        await contexto.SaveChangesAsync();
    }
}