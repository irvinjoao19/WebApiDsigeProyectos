using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Registro
    {
        public int registroId { get; set; }
        public int tipo { get; set; }
        public string nroObra { get; set; }
        public string nroPoste { get; set; }
        public string foto { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string usuarioId { get; set; }
        public string estado { get; set; }
        public string fecha { get; set; }
        public int active { get; set; }
        public int identity { get; set; }
        public List<RegistroDetalle> list { get; set; }
    }
}
