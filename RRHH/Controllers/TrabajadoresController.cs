using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;

namespace RRHH.Controllers
{
    public class TrabajadoresController : Controller
    {
        private Seguridad seguridad = new Seguridad();


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Personal", "Trabajador");
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

        [HttpGet]
        [Route("ConsultarTrabajadores")]
        public async Task<JsonResult> ConsultarTrabajadores(int empresa)
        {
            UsuarioVM perfiles = new UsuarioVM();
            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {

                RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT personas.id,personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf, " +
                                            "trabajadores.id, trabajadores.idregion, trabajadores.idcomuna,  " +
                                            "trabajadores.nacimiento, trabajadores.sexo, trabajadores.nrohijos,  " +
                                            "trabajadores.direccion,  trabajadores.idpais " +
                                            "FROM personas " +
                                            "inner join trabajadores on personas.id = trabajadores.idpersona " +
                                            "where trabajadores.habilitado = 1 ", BD_Cli);


                List<PersonasBaseVM> opcionesList = new List<PersonasBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen trabajadores en la coleccion";
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
                                        IdPais = int.Parse(dr["idpais"].ToString()),
                                        IdRegion = int.Parse(dr["idregion"].ToString()),
                                        IdComuna = int.Parse(dr["idcomuna"].ToString()),
                                        direccion = dr["direccion"].ToString(),
                                        nacimiento = dr["nacimiento"].ToString(),
                                        sexo = dr["sexo"].ToString(),
                                        nrohijos = int.Parse(dr["nrohijos"].ToString())
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {

                        r.Comuna = Generales.BuscaComuna(r.IdComuna);
                        r.Region = Generales.BuscaRegion(r.IdRegion);
                        r.Pais = Generales.BuscaPais(r.IdPais);
                    }

                    resultado.data = opcionesList;

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
                var Asunto = "Error al Consultar trabajadores";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar trabajadores", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return Json(new { info = perfiles });
            }

        }


        [HttpGet]
        [Route("ConsultaTrabajador")]
        public async Task<JsonResult> ConsultaTrabajador(string rut, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {

                string empresaX = HttpContext.Session.GetString("EmpresaLog");

                RutEmpresa = f.obtenerRUT(int.Parse(empresaX));
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT personas.id,personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf, " +
                                            "trabajadores.id, trabajadores.idregion,  trabajadores.idcomuna,  " +
                                            "trabajadores.nacimiento, trabajadores.sexo, trabajadores.nrohijos,  " +
                                            "trabajadores.direccion, trabajadores.idpais " +
                                            "FROM personas " +
                                            "inner join trabajadores on personas.id = trabajadores.idpersona " +
                                            " where personas.rut ='" + rut + "' and trabajadores.habilitado = 1 ", BD_Cli);

                List<PersonasBaseVM> opcionesList = new List<PersonasBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen trabajadores en la coleccion";
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
                                        IdPais = int.Parse(dr["idpais"].ToString()),
                                        IdRegion = int.Parse(dr["idregion"].ToString()),
                                        IdComuna = int.Parse(dr["idcomuna"].ToString()),
                                        direccion = dr["direccion"].ToString(),
                                        nacimiento = dr["nacimiento"].ToString(),
                                        sexo = dr["sexo"].ToString(),
                                        nrohijos = int.Parse(dr["nrohijos"].ToString())
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {

                        r.Comuna = Generales.BuscaComuna(r.IdComuna);
                        r.Region = Generales.BuscaRegion(r.IdRegion);
                        r.Pais = Generales.BuscaPais(r.IdPais);
                    }

                    resultado.data = opcionesList;

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
                var Asunto = "Error al Consultar trabajadores";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar un trabajador", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return Json(new { info = resultado });
            }

        }


        [HttpGet]
        [Route("ConsultaTrabajadorId")]
        public async Task<JsonResult> ConsultaTrabajadorId(int id, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {

                string empresaX = HttpContext.Session.GetString("EmpresaLog");

                RutEmpresa = f.obtenerRUT(int.Parse(empresaX));
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT personas.id,personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf, " +
                                            "trabajadores.id, trabajadores.idregion,  trabajadores.idcomuna,  trabajadores.idpais, " +
                                            "trabajadores.nacimiento, trabajadores.sexo, trabajadores.nrohijos,  " +
                                            "trabajadores.direccion, trabajadores.idpais " +
                                            "FROM personas " +
                                            "inner join trabajadores on personas.id = trabajadores.idpersona " +
                                            " where personas.id ='" + id + "' and trabajadores.habilitado = 1 ", BD_Cli);

                List<PersonasBaseVM> opcionesList = new List<PersonasBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen trabajadores en la coleccion";
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
                                        IdPais = int.Parse(dr["idpais"].ToString()),
                                        IdRegion = int.Parse(dr["idregion"].ToString()),
                                        IdComuna = int.Parse(dr["idcomuna"].ToString()),
                                        direccion = dr["direccion"].ToString(),
                                        nacimiento = dr["nacimiento"].ToString(),
                                        sexo = dr["sexo"].ToString(),
                                        nrohijos = int.Parse(dr["nrohijos"].ToString())
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        DateTime nacimiento = Convert.ToDateTime(r.nacimiento);
                        r.nacimiento = nacimiento.ToString("yyyy'-'MM'-'dd");
                        r.Comuna = Generales.BuscaComuna(r.IdComuna);
                        r.Region = Generales.BuscaRegion(r.IdRegion);
                        r.Pais = Generales.BuscaPais(r.IdPais);
                    }
                    resultado.data = opcionesList;

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
                var Asunto = "Error al Consultar trabajadores";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar un cliente", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return Json(new { info = resultado });
            }

        }


        [HttpPost]
        [Route("CrearTrabajador")]
        public JsonResult CrearTrabajador(PersonasBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string empresaX = HttpContext.Session.GetString("EmpresaLog");

            RutEmpresa = f.obtenerRUT(int.Parse(empresaX));
            var BD_Cli = "remuneracion_" + RutEmpresa;


            opciones.Region = opciones.Region?.ToString() ?? string.Empty;
            opciones.Apellidos = opciones.Apellidos?.ToString() ?? string.Empty;
            opciones.sexo = opciones.sexo?.ToString() ?? string.Empty;
            opciones.Email = opciones.Email?.ToString() ?? string.Empty;



            try
            {
                if (opciones.Rut == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar el RUT del trabajador";
                    return Json(new { info = resultado });
                }

                f.EjecutarConsultaSQLCli("select * from personas where rut='" + opciones.Rut + "'", BD_Cli);
                if (f.Tabla.Rows.Count > 0)
                {

                    int idPer = int.Parse(f.Tabla.Rows[0]["id"].ToString());

                    string query2 = "";


                    f.EjecutarConsultaSQLCli("update personas set rut='" + opciones.Rut + "', nombres='" + opciones.Nombres + "', email='" + opciones.Email + "', " +
                        "tlf='" + opciones.Tlf + "' where id=" + idPer + " ! ", BD_Cli);

                    f.EjecutarConsultaSQLCli("select * from trabajadores where idpersona = " + idPer + " ", BD_Cli);
                    if (f.Tabla.Rows.Count > 0)
                    {

                        query2 = "update trabajadores set [idregion]=" + opciones.IdRegion + ",[idcomuna]=" + opciones.IdComuna + ",[direccion]='" + opciones.direccion + "', " +
                                        " [nacimiento]='" + opciones.nacimiento + "',[sexo]='" + opciones.sexo + "',[nrohijos]=" + opciones.nrohijos +
                                        "  ,[idpais]="+opciones.IdPais + "  ,[habilitado]= 1" + 
                                        " where trabajadores.idpersona=" + idPer + "  ! ";

                        query2 += "update personas set [nombres]='" + opciones.Nombres + "',[apellidos]='" + opciones.Apellidos + "',[email]='" + opciones.Email + "', " +
                                            " [tlf]='" + opciones.Tlf + "' where personas.rut='" + opciones.Rut + "'  ! ";
                    }
                    else
                    {

                        query2 = "insert into trabajadores ([idpersona],[idregion],[idcomuna],[direccion], " +
                                " [nacimiento],[sexo],[nrohijos],[idpais],[habilitado]) " +
                                "values " +
                                "( " + idPer + "," + opciones.IdRegion + "," + opciones.IdComuna + ",'" + opciones.direccion + "'," +
                                 "  '" + opciones.nacimiento + "','" + opciones.sexo + "', " + opciones.nrohijos+ 
                                 ", " + opciones.IdPais+" ,1) ! ";
                    }



                    if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                    {
                        resultado.result = 1;
                        resultado.mensaje = "Proceso finalizado de manera exitosa.";
                    }
                    else
                    {
                        resultado.result = 0;
                        resultado.mensaje = "No se ingreso la información del trabajador.";
                    }

                }
                else
                {
                    string query1 = "insert into personas (rut,nombres,apellidos,email,tlf) " +
                    "values " +
                    "('" + opciones.Rut + "','" + opciones.Nombres + "','" + opciones.Apellidos + "','" + opciones.Email + "','" + opciones.Tlf + "') !";

                    if (f.EjecutarQuerySQLCli(query1, BD_Cli))
                    {

                        f.EjecutarConsultaSQLCli("select * from personas where rut='" + opciones.Rut + "'", BD_Cli);
                        int idPer = int.Parse(f.Tabla.Rows[0]["id"].ToString());

                        string query2 = "insert into trabajadores ([idpersona],[idregion],[idcomuna],[direccion], " +
                                        " [nacimiento],[sexo],[nrohijos], " +
                                        " [idpais],[habilitado]) " +
                                        "values " +
                                        "( " + idPer + "," + opciones.IdRegion + "," + opciones.IdComuna + ",'" + opciones.direccion + "'," +
                                        "  '" + opciones.nacimiento + "','" + opciones.sexo+ "', " + opciones.nrohijos + 
                                        ", " +opciones.IdPais+" ,1) ! ";

                        if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                        {
                            resultado.result = 1;
                            resultado.mensaje = "Trabajador ingresado de manera exitosa";
                        }
                        else
                        {
                            resultado.result = 0;
                            resultado.mensaje = "No se ingreso la información del trabajador";
                        }
                    }
                    else
                    {
                        resultado.result = 0;
                        resultado.mensaje = "Se generó un problema al guardar la informacion de la Persona";
                    }

                }

                return Json(new { info = resultado });
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Guardar trabajador";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar un trabajador en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar trabajador" + eG.Message.ToString();
                return Json(new { info = resultado });
            }

        }


        [HttpPost]
        [Route("eTrabajador")]
        public JsonResult eTrabajador(PersonasBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string empresaX = HttpContext.Session.GetString("EmpresaLog");

            RutEmpresa = f.obtenerRUT(int.Parse(empresaX));
            var BD_Cli = "remuneracion_" + RutEmpresa;


            opciones.Region = opciones.Region?.ToString() ?? string.Empty;
            opciones.Apellidos = opciones.Apellidos?.ToString() ?? string.Empty;
            opciones.Tlf = opciones.Tlf?.ToString() ?? string.Empty;
            opciones.sexo = opciones.sexo?.ToString() ?? string.Empty;
            opciones.Email = opciones.Email?.ToString() ?? string.Empty;


            try
            {
                if (opciones.Rut == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar el RUT del trabajador";
                    return Json(new { info = resultado });
                }

                f.EjecutarConsultaSQLCli("select * from personas where id='" + opciones.Id + "'", BD_Cli);
                if (f.Tabla.Rows.Count > 0)
                {
                    int idPer = int.Parse(f.Tabla.Rows[0]["id"].ToString());

                    string query = "update trabajadores set [idregion]=" + opciones.IdRegion + ",[idcomuna]=" + opciones.IdComuna + ",[direccion]='" + opciones.direccion + "', " +
                                       " [nacimiento]='" + opciones.nacimiento + "',[sexo]='" + opciones.sexo + "',[nrohijos]= " + opciones.nrohijos +
                                       ", [idpais]= " + opciones.IdPais +
                                        " where trabajadores.idpersona=" + idPer + "  ! ";

                    query += "update personas set rut='" + opciones.Rut + "', nombres='" + opciones.Nombres + "', email='" + opciones.Email + "', " +
                        "tlf='" + opciones.Tlf + "' where id=" + idPer + " ! ";

                    if (f.EjecutarQuerySQLCli(query, BD_Cli))
                    {
                        resultado.result = 1;
                        resultado.mensaje = "Trabajador editado exitosamente.";
                    }
                    else
                    {
                        resultado.result = 0;
                        resultado.mensaje = "No se editó la información del ctrabajador.";
                    }

                }
                else
                {

                    resultado.result = 0;
                    resultado.mensaje = "Se generó un problema al intentar editar la informacion del trabajador";

                }

                return Json(new { info = resultado });
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar trabajador";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar un trabajador en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar trabajador" + eG.Message.ToString();
                return Json(new { info = resultado });
            }

        }


        [HttpGet]
        [Route("dTrabajador")]
        public JsonResult dTrabajador(PersonasDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string empresaX = HttpContext.Session.GetString("EmpresaLog");

            RutEmpresa = f.obtenerRUT(int.Parse(empresaX));
            var BD_Cli = "remuneracion_" + RutEmpresa;

            try
            {
                if (opciones.Id == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la informacion del trabajador que desea eliminar";
                    return Json(new { info = resultado });
                }

                string query = "update trabajadores set habilitado=0 where idpersona=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Trabajador eliminado de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó la información del trabajador.";
                }

                return Json(new { info = resultado });
            }
            catch (Exception eG)
            {
                var Asunto = "Error al eliminar trabajador";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar un trabajador en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar un trabajador" + eG.Message.ToString();
                return Json(new { info = resultado });
            }

        }
    }
}
