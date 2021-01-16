using Entidades;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio
{
    public class NegocioDao
    {
        private static string db = ConfigurationManager.ConnectionStrings["conexionDsige"].ConnectionString;

        public static Usuario GetOne(Query q)
        {
            try
            {
                Usuario u = null;
                using (SqlConnection cn = new SqlConnection(db))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("Movil_GetUsuario_C", cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@usuario", SqlDbType.VarChar).Value = q.login;

                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            u = new Usuario();
                            if (EncriptarClave(q.pass, true) == dr.GetString(1))
                            {
                                u.usuarioId = dr.GetString(0);
                                u.nombre = dr.GetString(2);
                                u.dniCuadrillaId = dr.GetString(3);
                                u.cuadrillaId = dr.GetString(4);
                                u.mensaje = "Go";

                                Filtro f = new Filtro();

                                // Sucursal
                                SqlCommand cmdS = cn.CreateCommand();
                                cmdS.CommandTimeout = 0;
                                cmdS.CommandType = CommandType.StoredProcedure;
                                cmdS.CommandText = "Movil_GetSucursales_C";
                                cmdS.Parameters.Add("@usuario", SqlDbType.VarChar).Value = u.usuarioId;
                                SqlDataReader drS = cmdS.ExecuteReader();
                                if (drS.HasRows)
                                {
                                    List<Sucursal> sucursales = new List<Sucursal>();
                                    while (drS.Read())
                                    {
                                        sucursales.Add(new Sucursal()
                                        {
                                            codigo = drS.GetString(0),
                                            nombre = drS.GetString(1)
                                        });
                                    }
                                    f.sucursales = sucursales;
                                }

                                // Areas
                                SqlCommand cmdArea = cn.CreateCommand();
                                cmdArea.CommandTimeout = 0;
                                cmdArea.CommandType = CommandType.StoredProcedure;
                                cmdArea.CommandText = "Movil_GetArea_C";
                                cmdArea.Parameters.Add("@usuario", SqlDbType.VarChar).Value = u.usuarioId;
                                SqlDataReader drArea = cmdArea.ExecuteReader();
                                if (drArea.HasRows)
                                {
                                    List<Area> area = new List<Area>();
                                    while (drArea.Read())
                                    {
                                        area.Add(new Area()
                                        {
                                            areaId = drArea.GetString(0),
                                            descripcion = drArea.GetString(1),
                                            estado = drArea.GetString(2),
                                            manual = drArea.GetString(3),
                                            medidorEncontrado = drArea.GetString(4),
                                            medidorInstalado = drArea.GetString(5)
                                        });
                                    }
                                    f.areas = area;
                                }

                                // Centro de Costos
                                SqlCommand cmdCentro = cn.CreateCommand();
                                cmdCentro.CommandTimeout = 0;
                                cmdCentro.CommandType = CommandType.StoredProcedure;
                                cmdCentro.CommandText = "Movil_GetCentroCostos_C";
                                cmdCentro.Parameters.Add("@codigoUsuario", SqlDbType.VarChar).Value = u.usuarioId;

                                SqlDataReader drCentro = cmdCentro.ExecuteReader();
                                if (drCentro.HasRows)
                                {
                                    List<CentroCostos> centros = new List<CentroCostos>();
                                    while (drCentro.Read())
                                    {
                                        CentroCostos c = new CentroCostos();
                                        c.centroId = drCentro.GetString(0);
                                        c.orden = drCentro.GetString(1);
                                        c.descripcion = drCentro.GetString(2);
                                        c.sucursalId = drCentro.GetString(3);
                                        c.nombreSucursal = drCentro.GetString(4);

                                        //SqlCommand cmdCuadrilla = cn.CreateCommand();
                                        //cmdCuadrilla.CommandTimeout = 0;
                                        //cmdCuadrilla.CommandType = CommandType.StoredProcedure;
                                        //cmdCuadrilla.CommandText = "Movil_GetCuadrillas";
                                        //cmdCuadrilla.Parameters.Add("@centroCostoId", SqlDbType.VarChar).Value = centro.orden;
                                        //cmdCuadrilla.Parameters.Add("@codigoUsuario", SqlDbType.VarChar).Value = u.usuarioId;

                                        //SqlDataReader drCuadrilla = cmdCuadrilla.ExecuteReader();
                                        //if (drCuadrilla.HasRows)
                                        //{
                                        //    List<Cuadrilla> cuadrilla = new List<Cuadrilla>();
                                        //    while (drCuadrilla.Read())
                                        //    {
                                        //        cuadrilla.Add(new Cuadrilla()
                                        //        {
                                        //            orden = drCuadrilla.GetString(0),
                                        //            centroId = drCuadrilla.GetString(1),
                                        //            cuadrillaId = drCuadrilla.GetString(2),
                                        //            descripcion = drCuadrilla.GetString(3),
                                        //            dni = drCuadrilla.GetString(4)
                                        //        });
                                        //    }
                                        //    centro.cuadrillas = cuadrilla;
                                        //}
                                        centros.Add(c);
                                    }
                                    f.centros = centros;
                                }
                                u.filtro = f;
                            }
                            else
                            {
                                u.mensaje = "Pass";
                            }
                        }
                    }
                    cn.Close();
                }
                return u;

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public static string EncriptarClave(string cExpresion, bool bEncriptarCadena)
        {
            string cResult = "";
            string cPatron = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdefghijklmnopqrstuvwwyz";
            string cEncrip = "^çºªæÆöûÿø£Ø×ƒ¬½¼¡«»ÄÅÉêèï7485912360^çºªæÆöûÿø£Ø×ƒ¬½¼¡«»ÄÅÉêèï";


            if (bEncriptarCadena == true)
            {
                cResult = CHRTRAN(cExpresion, cPatron, cEncrip);
            }
            else
            {
                cResult = CHRTRAN(cExpresion, cEncrip, cPatron);
            }

            return cResult;

        }

        public static string CHRTRAN(string cExpresion, string cPatronBase, string cPatronReemplazo)
        {
            string cResult = "";

            int rgChar;
            int nPosReplace;

            for (rgChar = 1; rgChar <= Strings.Len(cExpresion); rgChar++)
            {
                nPosReplace = Strings.InStr(1, cPatronBase, Strings.Mid(cExpresion, rgChar, 1));

                if (nPosReplace == 0)
                {
                    nPosReplace = rgChar;
                    cResult = cResult + Strings.Mid(cExpresion, nPosReplace, 1);
                }
                else
                {
                    if (nPosReplace > cPatronReemplazo.Length)
                    {
                        nPosReplace = rgChar;
                        cResult = cResult + Strings.Mid(cExpresion, nPosReplace, 1);
                    }
                    else
                    {
                        cResult = cResult + Strings.Mid(cPatronReemplazo, nPosReplace, 1);
                    }
                }
            }
            return cResult;
        }

        public static Sync GetSyncComplete(Query q)
        {
            try
            {
                Sync s = new Sync();

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();

                    //SqlCommand cmdV = con.CreateCommand();
                    //cmdV.CommandTimeout = 0;
                    //cmdV.CommandType = CommandType.StoredProcedure;
                    //cmdV.CommandText = "Movil_GetVehiculos";
                    ////cmdV.Parameters.Add("@usuario", SqlDbType.VarChar).Value = q.usuarioId;
                    //SqlDataReader drV = cmdV.ExecuteReader();
                    //if (drV.HasRows)
                    //{
                    //    List<Vehiculo> v = new List<Vehiculo>();
                    //    while (drV.Read())
                    //    {
                    //        v.Add(new Vehiculo()
                    //        {
                    //            codigo = drV.GetString(0),
                    //            placa = drV.GetString(1),
                    //            tipoVehiculo = drV.GetString(2),
                    //            marca = drV.GetString(3),
                    //            modelo = drV.GetString(4),
                    //            anio = drV.GetString(5),
                    //            combustible = drV.GetString(6),
                    //            condicion = drV.GetString(7),
                    //            costo = drV.GetString(8),
                    //            serie = drV.GetString(9),
                    //            estado = 0
                    //
                    //        });
                    //    }
                    //    s.vehiculos = v;
                    //}
                    //
                    //SqlCommand cmdC = con.CreateCommand();
                    //cmdC.CommandTimeout = 0;
                    //cmdC.CommandType = CommandType.StoredProcedure;
                    //cmdC.CommandText = "Movil_GetCombos";
                    //SqlDataReader drC = cmdC.ExecuteReader();
                    //if (drC.HasRows)
                    //{
                    //    List<ParametroT> p = new List<ParametroT>();
                    //    while (drC.Read())
                    //    {
                    //        p.Add(new ParametroT()
                    //        {
                    //            tipo = drC.GetInt32(0),
                    //            campo1 = drC.GetString(1),
                    //            campo2 = drC.GetString(2)
                    //        });
                    //    }
                    //    s.parametrosT = p;
                    //}
                    //
                    //SqlCommand cmdE = con.CreateCommand();
                    //cmdE.CommandTimeout = 0;
                    //cmdE.CommandType = CommandType.StoredProcedure;
                    //cmdE.CommandText = "Movil_GetEstados";
                    //SqlDataReader drE = cmdE.ExecuteReader();
                    //if (drE.HasRows)
                    //{
                    //    List<Estado> e = new List<Estado>();
                    //    while (drE.Read())
                    //    {
                    //        e.Add(new Estado()
                    //        {
                    //            codigo = drE.GetString(0),
                    //            nombre = drE.GetString(1)
                    //        });
                    //    }
                    //    s.estados = e;
                    //}
                    //
                    //SqlCommand cmdAlm = con.CreateCommand();
                    //cmdAlm.CommandTimeout = 0;
                    //cmdAlm.CommandType = CommandType.StoredProcedure;
                    //cmdAlm.CommandText = "Movil_GetAlmacenes";
                    //cmdAlm.Parameters.Add("@sucursal", SqlDbType.VarChar).Value = q.sucursalId;
                    //cmdAlm.Parameters.Add("@usuario", SqlDbType.VarChar).Value = q.usuarioId;
                    //SqlDataReader drAl = cmdAlm.ExecuteReader();
                    //if (drAl.HasRows)
                    //{
                    //    List<Almacen> alm = new List<Almacen>();
                    //    while (drAl.Read())
                    //    {
                    //        alm.Add(new Almacen()
                    //        {
                    //            codigo = drAl.GetString(0),
                    //            tipoCodigo = drAl.GetString(1),
                    //            sucursalCodigo = drAl.GetString(2),
                    //            descripcion = drAl.GetString(3),
                    //            opCodigo = drAl.GetString(4),
                    //            inventario = drAl.GetString(5),
                    //            equipamiento = drAl.GetString(6),
                    //            seraEdelnor = drAl.GetString(7)
                    //        });
                    //    }
                    //    s.almacens = alm;
                    //}
                    //
                    //SqlCommand cmdParametro = con.CreateCommand();
                    //cmdParametro.CommandTimeout = 0;
                    //cmdParametro.CommandType = CommandType.StoredProcedure;
                    //cmdParametro.CommandText = "Movil_GetParametros";
                    //SqlDataReader drParametro = cmdParametro.ExecuteReader();
                    //if (drParametro.HasRows)
                    //{
                    //    List<ParametroE> parametro = new List<ParametroE>();
                    //    while (drParametro.Read())
                    //    {
                    //        parametro.Add(new ParametroE()
                    //        {
                    //            id_Configuracion = drParametro.GetInt32(0),
                    //            nombre_parametro = drParametro.GetString(1),
                    //            valor = drParametro.GetInt32(2)
                    //        });
                    //    }
                    //    s.parametrosE = parametro;
                    //}
                    //
                    //// Parte Diario
                    //
                    //SqlCommand cmdP = con.CreateCommand();
                    //cmdP.CommandTimeout = 0;
                    //cmdP.CommandType = CommandType.StoredProcedure;
                    //cmdP.CommandText = "Movil_GetParteDiario_C";
                    //cmdP.Parameters.Add("@Area", SqlDbType.VarChar).Value = q.areaId;
                    //cmdP.Parameters.Add("@UsuarioId", SqlDbType.VarChar).Value = q.usuarioId;
                    //cmdP.Parameters.Add("@CentroCostoId", SqlDbType.VarChar).Value = q.centroCostoId;
                    //SqlDataReader drP = cmdP.ExecuteReader();
                    //if (drP.HasRows)
                    //{
                    //    List<ParteDiario> parte = new List<ParteDiario>();
                    //    while (drP.Read())
                    //    {
                    //        var p = new ParteDiario();
                    //        p.parteDiarioId = drP.GetInt32(0);
                    //        p.identity = drP.GetInt32(0);
                    //        p.fecha = drP.GetDateTime(1).ToString("dd/MM/yyyy");
                    //        p.obraTd = drP.GetString(2);
                    //        p.codigoEstadoPd = drP.GetString(3);
                    //        p.estadoTd = drP.GetString(4);
                    //        p.codigoEstadoObra = drP.GetString(5);
                    //        p.estadoObra = drP.GetString(6);
                    //        p.descripcion = drP.GetString(7);
                    //        p.direccion = drP.GetString(8);
                    //        p.cliente = drP.GetString(9);
                    //        p.fechaAsignacion = drP.GetDateTime(10).ToString("dd/MM/yyyy");
                    //        p.fechaVencimiento = drP.GetDateTime(11).ToString("dd/MM/yyyy");
                    //        p.suministro = drP.GetString(12);
                    //        p.sed = drP.GetString(13);
                    //        p.observacion = drP.GetString(14);
                    //        p.empresaCodigo = drP.GetString(15);
                    //        p.areaCodigo = drP.GetString(16);
                    //        p.codigoInterno = drP.GetString(17);
                    //        p.costoCodigo = drP.GetString(18);
                    //        p.cuadrillaCodigo = drP.GetString(19);
                    //        p.dniCuadrilla = drP.GetString(20);
                    //        p.estadoCodigo = drP.GetString(21);
                    //        p.usuarioCreacion = drP.GetString(22);
                    //        p.fechaMovil = drP.GetDateTime(23).ToString("dd/MM/yyyy HH:mm:ss");
                    //        p.sucursalId = drP.GetString(24);
                    //        p.latitud = drP.GetString(25);
                    //        p.longitud = drP.GetString(26);
                    //        p.coordinadorDni = drP.GetString(27);
                    //        p.descripcionCoordinador = drP.GetString(28);
                    //        p.firmaMovil = drP.GetString(29);
                    //        p.tipo = 1;
                    //        p.estado = 1;
                    //        p.medencontrado_nro = "";
                    //        p.medencontrado_marca = "";
                    //        p.medencontrado_fase = "";
                    //        p.medencontrado_estado = "";
                    //        p.medencontrado_modelo = "";
                    //        p.medinstalado_nro = "";
                    //        p.medinstalado_marca = "";
                    //        p.medinstalado_fase = "";
                    //        p.medinstalado_estado = "";
                    //        p.medinstalado_modelo = "";
                    //
                    //        // PARTE DIARIO BAREMO
                    //
                    //        SqlCommand cmdPB = con.CreateCommand();
                    //        cmdPB.CommandTimeout = 0;
                    //        cmdPB.CommandType = CommandType.StoredProcedure;
                    //        cmdPB.CommandText = "Movil_GetParteDiarioBaremo_C";
                    //        cmdPB.Parameters.Add("@parteDiarioId", SqlDbType.VarChar).Value = p.parteDiarioId;
                    //
                    //        SqlDataReader dmPB = cmdPB.ExecuteReader();
                    //        if (dmPB.HasRows)
                    //        {
                    //            List<RegistroBaremo> rb = new List<RegistroBaremo>();
                    //
                    //            while (dmPB.Read())
                    //            {
                    //                rb.Add(new RegistroBaremo()
                    //                {
                    //                    registroBaremoId = dmPB.GetInt32(0),
                    //                    identityDetalle = dmPB.GetInt32(0),
                    //                    parteDiarioId = dmPB.GetInt32(1),
                    //                    identity = dmPB.GetInt32(1),
                    //                    codigoBaremo = dmPB.GetString(2),
                    //                    cantidadMovil = dmPB.GetDecimal(3),
                    //                    cantidadOk = dmPB.GetDecimal(4),
                    //                    fecha = dmPB.GetDateTime(5).ToString("dd/MM/yyyy"),
                    //                    tipo = 1,
                    //                    descripcion = dmPB.GetString(6),
                    //                    abreviatura = dmPB.GetString(7),
                    //                    unidadMedida = dmPB.GetString(8),
                    //                    estado = p.estadoCodigo
                    //                });
                    //            }
                    //            p.baremos = rb;
                    //        }
                    //
                    //        // PARTE DIARIO MATERIALES
                    //
                    //        SqlCommand cmdPM = con.CreateCommand();
                    //        cmdPM.CommandTimeout = 0;
                    //        cmdPM.CommandType = CommandType.StoredProcedure;
                    //        cmdPM.CommandText = "Movil_GetParteDiarioMaterial_C";
                    //        cmdPM.Parameters.Add("@parteDiarioId", SqlDbType.VarChar).Value = p.parteDiarioId;
                    //
                    //        SqlDataReader dmPM = cmdPM.ExecuteReader();
                    //        if (dmPM.HasRows)
                    //        {
                    //            List<RegistroMaterial> rm = new List<RegistroMaterial>();
                    //
                    //            while (dmPM.Read())
                    //            {
                    //                rm.Add(new RegistroMaterial()
                    //                {
                    //                    registroMaterialId = dmPM.GetInt32(0),
                    //                    identityDetalle = dmPM.GetInt32(0),
                    //                    identity = dmPM.GetInt32(1),
                    //                    parteDiarioId = dmPM.GetInt32(1),
                    //                    tipoMaterial = dmPM.GetInt32(2),
                    //                    almacenId = dmPM.GetString(3),
                    //                    codigoMaterial = dmPM.GetString(4),
                    //                    cantidadMovil = dmPM.GetDecimal(5),
                    //                    cantidadOk = dmPM.GetDecimal(6),
                    //                    fecha = dmPM.GetDateTime(7).ToString("dd/MM/yyyy"),
                    //                    tipo = 1,
                    //                    descripcion = dmPM.GetString(8),
                    //                    abreviatura = dmPM.GetString(9),
                    //                    unidadMedida = dmPM.GetString(10),
                    //                    guiaSalida = dmPM.GetString(11),
                    //                    nroSerie = dmPM.GetString(12),
                    //                    estado = p.estadoCodigo
                    //                });
                    //            }
                    //            p.materiales = rm;
                    //        }
                    //
                    //        // PARTE DIARIO PHOTOS
                    //
                    //        SqlCommand cmdPP = con.CreateCommand();
                    //        cmdPP.CommandTimeout = 0;
                    //        cmdPP.CommandType = CommandType.StoredProcedure;
                    //        cmdPP.CommandText = "Movil_GetParteDiarioFoto_C";
                    //        cmdPP.Parameters.Add("@parteDiarioId", SqlDbType.VarChar).Value = p.parteDiarioId;
                    //
                    //        SqlDataReader dmPP = cmdPP.ExecuteReader();
                    //        if (dmPP.HasRows)
                    //        {
                    //            List<RegistroPhoto> rp = new List<RegistroPhoto>();
                    //
                    //            while (dmPP.Read())
                    //            {
                    //                rp.Add(new RegistroPhoto()
                    //                {
                    //                    registroPhotoId = dmPP.GetInt32(0),
                    //                    identityFoto = dmPP.GetInt32(0),
                    //                    identity = dmPP.GetInt32(1),
                    //                    parteDiarioId = dmPP.GetInt32(1),
                    //                    nombre = dmPP.GetString(2),
                    //                    fecha = dmPP.GetDateTime(3).ToString("dd/MM/yyyy"),
                    //                    tipo = 1
                    //                });
                    //            }
                    //            p.photos = rp;
                    //        }
                    //        parte.Add(p);
                    //    }
                    //    s.parteDiarios = parte;
                    //}
                    //
                    //// Baremos
                    //
                    //SqlCommand cmdB = con.CreateCommand();
                    //cmdB.CommandTimeout = 0;
                    //cmdB.CommandType = CommandType.StoredProcedure;
                    //cmdB.CommandText = "Movil_GetBaremos_C";
                    //cmdB.Parameters.Add("@area", SqlDbType.VarChar).Value = q.areaId;
                    //cmdB.Parameters.Add("@cc", SqlDbType.VarChar).Value = q.centroCostoId;
                    //cmdB.Parameters.Add("@usuario", SqlDbType.VarChar).Value = q.usuarioId;
                    //SqlDataReader drB = cmdB.ExecuteReader();
                    //if (drB.HasRows)
                    //{
                    //    List<Baremo> baremo = new List<Baremo>();
                    //    while (drB.Read())
                    //    {
                    //        baremo.Add(new Baremo()
                    //        {
                    //            baremoId = drB.GetString(0),
                    //            descripcion = drB.GetString(1),
                    //            unidadMedida = drB.GetString(2),
                    //            abreviatura = drB.GetString(3),
                    //            actividadId = drB.GetInt32(4)
                    //        });
                    //    }
                    //    s.baremos = baremo;
                    //}
                    //
                    //// Materiales
                    //
                    //SqlCommand cmdM = con.CreateCommand();
                    //cmdM.CommandTimeout = 0;
                    //cmdM.CommandType = CommandType.StoredProcedure;
                    //cmdM.CommandText = "Movil_GetMateriales_C";
                    //cmdM.Parameters.Add("@area", SqlDbType.VarChar).Value = q.areaId;
                    //cmdM.Parameters.Add("@cc", SqlDbType.VarChar).Value = q.centroCostoId;
                    //cmdM.Parameters.Add("@usuario", SqlDbType.VarChar).Value = q.usuarioId;
                    //SqlDataReader drM = cmdM.ExecuteReader();
                    //if (drM.HasRows)
                    //{
                    //    List<Materiales> m = new List<Materiales>();
                    //    while (drM.Read())
                    //    {
                    //        m.Add(new Materiales()
                    //        {
                    //            id = drM.GetInt32(0),
                    //            tipoMaterial = drM.GetString(1),
                    //            materialId = drM.GetString(2),
                    //            descripcion = drM.GetString(3),
                    //            unidadMedida = drM.GetString(4),
                    //            abreviatura = drM.GetString(5),
                    //            stock = drM.GetDecimal(6),
                    //            obra = drM.GetString(7),
                    //            cc = drM.GetString(8),
                    //            almacenId = drM.GetString(9),
                    //            guiaSalida = drM.GetString(10),
                    //            exigeSerie = drM.GetString(11),
                    //            tipo = 1,
                    //            fecha = "",
                    //            guiaIngreso = drM.GetString(12),
                    //            guiaIngresoId = drM.GetString(13)
                    //        });
                    //    }
                    //    s.materiales = m;
                    //}
                    //
                    //// Obras TD
                    //
                    //SqlCommand cmdO = con.CreateCommand();
                    //cmdO.CommandTimeout = 0;
                    //cmdO.CommandType = CommandType.StoredProcedure;
                    //cmdO.CommandText = "Movil_GetObrasTd";
                    //cmdO.Parameters.Add("@usuarioId", SqlDbType.VarChar).Value = q.usuarioId;
                    //cmdO.Parameters.Add("@centroCostoId", SqlDbType.VarChar).Value = q.centroCostoId;
                    //SqlDataReader drO = cmdO.ExecuteReader();
                    //if (drO.HasRows)
                    //{
                    //    List<Obra> o = new List<Obra>();
                    //    while (drO.Read())
                    //    {
                    //        o.Add(new Obra()
                    //        {
                    //            obraId = drO.GetString(0),
                    //            descripcion = drO.GetString(1),
                    //            estado = drO.GetString(2),
                    //            direccion = drO.GetString(3),
                    //            cliente = drO.GetString(4),
                    //            fechaAsignacion = drO.GetDateTime(5).ToString("dd/MM/yyyy"),
                    //            fechaVencimiento = drO.GetDateTime(6).ToString("dd/MM/yyyy"),
                    //            usuarioCreacion = drO.GetString(7)
                    //        });
                    //    }
                    //    s.obras = o;
                    //}
                    //
                    //
                    //// Delegacion
                    //
                    SqlCommand cmdDE = con.CreateCommand();
                    cmdDE.CommandTimeout = 0;
                    cmdDE.CommandType = CommandType.StoredProcedure;
                    cmdDE.CommandText = "Dsige_Proy_M_Delegacion";
                    SqlDataReader drDE = cmdDE.ExecuteReader();
                    if (drDE.HasRows)
                    {
                        List<Delegacion> a = new List<Delegacion>();
                        while (drDE.Read())
                        {
                            a.Add(new Delegacion()
                            {
                                codigo = drDE.GetString(0),
                                descripcion = drDE.GetString(1),
                            });
                        }
                        s.delegaciones = a;
                    }
                    //
                    //// Requerimiento Material -- Logistica
                    //
                    SqlCommand cmdR = con.CreateCommand();
                    cmdR.CommandTimeout = 0;
                    cmdR.CommandType = CommandType.StoredProcedure;
                    cmdR.CommandText = "DSIGE_Movil_Traer_Articulos";
                    cmdR.Parameters.Add("@TipoSolicitud", SqlDbType.VarChar).Value = 'S';
                    cmdR.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = q.usuarioId;
                    SqlDataReader drR = cmdR.ExecuteReader();

                    if (drR.HasRows)
                    {
                        List<RequerimientoMaterial> r = new List<RequerimientoMaterial>();
                        while (drR.Read())
                        {
                            r.Add(new RequerimientoMaterial()
                            {
                                codigo = drR.GetString(0),
                                descripcion = drR.GetString(1),
                                abreviatura = drR.GetString(2),
                                tipoOrden = drR.GetString(3)
                            });
                        }
                        s.requerimientoMateriales = r;
                    }
                    //
                    ////s.resumen = r;
                    //
                    //SqlCommand cmdPe = con.CreateCommand();
                    //cmdPe.CommandTimeout = 0;
                    //cmdPe.CommandType = CommandType.StoredProcedure;
                    //cmdPe.CommandText = "Movil_getPersonal_CentroCosto";
                    //cmdPe.Parameters.Add("@UsuarioId", SqlDbType.VarChar).Value = q.usuarioId;
                    //cmdPe.Parameters.Add("@CentroCostoId", SqlDbType.VarChar).Value = q.centroCostoId;
                    //SqlDataReader drPe = cmdPe.ExecuteReader();
                    //if (drPe.HasRows)
                    //{
                    //    List<Personal> p = new List<Personal>();
                    //    while (drPe.Read())
                    //    {
                    //        p.Add(new Personal()
                    //        {
                    //            personalId = drPe.GetInt32(0),
                    //            empresaId = drPe.GetInt32(1),
                    //            nroDocumento = drPe.GetString(2),
                    //            apellido = drPe.GetString(3),
                    //            nombre = drPe.GetString(4)
                    //        });
                    //    }
                    //    s.personals = p;
                    //}


                    //SqlCommand cmdCo = con.CreateCommand();
                    //cmdCo.CommandTimeout = 0;
                    //cmdCo.CommandType = CommandType.StoredProcedure;
                    //cmdCo.CommandText = "Movil_GetUsuario_Coordinador_C";
                    //cmdCo.Parameters.Add("@Codigo_OP", SqlDbType.VarChar).Value = q.centroCostoId;
                    //SqlDataReader drCo = cmdCo.ExecuteReader();
                    //if (drCo.HasRows)
                    //{
                    //    List<Coordinador> c = new List<Coordinador>();
                    //    while (drCo.Read())
                    //    {
                    //        c.Add(new Coordinador()
                    //        {
                    //            codigo = drCo.GetString(0),
                    //            nombre = drCo.GetString(1)
                    //        });
                    //    }
                    //    s.coordinadors = c;
                    //}

                    //SqlCommand cmdA = con.CreateCommand();
                    //cmdA.CommandTimeout = 0;
                    //cmdA.CommandType = CommandType.StoredProcedure;
                    //cmdA.CommandText = "Movil_GetActividad_C";
                    //SqlDataReader drA = cmdA.ExecuteReader();
                    //if (drA.HasRows)
                    //{
                    //    List<Actividad> a = new List<Actividad>();
                    //    while (drA.Read())
                    //    {
                    //        a.Add(new Actividad()
                    //        {
                    //            actividadId = drA.GetInt32(0),
                    //            descripcion = drA.GetString(1)
                    //        });
                    //    }
                    //    s.actividades = a;
                    //}
                    //
                    //// MEDIDOR
                    //
                    //SqlCommand cmdMe = con.CreateCommand();
                    //cmdMe.CommandTimeout = 0;
                    //cmdMe.CommandType = CommandType.StoredProcedure;
                    //cmdMe.CommandText = "Movil_GetMateriales_Series_C";
                    //cmdMe.Parameters.Add("@area", SqlDbType.VarChar).Value = q.areaId;
                    //cmdMe.Parameters.Add("@cc", SqlDbType.VarChar).Value = q.centroCostoId;
                    //cmdMe.Parameters.Add("@usuario", SqlDbType.VarChar).Value = q.usuarioId;
                    //SqlDataReader drMe = cmdMe.ExecuteReader();
                    //if (drMe.HasRows)
                    //{
                    //    List<Medidor> m = new List<Medidor>();
                    //    while (drMe.Read())
                    //    {
                    //        m.Add(new Medidor()
                    //        {
                    //            medidorId = drMe.GetString(0),
                    //            sucursalCodigo = drMe.GetString(1),
                    //            almacenCodigo = drMe.GetString(2),
                    //            empleadoDni = drMe.GetString(3),
                    //            guiaNumero = drMe.GetString(4),
                    //            articuloCodigo = drMe.GetString(5)
                    //        });
                    //    }
                    //    s.medidores = m;
                    //}
                    //
                    //SqlCommand cmdTD = con.CreateCommand();
                    //cmdTD.CommandTimeout = 0;
                    //cmdTD.CommandType = CommandType.StoredProcedure;
                    //cmdTD.CommandText = "Movil_GetTipoDevolucion_C";
                    //SqlDataReader drTD = cmdTD.ExecuteReader();
                    //if (drTD.HasRows)
                    //{
                    //    List<TipoDevolucion> t = new List<TipoDevolucion>();
                    //    while (drTD.Read())
                    //    {
                    //        t.Add(new TipoDevolucion()
                    //        {
                    //            tipo = drTD.GetInt32(0),
                    //            descripcion = drTD.GetString(1),
                    //            estado = drTD.GetInt32(2)
                    //        });
                    //    }
                    //    s.devoluciones = t;
                    //}

                    SqlCommand cmdRE = con.CreateCommand();
                    cmdRE.CommandTimeout = 0;
                    cmdRE.CommandType = CommandType.StoredProcedure;
                    cmdRE.CommandText = "Dsige_Movil_Requerimiento_Estado";
                    SqlDataReader drRE = cmdRE.ExecuteReader();
                    if (drRE.HasRows)
                    {
                        List<Estado> e = new List<Estado>();
                        while (drRE.Read())
                        {
                            e.Add(new Estado()
                            {
                                codigo = drRE.GetString(0),
                                nombre = drRE.GetString(1)
                            });
                        }
                        s.requerimientoEstado = e;
                    }

                    SqlCommand cmdTi = con.CreateCommand();
                    cmdTi.CommandTimeout = 0;
                    cmdTi.CommandType = CommandType.StoredProcedure;
                    cmdTi.CommandText = "Dsige_Movil_Requerimiento_Tipo";
                    SqlDataReader drTi = cmdTi.ExecuteReader();
                    if (drTi.HasRows)
                    {
                        List<Estado> e = new List<Estado>();
                        while (drTi.Read())
                        {
                            e.Add(new Estado()
                            {
                                codigo = drTi.GetString(0),
                                nombre = drTi.GetString(1)
                            });
                        }
                        s.requerimientoTipo = e;
                    }

                    SqlCommand cmdRC = con.CreateCommand();
                    cmdRC.CommandTimeout = 0;
                    cmdRC.CommandType = CommandType.StoredProcedure;
                    cmdRC.CommandText = "Dsige_Movil_Requerimiento_CentroCostos";
                    SqlDataReader drRC = cmdRC.ExecuteReader();
                    if (drRC.HasRows)
                    {
                        List<Estado> e = new List<Estado>();
                        while (drRC.Read())
                        {
                            e.Add(new Estado()
                            {
                                codigo = drRC.GetString(0),
                                nombre = drRC.GetString(1)
                            });
                        }
                        s.requerimientoCentroCostos = e;
                    }

                    SqlCommand cmd1 = con.CreateCommand();
                    cmd1.CommandTimeout = 0;
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.CommandText = "DSIGE_Proy_M_Delegacion_Usuario";
                    cmd1.Parameters.Add("@usuario", SqlDbType.VarChar).Value = q.usuarioId;
                    SqlDataReader dr1 = cmd1.ExecuteReader();
                    if (dr1.HasRows)
                    {
                        List<Estado> e = new List<Estado>();
                        while (dr1.Read())
                        {
                            e.Add(new Estado()
                            {
                                codigo = dr1.GetString(0),
                                nombre = dr1.GetString(1)
                            });
                        }
                        s.comboEstados = e;
                    }

                    SqlCommand cmd2 = con.CreateCommand();
                    cmd2.CommandTimeout = 0;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.CommandText = "DSIGE_Proy_M_Locales_Usuario";
                    cmd2.Parameters.Add("@usuario", SqlDbType.VarChar).Value = q.usuarioId;
                    SqlDataReader dr2 = cmd2.ExecuteReader();
                    if (dr2.HasRows)
                    {
                        List<Estado> e = new List<Estado>();
                        while (dr2.Read())
                        {
                            e.Add(new Estado()
                            {
                                codigo = dr2.GetString(0),
                                nombre = dr2.GetString(1)
                            });
                        }
                        s.locales = e;
                    }

                    SqlCommand cmd3 = con.CreateCommand();
                    cmd3.CommandTimeout = 0;
                    cmd3.CommandType = CommandType.StoredProcedure;
                    cmd3.CommandText = "DSIGE_Proy_M_Almacen_Usuario";
                    cmd3.Parameters.Add("@usuario", SqlDbType.VarChar).Value = q.usuarioId;
                    SqlDataReader dr3 = cmd3.ExecuteReader();
                    if (dr3.HasRows)
                    {
                        List<Estado> e = new List<Estado>();
                        while (dr3.Read())
                        {
                            e.Add(new Estado()
                            {
                                codigo = dr3.GetString(0),
                                nombre = dr3.GetString(1),
								sucuCodigo = dr3.GetString(2)
                            });
                        }
                        s.almacenes = e;
                    }
					
					SqlCommand cmd4 = con.CreateCommand();
                    cmd4.CommandTimeout = 0;
                    cmd4.CommandType = CommandType.StoredProcedure;
                    cmd4.CommandText = "DSIGE_Proy_M_Estados_Aprobacion_Orden_Compra";
                    cmd4.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = q.usuarioId;
                    SqlDataReader dr4 = cmd4.ExecuteReader();
                    if (dr4.HasRows)
                    {
                        List<Estado> e = new List<Estado>();
                        while (dr4.Read())
                        {
                            e.Add(new Estado()
                            {
                                codigo = dr4.GetString(0),
                                nombre = dr4.GetString(1)
                            });
                        }
                        s.ordenEstados = e;
                    }

                    SqlCommand cmd5 = con.CreateCommand();
                    cmd5.CommandTimeout = 0;
                    cmd5.CommandType = CommandType.StoredProcedure;
                    cmd5.CommandText = "Movil_Menu_Logistica";
                    cmd5.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = q.usuarioId;
                    SqlDataReader dr5 = cmd5.ExecuteReader();
                    if (dr5.HasRows)
                    {
                        List<MenuLogistica> e = new List<MenuLogistica>();
                        while (dr5.Read())
                        {
                            e.Add(new MenuLogistica()
                            {
                                id = dr5.GetInt32(0),
                                nombre = dr5.GetString(1),
                                orden = dr5.GetInt32(2)
                            });
                        }
                        s.menu = e;
                    }

                    con.Close();
                }
                return s;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Mensaje SaveRegistro(Registro r)
        {
            try
            {
                Mensaje m = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();

                    // General
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Movil_SaveReparacion";
                    cmd.Parameters.Add("@registroId", SqlDbType.Int).Value = r.identity;
                    cmd.Parameters.Add("@tipo", SqlDbType.Int).Value = r.tipo;
                    cmd.Parameters.Add("@nroobra", SqlDbType.VarChar).Value = r.nroObra;
                    cmd.Parameters.Add("@foto", SqlDbType.VarChar).Value = r.foto;
                    cmd.Parameters.Add("@latitud", SqlDbType.VarChar).Value = r.latitud;
                    cmd.Parameters.Add("@longitud", SqlDbType.VarChar).Value = r.longitud;
                    cmd.Parameters.Add("@nroposte", SqlDbType.VarChar).Value = r.nroPoste;
                    cmd.Parameters.Add("@estado", SqlDbType.VarChar).Value = r.estado;
                    cmd.Parameters.Add("@usuario", SqlDbType.VarChar).Value = r.usuarioId;
                    cmd.Parameters.Add("@fecha", SqlDbType.VarChar).Value = r.fecha;

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        m = new Mensaje();

                        while (dr.Read())
                        {
                            m.mensaje = "Mensaje Enviado";
                            m.codigoBase = r.registroId;
                            m.codigoRetorno = dr.GetInt32(0);

                            foreach (var b in r.list)
                            {
                                SqlCommand cmdB = con.CreateCommand();
                                cmdB.CommandTimeout = 0;
                                cmdB.CommandType = CommandType.StoredProcedure;
                                cmdB.CommandText = "Movil_SavePuntoReparacion";
                                cmdB.Parameters.Add("@registroId", SqlDbType.Int).Value = m.codigoRetorno;
                                cmdB.Parameters.Add("@nombrePunto", SqlDbType.VarChar).Value = b.nombrePunto;
                                cmdB.Parameters.Add("@largo", SqlDbType.Decimal).Value = b.largo;
                                cmdB.Parameters.Add("@ancho", SqlDbType.Decimal).Value = b.ancho;
                                cmdB.Parameters.Add("@m3", SqlDbType.Decimal).Value = b.totalM3;
                                cmdB.Parameters.Add("@descripcion", SqlDbType.VarChar).Value = b.observacion;
                                cmdB.Parameters.Add("@foto1_antes", SqlDbType.VarChar).Value = b.foto1PuntoAntes;
                                cmdB.Parameters.Add("@foto2_antes", SqlDbType.VarChar).Value = b.foto2PuntoAntes;
                                cmdB.Parameters.Add("@foto3_antes", SqlDbType.VarChar).Value = b.foto3PuntoAntes;
                                cmdB.Parameters.Add("@foto1_despues", SqlDbType.VarChar).Value = b.foto1PuntoDespues;
                                cmdB.Parameters.Add("@foto2_despues", SqlDbType.VarChar).Value = b.foto2PuntoDespues;
                                cmdB.Parameters.Add("@foto3_despues", SqlDbType.VarChar).Value = b.foto3PuntoDespues;
                                cmdB.Parameters.Add("@estado", SqlDbType.VarChar).Value = b.estado;
                                cmdB.Parameters.Add("@usuario", SqlDbType.VarChar).Value = r.usuarioId;
                                cmdB.ExecuteNonQuery();
                            }
                        }
                    }

                    con.Close();
                }
                return m;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Mensaje SaveVehiculo(Vehiculo v)
        {
            try
            {
                Mensaje m = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();


                    m = new Mensaje
                    {
                        codigoBase = 1,
                        mensaje = "Enviado"
                    };


                    foreach (var c in v.control)
                    {
                        SqlCommand cmd = con.CreateCommand();
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "Movil_SaveVehiculoControlDiario";
                        cmd.Parameters.Add("@placa", SqlDbType.VarChar).Value = c.placa;
                        cmd.Parameters.Add("@fecha", SqlDbType.VarChar).Value = c.fecha;
                        cmd.Parameters.Add("@kmIngreso", SqlDbType.Decimal).Value = c.kmIngreso;
                        cmd.Parameters.Add("@kmSalida", SqlDbType.Decimal).Value = c.kmSalida;
                        cmd.Parameters.Add("@choferDni", SqlDbType.VarChar).Value = c.choferDni;
                        cmd.Parameters.Add("@gesCodigo", SqlDbType.VarChar).Value = c.gesCodigo;
                        cmd.Parameters.Add("@pubCodigo", SqlDbType.VarChar).Value = c.pubCodigo;
                        cmd.ExecuteNonQuery();
                    }

                    foreach (var r in v.registros)
                    {
                        SqlCommand cmd = con.CreateCommand();
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "Movil_SaveVehiculoVales";
                        cmd.Parameters.Add("@placa", SqlDbType.VarChar).Value = r.placa;
                        cmd.Parameters.Add("@gesCodigo", SqlDbType.VarChar).Value = r.gesCodigo;
                        cmd.Parameters.Add("@dni", SqlDbType.VarChar).Value = r.dni;
                        cmd.Parameters.Add("@nroVale", SqlDbType.VarChar).Value = r.nroVale;
                        cmd.Parameters.Add("@fecha", SqlDbType.VarChar).Value = r.fecha;
                        cmd.Parameters.Add("@tipo", SqlDbType.VarChar).Value = r.tipo;
                        cmd.Parameters.Add("@precioIGV", SqlDbType.Decimal).Value = r.precioIGV;
                        cmd.Parameters.Add("@cantidadGalones", SqlDbType.Decimal).Value = r.cantidadGalones;
                        cmd.Parameters.Add("@kmValeCombustible", SqlDbType.Decimal).Value = r.kmValeCombustible;
                        cmd.Parameters.Add("@rucGrifo", SqlDbType.VarChar).Value = r.rucGrifo;
                        cmd.Parameters.Add("@pubCodigo", SqlDbType.VarChar).Value = r.pubCodigo;
                        cmd.Parameters.Add("@foto", SqlDbType.VarChar).Value = r.foto;
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                return m;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Mensaje SaveEstadoMovil(EstadoMovil e)
        {
            try
            {
                Mensaje m = null;
                using (SqlConnection cn = new SqlConnection(db))
                {
                    cn.Open();
                    SqlCommand cmd = cn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "Movil_SaveEstadoCelular";
                    cmd.Parameters.Add("@usuarioId", SqlDbType.VarChar).Value = e.usuarioId;
                    cmd.Parameters.Add("@gpsActivo", SqlDbType.Bit).Value = e.gpsActivo;
                    cmd.Parameters.Add("@estadoBateria", SqlDbType.Int).Value = e.estadoBateria;
                    cmd.Parameters.Add("@fecha", SqlDbType.VarChar).Value = e.fecha;
                    cmd.Parameters.Add("@modoAvion", SqlDbType.Int).Value = e.modoAvion;
                    cmd.Parameters.Add("@planDatos", SqlDbType.Bit).Value = e.planDatos;
                    int a = cmd.ExecuteNonQuery();
                    if (a == 1)
                    {
                        m = new Mensaje
                        {
                            codigoBase = 1,
                            mensaje = "Enviado"
                        };
                    }

                    cn.Close();
                }

                return m;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Mensaje SaveOperarioGps(EstadoOperario e)
        {
            try
            {
                Mensaje m = null;

                using (SqlConnection cn = new SqlConnection(db))
                {
                    cn.Open();
                    SqlCommand cmd = cn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Movil_SaveGps";
                    cmd.Parameters.Add("@usuarioId", SqlDbType.VarChar).Value = e.usuarioId;
                    cmd.Parameters.Add("@latitud", SqlDbType.VarChar).Value = e.latitud;
                    cmd.Parameters.Add("@longitud", SqlDbType.VarChar).Value = e.longitud;
                    cmd.Parameters.Add("@fechaGPD", SqlDbType.VarChar).Value = e.fechaGPD;
                    cmd.Parameters.Add("@fecha", SqlDbType.VarChar).Value = e.fecha;

                    int a = cmd.ExecuteNonQuery();

                    if (a == 1)
                    {
                        m = new Mensaje
                        {
                            codigoBase = 1,
                            mensaje = "Enviado"
                        };
                    }

                    cn.Close();
                }

                return m;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Guardar Registro Parte Diario
        public static Mensaje SaveRegistroParteDiario(ParteDiario p)
        {
            try
            {
                Mensaje m = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();

                    // General
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Movil_SaveParteDiario_C";
                    cmd.Parameters.Add("@parteDiarioId", SqlDbType.Int).Value = p.identity;
                    cmd.Parameters.Add("@tipo", SqlDbType.Int).Value = p.tipo;
                    cmd.Parameters.Add("@pub_empr_codigo", SqlDbType.VarChar).Value = p.empresaCodigo;
                    cmd.Parameters.Add("@pub_area_codigo", SqlDbType.VarChar).Value = p.areaCodigo;
                    cmd.Parameters.Add("@partediario_obratd", SqlDbType.VarChar).Value = p.obraTd;
                    cmd.Parameters.Add("@codigo_interno", SqlDbType.VarChar).Value = p.codigoInterno;
                    cmd.Parameters.Add("@partediario_fecha", SqlDbType.VarChar).Value = p.fecha;
                    cmd.Parameters.Add("@ges_ordt_codigo", SqlDbType.VarChar).Value = p.costoCodigo;
                    cmd.Parameters.Add("@ges_cuad_codigo", SqlDbType.VarChar).Value = p.cuadrillaCodigo;
                    cmd.Parameters.Add("@ges_dni_cuadrilla", SqlDbType.VarChar).Value = p.dniCuadrilla;
                    cmd.Parameters.Add("@partediario_suministro", SqlDbType.VarChar).Value = p.suministro;
                    cmd.Parameters.Add("@partediario_sed", SqlDbType.VarChar).Value = p.sed;
                    cmd.Parameters.Add("@partediario_obs", SqlDbType.VarChar).Value = p.observacion;
                    cmd.Parameters.Add("@partediario_medencontrado_nro", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@partediario_medencontrado_marca", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@partediario_medencontrado_fase", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@partediario_medencontrado_estado", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@partediario_medencontrado_modelo", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@partediario_medinstalado_nro", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@partediario_medinstalado_marca", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@partediario_medinstalado_fase", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@partediario_medinstalado_estado", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@partediario_medinstalado_modelo", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@partediario_latitud", SqlDbType.VarChar).Value = p.latitud;
                    cmd.Parameters.Add("@partediario_longitud", SqlDbType.VarChar).Value = p.longitud;
                    cmd.Parameters.Add("@pub_esta_codigo", SqlDbType.VarChar).Value = "121";
                    cmd.Parameters.Add("@partediario_usucrea", SqlDbType.VarChar).Value = p.usuarioCreacion;
                    cmd.Parameters.Add("@partediario_usumodi", SqlDbType.VarChar).Value = p.usuarioCreacion;
                    cmd.Parameters.Add("@partediario_fechamovil", SqlDbType.VarChar).Value = p.fechaMovil;
                    cmd.Parameters.Add("@firma_Movil", SqlDbType.VarChar).Value = p.firmaMovil;
                    cmd.Parameters.Add("@Ges_DNI_Cordinador", SqlDbType.VarChar).Value = p.coordinadorDni;
                    cmd.Parameters.Add("@sucursal", SqlDbType.VarChar).Value = p.sucursalId;

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        m = new Mensaje();

                        while (dr.Read())
                        {

                            m.mensaje = "Mensaje Enviado";
                            m.codigoBase = p.parteDiarioId;
                            m.codigoRetorno = dr.GetInt32(0);

                            foreach (var b in p.baremos)
                            {
                                SqlCommand cmdB = con.CreateCommand();
                                cmdB.CommandTimeout = 0;
                                cmdB.CommandType = CommandType.StoredProcedure;
                                cmdB.CommandText = "Movil_SaveParteDiarioBaremo_C";
                                cmdB.Parameters.Add("@id_partediario", SqlDbType.Int).Value = m.codigoRetorno;
                                cmdB.Parameters.Add("@id_actividad", SqlDbType.Int).Value = b.actividadId;
                                cmdB.Parameters.Add("@codigobaremo", SqlDbType.VarChar).Value = b.codigoBaremo;
                                cmdB.Parameters.Add("@partediario_bar_cantidadmovil", SqlDbType.Decimal).Value = b.cantidadMovil;
                                cmdB.Parameters.Add("@partediario_bar_cantidadok", SqlDbType.Decimal).Value = b.cantidadOk;
                                cmdB.Parameters.Add("@partediario_fechamovil", SqlDbType.VarChar).Value = b.fecha;
                                cmdB.Parameters.Add("@tipo", SqlDbType.Int).Value = b.tipo;
                                cmdB.ExecuteNonQuery();
                            }

                            foreach (var ma in p.materiales)
                            {
                                SqlCommand cmdM = con.CreateCommand();
                                cmdM.CommandTimeout = 0;
                                cmdM.CommandType = CommandType.StoredProcedure;
                                cmdM.CommandText = "Movil_SaveParteDiarioMaterial_C";
                                cmdM.Parameters.Add("@id_partediario", SqlDbType.Int).Value = m.codigoRetorno;
                                cmdM.Parameters.Add("@partediario_tipomaterial", SqlDbType.Int).Value = ma.tipoMaterial;
                                cmdM.Parameters.Add("@almacenId", SqlDbType.VarChar).Value = ma.almacenId;
                                cmdM.Parameters.Add("@codigo_material", SqlDbType.VarChar).Value = ma.codigoMaterial;
                                cmdM.Parameters.Add("@partediario_mat_cantidadmovil", SqlDbType.Decimal).Value = ma.cantidadMovil;
                                cmdM.Parameters.Add("@partediario_mat_cantidadok", SqlDbType.Decimal).Value = ma.cantidadOk;
                                cmdM.Parameters.Add("@partediario_fechamovil", SqlDbType.VarChar).Value = ma.fecha;
                                cmdM.Parameters.Add("@tipo", SqlDbType.Int).Value = ma.tipo;
                                cmdM.Parameters.Add("@guiaSalida", SqlDbType.VarChar).Value = ma.guiaSalida;
                                cmdM.Parameters.Add("@nroSerie", SqlDbType.VarChar).Value = ma.nroSerie;
                                cmdM.Parameters.Add("@guiaIngresoId", SqlDbType.VarChar).Value = ma.guiaIngresoId;
                                cmdM.Parameters.Add("@guiaIngreso", SqlDbType.VarChar).Value = ma.guiaIngreso;
                                cmdM.ExecuteNonQuery();
                            }

                            foreach (var f in p.photos)
                            {
                                SqlCommand cmdF = con.CreateCommand();
                                cmdF.CommandTimeout = 0;
                                cmdF.CommandType = CommandType.StoredProcedure;
                                cmdF.CommandText = "Movil_SaveParteDiarioPhoto_C";
                                cmdF.Parameters.Add("@id_partediario", SqlDbType.Int).Value = m.codigoRetorno;
                                cmdF.Parameters.Add("@partediario_foto_url", SqlDbType.VarChar).Value = f.nombre;
                                cmdF.Parameters.Add("@partediario_fechamovil", SqlDbType.VarChar).Value = f.fecha;
                                cmdF.Parameters.Add("@tipo", SqlDbType.Int).Value = f.tipo;
                                cmdF.ExecuteNonQuery();
                            }
                        }
                    }

                    con.Close();
                }
                return m;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // Guardar Solicitud Cabecera
        public static Mensaje SaveRegistroGeneral(Solicitud s)
        {
            try
            {
                Mensaje mensaje = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();

                    // General
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Movil_SaveSolicitud_C";
                    cmd.Parameters.Add("@filtro", SqlDbType.Int).Value = s.filtro;
                    cmd.Parameters.Add("@tipo", SqlDbType.Int).Value = s.tipo;
                    cmd.Parameters.Add("@Id_Solicitud", SqlDbType.Int).Value = s.identity;
                    cmd.Parameters.Add("@id_tipmaterialsol", SqlDbType.Int).Value = s.tipoMaterialSol;
                    cmd.Parameters.Add("@tipoSolicitudId", SqlDbType.Int).Value = s.tipoSolicitudId;
                    cmd.Parameters.Add("@tipomaterial", SqlDbType.Int).Value = s.tipoMaterial;
                    cmd.Parameters.Add("@nro_interno", SqlDbType.VarChar).Value = s.nroInterno;
                    cmd.Parameters.Add("@fechaatencion_solicitud", SqlDbType.VarChar).Value = s.fechaAtencion;
                    cmd.Parameters.Add("@solicitud_obratd", SqlDbType.VarChar).Value = s.obraTd;
                    cmd.Parameters.Add("@obs_solicitud", SqlDbType.VarChar).Value = s.observacion;
                    cmd.Parameters.Add("@ges_ordt_codigo", SqlDbType.VarChar).Value = s.centroCosto;
                    cmd.Parameters.Add("@ges_cuad_codigo", SqlDbType.VarChar).Value = s.cuadrillaCodigo;
                    cmd.Parameters.Add("@ges_dni_cuadrilla", SqlDbType.VarChar).Value = s.dniCuadrilla;
                    cmd.Parameters.Add("@pub_esta_codigo", SqlDbType.VarChar).Value = s.pubEstadoCodigo;
                    cmd.Parameters.Add("@usuario", SqlDbType.VarChar).Value = s.usuario;
                    cmd.Parameters.Add("@solicitud_fechamovil", SqlDbType.VarChar).Value = s.fechaAsignacion;
                    cmd.Parameters.Add("@ges_dni_coordinador", SqlDbType.VarChar).Value = s.dniCoordinador;
                    cmd.Parameters.Add("@ges_dni_personal", SqlDbType.VarChar).Value = s.dniPersonal;
                    cmd.Parameters.Add("@sucursal", SqlDbType.VarChar).Value = s.sucursalId;

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        mensaje = new Mensaje();

                        while (dr.Read())
                        {
                            mensaje.mensaje = s.tipo == 1 ? "Registro Actualizado" : "Registro Guardado";
                            mensaje.codigoBase = s.solicitudId;
                            mensaje.codigoRetorno = dr.GetInt32(0);
                        }
                    }

                    con.Close();
                }
                return mensaje;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // Guardar Solicitud Detalle
        public static Mensaje SaveRegistroDetalle(RegistroMaterialSolicitud r)
        {
            try
            {
                Mensaje mensaje = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    // General
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Movil_SaveSolicitudDetalle_C";
                    cmd.Parameters.Add("@solicitudMaterial", SqlDbType.Int).Value = r.identityDetalle;
                    cmd.Parameters.Add("@filtro", SqlDbType.Int).Value = r.filtro;
                    cmd.Parameters.Add("@id_solicitud", SqlDbType.Int).Value = r.identity;
                    cmd.Parameters.Add("@id_tipmaterialsol", SqlDbType.Int).Value = r.tipoMaterial;
                    cmd.Parameters.Add("@codigo_material", SqlDbType.VarChar).Value = r.codigoMaterial;
                    cmd.Parameters.Add("@solicitud_mat_cantidadmovil", SqlDbType.Decimal).Value = r.cantidadMovil;
                    cmd.Parameters.Add("@solicitud_mat_cantidadok", SqlDbType.Decimal).Value = r.cantidadOk;
                    cmd.Parameters.Add("@solicitud_fechamovil", SqlDbType.VarChar).Value = r.fecha;
                    cmd.Parameters.Add("@tipo", SqlDbType.Int).Value = r.tipo;
                    cmd.Parameters.Add("@almacenId", SqlDbType.VarChar).Value = r.almacenId;
                    cmd.Parameters.Add("@usuarioId", SqlDbType.VarChar).Value = r.usuarioId;
                    cmd.Parameters.Add("@GuiaSalida", SqlDbType.VarChar).Value = r.guiaSalida;
                    cmd.Parameters.Add("@Stock", SqlDbType.Decimal).Value = 0;
                    cmd.Parameters.Add("@guiaIngresoId", SqlDbType.VarChar).Value = r.guiaIngresoId;
                    cmd.Parameters.Add("@guiaIngreso", SqlDbType.VarChar).Value = r.guiaIngreso;

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        mensaje = new Mensaje();

                        while (dr.Read())
                        {
                            mensaje.codigoBase = r.registroMaterialId;
                            mensaje.codigoRetorno = dr.GetInt32(0);
                            mensaje.mensaje = dr.GetString(1);
                        }
                    }

                    con.Close();
                }
                return mensaje;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Mensaje SaveRegistroPhoto(RegistroPhoto r)
        {
            try
            {
                Mensaje mensaje = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    // General
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Movil_SaveSolicitudFoto_C";
                    cmd.Parameters.Add("@id_solicitudFoto", SqlDbType.Int).Value = r.identityFoto;
                    cmd.Parameters.Add("@id_solicitud", SqlDbType.Int).Value = r.identity;
                    cmd.Parameters.Add("@solicitud_foto_nombre", SqlDbType.VarChar).Value = r.nombre;
                    cmd.Parameters.Add("@solicitud_fechamovil", SqlDbType.VarChar).Value = r.fecha;
                    cmd.Parameters.Add("@tipo", SqlDbType.Int).Value = r.tipo;
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        mensaje = new Mensaje();
                        while (dr.Read())
                        {
                            if (r.tipo == 0)
                            {
                                mensaje.mensaje = "Foto Guardada";
                                mensaje.codigoBase = r.registroPhotoId;
                                mensaje.codigoRetorno = dr.GetInt32(0);
                            }
                            else
                            {
                                mensaje.mensaje = "Foto Eliminada";
                                mensaje.codigoBase = r.registroPhotoId;
                            }
                        }
                    }

                    con.Close();
                }
                return mensaje;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // Stock de Materiales
        public static List<Materiales> GetStockMaterial(Query q)
        {
            try
            {
                List<Materiales> materiales = null;
                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Movil_GesStock_Materiales_C";
                    cmd.Parameters.Add("@usuarioId", SqlDbType.VarChar).Value = q.usuarioId;
                    cmd.Parameters.Add("@tipoSolicitud", SqlDbType.VarChar).Value = q.tipoSolicitud;
                    cmd.Parameters.Add("@tipoMaterial", SqlDbType.VarChar).Value = q.tipoMaterialSolicitud;
                    cmd.Parameters.Add("@almacenId", SqlDbType.VarChar).Value = q.almacenId;
                    cmd.Parameters.Add("@centroCosto", SqlDbType.VarChar).Value = q.centroCostoId;
                    cmd.Parameters.Add("@filtro", SqlDbType.Int).Value = q.filtro;
                    cmd.Parameters.Add("@codigoArticulo", SqlDbType.VarChar).Value = q.codigoArticulo;
                    cmd.Parameters.Add("@pageIndex", SqlDbType.Int).Value = q.pageIndex;
                    cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = q.pageSize;
                    cmd.Parameters.Add("@search", SqlDbType.VarChar).Value = q.search;
                    cmd.Parameters.Add("@Obra", SqlDbType.VarChar).Value = q.obraId;
                    cmd.Parameters.Add("@dniPersonal", SqlDbType.VarChar).Value = q.personalDni;

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        materiales = new List<Materiales>();
                        while (dr.Read())
                        {
                            materiales.Add(new Materiales()
                            {
                                materialId = dr.GetString(0),
                                descripcion = dr.GetString(1),
                                unidadMedida = dr.GetString(2),
                                abreviatura = dr.GetString(3),
                                stock = Convert.ToDecimal(dr.GetDecimal(4)),
                                fecha = dr.GetString(5),
                                guiaIngresoId = dr.GetString(6),
                                guiaIngreso = dr.GetString(7),
                                tipo = 2
                            });
                        }
                    }
                    con.Close();
                }
                return materiales;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // Get Solicitud
        public static List<Solicitud> GetSolicitudes(Query q)
        {
            try
            {
                List<Solicitud> s = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    // Solicitud
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Movil_GetSolicitudes_C";
                    cmd.Parameters.Add("@usuarioId", SqlDbType.VarChar).Value = q.usuarioId;
                    cmd.Parameters.Add("@cuadrilla", SqlDbType.VarChar).Value = q.cuadrillaId;
                    cmd.Parameters.Add("@centroCostoId", SqlDbType.VarChar).Value = q.centroCostoId;
                    cmd.Parameters.Add("@tipoSolicitud", SqlDbType.Int).Value = q.tipoSolicitud;
                    cmd.Parameters.Add("@tipoMaterialSolicitud", SqlDbType.Int).Value = q.tipoMaterialSolicitud;
                    cmd.Parameters.Add("@fechaRegistro", SqlDbType.VarChar).Value = q.fechaRegistro;
                    cmd.Parameters.Add("@estado", SqlDbType.VarChar).Value = q.estado;
                    cmd.Parameters.Add("@filtro", SqlDbType.VarChar).Value = q.filtro;
                    cmd.Parameters.Add("@pageIndex", SqlDbType.Int).Value = q.pageIndex;
                    cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = q.pageSize;
                    cmd.Parameters.Add("@search", SqlDbType.VarChar).Value = q.search;
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        s = new List<Solicitud>();

                        while (dr.Read())
                        {
                            var o = new Solicitud();
                            o.solicitudId = dr.GetInt32(0);
                            o.identity = dr.GetInt32(0);
                            o.tipoMaterialSol = dr.GetInt32(1);
                            o.filtro = dr.GetInt32(1);
                            o.tipoSolicitudId = dr.GetInt32(2);
                            o.tipoMaterial = dr.GetInt32(3);
                            o.nombreTipoMaterial = dr.GetString(4);
                            o.nroInterno = dr.GetString(5);
                            o.fechaAtencion = dr.GetDateTime(6).ToString("dd/MM/yyyy");
                            o.obraTd = dr.GetString(7);
                            o.codigoEstadoSol = dr.GetString(8);
                            o.estadoSol = dr.GetString(9);
                            o.codigoEstadoObra = dr.GetString(10);
                            o.estadoObra = dr.GetString(11);
                            o.descripcionObra = dr.GetString(12);
                            o.direccionObra = dr.GetString(13);
                            o.clienteObra = dr.GetString(14);
                            o.fechaAsignacion = dr.GetDateTime(15).ToString("dd/MM/yyyy");
                            o.fechaVencimiento = dr.GetDateTime(16).ToString("dd/MM/yyyy");
                            o.observacion = dr.GetString(17);
                            o.dniCuadrilla = dr.GetString(18);
                            o.centroCosto = dr.GetString(19);
                            o.pubEstadoCodigo = dr.GetString(20);
                            o.numeroGuia = dr.GetString(21);
                            o.cuadrillaCodigo = "";
                            o.usuario = dr.GetString(22);
                            o.dniCoordinador = dr.GetString(23);
                            o.nombreCoordinador = dr.GetString(24);
                            o.dniPersonal = dr.GetString(25);
                            o.nombrePersonal = dr.GetString(26);
                            o.sucursalId = dr.GetString(27);
                            o.tipo = 1;

                            SqlCommand cmdSM = con.CreateCommand();
                            cmdSM.CommandTimeout = 0;
                            cmdSM.CommandType = CommandType.StoredProcedure;
                            cmdSM.CommandText = "Movil_GetSolicitudDetalleById_C";
                            cmdSM.Parameters.Add("@solicitudId", SqlDbType.VarChar).Value = o.solicitudId;

                            SqlDataReader drSM = cmdSM.ExecuteReader();
                            if (drSM.HasRows)
                            {
                                List<RegistroMaterialSolicitud> rr = new List<RegistroMaterialSolicitud>();

                                while (drSM.Read())
                                {
                                    rr.Add(new RegistroMaterialSolicitud()
                                    {
                                        registroMaterialId = drSM.GetInt32(0),
                                        identityDetalle = drSM.GetInt32(0),
                                        identity = drSM.GetInt32(1),
                                        solicitudId = drSM.GetInt32(1),
                                        tipoMaterial = drSM.GetInt32(2),
                                        codigoMaterial = drSM.GetString(3),
                                        cantidadMovil = drSM.GetDecimal(4),
                                        cantidadOk = drSM.GetDecimal(5),
                                        fecha = drSM.GetDateTime(6).ToString("dd/MM/yyyy"),
                                        tipo = 1,
                                        filtro = drSM.GetInt32(2),
                                        descripcion = drSM.GetString(7),
                                        abreviatura = drSM.GetString(8),
                                        unidadMedida = drSM.GetString(9),
                                        almacenId = drSM.GetString(10),
                                        usuarioId = o.usuario,
                                        guiaSalida = drSM.GetString(11),
                                        tipoSolicitudId = o.tipoSolicitudId,
                                        cantidadAprobada = drSM.GetDecimal(12),
                                        estado = o.pubEstadoCodigo
                                    });
                                }
                                o.materiales = rr;
                            }

                            // SOLICITUD PHOTO

                            SqlCommand cmdSF = con.CreateCommand();
                            cmdSF.CommandTimeout = 0;
                            cmdSF.CommandType = CommandType.StoredProcedure;
                            cmdSF.CommandText = "Movil_GetSolicitudFotoById_C";
                            cmdSF.Parameters.Add("@solicitudId", SqlDbType.VarChar).Value = o.solicitudId;
                            SqlDataReader drSF = cmdSF.ExecuteReader();
                            if (drSF.HasRows)
                            {
                                List<RegistroPhotoSolicitud> f = new List<RegistroPhotoSolicitud>();

                                while (drSF.Read())
                                {
                                    f.Add(new RegistroPhotoSolicitud()
                                    {
                                        registroPhotoId = drSF.GetInt32(0),
                                        identityFoto = drSF.GetInt32(0),
                                        identity = drSF.GetInt32(1),
                                        solicitudId = drSF.GetInt32(1),
                                        nombre = drSF.GetString(2),
                                        fecha = drSF.GetDateTime(3).ToString("dd/MM/yyyy"),
                                        tipo = 1
                                    });

                                }
                                o.photos = f;
                            }

                            s.Add(o);
                        }
                    }
                    con.Close();
                }
                return s;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        // Aprobation 

        public static Mensaje GetAprobation(Query q)
        {
            try
            {
                Mensaje m = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Movil_GetAprobacion_C";
                    cmd.Parameters.Add("@solicitudId", SqlDbType.VarChar).Value = q.solicitudId;
                    cmd.Parameters.Add("@estado", SqlDbType.VarChar).Value = q.estado;
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        m = new Mensaje();
                        while (dr.Read())
                        {
                            m.mensaje = dr.GetString(0);
                        }
                    }
                    con.Close();
                }
                return m;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // Logistica

        public static List<Pedido> GetPedidosCompra(string usuario)
        {
            try
            {
                List<Pedido> p = null;
                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DSIGE_Proy_M_Lista_Aprobacion_Pedidos_Compra";
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = usuario;
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        p = new List<Pedido>();
                        while (dr.Read())
                        {
                            p.Add(new Pedido()
                            {
                                id = dr.GetInt32(0),
                                pedidoId = dr.GetInt32(1),
                                tipoPedidoId = dr.GetString(2),
                                nombreTipoPedido = dr.GetString(3),
                                nroPedido = dr.GetString(4),
                                fechaEnvio = dr.GetDateTime(5).ToString("dd/MM/yyyy"),
                                delegacion = dr.GetString(6),
                                ccId = dr.GetString(7),
                                centroCostos = dr.GetString(8),
                                dni = dr.GetString(9),
                                nombreEmpleado = dr.GetString(10),
                                pubEstaCodigo = dr.GetString(11),
                                estado = dr.GetString(12),
                                moneda = dr.GetString(13),
                                item = dr.GetInt32(14),
                                articulo = dr.GetString(15),
                                nombreArticulo = dr.GetString(16),
                                cantidad = Convert.ToDecimal(dr.GetDecimal(17)),
                                cantidadAprobada = Convert.ToDecimal(dr.GetDecimal(18)),
                                precio = Convert.ToDecimal(dr.GetDecimal(19))
                            });
                        }
                    }
                    con.Close();
                }
                return p;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<Orden> GetOrdenCompra(Query q)
        {
            try
            {
                List<Orden> p = null;
                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DSIGE_Proy_M_Lista_Aprobacion_Orden_Compra";
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = q.usuarioId;
                    cmd.Parameters.Add("@Estado", SqlDbType.Int).Value = q.estado;
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        p = new List<Orden>();
                        while (dr.Read())
                        {
                            Orden o = new Orden();
                            o.id = dr.GetInt32(0);
                            o.ordenId = dr.GetInt32(1);
                            o.tipoPedidoId = dr.GetString(2);
                            o.toc = dr.GetString(3);
                            o.nroOrden = dr.GetString(4);
                            o.fechaEmisionOrden = dr.GetDateTime(5).ToString("dd/MM/yyyy");
                            o.contableOt = dr.GetString(6);
                            o.usuSolicitante = dr.GetString(7);
                            o.usuCreaOrden = dr.GetString(8);
                            o.fechaCreaOrden = dr.GetDateTime(9).ToString("dd/MM/yyyy");
                            o.fechaAprobacion = dr.GetDateTime(10).ToString("dd/MM/yyyy");
                            o.provee = dr.GetString(11);
                            o.forma = dr.GetString(12);
                            o.mone = dr.GetString(13);
                            o.subtotalOc = Convert.ToDecimal(dr.GetDecimal(14));
                            o.igv = Convert.ToDecimal(dr.GetDecimal(15));
                            o.totalOc = Convert.ToDecimal(dr.GetDecimal(16));
                            o.codEstado = dr.GetString(17);
                            o.estado = dr.GetString(18);
                            o.item = dr.GetInt32(19);
                            o.articulo = dr.GetString(20);
                            o.nombreArticulo = dr.GetString(21);
                            o.cantidadAprobada = Convert.ToDecimal(dr.GetDecimal(22));
                            o.precio = Convert.ToDecimal(dr.GetDecimal(23));
                            o.importe = Convert.ToDecimal(dr.GetDecimal(24));
                            o.delegacion = dr.GetString(25);
                            o.cotiCodigo = dr.GetString(26);
                            o.nivelAcumulado = dr.GetInt32(27);


                            SqlCommand cmdD = con.CreateCommand();
                            cmdD.CommandTimeout = 0;
                            cmdD.CommandType = CommandType.StoredProcedure;
                            cmdD.CommandText = "Movil_Log_HistoricoPrecio";
                            cmdD.Parameters.Add("@Id_Articulo", SqlDbType.VarChar).Value = o.articulo;
                            SqlDataReader drD = cmdD.ExecuteReader();
                            if (drD.HasRows)
                            {
                                List<OrdenDetalle> f = new List<OrdenDetalle>();
                                while (drD.Read())
                                {
                                    f.Add(new OrdenDetalle()
                                    {
                                        id = drD.GetInt32(0),
                                        materialId = drD.GetString(1),
                                        fecha = drD.GetDateTime(2).ToString("dd/MM/yyyy"),
                                        proveedor = drD.GetString(3),
                                        razonSocial = drD.GetString(4),
                                        precio = Convert.ToDecimal(drD.GetDecimal(5))
                                    });
                                }
                                o.detalles = f;
                            }
                            p.Add(o);
                        }
                    }
                    con.Close();
                }
                return p;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<Anular> GetAnular(string usuario, string fi, string ff)
        {
            try
            {
                List<Anular> p = null;
                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DSIGE_Proy_M_Orden_Compra_Anulacion_Listar";
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = usuario;
                    cmd.Parameters.Add("@FechaIni", SqlDbType.VarChar).Value = fi;
                    cmd.Parameters.Add("@FechaFin", SqlDbType.VarChar).Value = ff;
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        p = new List<Anular>();
                        while (dr.Read())
                        {
                            Anular o = new Anular
                            {
                                id = dr.GetInt32(0),
                                ordenId = dr.GetInt32(1),
                                tipoPedidoId = dr.GetString(2),
                                toc = dr.GetString(3),
                                nroOrden = dr.GetString(4),
                                fechaEmisionOrden = dr.GetDateTime(5).ToString("dd/MM/yyyy"),
                                contableOt = dr.GetString(6),
                                usuSolicitante = dr.GetString(7),
                                usuCreaOrden = dr.GetString(8),
                                fechaCreaOrden = dr.GetDateTime(9).ToString("dd/MM/yyyy"),
                                fechaAprobacion = dr.GetDateTime(10).ToString("dd/MM/yyyy"),
                                provee = dr.GetString(11),
                                forma = dr.GetString(12),
                                mone = dr.GetString(13),
                                subtotalOc = Convert.ToDecimal(dr.GetDecimal(14)),
                                igv = Convert.ToDecimal(dr.GetDecimal(15)),
                                totalOc = Convert.ToDecimal(dr.GetDecimal(16)),
                                codEstado = dr.GetString(17),
                                estado = dr.GetString(18),
                                item = dr.GetInt32(19),
                                articulo = dr.GetString(20),
                                nombreArticulo = dr.GetString(21),
                                cantidadAprobada = Convert.ToDecimal(dr.GetDecimal(22)),
                                precio = Convert.ToDecimal(dr.GetDecimal(23)),
                                importe = Convert.ToDecimal(dr.GetDecimal(24))
                            };
                            p.Add(o);
                        }
                    }
                    con.Close();
                }
                return p;
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        public static Mensaje SaveRequerimiento(Requerimiento s)
        {
            try
            {
                Mensaje mensaje = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();

                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Movil_Log_Pedidos_Cab_InsertarActualizar";
                    cmd.Parameters.Add("@Estado", SqlDbType.VarChar).Value = s.estado;
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = s.usuario;
                    cmd.Parameters.Add("@TipoOpe", SqlDbType.Int).Value = s.identity;
                    cmd.Parameters.Add("@Log_PedC_Tipo", SqlDbType.VarChar).Value = s.tipoSolicitud;
                    cmd.Parameters.Add("@Log_PedC_Numero", SqlDbType.VarChar).Value = s.nroSolicitud;
                    cmd.Parameters.Add("@Ges_Dele_Codigo", SqlDbType.VarChar).Value = s.codigoDelegacion;
                    cmd.Parameters.Add("@Ges_Proy_Codigo", SqlDbType.VarChar).Value = s.codigoCentroCosto;
                    cmd.Parameters.Add("@Ges_Obra_Codigo", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@Ges_OrdT_Codigo", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@Ges_Empl_Dni", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@Log_PedC_Fecha", SqlDbType.VarChar).Value = s.fecha;
                    cmd.Parameters.Add("@Log_PedC_Observacion", SqlDbType.VarChar).Value = s.observaciones;
                    cmd.Parameters.Add("@Log_PedC_Flag", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@Pub_Mone_Codigo", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@Log_PedC_TipoCambio", SqlDbType.Decimal).Value = 0.0;
                    cmd.Parameters.Add("@Log_PedC_FechaAtencion", SqlDbType.VarChar).Value = s.fecha;
                    cmd.Parameters.Add("@Log_PedC_HoraAtencion", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@Log_PedCDireccionAtencion", SqlDbType.VarChar).Value = "";
                    cmd.ExecuteNonQuery();

                    foreach (var b in s.detalle)
                    {
                        SqlCommand cmdB = con.CreateCommand();
                        cmdB.CommandTimeout = 0;
                        cmdB.CommandType = CommandType.StoredProcedure;
                        cmdB.CommandText = "movil_Log_Pedidos_Det_InsertarActualizar";
                        cmdB.Parameters.Add("@Estado", SqlDbType.VarChar).Value = "";
                        cmdB.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = s.usuario;
                        cmdB.Parameters.Add("@TipoOpe", SqlDbType.Int).Value = b.identity;

                        cmdB.Parameters.Add("@Log_PedC_Tipo", SqlDbType.VarChar).Value = "";
                        cmdB.Parameters.Add("@Log_PedC_Numero", SqlDbType.VarChar).Value = "";
                        cmdB.Parameters.Add("@Alm_Arti_Codigo", SqlDbType.VarChar).Value = b.material;

                        cmdB.Parameters.Add("@Log_PedC_Item", SqlDbType.Int).Value = 0;
                        cmdB.Parameters.Add("@Log_PedD_Cant_Pedido", SqlDbType.Decimal).Value = b.cantidad;
                        cmdB.Parameters.Add("@Log_PedD_Cant_Aprobado1", SqlDbType.Decimal).Value = b.cantidad;
                        cmdB.Parameters.Add("@Log_PedD_Cant_Aprobado2", SqlDbType.Decimal).Value = b.cantidad;

                        cmdB.Parameters.Add("@Alm_UMed_Codigo", SqlDbType.VarChar).Value = b.um;
                        cmdB.Parameters.Add("@Log_PedD_Precio", SqlDbType.Decimal).Value = 0.0;
                        cmdB.Parameters.Add("@Log_PedD_Importe", SqlDbType.Decimal).Value = 0.0;
                        cmdB.Parameters.Add("@Log_PedD_CodVehiculo", SqlDbType.VarChar).Value = "";
                        cmdB.Parameters.Add("@Log_PedD_Obs", SqlDbType.VarChar).Value = b.descripionMaterial;
                        cmdB.Parameters.Add("@Log_PedD_Obra", SqlDbType.VarChar).Value = "";
                        cmdB.Parameters.Add("@Log_PedD_OtContable", SqlDbType.VarChar).Value = "";
                        cmdB.ExecuteNonQuery();
                    }
                    mensaje = new Mensaje
                    {
                        mensaje = "Registro Guardado",
                        codigoBase = s.requerimientoId
                    };

                    con.Close();
                }
                return mensaje;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Mensaje UpdateCantidad(Query s)
        {
            try
            {
                Mensaje mensaje = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();

                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DSIGE_Proy_M_Pedidos_OrdenCompra_ActualizarDetalle";
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = s.usuarioId;
                    cmd.Parameters.Add("@log_Pedd_identidad", SqlDbType.Int).Value = s.detalleId;
                    cmd.Parameters.Add("@Matricula", SqlDbType.VarChar).Value = s.matricula;
                    cmd.Parameters.Add("@CantidadAprobada", SqlDbType.Decimal).Value = s.cantidad;
                    cmd.ExecuteNonQuery();

                    mensaje = new Mensaje
                    {
                        mensaje = "Actualizado",
                        codigoBase = s.detalleId
                    };

                    con.Close();
                }
                return mensaje;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
		
		    public static Mensaje UpdateCantidadCampoJefeTiempoVida(Query s)
        {
            try
            {
                Mensaje mensaje = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();

                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DSIGE_Proy_M_Pedidos_OrdenCompra_ActualizarDetalle";
                    cmd.Parameters.Add("@TipoFormulario", SqlDbType.VarChar).Value = s.tipoProceso;
					cmd.Parameters.Add("@Cantidad", SqlDbType.Decimal).Value = s.cantidad;
                    cmd.Parameters.Add("@Alm_PedD_Identidad", SqlDbType.Int).Value = s.detalleId;
                    cmd.ExecuteNonQuery();

                    mensaje = new Mensaje
                    {
                        mensaje = "Actualizado",
                        codigoBase = s.detalleId
                    };

                    con.Close();
                }
                return mensaje;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Mensaje AprobacionOrdenCompra(Query s)
        {
            try
            {
                Mensaje mensaje = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DSIGE_Proy_M_Aprobacion_Orden_Compra_Aprueba_Rechaza";
                    cmd.Parameters.Add("@TipoProceso", SqlDbType.VarChar).Value = s.tipoProceso;
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = s.usuarioId;
                    cmd.Parameters.Add("@Log_OCom_Identidad", SqlDbType.Int).Value = s.detalleId;
                    cmd.Parameters.Add("@OBS_Rechazo", SqlDbType.VarChar).Value = s.search;
                    cmd.ExecuteNonQuery();

                    mensaje = new Mensaje
                    {
                        mensaje = "Actualizado",
                        codigoBase = s.detalleId
                    };

                    con.Close();
                }
                return mensaje;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Mensaje AprobacionPedido(Query s)
        {
            try
            {
                Mensaje mensaje = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DSIGE_Proy_M_Pedidos_OrdenCompra_Rechazar";
                    cmd.Parameters.Add("@TipoProceso", SqlDbType.VarChar).Value = s.tipoProceso;
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = s.usuarioId;
                    cmd.Parameters.Add("@Log_OCom_Identidad", SqlDbType.Int).Value = s.detalleId;
                    cmd.Parameters.Add("@OBS_Rechazo", SqlDbType.VarChar).Value = s.search;
                    cmd.ExecuteNonQuery();

                    mensaje = new Mensaje
                    {
                        mensaje = "Actualizado",
                        codigoBase = s.detalleId
                    };

                    con.Close();
                }
                return mensaje;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
		
		
        public static Mensaje AprobacionCampoJefeTiempoVida(Query s)
        {
            try
            {
                Mensaje mensaje = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DSIGE_Proy_M_Pedidos_OrdenCompra_Rechazar";
                    cmd.Parameters.Add("@TipoProceso", SqlDbType.VarChar).Value = s.tipoProceso;
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = s.usuarioId;
                    cmd.Parameters.Add("@Log_OCom_Identidad", SqlDbType.Int).Value = s.detalleId;
                    cmd.Parameters.Add("@OBS_Rechazo", SqlDbType.VarChar).Value = s.search;
                    cmd.ExecuteNonQuery();

                    mensaje = new Mensaje
                    {
                        mensaje = "Actualizado",
                        codigoBase = s.detalleId
                    };

                    con.Close();
                }
                return mensaje;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Mensaje AnulacionEvento(Query s)
        {
            try
            {
                Mensaje mensaje = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DSIGE_Proy_M_Orden_Compra_Anulacion_Evento";
                    cmd.Parameters.Add("@Log_OCom_Identidad", SqlDbType.Int).Value = s.detalleId;
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = s.usuarioId;
                    cmd.Parameters.Add("@Perfil", SqlDbType.VarChar).Value = s.matricula;
                    cmd.Parameters.Add("@OBS_Rechazo", SqlDbType.VarChar).Value = s.search;
                    cmd.ExecuteNonQuery();

                    mensaje = new Mensaje
                    {
                        mensaje = "Actualizado",
                        codigoBase = s.detalleId
                    };

                    con.Close();
                }
                return mensaje;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<CampoJefe> GetCampoJefe(Query r)
        {
            try
            {
                List<CampoJefe> p = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DSIGE_Proy_M_PedidosColaborador_Lista";
                    cmd.Parameters.Add("@Delegacion", SqlDbType.VarChar).Value = r.delegacionId;
                    cmd.Parameters.Add("@Sucursal", SqlDbType.VarChar).Value = r.sucursalId;
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = r.usuarioId;
                    cmd.Parameters.Add("@Almacen", SqlDbType.VarChar).Value = r.almacenId;
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        p = new List<CampoJefe>();
                        while (dr.Read())
                        {
                            p.Add(new CampoJefe()
                            {
                                id = dr.GetInt32(0),
                                idDet = dr.GetInt32(1),
                                pubSucuCodigo = dr.GetString(2),
                                almaCodigo = dr.GetString(3),
                                obraCodigo = dr.GetString(4),
                                pedcNumero = dr.GetString(5),
                                pedcFechaEnvio = dr.GetDateTime(6).ToString("dd/MM/yyyy HH:mm:ss"),
                                pedcFolioOrigen = dr.GetString(7),
                                nomApellidos = dr.GetString(8),
                                pedcFechaCreada = dr.GetDateTime(9).ToString("dd/MM/yyyy HH:mm:ss"),
                                usuarioCrea = dr.GetString(10),
                                nombreOt = dr.GetString(11),
                                almaDescripcion = dr.GetString(12),
                                nombreEstado = dr.GetString(13),
                                articulo = dr.GetString(14),
                                nombreArticulo = dr.GetString(15),
                                cantidadPedida = Convert.ToDecimal(dr.GetDecimal(16)),
                                cantidadAprobada = Convert.ToDecimal(dr.GetDecimal(17)),
                                guicGuiaRemision = dr.GetString(18),
                                guiaRemision = dr.GetString(19)
                            });
                        }
                    }
                    con.Close();
                }

                return p;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public static List<TiempoVida> GetTiempoVida(Query r)
        {
            try
            {
                List<TiempoVida> p = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DSIGE_Proy_M_PedidosTiempoVida_Lista";
                    cmd.Parameters.Add("@Delegacion", SqlDbType.VarChar).Value = r.delegacionId;
                    cmd.Parameters.Add("@Sucursal", SqlDbType.VarChar).Value = r.sucursalId;
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = r.usuarioId;
                    cmd.Parameters.Add("@Almacen", SqlDbType.VarChar).Value = r.almacenId;                     
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        p = new List<TiempoVida>();
                        while (dr.Read())
                        {
                            p.Add(new TiempoVida()
                            {
                                id = dr.GetInt32(0),
                                idDet = dr.GetInt32(1),
                                pubSucuCodigo = dr.GetString(2),
                                almaCodigo = dr.GetString(3),
                                obraCodigo = dr.GetString(4),
                                pedcNumero = dr.GetString(5),
                                pedcFechaEnvio = dr.GetDateTime(6).ToString("dd/MM/yyyy HH:mm:ss"),
                                pedcFolioOrigen = dr.GetString(7),
                                nomApellidos = dr.GetString(8),
                                pedcFechaCreada = dr.GetDateTime(9).ToString("dd/MM/yyyy HH:mm:ss"),
                                usuarioCrea = dr.GetString(10),
                                nombreOt = dr.GetString(11),
                                almaDescripcion = dr.GetString(12),
                                nombreEstado = dr.GetString(13),
                                articulo = dr.GetString(14),
                                nombreArticulo = dr.GetString(15),
                                cantidadPedida = Convert.ToDecimal(dr.GetDecimal(16)),
                                cantidadAprobada = Convert.ToDecimal(dr.GetDecimal(17)),
                                guicGuiaRemision = dr.GetString(18),
                                guiaRemision = dr.GetString(19)
                            });
                        }
                    }
                    con.Close();
                }


                return p;
            }
            catch (Exception e)
            {

                throw e;
            }
        }


        public static Mensaje GetAprobarItemsCampoJefeTiempoVida(Query s)
        {
            try
            {
                Mensaje mensaje = null;

                using (SqlConnection con = new SqlConnection(db))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DSIGE_Proy_M_Pedidos_OrdenCompra_ActualizarDetalle_Masivo";
                    cmd.Parameters.Add("@TipoFormulario", SqlDbType.VarChar).Value = s.tipoProceso;
                    cmd.Parameters.Add("@log_Pedd_identidad", SqlDbType.Int).Value = s.detalleId;
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = s.usuarioId;                   
                    cmd.ExecuteNonQuery();

                    mensaje = new Mensaje
                    {
                        mensaje = "Actualizado",
                        codigoBase = s.detalleId
                    };

                    con.Close();
                }
                return mensaje;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}

