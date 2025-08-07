using Microsoft.AspNetCore.Mvc;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.CierraProceso
{
    /// <summary>
    /// Servicio que cierra proceso de pago.
    /// </summary>
    public interface ICierraProcesoService
    {
        /// <summary>
        /// Realiza el cierre de un proceso de pago.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes de proceso</param>
        /// <param name="anio">año de proceso</param>
        /// <param name="pago">pago ha procesarr</param>
        /// <returns>Bool de termino</returns>
        public bool ProcesaCierraProcesoService(int idEmpresa, int mes, int anio, string pago);

    }

    public class CierraProcesoService : ICierraProcesoService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public CierraProcesoService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public bool ProcesaCierraProcesoService(int empresa,  int mes,int anio,string pago)
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


                f.EjecutarConsultaSQLCli("SELECT procesos.id,procesos.Estado " +
                                            "FROM procesos " +
                                            "WHERE procesos.habilitado = 1 " + 
                                            " and proceso.fechaProceso <= '"+fecfinstr+"' and procesos.fechaProcesos >= '"+fecinistr+
                                            "' and procesos.pago = '"+pago+"' and procesos.estado != 'Cerrado' ", BD_Cli);


                List<DetCierraProceso> opcionesList = new List<DetCierraProceso>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetCierraProceso()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        estado = dr["estado"].ToString()
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        string query = "update procesos set estado='Cerrado' where id=" + r.id + "  ! ";

                    }
                    return true;
                }
                else
                {
                 return false;  
                }
            }
            catch (Exception ex)
            {
                var Asunto = "Error en cuadratura de habres";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar cusdrar haberes", correos.destinatarioErrores);
                return false;
            }

        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetCierraProceso
    {
        public int id { get; set; }
        public string estado { get; set; }
    }

}

