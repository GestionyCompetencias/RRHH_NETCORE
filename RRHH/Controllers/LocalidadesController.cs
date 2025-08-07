using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace RRHH.Controllers
{
    public class LocalidadesController : Controller
    {
        private Seguridad seguridad = new Seguridad();

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }


        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        string RutEmpresa = "";

        [HttpGet]
        [Route("ConsultarPaises")]
        public async Task<JsonResult> ConsultarPaises(int empresa)
        {
            LocalidadesVM paises = new LocalidadesVM();
            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {

                string empresaX = HttpContext.Session.GetString("EmpresaLog");

                RutEmpresa = f.obtenerRUT(empresa);
                //var BD_Cli = "Contable_" + RutEmpresa;
                var BD_Cli = "Contable";

                f.EjecutarConsultaSQLCli("select * from paises ", BD_Cli);

                List<LocalidadesVM> opcionesList = new List<LocalidadesVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen paises en la coleccion";
                    resultado.result = 1;

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new LocalidadesVM()
                                    {
                                        idPais = int.Parse(dr["id"].ToString()),
                                        nombre = dr["nombre"].ToString()
                                    }).ToList();

                    resultado.data = opcionesList;

                }
                else
                {
                    resultado.mensaje = "No se han creado paises";
                    resultado.result = 0;
                }
                return Json(new { info = resultado });
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar Paises";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar los paises", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return Json(new { info = paises });
            }


        }


        [HttpGet]
        [Route("ConsultaPais/{id:int}")]
        public async Task<JsonResult> ConsultaPais(int id, int empresa)
        {
            LocalidadesVM paises = new LocalidadesVM();
            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {

                string empresaX = HttpContext.Session.GetString("EmpresaLog");

                RutEmpresa = f.obtenerRUT(int.Parse(empresaX));
                //var BD_Cli = "Contable_" + RutEmpresa;
                var BD_Cli = "Contable";

                f.EjecutarConsultaSQLCli("select * from paises where id=" + id + " ", BD_Cli);

                List<LocalidadesVM> opcionesList = new List<LocalidadesVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen paises en la coleccion";
                    resultado.result = 1;

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new LocalidadesVM()
                                    {
                                        idPais = int.Parse(dr["id"].ToString()),
                                        nombre = dr["nombre"].ToString()
                                    }).ToList();

                    resultado.data = opcionesList;

                }
                else
                {
                    resultado.mensaje = "No se han creado personas";
                    resultado.result = 0;
                }
                return Json(new { info = resultado });
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar Personas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar una persona", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return Json(new { info = paises });
            }


        }

        /// <summary>
        /// consultar regiones por pais
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ConsultarRegiones")]
        public async Task<JsonResult> ConsultarRegiones(int idpais, int empresa)
        {
            RegionesVM regiones = new RegionesVM();
            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {

                string empresaX = HttpContext.Session.GetString("EmpresaLog");

                RutEmpresa = f.obtenerRUT(empresa);
                //var BD_Cli = "Contable_" + RutEmpresa;
                var BD_Cli = "Contable";

                f.EjecutarConsultaSQLCli("select * from regiones where idpais=" + idpais + " ", BD_Cli);

                List<RegionesVM> opcionesList = new List<RegionesVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen regiones en la coleccion";
                    resultado.result = 1;

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new RegionesVM()
                                    {
                                        idPais = int.Parse(dr["idpais"].ToString()),
                                        idRegion = int.Parse(dr["id"].ToString()),
                                        nombre = dr["nombre"].ToString()
                                    }).ToList();

                    resultado.data = opcionesList;

                }
                else
                {
                    resultado.mensaje = "No se han creado personas";
                    resultado.result = 0;
                }
                return Json(new { info = resultado });
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar Personas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar una persona", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return Json(new { info = regiones });
            }

        }

        [HttpGet]
        [Route("ConsultaRegion/{id:int}")]
        public async Task<JsonResult> ConsultaRegion(int id, int empresa)
        {
            RegionesVM regiones = new RegionesVM();
            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {

                string empresaX = HttpContext.Session.GetString("EmpresaLog");

                RutEmpresa = f.obtenerRUT(empresa);
                //var BD_Cli = "Contable_" + RutEmpresa;
                var BD_Cli = "Contable";

                f.EjecutarConsultaSQLCli("select * from regiones where id=" + id + " ", BD_Cli);

                List<RegionesVM> opcionesList = new List<RegionesVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen regiones en la coleccion";
                    resultado.result = 1;

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new RegionesVM()
                                    {
                                        idPais = int.Parse(dr["idpais"].ToString()),
                                        idRegion = int.Parse(dr["id"].ToString()),
                                        nombre = dr["nombre"].ToString()
                                    }).ToList();

                    resultado.data = opcionesList;

                }
                else
                {
                    resultado.mensaje = "No se han creado personas";
                    resultado.result = 0;
                }
                return Json(new { info = resultado });
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar Personas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar una persona", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return Json(new { info = regiones });
            }

        }

        [HttpGet]
        [Route("ConsultarComunas")]
        public async Task<JsonResult> ConsultarComunas(int idregion, int empresa)
        {
            ComunaVM comunas = new ComunaVM();
            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {

                string empresaX = HttpContext.Session.GetString("EmpresaLog");

                RutEmpresa = f.obtenerRUT(empresa);
                //var BD_Cli = "Contable_" + RutEmpresa;
                var BD_Cli = "Contable";


                f.EjecutarConsultaSQLCli("select * from comunas where idregion=" + idregion + " ", BD_Cli);

                List<ComunaVM> opcionesList = new List<ComunaVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen regiones en la coleccion";
                    resultado.result = 1;

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new ComunaVM()
                                    {
                                        idComuna = int.Parse(dr["id"].ToString()),
                                        idRegion = int.Parse(dr["idregion"].ToString()),
                                        nombre = dr["nombre"].ToString()
                                    }).ToList();

                    resultado.data = opcionesList;

                }
                else
                {
                    resultado.mensaje = "No se han creado personas";
                    resultado.result = 0;
                }
                return Json(new { info = resultado });
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar Personas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar una persona", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return Json(new { info = comunas });
            }

        }

        [HttpGet]
        [Route("ConsultaComuna/{id:int}")]
        public async Task<JsonResult> ConsultaComuna(int id, int empresa)
        {
            ComunaVM comunas = new ComunaVM();
            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {

                string empresaX = HttpContext.Session.GetString("EmpresaLog");

                RutEmpresa = f.obtenerRUT(empresa);
                //var BD_Cli = "Contable_" + RutEmpresa;
                var BD_Cli = "Contable";


                f.EjecutarConsultaSQLCli("select * from comunas where id=" + id + " ", BD_Cli);

                List<ComunaVM> opcionesList = new List<ComunaVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen regiones en la coleccion";
                    resultado.result = 1;

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new ComunaVM()
                                    {
                                        idComuna = int.Parse(dr["id"].ToString()),
                                        idRegion = int.Parse(dr["idregion"].ToString()),
                                        nombre = dr["nombre"].ToString()
                                    }).ToList();

                    resultado.data = opcionesList;

                }
                else
                {
                    resultado.mensaje = "No se han creado personas";
                    resultado.result = 0;
                }
                return Json(new { info = resultado });
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar Personas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar una persona", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return Json(new { info = comunas });
            }

        }

    }
}
