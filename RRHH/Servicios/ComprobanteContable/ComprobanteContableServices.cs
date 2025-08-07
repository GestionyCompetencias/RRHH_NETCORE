using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;
using Microsoft.Data.SqlClient;

namespace RRHH.Servicios.ComprobanteContable
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IComprobanteContableService
    {
        /// <summary>
        /// Genera lista de comprobante.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes del haber informados que desea listar</param>
        /// <param name="anio">año del haber informados que desea listar</param>
        /// <param name="pago">pago del haber informados que desea listar</param>
        /// <param name="modulo">modulo de comprobante</param>
        /// <returns>Lista de comprobante contable</returns>
        public Resultado ListarComprobanteContableService(int idEmpresa, int mes, int anio, string pago,string modulo);

        /// <summary>
        /// Genera comprobante contable.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes del haber informados que desea listar</param>
        /// <param name="anio">año del haber informados que desea listar</param>
        /// <param name="pago">pago del haber informados que desea listar</param>
        /// <param name="modulo">modulo de comprobante</param>
        /// <returns>Resultado del proceso</returns>
        public Resultado ProcesarComprobanteContableService(int idEmpresa, int mes, int anio, string pago,string modulo);
    }

    public class ComprobanteContableService : IComprobanteContableService
    {
        private readonly IDatabaseManager _databaseManager;
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Generales Generales = new Generales();
        private Seguridad seguridad = new Seguridad();

        public ComprobanteContableService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public Resultado ListarComprobanteContableService(int empresa, int mes, int anio, string pago,string modulo)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            try
            {
                DateTime ini = new DateTime(anio, mes, 1);
                DateTime fin = f.UltimoDia(ini);
                string inistr = ini.ToString("yyyy'-'MM'-'dd");
                string finstr = fin.ToString("yyy'-'MM'-'dd");
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                f.EjecutarConsultaSQLCli("SELECT resultado.id,resultado.rutTrabajador, resultado.concepo,resultado.descripcion,resultado.monto " +
                                           " FROM resultado " +
                                            "WHERE resultado.habilitado = 1 and resultado.pago ='" + pago + "' and resultado.monto > 0 " +
                                            " and resultado.fechaPago >= '"+inistr+"', and resultado.fechaPago <= '"+finstr+"' "+
                                            " and ( resultado.concepto <1000 or resultado.concepto = 2300) Order by resultado.concepto", BD_Cli);

                List<detresultado> conceptos = new List<detresultado>();
                if (f.Tabla.Rows.Count > 0)
                {

                    conceptos = (from DataRow dr in f.Tabla.Rows
                                    select new detresultado()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        rut = dr["rutTrabajador"].ToString(),
                                        descripcion = dr["descripcion"].ToString(),
                                        concepto = int.Parse(dr["concepto"].ToString()),
                                        monto= decimal.Parse(dr["monto"].ToString())
                                    }).ToList();
                }
                int oldhab = 0;
                decimal totaldeb = 0;
                decimal totalhab = 0;
                decimal totgendeb = 0;
                decimal totgenhab = 0;
                decimal cero = 0;
                int cantidad = 0;
                string ids = null;
                if (conceptos.Count > 0)
                {
                    var conc = conceptos.GroupBy(x => x.concepto).Select(g => new { concepto = g.Key }).ToList();
                    List<detcomp> lista = new List<detcomp>();

                    foreach (var c in conc)
                    {
                        f.EjecutarConsultaSQLCli("SELECT conversionContable.id,conversionContable.modulo,conversionContable.pago,conversionContable.concepto, " +
                                                  "conversionContable.cuenta,conversionContable.tipoAuxiliar, conversionContable.codigoAuxiliar, " +
                                                  "conversionContable.debeHaber, conversionContable.tipoVencimiento, conversionContable.diaVencimiento,  " +
                                                  "conversionContable.mesVencimiento, conversionContable.agrupacion, conversionContable.provision  " +
                                                 " FROM conversionContable " +
                                                  "WHERE conversionContable.habilitado = 1 and conversionContable.modulo ='" + modulo +
                                                  "' and conversionContable.concepto ="+ c.concepto, BD_Cli);

                        List<ConversionContableBaseVM> conversion = new List<ConversionContableBaseVM>();
                        ConversionContableBaseVM regconv = conversion.FirstOrDefault();

                        if (f.Tabla.Rows.Count > 0)
                        {

                            conversion = (from DataRow dr in f.Tabla.Rows
                                              select new ConversionContableBaseVM()
                                              {
                                                  Id = int.Parse(dr["id"].ToString()),
                                                  modulo = dr["modulo"].ToString(),
                                                  pago = dr["pago"].ToString(),
                                                  concepto = int.Parse(dr["concepto"].ToString()),
                                                  cuenta = dr["cuenta"].ToString(),
                                                  debehaber = dr["debehaber"].ToString(),
                                                  tipoauxiliar = int.Parse(dr["tipoAuxiliar"].ToString()),
                                                  codigoauxiliar = dr["codigoauxiliar"].ToString(),
                                                  tipovencimiento = int.Parse(dr["tipoVencimiento"].ToString()),
                                                  diavencimiento = int.Parse(dr["diaVencimiento"].ToString()),
                                                  mesvencimiento = int.Parse(dr["mesVencimiento"].ToString()),
                                                  agrupacion = int.Parse(dr["agrupacion"].ToString()),
                                                  provision = int.Parse(dr["provision"].ToString())
                                              }).ToList();
                            regconv = conversion.FirstOrDefault();
                        }
                        var detalle = conceptos.Where(x => x.concepto == c.concepto).ToList();

                        detcomp tothab = new detcomp();
                        foreach (var d in detalle)
                        {
                            tothab.CUENTA = regconv.cuenta;
                            tothab.DESCRIPCION = BuscaCuenta(regconv.cuenta, BD_Cli);
                            if (oldhab < 100)
                            {
                                totaldeb = (decimal)(totaldeb + d.monto);
                                totgendeb = (decimal)(totgendeb + d.monto);
                            }
                            else
                            {
                                totalhab = (decimal)(totalhab + d.monto);
                                totgenhab = (decimal)(totgenhab + d.monto);
                            }
                            cantidad++;
                            ids = ids + d.id + "*";
                        }
                        if (c.concepto < 100)
                        {
                            tothab.DEBE = totaldeb.ToString("###,###,###");
                            tothab.HABER = cero.ToString("###,###,###");
                        }
                        else
                        {
                            tothab.DEBE = cero.ToString("###,###,###");
                            tothab.HABER = totalhab.ToString("###,###,###");
                        }
                        tothab.CANTIDAD = cantidad.ToString("###");
                        tothab.ENTIDAD = "";
                        tothab.IDS = ids;
                        totaldeb = 0;
                        totalhab = 0;
                        cantidad = 0;
                        ids = null;
                        lista.Add(tothab);
                    }

                    detcomp tothab2 = new detcomp();
                    tothab2.CONCEPTO = "";
                    tothab2.GLOSA = "";
                    tothab2.DEBE = totgendeb.ToString("###,###,###");
                    tothab2.HABER = totgenhab.ToString("###,###,###");
                    tothab2.CANTIDAD = "";
                    tothab2.ENTIDAD = "";
                    tothab2.RUT = "";
                    tothab2.IDS = "";
                    tothab2.CUENTA = "";
                    tothab2.DESCRIPCION = "";
                    lista.Add(tothab2);
                    resultado.mensaje = "Existen conceptos en la coleccion";
                    resultado.result = 1;
                    resultado.data = lista;
                }
                else
                {
                    resultado.mensaje = "No se han encontrado conceptos";
                    resultado.result = 0;
                }
                return resultado;
            }
            catch (System.Exception ex)
            {
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return resultado;
            }
        }
        public Resultado ProcesarComprobanteContableService(int empresa, int mes, int anio, string pago, string modulo)
        {
            Resultado resultado = new Resultado();
            return resultado;
        }
        public string BuscaCuenta(string cuenta, string BD_Cli)
        {
            string descripcion = "";
            SqlConnection cn;
            DataSet ds;
            SqlDataAdapter da;
            DataTable dt;
            string CadenaSqlCli = "Data Source=179.61.13.236,1433;Initial Catalog='" + BD_Cli + "';User ID=gycsolcl_gestionAdmin;Password=.7VzG#{Ty(Gvu{!:(fm}4:4/LT;TrustServerCertificate=True";
            cn = new SqlConnection(CadenaSqlCli);
            cn.Open();
            int cta = Convert.ToInt32(cuenta);
            da = new SqlDataAdapter("Select * from [dbo].plancuentasdet " +
                                     "where habilitado = 1 and contab = " + cta, cn);
            ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
            cn.Close();
            var data = new List<Conceptos>();
            string glosa = "";
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow h in dt.Rows)
                {
                    glosa = h["glosa"].ToString();
                }
            }
            descripcion = glosa;
            return descripcion;
        }
        //public int UltimoId(string tabla)
        //{
        //    int id = 0;
        //    var rutEmpresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();

        //    DataTable dt = f.Sql_Consulta("Select MAX(id) from " + tabla, rutEmpresa);
        //    DataRow dr = dt.Rows[0];
        //    id = Convert.ToInt32(dr["Column1"]);
        //    return id;
        //}
        //public int UltimoNumeroComprobante(int anio, int mes)
        //{
        //    int numero = 0;
        //    int proximo = 0;
        //    var rutEmpresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
        //    DataTable dt = f.Sql_Consulta("Select numero from [dbo].comprobantes " +
        //                             "where habilitado = 1 and year(fcontab) =" + anio + " and Month(fcontab) =" + mes, rutEmpresa);
        //    foreach (DataRow n in dt.Rows)
        //    {

        //        string num = n["numero"].ToString();
        //        if (num != null) proximo = Convert.ToInt32(num);
        //        if (proximo > numero) numero = proximo;
        //    }
        //    return numero;
        //}
        //public bool ExisteComprobante(string glosa)
        //{
        //    var rutEmpresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
        //    DataTable dt = f.Sql_Consulta("Select * from [dbo].comprobantes where habilitado = 1 and CONVERT(NVARCHAR(MAX),observ)= '" + glosa + "' ", rutEmpresa);
        //    if (dt.Rows.Count > 0) { return true; }
        //    return false;
        //}
        //public JsonResult MostrarDetalle(string ids)
        //{
        //    Resultado resultado = new Resultado();
        //    resultado.result = 0;
        //    var rutEmpresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString();
        //    try
        //    {

        //        string[] idsSeparados = ids.Split('*');
        //        int[] sel = new int[1000];
        //        int nsel = idsSeparados.Length - 1;
        //        List<detconcepto> lista = new List<detconcepto>();
        //        Decimal totdeb = 0;
        //        Decimal tothab = 0;
        //        Decimal cero = 0;
        //        for (int i = 0; i < nsel; i++)
        //        {
        //            int correl = Convert.ToInt32(idsSeparados[i]);
        //            remeresu resus = db.remeresu.Where(x => x.correl == correl).FirstOrDefault();
        //            if (resus != null)
        //            {
        //                tothab = tothab + (Decimal)resus.mto_pagar;
        //                totdeb = totdeb + (Decimal)resus.mto_pagar;
        //                detconcepto codigo = new detconcepto();
        //                codigo.RUT = resus.nrt_ruttr;
        //                codigo.NOMBRE = "";
        //                decimal valor = (Decimal)resus.mto_pagar;
        //                if (resus.cod_conce < 100)
        //                {
        //                    codigo.DEBE = valor.ToString("###,###,###");
        //                    codigo.HABER = cero.ToString("###,###,###");
        //                }
        //                else
        //                {
        //                    codigo.DEBE = cero.ToString("###,###,###");
        //                    codigo.HABER = valor.ToString("###,###,###");
        //                }
        //                var pers = db.PERSONA.Where(x => x.RUT == resus.nrt_ruttr).FirstOrDefault();
        //                if (pers != null) { codigo.NOMBRE = pers.APATERNO + " " + pers.AMATERNO + " " + pers.NOMBRE; }
        //                lista.Add(codigo);
        //            }
        //        }
        //        detconcepto codigo1 = new detconcepto();
        //        codigo1.RUT = "";
        //        codigo1.NOMBRE = "";
        //        codigo1.DEBE = totdeb.ToString("###,###,###");
        //        codigo1.HABER = tothab.ToString("###,###,###");
        //        lista.Add(codigo1);
        //        resultado.mensaje = "Existen conceptos en la coleccion";
        //        resultado.result = 1;
        //        resultado.data = lista;
        //    }
        //    catch (Exception ex)
        //    {
        //        var Asunto = "Error al consultar comprobantes de pago";
        //        var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
        //        resultado.result = -1;
        //        resultado.mensaje = "Fallo";
        //        return Json("");
        //    }
        //    return Json(resultado, JsonRequestBehavior.AllowGet);
        //}
        //public List<detconcepto> MostrarDetallesConcepto(string ids)
        //{
        //    List<detconcepto> lista = new List<detconcepto>();
        //    try
        //    {

        //        string[] idsSeparados = ids.Split('*');
        //        int[] sel = new int[1000];
        //        int nsel = idsSeparados.Length - 1;
        //        Decimal totdeb = 0;
        //        Decimal tothab = 0;
        //        Decimal cero = 0;
        //        for (int i = 0; i < nsel; i++)
        //        {
        //            int correl = Convert.ToInt32(idsSeparados[i]);
        //            remeresu resus = db.remeresu.Where(x => x.correl == correl).FirstOrDefault();
        //            if (resus != null)
        //            {
        //                tothab = tothab + (Decimal)resus.mto_pagar;
        //                totdeb = totdeb + (Decimal)resus.mto_pagar;
        //                detconcepto codigo = new detconcepto();
        //                codigo.RUT = resus.nrt_ruttr;
        //                var pers = db.PERSONA.Where(x => x.RUT == resus.nrt_ruttr).FirstOrDefault();
        //                if (pers != null) { codigo.NOMBRE = pers.APATERNO + " " + pers.AMATERNO + " " + pers.NOMBRE; }
        //                codigo.NOMBRE = "";
        //                decimal valor = (Decimal)resus.mto_pagar;
        //                if (resus.cod_conce < 100)
        //                {
        //                    codigo.DEBE = valor.ToString();
        //                    codigo.HABER = cero.ToString();
        //                }
        //                else
        //                {
        //                    codigo.DEBE = cero.ToString();
        //                    codigo.HABER = valor.ToString();
        //                }
        //                lista.Add(codigo);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
        //        return null;
        //    }
        //    return lista;
        //}
    }
}
public class detcomp
{

    public string CONCEPTO ;
    public string GLOSA ;
    public string DEHA ;
    public string DEBE ;
    public string HABER ;
    public string CUENTA ;
    public string DESCRIPCION ;
    public string RUT ;
    public int TIPO ;
    public string ENTIDAD ;
    public string CANTIDAD ;
    public string IDS ;
    public int GRUPOS ;
    public List<detconcepto> grupos ;
    public detcomp()
    {
        grupos = new List<detconcepto>();
    }

}
public class detresultado
{
    public int id;
    public string rut;
    public string descripcion;
    public int concepto;
    public decimal monto;
}
public class detconcepto
    {

        public string RUT ;
        public string NOMBRE ;
        public string DEBE ;
        public string HABER ;
    }
    public class ComprobantesVM
    {
        public string Comprobante ;
        public DateTime Fcompro ;
        public DateTime Fcontab ;
        public string Observ ;
        public string TipoComprob ;

        public List<DetaComprobanteVM> Detalles ;
        public ComprobantesVM()
        {
            Detalles = new List<DetaComprobanteVM>();
        }
    }
    public class DetaComprobanteVM
    {
        public string Cuenta ;
        public string Glosa ;
        public string Debehaber ;
        public double Monto ;
        public string Notas ;
        public int iddet ; 

        public List<FlagsComprobantesVM> Flags ;
        public DetaComprobanteVM()
        {
            Flags = new List<FlagsComprobantesVM>();
        }
    }

    public class FlagsComprobantesVM
    {
        public string cod_rut ;
        public string auxi_001 ;
        public string tip_docu ;
        public string num_docu ;
        public DateTime fec_emis ;
        public DateTime fec_venc ;
        public string cod_impu ;
        public string num_cont ;
        public double mto_dola ;
        public string cod_rcaj ;
        public string cod_remu ;
        public int tipo_aux ;
        public string aux ;
        public string cod_presup ;

    }