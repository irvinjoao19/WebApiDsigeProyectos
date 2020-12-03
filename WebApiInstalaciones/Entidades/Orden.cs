using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Orden
    {
        public int id { get; set; }
        public int ordenId { get; set; }        
        public string tipoPedidoId { get; set; }
        public string toc { get; set; }
        public string nroOrden { get; set; }
        public string fechaEmisionOrden { get; set; }
        public string contableOt { get; set; }
        public string usuSolicitante { get; set; }
        public string usuCreaOrden { get; set; }
        public string fechaCreaOrden { get; set; }
        public string fechaAprobacion { get; set; }
        public string provee { get; set; }
        public string forma { get; set; }
        public string mone { get; set; }
        public decimal subtotalOc { get; set; }
        public decimal igv { get; set; }
        public decimal totalOc { get; set; }
        public string codEstado { get; set; }
        public string estado { get; set; }
        public int item { get; set; }
        public string articulo { get; set; }
        public string nombreArticulo { get; set; }
        public decimal cantidadAprobada { get; set; }
        public decimal precio { get; set; }
        public decimal importe { get; set; }
        public string delegacion { get; set; }
        public List<OrdenDetalle> detalles { get; set; }
    }
}
