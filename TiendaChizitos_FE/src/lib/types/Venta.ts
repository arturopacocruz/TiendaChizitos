// --- ENTIDAD: VENTAS ---

export interface Venta {
    id: string;
    fecha: string;
    total: number;
    cliente: ClienteVentaOutput;
    detalle: DetalleVentaOutput[];
}

export interface ClienteVentaOutput {
    nombre: string;
    ci: number;
    extension?: string;
    porcentajeDescuento: number;
}

export interface DetalleVentaOutput {
    nombre: string;
    cantidad: number;
    precio: number;
    subtotal: number;
}

// DTO para generar venta (POST)
export interface GenerarVentaInput {
    ci: number;
    extension?: string;
    detalle: GenerarVentaDetalleInput[];
}

export interface GenerarVentaDetalleInput {
    productoId: string;
    cantidad: number;
} 