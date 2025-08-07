using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.LicenciasConsulta
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface ILicenciasConsultaService
    {
        /// <summary>
        /// Genera listado de licencias medicas.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista licencias medicas</returns>
        public List<DetConsultaLicencias> ListarLicenciasConsultaService(int idEmpresa, string desde,string hasta);

    }

    public class LicenciasConsultaService : ILicenciasConsultaService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public LicenciasConsultaService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetConsultaLicencias> ListarLicenciasConsultaService(int empresa,  string desde,string hasta)
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


                f.EjecutarConsultaSQLCli("SELECT licenciasmedicas.id,licenciasmedicas.codigoLicencia,licenciasmedicas.rutTrabajador,licenciasmedicas.fechaInicio " +
                                            ",licenciasmedicas.fechaTermino,licenciasmedicas.dias, licenciasmedicas.tipoLicencia, licenciasmedicas.comentario " +
                                            ",licenciasmedicas.tipoMedico,licenciasmedicas.PDF " +
                                            "FROM licenciasmedicas " +
                                            "WHERE licenciasmedicas.habilitado = 1 "+
                                            " and licenciasMedicas.fechainicio <= '" + hasta+"' and licenciasmedicas.fechatermino >= '"+desde+"' ", BD_Cli);


                List<DetConsultaLicencias> opcionesList = new List<DetConsultaLicencias>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetConsultaLicencias()
                                    {
                                        ruttrabajador = dr["rutTrabajador"].ToString(),
                                        codigolicencia = dr["codigoLicencia"].ToString(),
                                        fechainicio = dr["fechainicio"].ToString(),
                                        fechatermino = dr["fechatermino"].ToString(),
                                        dias = int.Parse(dr["dias"].ToString()),
                                        tipolicencia = int.Parse(dr["tipoLicencia"].ToString()),
                                        comentario = dr["comentario"].ToString(),
                                        tipomedico = int.Parse(dr["tipoMedico"].ToString())
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        DateTime inicio = Convert.ToDateTime(r.fechainicio);
                        r.fechainicio = inicio.ToString("dd'-'MM'-'yyyy");
                        DateTime termino = Convert.ToDateTime(r.fechatermino);
                        r.fechatermino = termino.ToString("dd'-'MM'-'yyyy");
                        PersonasBaseVM pers = Generales.BuscaPersona(r.ruttrabajador, BD_Cli);
                        r.nombre = pers.Nombres + " " + pers.Apellidos;
                        r.desclicencia = Generales.BuscaTipoLicencia(r.tipolicencia);
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
    public class DetConsultaLicencias
    {
        public string ruttrabajador { get; set; }
        public string nombre { get; set; }
        public string codigolicencia { get; set; }
        public string fechainicio { get; set; }
        public string fechatermino { get; set; }
        public int dias { get; set; }
        public int tipolicencia { get; set; }
        public string desclicencia { get; set; }
        public string comentario { get; set; }
        public int tipomedico { get; set; }
        public int habilitado { get; set; }
        public BinaryData PDF { get; set; }
    }
}


