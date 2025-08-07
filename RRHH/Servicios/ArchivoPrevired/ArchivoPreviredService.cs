using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.ArchivoPrevired
{
    /// <summary>
    /// Servicio para generar archivo de previred.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IArchivoPreviredService
    {
        /// <summary>
        /// Genera archivo de previred.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes de proceso</param>
        /// <param name="anio">año de proceso</param>
        /// <param name="path">ruta del archivo</param>
        /// <returns>Archivo de previred</returns>
        public string ListarArchivoPreviredService(int idEmpresa, int mes, int anio, string path );

    }

    public class ArchivoPreviredService : IArchivoPreviredService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();

		public ArchivoPreviredService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public string ListarArchivoPreviredService(int empresa,  int mes,int anio,string path)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {
                EmpresasVM empr = f.obtenerEmpresa(empresa);
                var BD_Cli = "remuneracion_" + empr.rut;
                DateTime fecini = new DateTime(anio, mes, 1);
                DateTime fecfin = f.UltimoDia(fecini);
                string fecinistr = fecini.ToString("yyyy'-'MM'-'dd");
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");
                List<Afps> listaafps = CargaAfp();
                List<Isapres> listaisapres = CargaIsapre();
                string archivo = "/temp/"+empr.rut + "-Previred-" + anio.ToString("000#") + mes.ToString("0#") + ".csv";
                string ruta = path + archivo; 

                f.EjecutarConsultaSQLCli("SELECT resultados.rutTrabajador,resultados.concepto,resultados.monto,resultados.cantidad,resultados.informado " +
                                            "FROM resultados " +
                                            "WHERE resultados.habilitado = 1 " + 
                                            " and resultados.fechaPago <= '"+fecfinstr+"' and resultados.fechaPago >= '"+fecinistr+
                                            "' and resultados.pago = 'L' ", BD_Cli);
                List<DetResultado> resultados = new List<DetResultado>();
                if (f.Tabla.Rows.Count > 0)
                {
                    resultados = (from DataRow dr in f.Tabla.Rows
                                    select new DetResultado()
                                    {
                                        rutTrabajador = dr["rutTrabajador"].ToString(),
                                        concepto = int.Parse(dr["concepto"].ToString()),
                                        monto = decimal.Parse(dr["monto"].ToString()),
                                        informado = decimal.Parse(dr["informado"].ToString()),
                                        cantidad = decimal.Parse(dr["cantidad"].ToString()),
                                    }).ToList();
                    var trabajadores = resultados.GroupBy(x=> x.rutTrabajador).Select(g=> new { rutTrabajador = g.Key }).ToList();
                    List<dynamic> libro = new List<dynamic>();
                    foreach (var t in trabajadores)
                    {
                        Detprevired linea = new Detprevired();
                        DetalleContrato cont = ConsultaContratoRUT(t.rutTrabajador, BD_Cli);
                        linea.rut = t.rutTrabajador.Substring(0, t.rutTrabajador.Length - 1); 
                        linea.dgv = t.rutTrabajador.Substring(t.rutTrabajador.Length - 1, 1);
                        linea.apaterno = cont.apellidos;
                        linea.nombres = cont.nombres;
                        linea.sexo = cont.sexo;
                        linea.nacionalidad= "0";
                        linea.pago="01";
                        linea.periododesde = mes.ToString("0#")+anio.ToString("000#");
                        linea.periodohasta = mes.ToString("0#") + anio.ToString("000#");
                        linea.regimen = "AFP";
                        linea.tipotrab= "0";
                        linea.tipolinea ="00";
                        linea.movimiento = "0";
                        linea.fechadesde = "          ";
                        linea.fechahasta ="          ";
                        linea.tramofamiliar = cont.tipocarga;
                        linea.cargassimples = cont.nrohijos.ToString("0#");
                        linea.cargasmaternales= "0";
                        linea.cargasinvalidas="0";
                        linea.familiarretroactivo= "     0";
                        linea.reintegrofamiliar= "     0";
                        linea.solicitudjoven = "N"  ;
        // Datos AFp
                        int afp = listaafps.Where(x => x.codigoafp == cont.idafptrab).Select(x => x.codigoprevired).FirstOrDefault();
                        linea.codigoafp = afp.ToString("0#");
                        linea.ahorro= "       0";
                        linea.rentasustituta = "       0";
                        linea.tasapactada = "00,00";
                        linea.aporteindemizacion = "        0";
                        linea.nroperiodos = "  "; ;
                        linea.desdesustituto="          ";
                        linea.hastasustituto="          ";
                        linea.puestotrabajo = "                                        ";
                        linea.porctrabajopesado= "00,00";
                        linea.cotiztrabajopesado = "      ";
        // Datos ahorro individual
                        linea.codigoAPVI="   ";
                        linea.contratoAPVI = "                    ";
                        linea.formaAPVI = " ";
                        linea.cotizAPVI= "        ";
                        linea.cotizdeposito = "        ";
        // Datos ahorro colectivo
                        linea.codigoAPVC = "   ";
                        linea.contratoAPVC= "                    ";
                        linea.formaAPVC=" ";
                        linea.cotiztrabAPVC = "        ";
                        linea.cotizempAPVC="        ";
        // Datos afiliado voluntario
                        linea.rutafiliado = "           ";
                        linea.dgvafiliado = " ";
                        linea.appafiliado = "                             ";
                        linea.apmafiliado = "                             ";
                        linea.nomafiliado = "                             ";
                        linea.movafiliado = " 0";
                        linea.desdeafiliado = "          ";
                        linea.hastaafiliafo = "          ";
                        linea.codigoafpafi = "  ";
                        linea.montocapital = "        ";
                        linea.montoahorro = "        ";
                        linea.nroperiodosafi="  ";
        // Datos FONASA
                        linea.codigocaja = "    ";
                        linea.tasacaja = "00,00";
                        linea.imponiblecaja = "        ";
                        linea.cotizcaja = "        ";
                        linea.imponibledesahucio = "        ";
                        linea.codigodesahucio = "    ";
                        linea.tasadesahucio = "00.00";
                        linea.cotizdesahucio = "        ";
                        linea.cotizaccidente = "        ";
                        linea.bono15386 = "        ";
                        linea.dctcargas = "        ";
                        linea.bonogobierno = "        ";
        // Datos salud
                        int isapre = listaisapres.Where(x=> x.codigoisapre == cont.idisapretrab).Select(x=> x.codigoprevired).FirstOrDefault();
                        linea.codigoisapre = isapre.ToString("0#");
                        linea.nroFUN = "                ";
                        linea.montoGES="        ";
        // 8 Datos caja compensacion
                        linea.codigoCCAF=" ";
                        linea.imponibleCCAF = " ";
                        linea.creditoCCAF = " ";
                        linea.dentalCCAF = " ";
                        linea.leasingCCAF = " ";
                        linea.seguroCCAF = " ";
                        linea.otrosCCAF = " ";
                        linea.cotizCCAF = " ";
                        linea.descuentoscargas = " ";
                        linea.otrosCCAF1 = " ";
                        linea.otrosCCAF2 = " ";
                        linea.bonogob = " ";
                        linea.codigosucursal = " ";
        //  0 Datos mutualidad
                        linea.codigomutual = " ";
                        linea.imponiblemutual = " ";
                        linea.cotizmutual = " ";
                        linea.sucursalmutual = " ";
        // 10 Datos administradora SC
                        linea.imponibleSC = " ";
                        linea.aportetrabajador = " ";
                        linea.aporteempresa = " ";
        // 11 Datos pagador subcisios
                        linea.rutpagadora = " ";
                        linea.dgvpagadora = " ";
        // 12 Otros datos empresa
                        linea.centrocostos = " ";

        var detalle = resultados.Where(x => x.rutTrabajador == t.rutTrabajador).ToList();
                        foreach (var r in detalle)
                        {
                            if (r.concepto == 2011)
                            {
                                linea.imponibleafp = r.monto.ToString("########");
                                linea.imponibleisapre = r.monto.ToString("########");
                            }
                            if (r.concepto == 910)linea.cotizobligatoria = r.monto.ToString("########");
                            if (r.concepto == 2401) linea.cotizseguro = r.monto.ToString("########");
                            if (r.concepto == 921)
                            {
                                if(cont.idisapretrab == 102)
                                {
                                    linea.cotizfonasa = r.monto.ToString("########");
                                    linea.cotizsalud = "  ";
                                    linea.monedapactada = " ";
                                    linea.cotizvoluntaria = "  ";
                                    linea.imponibleisapre = "        ";
                                }
                                else
                                {
                                    linea.monedapactada = "1";
                                    if (r.informado > 0)
                                    {
                                        linea.monedapactada = "2";
                                        linea.cotizpactada = r.informado.ToString("########");
                                    }
                                    linea.cotizsalud = r.monto.ToString("########");
                                }

                            }
                            if (r.concepto == 7 ) linea.asignacionfamiliar = r.monto.ToString("00000#");
                            if (r.concepto == 1 ) linea.diastrab = r.cantidad.ToString("0#");
                        }
                        libro.Add(linea);
                    }
                    TextWriter Escribir = new StreamWriter(ruta);
                    foreach (var s in libro) 
                    {
                    string lineaa = s.rut + ";" + s.dgv + ";" + s.apaterno + ";" + s.amaterno + ";" + s.nombres + ";" + s.sexo + ";" + s.nacionalidad +
                     ";" + s.pago + ";" + s.periododesde + ";" + s.periodohasta + ";" + s.regimen + ";" + s.tipotrab +
                     ";" + s.diastrab + ";" + s.tipolinea + ";" + s.movimiento + ";" + s.fechadesde + ";" + s.fechahasta +
                     ";" + s.tramofamiliar + ";" + s.cargassimples + ";" + s.cargasmaternales +
                     ";" + s.cargasinvalidas + ";" + s.asignacionfamiliar + ";" + s.familiarretroactivo + ";" + s.reintegrofamiliar + 
                     ";" + s.solicitudjoven + ";" + s.codigoafp + ";" + s.imponibleafp + ";" + s.cotizobligatoria + ";" + s.cotizseguro +
                     ";" + s.ahorro + ";" + s.rentasustituta + ";" + s.tasapactada + ";" + s.aporteindemizacion + ";" + s.nroperiodos + 
                     ";" + s.desdesustituto + ";" + s.hastasustituto +";" + s.puestotrabajo + ";" + s.porctrabajopesado + 
                     ";" + s.cotiztrabajopesado + ";" + s.codigoAPVI + ";" + s.contratoAPVI + ";" + s.formaAPVI +
                     ";" + s.cotizAPVI + ";" + s.cotizdeposito + ";" + s.codigoAPVC+ ";" + s.contratoAPVC + ";" + s.formaAPVC + 
                     ";" + s.cotiztrabAPVC + ";" + s.cotizempAPVC + ";" + s.rutafiliado + ";" + s.dgvafiliado + ";" + s.appafiliado + 
                     ";" + s.apmafiliado + ";" + s.nomafiliado + ";" + s.movafiliado + ";" + s.desdeafiliado + ";" + s.hastaafiliafo + 
                     ";" + s.codigoafpafi + ";" + s.montocapital + ";" + s.montoahorro + ";" + s.nroperiodosafi + ";" + s.codigocaja + 
                     ";" + s.tasacaja + ";" + s.imponiblecaja + ";" + s.cotizcaja + ";" + s.imponibledesahucio + ";" + s.codigodesahucio +
                     ";" + s.tasadesahucio + ";" + s.cotizdesahucio + ";" + s.cotizfonasa + ";" + s.cotizaccidente + ";" + s.bono15386 +
                     ";" + s.dctcargas + ";" + s.bonogobierno + ";" + s.codigoisapre + ";" + s.nroFUN + ";" + s.imponibleisapre+ 
                     ";" + s.monedapactada + ";" + s.cotizpactada + ";" + s.cotizsalud + ";" + s.cotizvoluntaria + ";" + s.montoGES + 
                     ";" + s.codigoCCAF + ";" + s.imponibleCCAF + ";" + s.creditoCCAF + ";" + s.dentalCCAF+ ";" + s.leasingCCAF + 
                     ";" + s.seguroCCAF + ";" + s.otrosCCAF + ";" + s.cotizCCAF + ";" + s.descuentoscargas + ";" + s.otrosCCAF1 + 
                     ";" + s.otrosCCAF2 + ";" + s.bonogob + ";" + s.codigosucursal + ";" + s.codigomutual + ";" + s.imponiblemutual + 
                     ";" + s.cotizmutual + ";" + s.sucursalmutual + ";" + s.imponibleSC + ";" + s.aportetrabajador + ";" + s.aporteempresa + 
                     ";" + s.rutpagadora + ";" + s.dgvpagadora + ";" + s.centrocostos;
                    Escribir.WriteLine(lineaa);
                    }
                   Escribir.Close();
                    return ruta;
                }
                return null;  
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error al generar archivo de previred";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al generar archivo de previred", correos.destinatarioErrores);
                return null;
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
                                            "contratos.rechazado,  contratos.fechaCreacion, contratos.tipoCarga,  contratos.articulo22,  " +
                                            "trabajadores.nacimiento,  trabajadores.sexo, trabajadores.nrohijos " +
                                           "FROM personas " +
                                            "inner join contratos on personas.id = contratos.idPersona " +
                                            "inner join trabajadores on personas.id = trabajadores.idpersona " +
                                            "where contratos.habilitado = 1 and  personas.rut = '" + rut + "' ", BD_Cli);

                List<DetalleContrato> opcionesList = new List<DetalleContrato>();
                if (f.Tabla.Rows.Count > 0)
                {
                    DetalleContrato contrato = new DetalleContrato();
                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetalleContrato()
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
                                        articulo22 = dr["articulo22"].ToString(),
                                        nacimiento = dr["nacimiento"].ToString(),
                                        sexo = dr["sexo"].ToString(),
                                        nrohijos = int.Parse(dr["nrohijos"].ToString())
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
        public List<Afps> CargaAfp()
        {
            try
            {
                f.EjecutarConsultaSQLCli("select * from isapres ", "Remuneracion");
                List<Afps> lista = new List<Afps>();
                if (f.Tabla.Rows.Count > 0)
                {
                    lista = (from DataRow dr in f.Tabla.Rows
                                        select new Afps()
                                        {
                                            codigoafp = int.Parse(dr["codigo"].ToString()),
                                            codigoprevired = int.Parse(dr["codigoprevired"].ToString())
                                        }).ToList();
                }
                return lista;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
        public List<Isapres> CargaIsapre()
        {
            try
            {
                f.EjecutarConsultaSQLCli("select * from isapres ", "Remuneracion");
                List<Isapres> lista = new List<Isapres>();
                if (f.Tabla.Rows.Count > 0)
                {
                    lista = (from DataRow dr in f.Tabla.Rows
                             select new Isapres()
                             {
                                 codigoisapre = int.Parse(dr["codigo"].ToString()),
                                 codigoprevired = int.Parse(dr["codigoprevired"].ToString())
                             }).ToList();
                }
                return lista;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

    }
}
namespace RRHH.Models.ViewModels
{
    public class Detprevired
    {
        //  1 Datos trabajador
        public string rut;
        public string dgv;
        public string apaterno;
        public string amaterno;
        public string nombres;
        public string sexo;
        public string nacionalidad;
        public string pago;
        public string periododesde;
        public string periodohasta;
        public string regimen;
        public string tipotrab;
        public string diastrab;
        public string tipolinea;
        public string movimiento;
        public string fechadesde;
        public string fechahasta;
        public string tramofamiliar;
        public string cargassimples;
        public string cargasmaternales;
        public string cargasinvalidas;
        public string asignacionfamiliar;
        public string familiarretroactivo;
        public string reintegrofamiliar;
        public string solicitudjoven;
        //  2 Datos AFp
        public string codigoafp;
        public string imponibleafp;
        public string cotizobligatoria;
        public string cotizseguro;
        public string ahorro;
        public string rentasustituta;
        public string tasapactada;
        public string aporteindemizacion;
        public string nroperiodos;
        public string desdesustituto;
        public string hastasustituto;
        public string puestotrabajo;
        public string porctrabajopesado;
        public string cotiztrabajopesado;
        //  3 Datos ahorro individual
        public string codigoAPVI;
        public string contratoAPVI;
        public string formaAPVI;
        public string cotizAPVI;
        public string cotizdeposito;
        // 4 Datos ahorro colectivo
        public string codigoAPVC;
        public string contratoAPVC;
        public string formaAPVC;
        public string cotiztrabAPVC;
        public string cotizempAPVC;
        // 5 Datos afiliado voluntario
        public string rutafiliado;
        public string dgvafiliado;
        public string appafiliado;
        public string apmafiliado;
        public string nomafiliado;
        public string movafiliado;
        public string desdeafiliado;
        public string hastaafiliafo;
        public string codigoafpafi;
        public string montocapital;
        public string montoahorro;
        public string nroperiodosafi;
        // 6 Datos FONASA
        public string codigocaja;
        public string tasacaja;
        public string imponiblecaja;
        public string cotizcaja;
        public string imponibledesahucio;
        public string codigodesahucio;
        public string tasadesahucio;
        public string cotizdesahucio;
        public string cotizfonasa;
        public string cotizaccidente;
        public string bono15386;
        public string dctcargas;
        public string bonogobierno;
        // 7 Datos salud
        public string codigoisapre;
        public string nroFUN;
        public string imponibleisapre;
        public string monedapactada;
        public string cotizpactada;
        public string cotizsalud;
        public string cotizvoluntaria;
        public string montoGES;
        // 8 Datos caja compensacion
        public string codigoCCAF;
        public string imponibleCCAF;
        public string creditoCCAF;
        public string dentalCCAF;
        public string leasingCCAF;
        public string seguroCCAF;
        public string otrosCCAF;
        public string cotizCCAF;
        public string descuentoscargas;
        public string otrosCCAF1;
        public string otrosCCAF2;
        public string bonogob;
        public string codigosucursal;
        //  9 Datos mutualidad
        public string codigomutual;
        public string imponiblemutual;
        public string cotizmutual;
        public string sucursalmutual;
        // 10 Datos administradora SC
        public string imponibleSC;
        public string aportetrabajador;
        public string aporteempresa;
        // 11 Datos pagador subcisios
        public string rutpagadora;
        public string dgvpagadora;
        // 12 Otros datos empresa
        public string centrocostos;
    }
    public class Afps
    {
        public int codigoafp { get; set; }
        public int codigoprevired { get; set; }

    }
    public class Isapres
    {
        public int codigoisapre;
        public int codigoprevired;

    }
}

