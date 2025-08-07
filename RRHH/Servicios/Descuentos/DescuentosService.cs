using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Collections.Immutable;
using System.Data;

namespace RRHH.Servicios.Descuentos
{
    /// <summary>
    /// Servicio para generar y operar con los descuentos de una empresa.
    /// Ejem. Sueldo base.
    /// </summary>
    public interface IDescuentosService
    {
        /// <summary>
        /// Genera lista de descuentos.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de descuentos</returns>
        public List<DetDescuentos> ListarDescuentosService(int idEmpresa);

        /// <summary>
        /// Consulta por id descuento.
        /// </summary>
        /// <param name="id">ID descuento">ID de una empresa.</param>
        /// <returns>Muestra informacion de descuento</returns>
        public List<DescuentoBaseVM> ConsultaDescuentoIdService(int id, int idEmpresa);

        /// <summary>
        /// Creación de descuento.
        /// </summary>
        /// <param name="opciones">Registro de descuento</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns
        public Resultado CrearDescuentoService(DescuentoBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Edita descuento.
        /// </summary>
        /// <param name="opciones">Registro de descuento</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns>
        public Resultado EditaDescuentoService(DescuentoBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Inhabilitar descuento.
        /// </summary>
        /// <param name="id">ID de descuento</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita descuento</returns>
        public Resultado InhabilitaDescuentoService(DescuentoDeleteVM opciones, int idEmpresa);

        /// <summary>
        /// Cargar tabla de codigo de la dirección del trabajo.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de codigo</returns>
        public Resultado ComboCodigosDTService(int idEmpresa);

        /// <summary>
        /// Cargar tabla de codigos de previred.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de codigos</returns>
        public Resultado ComboCodigosPreviredService(int idEmpresa);

    }

    public class DescuentosService : IDescuentosService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public DescuentosService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetDescuentos> ListarDescuentosService(int empresa)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT descuentos.id,descuentos.descuento,descuentos.descripcion,descuentos.prioridad,descuentos.minimo,descuentos.maximo, " +
                                            " descuentos.codigoDT, descuentos.codigoPrevired" +
                                            " FROM descuentos " +
                                            " WHERE descuentos.habilitado = 1 ", BD_Cli);


                List<DescuentoBaseVM> opcionesList = new List<DescuentoBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DescuentoBaseVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        descuento = int.Parse(dr["descuento"].ToString()),
                                        descripcion = dr["Descripcion"].ToString(),
                                        prioridad = int.Parse(dr["prioridad"].ToString()),
                                        minimo = int.Parse(dr["minimo"].ToString()),
                                        maximo = int.Parse(dr["maximo"].ToString()),
                                        codigoDT = dr["CodigoDT"].ToString(),
                                        codigoprevired = dr["codigoPrevired"].ToString(),
                                    }).ToList();
                    List<DetDescuentos> salida = new List<DetDescuentos>();
                    foreach(var d in opcionesList)
                    {
                        DetDescuentos registro = new DetDescuentos();
                        registro.id = d.id;
                        registro.descuento = d.descuento;
                        registro.descripcion = d.descripcion;
                        registro.minimo = d.minimo.ToString("###,###,###");
                        registro.maximo = d.maximo.ToString("###,###,###");
                        registro.codigoDT = d.codigoDT;
                        registro.prioridad = PrioridadDescuento(d.prioridad);
                        salida.Add(registro);
                    }
                    return salida;
                }
              return null;
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar descuentos";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar descuentos", correos.destinatarioErrores);
                return null;
            }

        }
        public List<DescuentoBaseVM> ConsultaDescuentoIdService(int id, int empresa)
        {

            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT descuentos.id,descuentos.descuento,descuentos.descripcion,descuentos.prioridad,descuentos.minimo,descuentos.maximo, " +
                                            " descuentos.codigoDT, descuentos.codigoPrevired" +
                                            " FROM descuentos " +
                                            " WHERE descuentos.habilitado = 1  and descuentos.id ="+id, BD_Cli);


                List<DescuentoBaseVM> opcionesList = new List<DescuentoBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DescuentoBaseVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        descuento = int.Parse(dr["descuento"].ToString()),
                                        descripcion = dr["Descripcion"].ToString(),
                                        prioridad = int.Parse(dr["prioridad"].ToString()),
                                        minimo = int.Parse(dr["minimo"].ToString()),
                                        maximo = int.Parse(dr["maximo"].ToString()),
                                        codigoDT = dr["CodigoDT"].ToString(),
                                        codigoprevired = dr["codigoPrevired"].ToString(),
                                    }).ToList();
                    return opcionesList;

                }
                return null;
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar descuento";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar descuento", correos.destinatarioErrores);

                return null;
            }
        }


        public Resultado CrearDescuentoService(DescuentoBaseVM opciones, int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            try
            {
                f.EjecutarConsultaSQLCli("SELECT descuentos.id,descuentos.descuento,descuentos.descripcion,descuentos.prioridad,descuentos.minimo,descuentos.maximo, " +
                                            " descuentos.codigoDT, descuentos.codigoPrevired" +
                                            " FROM descuentos " +
                                            " WHERE descuentos.habilitado = 1  and descuentos.descuento =" + opciones.descuento, BD_Cli);
                if (f.Tabla.Rows.Count > 0)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Codigo de descuento repetido.";
                    return resultado;
                }

                opciones.descripcion = opciones.descripcion.ToUpper();
                opciones.codigoprevired = "0";
                if (opciones.minimo == null) opciones.minimo = 0;
                if (opciones.maximo == null) opciones.maximo = 0;

                string query2 = "insert into descuentos (descuento, descripcion, prioridad, minimo, maximo, codigoDT, codigoPrevired, habilitado) " +
                    "values " +
                    "(" + opciones.descuento + ", '" + opciones.descripcion + "', " + opciones.prioridad + ", " + opciones.minimo + ", " + opciones.maximo +
                    ", '" + opciones.codigoDT+ "', '" + opciones.codigoprevired +  "' ,1)  ";
                if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Descuento ingresada de manera exitosa";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se ingreso la información de descuento";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al guardar descuento";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar descuento en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar descuento" + eG.Message.ToString();
                return resultado;
            }

        }
        public Resultado EditaDescuentoService(DescuentoBaseVM opciones, int empresa)
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
                    resultado.mensaje = "Debe indicar la descripción del descuento";
                    return resultado;
                }


                string query = "update descuentos set [descuento]=" + opciones.descuento + ",[descripcion]='" + opciones.descripcion +
                                   "', [prioridad]=" + opciones.prioridad + ",[minimo]=" + opciones.minimo + ",[maximo]= " + opciones.maximo+
                                   ", [codigoDT]= '" + opciones.codigoDT + "', [codigoPrevired]= '" + opciones.codigoprevired +
                                   "' where descuentos.id=" + opciones.id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Descuento editada exitosamente.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se editó la información del descuento.";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar descuento";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar una descuento en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar descuento" + eG.Message.ToString();
                return resultado;
            }

        }

        public Resultado InhabilitaDescuentoService(DescuentoDeleteVM opciones, int empresa)
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
                    resultado.mensaje = "Debe indicar la informacion del descuento que desea eliminar";
                    return resultado;
                }

                string query = "update descuentos set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Descuento eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó la información de la descuento.";
                }

                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al eliminar descuento";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar descuento en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una descuento" + eG.Message.ToString();
                return resultado;
            }
        }
        public Resultado ComboCodigosDTService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen codigos DT";
            resultado.result = 0;
            try
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "Remuneracion";

                f.EjecutarConsultaSQLCli("select * from parametrosGenerales Where habilitado = 1 and tabla= 'DTDSCTO' Order by descripcion", BD_Cli);

                if (f.Tabla.Rows.Count > 0)
                {
                    var opcionesList = (from DataRow dr in f.Tabla.Rows
                                        select new
                                        {
                                            id = int.Parse(dr["codigo"].ToString()),
                                            descripcion = dr["Descripcion"].ToString()
                                        }).ToList();
                    resultado.data = opcionesList;
                    resultado.mensaje = "Existen codigos en la coleccion";
                    resultado.result = 1;

                }
                return resultado;
            }
            catch (Exception ex)
            {
                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return resultado;
            }
        }
        public Resultado ComboCodigosPreviredService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen codigos previred";
            resultado.result = 0;
            try
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "Remuneracion" ;

                f.EjecutarConsultaSQLCli("select * from parametrosGenerales Where habilitado = 1 and tabla= 'PRHABER' Order by descripcion", BD_Cli);

                if (f.Tabla.Rows.Count > 0)
                {
                    var opcionesList = (from DataRow dr in f.Tabla.Rows
                                        select new
                                        {
                                            id = int.Parse(dr["id"].ToString()),
                                            descripcion = dr["Descripcion"].ToString()
                                        }).ToList();
                    resultado.data = opcionesList;
                    resultado.mensaje = "Existen codigos en la coleccion";
                    resultado.result = 1;

                }
                return resultado;
            }
            catch (Exception ex)
            {
                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return resultado;
            }
        }
        public string PrioridadDescuento(int prioridad)
        {
            if (prioridad == 1) return "OBIGATORIO";
            if (prioridad == 2) return "ALTA";
            if (prioridad == 3) return "MEDIA";
            if (prioridad == 4) return "BAJA";
            if (prioridad == 5) return "MUY BAJA";
            return null;
        }
    }
    }
public class DetDescuentos
{
    public int id { get; set; }
    public int descuento { get; set; }
    public string descripcion { get; set; }
    public string prioridad { get; set; }
    public string minimo { get; set; }
    public string maximo { get; set; }
    public string codigoDT { get; set; }
    public string codigoprevired { get; set; }
    public int habilitado { get; set; }
}

