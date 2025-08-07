using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.CambioClave;



namespace RRHH.Controllers
{
    public class CambioClaveController : Controller
    {
        private Seguridad seguridad = new Seguridad();
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Generales Generales = new Generales();
        private readonly ICambioClaveService _cambioclaveService;

        public CambioClaveController(ICambioClaveService cambioclaveService)
        {
            _cambioclaveService = cambioclaveService;
        }


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

        [HttpPost]
        [Route("CambioClave")]
        public async Task<JsonResult> CambioClave(string antigua, string nueva,string verifica)
        {
            UsuarioVM perfiles = new UsuarioVM();
            Resultado resultado = new Resultado();
            string usuario = HttpContext.Session.GetString("UserIdLog");
            resultado = _cambioclaveService.CambiarClaveService(antigua,nueva,verifica,usuario);
            return Json(new { info = perfiles });
        }
    }
}

