using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.LicenciasMedicas;

namespace RRHH.Controllers
{
    public class LicenciasMedicasController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly ILicenciasMedicasService _licenciasmedicasService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Control tiempo", "Licencias medicas");
        }

        public LicenciasMedicasController(ILicenciasMedicasService licenciasmedicasService)
        {
            _licenciasmedicasService = licenciasmedicasService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarLicenciasMedicas")]
        public async Task<JsonResult> ListarLicenciasMedicas(string rut)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen registros";
            resultado.result = 0;

            List<DetLicenciasMedicas> licenciasmedicas = _licenciasmedicasService.ListarLicenciasMedicasService(empresa, rut);
            if (licenciasmedicas != null)
            {
                resultado.data = licenciasmedicas;
                resultado.mensaje = "Si existen registros";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ConsultarLicenciaMedicaId")]
        public async Task<JsonResult> ConsultarLicenciaMedicaId(int id, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            List<DetLicenciasMedicas> licenciasmedicas = _licenciasmedicasService.ConsultaLicenciaMedicaIdService(id, empresa);
            if (licenciasmedicas != null)
            {
                resultado.data = licenciasmedicas;
                resultado.mensaje = "Si existe licencia";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpPost]
        [Route("CrearLicenciaMedica")]
        public JsonResult CrearLicenciaMedica(DetLicenciasMedicas opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado = _licenciasmedicasService.CrearLicenciaMedicaService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpPost]
        [Route("EditaLicenciaMedica")]
        public JsonResult EditaLicenciaMedica(DetLicenciasMedicas opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado = _licenciasmedicasService.EditaLicenciaMedicaService(opciones, empresa);
            return Json(new { info = resultado });


        }

        [HttpGet]
        [Route("InhabilitaLicenciaMedica")]
        public JsonResult InhabilitaLicenciaMedica(LicenciaDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado = _licenciasmedicasService.InhabilitaLicenciaMedicaService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("CargaTrabajadoresLic")]
        public JsonResult CargaTrabajadoresLic()
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado = _licenciasmedicasService.ComboTrabajadoresService(empresa);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("CargaTiposLicencias")]
        public JsonResult CargaTiposLicencias()
        {
            Resultado resultado = new Resultado();
            resultado = _licenciasmedicasService.ComboTipoLicenciasService();
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("CargaTiposMedicos")]
        public JsonResult CargaTiposMedico()
        {
            Resultado resultado = new Resultado();
            resultado = _licenciasmedicasService.ComboTipoMedicosService();
            return Json(new { info = resultado });
        }
    }
}
