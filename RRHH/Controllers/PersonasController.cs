using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace RRHH.Controllers
{
    public class PersonasController : Controller
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
        [Route("ConsultaPersona")]
        public async Task<JsonResult> ConsultaPersona(int id, int empresa)
        {
            UsuarioVM perfiles = new UsuarioVM();
            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {

                RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("select * from personas where id=" + id + " ", BD_Cli);

                List<PersonasBaseVM> opcionesList = new List<PersonasBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen peronas en la coleccion";
                    resultado.result = 1;

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new PersonasBaseVM()
                                    {
                                        Id = int.Parse(dr["id"].ToString()),
                                        Rut = dr["rut"].ToString(),
                                        Nombres = dr["nombres"].ToString(),
                                        Apellidos = dr["apellidos"].ToString(),
                                        Email = dr["email"].ToString()
                                    }).ToList();

                    resultado.data = opcionesList;

                }
                else
                {
                    resultado.mensaje = "No se han encontrado personas";
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
                return Json(new { info = perfiles });
            }

        }


        [HttpGet]
        [Route("ConsultaPersonaRut")]
        public async Task<JsonResult> ConsultaPersonaRut(string rut, int empresa)
        {
            UsuarioVM perfiles = new UsuarioVM();
            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {

                RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("select * from personas where rut='" + rut + "' ", BD_Cli);

                List<PersonasBaseVM> opcionesList = new List<PersonasBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen peronas en la coleccion";
                    resultado.result = 1;

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new PersonasBaseVM()
                                    {
                                        Id = int.Parse(dr["id"].ToString()),
                                        Rut = dr["rut"].ToString(),
                                        Nombres = dr["nombres"].ToString(),
                                        Apellidos = dr["apellidos"].ToString(),
                                        Email = dr["email"].ToString(),
                                        Tlf = dr["tlf"].ToString(),
                                    }).ToList();

                    resultado.data = opcionesList;

                }
                else
                {
                    resultado.mensaje = "No se han encontrado personas con ese numero de RUT";
                    resultado.result = 0;
                }
                return Json(new { info = resultado });
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar Personas Por RUT";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar una persona por RUT", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return Json(new { info = perfiles });
            }

        }
    }
}
