using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.PermisoConsulta;

namespace RRHH.Controllers
{
    public class PermisoConsultaController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IPermisoConsultaService _PermisoConsultaService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Permisos");
        }

        public PermisoConsultaController(IPermisoConsultaService PermisoConsultaService)
        {
            _PermisoConsultaService = PermisoConsultaService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarPermisosConsulta")]
        public async Task<JsonResult> ListarPermisosConsulta(String desde, string hasta)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            List<DetConsultaPermiso> PermisoConsulta = _PermisoConsultaService.ListarPermisoConsultaService(empresa, desde, hasta);
            if (PermisoConsulta != null)
            {
                resultado.data = PermisoConsulta;
                resultado.mensaje = "Si existen registros";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

    }
}
