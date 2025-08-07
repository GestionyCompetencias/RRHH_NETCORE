using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.Parametros;

namespace RRHH.Controllers
{
    public class ParametrosController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IParametrosService _parametrosService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Personal", "Empresa");
        }

        public ParametrosController(IParametrosService parametrosService)
        {
            _parametrosService = parametrosService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarParametros")]
        public async Task<JsonResult> ListarParametros(int empresa, string tabla)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen parametros";
            resultado.result = 0;

            List<ParametrosBaseVM> parametros = _parametrosService.ListarParametrosService(empresa, tabla);
            if (parametros != null)
            {
                resultado.data = parametros;
                resultado.mensaje = "Si existen parametros";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ConsultaParametroId")]
        public async Task<JsonResult> ConsultaParametroId(int id, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            List<ParametrosBaseVM> parametros = _parametrosService.ConsultaParametroIdService(id, empresa);
            if (parametros != null)
            {
                resultado.data = parametros;
                resultado.mensaje = "Si existen parametro";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpPost]
        [Route("CrearParametro")]
        public JsonResult CrearParametro(ParametrosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _parametrosService.CrearParametroService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpPost]
        [Route("EditaParametro")]
        public JsonResult EditaParametro(ParametrosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _parametrosService.EditaParametroService(opciones, empresa);
            return Json(new { info = resultado });


        }

        [HttpGet]
        [Route("InhabilitaParametro")]
        public JsonResult InhabilitaParametro(ParametrosDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _parametrosService.InhabilitaParametroService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("CargaTablas")]
        public JsonResult CargaTablas(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _parametrosService.ComboTablasService(empresa);
            return Json(new { info = resultado });
        }
    }
}
