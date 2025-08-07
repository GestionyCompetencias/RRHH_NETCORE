using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;

namespace RRHH.Controllers
{
    public class PerfilUsuarioController : Controller
    {
        private Seguridad seguridad = new Seguridad();


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Personal", "Empresa");
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Generales Generales = new Generales();
        string RutEmpresa = "";

        [HttpPost]
        [Route("PerfilUsuario")]
        public async Task<JsonResult> PerfilUsuario(string usuario, int perfil)
        {
            UsuarioVM perfiles = new UsuarioVM();
            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {
                var BD_Cli = "contable";
                f.EjecutarConsultaSQLCli("SELECT * " +
                            "FROM usuarios " +
                            "where usuarios.habilitado = 1 and usuarios.nombreUsu ='"+usuario+"' ", BD_Cli);


                List<PersonasBaseVM> pers = new List<PersonasBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen usuario en la coleccion";
                    resultado.result = 1;


                    if (usuario != null)
                    {
                        //var infoclie = db.CLIENTE.Where(x => x.RUT == Trabajador).ToList();
                        //f.EjecutarConsultaSQLCli("SELECT personas.id,personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf, " +
                        //           "trabajadores.id, trabajadores.idregion, trabajadores.idcomuna,  " +
                        //           "trabajadores.nacimiento, trabajadores.sexo, trabajadores.nrohijos,  " +
                        //           "trabajadores.direccion,  trabajadores.idpais " +
                        //           "FROM personas " +
                        //           "inner join trabajadores on personas.id = trabajadores.idpersona " +
                        //           "where trabajadores.habilitado = 1 and persona.rut ='" + rut + "' ", BD_Cli);


                        //List<PersonasBaseVM> pers = new List<PersonasBaseVM>();
                        //if (f.Tabla.Rows.Count > 0)
                        //{
                        //    if (infoclie.Count == 0)
                        //{
                        //    clie.RUT = rut;
                        //    clie.APATERNO = pers.APATERNO;
                        //    clie.AMATERNO = pers.AMATERNO;
                        //    clie.NOMBRES = pers.NOMBRE;
                        //    clie.CONTACTO = pers.CORREO;
                        //    db.CLIENTE.Add(clie);
                        //    db.SaveChanges();
                        //}
                        //if (perfil > 11) { return View(); }
                        //var infousu = db.USUARIO.Where(x => x.RUT == Trabajador).ToList();
                        //if (infousu.Count == 0)
                        //{
                        //    usua.RUT = Trabajador;
                        //    usua.CONTRASENA = f.GetMD5Hash(Trabajador.Substring(0, 6));
                        //    usua.HABILITADO = true;
                        //    db.USUARIO.Add(usua);
                        //    db.SaveChanges();
                        //}
                        //var infoperm = db.PERMISO.Where(x => x.RUT == Trabajador && x.EMPRESA == empresa).ToList();
                        //if (infoperm.Count != 0)
                        //{
                        //    perm = infoperm.Last();
                        //    perm.PERMISO1 = perfil;
                        //    db.SaveChanges();
                        //}
                        //else
                        //{
                        //    perm.RUT = Trabajador;
                        //    perm.EMPRESA = empresa;
                        //    perm.PERMISO1 = perfil;
                        //    db.PERMISO.Add(perm);
                        //    db.SaveChanges();
                        //}
                        //var rem1 = db.PERMISO_V2.Where(x => x.EMPRESA == empresa && x.RUT == Trabajador).ToList();
                        //if (rem1.Count != 0)
                        //{
                        //    foreach (var d1 in rem1)
                        //    {
                        //        db.PERMISO_V2.Remove(d1);
                        //        db.SaveChanges();
                        //    }

                        //}
                        //PERMISO_V2 permV2 = new PERMISO_V2();
                        //var infopermv20 = db.PERMISO_V2.Where(x => x.RUT == "69393527" && x.EMPRESA == empresa).ToList();
                        //foreach (var v2 in infopermv20)
                        //{
                        //    permV2.RUT = Trabajador;
                        //    permV2.EMPRESA = empresa;
                        //    permV2.PERMISO = v2.PERMISO;
                        //    permV2.SERVICIO = v2.SERVICIO;
                        //    db.PERMISO_V2.Add(permV2);
                        //    db.SaveChanges();
                        //}
                        //var rem2 = db.SERVICIOCONTRATADO.Where(x => x.EMPRESA == empresa && x.USUARIO == Trabajador).ToList();
                        //if (rem2.Count != 0)
                        //{
                        //    foreach (var d2 in rem2)
                        //    {
                        //        db.SERVICIOCONTRATADO.Remove(d2);
                        //        db.SaveChanges();
                        //    }

                        //}
                        //SERVICIOCONTRATADO serv = new SERVICIOCONTRATADO();
                        //var infoserv1 = db.SERVICIOCONTRATADO.Where(x => x.USUARIO == "69393527" && x.EMPRESA == empresa).ToList();
                        //foreach (var s in infoserv1)
                        //{
                        //    serv.USUARIO = Trabajador;
                        //    serv.EMPRESA = empresa;
                        //    serv.SERVICIO = s.SERVICIO;
                        //    serv.FINICIO = s.FINICIO;
                        //    serv.FTERMINO = s.FTERMINO;
                        //    serv.UNIDADES = s.UNIDADES;
                        //    serv.OBSERVACION = s.OBSERVACION;
                        //    db.SERVICIOCONTRATADO.Add(serv);
                        //    db.SaveChanges();

                        //}
                        ViewBag.exito = "S";
                    }

                }
                else
                {
                    resultado.mensaje = "No se han encontrado trabajadores";
                    resultado.result = 0;
                }


                return Json(new { info = resultado });
            }
            catch (Exception ex)
            {
                var Asunto = "Error al generar perfil de usuario";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar generar perfil de usuario", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return Json(new { info = perfiles });
            }

        }
    }
}

