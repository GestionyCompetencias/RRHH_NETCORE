using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.ArchivoDireccionTrabajo;

namespace RRHH.Controllers
{
    public class ArchivoDireccionTrabajoController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        private readonly IArchivoDireccionTrabajoService _ArchivoDireccionTrabajoService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Reportes");
        }

        public ArchivoDireccionTrabajoController(IArchivoDireccionTrabajoService ArchivoDireccionTrabajoService)
        {
            _ArchivoDireccionTrabajoService = ArchivoDireccionTrabajoService;
        }

        public IActionResult Index()
        {

            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpPost]
        [Route("ListarArchivoDireccionTrabajo")]
        public IActionResult ListarArchivoDireccionTrabajo(int mes, int anio)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen registros";
            resultado.result = 0;
            string ruta = Directory.GetCurrentDirectory();
            if (mes > 0 && mes < 13)
            {
                var path = _ArchivoDireccionTrabajoService.ListarArchivoDireccionTrabajoService(empresa, mes, anio, ruta);
                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;
                return File(memory, "direccion/csv", Path.GetFileName(path));
            }
            return Json(new { info = resultado });
        }
    }
}
