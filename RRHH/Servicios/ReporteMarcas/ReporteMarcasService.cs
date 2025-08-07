using Microsoft.AspNetCore.Mvc;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.ReporteMarcas
{
    /// <summary>
    /// Servicio para gestionar las marcas del trabajador.
    /// </summary>
    public interface IReporteMarcasService
    {
        /// <summary>
        /// Genera listado de marcas del trabajador.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="desde">desde fecha </param>
        /// <param name="hasta">hasta fecha</param>
        /// <returns>Lista marcas del periodo</returns>
        public List<DetMarcas> ListarMarcasTrabajadorService(int idEmpresa,  string desde, string hasta,string rut);


        /// <summary>
        /// Muestra imagen.
        /// </summary>
        /// <param name="rut">Rut trabajador</param>
        /// <param name="fecha">Fecha</param>
        /// <param name="tipo">tipo de marca</param>
        /// <returns>Imagen</returns>
        public FileStreamResult MuestraImagenService(string rut,DateTime fecha,string tipo);


        /// <summary>
        /// Carga lista de trabajadores.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de trabajadores</returns>
        public Resultado ComboTrabajadoresService(int idEmpresa);
 

    }

    public class ReporteMarcasService : IReporteMarcasService
    {
        private readonly IDatabaseManager _databaseManager;
		private static Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public ReporteMarcasService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetMarcas> ListarMarcasTrabajadorService(int empresa,string desde,string hasta, string rut)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                DateTime fecini = Convert.ToDateTime(desde);
                DateTime fecfin = Convert.ToDateTime(hasta);
                string fecinistr = fecini.ToString("yyyy'-'MM'-'dd");
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");
                var pers = Generales.BuscaPersona(rut, BD_Cli);
                string nombre = pers.Apellidos + " " + pers.Nombres;
                string ssn = rut.Substring(0, rut.Length - 1) +"-"+rut.Substring(rut.Length-1,1);

                f.EjecutarConsultaSQLCli("SELECT CHECKINOUT.CHECKTIME,CHECKINOUT.CHECKTYPE,USERINFO.SSN,USERINFO.USERID " +
                                            "FROM CHECKINOUT " +
                                            "inner join USERINFO on CHECKINOUT.USERID = USERINFO.USERID " +
                                        "WHERE CHECKINOUT.CHECKTIME <= '" + fecfinstr+ "' and CHECKINOUT.CHECKTIME >= '" + fecinistr+
                                            "' and USERINFO.SSN = '" + ssn +"' ", "gycsolcl_Relojcontrol");


                List<MarcacionBaseVM> Lista = new List<MarcacionBaseVM>();
                    Lista = (from DataRow dr in f.Tabla.Rows
                                    select new MarcacionBaseVM()
                                    {
                                        rutTrabajador = dr["SSN"].ToString(),
                                        marca = DateTime.Parse(dr["CHECKTIME"].ToString()),
                                        tipoMarca = dr["CHECKTYPE"].ToString(),
                                        diaSemana = int.Parse(dr["USERID"].ToString()),
                                    }).ToList();
                    List<DetMarcas> detalle = new List<DetMarcas>();
                foreach(var m in Lista)
                {
                    DetMarcas det = new DetMarcas();
                    det.ruttrabajador = m.rutTrabajador;
                    det.nombre = nombre;
                    det.fecha = m.marca.ToString("dd'-'MM'-'yyyy");
                    det.checktime = m.marca.ToString("HH':'mm");
                    det.checktype = "SALIDA";
                    det.userid = m.diaSemana.ToString();
                    if (m.tipoMarca == "I")
                    {
                        det.checktype = "ENTRADA";
                    }
                    det.coordenada = BuscaCoordenadas(m.rutTrabajador, m.marca, m.tipoMarca);
                    detalle.Add(det);
                }
                return detalle;
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error al Consultar marcas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar marcas", correos.destinatarioErrores);
                return null;
            }

        }

        public string BuscaCoordenadas(string rut, DateTime fecha, string tipomarca)
        {
            DateTime fecini = fecha;
            DateTime fecfin = fecha.AddDays(1);
            string fecinistr = fecini.ToString("yyyy'-'MM'-'dd");
            string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");
            string tipo = "I";
            if (tipomarca == "SALIDA") tipo = "O";
            rut = rut.Replace("-", "");
            f.EjecutarConsultaSQLCli("SELECT ASISTENCIA.G_LATITUD ,ASISTENCIA.G_LONGITUD,ASISTENCIA.IMAGEN " +
                                     "FROM [gycsolcl_gestionAdmin].ASISTENCIA " +
                                             "inner join [gycsolcl_gestionAdmin].TRABAJADOR_AUTORIZADO on ASISTENCIA.TRABAJADOR = TRABAJADOR_AUTORIZADO.USER_ID " +
                                    "WHERE ASISTENCIA.MARCACION <= '" + fecfinstr + "' and ASISTENCIA.MARCACION >= '" + fecinistr +
                                       "' and TRABAJADOR_AUTORIZADO.TRABAJADOR = '" + rut + "' and ASISTENCIA.TIPO_MARCA = '" + tipo + "' ", "gycsolcl_dbAndroid");


            var Lista = (from DataRow dr in f.Tabla.Rows
                     select new
                     {
                         longitud = dr["G_LONGITUD"].ToString(),
                         latitud = dr["G_LATITUD"].ToString()
                     }).ToList();
            var marca = Lista.FirstOrDefault();
            string coor = null;
            if (marca != null) 
            {
                string lat = Convert.ToString(marca.latitud);
                string lon = Convert.ToString(marca.longitud);
                coor = lat.Replace(",", ".") + ',' + lon.Replace(",", ".");
            }
            return coor;
        }

        public FileStreamResult MuestraImagenService(string rut, DateTime fecha, string tipomarca)
        {
            DateTime fecini = fecha;
            DateTime fecfin = fecha.AddDays(1);
            string fecinistr = fecini.ToString("yyyy'-'MM'-'dd");
            string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");
            string tipo = "I";
            if (tipomarca == "SALIDA") tipo = "O";
            rut = rut.Replace("-", "");
            f.EjecutarConsultaSQLCli("SELECT ASISTENCIA.G_LATITUD ,ASISTENCIA.G_LONGITUD,ASISTENCIA.IMAGEN " +
                                     "FROM [gycsolcl_gestionAdmin].ASISTENCIA " +
                                             "inner join [gycsolcl_gestionAdmin].TRABAJADOR_AUTORIZADO on ASISTENCIA.TRABAJADOR = TRABAJADOR_AUTORIZADO.USER_ID " +
                                    "WHERE ASISTENCIA.MARCACION <= '" + fecfinstr + "' and ASISTENCIA.MARCACION >= '" + fecinistr +
                                       "' and TRABAJADOR_AUTORIZADO.TRABAJADOR = '" + rut + "' and ASISTENCIA.TIPO_MARCA = '" + tipo + "' ", "gycsolcl_dbAndroid");

            DataRow dr = f.Tabla.Rows[0];
            byte[] datos = new byte[0];
            datos = (byte[])dr["IMAGEN"];
            System.IO.MemoryStream ms = new System.IO.MemoryStream(datos);
            return new FileStreamResult(ms, "image/jpg");
        }

        public Resultado ComboTrabajadoresService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            f.EjecutarConsultaSQLCli("SELECT personas.rut,personas.nombres,personas.apellidos " +
                                        "FROM personas " , BD_Cli);

            List<PersonasBaseVM> opcionesList = new List<PersonasBaseVM>();
            if (f.Tabla.Rows.Count > 0)
            {

                opcionesList = (from DataRow dr in f.Tabla.Rows
                                select new PersonasBaseVM()
                                {
                                    Rut = dr["Rut"].ToString(),
                                    Nombres = dr["Nombres"].ToString(),
                                    Apellidos = dr["Apellidos"].ToString()
                                }).ToList();
            }
            var data = new List<Conceptos>();
            foreach (var h in opcionesList)
            {
                data.Add(new Conceptos() { Codigo = h.Rut, Descripcion = h.Nombres+" "+h.Apellidos });
            }
            resultado.result = 1;
            resultado.data = data;
            return resultado;
        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetMarcas
    {
        public int id { get; set; }
        public string userid { get; set; }
        public string ruttrabajador { get; set; }
        public string nombre { get; set; }
        public string fecha { get; set; }
        public string checktime { get; set; }
        public string checktype { get; set; }
        public string coordenada { get; set; }

    }
}


