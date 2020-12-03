using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class VehiculoControl
    {
        public int controlId { get; set; }
        public string placa { get; set; } // fk
        public string fecha { get; set; }
        public decimal kmIngreso { get; set; }
        public decimal kmSalida { get; set; }
        public string choferDni { get; set; }
        public string gesCodigo { get; set; }
        public string pubCodigo { get; set; }        
    }
}
