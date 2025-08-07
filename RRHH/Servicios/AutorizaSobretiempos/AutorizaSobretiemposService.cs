using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace RRHH.Servicios.AutorizaSobretiempos
{
    /// <summary>
    /// Servicio para gestionar la autorización de sobretiempos.
    /// </summary>
    public interface IAutorizaSobretiemposService
    {
        /// <summary>
        /// Genera listado de solicitudes de sobretiempos.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="desde">desde fecha </param>
        /// <param name="hasta">hasta fecha</param>
        /// <returns>Lista sobretiempos</returns>
        public List<DetSobretiempos> ListarSobretiemposService(int idEmpresa,  string desde, string hasta);

        /// <summary>
        /// Autoriza sobretiempo.
        /// </summary>
        /// <param name="opciones">Registro de sobrtetiempo</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="usuario">Usuario logeado.</param>
        /// <param name="desde">Fecha inical del periodo</param>
        /// <param name="hasta">Fecha final del periodo</param>
        /// <returns>Sobretiempo autorizado</returns>
        public Resultado AutorizaSobretiempoService(string ids, int idEmpresa, string usuario,string desde, string hasta);


    }

    public class AutorizaSobretiemposService : IAutorizaSobretiemposService
    {
        private readonly IDatabaseManager _databaseManager;
		private static Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public AutorizaSobretiemposService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetSobretiempos> ListarSobretiemposService(int empresa,string desde,string hasta)
        {
            List<DetSobretiempos> detalle = new List<DetSobretiempos>();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                DateTime fecini = Convert.ToDateTime(desde);
                DateTime fecfin = Convert.ToDateTime(hasta);
                string fecinistr = fecini.ToString("yyyy'-'MM'-'dd");
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");


                f.EjecutarConsultaSQLCli("SELECT marcacion.RUT,marcacion.CHECKTIME,marcacion.CHECKTYPE " +
                                            "FROM MARCACION " +
                                            "WHERE marcacion.CHECKTIME <= '"+fecfinstr+"' and marcacion.CHECKTIME >= '"+fecinistr+
                                            "' Order By marcacion.CHECKTIME,marcacion.RUT", BD_Cli);


                List<MarcacionBaseVM> Lista = new List<MarcacionBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    Lista = (from DataRow dr in f.Tabla.Rows
                                    select new MarcacionBaseVM()
                                    {
                                        rutTrabajador = dr["RUT"].ToString(),
                                        marca = DateTime.Parse(dr["CHECKTIME"].ToString()),
                                        tipoMarca = dr["CHECKTYPE"].ToString()
                                    }).ToList();
                    var fech = Lista.GroupBy(x => x.marca.Date).Select(g => new { fecha = g.Key }).ToList();
                    foreach (var t in fech)
                    {
                        DateTime sig = t.fecha.AddDays(1);  
                        var trab = Lista.Where(x=> x.marca >= t.fecha && x.marca < sig).GroupBy(x => x.rutTrabajador).Select(g => new { rut = g.Key }).ToList();
                        foreach (var s in trab)
                        {
                              DateTime entrada = Lista.Where(x=> x.rutTrabajador == s.rut && x.marca >= t.fecha && x.marca < sig && x.tipoMarca =="I").Select(x=> x.marca).FirstOrDefault();
                              DateTime salida = Lista.Where(x => x.rutTrabajador == s.rut && x.marca >= t.fecha && x.marca < sig && x.tipoMarca == "O").Select(x=> x.marca).FirstOrDefault();
                              if (entrada != null && salida != null)
                              {
                                TimeSpan diferencia = salida - entrada;
                                if (diferencia.Minutes > 0)
                                {
                                    string fecha = t.fecha.ToString("yyyy'-'MM'-'dd");
                                    f.EjecutarConsultaSQLCli("SELECT horasExtras.rutTrabajador " +
                                                             "FROM horasExtras " +
                                                             "WHERE horasExtras.rutTrabajador = '" + s.rut + "' and horasExtras.fecha = '" + fecha + "' ", BD_Cli);
                                    if (f.Tabla.Rows.Count == 0)
                                    {
                                        DetSobretiempos sobr = new DetSobretiempos();
                                        sobr.ruttrabajador = s.rut;
                                        sobr.fecha = t.fecha.ToString("dd'-'MM'-'yyyy");
                                        string diasemana = t.fecha.DayOfWeek.ToString();
                                        sobr.diasem = f.SemanaLetras(diasemana);
                                        sobr.entrada = entrada.ToString("HH':'mm");
                                        sobr.salida = salida.ToString("HH':'mm");
                                        int horas = diferencia.Hours;
                                        int minut = diferencia.Minutes;
                                        sobr.horastrabajadas = horas.ToString("00") + ":" + minut.ToString("00");
                                        int minutos = (int)diferencia.TotalMinutes - 480;
                                        horas = minutos / 60;
                                        minut = minutos - horas * 60;
                                        sobr.horasextras = horas.ToString("00") + ":" + minut.ToString("00");
                                        detalle.Add(sobr);
                                    }
                                }
                               }
                        }
                    }
                    return detalle;
                }
                else
                {
                 return detalle;  
                }
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error al Consultar marcas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar marcas", correos.destinatarioErrores);
                return detalle;
            }

        }

        public Resultado AutorizaSobretiempoService(string ids,int empresa, string idusuario,string desde,string hasta)
        {
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            int id = Convert.ToInt32(idusuario);
            UsuarioVM usuario = Generales.BuscaUsuario(id);
  
            string[] idsSeparados = ids.Split("*");
            int nsel = idsSeparados.Length-1;


            Resultado resultado = new Resultado();
                resultado.result = 0;
                try
                {
                int ind;
                for (ind = 0; ind < nsel; ind++)
                {

                    string[] campos = idsSeparados[ind].Split(";");
                    string rut = campos[0];
                    string fec = campos[1];
                    string anio = fec.Substring(6,4);
                    string mes = fec.Substring(3, 2);
                    string dia = fec.Substring(0, 2);
                    string fecha = anio + "-" + mes + "-" + dia;

                    List<DetSobretiempos> sobre = ListarSobretiemposService(empresa, desde, hasta);
                    DetSobretiempos trab = sobre.Where(x => x.ruttrabajador == rut && x.fecha == fec).FirstOrDefault();
                    string horas = fecha + " " + trab.horasextras;
                    string query2 = "insert into horasExtras (rutTrabajador,fecha,horasExtras,habilitado) " +
                       "values " +
                       "('" + rut + "', '" + fecha + "', '" + horas + "' ,1)  ";
                    if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                    {
                        resultado.result = 1;
                        resultado.mensaje = "Horas extras autorizadas";
                    }
                }
            }
                catch (Exception eG)
                {
                    var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();
                    resultado.result = -1;
                    resultado.mensaje = "Fallo al intentar guardar asistencia" + eG.Message.ToString();
                }
                return resultado;
            }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetSobretiempos
    {
        public int id { get; set; }
        public string ruttrabajador { get; set; }
        public string fecha { get; set; }
        public string diasem { get; set; }
        public string entrada { get; set; }
        public string salida { get; set; }
        public string horastrabajadas { get; set; }
        public string horasextras { get; set; }
    }
}

