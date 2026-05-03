using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaChizitos.DTO.Cliente
{
    public class AñadirClienteOUT
    {
        public Guid Id { get; set; }
        public int Ci { get; set; }
        public string? Extension { get; set; }
        public required string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public bool EsFrecuente { get; set; }
        public decimal PorcentajeDescuento { get; set; }
    }
}