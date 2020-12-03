using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class RegistroDetalle
    {
        public int detalleId { get; set; }
        public int registroId { get; set; }  
        public int tipo { get; set; }
        public string nombrePunto { get; set; }
        public decimal largo { get; set; }
        public decimal ancho { get; set; }
        public decimal totalM3 { get; set; }
        public string observacion { get; set; }
        public string foto1PuntoAntes { get; set; }
        public string foto2PuntoAntes { get; set; }
        public string foto3PuntoAntes { get; set; }
        public string foto1PuntoDespues { get; set; }
        public string foto2PuntoDespues { get; set; }
        public string foto3PuntoDespues { get; set; }
        public int estado { get; set; }
    }
}
