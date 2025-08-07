using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.SaldosLiquidos;

namespace RRHH.Controllers
{
    public class SaldosLiquidosController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        private readonly ISaldosLiquidosService _SaldosLiquidosService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Reportes");
        }

        public SaldosLiquidosController(ISaldosLiquidosService SaldosLiquidosService)
        {
            _SaldosLiquidosService = SaldosLiquidosService;
        }

        public IActionResult Index()
        {

            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarSaldosLiquidos")]
        public async Task<JsonResult> ListarSaldosLiquidos(int mes, int anio)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen registros";
            resultado.result = 0;
            if (mes > 0 && mes < 13)
            {
                List<DetSaldos> SaldosLiquidos = _SaldosLiquidosService.ListarSaldosLiquidosService(empresa, mes, anio);
                if (SaldosLiquidos != null)
                {
                    resultado.data = SaldosLiquidos;
                    resultado.mensaje = "Si existen registros";
                    resultado.result = 1;
                }

            }
            return Json(new { info = resultado });
        }

    }
}
