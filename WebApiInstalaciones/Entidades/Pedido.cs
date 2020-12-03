using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Pedido
    {
        public int id { get; set; }
        public int pedidoId { get; set; }
        public string tipoPedidoId { get; set; }
        public string nombreTipoPedido { get; set; }
        public string nroPedido { get; set; }
        public string fechaEnvio { get; set; }
        public string delegacion { get; set; }
        public string ccId { get; set; }
        public string centroCostos { get; set; }
        public string dni { get; set; }
        public string nombreEmpleado { get; set; }
        public string pubEstaCodigo { get; set; }
        public string estado { get; set; }
        public string moneda { get; set; }
        public int item { get; set; }
        public string articulo { get; set; }
        public string nombreArticulo { get; set; }
        public decimal cantidad { get; set; }
        public decimal cantidadAprobada { get; set; }
        public decimal precio { get; set; }
    }
}
