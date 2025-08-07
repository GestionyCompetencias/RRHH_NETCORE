using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.PermisoAsistencia
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IPermisoAsistenciaService
    {
        /// <summary>
        /// Genera lista de permisos del trabajador.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="rut">HRut del trabajador a listar</param>
        /// <returns>Lista de permisos de trabajador</returns>
        public List<DetPermisoAsistencia> ListarPermisoAsistenciaService(int idEmpresa, string rut);

        /// <summary>
        /// Consulta por permiso 
        /// </summary>
        /// <param name="id">ID de haber informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Muestra informacion de permiso</returns>
        public List<DetPermisoAsistencia> ConsultaPermisoAsistenciaIdService(int id, int idEmpresa);

        /// <summary>
        /// Creación permiso 
        /// </summary>
        /// <param name="opciones">Registro de permiso medica</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns
        public Resultado CrearPermisoAsistenciaService(DetPermisoAsistencia opciones, int idEmpresa);

        /// <summary>
        /// Edita permiso.
        /// </summary>
        /// <param name="opciones">Registro de permiso medica</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns>
        public Resultado EditaPermisoAsistenciaService(DetPermisoAsistencia opciones, int idEmpresa);

        /// <summary>
        /// Inhabilitar permiso.
        /// </summary>
        /// <param name="id">ID de haber informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita permiso</returns>
        public Resultado InhabilitaPermisoAsistenciaService(PermisoDeleteVM opciones, int idEmpresa);

        /// <summary>
        /// Carga lista de trabajadores.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de trabajadores</returns>
        public Resultado ComboTrabajadoresService(int idEmpresa);

    }

    public class PermisoAsistenciaService : IPermisoAsistenciaService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public PermisoAsistenciaService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetPermisoAsistencia> ListarPermisoAsistenciaService(int empresa, string rut)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;


                f.EjecutarConsultaSQLCli("SELECT permisoasistencia.id,permisoasistencia.rutTrabajador,permisoasistencia.fechaInicio " +
                                            ",permisoasistencia.fechaTermino,permisoasistencia.goseSueldo, permisoasistencia.comentario " +
                                            "FROM permisoasistencia " +
                                            "WHERE permisoasistencia.habilitado = 1 and permisoasistencia.estado = ''" +
                                             "and permisoasistencia.rutTrabajador ='" + rut + "' ", BD_Cli);


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
                        PersonasBaseVM pers = Generales.BuscaPersona(r.ruttrabajador, BD_Cli);
                        r.horainicio = inicio.ToString("HH':'mm");
                        r.horatermino = termino.ToString("HH':'mm");
                        r.nombre = pers.Nombres + " " + pers.Apellidos;
                        r.gose = 0;
                        r.gosestr = "NO";
                        if (r.gosesueldo)
                        {
                            r.gose = 1;
                            r.gosestr = "SI";
                        }
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
        public List<DetPermisoAsistencia> ConsultaPermisoAsistenciaIdService(int id, int empresa)
        {

            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                f.EjecutarConsultaSQLCli("SELECT permisoasistencia.id,permisoasistencia.rutTrabajador,permisoasistencia.fechaInicio " +
                                            ",permisoasistencia.fechaTermino,permisoasistencia.goseSueldo, permisoasistencia.comentario " +
                                            "FROM permisoasistencia " +
                                            "WHERE permisoasistencia.habilitado = 1 and permisoasistencia.id =" + id , BD_Cli);


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
                        r.fechainicio = inicio.ToString("yyyy'-'MM'-'dd");
                        DateTime termino = Convert.ToDateTime(r.fechatermino);
                        r.fechatermino = termino.ToString("yyyy'-'MM'-'dd");
                        r.horainicio = inicio.ToString("HH':'mm");
                        r.horatermino = termino.ToString("HH':'mm");
                        r.gose = 0;
                        if (r.gosesueldo) r.gose = 1;
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
                var Asunto = "Error al Consultar permiso";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar Licencia", correos.destinatarioErrores);
                return null;
            }
        }


        public Resultado CrearPermisoAsistenciaService(DetPermisoAsistencia opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            try
            {
                DateTime hoy = DateTime.Now.Date;
                string hoystr = hoy.ToString("yyyy'-'MM'-'dd");
                opciones.gosesueldo =true;
                if (opciones.gose==0) opciones.gosesueldo = false;
                opciones.fechainicio = opciones.fechainicio + " " + opciones.horainicio;
                opciones.fechatermino = opciones.fechatermino + " " + opciones.horatermino;
                string query2 = "insert into permisoasistencia (ruttrabajador,fechaInicio,fechaTermino,goseSueldo,comentario,fechaIngreso,estado,habilitado) " +
                                "values " +
                                "('" + opciones.ruttrabajador + "','" + opciones.fechainicio + "','" + opciones.fechatermino +
                                "', '" + opciones.gosesueldo + "' , '" + opciones.comentario + "', '" + hoystr + "','',1) ";
                if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Registro ingresado de  manera exitosa";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se ingreso la información";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al guardar permiso";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar licenia en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar permiso" + eG.Message.ToString();
                return resultado;
            }

        }
        public Resultado EditaPermisoAsistenciaService(DetPermisoAsistencia opciones, int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            try
            {
                opciones.gosesueldo = true;
                if (opciones.gose == 0) opciones.gosesueldo = false;
                opciones.fechainicio = opciones.fechainicio + " " + opciones.horainicio;
                opciones.fechatermino = opciones.fechatermino + " " + opciones.horatermino;
                string query = "update permisoasistencia set [rutTrabajador]='" + opciones.ruttrabajador + 
                               "',  [fechaInicio]='" + opciones.fechainicio + "',[fechaTermino]='" + opciones.fechatermino + "',[goseSueldo]= '" + opciones.gosesueldo +
                               "',[comentario]='" + opciones.comentario +
                               "' where permisoasistencia.id=" + opciones.id ;

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 2;
                    resultado.mensaje = "Registro editado exitosamente.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se editó la información.";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar permiso";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar un permiso en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar permiso" + eG.Message.ToString();
                return resultado;
            }
        }

        public Resultado InhabilitaPermisoAsistenciaService(PermisoDeleteVM opciones, int empresa)
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
                    resultado.mensaje = "Debe indicar la informacion de la permiso  que desea eliminar";
                    return resultado;
                }

                string query = "update permisoasistencia set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Registro eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó el registro.";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al eliminar registro";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar permiso en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una permiso" + eG.Message.ToString();
                return resultado;
            }
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
    public class DetPermisoAsistencia
    {
        public int id { get; set; }
        public string ruttrabajador { get; set; }
        public string nombre { get; set; }
        public string fechainicio { get; set; }
        public string horainicio { get; set; }
        public string fechatermino { get; set; }
        public string horatermino { get; set; }
        public bool gosesueldo { get; set; }
        public int gose { get; set; }
        public string gosestr { get; set; }
        public int dias { get; set; }
        public string comentario { get; set; }
        public int habilitado { get; set; }
    }
}

