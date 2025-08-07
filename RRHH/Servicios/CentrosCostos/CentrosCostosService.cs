using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.CentrosCostos
{
    /// <summary>
    /// Servicio para generar y operar con las centrosCostos de una empresa.
    /// Ejem. Oficina central.
    /// </summary>
    public interface ICentrosCostoService
    {
        /// <summary>
        /// Genera lista de centros de costos.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de centrosCostos</returns>
        public List<CentrosCostosBaseVM> ListarCentrosCostosService(int idEmpresa);

        /// <summary>
        /// Consulta por id centros de costo.
        /// </summary>
        /// <param name="id">ID de cuenta especial</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Muestra informacion de centrosCosto</returns>
        public List<CentrosCostosBaseVM> ConsultaCentrosCostoIdService(int id, int idEmpresa);
        /// <summary>
        /// Creación de centros de costo.
        /// </summary>
        /// <param name="opciones">Registro de centrosCosto</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns
        public Resultado CrearCentrosCostoService(CentrosCostosBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Edita centrosCosto.
        /// </summary>
        /// <param name="opciones">Registro de centros de costo</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns>
        public Resultado EditaCentrosCostoService(CentrosCostosBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Inhabilitar centro de costo.
        /// </summary>
        /// <param name="id">ID de centro de costos</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita cuenta especial</returns>
        public Resultado InhabilitaCentrosCostoService(CentrosCostosDeleteVM opciones, int idEmpresa);

    }

    public class CentrosCostosService : ICentrosCostoService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public CentrosCostosService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<CentrosCostosBaseVM> ListarCentrosCostosService(int empresa)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT centrosCostos.id,centrosCostos.descripcion,centrosCostos.rutJefe,centrosCostos.observaciones " +
                                            "FROM centrosCostos " +
                                            "WHERE centrosCostos.habilitado = 1 ", BD_Cli);


                List<CentrosCostosBaseVM> opcionesList = new List<CentrosCostosBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new CentrosCostosBaseVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        descripcion = dr["Descripcion"].ToString(),
                                        rutJefe = dr["rutJefe"].ToString(),
                                        observaciones = dr["observaciones"].ToString()
                                    }).ToList();
                    return opcionesList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar centrosCostos";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar centrosCostos", correos.destinatarioErrores);
                return null;
            }

        }
        public List<CentrosCostosBaseVM> ConsultaCentrosCostoIdService(int id, int empresa)
        {

            try
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                f.EjecutarConsultaSQLCli("SELECT centrosCostos.id,centrosCostos.descripcion,centrosCostos.rutJefe,centrosCostos.observaciones " +
                                            "FROM centrosCostos " +
                                            "WHERE centrosCostos.habilitado = 1 and centrosCostos.id ="+ id, BD_Cli);


                List<CentrosCostosBaseVM> opcionesList = new List<CentrosCostosBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new CentrosCostosBaseVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        descripcion = dr["Descripcion"].ToString(),
                                        rutJefe = dr["rutJefe"].ToString(),
                                        observaciones = dr["observaciones"].ToString()
                                    }).ToList();
                    return opcionesList;
                }
                return null;
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar centros de costo";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar centro de costo", correos.destinatarioErrores);

                return null;
            }
        }


        public Resultado CrearCentrosCostoService(CentrosCostosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            opciones.descripcion = opciones.descripcion?.ToString() ?? string.Empty;
            opciones.observaciones = opciones.observaciones?.ToString() ?? string.Empty;

            try
            {
                if (opciones.descripcion == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la descripción del centro de costo";
                    return resultado;
                }
                opciones.descripcion = opciones.descripcion.ToUpper();
                opciones.observaciones = opciones.observaciones.ToUpper();

                string query2 = "insert into centrosCostos (descripcion,rutJefe,observaciones,habilitado) " +
                "values ('" + opciones.descripcion + "','" + opciones.rutJefe+ "','" + opciones.observaciones + "' ,1) ";
                if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Centro de costo ingresada de manera exitosa";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se ingreso la información de centro de costo";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Guardar centro de costo";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar centro de costo en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar centro de costo" + eG.Message.ToString();
                return resultado;
            }

        }
        public Resultado EditaCentrosCostoService(CentrosCostosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            opciones.descripcion = opciones.descripcion?.ToString() ?? string.Empty;
            opciones.observaciones = opciones.observaciones?.ToString() ?? string.Empty;

            try
            {
                if (opciones.descripcion == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la descripción del centro de costo";
                    return resultado;
                }

                opciones.descripcion = opciones.descripcion.ToUpper();
                opciones.observaciones = opciones.observaciones.ToUpper();

                string query = "update centrosCostos set [descripcion]='" + opciones.descripcion +
                                   "', [rutJefe]='" + opciones.rutJefe + "',[observaciones]='" + opciones.observaciones +
                                    "' where centrosCostos.id=" + opciones.id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Centro de costo editada exitosamente.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se editó la información del centro de costo.";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar centro de costo ";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar una centro de costo en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar centro de costo" + eG.Message.ToString();
                return resultado;
            }

        }

        public Resultado InhabilitaCentrosCostoService(CentrosCostosDeleteVM opciones, int empresa)
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
                    resultado.mensaje = "Debe indicar la informacion del centrosCosto que desea eliminar";
                    return resultado;
                }

                string query = "update centrosCostos set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Centro de costo eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó la información de la centro de costo.";
                }

                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al eliminar centro de costo";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar centro de costo en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una centro de costo" + eG.Message.ToString();
                return resultado;
            }
        }
    }
}
