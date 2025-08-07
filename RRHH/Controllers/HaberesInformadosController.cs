using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.HaberesInformados;

namespace RRHH.Controllers
{
    public class HaberesInformadosController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IHaberesInformadosService _haberesinformadosService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Información mensual");
        }

        public HaberesInformadosController(IHaberesInformadosService haberesinformadosService)
        {
            _haberesinformadosService = haberesinformadosService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarHaberesInformados")]
        public async Task<JsonResult> ListarHaberesInformados(int codigo, int mes, int anio, string pago)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen haberes informados";
            resultado.result = 0;

            List<DetHaberesInformados> haberesinformados = _haberesinformadosService.ListarHaberesInformadosService(empresa, codigo, mes,anio,pago);
            if (haberesinformados != null)
            {
                resultado.data = haberesinformados;
                resultado.mensaje = "Si existen haberes informados";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ConsultarHaberInformadoId")]
        public async Task<JsonResult> ConsultarHaberInformadoId(int id, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            List<HaberesInformadosBaseVM> haberesinformados = _haberesinformadosService.ConsultaHaberInformadoIdService(id, empresa);
            if (haberesinformados != null)
            {
                resultado.data = haberesinformados;
                resultado.mensaje = "Si existen haber informado";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpPost]
        [Route("CrearHaberInformado")]
        public JsonResult CrearHaberInformado(DetHaberesInformados opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado = _haberesinformadosService.CrearHaberInformadoService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpPost]
        [Route("EditaHaberInformado")]
        public JsonResult EditaHaberInformado(DetHaberesInformados opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _haberesinformadosService.EditaHaberInformadoService(opciones, empresa);
            return Json(new { info = resultado });


        }

        [HttpGet]
        [Route("InhabilitaHaberInformado")]
        public JsonResult InhabilitaHaberInformado(HaberesInformadosDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado = _haberesinformadosService.InhabilitaHaberInformadoService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("CargaHaberes")]
        public JsonResult CargaHaberes()
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado = _haberesinformadosService.ComboHaberesService(empresa);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("CargaTrabajadores")]
        public JsonResult CargaTrabajadores()
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado = _haberesinformadosService.ComboTrabajadoresService(empresa);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("CargaFechas")]
        public JsonResult CargaFechas(int mes, int anio)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "Carga de fechas";
            resultado.result = 1;
            List<string> fecstr = new List<string>();
            DateTime fecini = new DateTime(anio,mes,1);
            DateTime fecfin = f.UltimoDia(fecini);
            fecstr.Add(fecini.ToString("yyyy'-'MM'-'dd"));
            fecstr.Add(fecfin.ToString("yyyy'-'MM'-'dd"));
            resultado.data = fecstr;
            return Json(new { info = resultado });
        }
    }
}
