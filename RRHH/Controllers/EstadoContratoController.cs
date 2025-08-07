using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.EstadoContratos;

namespace RRHH.Controllers
{
    public class EstadoContratoController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        string RutEmpresa = "";
        private readonly IEstadoContratosService _estadocontratosService ;

        public EstadoContratoController(IEstadoContratosService estadocontratosService)
        {
            _estadocontratosService = estadocontratosService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Personal", "Empresa");
        }


        public IActionResult Index()
        {
            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpGet]
        [Route("ListarEstadoContratos")]
        public async Task<JsonResult> ListarEstadoContratos(int empresa, int estado)
        {
            {
                Resultado resultado = new Resultado();
                resultado.mensaje = "No existen contratos";
                resultado.result = 0;

                List<ContratosBaseVM> contratos = _estadocontratosService.ListarContratosService(empresa,estado);
                if (contratos != null)
                {
                    resultado.data = contratos;
                    resultado.mensaje = "Si existen contratos";
                    resultado.result = 1;
                }
                return Json(new { info = resultado });

            }
        }

        [HttpGet]
        [Route("ConsultaEstadoContratoId")]
        public async Task<JsonResult> ConsultaEstadoContratoId(int id, int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existe contrato";
            resultado.result = 0;
            List<ContratosBaseVM> contrato = _estadocontratosService.ConsultaContratoIdService(id, empresa);
            if (contrato != null)
            {
                resultado.data = contrato;
                resultado.mensaje = "Si existe contrato";
                resultado.result = 1;
            }
            return Json(new { info = resultado });
        }

    }
}
