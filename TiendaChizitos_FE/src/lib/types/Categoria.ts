// --- ENTIDAD: CATEGORÍAS ---

// Modelo base que viene del GET listarCategorias
export interface Categoria {
    id: string; // Guid en C#
    nombre: string;
}

// DTO para búsqueda con productos (CategoriaConListaProductosOutput)
export interface CategoriaConListaProductos {
    id: string;
    nombre: string;
    productos: ProductoSimplificado[];
}

export interface ProductoSimplificado {
    id: string;
    nombre: string;
    precio: number;
    stock: number;
    esVigente: boolean;
}

// DTO para resumen (CategoriaConProductosOutput)
export interface CategoriaResumen {
    id: string;
    nombre: string;
    totalProductos: number;
}

// DTO para creación (AgregarCategoriaInput)
export interface AgregarCategoriaInput {
    nombre: string;
}

// DTO para actualización (ActualizarCategoriaInput)
export interface ActualizarCategoriaInput {
    nombre: string;
} 