using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.VacacionesPersonal
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IVacacionesPersonalService
    {
        /// <summary>
        /// Genera listado de personal en vacaciones.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista personal en vacaciones</returns>
        public List<DetVacacionesPersonal> ListarVacacionesPersonalService(int idEmpresa, string desde,string hasta);

    }

    public class VacacionesPersonalService : IVacacionesPersonalService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public VacacionesPersonalService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetVacacionesPersonal> ListarVacacionesPersonalService(int empresa,  string desde,string hasta)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                DateTime fecini = DateTime.Now.Date;
                DateTime fecfin = f.UltimoDia(fecini);
                string fecinistr = fecini.ToString("yyyy'-'MM'-'dd");
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");


                f.EjecutarConsultaSQLCli("SELECT vacacionesSolicitud.id,vacacionesSolicitud.rutTrabajador,vacacionesSolicitud.fechaInicio " +
                                            ",vacacionesSolicitud.fechaTermino " +
                                            "FROM vacacionesSolicitud " +
                                            "WHERE vacacionesSolicitud.habilitado = 1 and vacacionesSolicitud.estadoSolicitud = 'Aprueba' "+
                                            " and vacacionesSolicitud.fechainicio <= '" + hasta+"' and vacacionesSolicitud.fechatermino >= '"+desde+"' ", BD_Cli);


                List<DetVacacionesPersonal> opcionesList = new List<DetVacacionesPersonal>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetVacacionesPersonal()
                                    {
                                        ruttrabajador = dr["rutTrabajador"].ToString(),
                                        fechainicio = dr["fechainicio"].ToString(),
                                        fechatermino = dr["fechatermino"].ToString()
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        DateTime inicio = Convert.ToDateTime(r.fechainicio);
                        r.fechainicio = inicio.ToString("dd'-'MM'-'yyyy");
                        DateTime termino = Convert.ToDateTime(r.fechatermino);
                        r.fechatermino = termino.ToString("dd'-'MM'-'yyyy");
                        PersonasBaseVM pers = Generales.BuscaPersona(r.ruttrabajador, BD_Cli);
                        r.nombre = pers.Nombres + " " + pers.Apellidos;
                        r.dias =(int) (Convert.ToDateTime(termino).Date - Convert.ToDateTime(inicio).Date).TotalDays + 1;
                    }
                    return opcionesList;
                }
                else
                {
                 return null;  
                }
            }
            catch (Exception ex)
            {
                var Asunto = "Error en consulta de personal en vacaciones";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar personal en vacaciones", correos.destinatarioErrores);
                return null;
            }

        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetVacacionesPersonal
    {
        public string ruttrabajador { get; set; }
        public string nombre { get; set; }
        public string fechainicio { get; set; }
        public string fechatermino { get; set; }
        public int dias { get; set; }
    }
}


