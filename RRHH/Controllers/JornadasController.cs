using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.Jornadas;

namespace RRHH.Controllers
{
    public class JornadasController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IJornadasService _jornadasService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Personal", "Empresa");
        }

        public JornadasController(IJornadasService jornadasService)
        {
            _jornadasService = jornadasService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarJornadas")]
        public async Task<JsonResult> ListarJornadas(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen jornadas";
            resultado.result = 0;

            List<DetJornadas> jornadas = _jornadasService.ListarJornadasService(empresa);
            if (jornadas != null)
            {
                resultado.data = jornadas;
                resultado.mensaje = "Si existen jornadas";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }


        [HttpGet]
        [Route("ConsultaJornadaId")]
        public async Task<JsonResult> ConsultaJornadaId(int id, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            List<JornadasBaseVM> jornadas = _jornadasService.ConsultaJornadaIdService(id, empresa);
            if (jornadas != null)
            {
                resultado.data = jornadas;
                resultado.mensaje = "Si existen jornada";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("CrearJornada")]
        public JsonResult CrearJornada(JornadasBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _jornadasService.CrearJornadaService(opciones, empresa);
            return Json(new { info = resultado });
        }


        [HttpPost]
        [Route("EditaJornada")]
        public JsonResult EditaJornada(JornadasBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _jornadasService.EditaJornadaService(opciones, empresa);
            return Json(new { info = resultado });


        }


        [HttpGet]
        [Route("InhabilitaJornada")]
        public JsonResult InhabilitaJornada(JornadasDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _jornadasService.InhabilitaJornadaService(opciones, empresa);
            return Json(new { info = resultado });
        }
    }
}
public class DetJornadas
{
    public int id { get; set; }
    public string codigo { get; set; }
    public string descripcion { get; set; }
    public int diasTrabajo { get; set; }
    public int diasDescanso { get; set; }
    public int diasTotales { get; set; }
    public int numeroCiclos { get; set; }
    public int horasSemanales { get; set; }
    public string fechaCreacion { get; set; }
    public string resolucion { get; set; }
    public string fechaResolucion { get; set; }
}
