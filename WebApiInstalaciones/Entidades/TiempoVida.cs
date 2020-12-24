using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class TiempoVida
    {
        public int id { get; set; }
        public int idDet { get; set; }
        public string pubSucuCodigo { get; set; }
        public string almaCodigo { get; set; }
        public string obraCodigo { get; set; }
        public string pedcNumero { get; set; }
        public string pedcFechaEnvio { get; set; }
        public string pedcFolioOrigen { get; set; }
        public string nomApellidos { get; set; }
        public string pedcFechaCreada { get; set; }
        public string usuarioCrea { get; set; }
        public string nombreOt { get; set; }
        public string almaDescripcion { get; set; }
        public string nombreEstado { get; set; }
        public string articulo { get; set; }
        public string nombreArticulo { get; set; }
        public decimal cantidadPedida { get; set; }
        public decimal cantidadAprobada { get; set; }
        public string guicGuiaRemision { get; set; }
        public string guiaRemision { get; set; }
    }
}
