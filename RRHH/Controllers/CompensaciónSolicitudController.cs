using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.CompensacionSolicitud;

namespace RRHH.Controllers
{
    public class CompensacionSolicitudController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        private readonly ICompensacionSolicitudService _CompensacionSolicitudService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Vacaciones");
        }

        public CompensacionSolicitudController(ICompensacionSolicitudService CompensacionSolicitudService)
        {
            _CompensacionSolicitudService = CompensacionSolicitudService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ConsultarCompensacionSolicitud")]
        public async Task<JsonResult> ConsultarCompensacionSolicitud(string rut, int dias)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = _CompensacionSolicitudService.MostrarCompensacionSolicitudService(empresa, dias, rut);
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("ProcesarCompensacionSolicitud")]
        public async Task<JsonResult> ProcesarCompensacionSolicitud(string rut, int dias)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = _CompensacionSolicitudService.ProcesarCompensacionSolicitudService(empresa, dias, rut);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("VerificaCompensacionSolicitud")]
        public async Task<JsonResult> VerificaCompensacionSolicitud(string rut, int dias)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = _CompensacionSolicitudService.VerificaCompensacionSolicitudService(empresa, dias, rut);
            return Json(new { info = resultado });
        }
    }
}
