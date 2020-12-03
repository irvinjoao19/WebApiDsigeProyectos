using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Vehiculo
    {
        public string codigo { get; set; }
        public string placa { get; set; }
        public string tipoVehiculo { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public string anio { get; set; }
        public string combustible { get; set; }
        public string condicion { get; set; }
        public string costo { get; set; }
        public string serie { get; set; }

        public int estado { get; set; }

        public List<VehiculoControl> control { get; set; }
        public List<VehiculoVales> registros { get; set; }

    }
}
