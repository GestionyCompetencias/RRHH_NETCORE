using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.Liquidacion;

namespace RRHH.Controllers
{
    public class LiquidacionController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly ILiquidacionService _LiquidacionService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Procesos");
        }

        public LiquidacionController(ILiquidacionService LiquidacionService)
        {
            _LiquidacionService = LiquidacionService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ProcesarLiquidacion")]
        public async Task<JsonResult> ProcesarLiquidacion(int mes, int anio)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            string pago = "L";
            resultado = _LiquidacionService.ProcesarLiquidacionService(empresa, mes, anio, pago,UsuarioLogeado);
            return Json(new { info = resultado });
        }

    }
}
