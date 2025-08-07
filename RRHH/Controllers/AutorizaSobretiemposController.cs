using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.AutorizaSobretiempos;

namespace RRHH.Controllers
{
    public class AutorizaSobretiemposController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        private readonly IAutorizaSobretiemposService _autorizasobretiemposService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Autoriza horas extras");
        }

        public AutorizaSobretiemposController(IAutorizaSobretiemposService autorizasobretiemposService)
        {
            _autorizasobretiemposService = autorizasobretiemposService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarSobretiempos")]
        public async Task<JsonResult> ListarAutorizaSobretiempos(string des, string has)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen horas extras";
            resultado.result = 0;

            List<DetSobretiempos> sobretiempos = _autorizasobretiemposService.ListarSobretiemposService(empresa, des, has);
            if (sobretiempos != null)
            {
                resultado.data = sobretiempos;
                resultado.mensaje = "Si existen horas extras";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("AutorizaHorasExtras")]
        public JsonResult AutorizaHorasExtras(string ids, string des, string has)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = _autorizasobretiemposService.AutorizaSobretiempoService(ids, empresa, UsuarioLogeado,des,has);
            return Json(new { info = resultado });
        }
    }
}
