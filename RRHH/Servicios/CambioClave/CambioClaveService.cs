using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.CambioClave
{
    /// <summary>
    /// Servicio para generar y operar con las cargos de una empresa.
    /// Ejem. Oficina central.
    /// </summary>
    public interface ICambioClaveService
    {
        /// <summary>
        /// Actualiza cambop de clave.
        /// </summary>
        /// <param name="antigua">Clve antigua.</param>
        /// <param name="nueva">Clve nueva</param>
        /// <param name="verifica">Clve de verificacion.</param>
        /// <param name="usuario">Usuario</param>
        /// <returns>Resultado</returns>
        public Resultado CambiarClaveService(string antigua, string nueva, string verifica,string usuario);


    }

    public class CambioClaveService : ICambioClaveService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public CambioClaveService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }


        public Resultado CambiarClaveService(string antigua, string nueva, string verifica, string usuario)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {
                var BD_Cli = "contable";

                f.EjecutarConsultaSQLCli("SELECT usuarios.contra " +
                                            "FROM usuarios " +
                                            "WHERE usuarios.id = " + usuario, BD_Cli);


                if (f.Tabla.Rows.Count > 0)
                {
                    DataRow dr = f.Tabla.Rows[0];
                    string clave = dr["Contra"].ToString();
                    if (clave == antigua)
                    {
                        if (verifica == nueva)
                        {
                            string query = "update usuarios set [contra]='" + nueva + "' " +
                                           " where usuarios.id=" + usuario + "  ! ";
                            if (f.EjecutarQuerySQLCli(query, BD_Cli))
                            {
                                resultado.mensaje = "Cambio de clave exitoso";
                                resultado.result = 1;
                            }
                            else
                            {
                                resultado.mensaje = "Error en cambio de clave";
                                resultado.result = 0;
                            }
                        }
                        else
                        {
                            resultado.mensaje = "Clave nueva no coincide";
                            resultado.result = 0;
                        }


                    }
                    else
                    {
                        resultado.mensaje = "Contraseña antigua no corresponde";
                        resultado.result = 0;

                    }
                }
                else
                {
                    resultado.mensaje = "No se han encontrado usuario";
                    resultado.result = 0;
                }
                return resultado ;
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Cambiar clave";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar cambiar clave", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return null;
            }


        }

    }
}
