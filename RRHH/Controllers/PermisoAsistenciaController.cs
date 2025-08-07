using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.PermisoAsistencia;

namespace RRHH.Controllers
{
    public class PermisoAsistenciaController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IPermisoAsistenciaService _permisoasistenciaService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Permisos");
        }

        public PermisoAsistenciaController(IPermisoAsistenciaService permisoasistenciaService)
        {
            _permisoasistenciaService = permisoasistenciaService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarPermisoAsistencia")]
        public async Task<JsonResult> ListarPermisoAsistencia(string rut)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen registros";
            resultado.result = 0;

            List<DetPermisoAsistencia> permisoasistencia = _permisoasistenciaService.ListarPermisoAsistenciaService(empresa, rut);
            if (permisoasistencia != null)
            {
                resultado.data = permisoasistencia;
                resultado.mensaje = "Si existen registros";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ConsultarPermisoAsistenciaId")]
        public async Task<JsonResult> ConsultarPermisoAsistenciaId(int id, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            List<DetPermisoAsistencia> permisoasistencia = _permisoasistenciaService.ConsultaPermisoAsistenciaIdService(id, empresa);
            if (permisoasistencia != null)
            {
                resultado.data = permisoasistencia;
                resultado.mensaje = "Si existe licencia";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpPost]
        [Route("CrearPermisoAsistencia")]
        public JsonResult CrearPermisoAsistencia(DetPermisoAsistencia opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado = _permisoasistenciaService.CrearPermisoAsistenciaService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpPost]
        [Route("EditaPermisoAsistencia")]
        public JsonResult EditaPermisoAsistencia(DetPermisoAsistencia opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado = _permisoasistenciaService.EditaPermisoAsistenciaService(opciones, empresa);
            return Json(new { info = resultado });


        }

        [HttpGet]
        [Route("InhabilitaPermisoAsistencia")]
        public JsonResult InhabilitaPermisoAsistencia(PermisoDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado = _permisoasistenciaService.InhabilitaPermisoAsistenciaService(opciones, empresa);
            return Json(new { info = resultado });
        }
    }
}
