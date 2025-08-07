using Microsoft.Extensions.Hosting;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.Jornadas
{
    /// <summary>
    /// Servicio para generar y operar con las jornadas de una empresa.
    /// Ejem. Administrativo .
    /// </summary>
    public interface IJornadasService
    {
        /// <summary>
        /// Genera lista de jornadas.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de jornadas</returns>
        public List<DetJornadas> ListarJornadasService(int idEmpresa);

        /// <summary>
        /// Consulta una jornada.
        /// </summary>
        /// <param name="id">ID de la jornadal</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Muestra informacion de jornada</returns>
        public List<JornadasBaseVM> ConsultaJornadaIdService(int id, int idEmpresa);
        /// <summary>
        /// Creación de jornada.
        /// </summary>
        /// <param name="opciones">Registro de jornada</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns
        public Resultado CrearJornadaService(JornadasBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Edita jornada.
        /// </summary>
        /// <param name="opciones">Registro de jornada</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns>
        public Resultado EditaJornadaService(JornadasBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Inhabilitar jornada.
        /// </summary>
        /// <param name="id">ID de jornada</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita jornada</returns>
        public Resultado InhabilitaJornadaService(JornadasDeleteVM opciones, int idEmpresa);

    }

    public class JornadasService : IJornadasService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public JornadasService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetJornadas> ListarJornadasService(int empresa)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                    string RutEmpresa = f.obtenerRUT(empresa);
                    var BD_Cli = "remuneracion_" + RutEmpresa;

                    f.EjecutarConsultaSQLCli("SELECT jornadas.id,jornadas.codigo,jornadas.descripcion,jornadas.diasTrabajo,jornadas.diasDescanso,jornadas.numeroCiclos, " +
                                                "jornadas.horasSemanales, jornadas.fechaCreacion, jornadas.resolucion, jornadas.fechaResolucion  " +
                                                "FROM jornadas " +
                                                "WHERE jornadas.habilitado = 1 ", BD_Cli);


                    List<DetJornadas> opcionesList = new List<DetJornadas>();
                    if (f.Tabla.Rows.Count > 0)
                    {

                        opcionesList = (from DataRow dr in f.Tabla.Rows
                                        select new DetJornadas()
                                        {
                                            id = int.Parse(dr["id"].ToString()),
                                            codigo = dr["codigo"].ToString(),
                                            descripcion = dr["descripcion"].ToString(),
                                            diasTrabajo = int.Parse(dr["diasTrabajo"].ToString()),
                                            diasDescanso = int.Parse(dr["diasDescanso"].ToString()),
                                            numeroCiclos = int.Parse(dr["numeroCiclos"].ToString()),
                                            horasSemanales = int.Parse(dr["horasSemanales"].ToString()),
                                            fechaCreacion = dr["fechaCreacion"].ToString(),
                                            resolucion = dr["resolucion"].ToString(),
                                            fechaResolucion = dr["fechaResolucion"].ToString()
                                        }).ToList();
                        foreach (var r in opcionesList)
                        {

                            DateTime creacion = Convert.ToDateTime(r.fechaCreacion);
                            r.fechaCreacion = creacion.ToString("dd'-'MM'-'yyyy");
                            DateTime resolucion = Convert.ToDateTime(r.fechaResolucion);
                            r.fechaResolucion = resolucion.ToString("dd'-'MM'-'yyyy");
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
                var Asunto = "Error al Consultar jornadas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar jornadas", correos.destinatarioErrores);
                return null;
            }

        }

        public List<JornadasBaseVM> ConsultaJornadaIdService(int id, int empresa)
        {

            try
            {


                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT jornadas.id,jornadas.codigo,jornadas.descripcion,jornadas.diasTrabajo,jornadas.diasDescanso,jornadas.numeroCiclos, " +
                                            "jornadas.horasSemanales, jornadas.fechaCreacion, jornadas.resolucion, jornadas.fechaResolucion  " +
                                            "FROM jornadas " +
                                            "WHERE jornadas.habilitado = 1 and id =" + id + " ", BD_Cli);


                List<JornadasBaseVM> opcionesList = new List<JornadasBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new JornadasBaseVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        codigo = dr["Codigo"].ToString(),
                                        descripcion = dr["Descripcion"].ToString(),
                                        diasTrabajo = int.Parse(dr["diasTrabajo"].ToString()),
                                        diasDescanso = int.Parse(dr["diasDescanso"].ToString()),
                                        numeroCiclos = int.Parse(dr["numeroCiclos"].ToString()),
                                        horasSemanales = int.Parse(dr["horasSemanales"].ToString()),
                                        fechaCreacion = dr["fechaCreacion"].ToString(),
                                        resolucion = dr["resolucion"].ToString(),
                                        fechaResolucion = dr["fechaResolucion"].ToString()
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {

                        DateTime creacion = Convert.ToDateTime(r.fechaCreacion);
                        r.fechaCreacion = creacion.ToString("yyyy'-'MM'-'dd");
                        DateTime resolucion = Convert.ToDateTime(r.fechaResolucion);
                        r.fechaResolucion = resolucion.ToString("yyyy'-'MM'-'dd");
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
                var Asunto = "Error al Consultar jornada";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar jornada", correos.destinatarioErrores);

                return null;
            }
        }


        public Resultado CrearJornadaService(JornadasBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;


            try
            {
                if (opciones.descripcion == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la descripción de la jornada";
                    return resultado;
                }
                DateTime hoy = DateTime.Now.Date;
                string hoystr = hoy.ToString("yyyy'-'MM'-'dd");
                opciones.descripcion = opciones.descripcion.ToUpper();
                if (opciones.fechaResolucion == null) opciones.fechaResolucion = hoystr;
                string query2 = "insert into jornadas (codigo,descripcion,diasTrabajo,diasDescanso,numeroCiclos,horasSemanales,fechaCreacion,resolucion,fechaResolucion,habilitado) " +
                "values " +
                "('" + opciones.codigo + "','" + opciones.descripcion + "'," + opciones.diasTrabajo + " ," + opciones.diasDescanso + " ," + opciones.numeroCiclos +
                ", " + opciones.horasSemanales + ", '" + hoystr + "','" + opciones.resolucion + "','" + opciones.fechaResolucion + "' ,1) ! ";
                if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Jornada ingresada de manera exitosa";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se ingreso la información de jornada";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al guardar jornada";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar jornada en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar jornada" + eG.Message.ToString();
                return resultado;
            }

        }
        public Resultado EditaJornadaService(JornadasBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            opciones.descripcion = opciones.descripcion?.ToString() ?? string.Empty;

            try
            {
                if (opciones.descripcion == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la descripción de la jornada";
                    return resultado;
                }
                string query = "update jornadas set [codigo]='" + opciones.codigo + "',[descripcion]= '" + opciones.descripcion +
                                   "', [diasTrabajo]=" + opciones.diasTrabajo + ",[diasDescanso]=" + opciones.diasDescanso + ",[numeroCiclos]= " + opciones.numeroCiclos +
                                   ", [horasSemanales]= " + opciones.horasSemanales +
                                   ",  [resolucion]= '" + opciones.resolucion + "', [fechaResolucion]= '" + opciones.fechaResolucion +
                                    "' where jornadas.id=" + opciones.id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Jornada editada exitosamente.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se editó la información de la jornada.";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar jornada";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar un jornada en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar jornada" + eG.Message.ToString();
                return resultado;
            }


        }

        public Resultado InhabilitaJornadaService(JornadasDeleteVM opciones, int empresa)
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
                    resultado.mensaje = "Debe indicar la informacion del jornada que desea eliminar";
                    return resultado;
                }

                string query = "update jornadas set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Jornada eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó la información de la jornada.";
                }

                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al eliminar jornada";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar jornada en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una jornada" + eG.Message.ToString();
                return resultado;
            }
        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetJornadas
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public string inicio { get; set; }
        public string termino { get; set; }
        public string requisitos { get; set; }
        public string funciones { get; set; }
        public string sueldoMinimo { get; set; }
        public string sueldoMaximo { get; set; }
    }
}