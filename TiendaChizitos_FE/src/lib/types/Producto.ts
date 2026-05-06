export interface Producto {
    id: string;
    nombre: string;
    precio: number;
    stock: number;
    esVigente: boolean;
    categoriaId?: string;
    categoria?: {
        id: string;
        nombre: string;
    };
}

export interface AgregarProductoInput {
    nombre: string;
    precio: number;
    stock: number;
    esVigente: boolean;
    categoriaId?: string;
}

export interface ActualizarProductoInput {
    nombre: string;
    precio: number;
    stock: number;
    esVigente: boolean;
    categoriaId?: string;
} 