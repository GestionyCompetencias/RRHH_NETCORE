using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.VacacionesAutorizaJefe;

namespace RRHH.Controllers
{
    public class VacacionesAutorizaJefeController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IVacacionesAutorizaJefeService _VacacionesAutorizaJefeService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Vacaciones");
        }

        public VacacionesAutorizaJefeController(IVacacionesAutorizaJefeService VacacionesAutorizaJefeService)
        {
            _VacacionesAutorizaJefeService = VacacionesAutorizaJefeService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarVacacionesAutorizaJefe")]
        public async Task<JsonResult> ListarVacacionesAutorizaJefe(String desde, string hasta)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = _VacacionesAutorizaJefeService.ListarVacacionesAutorizaJefeService(empresa, desde, hasta);
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("AutorizaVacaciones")]
        public JsonResult AutorizaVacaciones(string ids, string des, string has)
        {
                string empresastr = HttpContext.Session.GetString("EmpresaLog");
                string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
                int empresa = int.Parse(empresastr);
                Resultado resultado = _VacacionesAutorizaJefeService.AutorizaVacacionesService(ids, empresa, UsuarioLogeado, des, has);
                return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("RechazaVacaciones")]
        public JsonResult RechazaVacaciones(string ids, string des, string has)
        {
                string empresastr = HttpContext.Session.GetString("EmpresaLog");
                string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
                int empresa = int.Parse(empresastr);
                Resultado resultado = _VacacionesAutorizaJefeService.RechazaVacacionesService(ids, empresa, UsuarioLogeado, des, has);
                return Json(new { info = resultado });
        }
    }
}
