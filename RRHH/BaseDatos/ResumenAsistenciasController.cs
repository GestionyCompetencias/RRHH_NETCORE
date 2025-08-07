using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.ResumenAsistencias;

namespace RRHH.Controllers
{
    public class ResumenAsistenciasController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        private readonly IResumenAsistenciasService _ResumenAsistenciasService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Reportes");
        }

        public ResumenAsistenciasController(IResumenAsistenciasService ResumenAsistenciasService)
        {
            _ResumenAsistenciasService = ResumenAsistenciasService;
        }

        public IActionResult Index()
        {

            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarResumenAsistencias")]
        public async Task<JsonResult> ListarResumenAsistencias(int mes, int anio)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existe información en la fecha solicitada";
            resultado.result = 0;
            if (mes > 0 && mes < 13)
            {
                List<DetAsistencias> ResumenAsistencias = _ResumenAsistenciasService.ListarResumenAsistenciasService(empresa, mes, anio);
                if (ResumenAsistencias != null)
                {
                    resultado.data = ResumenAsistencias;
                    resultado.mensaje = "Si existe información";
                    resultado.result = 1;
                }

            }
            return Json(new { info = resultado });
        }

    }
}
