using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.CierraProceso;

namespace RRHH.Controllers
{
    public class CierraProcesoController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly ICierraProcesoService _CierraProcesoService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Proceso");
        }

        public CierraProcesoController(ICierraProcesoService CierraProcesoService)
        {
            _CierraProcesoService = CierraProcesoService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ProcesaCierraProceso")]
        public async Task<JsonResult> ProcesaCierraProceso(int mes, int anio, string pago)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existe proceso a cerrar";
            resultado.result = 0;
            if (_CierraProcesoService.ProcesaCierraProcesoService(empresa, mes, anio, pago))
            {
                resultado.data = null;
                resultado.mensaje = "Cierre realizado exitosamente";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

    }
}
