using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class OrdenDetalle
    {

        public int id { get; set; }
        public string materialId { get; set; }
        public string fecha { get; set; }
        public string proveedor { get; set; }
        public string razonSocial { get; set; }
        public decimal precio { get; set; }
    }
}
