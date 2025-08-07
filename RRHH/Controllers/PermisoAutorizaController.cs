using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.PermisoAutoriza;

namespace RRHH.Controllers
{
    public class PermisoAutorizaController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IPermisoAutorizaService _PermisoAutorizaService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Permisos");
        }

        public PermisoAutorizaController(IPermisoAutorizaService PermisoAutorizaService)
        {
            _PermisoAutorizaService = PermisoAutorizaService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarPermisoAutoriza")]
        public async Task<JsonResult> ListarPermisoAutoriza(String desde, string hasta)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            List<DetPermisoAsistencia> PermisoAutoriza = _PermisoAutorizaService.ListarPermisoAutorizaService(empresa, desde, hasta);
            if (PermisoAutoriza != null)
            {
                resultado.data = PermisoAutoriza;
                resultado.mensaje = "Si existen registros";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("AutorizaPermisos")]
        public JsonResult AutorizaPermisos(string ids, string des, string has)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "Debe selecionar registros";
            if(ids != null && ids.Length != 0)
            {
                string empresastr = HttpContext.Session.GetString("EmpresaLog");
                string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
                int empresa = int.Parse(empresastr);
                resultado = _PermisoAutorizaService.AutorizaPermisosService(ids, empresa, UsuarioLogeado, des, has);
            }
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("RechazaPermisos")]
        public JsonResult RechazaPermisos(string ids, string des, string has)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "Debe selecionar registros";
            if (ids != null && ids.Length != 0)
            {
                string empresastr = HttpContext.Session.GetString("EmpresaLog");
                string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
                int empresa = int.Parse(empresastr);
                resultado = _PermisoAutorizaService.RechazaPermisosService(ids, empresa, UsuarioLogeado, des, has);
            }
            return Json(new { info = resultado });
        }
    }
}
