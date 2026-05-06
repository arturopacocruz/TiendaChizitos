export interface Cliente {
  id: string;                  // El GUID que viene de SQL Server
  ci: number;                  // Carnet de Identidad
  extension: string;           // Ej: "B2"
  nombre: string;              // Nombre completo
  fechaNacimiento: string;     // Fecha en formato ISO
  esFrecuente: boolean;        // Lógica para el Chip de MUI
  porcentajeDescuento: number; // Valor numérico
  ventas: any[];               // Arreglo de ventas (puedes tiparlo después)
}