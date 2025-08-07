using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.Contratos;

namespace RRHH.Controllers
{
    public class ContratosController : Controller
    {
        private Seguridad seguridad = new Seguridad();
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IContratosService _contratosService;

        public ContratosController(IContratosService contratosService)
        {
            _contratosService = contratosService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Personal", "Contratos");
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }




        [HttpGet]
        [Route("ListarContratos")]
        public async Task<JsonResult> Listarcontratos(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen contratos";
            resultado.result = 0;

            List<ContratosBaseVM> contratos = _contratosService.ListarContratosService(empresa);
            if (contratos != null)
            {
                resultado.data = contratos;
                resultado.mensaje = "Si existen contratos";
                resultado.result = 1;
            }
            return Json(new { info = resultado });

        }

        [HttpGet]
        [Route("ConsultaContratoId")]
        public async Task<JsonResult> ConsultaContratoId(int id, int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existe contrato";
            resultado.result = 0;
            DetalleContrato contrato = _contratosService.ConsultaContratoIdService(id, empresa);
            if (contrato != null)
            {
                resultado.data = contrato;
                resultado.mensaje = "Si existe contrato";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpPost]
        [Route("CrearContrato")]
        public JsonResult CrearContrato(DetalleContrato opciones, int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _contratosService.CrearContratoService(opciones, empresa);
            return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("EditaContrato")]
        public JsonResult EditaContrato(ContratosBaseVM opciones, int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _contratosService.EditaContratoService(opciones, empresa);
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("InhabilitaContrato")]
        public JsonResult InhabilitaContrato(ContratosDeleteVM opciones, int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _contratosService.InhabilitaContratoService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ComboTiposContrato")]
        public async Task<JsonResult> ComboTiposContrato(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado = _contratosService.ComboTiposContratoService(empresa);
            return Json(new { info = resultado });

        }

        [HttpGet]
        [Route("ComboFaenas")]
        public async Task<JsonResult> ComboFaenas(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _contratosService.ComboFaenasService(empresa);
            return Json(new { info = resultado });

        }

        [HttpGet]
        [Route("ComboCargos")]
        public async Task<JsonResult> ComboCargos(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _contratosService.ComboCargosService(empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ComboCentros")]
        public async Task<JsonResult> ComboCentros(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _contratosService.ComboCentrosService(empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ComboJornadas")]
        public async Task<JsonResult> ComboJornadas(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _contratosService.ComboJornadasService(empresa);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("ComboBancos")]
        public async Task<JsonResult> ComboBancos(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _contratosService.ComboBancosService(empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ComboAfps")]
        public async Task<JsonResult> ComboAfps(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _contratosService.ComboAfpsService(empresa);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("ComboIsapres")]
        public async Task<JsonResult> ComboIsapres(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _contratosService.ComboIsapresService(empresa);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("ComboTiposCuentas")]
        public async Task<JsonResult> ComboTiposCuentas(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _contratosService.ComboTiposCuentasService(empresa);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("ExistePersona")]
        public async Task<JsonResult> ExistePersona(string rut, int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _contratosService.ExistePersonaService(rut,empresa);
            return Json(new { info = resultado });

        }
    }
}
