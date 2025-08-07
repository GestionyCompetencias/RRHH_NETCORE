using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.CuadraturaAsistencias;

namespace RRHH.Controllers
{
    public class CuadraturaAsistenciasController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly ICuadraturaAsistenciasService _CuadraturaAsistenciasService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Información mensual");
        }

        public CuadraturaAsistenciasController(ICuadraturaAsistenciasService CuadraturaAsistenciasService)
        {
            _CuadraturaAsistenciasService = CuadraturaAsistenciasService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarCuadraturaAsistencias")]
        public async Task<JsonResult> ListarCuadraturaAsistencias(int mes, int anio)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen registros de asistencia";
            resultado.result = 0;
            string pago = "L";
            List<ResCuadraturaAsistencias> CuadraturaAsistencias = _CuadraturaAsistenciasService.ListarCuadraturaAsistenciasService(empresa, mes, anio, pago);
            if (CuadraturaAsistencias != null)
            {
                resultado.data = CuadraturaAsistencias;
                resultado.mensaje = "Si existen registros";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

    }
}
