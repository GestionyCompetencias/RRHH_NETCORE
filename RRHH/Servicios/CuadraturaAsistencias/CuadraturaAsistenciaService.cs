using Microsoft.AspNetCore.Mvc;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.CuadraturaAsistencias
{
    /// <summary>
    /// Servicio para cuadrar la asistencia.
    /// Ejem. Oficina central.
    /// </summary>
    public interface ICuadraturaAsistenciasService
    {
        /// <summary>
        /// Genera lista de cuadratura de asistencia.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes a cuadar</param>
        /// <param name="anio">año a cuadrar</param>
        /// <param name="pago">pago a cuadrar</param>
        /// <returns>Lista de asistencias </returns>
        public List<ResCuadraturaAsistencias> ListarCuadraturaAsistenciasService(int idEmpresa, int mes, int anio, string pago);

    }

    public class CuadraturaAsistenciasService : ICuadraturaAsistenciasService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public CuadraturaAsistenciasService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<ResCuadraturaAsistencias> ListarCuadraturaAsistenciasService(int empresa,  int mes,int anio,string pago)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                DateTime fecini = new DateTime(anio, mes, 1);
                DateTime fecfin = f.UltimoDia(fecini);
                string fecinistr = fecini.ToString("yyyy'-'MM'-'dd");
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");


                f.EjecutarConsultaSQLCli("SELECT asistenciasinformadas.rutTrabajador,asistenciasinformadas.fechaAsistencia, asistenciasinformadas.dias"+
                                            ", asistenciasinformadas.horasExtras1, asistenciasinformadas.horasExtras2, asistenciasinformadas.horasExtras3" +
                                            ", asistenciasinformadas.diasColacion, asistenciasinformadas.horasColacion, asistenciasinformadas.diasMovilizacion " +
                                            "FROM asistenciasinformadas " +
                                            "WHERE asistenciasinformadas.habilitado = 1 " + 
                                            " and asistenciasinformadas.fechaAsistencia <= '"+fecfinstr+"' and asistenciasinformadas.fechaAsistencia >= '"+fecinistr+
                                            "'  ", BD_Cli);


                List<DetCuadraturaAsistencias> opcionesList = new List<DetCuadraturaAsistencias>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetCuadraturaAsistencias()
                                    {
                                        rutTrabajador = dr["rutTrabajador"].ToString(),
                                        dias = int.Parse(dr["dias"].ToString()),
                                        horasExtras1 = decimal.Parse(dr["horasExtras1"].ToString()),
                                        horasExtras2 = decimal.Parse(dr["horasExtras2"].ToString()),
                                        horasExtras3 = decimal.Parse(dr["horasExtras3"].ToString()),
                                        diasColacion = int.Parse(dr["diasColacion"].ToString()),
                                        horasColacion = decimal.Parse(dr["horasColacion"].ToString()),
                                        diasMovilizacion = int.Parse(dr["diasMovilizacion"].ToString()),
                                    }).ToList();
                    var asistencias = opcionesList.GroupBy(x=> x.rutTrabajador).Select(g=> new { rutTrabajador = g.Key }).ToList();
                    List<ResCuadraturaAsistencias> resumen = new List<ResCuadraturaAsistencias>();
                    int totdias = 0;
                    decimal totextra1 = 0;
                    decimal totextra2 = 0;
                    decimal totextra3 = 0;
                    decimal tothrscol = 0;
                    int totdiacol = 0;
                    int totdiamov = 0;
                    foreach (var r in asistencias)
                    {
                        var detalle = opcionesList.Where(x => x.rutTrabajador == r.rutTrabajador).ToList();

                        foreach (var d in detalle)
                        {
                            totdias += d.dias;
                            totextra1 += d.horasExtras1;
                            totextra2 += d.horasExtras2;
                            totextra3 += d.horasExtras2;
                            tothrscol += d.horasColacion;
                            totdiacol += d.diasColacion;
                            totdiamov += d.diasMovilizacion;
                        }
                        ResCuadraturaAsistencias res = new ResCuadraturaAsistencias();
                        res.rutTrabajador = r.rutTrabajador;
                        res.nombre = f.NombrePersona(empresa, r.rutTrabajador);
                        res.dias = totdias;
                        res.horasExtras1 = totextra1;
                        res.horasExtras2 = totextra2;
                        res.horasExtras3 = totextra3;
                        res.horasColacion = tothrscol;
                        res.diasColacion = totdiacol;
                        res.diasMovilizacion = totdiamov;
                        resumen.Add(res);
                        totdias = 0;
                        totextra1 = 0;
                        totextra2 = 0;
                        totextra3 = 0;
                        tothrscol = 0;
                        totdiacol = 0;
                        totdiamov = 0;
                    }
                    return resumen;
                }
                else
                {
                 return null;  
                }
            }
            catch (Exception ex)
            {
                var Asunto = "Error en cuadratura de asistencias";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar cuadrar asistencias", correos.destinatarioErrores);
                return null;
            }

        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetCuadraturaAsistencias
    {
        public string rutTrabajador { get; set; }
        public int dias { get; set; }
        public decimal horasExtras1 { get; set; }
        public decimal horasExtras2 { get; set; }
        public decimal horasExtras3 { get; set; }
        public int diasColacion { get; set; }
        public decimal horasColacion { get; set; }
        public int diasMovilizacion { get; set; }
    }
    public class ResCuadraturaAsistencias
    {
        public string rutTrabajador { get; set; }
        public string nombre { get; set; }
        public int dias { get; set; }
        public decimal horasExtras1 { get; set; }
        public decimal horasExtras2 { get; set; }
        public decimal horasExtras3 { get; set; }
        public int diasColacion { get; set; }
        public decimal horasColacion { get; set; }
        public int diasMovilizacion { get; set; }
    }
}

