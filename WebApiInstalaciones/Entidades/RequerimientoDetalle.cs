using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class RequerimientoDetalle
    {
        public int detalleId { get; set; }
        public int requerimientoId { get; set; }
        public string material { get; set; }
        public string descripionMaterial { get; set; }
        public string um { get; set; }
        public decimal cantidad { get; set; }
        public int identity { get; set; }
    }
}
