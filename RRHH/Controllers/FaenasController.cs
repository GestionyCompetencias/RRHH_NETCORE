using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.Faenas;


namespace RRHH.Controllers
{
    public class FaenasController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IFaenasService _faenasService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Personal", "Empresa");
        }

        public FaenasController(IFaenasService faenasService)
        {
            _faenasService = faenasService;
        }


        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarFaenas")]
        public async Task<JsonResult> ListarFaenas(int empresa)
        {
            UsuarioVM perfiles = new UsuarioVM();
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen faenas";
            resultado.result = 0;

                List<FaenasBaseVM> faenas = _faenasService.ListarFaenasService(empresa);
                if (faenas != null)
                {
                    resultado.data = faenas;
                    resultado.mensaje = "Si existen faenas";
                    resultado.result = 1;
                }
                return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("ConsultaFaenaId")]
        public async Task<JsonResult> ConsultaFaenaId(int id, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen faena";
            resultado.result = 0;
                List<FaenasBaseVM> faenas = _faenasService.ConsultaFaenaIdService(id,empresa);
                if (faenas != null)
                {
                    resultado.data = faenas;
                    resultado.mensaje = "Si existen faena";
                    resultado.result = 1;
                }

                return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("CrearFaena")]
        public JsonResult CrearFaena(FaenasBaseVM opciones, int empresa)
        {
            Resultado resultado = new Resultado();
            resultado = _faenasService.CrearFaenaService(opciones, empresa);
            return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("EditaFaena")]
        public JsonResult EditaFaena(FaenasBaseVM opciones, int empresa)
        {
            Resultado resultado = new Resultado();
            resultado = _faenasService.EditaFaenaService(opciones, empresa);
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("InhabilitaFaena")]
        public JsonResult InhabilitaFaena(FaenasDeleteVM opciones, int empresa)
        {
            Resultado resultado = new Resultado();
            resultado = _faenasService.InhabilitaFaenaService(opciones, empresa);
            return Json(new { info = resultado });
        }
    }
}
