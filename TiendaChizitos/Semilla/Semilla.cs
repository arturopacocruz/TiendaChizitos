using System;
using TiendaChizitos.Data;
using TiendaChizitos.Entidades;

namespace TiendaChizitos.Semilla;

public class Semilla
{
    public static async Task Poblar(AppDbContext contexto)
    {
        if (contexto.Clientes.Any())
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


        // CLIENTES

        var clientes = new List<Cliente>()
        {
            new()
            {
                Id = Guid.Parse("3f9f0d1e-5c88-4d45-978d-7e4c1a1b0a01"),
                Ci = 1234567,
                Extension = "LP",
                Nombre = "Ana Pérez",
                FechaNacimiento = new DateTime(1990, 5, 10),
                EsFrecuente = false,
                PorcentajeDescuento = 0m
            },
            new()
            {
                Id = Guid.Parse("8d7c3a2b-1f44-4e29-9e5d-2f8b7e4d0c02"),
                Ci = 2345678,
                Extension = "CB",
                Nombre = "Juan Gómez",
                FechaNacimiento = new DateTime(1985, 9, 22),
                EsFrecuente = false,
                PorcentajeDescuento = 0m
            },
            new()
            {
                Id = Guid.Parse("b5a2d1f6-3c77-4a12-a8f3-0d6e5b2c0f03"),
                Ci = 3456789,
                Extension = "SC",
                Nombre = "María Torres",
                FechaNacimiento = new DateTime(1995, 2, 14),
                EsFrecuente = false,
                PorcentajeDescuento = 0m
            },
            new()
            {
                Id = Guid.Parse("f1e2d3c4-b5a6-4f78-9e0d-1a2b3c4d5e6f"),
                Ci = 4567890,
                Extension = "TJ",
                Nombre = "Carlos Rodríguez",
                FechaNacimiento = new DateTime(1988, 12, 5),
                EsFrecuente = true,
                PorcentajeDescuento = 5m
            }
        };

        contexto.Clientes.AddRange(clientes);


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

        contexto.Productos.AddRange(productos);

        await contexto.SaveChangesAsync();
    }
}