using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.Cargos
{
    /// <summary>
    /// Servicio para generar y operar con las cargos de una empresa.
    /// Ejem. Oficina central.
    /// </summary>
    public interface ICargosService
    {
        /// <summary>
        /// Genera lista de cargos.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de cargos</returns>
        public List<DetCargos> ListarCargosService(int idEmpresa);

        /// <summary>
        /// Consulta por id cargo.
        /// </summary>
        /// <param name="id">ID cargo">ID de una empresa.</param>
        /// <returns>Muestra informacion de cargo</returns>
        public List<CargosBaseVM> ConsultaCargoIdService(int id, int idEmpresa);
        /// <summary>
        /// Creación de cargo.
        /// </summary>
        /// <param name="opciones">Registro de cargo</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns
        public Resultado CrearCargoService(CargosBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Edita cargo.
        /// </summary>
        /// <param name="opciones">Registro de cargo</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns>
        public Resultado EditaCargoService(CargosBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Inhabilitar cargo.
        /// </summary>
        /// <param name="id">ID de cargo</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita cargo</returns>
        public Resultado InhabilitaCargoService(CargosDeleteVM opciones, int idEmpresa);

    }

    public class CargosService : ICargosService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public CargosService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetCargos> ListarCargosService(int empresa)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT cargos.id,cargos.codigo,cargos.descripcion,cargos.inicio,cargos.termino,cargos.requisitos, " +
                                            "cargos.funciones, cargos.sueldoMin, cargos.sueldoMax  " +
                                            "FROM cargos " +
                                            "WHERE cargos.habilitado = 1 ", BD_Cli);


                List<DetCargos> opcionesList = new List<DetCargos>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetCargos()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        codigo = dr["Codigo"].ToString(),
                                        descripcion = dr["Descripcion"].ToString(),
                                        inicio = dr["inicio"].ToString(),
                                        termino = dr["termino"].ToString(),
                                        requisitos = dr["requisitos"].ToString(),
                                        funciones = dr["funciones"].ToString(),
                                        sueldoMinimo = dr["sueldoMin"].ToString(),
                                        sueldoMaximo = dr["sueldoMax"].ToString()
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {

                        DateTime inicio = Convert.ToDateTime(r.inicio);
                        r.inicio = inicio.ToString("dd'-'MM'-'yyyy");
                        DateTime termino = Convert.ToDateTime(r.termino);
                        r.termino = termino.ToString("dd'-'MM'-'yyyy");
                        int minimo = Convert.ToInt32(r.sueldoMinimo);
                        r.sueldoMinimo = minimo.ToString("###,###,###");
                        int maximo = Convert.ToInt32(r.sueldoMaximo);
                        r.sueldoMaximo = maximo.ToString("###,###,###");
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
                var Asunto = "Error al Consultar cargos";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar cargos", correos.destinatarioErrores);
                return null;
            }

        }
        public List<CargosBaseVM> ConsultaCargoIdService(int id, int empresa)
        {

            try
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT cargos.id,cargos.codigo,cargos.descripcion,cargos.inicio,cargos.termino,cargos.requisitos, " +
                                            "cargos.funciones, cargos.sueldoMin, cargos.sueldoMax  " +
                                            "FROM cargos " +
                                            " where cargos.id =" + id + " and cargos.habilitado = 1 ", BD_Cli);

                List<CargosBaseVM> opcionesList = new List<CargosBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new CargosBaseVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        codigo = dr["Codigo"].ToString(),
                                        descripcion = dr["Descripcion"].ToString(),
                                        inicio = dr["inicio"].ToString(),
                                        termino = dr["termino"].ToString(),
                                        requisitos = dr["requisitos"].ToString(),
                                        funciones = dr["funciones"].ToString(),
                                        sueldoMinimo = int.Parse(dr["sueldoMin"].ToString()),
                                        sueldoMaximo = int.Parse(dr["sueldoMax"].ToString())
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {

                        DateTime inicio = Convert.ToDateTime(r.inicio);
                        r.inicio = inicio.ToString("yyyy'-'MM'-'dd");
                        DateTime termino = Convert.ToDateTime(r.termino);
                        r.termino = termino.ToString("yyyy'-'MM'-'dd");
                    }
                    return opcionesList;

                }
                return null;
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar cargo";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar cargo", correos.destinatarioErrores);

                return null;
            }
        }


        public Resultado CrearCargoService(CargosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            opciones.requisitos = opciones.requisitos?.ToString() ?? string.Empty;
            opciones.funciones = opciones.funciones?.ToString() ?? string.Empty;

            try
            {
                if (opciones.descripcion == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la descripción del cargo";
                    return resultado;
                }
                opciones.descripcion = opciones.descripcion.ToUpper();
                string query2 = "insert into cargos (codigo,descripcion,inicio,termino,requisitos,funciones,sueldoMin,sueldoMax,habilitado) " +
                "values " +
                "('" + opciones.codigo + "','" + opciones.descripcion + "','" + opciones.inicio + "','" + opciones.termino + "','" + opciones.requisitos +
                "', '" + opciones.funciones + "', " + opciones.sueldoMinimo + "," + opciones.sueldoMaximo + " ,1) ! ";
                if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Cargo ingresada de manera exitosa";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se ingreso la información de cargo";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Guardar cargo";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar cargo en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar cargo" + eG.Message.ToString();
                return resultado;
            }

        }
        public Resultado EditaCargoService(CargosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            opciones.descripcion = opciones.descripcion?.ToString() ?? string.Empty;
            opciones.funciones = opciones.funciones?.ToString() ?? string.Empty;

            try
            {
                if (opciones.descripcion == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la descripción del cargo";
                    return resultado;
                }


                string query = "update cargos set [codigo]='" + opciones.codigo + "',[descripcion]='" + opciones.descripcion +
                                   "', [inicio]='" + opciones.inicio + "',[termino]='" + opciones.termino + "',[requisitos]= '" + opciones.requisitos +
                                   "', [funciones]= '" + opciones.funciones + "', [sueldoMin]= " + opciones.sueldoMinimo + ", [sueldoMax]= " + opciones.sueldoMaximo +
                                    " where cargos.id=" + opciones.id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Cargo editada exitosamente.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se editó la información del cargo.";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar cargo";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar una cargo en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar cargo" + eG.Message.ToString();
                return resultado;
            }

        }

        public Resultado InhabilitaCargoService(CargosDeleteVM opciones, int empresa)
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
                    resultado.mensaje = "Debe indicar la informacion del cargo que desea eliminar";
                    return resultado;
                }

                string query = "update cargos set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Cargo eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó la información de la cargo.";
                }

                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al eliminar cargo";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar cargo en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una cargo" + eG.Message.ToString();
                return resultado;
            }
        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetCargos
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