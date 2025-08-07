using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.SolicitudVacaciones;

namespace RRHH.Controllers
{
    public class VacacionesSolicitudController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        private readonly ISolicitudVacacionesService _solicitudvacacionesService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Ajuste de marcación");
        }

        public VacacionesSolicitudController(ISolicitudVacacionesService solicitudvacacionesService)
        {
            _solicitudvacacionesService = solicitudvacacionesService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ConsultarSolicitudVacaciones")]
        public async Task<JsonResult> ConsultarSolicitudVacaciones(string rut, string des, string has)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = _solicitudvacacionesService.MostrarSolicitudVacacionesService(empresa, des, has, rut);
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("ProcesarSolicitudVacaciones")]
        public async Task<JsonResult> ProcesarSolicitudVacaciones(string rut, string des, string has)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = _solicitudvacacionesService.ProcesarSolicitudVacacionesService(empresa, des, has, rut);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("VerificaSolicitudVacaciones")]
        public async Task<JsonResult> VerificaSolicitudVacaciones(string rut, string des, string has)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = _solicitudvacacionesService.VerificaSolicitudVacacionesService(empresa, des, has, rut);
            return Json(new { info = resultado });
        }
    }
}
