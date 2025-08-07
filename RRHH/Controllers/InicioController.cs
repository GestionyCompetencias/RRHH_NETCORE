using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace RRHH.Controllers
{
	public class InicioController : Controller
	{

        private Seguridad seguridad = new Seguridad();

        public IActionResult Index()
		{
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            string EmpresaLogeada = HttpContext.Session.GetString("EmpresaLog");
            string stringBDcli = HttpContext.Session.GetString("stringBDcli");

            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");

            return View();
		}

        [HttpGet]
        [Route("SesionUsuario")]
        public string SesionUsuario()
		{
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
			return $"{UsuarioLogeado}";
        }

        [HttpGet]
        [Route("EmpresaLog")]
        public string EmpresaLog()
        {
            string empresaLog = HttpContext.Session.GetString("EmpresaLog");
            return $"{empresaLog}";
        }

    }
}
