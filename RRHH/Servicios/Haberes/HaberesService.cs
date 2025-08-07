using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.Haberes
{
    /// <summary>
    /// Servicio para generar y operar con los haberes de una empresa.
    /// Ejem. Sueldo base.
    /// </summary>
    public interface IHaberesService
    {
        /// <summary>
        /// Genera lista de haberes.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de haberes</returns>
        public List<HaberBaseVM> ListarHaberesService(int idEmpresa);

        /// <summary>
        /// Consulta por id habere.
        /// </summary>
        /// <param name="id">ID habere">ID de una empresa.</param>
        /// <returns>Muestra informacion de haber</returns>
        public List<HaberBaseVM> ConsultaHaberIdService(int id, int idEmpresa);

        /// <summary>
        /// Creación de haber.
        /// </summary>
        /// <param name="opciones">Registro de haber</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns
        public Resultado CrearHaberService(HaberBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Edita haber.
        /// </summary>
        /// <param name="opciones">Registro de haber</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns>
        public Resultado EditaHaberService(HaberBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Inhabilitar habere.
        /// </summary>
        /// <param name="id">ID de habere</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita habere</returns>
        public Resultado InhabilitaHaberService(HaberDeleteVM opciones, int idEmpresa);

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

    public class HaberesService : IHaberesService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public HaberesService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<HaberBaseVM> ListarHaberesService(int empresa)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT haberes.id,haberes.haber,haberes.descripcion,haberes.imponible,haberes.tributable,haberes.numeroMeses, " +
                                            " haberes.garantizado, haberes.retenible, haberes.calculado, haberes.tiempo, haberes.deducible, "+
                                            " haberes.baseLicencia, haberes.baseSobretiempo, " +
                                            " haberes.baseIndemnizacion, haberes.baseVariable, haberes.codigoDT, haberes.codigoPrevired" +
                                            " FROM haberes " +
                                            " WHERE haberes.habilitado = 1 ", BD_Cli);


                List<HaberBaseVM> opcionesList = new List<HaberBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new HaberBaseVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        haber = int.Parse(dr["haber"].ToString()),
                                        descripcion = dr["Descripcion"].ToString(),
                                        imponible = dr["imponible"].ToString(),
                                        tributable = dr["tributable"].ToString(),
                                        numeromeses = int.Parse(dr["numeroMeses"].ToString()),
                                        garantizado = dr["garantizado"].ToString(),
                                        retenible = dr["retenible"].ToString(),
                                        calculado = dr["calculado"].ToString(),
                                        tiempo = dr["tiempo"].ToString(),
                                        deducible = dr["deducible"].ToString(),
                                        baselicencia = dr["baseLicencia"].ToString(),
                                        basesobretiempo = dr["baseSobretiempo"].ToString(),
                                        baseindemnizacion = dr["baseIndemnizacion"].ToString(),
                                        basevariable = dr["baseVariable"].ToString(),
                                        codigoDT = dr["CodigoDT"].ToString(),
                                        codigoprevired = dr["codigoPrevired"].ToString(),
                                    }).ToList();
                    return opcionesList;

                }
              return null;
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar haberes";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar haberes", correos.destinatarioErrores);
                return null;
            }

        }
        public List<HaberBaseVM> ConsultaHaberIdService(int id, int empresa)
        {

            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT haberes.id,haberes.haber,haberes.descripcion,haberes.imponible,haberes.tributable,haberes.numeroMeses, " +
                                            " haberes.garantizado, haberes.retenible, haberes.calculado, haberes.tiempo, haberes.deducible, " +
                                            " haberes.baseLicencia, haberes.baseSobretiempo, " +
                                            " haberes.baseIndemnizacion, haberes.baseVariable, haberes.codigoDT, haberes.codigoPrevired" +
                                            " FROM haberes " +
                                            " WHERE haberes.habilitado = 1 and haberes.id ="+ id, BD_Cli);


                List<HaberBaseVM> opcionesList = new List<HaberBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new HaberBaseVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        haber = int.Parse(dr["haber"].ToString()),
                                        descripcion = dr["Descripcion"].ToString(),
                                        imponible = dr["imponible"].ToString(),
                                        tributable = dr["tributable"].ToString(),
                                        numeromeses = int.Parse(dr["numeroMeses"].ToString()),
                                        garantizado = dr["garantizado"].ToString(),
                                        retenible = dr["retenible"].ToString(),
                                        calculado = dr["calculado"].ToString(),
                                        tiempo = dr["tiempo"].ToString(),
                                        deducible = dr["deducible"].ToString(),
                                        baselicencia = dr["baseLicencia"].ToString(),
                                        basesobretiempo = dr["baseSobretiempo"].ToString(),
                                        baseindemnizacion = dr["baseIndemnizacion"].ToString(),
                                        basevariable = dr["baseVariable"].ToString(),
                                        codigoDT = dr["CodigoDT"].ToString(),
                                        codigoprevired = dr["codigoPrevired"].ToString(),
                                    }).ToList();
                    return opcionesList;

                }
               return null;
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar haber";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar haber", correos.destinatarioErrores);

                return null;
            }
        }


        public Resultado CrearHaberService(HaberBaseVM opciones, int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            try
            {
                f.EjecutarConsultaSQLCli("SELECT haberes.id,haberes.haber,haberes.descripcion,haberes.imponible,haberes.tributable,haberes.numeroMeses, " +
                                            " haberes.garantizado, haberes.retenible, haberes.calculado, haberes.tiempo, haberes.deducible, " +
                                            " haberes.baseLicencia, haberes.baseSobretiempo, " +
                                            " haberes.baseIndemnizacion, haberes.baseVariable, haberes.codigoDT, haberes.codigoPrevired" +
                                            " FROM haberes " +
                                            " WHERE haberes.habilitado = 1 and haberes.haber =" + opciones.haber , BD_Cli);
                if (f.Tabla.Rows.Count > 0)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Codigo de haber repetido.";
                    return resultado;
                }

                opciones.descripcion = opciones.descripcion.ToUpper();
                opciones.calculado = "N";
                opciones.tiempo = "N";
                opciones.codigoprevired = "0";
                opciones.numeromeses = 12;

                string query2 = "insert into haberes (haber, descripcion, imponible, tributable, numeroMeses, garantizado, retenible, calculado, tiempo"+
                    ", deducible, baseLicencia, baseSobretiempo, baseIndemnizacion, baseVariable, codigoDT, codigoPrevired, habilitado) " +
                    "values " +
                    "(" + opciones.haber + ", '" + opciones.descripcion + "', '" + opciones.imponible + "', '" + opciones.tributable + "', " + opciones.numeromeses +
                    ", '" + opciones.garantizado + "', '" + opciones.retenible + "', '" + opciones.calculado + "', '" + opciones.tiempo + "', '" + opciones.deducible +
                    "', '" + opciones.baselicencia + "', '" + opciones.basesobretiempo + "', '" + opciones.baseindemnizacion+ "', '" +  opciones.basevariable +
                    "', '" + opciones.codigoDT+ "', '" + opciones.codigoprevired +  "' ,1)  ";
                if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Haber ingresada de manera exitosa";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se ingreso la información de haber";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al guardar haber";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar haber en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar haber" + eG.Message.ToString();
                return resultado;
            }

        }
        public Resultado EditaHaberService(HaberBaseVM opciones, int empresa)
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
                    resultado.mensaje = "Debe indicar la descripción del habere";
                    return resultado;
                }


                string query = "update haberes set [haber]=" + opciones.haber + ",[descripcion]='" + opciones.descripcion +
                                   "', [imponible]='" + opciones.imponible + "',[tributable]='" + opciones.tributable + "',[garantizado]= '" + opciones.garantizado +
                                   "', [retenible]= '" + opciones.retenible + "', [calculado]= '" + opciones.calculado + "', [tiempo]= '" + opciones.tiempo +
                                   "', [deducible]= '" + opciones.deducible + "', [baseLicencia]= '" + opciones.baselicencia + "', [baseSobretiempo]= '" + opciones.basesobretiempo +
                                   "', [baseIndemnizacion]= '" + opciones.baseindemnizacion + "', [baseVariable]= '" + opciones.basevariable + "', [codigoDT]= '" + opciones.codigoDT +
                                   "', [codigoPrevired]= '" + opciones.codigoprevired +
                                   "' where haberes.id=" + opciones.id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Haber editada exitosamente.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se editó la información del haber.";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar haber";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar una haber en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar haber" + eG.Message.ToString();
                return resultado;
            }

        }

        public Resultado InhabilitaHaberService(HaberDeleteVM opciones, int empresa)
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
                    resultado.mensaje = "Debe indicar la informacion del habere que desea eliminar";
                    return resultado;
                }

                string query = "update haberes set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Haber eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó la información de la haber.";
                }

                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al eliminar haber";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar haber en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una haber" + eG.Message.ToString();
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
                var BD_Cli = "Remuneracion" ;

                f.EjecutarConsultaSQLCli("select * from parametrosGenerales Where habilitado = 1 and tabla= 'DTHABER' Order by descripcion", BD_Cli);

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

    }
}
