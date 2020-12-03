using System.Collections.Generic;

namespace Entidades
{
    public class Sync
    {
        public List<Vehiculo> vehiculos { get; set; }
        public List<ParametroT> parametrosT { get; set; }
        public List<Estado> estados { get; set; }
        public List<Almacen> almacens { get; set; }
        public List<ParametroE> parametrosE { get; set; }
        public List<ParteDiario> parteDiarios { get; set; }
        public List<Baremo> baremos { get; set; }
        public List<Materiales> materiales { get; set; }
        public List<Obra> obras { get; set; }
        public List<Articulo> articulos { get; set; }
        public List<Coordinador> coordinadors { get; set; }
        public List<Actividad> actividades { get; set; }
        public List<Medidor> medidores { get; set; }
        public List<TipoDevolucion> devoluciones { get; set; }
        public List<Personal> personals { get; set; }
        public List<Delegacion> delegaciones { get; set; }
        public List<RequerimientoMaterial> requerimientoMateriales { get; set; }
        public List<Estado> requerimientoEstado { get; set; }
        public List<Estado> requerimientoTipo { get; set; }
        public List<Estado> requerimientoCentroCostos { get; set; }
        public List<Estado> comboEstados { get; set; }
    }
}
