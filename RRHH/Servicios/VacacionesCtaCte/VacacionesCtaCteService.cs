using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.VacacionesCtaCte
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IVacacionesCtaCteService
    {
        /// <summary>
        /// Genera listado ctacte de vacaciones.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista ctacte de vacaciones</returns>
        public Resultado ListarVacacionesCtaCteService(int idEmpresa, string desde,string hasta);

    }

    public class VacacionesCtaCteService : IVacacionesCtaCteService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public VacacionesCtaCteService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public Resultado ListarVacacionesCtaCteService(int empresa,  string desde,string hasta)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen registros";
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                DateTime fecini = DateTime.Now.Date;
                DateTime fecfin = f.UltimoDia(fecini);
                string fecinistr = fecini.ToString("yyyy'-'MM'-'dd");
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");


                f.EjecutarConsultaSQLCli("SELECT vacacionesUsos.id,vacacionesUsos.rutTrabajador,vacacionesUsos.fechaInicio " +
                                            ",vacacionesUsos.fechaTermino,vacacionesUsos.anoInicio,vacacionesUsos.anoTermino,vacacionesUsos.tipoUso " +
                                            "FROM vacacionesUsos " +
                                            "WHERE vacacionesUsos.habilitado = 1 and vacacionesUsos.estadoUsos = 'Aprueba' "+
                                            " and vacacionesUsos.fechainicio <= '" + hasta+"' and vacacionesUsos.fechatermino >= '"+desde+"' ", BD_Cli);


                List<DetVacacionesCtaCte> opcionesList = new List<DetVacacionesCtaCte>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetVacacionesCtaCte()
                                    {
                                        ruttrabajador = dr["rutTrabajador"].ToString(),
                                        anoinicio = int.Parse(dr["anoInicio"].ToString()),
                                        anotermino = int.Parse(dr["anoTermino"].ToString()),
                                        tipouso = dr["rutTrabajador"].ToString(),
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
                        r.periodo = r.anoinicio.ToString("####") + "-" + r.anotermino.ToString("####");
                    }
                    resultado.result = 1;
                    resultado.mensaje = "Existen registros";
                }
                return resultado;
            }
            catch (Exception ex)
            {
                var Asunto = "Error en consulta de personal en vacaciones";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar personal en vacaciones", correos.destinatarioErrores);
                return resultado;
            }

        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetVacacionesCtaCte
    {
        public string ruttrabajador { get; set; }
        public string nombre { get; set; }
        public string periodo { get; set; }
        public string tipouso { get; set; }
        public string fechainicio { get; set; }
        public string fechatermino { get; set; }
        public int dias { get; set; }
        public int anoinicio { get; set; }
        public int anotermino { get; set; }
    }
}


