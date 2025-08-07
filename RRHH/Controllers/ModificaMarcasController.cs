using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.ModificaMarcas;

namespace RRHH.Controllers
{
    public class ModificaMarcasController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        private readonly IModificaMarcasService _modificamarcasService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Ajuste de marcación");
        }

        public ModificaMarcasController(IModificaMarcasService modificamarcasService)
        {
            _modificamarcasService = modificamarcasService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarMarcasTrabajador")]
        public async Task<JsonResult> ListarMarcasTrabajador(string rut, string des, string has)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen asistencias informadas";
            resultado.result = 0;

            List<DetMarcasTrabajador> marcastrabajador = _modificamarcasService.ListarMarcasTrabajadorService(empresa, des, has,rut);
            if (marcastrabajador != null)
            {
                resultado.data = marcastrabajador;
                resultado.mensaje = "Si existen hasistencia informada";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ConsultaMarcaTrabajador")]
        public async Task<JsonResult> ConsultaMarcaTrabajador(string id)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            DetMarcasTrabajador marcastrabajador = _modificamarcasService.ConsultaMarcaTrabajadorIdService(id, empresa);
            if (marcastrabajador != null)
            {
                resultado.data = marcastrabajador;
                resultado.mensaje = "Si existen asistencia";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpPost]
        [Route("ModificaMarcaTrabajador")]
        public JsonResult ModificaMarcaTrabajador(DetMarcasTrabajador opciones)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = _modificamarcasService.ModificaMarcaTrabajadorService(opciones, empresa,UsuarioLogeado);
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("InhabilitaMarca")]
        public JsonResult InhabilitaMarca(MarcacionDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 1;
            resultado = _modificamarcasService.InhabilitaMarcaTrabajadorService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ComboPersonas")]
        public JsonResult ComboPersonas()
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 1;
            resultado = _modificamarcasService.ComboTrabajadoresService(empresa);
            return Json(new { info = resultado });
        }
    }
}
