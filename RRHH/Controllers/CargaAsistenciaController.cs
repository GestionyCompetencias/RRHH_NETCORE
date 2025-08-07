using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.CargaAsistencia;

namespace RRHH.Controllers
{
    public class CargaAsistenciaController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly ICargaAsistenciaService _cargaasistenciaService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Información mensual");
        }

        public CargaAsistenciaController(ICargaAsistenciaService cargaasistenciaService)
        {
            _cargaasistenciaService = cargaasistenciaService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("CargarAsistencias")]
        public async Task<JsonResult> CargarAsistencias(int mes, int anio)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado = _cargaasistenciaService.TraspasaAsistenciaService(empresa, mes, anio);
            return Json(new { info = resultado });
        }
    }
}
