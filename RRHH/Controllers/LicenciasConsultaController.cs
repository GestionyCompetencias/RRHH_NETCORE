using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.LicenciasConsulta;

namespace RRHH.Controllers
{
    public class LicenciasConsultaController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly ILicenciasConsultaService _LicenciasConsultaService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Licencias medicas");
        }

        public LicenciasConsultaController(ILicenciasConsultaService LicenciasConsultaService)
        {
            _LicenciasConsultaService = LicenciasConsultaService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarLicenciasConsulta")]
        public async Task<JsonResult> ListarLicenciasConsulta(String desde, string hasta)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            List<DetConsultaLicencias> LicenciasConsulta = _LicenciasConsultaService.ListarLicenciasConsultaService(empresa, desde,hasta);
            if (LicenciasConsulta != null)
            {
                resultado.data = LicenciasConsulta;
                resultado.mensaje = "Si existen registros";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

    }
}
