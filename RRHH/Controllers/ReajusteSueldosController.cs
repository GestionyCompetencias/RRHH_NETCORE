using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.ReajusteSueldos;

namespace RRHH.Controllers
{
    public class ReajusteSueldosController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IReajusteSueldosService _ReajusteSueldosService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Procesos");
        }

        public ReajusteSueldosController(IReajusteSueldosService ReajusteSueldosService)
        {
            _ReajusteSueldosService = ReajusteSueldosService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ProcesaReajusteSueldos")]
        public async Task<JsonResult> ProcesaReajusteSueldos(string reajuste)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen registros";
            resultado.result = 0;
            Decimal reajustede = Convert.ToDecimal(reajuste);
            List<DetReajusteSueldos> ReajusteSueldos = _ReajusteSueldosService.ListarReajusteSueldosService(empresa, reajustede);
            if (ReajusteSueldos != null)
            {
                resultado.data = ReajusteSueldos;
                resultado.mensaje = "Si existen registros";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

    }
}
