// Entidad completa que devuelve la base de datos
export interface Cliente {
    id: string;
    ci: number;
    nombre: string;
    extension?: string;
    fechaNacimiento: string;
    esFrecuente: boolean;
    porcentajeDescuento: number;
}

// DTO para creación (AñadirClienteIN)
export interface AñadirClienteIN {
    ci: number;
    nombre: string;
    extension?: string;
    fechaNacimiento: string;
}

// DTO para actualización (ActualizarClienteIN)
export interface ActualizarClienteIN {
    ci: number;
    nombre: string;
    extension?: string;
    fechaNacimiento: string;
}

// DTO para búsqueda con el nuevo campo de extensión
export interface BuscarParams {
    nombre?: string;
    ci?: number;
    extension?: string;
}
