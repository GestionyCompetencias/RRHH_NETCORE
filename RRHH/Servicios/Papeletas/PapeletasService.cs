using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.Papeletas
{
    /// <summary>
    /// Servicio para generar archivo de previred.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IPapeletasService
    {
        /// <summary>
        /// Genera archivo de papeleta.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes de proceso</param>
        /// <param name="anio">año de proceso</param>
        /// <returns>Archivo de papeletas</returns>
        public List<Papeleta> ListarPapeletasService(int idEmpresa, int mes, int anio);

    }

    public class PapeletasService : IPapeletasService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();

		public PapeletasService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<Papeleta> ListarPapeletasService(int empresa,  int mes,int anio)
        {
            List<Papeleta> papeletas = new List<Papeleta>();
            try
            {
                EmpresasVM empr = f.obtenerEmpresa(empresa);
                string RutEmpresa = empr.rut;
                var BD_Cli = "remuneracion_" + RutEmpresa;
                DateTime fecini = new DateTime(anio, mes, 1);
                DateTime fecfin = f.UltimoDia(fecini);
                string fecinistr = fecini.ToString("yyyy'-'MM'-'dd");
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");


                f.EjecutarConsultaSQLCli("SELECT resultados.rutTrabajador,resultados.concepto,resultados.monto,resultados.cantidad,resultados.informado " +
                                            "FROM resultados " +
                                            "WHERE resultados.habilitado = 1 " + 
                                            " and resultados.fechaPago <= '"+fecfinstr+"' and resultados.fechaPago >= '"+fecinistr+
                                            "' and resultados.pago = 'L' ", BD_Cli);


                List<Resultados> opcionesList = new List<Resultados>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new Resultados()
                                    {
                                        rutTrabajador = dr["rutTrabajador"].ToString(),
                                        concepto = int.Parse(dr["concepto"].ToString()),
                                        monto = decimal.Parse(dr["monto"].ToString()),
                                        cantidad = decimal.Parse(dr["cantidad"].ToString()),
                                        informado = decimal.Parse(dr["informado"].ToString()),
                                    }).ToList();
                    var trabajadores = opcionesList.GroupBy(x=> x.rutTrabajador).Select(g=> new { rutTrabajador = g.Key }).ToList();
                    foreach (var t in trabajadores)
                    {
                        Papeleta pap = new Papeleta();
                        // información de contrato
                        pap.contrato =  ConsultaContratoRUT(t.rutTrabajador, BD_Cli);
                        pap.contrato.rut = pap.contrato.rut.Substring(0, pap.contrato.rut.Length - 1) + "-" + pap.contrato.rut.Substring(pap.contrato.rut.Length - 1, 1);

                        // resultados del trabajador 
                        var detalle = opcionesList.Where(x => x.rutTrabajador == t.rutTrabajador).ToList();
                        List<Resultados> haberes = new List<Resultados>();
                        List<Resultados> desctos = new List<Resultados>();
                        List<Resultados> legales = new List<Resultados>();
                        InfoPapeletaM otrainf = new InfoPapeletaM();
                        Decimal horasextras = 0;

                        foreach (var r in detalle)
                        {
                            if (r.concepto < 100)
                            {
                                haberes.Add(r);
                                if (r.concepto == 1) otrainf.dias = (int)r.cantidad;
                                if (r.concepto == 3 || r.concepto == 4 || r.concepto ==5 ) horasextras = horasextras + r.cantidad;
                                if (r.concepto == 7) otrainf.cargas = (int)r.cantidad;
                            }
                            else
                            { 
                                if(r.concepto < 900)
                                {
                                    desctos.Add(r);
                                }
                                else
                                {
                                        if (r.concepto == 2010) otrainf.imponible = (int)r.monto;
                                        if (r.concepto == 2030) otrainf.tributable = (int)r.monto;
                                        if (r.concepto == 910)
                                        {
                                            otrainf.valafp = (int)r.monto;
                                            otrainf.porafp = r.informado.ToString("#0.00");
                                            int afp = (int)r.cantidad;
                                            otrainf.nomafp = f.BuscaAfpCodigo(BD_Cli, afp);
                                        }
                                        if (r.concepto == 921)
                                        {
                                            otrainf.valisa = (int)r.monto;
                                            otrainf.porisa = r.informado.ToString("#0.00");
                                            int isapre = (int)r.cantidad;
                                            otrainf.nomisa = f.BuscaIsapreCodigo(BD_Cli,isapre);
                                        }
                                        if (r.concepto == 916)
                                        {
                                            otrainf.valseg = (int)r.monto;
                                            otrainf.porseg = r.informado.ToString("#0.00");
                                        }
                                        if (r.concepto == 990)
                                        {
                                            otrainf.impto = (int)r.monto;
                                        }
                                        if (r.concepto == 2100) otrainf.totalhaberes = (int)r.monto;
                                        if (r.concepto == 2200) otrainf.totaldescuentos = (int)r.monto;
                                        if (r.concepto == 2300) otrainf.saldo = (int)r.monto;
                                        if (r.concepto == 2010) otrainf.imponible = (int)r.monto;
                                        if (r.concepto == 2010) otrainf.imponible = (int)r.monto;
                                        if (r.concepto == 2010) otrainf.imponible = (int)r.monto;
                                        if (r.concepto == 2010) otrainf.imponible = (int)r.monto;
                                }
                            }
                        }
                        otrainf.horasextras = horasextras.ToString("##0.00");
                        otrainf.mesdepago = f.MesLetras(mes);
                        otrainf.aniodepago= anio.ToString();
                        otrainf.empresa = empr.razonsocial;
                        otrainf.rutempresa = empr.rut;
                        FaenasBaseVM faen = Generales.BuscaFaena(pap.contrato.idfaena, BD_Cli);
                        otrainf.faenatrabajador = faen.Descripcion ;
                        otrainf.fechapago = fecfin.ToString();
                        otrainf.cargo = "AYUDANTE";


        pap.haberes = haberes;
                        pap.descuentos = desctos;   
                        pap.Legales = legales;
                        pap.otrainf = otrainf;
                        papeletas.Add(pap);
                    }
                    return papeletas;
                }
                else
                {
                 return papeletas;  
                }
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error al generar archivo de previred";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al generar archivo de previred", correos.destinatarioErrores);
                return papeletas;
            }

        }
        public DetalleContrato ConsultaContratoRUT(string rut, string BD_Cli)
        {

            try
            {

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
                                            "where contratos.habilitado = 1 and  personas.rut = '" + rut + "' ", BD_Cli);

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
            catch (System.Exception ex)
            {
                var Asunto = "Error al Consultar contratos";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar un cliente", correos.destinatarioErrores);
                return null;
            }

        }

    }
}

namespace RRHH.Models.ViewModels
{
    public class Papeleta
    {
        public DetalleContrato contrato;
        public List<Resultados> haberes;
        public List<Resultados> descuentos;
        public List<Resultados> Legales;
        public InfoPapeletaM otrainf;
    }
    public class InfoPapeletaM
    {
        public string mesdepago;
        public string aniodepago;
        public string empresa;
        public string rutempresa;
        public string faenatrabajador;
        public int dias;
        public int cargas;
        public string horasextras;
        public int imponible;
        public int tributable;
        public string nomafp;
        public int valafp;
        public string porafp;
        public string nomisa;
        public int valisa;
        public string porisa;
        public int totalhaberes;
        public int totaldescuentos;
        public string fechapago;
        public int saldo;
        public string cargo;
        public int valseg;
        public string porseg;
        public int impto;
    }
}
