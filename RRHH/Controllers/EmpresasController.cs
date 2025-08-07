using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using RRHH.Repositorios;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Transactions;
using Microsoft.AspNetCore.ResponseCompression;

namespace RRHH.Controllers
{

    public class EmpresasController : Controller
    {

        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();

        private readonly IEmpresaRepository _empresaRepository;
        
        public EmpresasController(IEmpresaRepository empresaRepository)
        {
            _empresaRepository = empresaRepository;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("consultarempresas")]
        public async Task<JsonResult> ConsultarEmpresa()
        {
            try
            {
                List<EmpresasVM> empresasHabilitadas = _empresaRepository.GetAllEmpresas(true);
                Resultado response = new Resultado();
                if (empresasHabilitadas.Count > 0) 
                {
                    response.result = 0;
                    response.mensaje = "Existen datos en la colección";
                    response.data = empresasHabilitadas;
                }
                else
                {
                    response.result = 0;
                    response.mensaje = "No se han creado empresas";
                }

                return Json(new { info = response });
            }
            catch (Exception ex)
            {
                Resultado response = new Resultado();

                response.result = -1;
                response.mensaje = "Fallo";
                // TODO: Agregar notificación a rollbar en caso de excepción
                string asunto = "Error al Consultar Empresas";
                string mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
               
                return Json(new { info = response });
            }
        }


        [HttpGet]
        [Route("ConsultarMisEmpresas")]
        public async Task<JsonResult> ConsultarMisEmpresas(int idUser)
        {
            try
            {
                Resultado response = new Resultado();
                // Verifica usuario logeado   
                string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
                int IdusuarioLog = Convert.ToInt32(UsuarioLogeado);
                if (IdusuarioLog != idUser)
                { 
                    response.result = 0;
                    response.mensaje = "No existe usuario";
                }
                else
                {
                    List<EmpresasVM> empresasUsuario = _empresaRepository.GetEmpresasByUsuario(idUser);
                    if(empresasUsuario.Count > 0)
                    {
                        response.result = 1;
                        response.mensaje = "Existen datos en la colección";
                        response.data = empresasUsuario;

                    }
                    else
                    {
                        response.result = 0;
                        response.mensaje = "No se han creado empresas";

                    }
                }
                return Json(new { info = response });
            }
            catch (Exception ex)
            {
                Resultado response = new Resultado();
                response.result = -1;
                response.mensaje = "Fallo";
                // TODO: Agregar notificación a rollbar en caso de excepción
                string asunto = "Error al Consultar Empresas";
                string mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
               
                return Json(new { info = response});    
            }
        }



        [HttpGet]
        [Route("ConsultaEmpresa")]
        public async Task<JsonResult> ConsultaEmpresa(int id)
        {

            EmpresasConsultaVM empresas = new EmpresasConsultaVM();
            Resultado resultado = new Resultado();
            resultado.result = 0;

            var idEncar = 0;
            var idRepre = 0;

            try
            {

                f.EjecutarConsultaSQL("select empresas.[id],empresas.[rut],empresas.[razonsocial],empresas.[fantasia],empresas.[giro],empresas.[direccion],empresas.[idpais] " +
                    " , empresas.[idregion], empresas.[idcomuna], empresas.[idrepresentante], empresas.[idencargado], empresas.[email], empresas.[obs] " +
                    " , empresas.[habilitado], empresas.[conexion], empresas.[usuario], empresas.[contra], empresas.[basedatos] " +
                    " , paises.nombre, regiones.nombre " +
                    " from empresas " +
                    " inner join paises on empresas.idpais = paises.id " +
                    " inner " +
                    " join regiones on empresas.idregion = regiones.id where empresas.id=" + id + " and empresas.habilitado=1");

                List<EmpresasConsultaVM> opcionesList = new List<EmpresasConsultaVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen datos en la coleccion";
                    resultado.result = 1;

                    idEncar = int.Parse(f.Tabla.Rows[0]["idencargado"].ToString());
                    idRepre = int.Parse(f.Tabla.Rows[0]["idrepresentante"].ToString());

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new EmpresasConsultaVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),

                                        rut = dr["rut"].ToString(),
                                        razonsocial = dr["razonsocial"].ToString(),
                                        fantasia = dr["fantasia"].ToString(),
                                        giro = dr["giro"].ToString(),

                                        direccion = dr["giro"].ToString(),

                                        idPais = int.Parse(dr["idpais"].ToString()),
                                        idregion = int.Parse(dr["idregion"].ToString()),
                                        idcomuna = int.Parse(dr["idcomuna"].ToString()),

                                        idencargado = int.Parse(dr["idencargado"].ToString()),
                                        idrepresentante = int.Parse(dr["idrepresentante"].ToString()),

                                        email = dr["email"].ToString(),
                                        obs = dr["obs"].ToString()
                                    }).ToList();

                    resultado.data = opcionesList;

                }
                else
                {
                    resultado.mensaje = "No se han creado empresas";
                    resultado.result = 0;
                }
                return Json(new { info = resultado });
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar Empresas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar una empresa", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return Json(new { info = empresas });
            }

        }

        [HttpGet]
        [Route("ConsultaEmpresaRut")]
        public async Task<JsonResult> ConsultaEmpresaRut(string empresa)
        {

            EmpresasConsultaVM empresas = new EmpresasConsultaVM();
            Resultado resultado = new Resultado();
            resultado.result = 0;

            var idEncar = 0;
            var idRepre = 0;

            try
            {

                f.EjecutarConsultaSQL("select empresas.[id],empresas.[rut],empresas.[razonsocial],empresas.[fantasia],empresas.[giro],empresas.[direccion],empresas.[idpais] " +
                    " , empresas.[idregion], empresas.[idcomuna], empresas.[idrepresentante], empresas.[idencargado], empresas.[email], empresas.[obs] " +
                    " , empresas.[habilitado], empresas.[conexion], empresas.[usuario], empresas.[contra], empresas.[basedatos] " +
                    " , paises.nombre, regiones.nombre " +
                    " from empresas " +
                    " inner join paises on empresas.idpais = paises.id " +
                    " inner " +
                    " join regiones on empresas.idregion = regiones.id where empresas.rut=" + empresa + " and empresas.habilitado=1");

                List<EmpresasConsultaVM> opcionesList = new List<EmpresasConsultaVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen datos en la coleccion";
                    resultado.result = 1;

                    idEncar = int.Parse(f.Tabla.Rows[0]["idencargado"].ToString());
                    idRepre = int.Parse(f.Tabla.Rows[0]["idrepresentante"].ToString());

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new EmpresasConsultaVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),

                                        rut = dr["rut"].ToString(),
                                        razonsocial = dr["razonsocial"].ToString(),
                                        fantasia = dr["fantasia"].ToString(),
                                        giro = dr["giro"].ToString(),

                                        direccion = dr["giro"].ToString(),

                                        idPais = int.Parse(dr["idpais"].ToString()),
                                        idregion = int.Parse(dr["idregion"].ToString()),
                                        idcomuna = int.Parse(dr["idcomuna"].ToString()),

                                        idencargado = int.Parse(dr["idencargado"].ToString()),
                                        idrepresentante = int.Parse(dr["idrepresentante"].ToString()),

                                        email = dr["email"].ToString(),
                                        obs = dr["obs"].ToString()
                                    }).ToList();

                    resultado.data = opcionesList;

                }
                else
                {
                    resultado.mensaje = "No se han creado empresas";
                    resultado.result = 0;
                }
                return Json(new { info = resultado });
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar Empresas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar una empresa", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return Json(new { info = empresas });
            }

        }

        [HttpGet]
        [Route("ConsultarEmpresasUsuario")]
        public async Task<JsonResult> ConsultarEmpresasUsuario(int idUsuario)
        {
            try
            {
                Resultado response = new Resultado();
                int IdusuarioLog = Convert.ToInt32(HttpContext.Session.GetString("UserIdLog"));
                if (IdusuarioLog != idUsuario)
                {
                    response.result = 0;
                    response.mensaje = "Usuario erroneo";
                }
                else
                {
                    List<EmpresasVM> empresasHabilitadasUsuario = _empresaRepository.GetEmpresasHabilitadasByUsuario(idUsuario);
                    if (empresasHabilitadasUsuario.Count > 0)
                    {
                        response.result = 1;
                        response.mensaje = "Existen datos en la colección";
                        response.data = empresasHabilitadasUsuario;
                    }
                    else
                    {
                        response.result = 0;
                        response.mensaje = "No se han creado empresas";

                    }
                }
                return Json(new { info = response });
            }
            catch (Exception ex)
            {
                Resultado response = new Resultado();
                response.result = -1;
                response.mensaje = "Fallo";
                // TODO: Agregar notificación a rollbar en caso de excepción
                string asunto = "Error al Consultar Empresas";
                string mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();

                return Json(new { info = response });
            }
        }


        [HttpPost]
        [Route("CrearEmpresa")]
        public JsonResult CrearEmpresa(EmpresasDtoVM oEmpresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            try
            {
                string nuevaBD = "remuneracion_" + oEmpresa.rut;
                string query = "";
                int idRepre = 0;
                int idEncar = 0;

                if (oEmpresa.id == 0)
                {

                    List<EmpresasVM> empresasRut = new List<EmpresasVM>();
                    empresasRut = _empresaRepository.GetEmpresasByRut(oEmpresa.rut);
                    if (empresasRut.Count > 0)
                    {
                        resultado.result = 0;
                        resultado.mensaje = "Empresa ya esta creada, debe actualizar.";
                    }
                    else
                    {

                        query = "insert into empresas (rut,razonsocial,fantasia,giro, " +
                                "idpais,idregion,idcomuna," +
                                "email,obs,padre," +
                                "basedatos,habilitado,direccion) values " +
                                "('" + oEmpresa.rut + "','" + oEmpresa.razonsocial + "','" + oEmpresa.fantasia + "','" + oEmpresa.giro + "'," +
                                "'" + oEmpresa.idPais + "','" + oEmpresa.idregion + "','" + oEmpresa.idcomuna + "'," +
                                "'" + oEmpresa.email + "','" + oEmpresa.obs + "','" + oEmpresa.padre + "'," +
                                "'" + nuevaBD + "',1,'" + oEmpresa.direccion + "') !";


                        if (f.EjecutarQuerySQL(query))
                        {

                            if (f.crearNuevaBD(oEmpresa.rut))
                            {
                                Tareas tareas = new Tareas();
                                tareas.llenarTablasNuevaBD(nuevaBD, query);

                                //  CREAMOS EL NUEVO USUARIO DEL SISTEMA
                                string queryNuevoUsuario = "insert into usuarios (nombreUsu, idPerfil, contra, email, nombres, apellidos, whatsapp) " +
                                                "values " +
                                                "('" + oEmpresa.email + "','2','123456','" + oEmpresa.email + "'," +
                                                 "'Administrador','Administrador','') !";

                                f.EjecutarQuerySQL(queryNuevoUsuario);

                                //CONSULTAMOS EL ID DE LA EMPRESA QUE ACABAMOS DE CREAR
                                f.EjecutarConsultaSQL("select id from empresas where rut=" + oEmpresa.rut + "");
                                int idEmpresaNueva = int.Parse(f.Tabla.Rows[0]["id"].ToString());

                                //CONSULTAMOS EL ID DEL USUARIO QUE ACABAMOS DE CREAR
                                string queryC = string.Format("select id from usuarios where habilitado=1 and email='{0}'", oEmpresa.email);
                                f.EjecutarConsultaSQL(queryC);
                                int idUsuarioNuevo = int.Parse(f.Tabla.Rows[0]["id"].ToString());

                                //REGISTRAMOS EL USUARIO EN LA TABLA USUARIOEMPRESA
                                f.EjecutarQuerySQL("insert into usuarioempresa (idusuario,idempresa) values (" + idUsuarioNuevo + "," + idEmpresaNueva + ") !");

                                //REGISTRAMOS LOS DATOS DEL REPRESENTANTE LEGAL Y DEL ENCARGADO
                                if (oEmpresa.rutencargado != oEmpresa.rutrepresentante)
                                {
                                    if (oEmpresa.rutencargado!= null)
                                    {
                                        f.EjecutarConsultaSQLCli("select id from personas where rut='" + oEmpresa.rutencargado + "'", nuevaBD);
                                        idEncar = int.Parse(f.Tabla.Rows[0]["id"].ToString());
                                        if (idEncar == 0)
                                        {
                                            query = "insert into personas (rut,nombres,apellidos,email) values" +
                                                "('" + oEmpresa.rutencargado + "','" + oEmpresa.nombresencargado + "','" + oEmpresa.apellidosencargado + "','" + oEmpresa.emailencargado + "'); !";
                                            f.EjecutarQuerySQLCli(query, nuevaBD);
                                            f.EjecutarConsultaSQLCli("select id from personas where rut='" + oEmpresa.rutencargado + "'", nuevaBD);
                                            idEncar = int.Parse(f.Tabla.Rows[0]["id"].ToString());

                                        }
                                    }
                                    if (oEmpresa.rutrepresentante!= null)
                                    {
                                        f.EjecutarConsultaSQLCli("select id from personas where rut='" + oEmpresa.rutrepresentante + "'", nuevaBD);
                                        idRepre = int.Parse(f.Tabla.Rows[0]["id"].ToString());
                                        if(idRepre == 0)
                                        {
                                            query = "insert into personas (rut,nombres,apellidos,email) values" +
                                                "('" + oEmpresa.rutrepresentante + "','" + oEmpresa.nombresrepresentante + "','" + oEmpresa.apellidosrepresentante + "','" + oEmpresa.emailrepresentante + "'); !";
                                            f.EjecutarQuerySQLCli(query, nuevaBD);
                                            f.EjecutarConsultaSQLCli("select id from personas where rut='" + oEmpresa.rutrepresentante + "'", nuevaBD);
                                            idRepre = int.Parse(f.Tabla.Rows[0]["id"].ToString());
                                        }
                                    }


                                    //****************************************************************************************************************************


                                }
                                else
                                {
                                    if (oEmpresa.rutencargado != null)
                                    {
                                        f.EjecutarConsultaSQLCli("select id from personas where rut='" + oEmpresa.rutencargado + "'", nuevaBD);
                                        idEncar = int.Parse(f.Tabla.Rows[0]["id"].ToString());
                                        idRepre = idEncar;
                                        if (idEncar == 0)
                                        {
                                            query = "insert into personas (rut,nombres,apellidos,email) values" +
                                        "('" + oEmpresa.rutencargado + "','" + oEmpresa.nombresencargado + "','" + oEmpresa.apellidosencargado + "','" + oEmpresa.emailencargado + "'); !";

                                            f.EjecutarQuerySQLCli(query, nuevaBD);
                                            f.EjecutarConsultaSQLCli("select id from personas where rut='" + oEmpresa.rutencargado + "'", nuevaBD);
                                            idEncar = int.Parse(f.Tabla.Rows[0]["id"].ToString());
                                            idRepre = int.Parse(f.Tabla.Rows[0]["id"].ToString());
                                        }
                                    }
                                }

                                if (oEmpresa.rutrepresentante != null)
                                {
                                    f.EjecutarConsultaSQLCli("select id from personas where rut='" + oEmpresa.rutrepresentante + "'", nuevaBD);
                                    idRepre = int.Parse(f.Tabla.Rows[0]["id"].ToString());
                                    if (idRepre == 0)
                                    {
                                        query = "insert into personas (rut,nombres,apellidos,email) values" +
                                            "('" + oEmpresa.rutrepresentante + "','" + oEmpresa.nombresrepresentante + "','" + oEmpresa.apellidosrepresentante + "','" + oEmpresa.emailrepresentante + "'); !";
                                        f.EjecutarQuerySQLCli(query, nuevaBD);
                                        f.EjecutarConsultaSQLCli("select id from personas where rut='" + oEmpresa.rutrepresentante + "'", nuevaBD);
                                        idRepre = int.Parse(f.Tabla.Rows[0]["id"].ToString());
                                    }
                                }
                                if (oEmpresa.rutencargado != null)
                                {
                                    f.EjecutarConsultaSQLCli("select id from personas where rut='" + oEmpresa.rutencargado + "'", nuevaBD);
                                    idEncar = int.Parse(f.Tabla.Rows[0]["id"].ToString());
                                    if (idEncar == 0)
                                    {
                                        query = "insert into personas (rut,nombres,apellidos,email) values" +
                                            "('" + oEmpresa.rutencargado + "','" + oEmpresa.nombresencargado + "','" + oEmpresa.apellidosencargado + "','" + oEmpresa.emailencargado + "'); !";
                                        f.EjecutarQuerySQLCli(query, nuevaBD);
                                        f.EjecutarConsultaSQLCli("select id from personas where rut='" + oEmpresa.rutencargado + "'", nuevaBD);
                                        idEncar = int.Parse(f.Tabla.Rows[0]["id"].ToString());

                                    }
                                }


                                //f.EjecutarQuerySQLCli("update empresas set idrepresentante=" + idRepre + ",idencargado=" + idEncar + " where rut='" + oEmpresa.rut + "' !", nuevaBD);
                                //f.EjecutarQuerySQL("update empresas set idrepresentante=" + idRepre + ",idencargado=" + idEncar + " where rut='" + oEmpresa.rut + "' !");

                                resultado.result = 1;
                                resultado.mensaje = "Proceso finalizado exitosamente";
                            }
                            else
                            {
                                resultado.result = 1;
                                resultado.mensaje = "Proceso finalizado con problemas - Se creo la empresa pero ocurrió u problema al crear la BD";
                            }

                            resultado.result = 1;
                            resultado.mensaje = "Proceso finalizado exitosamente";
                        }
                        else
                        {
                            resultado.result = 0;
                            resultado.mensaje = "No se pudo ingresar el registro";
                        }
                    }
                }
                else
                {

                    string queryUpdate = "update empresas set razonsocial='" + oEmpresa.razonsocial + "',fantasia='" + oEmpresa.fantasia + "',giro='" + oEmpresa.giro + "', " +
                            "idpais='" + oEmpresa.idPais + "',idregion='" + oEmpresa.idregion + "',idcomuna='" + oEmpresa.idcomuna + "', " +
                            "email='" + oEmpresa.email + "',obs='" + oEmpresa.obs + "',direccion='" + oEmpresa.direccion + "' " +
                            "where id='" + oEmpresa.id + "' ";

                    f.EjecutarQuerySQL(queryUpdate);


                    f.EjecutarQuerySQLCli("update personas set nombres='" + oEmpresa.nombresrepresentante + "',apellidos='" + oEmpresa.apellidosrepresentante + "' where rut='" + oEmpresa.rutrepresentante + "' !", nuevaBD);
                    f.EjecutarQuerySQLCli("update personas set nombres='" + oEmpresa.nombresencargado + "',apellidos='" + oEmpresa.apellidosencargado + "' where rut='" + oEmpresa.rutencargado + "' !", nuevaBD);

                }

                //return Json(new { info = resultado });
                return Json( resultado );
            }
            catch (Exception eG)
            {

                var Asunto = "Error al Consultar Personas";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar una persona", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar la información" + eG.Message.ToString();
                return Json(new { info = resultado });

            }

        }




        [HttpPost]
        [Route("eEmpresa")]
        public JsonResult eEmpresa(BancosVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            var BD = f.CadenaConexionCliente("Contable");

            try
            {
                if (opciones.Nombre == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar el Nombre del Banco";
                    return Json(new { info = resultado });
                }



                using (TransactionScope transaction = new TransactionScope())
                {

                    string connectString1 = f.CadenaConexionCliente(BD);
                    using (SqlConnection conexion = new SqlConnection(connectString1))
                    {
                        conexion.Open();

                        string query = "update empresas set [rut]=@param_rut,[razonsocial]=@param_razon,[fantasia]=@param_fanta,[giro]=@param_giro," +
                            "[idpais]=@param_pais,[idregion]=@param_region,[idcomuna]=@param_comuna,[idrepresentante]=@param_repres," +
                            "[idencargado]=@param_encarg,[email]=@param_email,[obs]=@param_obs,[direccion]=@param_direccion " +
                            " where id=@param_id ";

                        SqlCommand comando = new SqlCommand(query, conexion);

                        comando.Parameters.Add("@param_rut", SqlDbType.VarChar).Value = opciones.Nombre;
                        comando.Parameters.Add("@param_razon", SqlDbType.VarChar).Value = opciones.Nombre;
                        comando.Parameters.Add("@param_fanta", SqlDbType.VarChar).Value = opciones.Nombre;
                        comando.Parameters.Add("@param_giro", SqlDbType.VarChar).Value = opciones.Nombre;

                        comando.Parameters.Add("@param_pais", SqlDbType.VarChar).Value = opciones.Nombre;
                        comando.Parameters.Add("@param_region", SqlDbType.VarChar).Value = opciones.Nombre;
                        comando.Parameters.Add("@param_comuna", SqlDbType.VarChar).Value = opciones.Nombre;
                        comando.Parameters.Add("@param_repres", SqlDbType.VarChar).Value = opciones.Nombre;

                        comando.Parameters.Add("@param_encarg", SqlDbType.VarChar).Value = opciones.Nombre;
                        comando.Parameters.Add("@param_email", SqlDbType.VarChar).Value = opciones.Nombre;
                        comando.Parameters.Add("@param_obs", SqlDbType.VarChar).Value = opciones.Nombre;
                        comando.Parameters.Add("@param_direccion", SqlDbType.VarChar).Value = opciones.Nombre;

                        comando.Parameters.Add("@param_id", SqlDbType.Int).Value = opciones.Id;

                        comando.ExecuteNonQuery();

                    }
                    try
                    {

                        transaction.Complete();
                        resultado.result = 1;
                        resultado.mensaje = "Banco Editado de manera exitosa.";
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        resultado.result = 0;
                        resultado.mensaje = "No se editó la información del banco.";
                    }


                }

                return Json(new { info = resultado });
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar Banco";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar un banco en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar banco" + eG.Message.ToString();
                return Json(new { info = resultado });
            }

        }




        [HttpGet]
        [Route("dEmpresa")]
        public JsonResult dEmpresa(int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            var BD = f.CadenaConexionCliente("Contable");

            try
            {

                using (TransactionScope transaction = new TransactionScope())
                {

                    string connectString1 = f.CadenaConexionCliente(BD);
                    using (SqlConnection conexion = new SqlConnection(connectString1))
                    {
                        conexion.Open();

                        string query = "update empresas set [habilitado]=0 " +
                                        " where id=@param_id ";

                        SqlCommand comando = new SqlCommand(query, conexion);
                        comando.Parameters.Add("@param_id", SqlDbType.Int).Value = empresa;

                        comando.ExecuteNonQuery();

                    }
                    try
                    {

                        transaction.Complete();
                        resultado.result = 1;
                        resultado.mensaje = "Empresa eliminada de manera exitosa.";
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        resultado.result = 0;
                        resultado.mensaje = "No se pudo eliminar la información de la empresa.";
                    }


                }

                return Json(new { info = resultado });
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Eliminar Empresa";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar una emrpesa en el sistema - empresa: " + empresa, correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar empresa" + eG.Message.ToString();
                return Json(new { info = resultado });
            }

        }
    }
}
