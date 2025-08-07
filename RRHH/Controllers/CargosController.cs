using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.Cargos;

namespace RRHH.Controllers
{
    public class CargosController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly ICargosService _cargosService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Personal", "Empresa");
        }

        public CargosController(ICargosService cargosService)
        {
            _cargosService = cargosService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarCargos")]
        public async Task<JsonResult> ListarCargos(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen cargos";
            resultado.result = 0;

            List<DetCargos> cargos = _cargosService.ListarCargosService(empresa);
            if (cargos != null)
            {
                resultado.data = cargos;
                resultado.mensaje = "Si existen cargos";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("ConsultaCargoId")]
        public async Task<JsonResult> ConsultaCargoId(int id, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            List<CargosBaseVM> cargos = _cargosService.ConsultaCargoIdService(id, empresa);
            if (cargos != null)
            {
                resultado.data = cargos;
                resultado.mensaje = "Si existen cargo";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("CrearCargo")]
        public JsonResult CrearCargo(CargosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _cargosService.CrearCargoService(opciones, empresa);
            return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("EditaCargo")]
        public JsonResult EditaCargo(CargosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _cargosService.EditaCargoService(opciones, empresa);
            return Json(new { info = resultado });


        }


        [HttpGet]
        [Route("InhabilitaCargo")]
        public JsonResult InhabilitaCargo(CargosDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _cargosService.InhabilitaCargoService(opciones, empresa);
                return Json(new { info = resultado });
        }
    }
}
