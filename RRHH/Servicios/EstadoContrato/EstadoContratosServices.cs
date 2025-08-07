using Microsoft.AspNetCore.Mvc;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;
using System.Diagnostics.Contracts;

namespace RRHH.Servicios.EstadoContratos
{
    /// <summary>
    /// Servicio para consultar por los contratos.

    /// </summary>
    public interface IEstadoContratosService
    {
        /// <summary>
        /// Genera lista de contratos.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="Estado">Estado del contrato</param>
        /// <returns>Lista de contratos</returns>
        public List<ContratosBaseVM> ListarContratosService(int idEmpresa, int estado);

        /// <summary>
        /// Consulta por id contrato.
        /// </summary>
        /// <param name="id">ID contrato">ID de una empresa.</param>
        /// <returns>Muestra informacion de contrato</returns>
        public List<ContratosBaseVM> ConsultaContratoIdService(int id, int idEmpresa);
 
    }

    public class EstadoContratosService : IEstadoContratosService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public EstadoContratosService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<ContratosBaseVM> ListarContratosService(int empresa, int estado)
        {
            try
            {

                String RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                DateTime hoy = DateTime.Now.Date;
                string hoystr = hoy.ToString("yyyy'-'MM'-'dd");

                if(estado == 1)
                {
                    f.EjecutarConsultaSQLCli("SELECT personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf, " +
                                                "contratos.id , contratos.contrato, contratos.idpersona, contratos.sueldoBase, " +
                                                "contratos.inicio, contratos.termino, contratos.idTipoContrato, contratos.idfaena,  " +
                                                "contratos.idcargo,  contratos.idcentroCosto, contratos.idjornada,  contratos.observaciones,  " +
                                                "contratos.firmaTrabajador,  contratos.firmaEmpresa, contratos.rutValidador,  contratos.fechaValidacion,  " +
                                                "contratos.rechazado,  contratos.fechaCreacion, contratos.tipoCarga,  contratos.articulo22  " +
                                               "FROM personas " +
                                                "inner join contratos on personas.id = contratos.idPersona " +
                                                "where contratos.habilitado = 1 and contratos.firmaTrabajador = 1 and contratos.firmaempresa =1"+
                                                " and contratos.rechazado =0 and contratos.inicio < '"+hoystr+"' and contratos.termino > '"+hoystr+"' ", BD_Cli);

                }
                if (estado == 2)
                {
                    f.EjecutarConsultaSQLCli("SELECT personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf, " +
                                                "contratos.id , contratos.contrato, contratos.idpersona, contratos.sueldoBase, " +
                                                "contratos.inicio, contratos.termino, contratos.idTipoContrato, contratos.idfaena,  " +
                                                "contratos.idcargo,  contratos.idcentroCosto, contratos.idjornada,  contratos.observaciones,  " +
                                                "contratos.firmaTrabajador,  contratos.firmaEmpresa, contratos.rutValidador,  contratos.fechaValidacion,  " +
                                                "contratos.rechazado,  contratos.fechaCreacion, contratos.tipoCarga,  contratos.articulo22  " +
                                               "FROM personas " +
                                                "inner join contratos on personas.id = contratos.idPersona " +
                                                "where contratos.habilitado = 1 and contratos.firmaTrabajador = 1 and contratos.firmaempresa =1" +
                                                " and contratos.rechazado =0 and contratos.inicio < '" + hoystr + "' and contratos.termino < '" + hoystr + "' ", BD_Cli);

                }
                if (estado == 3)
                {
                    f.EjecutarConsultaSQLCli("SELECT personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf, " +
                                                "contratos.id , contratos.contrato, contratos.idpersona, contratos.sueldoBase, " +
                                                "contratos.inicio, contratos.termino, contratos.idTipoContrato, contratos.idfaena,  " +
                                                "contratos.idcargo,  contratos.idcentroCosto, contratos.idjornada,  contratos.observaciones,  " +
                                                "contratos.firmaTrabajador,  contratos.firmaEmpresa, contratos.rutValidador,  contratos.fechaValidacion,  " +
                                                "contratos.rechazado,  contratos.fechaCreacion, contratos.tipoCarga,  contratos.articulo22  " +
                                               "FROM personas " +
                                                "inner join contratos on personas.id = contratos.idPersona " +
                                                "where contratos.habilitado = 1 and contratos.firmaTrabajador = 1 and contratos.firmaempresa =0" +
                                                " and contratos.rechazado =0 and contratos.inicio < '" + hoystr + "' and contratos.termino < '" + hoystr + "' ", BD_Cli);

                }
                if (estado == 4)
                {
                    f.EjecutarConsultaSQLCli("SELECT personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf, " +
                                                "contratos.id , contratos.contrato, contratos.idpersona, contratos.sueldoBase, " +
                                                "contratos.inicio, contratos.termino, contratos.idTipoContrato, contratos.idfaena,  " +
                                                "contratos.idcargo,  contratos.idcentroCosto, contratos.idjornada,  contratos.observaciones,  " +
                                                "contratos.firmaTrabajador,  contratos.firmaEmpresa, contratos.rutValidador,  contratos.fechaValidacion,  " +
                                                "contratos.rechazado,  contratos.fechaCreacion, contratos.tipoCarga,  contratos.articulo22  " +
                                               "FROM personas " +
                                                "inner join contratos on personas.id = contratos.idPersona " +
                                                "where contratos.habilitado = 1 and contratos.firmaTrabajador = 0 and contratos.firmaempresa =0" +
                                                " and contratos.rechazado =0 and contratos.inicio < '" + hoystr + "' and contratos.termino < '" + hoystr + "' ", BD_Cli);

                }
                if (estado == 4)
                {
                    f.EjecutarConsultaSQLCli("SELECT personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf, " +
                                                "contratos.id , contratos.contrato, contratos.idpersona, contratos.sueldoBase, " +
                                                "contratos.inicio, contratos.termino, contratos.idTipoContrato, contratos.idfaena,  " +
                                                "contratos.idcargo,  contratos.idcentroCosto, contratos.idjornada,  contratos.observaciones,  " +
                                                "contratos.firmaTrabajador,  contratos.firmaEmpresa, contratos.rutValidador,  contratos.fechaValidacion,  " +
                                                "contratos.rechazado,  contratos.fechaCreacion, contratos.tipoCarga,  contratos.articulo22  " +
                                               "FROM personas " +
                                                "inner join contratos on personas.id = contratos.idPersona " +
                                                "where contratos.habilitado = 1 " +
                                                " and contratos.rechazado =1 and contratos.inicio < '" + hoystr + "' and contratos.termino < '" + hoystr + "' ", BD_Cli);

                }


                List<ContratosBaseVM> opcionesList = new List<ContratosBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new ContratosBaseVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        rut = dr["rut"].ToString(),
                                        nombres = dr["nombres"].ToString(),
                                        apellidos = dr["apellidos"].ToString(),
                                        idtipocontrato = int.Parse(dr["idTipoContrato"].ToString()),
                                        contrato = dr["contrato"].ToString(),
                                        inicio = dr["inicio"].ToString(),
                                        termino = dr["termino"].ToString(),
                                        idfaena = int.Parse(dr["idFaena"].ToString()),
                                        idcargo = int.Parse(dr["idCargo"].ToString()),
                                        idcentrocosto = int.Parse(dr["idCentroCosto"].ToString()),
                                        idjornada = int.Parse(dr["idJornada"].ToString()),
                                        sueldobase = dr["sueldoBase"].ToString(),
                                        observaciones = dr["observaciones"].ToString(),
                                        firmatrabajador = dr["firmaTrabajador"].ToString(),
                                        firmaempresa = dr["firmaEmpresa"].ToString(),
                                        tipocarga = dr["tipoCarga"].ToString(),
                                        articulo22 = dr["articulo22"].ToString()
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {

                        DateTime inicio = Convert.ToDateTime(r.inicio);
                        r.inicio = inicio.ToString("dd'-'MM'-'yyyy");
                        DateTime termino = Convert.ToDateTime(r.termino);
                        r.termino = termino.ToString("dd'-'MM'-'yyyy");
                        r.tipocontrato = Generales.BuscaTipoContrato(r.idtipocontrato, BD_Cli);
                        FaenasBaseVM faen = Generales.BuscaFaena(r.idfaena, BD_Cli);
                        r.faena = faen.Descripcion;
                        r.cargo = Generales.BuscaCargo(r.idcargo, BD_Cli);
                        r.centrocosto = Generales.BuscaCentroCosto(r.idcentrocosto, BD_Cli);
                        r.jornada = Generales.BuscaJornada(r.idjornada, BD_Cli);
                        r.rut = r.rut.Substring(0, r.rut.Length - 1) + '-' + r.rut.Substring(r.rut.Length-1, 1);
                    }

                    return opcionesList;
                }
                return  null ;
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar contratos";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar contratos", correos.destinatarioErrores);
                return null;
            }


        }
        public List<ContratosBaseVM> ConsultaContratoIdService(int id, int empresa)
        {

            try
            {


                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf, " +
                                            "contratos.id , contratos.contrato, contratos.idpersona,  " +
                                            "contratos.inicio, contratos.termino, contratos.idTipoContrato, contratos.idfaena,  " +
                                            "contratos.idcargo,  contratos.idcentroCosto, contratos.idjornada,  contratos.observaciones, contratos.sueldoBase,  " +
                                            "contratos.firmaTrabajador,  contratos.firmaEmpresa, contratos.rutValidador,  contratos.fechaValidacion,  " +
                                            "contratos.rechazado,  contratos.fechaCreacion, contratos.tipoCarga,  contratos.articulo22  " +
                                           "FROM personas " +
                                            "inner join contratos on personas.id = contratos.idPersona " +
                                            "where contratos.habilitado = 1 and  contratos.id = " + id, BD_Cli);


                List<ContratosBaseVM> opcionesList = new List<ContratosBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new ContratosBaseVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        rut = dr["rut"].ToString(),
                                        nombres = dr["nombres"].ToString(),
                                        apellidos = dr["apellidos"].ToString(),
                                        idtipocontrato = int.Parse(dr["idTipoContrato"].ToString()),
                                        contrato = dr["contrato"].ToString(),
                                        inicio = dr["inicio"].ToString(),
                                        termino = dr["termino"].ToString(),
                                        idfaena = int.Parse(dr["idFaena"].ToString()),
                                        idcargo = int.Parse(dr["idCargo"].ToString()),
                                        idcentrocosto = int.Parse(dr["idCentroCosto"].ToString()),
                                        idjornada = int.Parse(dr["idJornada"].ToString()),
                                        sueldobase = dr["sueldoBase"].ToString(),
                                        observaciones = dr["observaciones"].ToString(),
                                        firmatrabajador = dr["firmaTrabajador"].ToString(),
                                        firmaempresa = dr["firmaEmpresa"].ToString(),
                                        tipocarga = dr["tipoCarga"].ToString(),
                                        articulo22 = dr["articulo22"].ToString()
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {

                        DateTime inicio = Convert.ToDateTime(r.inicio);
                        r.inicio = inicio.ToString("yyyy'-'MM'-'dd");
                        DateTime termino = Convert.ToDateTime(r.termino);
                        r.termino = termino.ToString("yyyy'-'MM'-'dd");
                        r.tipocontrato = Generales.BuscaTipoContrato(r.idtipocontrato, BD_Cli);
                        FaenasBaseVM faen = Generales.BuscaFaena(r.idfaena, BD_Cli);
                        r.faena= faen.Descripcion;
                        r.cargo = Generales.BuscaCargo(r.idcargo, BD_Cli);
                        r.centrocosto = Generales.BuscaCentroCosto(r.idcentrocosto, BD_Cli);
                        r.jornada = Generales.BuscaJornada(r.idjornada, BD_Cli);
                        f.EjecutarConsultaSQLCli("select * from personas where rut='" + r.rut + "'", BD_Cli);
                        if (f.Tabla.Rows.Count > 0)
                        {
                            DataRow pers = f.Tabla.Rows[0];
                            r.nombres = pers["nombres"].ToString();
                            r.apellidos = pers["apellidos"].ToString();
                        }
                        return opcionesList;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar contratos";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar un cliente", correos.destinatarioErrores);
                return null;
            }

        }
    }
}
