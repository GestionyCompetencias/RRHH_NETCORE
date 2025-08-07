using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.VacacionesCtaCte;

namespace RRHH.Controllers
{
    public class VacacionesCtaCteController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IVacacionesCtaCteService _VacacionesCtaCteService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Vacaciones");
        }

        public VacacionesCtaCteController(IVacacionesCtaCteService VacacionesCtaCteService)
        {
            _VacacionesCtaCteService = VacacionesCtaCteService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarVacacionesCtaCte")]
        public async Task<JsonResult> ListarVacacionesCtaCte(String desde, string hasta)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado  = _VacacionesCtaCteService.ListarVacacionesCtaCteService(empresa, desde, hasta);
            return Json(new { info = resultado });
        }

    }
}
