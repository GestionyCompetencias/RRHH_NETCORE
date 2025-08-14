using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.ComprobanteContable;

namespace RRHH.Controllers
{
    public class ComprobanteContableController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IComprobanteContableService _ComprobanteContableService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Interfaz contabilidad");
        }

        public ComprobanteContableController(IComprobanteContableService ComprobanteContableService)
        {
            _ComprobanteContableService = ComprobanteContableService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarComprobanteContable")]
        public async Task<JsonResult> ListarComprobanteContable(int mes, int anio, string tipo)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            string pago = "L";
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No se ha encontrado información";
            List<detcomp> detalle = _ComprobanteContableService.ListarComprobanteContableService(empresa, mes, anio, pago, tipo);
            if(detalle != null)
            {
                resultado.result = 1;
                resultado.mensaje = "Existen registros";
                resultado.data = detalle;
            }
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("ProcesarComprobanteContable")]
        public async Task<JsonResult> ProcesarComprobanteContable(int mes, int anio, string tipo)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            string pago = "L";
            resultado = _ComprobanteContableService.ProcesarComprobanteContableService(empresa, mes, anio, pago,tipo);
            return Json(new { info = resultado });
        }

    }
}
