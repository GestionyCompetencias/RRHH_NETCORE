using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.Descuentos;

namespace RRHH.Controllers
{
    public class DescuentosController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IDescuentosService _descuentosService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Conceptos");
        }

        public DescuentosController(IDescuentosService descuentosService)
        {
            _descuentosService = descuentosService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarDescuentos")]
        public async Task<JsonResult> ListarDescuentos(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen descuentos";
            resultado.result = 0;

            List<DetDescuentos> descuentos = _descuentosService.ListarDescuentosService(empresa);
            if (descuentos != null)
            {
                resultado.data = descuentos;
                resultado.mensaje = "Si existen descuentos";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("ConsultaDescuentoId")]
        public async Task<JsonResult> ConsultaDescuentoId(int id, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            List<DescuentoBaseVM> descuentos = _descuentosService.ConsultaDescuentoIdService(id, empresa);
            if (descuentos != null)
            {
                resultado.data = descuentos;
                resultado.mensaje = "Si existen descuentos";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("CrearDescuento")]
        public JsonResult CrearDescuento(DescuentoBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado = _descuentosService.CrearDescuentoService(opciones, empresa);
            return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("EditaDescuento")]
        public JsonResult EditaDescuento(DescuentoBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado = _descuentosService.EditaDescuentoService(opciones, empresa);
            return Json(new { info = resultado });


        }


        [HttpGet]
        [Route("InhabilitaDescuento")]
        public JsonResult InhabilitaDescuento(DescuentoDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _descuentosService.InhabilitaDescuentoService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ComboDescuentosDT")]
        public async Task<JsonResult> ComboDescuentosDT(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado = _descuentosService.ComboCodigosDTService(empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ComboCodigosPrevired")]
        public async Task<JsonResult> ComboCodigosPreviredService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado = _descuentosService.ComboCodigosPreviredService(empresa);
            return Json(new { info = resultado });

        }

    }
}
