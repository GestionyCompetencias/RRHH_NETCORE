using Microsoft.AspNetCore.Mvc;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.HaberesInformados
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IHaberesInformadosService
    {
        /// <summary>
        /// Genera lista de haber informados.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="haber">Haber informados que desea listar</param>
        /// <param name="mes">mes del haber informados que desea listar</param>
        /// <param name="anio">año del haber informados que desea listar</param>
        /// <param name="pago">pago del haber informados que desea listar</param>
        /// <returns>Lista de haberes informados</returns>
        public List<DetHaberesInformados> ListarHaberesInformadosService(int idEmpresa, int haber, int mes, int anio, string pago);

        /// <summary>
        /// Consulta por haber informado por id.
        /// </summary>
        /// <param name="id">ID de haber informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Muestra informacion de haber informado</returns>
        public List<HaberesInformadosBaseVM> ConsultaHaberInformadoIdService(int id, int idEmpresa);
        /// <summary>
        /// Creación de haber informado.
        /// </summary>
        /// <param name="opciones">Registro de haber informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns
        public Resultado CrearHaberInformadoService(DetHaberesInformados opciones, int idEmpresa);

        /// <summary>
        /// Edita haber informado.
        /// </summary>
        /// <param name="opciones">Registro de haber informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns>
        public Resultado EditaHaberInformadoService(DetHaberesInformados opciones, int idEmpresa);

        /// <summary>
        /// Inhabilitar haber informado.
        /// </summary>
        /// <param name="id">ID de haber informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita haber informado</returns>
        public Resultado InhabilitaHaberInformadoService(HaberesInformadosDeleteVM opciones, int idEmpresa);

        /// <summary>
        /// Carga lista de haberes.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de haberes</returns>
        public Resultado ComboHaberesService(int idEmpresa);

        /// <summary>
        /// Carga lista de trabajadores.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de trabajadores</returns>
        public Resultado ComboTrabajadoresService(int idEmpresa);

    }

    public class HaberesInformadosService : IHaberesInformadosService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public HaberesInformadosService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetHaberesInformados> ListarHaberesInformadosService(int empresa, int haber, int mes,int anio,string pago)
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


                f.EjecutarConsultaSQLCli("SELECT haberesinformados.id,haberesinformados.haber,haberesinformados.rutTrabajador,haberesinformados.correlativo " +
                                            ",haberesinformados.afecta,haberesinformados.pago, haberesinformados.tipoCalculo, haberesinformados.monto " +
                                            ",haberesinformados.dias,haberesinformados.fechaDesde, haberesinformados.fechaHasta, haberesinformados.fechaIngreso " +
                                            ",haberesinformados.pagina " +
                                            "FROM haberesinformados " +
                                            "WHERE haberesinformados.habilitado = 1 and haberesinformados.haber =" + haber + 
                                            " and haberesinformados.fechaDesde <= '"+fecfinstr+"' and haberesinformados.fechaHasta >= '"+fecinistr+
                                            "' and haberesinformados.pago = '"+pago+"' ", BD_Cli);


                List<DetHaberesInformados> opcionesList = new List<DetHaberesInformados>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetHaberesInformados()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        haber = int.Parse(dr["haber"].ToString()),
                                        rutTrabajador = dr["rutTrabajador"].ToString(),
                                        correlativo = int.Parse(dr["correlativo"].ToString()),
                                        afecta = dr["afecta"].ToString(),
                                        pago = dr["pago"].ToString(),
                                        tipoCalculo = dr["tipoCalculo"].ToString(),
                                        monto = dr["monto"].ToString(),
                                        dias = int.Parse(dr["dias"].ToString()),
                                        fechaDesde = dr["fechaDesde"].ToString(),
                                        fechaHasta = dr["fechaHasta"].ToString(),
                                        fechaIngreso = dr["fechaIngreso"].ToString(),
                                        pagina = int.Parse(dr["pagina"].ToString())
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        Decimal monto = Convert.ToDecimal(r.monto);
                        r.monto = monto.ToString("###,###,##0");
                        DateTime desde = Convert.ToDateTime(r.fechaDesde);
                        r.fechaDesde = desde.ToString("dd'-'MM'-'yyyy");
                        DateTime hasta = Convert.ToDateTime(r.fechaHasta);
                        r.fechaHasta = hasta.ToString("dd'-'MM'-'yyyy");
                        PersonasBaseVM pers = Generales.BuscaPersona(r.rutTrabajador, BD_Cli);
                        r.nombre = pers.Nombres + " " + pers.Apellidos;
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
                var Asunto = "Error al Consultar haber informados";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar haber informados", correos.destinatarioErrores);
                return null;
            }

        }
        public List<HaberesInformadosBaseVM> ConsultaHaberInformadoIdService(int id, int empresa)
        {

            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT haberesinformados.id,haberesinformados.haber,haberesinformados.rutTrabajador,haberesinformados.correlativo " +
                                            ",haberesinformados.afecta,haberesinformados.pago, haberesinformados.tipoCalculo, haberesinformados.monto " +
                                            ",haberesinformados.dias,haberesinformados.fechaDesde, haberesinformados.fechaHasta, haberesinformados.fechaIngreso " +
                                            ",haberesinformados.pagina " +
                                           "FROM haberesinformados " +
                                            "WHERE haberesinformados.habilitado = 1 and haberesinformados.id =" + id + " ", BD_Cli);


                List<HaberesInformadosBaseVM> opcionesList = new List<HaberesInformadosBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new HaberesInformadosBaseVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        haber = int.Parse(dr["haber"].ToString()),
                                        rutTrabajador = dr["rutTrabajador"].ToString(),
                                        correlativo = int.Parse(dr["correlativo"].ToString()),
                                        afecta = dr["afecta"].ToString(),
                                        pago = dr["pago"].ToString(),
                                        tipoCalculo = dr["tipoCalculo"].ToString(),
                                        monto = decimal.Parse(dr["monto"].ToString()),
                                        dias = int.Parse(dr["dias"].ToString()),
                                        fechaDesde = dr["fechaDesde"].ToString(),
                                        fechaHasta = dr["fechaHasta"].ToString(),
                                        fechaIngreso = dr["fechaIngreso"].ToString(),
                                        pagina = int.Parse(dr["pagina"].ToString())
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {

                        DateTime desde = Convert.ToDateTime(r.fechaDesde);
                        r.fechaDesde = desde.ToString("yyyy'-'MM'-'dd");
                        DateTime hasta = Convert.ToDateTime(r.fechaHasta);
                        r.fechaHasta = hasta.ToString("yyyy'-'MM'-'dd");
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
                var Asunto = "Error al Consultar haber informado";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar haber informado", correos.destinatarioErrores);
                return null;
            }
        }


        public Resultado CrearHaberInformadoService(DetHaberesInformados opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            try
            {
                if ( opciones.haber ==0)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar haber";
                    return resultado;
                }
                if (opciones.monto != null)
                    opciones.monto = opciones.monto.Replace(",", ".");
                else opciones.monto = "0";

                    DateTime hoy = DateTime.Now.Date;
                string hoystr = hoy.ToString("yyyy'-'MM'-'dd");

                string query2 = "insert into haberesinformados (haber,rutTrabajador,correlativo,afecta,pago,tipoCalculo,monto,dias,fechaDesde,fechaHasta,fechaIngreso,pagina,habilitado) " +
                "values " +
                "(" + opciones.haber + ",'" + opciones.rutTrabajador + "'," + opciones.correlativo + ",'" + opciones.afecta + "','" + opciones.pago +
                "', '" + opciones.tipoCalculo + "', '" + opciones.monto + "' , " + opciones.dias + " ,'" + opciones.fechaDesde + "','" + opciones.fechaHasta +
                "', '" + hoystr + "'," + opciones.pagina + " ,1) ";
                if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Haber Informado ingresada de manera exitosa";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se ingreso la información de haber informado";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al guardar haber informado";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar haber informado en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar haber informado" + eG.Message.ToString();
                return resultado;
            }

        }
        public Resultado EditaHaberInformadoService(DetHaberesInformados opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;



            try
            {
                if (opciones.haber == null || opciones.haber ==0)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar haber";
                    return resultado;
                }

                if (opciones.monto != null)
                    opciones.monto = opciones.monto.Replace(",", ".");
                else opciones.monto = "0";


                string query = "update haberesinformados set [haber]=" + opciones.haber + " ,[rutTrabajador]='" + opciones.rutTrabajador + "' ,[correlativo]=" + opciones.correlativo +
                               ",  [afecta]='" + opciones.afecta + "',[pago]='" + opciones.pago + "',[tipoCalculo]= '" + opciones.tipoCalculo +
                               "', [monto]='" + opciones.monto + "',[dias]=" + opciones.dias + ",[fechaDesde]= '" + opciones.fechaDesde +
                               "', [fechaHasta]= '" + opciones.fechaHasta + "', [pagina]= " + opciones.pagina + 
                               " where haberesinformados.id=" + opciones.id ;

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 2;
                    resultado.mensaje = "Haber Informado editado exitosamente.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se editó la información del haber informado.";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar haber informado";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar un haber informado en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar haber informado" + eG.Message.ToString();
                return resultado;
            }
        }

        public Resultado InhabilitaHaberInformadoService(HaberesInformadosDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            try
            {
                if (opciones.Id == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la informacion del haber informado que desea eliminar";
                    return resultado;
                }

                string query = "update haberesinformados set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Haber Informado eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó la información del haber informado.";
                }

                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al eliminar haber informado";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar haber informado en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una haber informado" + eG.Message.ToString();
                return resultado;
            }
        }
        public Resultado ComboHaberesService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            f.EjecutarConsultaSQLCli("SELECT haberes.haber,haberes.Descripcion " +
                                        "FROM haberes " +
                                        " where haberes.habilitado = 1 ", BD_Cli);

            List<HaberBaseVM> opcionesList = new List<HaberBaseVM>();
            if (f.Tabla.Rows.Count > 0)
            {

                opcionesList = (from DataRow dr in f.Tabla.Rows
                                select new HaberBaseVM()
                                {
                                    haber = int.Parse(dr["haber"].ToString()),
                                    descripcion = dr["Descripcion"].ToString(),
                                }).ToList();
            }
            var data = new List<Conceptos>();
            foreach (var h in opcionesList)
            {
                data.Add(new Conceptos() { Codigo = Convert.ToString(h.haber), Descripcion = h.descripcion });
            }
            resultado.result = 1;
            resultado.data = data;
            return resultado;
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
    public class DetHaberesInformados
    {
        public int id { get; set; }
        public int haber { get; set; }
        public string rutTrabajador { get; set; }
        public string nombre { get; set; }
        public int correlativo { get; set; }
        public string afecta { get; set; }
        public string pago { get; set; }
        public string tipoCalculo { get; set; }
        public string monto { get; set; }
        public int dias { get; set; }
        public string fechaDesde { get; set; }
        public string fechaHasta { get; set; }
        public string fechaIngreso { get; set; }
        public int pagina { get; set; }
        public int habilitado { get; set; }
    }
}

