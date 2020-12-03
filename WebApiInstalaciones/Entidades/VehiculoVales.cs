using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class VehiculoVales
    {
        public int valeId { get; set; }
        public string placa { get; set; }
        public string gesCodigo { get; set; }
        public string dni { get; set; }
        public string nroVale { get; set; }
        public string fecha { get; set; }
        public string tipo { get; set; }
        public decimal precioIGV { get; set; }
        public decimal cantidadGalones { get; set; }
        public decimal kmValeCombustible { get; set; }
        public string rucGrifo { get; set; }
        public string pubCodigo { get; set; }
        public string foto { get; set; }
    }
}
