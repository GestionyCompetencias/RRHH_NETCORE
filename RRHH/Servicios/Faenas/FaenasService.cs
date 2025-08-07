using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;
using Microsoft.AspNetCore.Mvc;

namespace RRHH.Servicios.Faenas
{
    /// <summary>
    /// Servicio para generar y operar con las faenas de una empresa.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IFaenasService
    {
        /// <summary>
        /// Genera lista de faenas.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de faenas</returns>
        public List<FaenasBaseVM> ListarFaenasService(int idEmpresa);

        /// <summary>
        /// Consulta por id faena.
        /// </summary>
        /// <param name="id">ID de cuenta especial</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Muestra informacion de faena</returns>
        public List<FaenasBaseVM> ConsultaFaenaIdService(int id, int idEmpresa);
        /// <summary>
        /// Creación de faena.
        /// </summary>
        /// <param name="opciones">Registro de faena</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns
        public Resultado CrearFaenaService(FaenasBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Edita faena.
        /// </summary>
        /// <param name="opciones">Registro de faena</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns>
        public Resultado EditaFaenaService(FaenasBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Inhabilitar cuenta especial.
        /// </summary>
        /// <param name="id">ID de cuenta especial</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita cuenta especial</returns>
        public Resultado InhabilitaFaenaService(FaenasDeleteVM opciones, int idEmpresa);

    }

    public class FaenasService : IFaenasService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public FaenasService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<FaenasBaseVM> ListarFaenasService(int empresa)
        {
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                f.EjecutarConsultaSQLCli("SELECT faenas.id,faenas.contrato,faenas.descripcion,faenas.inicio,faenas.termino,faenas.direccion, " +
                                            "faenas.idpais, faenas.idregion, faenas.idcomuna  " +
                                            "FROM faenas " +
                                            "WHERE faenas.habilitado = 1 ", BD_Cli);

                List<FaenasBaseVM> opcionesList = new List<FaenasBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new FaenasBaseVM()
                                    {
                                        Id = int.Parse(dr["id"].ToString()),
                                        Contrato = dr["Contrato"].ToString(),
                                        Descripcion = dr["Descripcion"].ToString(),
                                        Inicio = dr["inicio"].ToString(),
                                        Termino = dr["termino"].ToString(),
                                        Direccion = dr["direccion"].ToString(),
                                        idPais = int.Parse(dr["idpais"].ToString()),
                                        idRegion = int.Parse(dr["idregion"].ToString()),
                                        idComuna = int.Parse(dr["idcomuna"].ToString())
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {

                        DateTime inicio = Convert.ToDateTime(r.Inicio);
                        r.Inicio = inicio.ToString("dd'-'MM'-'yyyy");
                        DateTime termino = Convert.ToDateTime(r.Termino);
                        r.Termino = termino.ToString("dd'-'MM'-'yyyy");
                        r.Comuna = Generales.BuscaComuna(r.idComuna);
                        r.Region = Generales.BuscaRegion(r.idRegion);
                        r.Pais = Generales.BuscaPais(r.idPais);
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
                var Asunto = "Error al Consultar faenas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar faenas", correos.destinatarioErrores);
                return null;
            }

        }
        public List<FaenasBaseVM> ConsultaFaenaIdService(int id, int empresa)
        {

            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT faenas.id,faenas.contrato,faenas.descripcion,faenas.inicio,faenas.termino,faenas.direccion, " +
                                            "faenas.idpais, faenas.idregion, faenas.idcomuna  " +
                                            "FROM faenas " +
                                            " where faenas.id ='" + id + "' and faenas.habilitado = 1 ", BD_Cli);

                List<FaenasBaseVM> opcionesList = new List<FaenasBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new FaenasBaseVM()
                                    {
                                        Id = int.Parse(dr["id"].ToString()),
                                        Contrato = dr["Contrato"].ToString(),
                                        Descripcion = dr["Descripcion"].ToString(),
                                        Inicio = dr["inicio"].ToString(),
                                        Termino = dr["termino"].ToString(),
                                        Direccion = dr["direccion"].ToString(),
                                        idPais = int.Parse(dr["idpais"].ToString()),
                                        idRegion = int.Parse(dr["idregion"].ToString()),
                                        idComuna = int.Parse(dr["idcomuna"].ToString())
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        DateTime inicio = Convert.ToDateTime(r.Inicio);
                        r.Inicio = inicio.ToString("yyyy'-'MM'-'dd");
                        DateTime termino = Convert.ToDateTime(r.Termino);
                        r.Termino = termino.ToString("yyyy'-'MM'-'dd");
                        r.Comuna = Generales.BuscaComuna(r.idComuna);
                        r.Region = Generales.BuscaRegion(r.idRegion);
                        r.Pais = Generales.BuscaPais(r.idPais);
                    }
                    return opcionesList;
                }
                else
                {
                }
                return null;
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar faena";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar faena", correos.destinatarioErrores);

                return null;
            }
        }


        public Resultado CrearFaenaService(FaenasBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;


            opciones.Region = opciones.Region?.ToString() ?? string.Empty;
            opciones.Descripcion = opciones.Descripcion?.ToString() ?? string.Empty;
            opciones.Direccion = opciones.Direccion?.ToString() ?? string.Empty;



            try
            {
                if (opciones.Descripcion == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la descripción de la faena";
                    return resultado;
                }

                string query2 = "insert into faenas (contrato,descripcion,inicio,termino,direccion,idpais,idregion,idcomuna,habilitado) " +
                "values " +
                "('" + opciones.Contrato + "','" + opciones.Descripcion + "','" + opciones.Inicio + "','" + opciones.Termino + "','" + opciones.Direccion +
                "', " + opciones.idPais + "," + opciones.idRegion + "," + opciones.idComuna + " ,1) ! ";
                if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Faena ingresada de manera exitosa";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se ingreso la información de faena";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Guardar faena";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar faena en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar faena" + eG.Message.ToString();
                return resultado;
            }

        }
        public Resultado EditaFaenaService(FaenasBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;


            opciones.Region = opciones.Region?.ToString() ?? string.Empty;
            opciones.Descripcion = opciones.Descripcion?.ToString() ?? string.Empty;
            opciones.Direccion = opciones.Direccion?.ToString() ?? string.Empty;


            try
            {
                if (opciones.Descripcion == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la descripción de la faena";
                    return resultado;
                }


                string query = "update faenas set [idregion]=" + opciones.idRegion + ",[idcomuna]=" + opciones.idComuna + ",[direccion]='" + opciones.Direccion + "', " +
                                   " [inicio]='" + opciones.Inicio + "',[termino]='" + opciones.Termino + "',[idpais]= " + opciones.idPais +
                                   ", [descripcion]= '" + opciones.Descripcion + "', [contrato]= " + opciones.Contrato +
                                    " where faenas.id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Faena editada exitosamente.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se editó la información de la faena.";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar faena";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar una faena en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar faena" + eG.Message.ToString();
                return resultado;
            }

        }

        public Resultado InhabilitaFaenaService(FaenasDeleteVM opciones, int empresa)
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
                    resultado.mensaje = "Debe indicar la informacion de la faena que desea eliminar";
                    return resultado;
                }

                string query = "update faenas set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Faena eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó la información de la faena.";
                }

                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al eliminar faena";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar faena en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una faena" + eG.Message.ToString();
                return resultado;
            }
        }
    }
}
