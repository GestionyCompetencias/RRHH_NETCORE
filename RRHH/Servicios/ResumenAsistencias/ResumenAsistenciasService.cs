using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.ResumenAsistencias
{
    /// <summary>
    /// Servicio para generar el libro de remuenraciones.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IResumenAsistenciasService
    {
        /// <summary>
        /// Genera libro de remuneraciones.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes del haber informados que desea listar</param>
        /// <param name="anio">año del haber informados que desea listar</param>
        /// <returns>Lista de haberes informados</returns>
        public List<DetAsistencias> ListarResumenAsistenciasService(int idEmpresa, int mes, int anio);

    }

    public class ResumenAsistenciasService : IResumenAsistenciasService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public ResumenAsistenciasService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetAsistencias> ListarResumenAsistenciasService(int empresa,  int mes,int anio)
        {
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                DateTime fecini = new DateTime(anio, mes, 1);
                DateTime fecfin = f.UltimoDia(fecini);
                string fecinistr = fecini.ToString("yyyy'-'MM'-'dd");
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");


                f.EjecutarConsultaSQLCli("SELECT MARCACION.RUT,MARCACION.CHECKTIME,MARCACION.CHECKTYPE,MARCACION.MODIFICADA " +
                                            "FROM MARCACION " +
                                            "WHERE MARCACION.CHECKTIME <= '"+fecfinstr+"' and MARCACION.CHECKTIME >= '"+fecinistr+"' ", BD_Cli);


                List<Detalle> opcionesList = new List<Detalle>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new Detalle()
                                    {
                                        rutTrabajador = dr["RUT"].ToString(),
                                        fecha = DateTime.Parse(dr["CHECKTIME"].ToString()),
                                        tipoasistencia = dr["CHECKTYPE"].ToString()
                                    }).ToList();
                    List<DetAsistencias> lista = new List<DetAsistencias>();
                    var trabajadores = opcionesList.GroupBy(x => x.rutTrabajador).Select(g => new { rutTrabajador = g.Key }).ToList();
                    foreach (var t in trabajadores)
                    {
                        string rut = t.rutTrabajador;
                        var detalle = opcionesList.Where(x => x.rutTrabajador == rut).GroupBy(x => x.fecha.Date).Select(g => new { fecha = g.Key }).ToList();
                        DetAsistencias linea = new DetAsistencias();
                        int totpresente = 0;
                        int totdescanso = 0;
                        int totfalla = 0;
                        int totlicencia = 0;
                        int totpermisosg = 0;
                        int totpermiso = 0;
                        int totvacacion = 0;
                        int tototro = 0;
                        int tottotal = 0;
                        // Revisa la asitencia
                        foreach (var d in detalle)
                        {
                            totpresente++;
                            tottotal++;
                        }
                        // Revisa licencias
                        // Revisa descansos
                        // Revisa permisos
                        // Revisa vacaciones
                        linea.rut = t.rutTrabajador.ToString();
                        PersonasBaseVM pers = Generales.BuscaPersona(rut, BD_Cli);
                        if(pers != null)
                        {
                            linea.nombre = pers.Apellidos + " " + pers.Nombres;
                            linea.presente = totpresente;
                            linea.descanso = totdescanso;
                            linea.falla = totfalla;
                            linea.licencia = totlicencia;
                            linea.permisosg = totpermisosg;
                            linea.permiso = totpermiso;
                            linea.vacacion = totvacacion;
                            linea.otro = tototro;
                            linea.total = tottotal;
                            lista.Add(linea);
                        }
                    }
                    return lista;
                }
                else
                {
                 return null;  
                }
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error en libro de remuneraciones";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al generar el libro de remuneraciones", correos.destinatarioErrores);
                return null;
            }

        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetAsistencias
    {

        public string rut { get; set; }
        public string nombre { get; set; }
        public int presente { get; set; }
        public int descanso { get; set; }
        public int falla { get; set; }
        public int licencia { get; set; }
        public int permisosg { get; set; }
        public int permiso { get; set; }
        public int vacacion { get; set; }
        public int otro { get; set; }
        public int total { get; set; }
    }
    public class Detalle
    {
        public string rutTrabajador { get; set; }
        public DateTime fecha { get; set; }
        public string tipoasistencia { get; set; }
    }

}

