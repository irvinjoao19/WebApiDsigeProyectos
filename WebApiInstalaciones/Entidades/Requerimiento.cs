using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Requerimiento
    {
        public int requerimientoId { get; set; }
        public string nroSolicitud { get; set; }
        public string tipoSolicitud { get; set; }
        public string fecha { get; set; }
        public string codigoDelegacion { get; set; }
        public string nombreDelegacion { get; set; }
        public string codigoCentroCosto { get; set; }
        public string nombreCentroCosto { get; set; }
        public string observaciones { get; set; }
        public string estado { get; set; }
        public int estadoId { get; set; }
        public int identity { get; set; }
        public string usuario { get; set; }
        public List<RequerimientoDetalle> detalle { get; set; }
    }
}
