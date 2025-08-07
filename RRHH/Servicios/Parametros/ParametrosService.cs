using Microsoft.AspNetCore.Mvc;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.Parametros
{
    /// <summary>
    /// Servicio para generar y operar con las parametros de una empresa.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IParametrosService
    {
        /// <summary>
        /// Genera lista de parametros.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="tabla">Tabla de parametros que desea listar</param>
        /// <returns>Lista de jornadas</returns>
        public List<ParametrosBaseVM> ListarParametrosService(int idEmpresa, string tabla);

        /// <summary>
        /// Consulta por parametro por id.
        /// </summary>
        /// <param name="id">ID de parametro</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Muestra informacion de parametro</returns>
        public List<ParametrosBaseVM> ConsultaParametroIdService(int id, int idEmpresa);
        /// <summary>
        /// Creación de parametro.
        /// </summary>
        /// <param name="opciones">Registro de parametro</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns
        public Resultado CrearParametroService(ParametrosBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Edita parametro.
        /// </summary>
        /// <param name="opciones">Registro de parametro</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns>
        public Resultado EditaParametroService(ParametrosBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Inhabilitar parametro.
        /// </summary>
        /// <param name="id">ID de parametro</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita parametro</returns>
        public Resultado InhabilitaParametroService(ParametrosDeleteVM opciones, int idEmpresa);

        /// <summary>
        /// Carga lista de tablas en parametros.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de tablas</returns>
        public Resultado ComboTablasService(int idEmpresa);

    }

    public class ParametrosService : IParametrosService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public ParametrosService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<ParametrosBaseVM> ListarParametrosService(int empresa, string tabla)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT parametros.id,parametros.tabla,parametros.codigo,parametros.descripcion,parametros.valor,parametros.fecha, " +
                                            "parametros.inicio, parametros.termino " +
                                            "FROM parametros " +
                                            "WHERE parametros.habilitado = 1 and parametros.tabla =" + tabla.Trim() , BD_Cli);


                List<ParametrosBaseVM> opcionesList = new List<ParametrosBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {
                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new ParametrosBaseVM()
                                    {
                                        Id = int.Parse(dr["id"].ToString()),
                                        Codigo = dr["codigo"].ToString(),
                                        Tabla = dr["tabla"].ToString(),
                                        Descripcion = dr["Descripcion"].ToString(),
                                        Valor = dr["valor"].ToString(),
                                        Fecha = dr["fecha"].ToString(),
                                        Inicio = dr["inicio"].ToString(),
                                        Termino = dr["termino"].ToString()
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        DateTime inicio = Convert.ToDateTime(r.Inicio);
                        r.Inicio = inicio.ToString("dd'-'MM'-'yyyy");
                        DateTime termino = Convert.ToDateTime(r.Termino);
                        r.Termino = termino.ToString("dd'-'MM'-'yyyy");
                        DateTime fecha = Convert.ToDateTime(r.Fecha);
                        r.Fecha = fecha.ToString("dd'-'MM'-'yyyy");
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
                var Asunto = "Error al Consultar parametros";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar parametros", correos.destinatarioErrores);
                return null;
            }
        }
        public List<ParametrosBaseVM> ConsultaParametroIdService(int id, int empresa)
        {
            try
            {
                 string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT parametros.id,parametros.tabla,parametros.codigo,parametros.descripcion,parametros.valor,parametros.fecha, " +
                                            "parametros.inicio, parametros.termino " +
                                            "FROM parametros " +
                                            " where parametros.id =" + id + " and parametros.habilitado = 1 ", BD_Cli);
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
                    foreach (var r in opcionesList)
                    {

                        DateTime inicio = Convert.ToDateTime(r.Inicio);
                        r.Inicio = inicio.ToString("yyyy'-'MM'-'dd");
                        DateTime termino = Convert.ToDateTime(r.Termino);
                        r.Termino = termino.ToString("yyyy'-'MM'-'dd");
                        DateTime fecha = Convert.ToDateTime(r.Fecha);
                        r.Fecha = fecha.ToString("yyyy'-'MM'-'dd");
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
                var Asunto = "Error al Consultar parametro";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar parametro", correos.destinatarioErrores);
                return null;
            }
        }


        public Resultado CrearParametroService(ParametrosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            opciones.Descripcion = opciones.Descripcion?.ToString() ?? string.Empty;
            opciones.Valor = opciones.Valor?.ToString() ?? string.Empty;
            opciones.Fecha = opciones.Fecha?.ToString() ?? string.Empty;

            try
            {
                if (opciones.Descripcion == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la descripción del parametro";
                    return resultado;
                }

                string query2 = "insert into parametros (tabla,codigo,descripcion,valor,fecha,inicio,termino,habilitado) " +
                "values " +
                "('" + opciones.Tabla + "','" + opciones.Codigo + "','" + opciones.Descripcion + "','" + opciones.Valor + "','" + opciones.Fecha +
                "', '" + opciones.Inicio + "','" + opciones.Termino + "' ,1) ! ";
                if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Parametro ingresada de manera exitosa";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se ingreso la información de parametro";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al guardar parametro";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar parametro en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar parametro" + eG.Message.ToString();
                return resultado;
            }

        }
        public Resultado EditaParametroService(ParametrosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            opciones.Descripcion = opciones.Descripcion?.ToString() ?? string.Empty;
            opciones.Valor = opciones.Valor?.ToString() ?? string.Empty;
            opciones.Fecha = opciones.Fecha?.ToString() ?? string.Empty;


            try
            {
                if (opciones.Descripcion == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la descripción del parametro";
                    return resultado;
                }


                string query = "update parametros set [tabla]='" + opciones.Tabla + "' ,[codigo]='" + opciones.Codigo + "' ,[descripcion]='" + opciones.Descripcion + "', " +
                                   " [valor]='" + opciones.Valor + "',[fecha]='" + opciones.Fecha + "',[inicio]= '" + opciones.Inicio +
                                   "', [termino]= '" + opciones.Termino + "' " +
                                    " where parametros.id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Parametro editado exitosamente.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se editó la información del parametro.";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar parametro";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar un parametro en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar parametro" + eG.Message.ToString();
                return resultado;
            }
        }

        public Resultado InhabilitaParametroService(ParametrosDeleteVM opciones, int empresa)
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
                    resultado.mensaje = "Debe indicar la informacion del parametro que desea eliminar";
                    return resultado;
                }

                string query = "update parametros set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Parametro eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó la información de la parametro.";
                }

                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al eliminar parametro";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar parametro en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una parametro" + eG.Message.ToString();
                return resultado;
            }
        }
        public Resultado ComboTablasService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            f.EjecutarConsultaSQLCli("SELECT parametros.id,parametros.tabla,parametros.codigo,parametros.descripcion,parametros.valor,parametros.fecha, " +
                                        "parametros.inicio, parametros.termino " +
                                        "FROM parametros " +
                                        " where parametros.habilitado = 9 ", BD_Cli);

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
                data.Add(new Conceptos() { Codigo = Convert.ToString(h.Tabla), Descripcion = h.Descripcion });
            }
            resultado.result = 1;
            resultado.data = data;
            return resultado;
        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class Conceptos
    {

        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }
}

