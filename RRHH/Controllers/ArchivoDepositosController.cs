using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.ArchivoDepositos;

namespace RRHH.Controllers
{
    public class ArchivoDepositosController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        private readonly IArchivoDepositosService _ArchivoDepositosService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Reportes");
        }

        public ArchivoDepositosController(IArchivoDepositosService ArchivoDepositosService)
        {
            _ArchivoDepositosService = ArchivoDepositosService;
        }

        public IActionResult Index()
        {

            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarArchivoDepositos")]
        public async Task<JsonResult> ListarArchivoDepositos(int mes, int anio)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen registros";
            resultado.result = 0;
            if (mes > 0 && mes < 13)
            {
                List<DetDeposito> ArchivoDepositos = _ArchivoDepositosService.ListarArchivoDepositosService(empresa, mes, anio);
                if (ArchivoDepositos != null)
                {
                    resultado.data = ArchivoDepositos;
                    resultado.mensaje = "Si existen registros";
                    resultado.result = 1;
                }

            }
            return Json(new { info = resultado });
        }

    }
}
