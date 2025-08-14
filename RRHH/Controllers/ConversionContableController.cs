using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.ConversionContable;
using Microsoft.Data.SqlClient;


namespace RRHH.Controllers
{
    public class ConversionContableController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IConversionContableService _conversionContableService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Interfaz contabilidad");
        }

        public ConversionContableController(IConversionContableService conversionContableService)
        {
            _conversionContableService = conversionContableService;
        }


        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarConversionContable")]
        public async Task<JsonResult> ListarConversionContable(string tipo)
        {
            UsuarioVM perfiles = new UsuarioVM();
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen conversionContable";
            resultado.result = 0;
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);

            List<ConversionContableBaseVM> conversionContable = _conversionContableService.ListarConversionContableService(empresa,tipo);
            if (conversionContable != null)
            {
                resultado.data = conversionContable;
                resultado.mensaje = "Si existen conversionContable";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("ConsultaConversionContableId")]
        public async Task<JsonResult> ConsultaConversionContableId(int id)
        {

            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen conversion";
            resultado.result = 0;
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            ConversionContableBaseVM conversionContable = _conversionContableService.ConsultaConversionContableIdService(id, empresa);
            if (conversionContable != null)
            {
                resultado.data = conversionContable;
                resultado.mensaje = "Si existen conversiones";
                resultado.result = 1;
            }

            return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("CrearConversionContable")]
        public JsonResult CrearConversionContable(ConversionContableBaseVM opciones)
        {
            Resultado resultado = new Resultado();
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            resultado = _conversionContableService.CrearConversionContableService(opciones, empresa);
            return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("EditaConversionContable")]
        public JsonResult EditaConversionContable(ConversionContableBaseVM opciones)
        {
            Resultado resultado = new Resultado();
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            resultado = _conversionContableService.EditaConversionContableService(opciones, empresa);
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("InhabilitaConversionContable")]
        public JsonResult InhabilitaConversionContable(ConversionContableDeleteVM opciones)
        {
            Resultado resultado = new Resultado();
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            resultado = _conversionContableService.InhabilitaConversionContableService(opciones, empresa);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("ComboConceptos")]
        public JsonResult ComboConceptos()
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado = _conversionContableService.CargaConceptosService(empresa);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("ComboCuentas")]
        public JsonResult ComboCuentas()
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado = _conversionContableService.CargaCuentasService(empresa);
            return Json(new { info = resultado });
        }
    }
}
