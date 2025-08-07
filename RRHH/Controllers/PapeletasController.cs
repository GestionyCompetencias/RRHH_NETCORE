using RRHH.BaseDatos;
using RRHH.Models.Estructuras.Template.Componentes;
using RRHH.Models.ViewModels;
using RRHH.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using RRHH.Servicios.Papeletas;
using Rotativa.AspNetCore;
using Rollbar.DTOs;

namespace RRHH.Controllers
{
    public class PapeletasController : Controller
    {
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Seguridad seguridad = new Seguridad();
        private Generales Generales = new Generales();
        private readonly IPapeletasService _PapeletasService;


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewData["breadcrumb"] = new Breadcrumb("Remuneraciones", "Reportes");
        }

        public PapeletasController(IPapeletasService PapeletasService)
        {
            _PapeletasService = PapeletasService;
        }

        public IActionResult Index()
        {

            string UsuarioLogeado = HttpContext.Session.GetString("UserIdLog");
            if (seguridad.ValidarUsuario(UsuarioLogeado) == false) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpPost]
        [Route("ListarPapeletas")]
        public IActionResult ListarPapeletas(int mes, int anio)
        {
            string empresastr = HttpContext.Session.GetString("EmpresaLog");
            int empresa = int.Parse(empresastr);
            EmpresasVM empr = f.obtenerEmpresa(empresa);
            string RutEmpresa = empr.rut;
            string ruta = Directory.GetCurrentDirectory();
            string filename = "Pap-" + empr.rut + "-" + anio.ToString("##") + mes.ToString("##")+".pdf";
            if (mes > 0 && mes < 13)
            {
                List<Papeleta> Papeletas = _PapeletasService.ListarPapeletasService(empresa, mes, anio);
                if (Papeletas.Count > 0)
                {
                    RotativaConfiguration.Setup(ruta);
                    return new ViewAsPdf("PapeletaMasiva", Papeletas)
                    {
                        FileName = filename,
                        PageOrientation =  Rotativa.AspNetCore.Options.Orientation.Portrait,
                        PageHeight = 216,
                        PageSize = Rotativa.AspNetCore.Options.Size.Letter,
                        PageMargins = { Left = 0, Top = 10, Right = 0, Bottom = 0 },
                        CustomSwitches = "--disable-smart-shrinking"
                    };
                }
            }
            return View();
        }
    }
}
