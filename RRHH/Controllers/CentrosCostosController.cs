using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.CentrosCostos;

namespace RRHH.Controllers
{
    public class CentrosCostosController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly ICentrosCostoService _centrosCostosService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Personal", "Empresa");
        }

        public CentrosCostosController(ICentrosCostoService centrosCostosService)
        {
            _centrosCostosService = centrosCostosService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarCentrosCostos")]
        public async Task<JsonResult> ListarCentrosCostos(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen centros de costos";
            resultado.result = 0;

            List<CentrosCostosBaseVM> centrosCostos = _centrosCostosService.ListarCentrosCostosService(empresa);
            if (centrosCostos != null)
            {
                resultado.data = centrosCostos;
                resultado.mensaje = "Si existen centros de costos";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("ConsultaCentrosCostosId")]
        public async Task<JsonResult> ConsultaCentrosCostosId(int id, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            List<CentrosCostosBaseVM> centrosCostos = _centrosCostosService.ConsultaCentrosCostoIdService(id, empresa);
            if (centrosCostos != null)
            {
                resultado.data = centrosCostos;
                resultado.mensaje = "Si existen centrosCosto";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("CrearCentrosCostos")]
        public JsonResult CrearCentrosCostos(CentrosCostosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _centrosCostosService.CrearCentrosCostoService(opciones, empresa);
            return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("EditaCentrosCostos")]
        public JsonResult EditaCentrosCostos(CentrosCostosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _centrosCostosService.EditaCentrosCostoService(opciones, empresa);
            return Json(new { info = resultado });


        }


        [HttpGet]
        [Route("InhabilitaCentrosCostos")]
        public JsonResult InhabilitaCentrosCostos(CentrosCostosDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _centrosCostosService.InhabilitaCentrosCostoService(opciones, empresa);
            return Json(new { info = resultado });
        }
    }
}
