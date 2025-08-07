using Microsoft.AspNetCore.Mvc;
using Rollbar.DTOs;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.AsistenciasInformadas
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IAsistenciasInformadasService
    {
        /// <summary>
        /// Genera listado de asistencia informados.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes de proceso</param>
        /// <param name="anio">año de proceso</param>
        /// <returns>Lista asistencia informados</returns>
        public List<DetAsistenciasInformadas> ListarAsistenciasInformadasService(int idEmpresa,  int mes, int anio);

        /// <summary>
        /// Consulta asistencia informado por id.
        /// </summary>
        /// <param name="id">ID de haber informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Muestra informacion de asistencia</returns>
        public List<AsistenciasInformadasBaseVM> ConsultaAsistenciaInformadaIdService(int id, int idEmpresa);

        /// <summary>
        /// Creación de asistencia informado.
        /// </summary>
        /// <param name="opciones">Registro deasistencia informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns
        public Resultado CrearAsistenciaInformadaService(DetAsistenciasInformadas opciones, int idEmpresa);

        /// <summary>
        /// Edita asistencia informado.
        /// </summary>
        /// <param name="opciones">Registro de asistencia informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns>
        public Resultado EditaAsistenciaInformadaService(DetAsistenciasInformadas opciones, int idEmpresa);

        /// <summary>
        /// Inhabilitar asistencia.
        /// </summary>
        /// <param name="id">ID de asistencia informada</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita asistencia informada</returns>
        public Resultado InhabilitaAsistenciaInformadaService(AsistenciasInformadasDeleteVM opciones, int idEmpresa);


        /// <summary>
        /// Carga lista de trabajadores.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de trabajadores</returns>
        public Resultado ComboTrabajadoresService(int idEmpresa);
 
        /// <summary>
        /// Carga lista de inasistencias.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de inasistencias</returns>
        public Resultado ComboInasistenciasService(int idEmpresa);

    }

    public class AsistenciasInformadasService : IAsistenciasInformadasService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public AsistenciasInformadasService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetAsistenciasInformadas> ListarAsistenciasInformadasService(int empresa,int mes,int anio)
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


                f.EjecutarConsultaSQLCli("SELECT asistenciasinformadas.id,asistenciasinformadas.rutTrabajador,asistenciasinformadas.fechaAsistencia,asistenciasinformadas.codigoInasis " +
                                            ",asistenciasinformadas.dias,asistenciasinformadas.horasExtras1, asistenciasinformadas.horasExtras2, asistenciasinformadas.horasExtras3 " +
                                            ",asistenciasinformadas.diasColacion,asistenciasinformadas.horasColacion, asistenciasinformadas.diasMovilizacion " +
                                            "FROM asistenciasInformadas " +
                                            "WHERE asistenciasinformadas.habilitado = 1 " + 
                                            " and asistenciasinformadas.fechaAsistencia <= '"+fecfinstr+"' and asistenciasinformadas.fechaAsistencia >= '"+fecinistr+"' ", BD_Cli);


                List<DetAsistenciasInformadas> opcionesList = new List<DetAsistenciasInformadas>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetAsistenciasInformadas()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        rutTrabajador = dr["rutTrabajador"].ToString(),
                                        fechaAsistencia = dr["fechaasistencia"].ToString(),
                                        codigoInasis = dr["codigoInasis"].ToString(),
                                        dias = int.Parse(dr["dias"].ToString()),
                                        horasExtras1 = dr["horasExtras1"].ToString(),
                                        horasExtras2 = dr["horasExtras2"].ToString(),
                                        horasExtras3 = dr["horasExtras3"].ToString(),
                                        diasColacion = int.Parse(dr["diasColacion"].ToString()),
                                        horasColacion = dr["horasColacion"].ToString(),
                                        diasMovilizacion = int.Parse(dr["diasMovilizacion"].ToString()),
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {

                        r.inasistencia = BuscaInasistencia(empresa, r.codigoInasis);
                        DateTime asistencia = Convert.ToDateTime(r.fechaAsistencia);
                        r.fechaAsistencia = asistencia.ToString("dd'-'MM'-'yyyy");
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
            catch (System.Exception ex)
            {
                var Asunto = "Error al Consultar asistencias informadas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar asistencias informadas", correos.destinatarioErrores);
                return null;
            }

        }
        public List<AsistenciasInformadasBaseVM> ConsultaAsistenciaInformadaIdService(int id, int empresa)
        {

            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;


                f.EjecutarConsultaSQLCli("SELECT asistenciasinformadas.id,asistenciasinformadas.rutTrabajador,asistenciasinformadas.fechaAsistencia,asistenciasinformadas.codigoInasis " +
                                            ",asistenciasinformadas.dias,asistenciasinformadas.horasExtras1, asistenciasinformadas.horasExtras2, asistenciasinformadas.horasExtras3 " +
                                            ",asistenciasinformadas.diasColacion,asistenciasinformadas.horasColacion, asistenciasinformadas.diasMovilizacion " +
                                            "FROM asistenciasInformadas " +
                                            "WHERE asistenciasinformadas.habilitado = 1  and asistenciasinformadas.id =" +id , BD_Cli);


                List<AsistenciasInformadasBaseVM> opcionesList = new List<AsistenciasInformadasBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new AsistenciasInformadasBaseVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        rutTrabajador = dr["rutTrabajador"].ToString(),
                                        fechaAsistencia = dr["fechaasistencia"].ToString(),
                                        codigoInasis = dr["codigoInasis"].ToString(),
                                        horasExtras1 = decimal.Parse(dr["horasExtras1"].ToString()),
                                        horasExtras2 = decimal.Parse(dr["horasExtras2"].ToString()),
                                        horasExtras3 = decimal.Parse(dr["horasExtras3"].ToString()),
                                        diasColacion = int.Parse(dr["diasColacion"].ToString()),
                                        horasColacion = decimal.Parse(dr["horascolacion"].ToString()),
                                        diasMovilizacion = int.Parse(dr["diasMovilizacion"].ToString()),
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {

                        DateTime asistencia = Convert.ToDateTime(r.fechaAsistencia);
                        r.fechaAsistencia = asistencia.ToString("yyyy'-'MM'-'dd");
                    }
                    return opcionesList;
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error al Consultar asistencia informada";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultarasistencia informada", correos.destinatarioErrores);
                return null;
            }
        }


        public Resultado CrearAsistenciaInformadaService(DetAsistenciasInformadas opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            try
            {
                if (opciones.rutTrabajador == null )
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar trabajador";
                    return resultado;
                }
                if (opciones.horasExtras1 != null)
                    opciones.horasExtras1 = opciones.horasExtras1.Replace(",", ".");
                else opciones.horasExtras1 = "0";
                if (opciones.horasExtras2 != null)
                    opciones.horasExtras2 = opciones.horasExtras2.Replace(",", ".");
                else opciones.horasExtras2 = "0";
                if (opciones.horasExtras3 != null)
                    opciones.horasExtras3 = opciones.horasExtras3.Replace(",", ".");
                else opciones.horasExtras3 = "0";
                if (opciones.horasColacion != null)
                    opciones.horasColacion = opciones.horasColacion.Replace(",", ".");
                else opciones.horasColacion = "0";

                string query2 = "insert into asistenciasinformadas (rutTrabajador,fechaAsistencia,codigoInasis,dias,horasExtras1,horasExtras2,horasExtras3"+
                                ", diasColacion,horasColacion,diasMovilizacion,habilitado) " +
                "values " +
                "('" +  opciones.rutTrabajador + "','" + opciones.fechaAsistencia + "','" + opciones.codigoInasis + "'," + opciones.dias +
                ", '" + opciones.horasExtras1 + "', '" + opciones.horasExtras2 + "' , '" + opciones.horasExtras3 + "' ," + opciones.diasColacion + ",'" + opciones.horasColacion +
                "', "+ opciones.diasMovilizacion + " ,1) ";
                if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Asistencia informada ingresada de manera exitosa";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se ingreso la información de asistencia informada";
                }
                return resultado;
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error al guardar asistencia informado";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar asistencia en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar asistencia informada" + ex.Message.ToString();
                return resultado;
            }

        }
        public Resultado EditaAsistenciaInformadaService(DetAsistenciasInformadas opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;



            try
            {
                if (opciones.rutTrabajador == null )
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar trabajador";
                    return resultado;
                }
                if (opciones.horasExtras1 != null)
                    opciones.horasExtras1 = opciones.horasExtras1.Replace(",", ".");
                else opciones.horasExtras1 = "0";
                if (opciones.horasExtras2 != null)
                    opciones.horasExtras2 = opciones.horasExtras2.Replace(",", ".");
                else opciones.horasExtras2 = "0";
                if (opciones.horasExtras3 != null)
                    opciones.horasExtras3 = opciones.horasExtras3.Replace(",", ".");
                else opciones.horasExtras3 = "0";
                if (opciones.horasColacion != null)
                    opciones.horasColacion = opciones.horasColacion.Replace(",", ".");
                else opciones.horasColacion = "0";

                string query = "update asistenciasinformadas set [rutTrabajador]='" + opciones.rutTrabajador + "' ,[fechaAsistencia]='" + opciones.fechaAsistencia +
                               "',  [codigoInases]='" + opciones.codigoInasis + "',[dias]='" + opciones.dias + "',[horasExtras1]= '" + opciones.horasExtras1 +
                               "', [horasExtras2]='" + opciones.horasExtras2 + "',[horasExtras3]=" + opciones.horasExtras3 + ",[diasColacion]= '" + opciones.diasColacion +
                               "', [horasColacion]= '" + opciones.horasColacion + "', [diasMovilizacion]= " + opciones.diasMovilizacion + 
                               " where asistenciasinformadas.id=" + opciones.id ;

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 2;
                    resultado.mensaje = "Asistencia Informada editada exitosamente.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se editó la información de asistencia.";
                }
                return resultado;
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error al Editar Asistencia";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar un asistencia en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar una asistencia" + ex.Message.ToString();
                return resultado;
            }
        }

        public Resultado InhabilitaAsistenciaInformadaService(AsistenciasInformadasDeleteVM opciones, int empresa)
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
                    resultado.mensaje = "Debe indicar la informacion de la asistencia que desea eliminar";
                    return resultado;
                }

                string query = "update asistenciasinformadas set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Asistencia eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó la información de asistencia.";
                }

                return resultado;
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error al eliminar asistencia";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminarasistencia en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una asistencia" + ex.Message.ToString();
                return resultado;
            }
        }
        public Resultado ComboInasistenciasService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion" ;

            f.EjecutarConsultaSQLCli("SELECT parametrosGenerales.id,parametrosGenerales.tabla,parametrosGenerales.codigo,parametrosGenerales.descripcion, " +
                                        " parametrosGenerales.valor,parametrosGenerales.fecha, " +
                                        "parametrosGenerales.inicio, parametrosGenerales.termino " +
                                        "FROM parametrosGenerales " +
                                        " where parametrosGenerales.habilitado = 1 and parametrosGenerales.tabla = 'AUSENCIAS' ", BD_Cli);

            List<ParametrosBaseVM> opcionesList = new List<ParametrosBaseVM>();
            if (f.Tabla.Rows.Count > 0)
            {

                opcionesList = (from DataRow dr in f.Tabla.Rows
                                select new ParametrosBaseVM()
                                {
                                    Id = int.Parse(dr["id"].ToString()),
                                    Tabla = dr["tabla"].ToString(),
                                    Codigo = dr["codigo"].ToString(),
                                    Descripcion = dr["Descripcion"].ToString(),
                                    Valor = dr["valor"].ToString(),
                                    Fecha = dr["fecha"].ToString(),
                                    Inicio = dr["inicio"].ToString(),
                                    Termino = dr["termino"].ToString()
                                }).ToList();
            }
            var data = new List<Conceptos>();
            foreach (var h in opcionesList)
            {
                data.Add(new Conceptos() { Codigo = h.Codigo.Trim(), Descripcion = h.Descripcion });
            }
            resultado.result = 1;
            resultado.data = data;
            return resultado;
        }
        public string BuscaInasistencia(int empresa, string codigo)
        {

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion";
            string salida = "No existe";
            f.EjecutarConsultaSQLCli("SELECT parametrosGenerales.id,parametrosGenerales.tabla,parametrosGenerales.codigo,parametrosGenerales.descripcion, " +
                                        " parametrosGenerales.valor,parametrosGenerales.fecha, " +
                                        "parametrosGenerales.inicio, parametrosGenerales.termino " +
                                        "FROM parametrosGenerales " +
                                        " where parametrosGenerales.codigo= '"+codigo+"' ", BD_Cli);

            List<ParametrosBaseVM> opcionesList = new List<ParametrosBaseVM>();
            if (f.Tabla.Rows.Count > 0)
            {

                opcionesList = (from DataRow dr in f.Tabla.Rows
                                select new ParametrosBaseVM()
                                {
                                    Id = int.Parse(dr["id"].ToString()),
                                    Tabla = dr["tabla"].ToString(),
                                    Codigo = dr["codigo"].ToString(),
                                    Descripcion = dr["Descripcion"].ToString(),
                                    Valor = dr["valor"].ToString(),
                                    Fecha = dr["fecha"].ToString(),
                                    Inicio = dr["inicio"].ToString(),
                                    Termino = dr["termino"].ToString()
                                }).ToList();
            }
            foreach (var h in opcionesList)
            {
                salida = h.Descripcion;
            }
            return salida;
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
    public class DetAsistenciasInformadas
    {
        public int id { get; set; }
        public string rutTrabajador { get; set; }
        public string nombre { get; set; }
        public string fechaAsistencia  { get; set; }
        public string codigoInasis { get; set; }
        public string inasistencia { get; set; }
        public int dias { get; set; }
        public string horasExtras1 { get; set; }
        public string horasExtras2 { get; set; }
        public string horasExtras3 { get; set; }
        public int diasColacion { get; set; }
        public string horasColacion { get; set; }
        public int diasMovilizacion { get; set; }
        public int habilitado { get; set; }
    }
}

