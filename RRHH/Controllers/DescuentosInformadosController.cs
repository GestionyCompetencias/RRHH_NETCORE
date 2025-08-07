using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.DescuentosInformados;

namespace RRHH.Controllers
{
    public class DescuentosInformadosController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IDescuentosInformadosService _descuentosinformadosService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Información mensual");
        }

        public DescuentosInformadosController(IDescuentosInformadosService descuentosinformadosService)
        {
            _descuentosinformadosService = descuentosinformadosService;
        }

        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
 
            return View();
        }

        [HttpGet]
        [Route("ListarDescuentosInformados")]
        public async Task<JsonResult> ListarDescuentosInformados(int codigo, int mes, int anio,string pago)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen descuentos informados";
            resultado.result = 0;
            List<DetDescuentosInformados> descuentosinformados = _descuentosinformadosService.ListarDescuentosInformadosService(empresa, codigo,mes,anio,pago);
            if (descuentosinformados != null)
            {
                resultado.data = descuentosinformados;
                resultado.mensaje = "Si existen descuentos informados";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("ConsultarDescuentoInformadoId")]
        public async Task<JsonResult> ConsultarDescuentoInformadoId(int id, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            List<DescuentosInformadosBaseVM> descuentosinformados = _descuentosinformadosService.ConsultaDescuentoInformadoIdService(id, empresa);
            if (descuentosinformados != null)
            {
                resultado.data = descuentosinformados;
                resultado.mensaje = "Si existen descuento informado";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

        [HttpPost]
        [Route("CrearDescuentoInformado")]
        public JsonResult CrearDescuentoInformado(DescuentosInformadosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado = _descuentosinformadosService.CrearDescuentoInformadoService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpPost]
        [Route("EditaDescuentoInformado")]
        public JsonResult EditaDescuentoInformado(DescuentosInformadosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado = _descuentosinformadosService.EditaDescuentoInformadoService(opciones, empresa);
            return Json(new { info = resultado });


        }

        [HttpGet]
        [Route("InhabilitaDescuentoInformado")]
        public JsonResult InhabilitaDescuentoInformado(DescuentosInformadosDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 1;
            resultado = _descuentosinformadosService.InhabilitaDescuentoInformadoService(opciones, empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("CargaDescuentos")]
        public JsonResult CargaDescuentos()
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 1;
            resultado = _descuentosinformadosService.ComboDescuentosService(empresa);
            return Json(new { info = resultado });
        }
        [HttpGet]
        [Route("CargaTrabajadoresDct")]
        public JsonResult CargaTrabajadoresDct()
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result =1;
            resultado = _descuentosinformadosService.ComboTrabajadoresService(empresa);
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("CargaMes")]
        public JsonResult CargaMesHab()
        {
            Resultado resultado = new Resultado();
            resultado = f.Meses();
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("CargaAnio")]
        public JsonResult CargaAnioHab()
        {
            Resultado resultado = new Resultado();
            resultado = f.Anos();
            return Json(new { info = resultado });
        }

        [HttpGet]
        [Route("CargaPago")]
        public JsonResult CargaPago()
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 1;
            resultado = f.TiposPagos(empresa);
            return Json(new { info = resultado });
        }

        //[HttpGet]
        //[Route("CargaFechas")]
        //public JsonResult CargaFechas(int mes, int anio)
        //{
        //    Resultado resultado = new Resultado();
        //    resultado.mensaje = "Carga de fechas";
        //    resultado.result = 1;
        //    List<string> fecstr = new List<string>();
        //    DateTime fecini = new DateTime(anio, mes, 1);
        //    DateTime fecfin = f.UltimoDia(fecini);
        //    fecstr.Add(fecini.ToString("yyyy'-'MM'-'dd"));
        //    fecstr.Add(fecfin.ToString("yyyy'-'MM'-'dd"));
        //    resultado.data = fecstr;
        //    return Json(new { info = resultado });
        //}

    }
}
