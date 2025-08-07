using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.PermisoAutoriza
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IPermisoAutorizaService
    {
        /// <summary>
        /// Genera listado de permisos.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista permisos</returns>
        public List<DetPermisoAsistencia> ListarPermisoAutorizaService(int idEmpresa, string desde,string hasta);

        /// <summary>
        /// Autoriza permosos.
        /// </summary>
        /// <param name="ids">ID de los permisos autorizados</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="idusuario">ID de la persona que aprueba.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista permisos</returns>
        public Resultado AutorizaPermisosService(string ids, int idempresa,string idusuario, string desde, string hasta);
 
        /// <summary>
        /// Rechaza permisos
        /// </summary>
        /// <param name="ids">ID de los permisos autorizados</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="idusuario">ID de la persona que aprueba.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista permisos</returns>
        public Resultado RechazaPermisosService(string ids, int idempresa, string idusuario, string desde, string hasta);

    }

    public class PermisoAutorizaService : IPermisoAutorizaService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public PermisoAutorizaService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetPermisoAsistencia> ListarPermisoAutorizaService(int empresa,  string desde,string hasta)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                DateTime fecfin = Convert.ToDateTime(hasta).AddDays(1);
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");


                f.EjecutarConsultaSQLCli("SELECT permisoasistencia.id,permisoasistencia.rutTrabajador,permisoasistencia.fechaInicio " +
                                            ",permisoasistencia.fechaTermino,permisoasistencia.goseSueldo, permisoasistencia.comentario " +
                                            "FROM permisoasistencia " +
                                            "WHERE permisoasistencia.habilitado = 1 and permisoasistencia.estado = ''" +
                                            " and permisoasistencia.fechainicio <= '" + fecfinstr+ "' and permisoasistencia.fechatermino >= '" + desde+"' ", BD_Cli);


                List<DetPermisoAsistencia> opcionesList = new List<DetPermisoAsistencia>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetPermisoAsistencia()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        ruttrabajador = dr["rutTrabajador"].ToString(),
                                        fechainicio = dr["fechainicio"].ToString(),
                                        fechatermino = dr["fechatermino"].ToString(),
                                        gosesueldo = bool.Parse(dr["goseSueldo"].ToString()),
                                        comentario = dr["comentario"].ToString(),
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        DateTime inicio = Convert.ToDateTime(r.fechainicio);
                        r.fechainicio = inicio.ToString("dd'-'MM'-'yyyy");
                        DateTime termino = Convert.ToDateTime(r.fechatermino);
                        r.fechatermino = termino.ToString("dd'-'MM'-'yyyy");
                        r.horainicio = inicio.ToString("HH':'mm");
                        r.horatermino = termino.ToString("HH':'mm");
                        PersonasBaseVM pers = Generales.BuscaPersona(r.ruttrabajador, BD_Cli);
                        r.nombre = pers.Nombres + " " + pers.Apellidos;
                        r.gose = 0;
                        r.gosestr = "NO";
                        if (r.gosesueldo)
                        {
                            r.gose = 1;
                            r.gosestr = "SI";
                        }
                        r.dias = (int)termino.Subtract(inicio).TotalDays +1;
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
                var Asunto = "Error en consulta de licencias";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar licencias", correos.destinatarioErrores);
                return null;
            }
        }
        public Resultado AutorizaPermisosService(string ids, int empresa, string idusuario, string desde, string hasta)
        {
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            string[] idsSeparados = ids.Split("*");
            int nsel = idsSeparados.Length - 1;
            Resultado resultado = new Resultado();
            resultado.result = 0;
            DateTime hoy = DateTime.Now;
            try
            {
                int ind;
                for (ind = 0; ind < nsel; ind++)
                {
                    if (idsSeparados[ind] != null)
                    {
                        int id = Convert.ToInt32(idsSeparados[ind]);
                        string query = "update permisoasistencia set estado='A', fechaVerificacion='"+hoy+"', idusuario= "+idusuario+
                            " where id=" + id + "  ! ";

                        if (f.EjecutarQuerySQLCli(query, BD_Cli))
                        {
                            resultado.result = 1;
                            resultado.mensaje = "Registro actualizado de manera exitosa.";
                        }
                    }

                }
            }
            catch (Exception eG)
            {
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();
                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar actualizar permiso" + eG.Message.ToString();
            }
            return resultado;
        }
        public Resultado RechazaPermisosService(string ids, int empresa, string idusuario, string desde, string hasta)
        {
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            string[] idsSeparados = ids.Split("*");
            int nsel = idsSeparados.Length - 1;
            Resultado resultado = new Resultado();
            resultado.result = 0;
            DateTime hoy = DateTime.Now;
            try
            {
                int ind;
                for (ind = 0; ind < nsel; ind++)
                {
                    if (idsSeparados[ind] != null)
                    {
                        int id = Convert.ToInt32(idsSeparados[ind]);
                        string query = "update permisoasistencia set estado='R', fechaVerificacion='" + hoy + "', idusuario= " + idusuario +
                            " where id=" + id + "  ! ";

                        if (f.EjecutarQuerySQLCli(query, BD_Cli))
                        {
                            resultado.result = 1;
                            resultado.mensaje = "Registro actualizado de manera exitosa.";
                        }
                    }

                }
            }
            catch (Exception eG)
            {
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();
                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar actualizar permiso" + eG.Message.ToString();
            }
            return resultado;
        }
    }
}

