using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.AsistenciasInformadas;

namespace RRHH.Controllers
{
    public class AsistenciasInformadasController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IAsistenciasInformadasService _asistenciasinformadasService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Información mensual");
        }

        public AsistenciasInformadasController(IAsistenciasInformadasService asistenciasinformadasService)
        {
            _asistenciasinformadasService = asistenciasinformadasService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarAsistenciasInformadas")]
        public async Task<JsonResult> ListarAsistenciasInformadas(int mes, int anio)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen asistencias informadas";
            resultado.result = 0;

            List<DetAsistenciasInformadas> asistenciasinformadas = _asistenciasinformadasService.ListarAsistenciasInformadasService(empresa, mes, anio);
            if (asistenciasinformadas != null)
            {
                resultado.data = asistenciasinformadas;
                resultado.mensaje = "Si existen hasistencia informada";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ConsultarAsistenciaInformadaId")]
        public async Task<JsonResult> ConsultarAsistenciaInformadaId(int id, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            List<AsistenciasInformadasBaseVM> asistenciasinformadas = _asistenciasinformadasService.ConsultaAsistenciaInformadaIdService(id, empresa);
            if (asistenciasinformadas != null)
            {
                resultado.data = asistenciasinformadas;
                resultado.mensaje = "Si existen asistencia";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpPost]
        [Route("CrearAsistenciaInformada")]
        public JsonResult CrearAsistenciaInformada(DetAsistenciasInformadas opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado = _asistenciasinformadasService.CrearAsistenciaInformadaService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpPost]
        [Route("EditaAsistenciaInformada")]
        public JsonResult EditaHaberInformado(DetAsistenciasInformadas opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _asistenciasinformadasService.EditaAsistenciaInformadaService(opciones, empresa);
            return Json(new { info = resultado });


        }

        [HttpGet]
        [Route("InhabilitaAsistenciaInformada")]
        public JsonResult InhabilitaAsistenciaInformada(AsistenciasInformadasDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 1;
            resultado = _asistenciasinformadasService.InhabilitaAsistenciaInformadaService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("CargaInasistencias")]
        public JsonResult CargaInasistencias()
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 1;
            resultado = _asistenciasinformadasService.ComboInasistenciasService(empresa);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("CargaTrabajadores")]
        public JsonResult CargaTrabajadores()
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 1;
            resultado = _asistenciasinformadasService.ComboTrabajadoresService(empresa);
            return Json(new { info = resultado });
        }
    }
}
