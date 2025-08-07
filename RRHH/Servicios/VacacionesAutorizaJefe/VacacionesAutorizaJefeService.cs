using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.VacacionesAutorizaJefe
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IVacacionesAutorizaJefeService
    {
        /// <summary>
        /// Genera listado de vacaciones.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista vacaciones</returns>
        public Resultado ListarVacacionesAutorizaJefeService(int idEmpresa, string desde,string hasta);

        /// <summary>
        /// Autoriza permosos.
        /// </summary>
        /// <param name="ids">ID de los vacaciones autorizados</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="idusuario">ID de la persona que aprueba.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista vacaciones</returns>
        public Resultado AutorizaVacacionesService(string ids, int idempresa,string idusuario, string desde, string hasta);
 
        /// <summary>
        /// Rechaza vacaciones
        /// </summary>
        /// <param name="ids">ID de los vacaciones autorizados</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="idusuario">ID de la persona que aprueba.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista vacaciones</returns>
        public Resultado RechazaVacacionesService(string ids, int idempresa, string idusuario, string desde, string hasta);

    }

    public class VacacionesAutorizaJefeService : IVacacionesAutorizaJefeService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public VacacionesAutorizaJefeService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public Resultado ListarVacacionesAutorizaJefeService(int empresa,  string desde,string hasta)
        {
            UsuarioVM perfiles = new UsuarioVM();
            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                resultado.mensaje = "No existen registros";
                DateTime fecfin = Convert.ToDateTime(hasta).AddDays(1);
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");
                f.EjecutarConsultaSQLCli("SELECT vacacionesSolicitud.id,vacacionesSolicitud.rutTrabajador " +
                                            ",vacacionesSolicitud.fechaInicio,vacacionesSolicitud.fechaTermino " +
                                            "FROM vacacionesSolicitud " +
                                            "WHERE vacacionesSolicitud.habilitado = 1 and vacacionesSolicitud.estadoSolicitud = 'Solicitud'" +
                                            " and vacacionesSolicitud.fechainicio <= '" + fecfinstr+ "' and vacacionesSolicitud.fechatermino >= '" + desde+"' ", BD_Cli);


                List<DetAutorizaJefe> opcionesList = new List<DetAutorizaJefe>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetAutorizaJefe()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        ruttrabajador = dr["rutTrabajador"].ToString(),
                                        fechainicio = dr["fechainicio"].ToString(),
                                        fechatermino = dr["fechatermino"].ToString()
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        DateTime inicio = Convert.ToDateTime(r.fechainicio);
                        r.fechainicio = inicio.ToString("dd'-'MM'-'yyyy");
                        DateTime termino = Convert.ToDateTime(r.fechatermino);
                        r.fechatermino = termino.ToString("dd'-'MM'-'yyyy");
                        PersonasBaseVM pers = Generales.BuscaPersona(r.ruttrabajador, BD_Cli);
                        r.nombre = pers.Nombres + " " + pers.Apellidos;
                        r.diastotal = (int)termino.Subtract(inicio).TotalDays +1;
                    }
                    resultado.result = 1;
                    resultado.data = opcionesList;
                    resultado.mensaje = "Existen registros";
                }
                return resultado;
            }
            catch (Exception ex)
            {
                var Asunto = "Error en listar autorizacion";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" ;
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar autorizacion", correos.destinatarioErrores);
                return resultado;
            }
        }
        public Resultado AutorizaVacacionesService(string ids, int empresa, string idusuario, string desde, string hasta)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "Debe selecionar registros";
            if (ids != null && ids.Length != 0)
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                string[] idsSeparados = ids.Split("*");
                int nsel = idsSeparados.Length - 1;
                DateTime hoy = DateTime.Now;
                string hoystr = hoy.ToString("yyyy'-'MM'-'dd' 'hh':'mm");
                try
                {
                    int ind;
                    for (ind = 0; ind < nsel; ind++)
                    {
                        if (idsSeparados[ind] != null)
                        {
                            int id = Convert.ToInt32(idsSeparados[ind]);
                            string query = "update vacacionesSolicitud set estadoSolicitud='Aprobada', fechaApruebaJefe='" + hoystr + "', idUsuarioJefe= " + idusuario +
                                " where id=" + id;

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
                    var Mensaje = eG.Message.ToString() + "<br><hr><br>";
                    resultado.result = -1;
                    resultado.mensaje = "Fallo al intentar actualizar autorizacion" + eG.Message.ToString();
                }
            }
            return resultado;
        }
        public Resultado RechazaVacacionesService(string ids, int empresa, string idusuario, string desde, string hasta)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "Debe selecionar registros";
            if (ids != null && ids.Length != 0)
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                string[] idsSeparados = ids.Split("*");
                int nsel = idsSeparados.Length - 1;
                DateTime hoy = DateTime.Now;
                string hoystr = hoy.ToString("yyyy'-'MM'-'dd' 'hh':'mm");
                try
                {
                    int ind;
                    for (ind = 0; ind < nsel; ind++)
                    {
                        if (idsSeparados[ind] != null)
                        {
                            int id = Convert.ToInt32(idsSeparados[ind]);
                            string query = "update vacacionesSolicitud set estadoSolicitud='Rechaza', fechaApruebaJefe='" + hoystr + "', idUsuarioJefe= " + idusuario +
                                " where id=" + id;

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
                    var Mensaje = eG.Message.ToString() + "<br><hr><br>";
                    resultado.result = -1;
                    resultado.mensaje = "Fallo al intentar actualizar autorizacion" + eG.Message.ToString();
                }
            }
            return resultado;
        }
    }
}

public class DetAutorizaJefe
{
    public int id { get; set; }
    public string ruttrabajador { get; set; }
    public string nombre { get; set; }
    public string fechainicio { get; set; }
    public string fechatermino { get; set; }
    public int diastotal { get; set; }
}
