using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.CompensacionAutorizaPersonal;

namespace RRHH.Controllers
{
    public class CompensacionAutorizaPersonalController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly ICompensacionAutorizaPersonalService _CompensacionAutorizaPersonalService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Compensacion");
        }

        public CompensacionAutorizaPersonalController(ICompensacionAutorizaPersonalService CompensacionAutorizaPersonalService)
        {
            _CompensacionAutorizaPersonalService = CompensacionAutorizaPersonalService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarCompensacionAutorizaPersonal")]
        public async Task<JsonResult> ListarCompensacionAutorizaPersonal(String desde, string hasta)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = _CompensacionAutorizaPersonalService.ListarCompensacionAutorizaPersonalService(empresa, desde, hasta);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("AutorizaCompensacion")]
        public JsonResult AutorizaCompensacion(string ids, string des, string has)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = _CompensacionAutorizaPersonalService.AutorizaCompensacionService(ids, empresa, UsuarioLogeado, des, has);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("RechazaCompensacion")]
        public JsonResult RechazaCompensacion(string ids, string des, string has)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = _CompensacionAutorizaPersonalService.RechazaCompensacionService(ids, empresa, UsuarioLogeado, des, has);
            return Json(new { info = resultado });
        }
    }
}
