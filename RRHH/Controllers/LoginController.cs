using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using RRHH.Servicios.Sessions;


namespace RRHH.Controllers
{
    public class LoginController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }


        private Correos correos = new Correos();
        private Funciones f = new Funciones();
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;

        public LoginController(IConfiguration config, IAuthService authService)
        {
            _config = config;
            _authService = authService;
        }

        [HttpPost]
        public JsonResult Logins(LoginUsuario userLogin)
        {
            resultLoginVM inicioSesion = _authService.Login(userLogin.Usuario, userLogin.Password);
            Resultado resultado = new Resultado();
            // Si el inicio de sesión no fue posible, retornar fallo
            if (inicioSesion == null)
            {
                resultado.result = 0;
                resultado.mensaje = "El inicio de sesión no fue posible";
                return Json(new { info = resultado });
            }

            // Enviar datos de sesión a JS
            resultado.result = 1;
            resultado.mensaje = "Existen datos para este usuario";
            resultado.data = inicioSesion;
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("SeleccionarEmpresa/{id:int}")]
        public async Task<JsonResult> SeleccionarEmpresa(int id)
        {
            EmpresasVM empresas = new EmpresasVM();
            Resultado resultado = new Resultado();
            resultado.result = 0;

            var idEncar = 0;
            var idRepre = 0;

            try
            {

                f.EjecutarConsultaSQL("select empresas.[id],empresas.[rut],empresas.[razonsocial],empresas.[fantasia],empresas.[giro],empresas.[idpais] " +
                    " , empresas.[idregion], empresas.[idcomuna], empresas.[idrepresentante], empresas.[idencargado], empresas.[email], empresas.[obs] " +
                    " , empresas.[habilitado], empresas.[conexion], empresas.[usuario], empresas.[contra], empresas.[basedatos] " +
                    " , paises.nombre, regiones.nombre " +
                    " from empresas " +
                    " inner join paises on empresas.idpais = paises.id " +
                    " inner " +
                    " join regiones on empresas.idregion = regiones.id where empresas.id=" + id + " and empresas.habilitado=1");

                List<EmpresasVM> opcionesList = new List<EmpresasVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    resultado.mensaje = "Existen datos en la coleccion";
                    resultado.result = 1;

                    idEncar = int.Parse(f.Tabla.Rows[0]["idencargado"].ToString());
                    idRepre = int.Parse(f.Tabla.Rows[0]["idrepresentante"].ToString());

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new EmpresasVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        rut = dr["rut"].ToString(),
                                        razonsocial = dr["razonsocial"].ToString(),
                                        fantasia = dr["fantasia"].ToString(),
                                        giro = dr["giro"].ToString(),
                                        idPais = int.Parse(dr["idpais"].ToString()),
                                        idregion = int.Parse(dr["idregion"].ToString()),
                                        idcomuna = int.Parse(dr["idcomuna"].ToString()),

                                        idencargado = int.Parse(dr["idencargado"].ToString()),
                                        idrepresentante = int.Parse(dr["idrepresentante"].ToString()),

                                        email = dr["email"].ToString(),
                                        obs = dr["obs"].ToString()
                                    }).ToList();

                    resultado.data = opcionesList;

                    HttpContext.Session.SetString("EmpresaLog", f.Tabla.Rows[0]["id"].ToString());
                    HttpContext.Session.SetString("NombreEmpresaLog", f.Tabla.Rows[0]["razonsocial"].ToString());
                    HttpContext.Session.SetString("stringBDcli", "contable_" + f.Tabla.Rows[0]["rut"].ToString());


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




        private string Generate(UsuarioVM user)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Crear los claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.usuraio),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.GivenName, user.nombre),
                new Claim(ClaimTypes.Surname, user.apellido),
                new Claim(ClaimTypes.Role, user.perfil),
            };

            //Crear el token
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(600),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UsuarioVM usuarioLogin(LoginUsuario userLogin)
        {
            UsuarioVM usu = new UsuarioVM();

            try
            {
                f.EjecutarConsultaSQL("select * from usuarios where nombreUsu='" + userLogin.Usuario + "' and contra='" + userLogin.Password + "' ");

                List<UsuarioVM> opcionesList = new List<UsuarioVM>();
                if (f.Tabla.Rows.Count > 0)
                {
                    usu.id = int.Parse(f.Tabla.Rows[0]["id"].ToString());
                    usu.usuraio = f.Tabla.Rows[0]["nombreusu"].ToString();
                    usu.nombre = f.Tabla.Rows[0]["nombres"].ToString();
                    usu.apellido = f.Tabla.Rows[0]["apellidos"].ToString();
                    usu.perfil = f.Tabla.Rows[0]["idperfil"].ToString();
                    usu.email = f.Tabla.Rows[0]["email"].ToString();
                    usu.telef = f.Tabla.Rows[0]["whatsapp"].ToString();

                    return usu;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult RecuperaClave()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RecuperaClave(string Usuario, string btn_cancelar)
        {
            try
            {
                if(btn_cancelar== "Cancelar")
                {
                    ViewBag.Cancel = true;
                    return Redirect("~/login");
                }
                UsuarioVM usu = new UsuarioVM();
            f.EjecutarConsultaSQL("select * from usuarios where nombreUsu='" + Usuario + "' ");

            List<UsuarioVM> opcionesList = new List<UsuarioVM>();
            if (f.Tabla.Rows.Count > 0)
            {
                usu.id = int.Parse(f.Tabla.Rows[0]["id"].ToString());
                usu.usuraio = f.Tabla.Rows[0]["nombreusu"].ToString();
                usu.nombre = f.Tabla.Rows[0]["nombres"].ToString();
                usu.apellido = f.Tabla.Rows[0]["apellidos"].ToString();
                usu.perfil = f.Tabla.Rows[0]["idperfil"].ToString();
                usu.email = f.Tabla.Rows[0]["email"].ToString();
                usu.telef = f.Tabla.Rows[0]["whatsapp"].ToString();

                Random rnd = new Random();
                string Pass = rnd.Next(100000, 999999).ToString();
                string clave = Pass;
                string Passenc = GetMD5Hash(Pass);
                string CONTRASENA = Pass;
                var Asunto = "Cambio de clave";
                var Mensaje ="Debe usar el siguiente codigo de verificación, para cambiar clave : "+Pass ;
                HttpContext.Session.SetString("UsuarioRec", Usuario);
                HttpContext.Session.SetString("CodigoRec", Pass);

                correos.SendEmail(Mensaje, Asunto, "Cambio de clave", usu.email);
                ViewBag.Redireccionar = true;
                return Redirect("~/login/NuevaClave");
            }
            else
            {
                    ViewBag.ErrorInfo = true;
            }
            }
            catch (Exception)
            {
                ViewBag.Error = "Some Error";
            }
            return View();
        }
        public ActionResult NuevaClave()
        {
            ViewBag.Inicio = true;
            ViewBag.Exito = false;
            ViewBag.ErrorCodigo = false;
            return View();
        }
        [HttpPost]
        public ActionResult NuevaClave(string codigo, string clave, string replica)
        {
            string UsuarioRec = HttpContext.Session.GetString("UsuarioRec");
            string CodigoRec = HttpContext.Session.GetString("CodigoRec");
            ViewBag.Inicio = false;
            ViewBag.Exito = false;
            ViewBag.ErrorCodigo = false;
            ViewBag.ErrorIgual = false;
            ViewBag.ErrorClave = false;
            try
            {
                if(codigo== CodigoRec && clave == replica && clave.Length > 5)
                {
                    f.EjecutarConsultaSQL("Update usuarios set [contra] = '" + clave + "' where nombreUsu='" + UsuarioRec + "' ");
                    ViewBag.Exito = true;
                    return Redirect("/Login");
                }
                else
                {
                    if (codigo != CodigoRec)
                        ViewBag.ErrorCodigo = true;
                    if (clave != replica)
                        ViewBag.ErrorIgual = true;
                    if (clave.Length < 6)
                        ViewBag.ErrorClave = true;
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Some Error";
            }
            return View();
        }
        public ActionResult CerrarSesion()
        {
            bool resultadoLogout = _authService.Logout();
            if (!resultadoLogout)
                // TODO: crear y redireccionar a página de error
                return new RedirectResult("/Login", false);

            return new RedirectResult("/Login", false);
        }
        public string GetMD5Hash(string input)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            String hash = s.ToString();
            return hash;
        }

    }
}
