using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.VacacionesAutorizaPersonal;

namespace RRHH.Controllers
{
    public class VacacionesAutorizaPersonalController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IVacacionesAutorizaPersonalService _VacacionesAutorizaPersonalService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Vacaciones");
        }

        public VacacionesAutorizaPersonalController(IVacacionesAutorizaPersonalService VacacionesAutorizaPersonalService)
        {
            _VacacionesAutorizaPersonalService = VacacionesAutorizaPersonalService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarVacacionesAutorizaPersonal")]
        public async Task<JsonResult> ListarVacacionesAutorizaPersonal(String desde, string hasta)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = _VacacionesAutorizaPersonalService.ListarVacacionesAutorizaPersonalService(empresa, desde, hasta);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("AutorizaVacacionesPersonal")]
        public JsonResult AutorizaVacacionesPersonal(string ids, string des, string has)
        {
                string empresastr = HttpContext.Session.GetString("EmpresaLog");
                string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
                int empresa = int.Parse(empresastr);
                Resultado resultado = _VacacionesAutorizaPersonalService.AutorizaVacacionesService(ids, empresa, UsuarioLogeado, des, has);
                return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("RechazaVacacionesPersonal")]
        public JsonResult RechazaVacacionesPersonal(string ids, string des, string has)
        {
                string empresastr = HttpContext.Session.GetString("EmpresaLog");
                string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
                int empresa = int.Parse(empresastr);
                Resultado resultado = _VacacionesAutorizaPersonalService.RechazaVacacionesService(ids, empresa, UsuarioLogeado, des, has);
                return Json(new { info = resultado });
        }
    }
}
