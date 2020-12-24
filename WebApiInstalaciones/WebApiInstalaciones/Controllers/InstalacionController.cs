using Entidades;
using Negocio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Http;

namespace WebApiInstalaciones.Controllers
{

    [RoutePrefix("api/Proyectos")]
    public class InstalacionController : ApiController
    {

        private static string path = ConfigurationManager.AppSettings["uploadFile"];

        [HttpPost]
        [Route("Login")]
        public IHttpActionResult GetLogin(Query q)
        {
            Usuario u = NegocioDao.GetOne(q);

            if (u != null)
            {
                if (u.mensaje == "Pass")
                    return BadRequest("Contraseña Incorrecta");
                else
                    return Ok(u);
            }
            else return BadRequest("Usuario no existe");

        }

        [HttpGet]
        [Route("Encriptar")]
        public IHttpActionResult GetEncriptar(string nombre, bool activo)
        {
            string login = NegocioDao.EncriptarClave(nombre, activo);
            return Ok(login);
        }

        [HttpPost]
        [Route("SyncComplete")]
        public IHttpActionResult GetSyncComplete(Query q)
        {
            try
            {
                //Query q
                return Ok(NegocioDao.GetSyncComplete(q));
            }
            catch (Exception)
            {
                return BadRequest("No puedes Sincronizar");
            }
        }

        [HttpPost]
        [Route("SaveVehiculo")]
        public IHttpActionResult SaveVehiculo()
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath("~/Imagen/");
                var files = HttpContext.Current.Request.Files;
                var testValue = HttpContext.Current.Request.Form["data"];
                Vehiculo v = JsonConvert.DeserializeObject<Vehiculo>(testValue);
                Mensaje m = NegocioDao.SaveVehiculo(v);
                if (m != null)
                {

                    for (int i = 0; i < files.Count; i++)
                    {
                        string fileName = Path.GetFileName(files[i].FileName);
                        files[i].SaveAs(path + fileName);
                    }

                    return Ok(m);
                }
                else
                    return BadRequest("Error");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // Save Registro Parte Diario
        [HttpPost]
        [Route("SaveRegistro")]
        public IHttpActionResult SaveRegistro()
        {
            try
            {
                //string path = HttpContext.Current.Server.MapPath("~/Imagen/");
                var files = HttpContext.Current.Request.Files;
                var testValue = HttpContext.Current.Request.Form["data"];
                Registro r = JsonConvert.DeserializeObject<Registro>(testValue);
                Mensaje m = NegocioDao.SaveRegistro(r);
                if (m != null)
                {

                    for (int i = 0; i < files.Count; i++)
                    {
                        string fileName = Path.GetFileName(files[i].FileName);
                        files[i].SaveAs(path + fileName);
                    }

                    return Ok(m);
                }
                else
                    return BadRequest("Error");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("SaveOperarioGps")]
        public IHttpActionResult SaveOperarioGps(EstadoOperario o)
        {
            Mensaje m = NegocioDao.SaveOperarioGps(o);
            if (m != null)
                return Ok(m);
            else
                return BadRequest("Error");
        }

        [HttpPost]
        [Route("SaveEstadoMovil")]
        public IHttpActionResult SaveEstadoMovil(EstadoMovil e)
        {
            Mensaje m = NegocioDao.SaveEstadoMovil(e);
            if (m != null)
                return Ok(m);
            else
                return BadRequest("Error");
        }

        // Parte Diario

        [HttpPost]
        [Route("SaveParteDiario")]
        public IHttpActionResult SaveParteDiario()
        {
            try
            {
                //string path = HttpContext.Current.Server.MapPath("~/Imagen/");
                var files = HttpContext.Current.Request.Files;
                var testValue = HttpContext.Current.Request.Form["data"];
                ParteDiario p = JsonConvert.DeserializeObject<ParteDiario>(testValue);

                for (int i = 0; i < files.Count; i++)
                {
                    string fileName = Path.GetFileName(files[i].FileName);
                    files[i].SaveAs(path + fileName);
                }

                Mensaje mensaje = NegocioDao.SaveRegistroParteDiario(p);
                return Ok(mensaje);
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        // SOLICITUD

        [HttpPost]
        [Route("SaveGeneralSolicitud")]
        public IHttpActionResult SaveGeneralSolicitud(Solicitud s)
        {
            Mensaje m = NegocioDao.SaveRegistroGeneral(s);
            if (m != null)
            {
                return Ok(m);
            }
            else
                return BadRequest("Error");
        }

        [HttpPost]
        [Route("SaveRegistroSolicitudMaterial")]
        public IHttpActionResult SaveRegistroDetalle(RegistroMaterialSolicitud r)
        {
            Mensaje m = NegocioDao.SaveRegistroDetalle(r);
            if (m != null)
            {
                return Ok(m);
            }
            else
                return BadRequest("Error Verificar");

        }

        [HttpPost]
        [Route("SaveRegistroPhoto")]
        public IHttpActionResult SaveRegistroPhoto()
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath("~/Imagen/");
                var files = HttpContext.Current.Request.Files;
                var testValue = HttpContext.Current.Request.Form["data"];
                RegistroPhoto r = JsonConvert.DeserializeObject<RegistroPhoto>(testValue);

                if (r.tipo == 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        string fileName = Path.GetFileName(files[i].FileName);
                        files[i].SaveAs(path + fileName);
                    }
                }
                else
                {
                    string fileName = Path.GetFileName(files[0].FileName);
                    string foto = path + fileName;
                    if (File.Exists(foto))
                    {
                        try
                        {
                            File.Delete(foto);
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    }
                }

                Mensaje mensaje = NegocioDao.SaveRegistroPhoto(r);
                return Ok(mensaje);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // STOCK SOLICITUD

        [HttpPost]
        [Route("GetPaginationSolicitud")]
        public IHttpActionResult GetPaginationSolicitud(Query q)
        {
            List<Solicitud> m = NegocioDao.GetSolicitudes(q);
            if (m != null)
            {
                return Ok(m);
            }
            else
                return BadRequest("No hay Solicitudes");
        }

        // STOCK DE MATERIALES
        [HttpPost]
        [Route("GetStockMaterial")]
        public IHttpActionResult GetStockMaterial(Query q)
        {
            List<Materiales> m = NegocioDao.GetStockMaterial(q);
            if (m != null)
            {
                return Ok(m);
            }
            else
            {
                return BadRequest("No hay Datos");
            }
        }

        [HttpPost]
        [Route("Aprobation")]
        public IHttpActionResult Aprobation(Query q)
        {
            Mensaje m = NegocioDao.GetAprobation(q);
            if (m != null)
            {
                return Ok(m);
            }
            else
            {
                return BadRequest("Error verificar");
            }
        }


        // LOGISTICA

        [HttpGet]
        [Route("PedidosCompra")]
        public IHttpActionResult PedidosCompra(string usuario)
        {
            try
            {
                List<Pedido> m = NegocioDao.GetPedidosCompra(usuario);
                if (m != null)
                    return Ok(m);
                else
                    return BadRequest("No hay datos");
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        [HttpPost]
        [Route("OrdenCompra")]
        public IHttpActionResult OrdenCompra(Query q)
        {
            try
            {
                List<Orden> m = NegocioDao.GetOrdenCompra(q);
                if (m != null)
                    return Ok(m);
                else
                    return BadRequest("No hay datos");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        [Route("GetAnulacion")]
        public IHttpActionResult GetAnulacion(string usuario, string fi, string ff)
        {
            try
            {
                List<Anular> m = NegocioDao.GetAnular(usuario, fi, ff);
                if (m != null)
                    return Ok(m);
                else
                    return BadRequest("No hay datos");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("SaveRequerimiento")]
        public IHttpActionResult SaveRequerimiento(Requerimiento r)
        {
            try
            {
                Mensaje m = NegocioDao.SaveRequerimiento(r);
                if (m != null)
                    return Ok(m);
                else
                    return BadRequest("No hay datos");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("UpdateCantidad")]
        public IHttpActionResult UpdateCantidad(Query r)
        {
            try
            {
                Mensaje m = NegocioDao.UpdateCantidad(r);
                if (m != null)
                    return Ok(m);
                else
                    return BadRequest("Error");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("UpdateCantidadCampoJefeTiempoVida")]
        public IHttpActionResult UpdateCantidadCampoJefeTiempoVida(Query r)
        {
            try
            {
                Mensaje m = NegocioDao.UpdateCantidadCampoJefeTiempoVida(r);
                if (m != null)
                    return Ok(m);
                else
                    return BadRequest("Error");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("AprobacionOrdenCompra")]
        public IHttpActionResult AprobacionOrdenCompra(Query r)
        {
            try
            {
                Mensaje m = NegocioDao.AprobacionOrdenCompra(r);
                if (m != null)
                    return Ok(m);
                else
                    return BadRequest("Error");
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        [HttpPost]
        [Route("AprobacionPedido")]
        public IHttpActionResult AprobacionPedido(Query r)
        {
            try
            {
                Mensaje m = NegocioDao.AprobacionPedido(r);
                if (m != null)
                    return Ok(m);
                else
                    return BadRequest("Error");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("AprobacionCampoJefeTiempoVida")]
        public IHttpActionResult AprobacionCampoJefeTiempoVida(Query r)
        {
            try
            {
                Mensaje m = NegocioDao.AprobacionCampoJefeTiempoVida(r);
                if (m != null)
                    return Ok(m);
                else
                    return BadRequest("Error");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("AnularEvento")]
        public IHttpActionResult AnularEvento(Query r)
        {
            try
            {
                Mensaje m = NegocioDao.AnulacionEvento(r);
                if (m != null)
                    return Ok(m);
                else
                    return BadRequest("Error");
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        // nuevo
        [HttpPost]
        [Route("CampoJefe")]
        public IHttpActionResult CampoJefe(Query r)
        {
            try
            {
                List<CampoJefe> m = NegocioDao.GetCampoJefe(r);
                if (m != null)
                    return Ok(m);
                else
                    return BadRequest("No hay datos");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("TiempoVida")]
        public IHttpActionResult TiempoVida(Query r)
        {
            try
            {
                List<TiempoVida> m = NegocioDao.GetTiempoVida(r);
                if (m != null)
                    return Ok(m);
                else
                    return BadRequest("No hay datos");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("AprobarItemsCampoJefeTiempoVida")]
        public IHttpActionResult AprobarItemsCampoJefeTiempoVida(Query r)
        {
            try
            {
                Mensaje m = NegocioDao.GetAprobarItemsCampoJefeTiempoVida(r);
                if (m != null)
                    return Ok(m);
                else
                    return BadRequest("No hay datos");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        



    }
}
