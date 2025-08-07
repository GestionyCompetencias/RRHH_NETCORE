using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.PermisoConsulta
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IPermisoConsultaService
    {
        /// <summary>
        /// Genera listado de permisos.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista permisos</returns>
        public List<DetConsultaPermiso> ListarPermisoConsultaService(int idEmpresa, string desde,string hasta);

    }

    public class PermisoConsultaService : IPermisoConsultaService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public PermisoConsultaService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetConsultaPermiso> ListarPermisoConsultaService(int empresa,  string desde,string hasta)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                DateTime fecfin = Convert.ToDateTime(hasta).AddDays(1);
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");


                f.EjecutarConsultaSQLCli("SELECT permisoasistencia.id,permisoasistencia.rutTrabajador,permisoasistencia.fechaInicio " +
                                            ",permisoasistencia.fechaTermino,permisoasistencia.goseSueldo, permisoasistencia.comentario " +
                                            "FROM permisoasistencia " +
                                            "WHERE permisoasistencia.habilitado = 1 and permisoasistencia.estado = ''" +
                                            " and permisoasistencia.fechainicio <= '" + fecfinstr+ "' and permisoasistencia.fechatermino >= '" + desde+"' ", BD_Cli);


                List<DetConsultaPermiso> opcionesList = new List<DetConsultaPermiso>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetConsultaPermiso()
                                    {
                                        ruttrabajador = dr["rutTrabajador"].ToString(),
                                        fechainicio = dr["fechainicio"].ToString(),
                                        fechatermino = dr["fechatermino"].ToString(),
                                        gosesueldo = bool.Parse(dr["goseSueldo"].ToString()),
                                        comentario = dr["comentario"].ToString(),
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        DateTime inicio = Convert.ToDateTime(r.fechainicio);
                        r.fechainicio = inicio.ToString("dd'-'MM'-'yyyy");
                        DateTime termino = Convert.ToDateTime(r.fechatermino);
                        r.fechatermino = termino.ToString("dd'-'MM'-'yyyy");
                        PersonasBaseVM pers = Generales.BuscaPersona(r.ruttrabajador, BD_Cli);
                        r.horainicio = inicio.ToString("HH':'mm");
                        r.horatermino = termino.ToString("HH':'mm");
                        r.nombre = pers.Nombres + " " + pers.Apellidos;
                        r.gose = 0;
                        r.gosestr = "NO";
                        if (r.gosesueldo)
                        {
                            r.gose = 1;
                            r.gosestr = "SI";
                        }
                        r.dias = (int)termino.Subtract(inicio).TotalDays +1;
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
                var Asunto = "Error en consulta de licencias";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar licencias", correos.destinatarioErrores);
                return null;
            }

        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetConsultaPermiso
    {
        public string ruttrabajador { get; set; }
        public string nombre { get; set; }
        public string fechainicio { get; set; }
        public string horainicio { get; set; }
        public string fechatermino { get; set; }
        public string horatermino { get; set; }
        public bool gosesueldo { get; set; }
        public int gose { get; set; }
        public string gosestr { get; set; }
        public int dias { get; set; }
        public string comentario { get; set; }
        public int habilitado { get; set; }
    }
}


