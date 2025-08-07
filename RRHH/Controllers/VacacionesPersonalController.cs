using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.VacacionesPersonal;

namespace RRHH.Controllers
{
    public class VacacionesPersonalController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IVacacionesPersonalService _VacacionesPersonalService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Vacaciones");
        }

        public VacacionesPersonalController(IVacacionesPersonalService VacacionesPersonalService)
        {
            _VacacionesPersonalService = VacacionesPersonalService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarVacacionesPersonal")]
        public async Task<JsonResult> ListarVacacionesPersonal(String desde, string hasta)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            List<DetVacacionesPersonal> VacacionesPersonal = _VacacionesPersonalService.ListarVacacionesPersonalService(empresa, desde, hasta);
            if (VacacionesPersonal != null)
            {
                resultado.data = VacacionesPersonal;
                resultado.mensaje = "Si existen registros";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

    }
}
