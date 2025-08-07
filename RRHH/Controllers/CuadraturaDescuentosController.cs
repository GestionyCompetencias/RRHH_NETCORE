using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.CuadraturaDescuentos;

namespace RRHH.Controllers
{
    public class CuadraturaDescuentosController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly ICuadraturaDescuentosService _CuadraturaDescuentosService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Información mensual");
        }

        public CuadraturaDescuentosController(ICuadraturaDescuentosService CuadraturaDescuentosService)
        {
            _CuadraturaDescuentosService = CuadraturaDescuentosService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarCuadraturaDescuentos")]
        public async Task<JsonResult> ListarCuadraturaDescuentos(int mes, int anio)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen descuentos informados";
            resultado.result = 0;
            string pago = "L";
            List<ResCuadraturaDescuentos> CuadraturaDescuentos = _CuadraturaDescuentosService.ListarCuadraturaDescuentosService(empresa, mes, anio, pago);
            if (CuadraturaDescuentos != null)
            {
                resultado.data = CuadraturaDescuentos;
                resultado.mensaje = "Si existen descuentos informados";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

    }
}
