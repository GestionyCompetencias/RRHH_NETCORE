using Microsoft.AspNetCore.Mvc;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;
using System.Diagnostics.Contracts;

namespace RRHH.Servicios.Contratos
{
    /// <summary>
    /// Servicio para generar y operar con las contratos de una empresa.

    /// </summary>
    public interface IContratosService
    {
        /// <summary>
        /// Genera lista de contratos.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de contratos</returns>
        public List<ContratosBaseVM> ListarContratosService(int idEmpresa);

        /// <summary>
        /// Consulta por id contrato.
        /// </summary>
        /// <param name="id">ID contrato">ID de contrato.</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Muestra informacion de contrato</returns>
        public DetalleContrato ConsultaContratoIdService(int id, int idEmpresa);

        /// <summary>
        /// Consulta por RUT contrato.
        /// </summary>
        /// <param name="rut">ID contrato">ID del trabajador.</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Muestra informacion de contrato</returns>
        public DetalleContrato ConsultaContratoRUTService(string rut, int idEmpresa);

        /// <summary>
        /// Creación de contrato.
        /// </summary>
        /// <param name="opciones">Registro de contrato</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns
        public Resultado CrearContratoService(DetalleContrato opciones, int idEmpresa);

        /// <summary>
        /// Edita contrato.
        /// </summary>
        /// <param name="opciones">Registro de contrato</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns>
        public Resultado EditaContratoService(ContratosBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Inhabilitar contrato.
        /// </summary>
        /// <param name="id">ID de contrato</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita contrato</returns>
        public Resultado InhabilitaContratoService(ContratosDeleteVM opciones, int idEmpresa);

        /// <summary>
        /// Cargar tabla de tipos de contrato.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de tipos de contrato</returns>
        public Resultado ComboTiposContratoService(int idEmpresa);

        /// <summary>
        /// Cargar tabla de faenas.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de faenas</returns>
        public Resultado ComboFaenasService(int idEmpresa);

        /// <summary>
        /// Cargar tabla de cargos.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de cargos</returns>
        public Resultado ComboCargosService(int idEmpresa);

        /// <summary>
        /// Cargar tabla de centros de responsabilidad.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de centros</returns>
        public Resultado ComboCentrosService(int idEmpresa);

        /// <summary>
        /// Cargar tabla de jornadas.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de jornadas de trabajo</returns>
        public Resultado ComboJornadasService(int idEmpresa);

        /// <summary>
        /// Verifica la existencia de una persona.
        /// </summary>
        /// <param name="rut">Rut de la persona.</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de tipos de contrato</returns>
        public Resultado ExistePersonaService(string rut, int idEmpresa);

        /// <summary>
        /// Cargar tabla de bancos.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de bancos</returns>
        public Resultado ComboBancosService(int idEmpresa);

        /// <summary>
        /// Cargar tabla de afps.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de afps</returns>
        public Resultado ComboAfpsService(int idEmpresa);

        /// <summary>
        /// Cargar tabla de isapres.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de isapres</returns>
        public Resultado ComboIsapresService(int idEmpresa);

        /// <summary>
        /// Cargar tabla de tipos de cuentas.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de tipos de cuentas</returns>
        public Resultado ComboTiposCuentasService(int idEmpresa);
    }

    public class ContratosService : IContratosService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public ContratosService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<ContratosBaseVM> ListarContratosService(int empresa)
        {
            try
            {

                String RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf, " +
                                            "contratos.id , contratos.contrato, contratos.idpersona, contratos.sueldoBase, " +
                                            "contratos.inicio, contratos.termino, contratos.idTipoContrato, contratos.idfaena,  " +
                                            "contratos.idcargo,  contratos.idcentroCosto, contratos.idjornada,  contratos.observaciones,  " +
                                            "contratos.firmaTrabajador,  contratos.firmaEmpresa, contratos.rutValidador,  contratos.fechaValidacion,  " +
                                            "contratos.rechazado,  contratos.fechaCreacion, contratos.tipoCarga,  contratos.articulo22  " +
                                           "FROM personas " +
                                            "inner join contratos on personas.id = contratos.idPersona " +
                                            "where contratos.habilitado = 1 ", BD_Cli);


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
        public DetalleContrato ConsultaContratoIdService(int id, int empresa)
        {

            try
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf, " +
                                            "contratos.id , contratos.contrato, contratos.idpersona,  " +
                                            "contratos.inicio, contratos.termino, contratos.idTipoContrato, contratos.idfaena,  " +
                                            "contratos.idcargo,  contratos.idcentroCosto, contratos.idjornada, "+
                                            "contratos.idBancoTrab, contratos.idAfpTrab, contratos.idIsapreTrab, " +
                                            "contratos.observaciones, contratos.sueldoBase,  " +
                                            "contratos.firmaTrabajador,  contratos.firmaEmpresa, contratos.rutValidador,  contratos.fechaValidacion,  " +
                                            "contratos.rechazado,  contratos.fechaCreacion, contratos.tipoCarga,  contratos.articulo22  " +
                                           "FROM personas " +
                                            "inner join contratos on personas.id = contratos.idPersona " +
                                            "where contratos.habilitado = 1 and  contratos.id = " + id, BD_Cli);

                List<ContratosBaseVM> opcionesList = new List<ContratosBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {
                    DetalleContrato contrato = new DetalleContrato();
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
                                        idbancotrab = int.Parse(dr["idbancotrab"].ToString()),
                                        idafptrab = int.Parse(dr["idafptrab"].ToString()),
                                        idisapretrab = int.Parse(dr["idisapretrab"].ToString()),
                                        sueldobase = dr["sueldoBase"].ToString(),
                                        observaciones = dr["observaciones"].ToString(),
                                        firmatrabajador = dr["firmaTrabajador"].ToString(),
                                        firmaempresa = dr["firmaEmpresa"].ToString(),
                                        tipocarga = dr["tipoCarga"].ToString(),
                                        articulo22 = dr["articulo22"].ToString()
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        contrato.id = r.id;
                        contrato.rut = r.rut;
                        contrato.nombres = r.nombres;
                        contrato.apellidos = r.apellidos;
                        contrato.idtipocontrato = r.idtipocontrato;
                        contrato.contrato = r.contrato;
                        contrato.inicio = r.inicio;
                        contrato.termino = r.termino;
                        contrato.idfaena = r.idfaena;
                        contrato.idcargo = r.idcargo;
                        contrato.idcentrocosto = r.idcentrocosto;
                        contrato.idjornada = r.idjornada;
                        contrato.idisapretrab = r.idisapretrab;
                        contrato.idbancotrab = r.idbancotrab;
                        contrato.idafptrab = r.idafptrab;
                        contrato.sueldobase = r.sueldobase;
                        contrato.observaciones = r.observaciones;
                        contrato.firmatrabajador = r.firmatrabajador;
                        contrato.firmaempresa = r.firmaempresa;
                        contrato.tipocarga = r.tipocarga;
                        contrato.articulo22 = r.articulo22;
                        DateTime inicio = Convert.ToDateTime(r.inicio);
                        contrato.inicio = inicio.ToString("yyyy'-'MM'-'dd");
                        DateTime termino = Convert.ToDateTime(r.termino);
                        contrato.termino = termino.ToString("yyyy'-'MM'-'dd");
                        contrato.tipocontrato = Generales.BuscaTipoContrato(contrato.idtipocontrato, BD_Cli);
                        FaenasBaseVM faen = Generales.BuscaFaena(r.idfaena, BD_Cli);
                        contrato.faena = faen.Descripcion;
                        contrato.cargo = Generales.BuscaCargo(r.idcargo, BD_Cli);
                        contrato.centrocosto = Generales.BuscaCentroCosto(r.idcentrocosto, BD_Cli);
                        contrato.jornada = Generales.BuscaJornada(r.idjornada, BD_Cli);
                        f.EjecutarConsultaSQLCli("select * from personas where rut='" + r.rut + "'", BD_Cli);
                        if (f.Tabla.Rows.Count > 0)
                        {
                            DataRow pers = f.Tabla.Rows[0];
                            contrato.nombres = pers["nombres"].ToString();
                            contrato.apellidos = pers["apellidos"].ToString();
                        }
                        BancosTrabajadorBaseVM banco = Generales.BuscaBancoTrabajador(r.idbancotrab, BD_Cli);
                        if (banco != null)
                        {
                            contrato.idbanco = banco.idbanco;
                            contrato.descripcionbanco = banco.descripcionBanco;
                            contrato.idtipocuenta = banco.idtipocta;
                            contrato.numerocuenta = banco.numerocuenta;
                        }
                        AfpsTrabajadorBaseVM afp = Generales.BuscaAfpTrabajador(r.idafptrab, BD_Cli);
                        if (afp != null)
                        {
                            contrato.codigoafp = afp.codigoAfp;
                            contrato.descripcionafp = afp.descripcion;
                            contrato.tipoapv = afp.tipoApv;
                            contrato.apv = afp.apv.ToString();
                        }
                        IsapresTrabajadorBaseVM isapre = Generales.BuscaIsapreTrabajador(r.idisapretrab, BD_Cli);
                        if (isapre != null)
                        {
                            contrato.codigoisapre = isapre.codigoisapre;
                            contrato.descripcionisapre = isapre.descripcion;
                            contrato.ufs = isapre.numeroUf.ToString();
                        }
                    }
                    return contrato;
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

        public DetalleContrato ConsultaContratoRUTService(string rut, int empresa)
        {

            try
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf, " +
                                            "contratos.id , contratos.contrato, contratos.idpersona,  " +
                                            "contratos.inicio, contratos.termino, contratos.idTipoContrato, contratos.idfaena,  " +
                                            "contratos.idcargo,  contratos.idcentroCosto, contratos.idjornada, " +
                                            "contratos.idBancoTrab, contratos.idAfpTrab, contratos.idIsapreTrab, " +
                                            "contratos.observaciones, contratos.sueldoBase,  " +
                                            "contratos.firmaTrabajador,  contratos.firmaEmpresa, contratos.rutValidador,  contratos.fechaValidacion,  " +
                                            "contratos.rechazado,  contratos.fechaCreacion, contratos.tipoCarga,  contratos.articulo22  " +
                                           "FROM personas " +
                                            "inner join contratos on personas.id = contratos.idPersona " +
                                            "where contratos.habilitado = 1 and  personas.rut = '" + rut+"' ", BD_Cli);

                List<ContratosBaseVM> opcionesList = new List<ContratosBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {
                    DetalleContrato contrato = new DetalleContrato();
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
                                        idbancotrab = int.Parse(dr["idbancotrab"].ToString()),
                                        idafptrab = int.Parse(dr["idafptrab"].ToString()),
                                        idisapretrab = int.Parse(dr["idisapretrab"].ToString()),
                                        sueldobase = dr["sueldoBase"].ToString(),
                                        observaciones = dr["observaciones"].ToString(),
                                        firmatrabajador = dr["firmaTrabajador"].ToString(),
                                        firmaempresa = dr["firmaEmpresa"].ToString(),
                                        tipocarga = dr["tipoCarga"].ToString(),
                                        articulo22 = dr["articulo22"].ToString()
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        contrato.id = r.id;
                        contrato.rut = r.rut;
                        contrato.nombres = r.nombres;
                        contrato.apellidos = r.apellidos;
                        contrato.idtipocontrato = r.idtipocontrato;
                        contrato.contrato = r.contrato;
                        contrato.inicio = r.inicio;
                        contrato.termino = r.termino;
                        contrato.idfaena = r.idfaena;
                        contrato.idcargo = r.idcargo;
                        contrato.idcentrocosto = r.idcentrocosto;
                        contrato.idjornada = r.idjornada;
                        contrato.idisapretrab = r.idisapretrab;
                        contrato.idbancotrab = r.idbancotrab;
                        contrato.idafptrab = r.idafptrab;
                        contrato.sueldobase = r.sueldobase;
                        contrato.observaciones = r.observaciones;
                        contrato.firmatrabajador = r.firmatrabajador;
                        contrato.firmaempresa = r.firmaempresa;
                        contrato.tipocarga = r.tipocarga;
                        contrato.articulo22 = r.articulo22;
                        DateTime inicio = Convert.ToDateTime(r.inicio);
                        contrato.inicio = inicio.ToString("yyyy'-'MM'-'dd");
                        DateTime termino = Convert.ToDateTime(r.termino);
                        contrato.termino = termino.ToString("yyyy'-'MM'-'dd");
                        contrato.tipocontrato = Generales.BuscaTipoContrato(contrato.idtipocontrato, BD_Cli);
                        FaenasBaseVM faen = Generales.BuscaFaena(r.idfaena, BD_Cli);
                        contrato.faena = faen.Descripcion;
                        contrato.cargo = Generales.BuscaCargo(r.idcargo, BD_Cli);
                        contrato.centrocosto = Generales.BuscaCentroCosto(r.idcentrocosto, BD_Cli);
                        contrato.jornada = Generales.BuscaJornada(r.idjornada, BD_Cli);
                        f.EjecutarConsultaSQLCli("select * from personas where rut='" + r.rut + "'", BD_Cli);
                        if (f.Tabla.Rows.Count > 0)
                        {
                            DataRow pers = f.Tabla.Rows[0];
                            contrato.nombres = pers["nombres"].ToString();
                            contrato.apellidos = pers["apellidos"].ToString();
                        }
                        BancosTrabajadorBaseVM banco = Generales.BuscaBancoTrabajador(r.idbancotrab, BD_Cli);
                        if (banco != null)
                        {
                            contrato.idbanco = banco.idbanco;
                            contrato.descripcionbanco = banco.descripcionBanco;
                            contrato.idtipocuenta = banco.idtipocta;
                            contrato.numerocuenta = banco.numerocuenta;
                        }
                        AfpsTrabajadorBaseVM afp = Generales.BuscaAfpTrabajador(r.idafptrab, BD_Cli);
                        if (afp != null)
                        {
                            contrato.codigoafp = afp.codigoAfp;
                            contrato.descripcionafp = afp.descripcion;
                            contrato.tipoapv = afp.tipoApv;
                            contrato.apv = afp.apv.ToString();
                        }
                        IsapresTrabajadorBaseVM isapre = Generales.BuscaIsapreTrabajador(r.idisapretrab, BD_Cli);
                        if (isapre != null)
                        {
                            contrato.codigoisapre = isapre.codigoisapre;
                            contrato.descripcionisapre = isapre.descripcion;
                            contrato.ufs = isapre.numeroUf.ToString();
                        }
                    }
                    return contrato;
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

        public Resultado CrearContratoService(DetalleContrato opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            DateTime hoy = DateTime.Now.Date;
            if (opciones.articulo22 == null || opciones.articulo22 == "") opciones.articulo22 = "N";
            if (opciones.tipocarga == null || opciones.tipocarga == "") opciones.tipocarga = "D";
            string hoystr = hoy.ToString("yyyy'-'MM'-'dd");
            opciones.contrato = opciones.contrato?.ToString() ?? string.Empty;
            opciones.inicio = opciones.inicio?.ToString() ?? string.Empty;
            opciones.termino = opciones.termino?.ToString() ?? string.Empty;
            opciones.observaciones = opciones.observaciones?.ToString() ?? string.Empty;

            try
            {
                if (opciones.rut == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar el RUT del contrato";
                    return resultado;
                }
                f.EjecutarConsultaSQLCli("select * from personas where rut='" + opciones.rut + "'", BD_Cli);
                if (f.Tabla.Rows.Count > 0)
                {
                    DataRow pers = f.Tabla.Rows[0];
                    int idpersona = int.Parse(pers["id"].ToString());
                    int idbancotrabajador = 0;
                    int idafptrabajador = 0;
                    int idisapretrabajador = 0;
                    string query1 = "INSERT INTO [dbo].[bancosTrabajador] ([idBanco] ,[idPersona],[fechaInicio] ,[idtipoCuenta] ,[numeroCuenta] ,[habilitado]) VALUES " +
                                    "(" + opciones.idbancotrab +", "+idpersona+ ", '" + hoystr + "', " + opciones.idtipocuenta + ", '" + opciones.numerocuenta + "', 1)";
                    if (f.EjecutarQuerySQLCli(query1, BD_Cli) == false)
                    {
                        resultado.result = 0;
                        resultado.mensaje = "Problemas al insertar banco de trabajador.";
                        return resultado;
                    }
                    else
                    {
                        f.EjecutarConsultaSQLCli("select bancosTrabajador.id from bancosTrabajador Where bancosTrabajador.idBanco =" + opciones.idbancotrab +
                            "  and bancosTrabajador.numerocuenta ='" + opciones.numerocuenta + "' and habilitado = 1 ", BD_Cli);
                        if (f.Tabla.Rows.Count > 0)
                        {
                            DataRow dtb = f.Tabla.Rows[0];
                            idbancotrabajador = int.Parse(dtb["id"].ToString());
                        }

                    }
                    decimal apv = 0;
                    if (opciones.tipoapv == "0") opciones.apv = "0";
                    if (opciones.apv != "0") apv = Convert.ToDecimal(opciones.apv);
                    string query11 = "INSERT INTO [dbo].[afpsTrabajador] ([codigoAfp] ,[idPersona] ,[fechaInicio] ,[tipoApv] ,[apv] ,[habilitado]) VALUES " +
                                    "(" + opciones.idafptrab + ", " + idpersona + ", '" + hoystr + "', '" + opciones.tipoapv + "', '" + apv + "', 1)";
                    if (f.EjecutarQuerySQLCli(query11, BD_Cli) == false)
                    {
                        resultado.result = 0;
                        resultado.mensaje = "Problemas al insertar afp de trabajador.";
                        return resultado;
                    }
                    else
                    {
                        f.EjecutarConsultaSQLCli("select afpsTrabajador.id from afpsTrabajador Where afpsTrabajador.codigoAfp =" + opciones.idafptrab +
                            "  and afpsTrabajador.apv =" + apv + " and habilitado = 1 ", BD_Cli);
                        if (f.Tabla.Rows.Count > 0)
                        {
                            DataRow dtb = f.Tabla.Rows[0];
                            idafptrabajador = int.Parse(dtb["id"].ToString());
                        }
                    }
                    Decimal ufs = 0;
                    if (opciones.ufs != null && opciones.ufs != "") ufs = Convert.ToDecimal(opciones.ufs);
                    string query12 = "INSERT INTO [dbo].[isapresTrabajador] ([codigoIsapre] ,[idPersona] ,[fechaInicio]  ,[numeroUf] ,[habilitado]) VALUES " +
                                    "(" + opciones.idisapretrab + ", " + idpersona + ", '" + hoystr +"', " + ufs + ", 1)";
                    if (f.EjecutarQuerySQLCli(query12, BD_Cli) == false)
                    {
                        resultado.result = 0;
                        resultado.mensaje = "Problemas al insertar isapre de trabajador.";
                        return resultado;
                    }
                    else
                    {
                        f.EjecutarConsultaSQLCli("select isapresTrabajador.id from isapresTrabajador Where isapresTrabajador.codigoisapre =" + opciones.idisapretrab +
                            "  and isapresTrabajador.numeroUf =" + ufs + " and habilitado = 1 ", BD_Cli);
                        if (f.Tabla.Rows.Count > 0)
                        {
                            DataRow dtb = f.Tabla.Rows[0];
                            idisapretrabajador = int.Parse(dtb["id"].ToString());
                        }
                    }

                    string query2 = "insert into contratos ([contrato],[idPersona],[inicio],[termino],[idTipoContrato], " +
                                     " [idFaena],[idCargo],[idCentroCosto],[idJornada],[observaciones],[sueldoBase],[firmaTrabajador], " +
                                     " [idBancoTrab],[idAfpTrab],[idIsapreTrab], " +
                                      " [firmaEmpresa],[fechaCreacion],[tipoCarga],[articulo22],[rechazado],[habilitado]) " +
                                    "values " +
                                     "( '" + opciones.contrato + "'," + idpersona + ", '" + opciones.inicio + "', '" + opciones.termino +
                                     "',  " + opciones.idtipocontrato + ", " + opciones.idfaena + ", " + opciones.idcargo +
                                     ",  " + opciones.idcentrocosto + ", " + opciones.idjornada + ", '" + opciones.observaciones + "', '" + opciones.sueldobase +
                                     "',  '" + opciones.firmatrabajador + "', "+idbancotrabajador+ ", "+idafptrabajador+", "+idisapretrabajador +
                                     ",'" + opciones.firmaempresa + "', '" + hoystr +
                                     "', '" + opciones.tipocarga + "', '" + opciones.articulo22 + "',0 ,1)  ";

                    if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                    {
                        resultado.result = 1;
                        resultado.mensaje = "Contrato ingresado de manera exitosa";
                    }
                    else
                    {
                        resultado.result = 0;
                        resultado.mensaje = "No se ingreso la información del contrato";
                    }
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No existe persona";

                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Guardar contrato";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar un contrato en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar contrato" + eG.Message.ToString();
                return resultado;
            }


        }
        public Resultado EditaContratoService(ContratosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;


            DateTime hoy = DateTime.Now.Date;
            string hoystr = hoy.ToString("yyyy'-'MM'-'dd");
            opciones.contrato = opciones.contrato?.ToString() ?? string.Empty;
            opciones.inicio = opciones.inicio?.ToString() ?? string.Empty;
            opciones.termino = opciones.termino?.ToString() ?? string.Empty;
            opciones.observaciones = opciones.observaciones?.ToString() ?? string.Empty;



            try
            {
                if (opciones.rut == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar el RUT del contrato";
                    return resultado ;
                }
                //f.EjecutarConsultaSQLCli("select * from personas where rut='" + opciones.rut + "'", BD_Cli);
                //if (f.Tabla.Rows.Count > 0)
                //{
                //    int idPer = int.Parse(f.Tabla.Rows[0]["id"].ToString());
                //}
                string query = "update contratos set [contrato]='" + opciones.contrato + "',[inicio]='" + opciones.inicio + "',[termino]='" + opciones.termino +
                                   "', [idtipocontrato]=" + opciones.idtipocontrato + ",[idfaena]=" + opciones.idfaena + ",[idcargo]= " + opciones.idcargo +
                                   ", [idcentroCosto]=" + opciones.idcentrocosto + ",[idjornada]=" + opciones.idjornada + ",[observaciones]= '" + opciones.observaciones +
                                   "', [firmaTrabajador]='" + opciones.firmatrabajador + "',[firmaEmpresa]='" + opciones.firmaempresa + "',[fechaCreacion]= '" + hoystr +
                                   "', [tipoCarga]='" + opciones.tipocarga + "',[articulo22]='" + opciones.articulo22 + "',[sueldoBase]='" + opciones.sueldobase +"' "+
                                   " where contratos.id=" + opciones.id ;

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "contrato editado exitosamente.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se editó la información del contrato.";
                }

                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar contrato";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar un contrato en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar contrato" + eG.Message.ToString();
                return resultado;
            }

        }

        public Resultado InhabilitaContratoService(ContratosDeleteVM opciones, int empresa)
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
                    resultado.mensaje = "Debe indicar la informacion del contrato que desea eliminar";
                    return resultado;
                }

                string query = "update contratos set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Contrato eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó la información de la contrato.";
                }

                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al eliminar contrato";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar contrato en el sistema", correos.destinatarioErrores);
                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una contrato" + eG.Message.ToString();
                return resultado;
            }
        }
        public Resultado ComboTiposContratoService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen tipos de contrato";
            resultado.result = 0;
            try
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "Remuneracion";
                f.EjecutarConsultaSQLCli("select * from tiposContratos ", BD_Cli);

                if (f.Tabla.Rows.Count > 0)
                {
                    var opcionesList = (from DataRow dr in f.Tabla.Rows
                                        select new
                                        {
                                            id = int.Parse(dr["id"].ToString()),
                                            descripcion = dr["Descripcion"].ToString()
                                        }).ToList();
                    resultado.data = opcionesList;
                    resultado.mensaje = "Existen tipos de contrato en la coleccion";
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
        public Resultado ComboFaenasService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen faenas";
            resultado.result = 0;
            try
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "Remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("select * from faenas Where habilitado = 1", BD_Cli);

                if (f.Tabla.Rows.Count > 0)
                {
                    var opcionesList = (from DataRow dr in f.Tabla.Rows
                                        select new
                                        {
                                            id = int.Parse(dr["id"].ToString()),
                                            descripcion = dr["Descripcion"].ToString()
                                        }).ToList();
                    resultado.data = opcionesList;
                    resultado.mensaje = "Existen faenas en la coleccion";
                    resultado.result = 1;

                }
                return resultado ;
            }
            catch (Exception ex)
            {
                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return resultado;
            }
        }
        public Resultado ComboCargosService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen cargos";
            resultado.result = 0;
            try
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "Remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("select * from cargos Where habilitado = 1", BD_Cli);

                if (f.Tabla.Rows.Count > 0)
                {
                    var opcionesList = (from DataRow dr in f.Tabla.Rows
                                        select new
                                        {
                                            id = int.Parse(dr["id"].ToString()),
                                            descripcion = dr["Descripcion"].ToString()
                                        }).ToList();
                    resultado.data = opcionesList;
                    resultado.mensaje = "Existen cargos en la coleccion";
                    resultado.result = 1;

                }
                return resultado ;
            }
            catch (Exception ex)
            {
                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return resultado;
            }
        }
        public Resultado ComboCentrosService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen centros";
            resultado.result = 0;
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "Remuneracion_" + RutEmpresa;


                f.EjecutarConsultaSQLCli("select * from centrosCostos Where habilitado = 1", BD_Cli);

                if (f.Tabla.Rows.Count > 0)
                {

                    var opcionesList = (from DataRow dr in f.Tabla.Rows
                                        select new
                                        {
                                            id = int.Parse(dr["id"].ToString()),
                                            descripcion = dr["Descripcion"].ToString()
                                        }).ToList();
                    resultado.data = opcionesList;
                    resultado.mensaje = "Existen centros en la coleccion";
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
        public Resultado ComboJornadasService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen jornadas";
            resultado.result = 0;
            try
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "Remuneracion_" + RutEmpresa;


                f.EjecutarConsultaSQLCli("select * from jornadas Where habilitado = 1", BD_Cli);

                if (f.Tabla.Rows.Count > 0)
                {

                    var opcionesList = (from DataRow dr in f.Tabla.Rows
                                        select new
                                        {
                                            id = int.Parse(dr["id"].ToString()),
                                            descripcion = dr["Descripcion"].ToString()
                                        }).ToList();
                    resultado.data = opcionesList;
                    resultado.mensaje = "Existen jornadas en la coleccion";
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
        public Resultado ComboBancosService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen bancos";
            resultado.result = 0;
            try
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "Remuneracion" ;
                f.EjecutarConsultaSQLCli("select * from bancos ", BD_Cli);
                if (f.Tabla.Rows.Count > 0)
                {
                    var opcionesList = (from DataRow dr in f.Tabla.Rows
                                        select new
                                        {
                                            id = int.Parse(dr["id"].ToString()),
                                            descripcion = dr["Descripcion"].ToString()
                                        }).ToList();
                    resultado.data = opcionesList;
                    resultado.mensaje = "Existen registros";
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
        public Resultado ComboAfpsService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen afps";
            resultado.result = 0;
            try
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "Remuneracion";
                f.EjecutarConsultaSQLCli("select * from afps ", BD_Cli);
                if (f.Tabla.Rows.Count > 0)
                {
                    var opcionesList = (from DataRow dr in f.Tabla.Rows
                                        select new
                                        {
                                            id = int.Parse(dr["codigo"].ToString()),
                                            descripcion = dr["Descripcion"].ToString()
                                        }).ToList();
                    resultado.data = opcionesList;
                    resultado.mensaje = "Existen registros";
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
        public Resultado ComboIsapresService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen isapres";
            resultado.result = 0;
            try
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "Remuneracion";
                f.EjecutarConsultaSQLCli("select * from isapres ", BD_Cli);
                if (f.Tabla.Rows.Count > 0)
                {
                    var opcionesList = (from DataRow dr in f.Tabla.Rows
                                        select new
                                        {
                                            id = int.Parse(dr["codigo"].ToString()),
                                            descripcion = dr["Descripcion"].ToString()
                                        }).ToList();
                    resultado.data = opcionesList;
                    resultado.mensaje = "Existen registros";
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
        public Resultado ComboTiposCuentasService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen tipos de cuentas";
            resultado.result = 0;
            try
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "Remuneracion" ;
                f.EjecutarConsultaSQLCli("select * from tiposcuentas ", BD_Cli);
                if (f.Tabla.Rows.Count > 0)
                {
                    var opcionesList = (from DataRow dr in f.Tabla.Rows
                                        select new
                                        {
                                            id = int.Parse(dr["id"].ToString()),
                                            descripcion = dr["Descripcion"].ToString()
                                        }).ToList();
                    resultado.data = opcionesList;
                    resultado.mensaje = "Existen registros";
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

        public Resultado ExistePersonaService(string rut, int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No se existe persona";
            resultado.result = 0;
            try
            {
                rut = rut.Replace("-", "");
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "Remuneracion_" + RutEmpresa;
                ContratosBaseVM opcionesList = new ContratosBaseVM();
                f.EjecutarConsultaSQLCli("select * from personas where rut='" + rut + "'", BD_Cli);
                if (f.Tabla.Rows.Count > 0)
                {
                    DataRow pers = f.Tabla.Rows[0];
                    opcionesList.nombres = pers["nombres"].ToString();
                    opcionesList.apellidos = pers["apellidos"].ToString();
                    resultado.data = opcionesList;
                    resultado.mensaje = "Existe persona";
                    resultado.result = 1;
                }
                return resultado;
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar Persona";
                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return resultado;

            }
        }
    }
}
public class DetalleContrato
{
    public int id { get; set; }
    public int idPersona { get; set; }
    public string rut { get; set; }
    public string nombres { get; set; }
    public string apellidos { get; set; }
    public string tipocontrato { get; set; }
    public int idtipocontrato { get; set; }
    public string contrato { get; set; }
    public string inicio { get; set; }
    public string termino { get; set; }
    public string faena { get; set; }
    public int idfaena { get; set; }
    public string cargo { get; set; }
    public int idcargo { get; set; }
    public string centrocosto { get; set; }
    public int idcentrocosto { get; set; }
    public string jornada { get; set; }
    public int idjornada { get; set; }
    public string sueldobase { get; set; }
    public string observaciones { get; set; }
    public string firmatrabajador { get; set; }
    public string firmaempresa { get; set; }
    public string tipocarga { get; set; }
    public string articulo22 { get; set; }
    public int idbancotrab { get; set; }
    public int idbanco{ get; set; }
    public string descripcionbanco { get; set; }
    public int idtipocuenta { get; set; }
    public string numerocuenta { get; set; }
    public int idafptrab { get; set; }
    public int codigoafp { get; set; }
    public string descripcionafp { get; set; }
    public string tipoapv { get; set; }
    public string apv { get; set; }
    public int idisapretrab { get; set; }
    public int codigoisapre { get; set; }
    public string descripcionisapre { get; set; }
    public string ufs { get; set; }
    public string nacimiento { get; set; }
    public string sexo { get; set; }
    public int nrohijos { get; set; }

}
