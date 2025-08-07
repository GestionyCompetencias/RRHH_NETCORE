using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.ReporteMarcas;

namespace RRHH.Controllers
{
    public class ReporteMarcasController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        private readonly IReporteMarcasService _reportemarcasService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Reportes");
        }

        public ReporteMarcasController(IReporteMarcasService reportemarcasService)
        {
            _reportemarcasService = reportemarcasService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarMarcas")]
        public async Task<JsonResult> ListarMarcas(string rut ,string des, string has)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen marcas";
            resultado.result = 0;

            List<DetMarcas> marcas = _reportemarcasService.ListarMarcasTrabajadorService(empresa, des, has, rut);
            if (marcas != null)
            {
                resultado.data = marcas;
                resultado.mensaje = "Si existen marcas";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("VerImagen")]
        public ActionResult VerImagen(string id)
        {
            string[] campos = id.Split('*');
            string rut = campos[0];
            DateTime fecha = Convert.ToDateTime(campos[1]);
            string tipo = campos[2];
            FileStreamResult marcas = _reportemarcasService.MuestraImagenService( rut, fecha, tipo);
            if (marcas != null)
            {
                return marcas;
            }
            return null;
        }
    }
}
