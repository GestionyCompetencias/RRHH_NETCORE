using Microsoft.AspNetCore.Mvc;
using Rollbar.DTOs;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.ArchivoDepositos
{
    /// <summary>
    /// Servicio para generar archivo de depositos.
    /// </summary>
    public interface IArchivoDepositosService
    {
        /// <summary>
        /// Genera archivo de ddepositos.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes de proceso</param>
        /// <param name="anio">año de proceso</param>
        /// <returns>Archivo de depositos</returns>
        public List<DetDeposito> ListarArchivoDepositosService(int idEmpresa, int mes, int anio);

    }

    public class ArchivoDepositosService : IArchivoDepositosService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();

		public ArchivoDepositosService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetDeposito> ListarArchivoDepositosService(int empresa,  int mes,int anio)
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


                f.EjecutarConsultaSQLCli("SELECT resultados.rutTrabajador,resultados.concepto,resultados.monto,resultados.cantidad " +
                                            "FROM resultados " +
                                            "WHERE resultados.habilitado = 1 " + 
                                            " and resultados.fechaPago <= '"+fecfinstr+"' and resultados.fechaPago >= '"+fecinistr+
                                            "' and resultados.pago = 'L' ", BD_Cli);


                List<DetResultado> opcionesList = new List<DetResultado>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetResultado()
                                    {
                                        rutTrabajador = dr["rutTrabajador"].ToString(),
                                        concepto = int.Parse(dr["concepto"].ToString()),
                                        monto = decimal.Parse(dr["monto"].ToString()),
                                        cantidad = decimal.Parse(dr["cantidad"].ToString()),
                                    }).ToList();
                    var trabajadores = opcionesList.GroupBy(x=> x.rutTrabajador).Select(g=> new { rutTrabajador = g.Key }).ToList();
                    List<DetDeposito> libro = new List<DetDeposito>();
                    foreach (var t in trabajadores)
                    {
                        string rut = t.rutTrabajador;
                        var detalle = opcionesList.Where(x => x.rutTrabajador == rut).ToList();
                        DetDeposito linea = new DetDeposito();

                        foreach (var r in detalle)
                        {
                            linea.rut = rut;
                            linea.nombre = f.NombrePersona(empresa, rut);
                            linea.tipocuenta = 0;
                            linea.numerocuenta = "";
                            linea.rut = linea.rut.Substring(0, linea.rut.Length - 1) + "-" + linea.rut.Substring(linea.rut.Length - 1, 1);
                            if (r.concepto == 2300) linea.monto = (int)r.monto;
                        }
                        libro.Add(linea);
                    }
                    return libro;
                }
                else
                {
                 return null;  
                }
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error de depositos";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al generar archivo de depositos", correos.destinatarioErrores);
                return null;
            }

        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetDeposito
    {

        public string rut { get; set; }
        public string nombre { get; set; }
        public int banco { get; set; }
        public int tipocuenta { get; set; }
        public string numerocuenta { get; set; }
        public int monto { get; set; }
    }
}

