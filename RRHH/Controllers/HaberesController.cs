using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.Haberes;

namespace RRHH.Controllers
{
    public class HaberesController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IHaberesService _haberesService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Conceptos");
        }

        public HaberesController(IHaberesService haberesService)
        {
            _haberesService = haberesService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarHaberes")]
        public async Task<JsonResult> ListarHaberes(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen haberes";
            resultado.result = 0;

            List<HaberBaseVM> haberes = _haberesService.ListarHaberesService(empresa);
            if (haberes != null)
            {
                resultado.data = haberes;
                resultado.mensaje = "Si existen haberes";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("ConsultaHaberId")]
        public async Task<JsonResult> ConsultaHaberId(int id, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            List<HaberBaseVM> haberes = _haberesService.ConsultaHaberIdService(id, empresa);
            if (haberes != null)
            {
                resultado.data = haberes;
                resultado.mensaje = "Si existen habere";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("CrearHaber")]
        public JsonResult CrearHaber(HaberBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado = _haberesService.CrearHaberService(opciones, empresa);
            return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("EditaHaber")]
        public JsonResult EditaHaber(HaberBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado = _haberesService.EditaHaberService(opciones, empresa);
            return Json(new { info = resultado });


        }


        [HttpGet]
        [Route("InhabilitaHaber")]
        public JsonResult InhabilitaHaber(HaberDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _haberesService.InhabilitaHaberService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ComboCodigosDT")]
        public async Task<JsonResult> ComboCodigosDT(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado = _haberesService.ComboCodigosDTService(empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ComboCodigosPrevired")]
        public async Task<JsonResult> ComboCodigosPreviredService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado = _haberesService.ComboCodigosPreviredService(empresa);
            return Json(new { info = resultado });

        }

    }
}
