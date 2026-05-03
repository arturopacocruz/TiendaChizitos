using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaChizitos.DTO.Cliente
{
    public class AñadirClienteIN
    {
        public int Ci { get; set; }
        public string? Extension { get; set; }
        public required string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }
}