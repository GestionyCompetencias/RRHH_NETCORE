using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Microsoft.Extensions.Configuration.Ini;

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
        public List<detcomp> ListarComprobanteContableService(int idEmpresa, int mes, int anio, string pago,string modulo);

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

        public List<detcomp> ListarComprobanteContableService(int empresa, int mes, int anio, string pago,string modulo)
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
                f.EjecutarConsultaSQLCli("SELECT resultados.id,resultados.rutTrabajador, resultados.concepto,resultados.descripcion,resultados.monto " +
                                           " FROM resultados " +
                                            "WHERE resultados.habilitado = 1 and resultados.pago ='" + pago + "' and resultados.monto > 0 " +
                                            " and resultados.fechaPago >= '"+inistr+"' and resultados.fechaPago <= '"+finstr+"' "+
                                            " Order by resultados.concepto", BD_Cli);

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
                int codigo = 0;
                if (conceptos.Count > 0)
                {
                    var conc = conceptos.GroupBy(x => x.concepto).Select(g => new { concepto = g.Key }).ToList();
                    List<detcomp> lista = new List<detcomp>();
                    foreach (var c in conc)
                    {
                        codigo = c.concepto;
                        ConversionContableBaseVM regconv = BuscaConversion(modulo, c.concepto, "", "D", BD_Cli);
                        var detalle = conceptos.Where(x => x.concepto == c.concepto).ToList();
                        if(regconv!= null)
                        {
                            detcomp tothab = new detcomp();
                            foreach (var d in detalle)
                            {
                                tothab.CUENTA = regconv.cuenta;
                                tothab.DESCRIPCION = Generales.BuscaCuenta(regconv.cuenta, BD_Cli);
                                tothab.CONCEPTO = codigo.ToString();
                                totaldeb = (decimal)(totaldeb + d.monto);
                                totgendeb = (decimal)(totgendeb + d.monto);
                                cantidad++;
                                ids = ids + d.id + "*";
                            }
                            tothab.DEBE = totaldeb.ToString("###,###,###");
                            tothab.HABER = cero.ToString("###,###,###");
                            tothab.CANTIDAD = cantidad.ToString("###");
                            tothab.ENTIDAD = "";
                            tothab.GLOSA = Generales.BuscaConcepto(codigo, BD_Cli);
                            tothab.IDS = ids;
                            totaldeb = 0;
                            totalhab = 0;
                            cantidad = 0;
                            ids = null;
                            lista.Add(tothab);
                        }
                    }
                    detcomp tothab2 = new detcomp();
                    tothab2.CONCEPTO = "1001";
                    tothab2.GLOSA = "";
                    tothab2.DEBE = totgendeb.ToString("###,###,###");
                    tothab2.HABER = totgenhab.ToString("###,###,###");
                    tothab2.CANTIDAD = "";
                    tothab2.ENTIDAD = "";
                    tothab2.RUT = "";
                    tothab2.IDS = "";
                    tothab2.CUENTA = "";
                    tothab2.DESCRIPCION = "";

                    totaldeb = 0;
                    totalhab = 0;

                    var concdc = conceptos.GroupBy(x => x.concepto).Select(g => new { concepto = g.Key }).ToList();
                    List<detcomp> listadc = new List<detcomp>();
                    foreach (var dct in concdc)
                    {
                        codigo = dct.concepto;
                        if (codigo == 2300) codigo = 999;
                        ConversionContableBaseVM regconv = BuscaConversion(modulo, dct.concepto,"","H", BD_Cli);
                        var detalle = conceptos.Where(x => x.concepto == dct.concepto).ToList();
                        if (regconv != null)
                        {
                            detcomp totdct = new detcomp();
                            foreach (var f in detalle)
                            {
                                totdct.CUENTA = regconv.cuenta;
                                totdct.DESCRIPCION = Generales.BuscaCuenta(regconv.cuenta, BD_Cli);
                                totdct.CONCEPTO = codigo.ToString();
                                totalhab = (decimal)(totalhab + f.monto);
                                totgenhab = (decimal)(totgenhab + f.monto);
                                cantidad++;
                                ids = ids + f.id + "*";
                            }
                            totdct.DEBE = cero.ToString("###,###,###");
                            totdct.HABER = totalhab.ToString("###,###,###");
                            totdct.CANTIDAD = cantidad.ToString("###");
                            totdct.ENTIDAD = "";
                            totdct.GLOSA = Generales.BuscaConcepto(codigo, BD_Cli);
                            totdct.IDS = ids;
                            totaldeb = 0;
                            totalhab = 0;
                            cantidad = 0;
                            ids = null;
                            lista.Add(totdct);
                        }
                    }

                    detcomp totdct2 = new detcomp();
                    totdct2.CONCEPTO = "9999";
                    totdct2.GLOSA = "";
                    totdct2.DEBE = totgendeb.ToString("###,###,###");
                    totdct2.HABER = totgenhab.ToString("###,###,###");
                    totdct2.CANTIDAD = "";
                    totdct2.ENTIDAD = "";
                    totdct2.RUT = "";
                    totdct2.IDS = "";
                    totdct2.CUENTA = "";
                    totdct2.DESCRIPCION = "";
                    lista.Add(totdct2);
 
                    resultado.mensaje = "Existen conceptos en la coleccion";
                    resultado.result = 1;
                    resultado.data = lista;
                    return lista;
                }
                else
                {
                    resultado.mensaje = "No se han encontrado conceptos";
                    resultado.result = 0;
                }
                return null;
            }
            catch (System.Exception ex)
            {
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return null;
            }
        }
        public Resultado ProcesarComprobanteContableService(int empresa, int mes, int anio, string pago, string modulo)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "Fallo al insertar comprobante";
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            DateTime hoy = DateTime.Now;
            try
            {
                ComprobantesVM comp = GeneraComprobante(mes, anio, pago,modulo ,BD_Cli);
                if (ExisteComprobante(comp.Observ, BD_Cli))
                {
                    resultado.result = 0;
                    resultado.mensaje = "Comprobante fue generado anteriormente";
                }
                else
                {
                    if (InsertaComprobante(comp, BD_Cli))
                    {
                        resultado.result = 1;
                        resultado.mensaje = "Comprobante ingresado con exito";
                    }
                }
            }
            catch (Exception ex)
            {
                var Asunto = "Error al consultar comprobantes de pago";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                resultado.result = -1;
                resultado.mensaje = "Fallo";
                return null;
            }
            return resultado;
        }
        public ComprobantesVM GeneraComprobante(int mes, int anio, string pago,string modulo,string BD_Cli)
        {
            DateTime ini = new DateTime(anio, mes, 1);
            DateTime fin = f.UltimoDia(ini);
            DateTime hoy = DateTime.Now.Date;
            string fechoy = hoy.ToString("yyyy'-'MM'-'dd");
            string inistr = ini.ToString("yyyy'-'MM'-'dd");
            string finstr = fin.ToString("yyy'-'MM'-'dd");
            ComprobantesVM comp = new ComprobantesVM();
            try
            {
                List<detcomp> lista = new List<detcomp>();
                int oldhab = 0;
                f.EjecutarConsultaSQLCli("SELECT resultados.id,resultados.rutTrabajador, resultados.concepto,resultados.descripcion,resultados.monto " +
                                           " FROM resultados " +
                                            "WHERE resultados.habilitado = 1 and resultados.pago ='" + pago + "' and resultados.monto > 0 " +
                                            " and resultados.fechaPago >= '" + inistr + "' and resultados.fechaPago <= '" + finstr + "' " +
                                            " and ( resultados.concepto < 1000 or resultados.concepto = 2300) Order by resultados.concepto", BD_Cli);

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
                                     monto = decimal.Parse(dr["monto"].ToString())
                                 }).ToList();
                string oldglo = "";
                string oldrut = "";
                decimal totaldeb = 0;
                decimal totalhab = 0;
                decimal cero = 0;
                string ids = null;
                int old_concepto = 0;
                DateTime fecpro = new DateTime(anio, mes, 1);
                fecpro = f.UltimoDia(fecpro);


                    comp.Fcompro = hoy;
                    comp.Fcontab = fecpro;
                    comp.Observ = "Liquidación de remuneraciones fecha " + fecpro.ToString("dd'-'MM'-'yyyy");
                    comp.TipoComprob = "3";

                    foreach (var c in conceptos)
                    {
                        if (oldhab != 0 && oldhab != c.concepto)
                        {
                            detcomp tothab = new detcomp();
                            tothab.CONCEPTO = oldhab.ToString();
                            tothab.GLOSA = oldglo;
                            tothab.RUT = oldrut;
                            ConversionContableBaseVM regconv = new ConversionContableBaseVM();
                            if (c.concepto <100)
                            {
                                regconv = BuscaConversion(modulo, c.concepto, "", "D", BD_Cli);
                            }
                            else
                            {
                                regconv = BuscaConversion(modulo, c.concepto, "", "H", BD_Cli);
                            }
                            if (regconv != null)
                            {
                                tothab.CUENTA = regconv.cuenta;
                                tothab.DESCRIPCION = Generales.BuscaCuenta(regconv.cuenta, BD_Cli);
                                tothab.GRUPOS = regconv.agrupacion;
                                tothab.ENTIDAD = regconv.codigoauxiliar;
                                tothab.TIPO = regconv.tipoauxiliar;
                                tothab.DEHA = regconv.debehaber;
                                if (regconv.agrupacion != 2)
                                {
                                    tothab.grupos = MostrarDetallesConcepto(ids, BD_Cli);
                                }
                            }
                            tothab.IDS = ids;
                            if (oldhab < 100)
                            {
                                tothab.DEBE = totaldeb.ToString();
                                tothab.HABER = cero.ToString();
                            }
                            else
                            {
                                tothab.DEBE = cero.ToString();
                                tothab.HABER = totalhab.ToString();
                            }
                            lista.Add(tothab);
                            totaldeb = 0;
                            totalhab = 0;
                            ids = null;
                        }
                        oldglo = c.descripcion;
                        oldrut = c.rut;
                        oldhab = c.concepto;
                        old_concepto = c.concepto;
                        ids = ids + c.id + "*";
                        if (oldhab < 100)
                        {
                            totaldeb = (decimal)(totaldeb + c.monto);
                        }
                        else
                        {
                            totalhab = (decimal)(totalhab + c.monto);
                        }
                    }
                    detcomp tothab1 = new detcomp();
                    tothab1.CONCEPTO = oldhab.ToString();
                    tothab1.GLOSA = oldglo;
                    if (oldhab < 100)
                    {
                        tothab1.DEBE = totaldeb.ToString();
                        tothab1.HABER = cero.ToString();
                    }
                    else
                    {
                        tothab1.HABER = totalhab.ToString();
                        tothab1.DEBE = cero.ToString();
                    }
                    tothab1.RUT = oldrut;
                    ConversionContableBaseVM conv1 = new ConversionContableBaseVM();
                    if (old_concepto < 100)
                    {
                        conv1 = BuscaConversion(modulo, old_concepto, "", "D", BD_Cli);
                    }
                    else
                    {
                        conv1 = BuscaConversion(modulo, old_concepto, "", "H", BD_Cli);
                    }
                    if (conv1 != null)
                    {
                        tothab1.CUENTA = conv1.cuenta;
                        tothab1.DESCRIPCION = Generales.BuscaCuenta(conv1.cuenta,BD_Cli);
                        tothab1.GRUPOS = conv1.agrupacion;
                        tothab1.ENTIDAD = conv1.codigoauxiliar;
                        tothab1.TIPO = conv1.tipoauxiliar;
                        if (conv1.agrupacion != 2)
                        {
                            tothab1.grupos = MostrarDetallesConcepto(ids,BD_Cli);
                        }
                    }
                    tothab1.IDS = ids;
                    lista.Add(tothab1);

                }
                List<DetaComprobanteVM> detalle = new List<DetaComprobanteVM>();
                int con = 0;
                foreach (var d in lista)
                {
                    if (d.CONCEPTO == "103")
                        con = Convert.ToInt32(d.CONCEPTO);
                    if (d.GRUPOS == 2)
                    {
                        DetaComprobanteVM detcom = new DetaComprobanteVM();
                        detcom.Cuenta = d.CUENTA;
                        detcom.Glosa = d.GLOSA;
                        detcom.Debehaber = "H";
                        detcom.Monto = 0;
                        if (d.DEBE != null && d.DEBE != "0")
                        {
                            detcom.Debehaber = "D";
                            detcom.Monto = Convert.ToDouble(d.DEBE);
                        }
                        else
                        {
                            if (d.HABER != null) detcom.Monto = Convert.ToDouble(d.HABER);
                            detcom.Debehaber = "H";
                        }
                        FlagsComprobantesVM flags = new FlagsComprobantesVM();
                        flags.cod_rut = d.ENTIDAD;
                        flags.auxi_001 = "";
                        flags.tip_docu = "";
                        flags.num_docu = "";
                        flags.fec_emis = hoy;
                        flags.fec_venc = hoy;
                        flags.cod_impu = "";
                        flags.num_cont = "";
                        flags.mto_dola = 0;
                        flags.cod_rcaj = "";
                        flags.cod_remu = "";
                        flags.tipo_aux = 0;
                        flags.aux = "";
                        flags.cod_presup = "";
                        detcom.Flags.Add(flags);
                        detalle.Add(detcom);
                    }
                    else
                    {
                        List<detconcepto> agrupacion = new List<detconcepto>();

                        if (d.GRUPOS == 1)
                        {
                            agrupacion = d.grupos;
                            foreach (var i in agrupacion)
                            {
                                DetaComprobanteVM detcom = new DetaComprobanteVM();
                                detcom.Cuenta = d.CUENTA;
                                detcom.Glosa = d.GLOSA;
                                detcom.Debehaber = "H";
                                detcom.Monto = 0;
                                if (i.DEBE != null && i.DEBE != "0")
                                {
                                    detcom.Debehaber = "D";
                                    detcom.Monto = Convert.ToDouble(i.DEBE);
                                }
                                else
                                {
                                    if (i.HABER != null) detcom.Monto = Convert.ToDouble(i.HABER);
                                    detcom.Debehaber = "H";
                                }
                                FlagsComprobantesVM flags = new FlagsComprobantesVM();
                                flags.cod_rut = i.RUT;
                                flags.auxi_001 = "";
                                flags.tip_docu = "";
                                flags.num_docu = "";
                                flags.fec_emis = hoy;
                                flags.fec_venc = hoy;
                                flags.cod_impu = "";
                                flags.num_cont = "";
                                flags.mto_dola = 0;
                                flags.cod_rcaj = "";
                                flags.cod_remu = "";
                                flags.tipo_aux = 0;
                                flags.aux = "";
                                flags.cod_presup = "";
                                detcom.Flags.Add(flags);
                                detalle.Add(detcom);
                            }
                        }
                        List<detresultado> resu = new List<detresultado>();
                        if (d.GRUPOS == 3)
                        {
                            if (d.TIPO == 3)
                            {
                                resu = BuscaResultados(con,pago, inistr, finstr, BD_Cli);
                            }
                            if (d.TIPO == 4)
                            {
                                resu = BuscaResultados(921,pago, inistr, finstr, BD_Cli);
                                oldhab = 921;
                            }
                            if (d.TIPO == 5)
                            {
                                resu = BuscaResultados(910,pago, inistr, finstr, BD_Cli);
                            }
                            if (d.TIPO == 6)
                            {
                                resu = BuscaResultados(910,pago, inistr, finstr,  BD_Cli);
                            }
                            double total = 0;
                            int oldcnt = 0;
                            foreach (var item in resu)
                            {
                                if (oldcnt != 0 && oldcnt != item.concepto)
                                {
                                    string auxiliar = oldcnt.ToString();
                                    ConversionContableBaseVM conv = BuscaConversion(modulo, oldhab, auxiliar,"D",BD_Cli);
                                    DetaComprobanteVM detcom = new DetaComprobanteVM();
                                    detcom.Cuenta = conv.cuenta;
                                    detcom.Glosa = d.GLOSA;
                                    detcom.Debehaber = conv.debehaber;
                                    detcom.Monto = total;
                                    FlagsComprobantesVM flags = new FlagsComprobantesVM();
                                    flags.cod_rut = "";
                                    flags.auxi_001 = "";
                                    flags.tip_docu = "";
                                    flags.num_docu = "";
                                    flags.fec_emis = hoy;
                                    flags.fec_venc = hoy;
                                    flags.cod_impu = "";
                                    flags.num_cont = "";
                                    flags.mto_dola = 0;
                                    flags.cod_rcaj = "";
                                    flags.cod_remu = "";
                                    flags.tipo_aux = 0;
                                    flags.aux = "";
                                    flags.cod_presup = "";
                                    detcom.Flags.Add(flags);
                                    detalle.Add(detcom);
                                    total = 0;
                                }
                                oldcnt = (int)item.concepto;
                                Double monto = (double)item.monto;
                                total = total + monto;
                            }

                            string auxiliar1 = oldcnt.ToString();
                            ConversionContableBaseVM conv1 = BuscaConversion(modulo, con, "","H", BD_Cli);
                            DetaComprobanteVM detcom1 = new DetaComprobanteVM();
                            detcom1.Cuenta = conv1.cuenta;
                            detcom1.Glosa = d.GLOSA;
                            detcom1.Debehaber = conv1.debehaber;
                            detcom1.Monto = total;
                            FlagsComprobantesVM flags1 = new FlagsComprobantesVM();
                            flags1.cod_rut = auxiliar1;
                            flags1.auxi_001 = "";
                            flags1.tip_docu = "";
                            flags1.num_docu = "";
                            flags1.fec_emis = hoy;
                            flags1.fec_venc = hoy;
                            flags1.cod_impu = "";
                            flags1.num_cont = "";
                            flags1.mto_dola = 0;
                            flags1.cod_rcaj = "";
                            flags1.cod_remu = "";
                            flags1.tipo_aux = 0;
                            flags1.aux = "";
                            flags1.cod_presup = "";
                            detcom1.Flags.Add(flags1);
                            detalle.Add(detcom1);
                        }
                    }
                }
                comp.Detalles = detalle;
                return comp;
            }
            catch (Exception ex)
            {
                return comp;
            }
        }


        public int UltimoNumeroComprobante(int anio, int mes, string BD_Cli)
        {
            int numero = 0;
            int proximo = 0;
            f.EjecutarConsultaSQLCli("Select numero from [dbo].comprobantes " +
                                     "where habilitado = 1 and year(fcontab) =" + anio + " and Month(fcontab) =" + mes, BD_Cli);
            if (f.Tabla.Rows.Count > 0)
                {
                foreach(DataRow dr in f.Tabla.Rows)
                {
                    string num = dr["numero"].ToString();
                    if (num != null) proximo = Convert.ToInt32(num);
                    if (proximo > numero) numero = proximo;
                }
            }
            return numero;
        }
        public bool ExisteComprobante(string glosa,string BD_Cli)
        {
            f.EjecutarConsultaSQLCli("Select * from [dbo].comprobantes where habilitado = 1 and CONVERT(NVARCHAR(MAX),observ)= '" + glosa + "' ", BD_Cli);
            if (f.Tabla.Rows.Count > 0) { return true; }
            return false;
        }
        public List<detconcepto> MostrarDetalle(string ids, string BD_Cli)
        {
            List<detconcepto> lista = new List<detconcepto>();
            try
            {

                string[] idsSeparados = ids.Split('*');
                int[] sel = new int[1000];
                int nsel = idsSeparados.Length - 1;
                Decimal totdeb = 0;
                Decimal tothab = 0;
                Decimal cero = 0;
                decimal monto = 0;
                string rutTrabajador = null;
                int concepto = 0;
                for (int i = 0; i < nsel; i++)
                {
                    int correl = Convert.ToInt32(idsSeparados[i]);
                    f.EjecutarConsultaSQLCli("Select * from resultados " +
                                             "where id ="+correl, BD_Cli);
                    if (f.Tabla.Rows.Count > 0)
                    {
                        foreach (DataRow dr in f.Tabla.Rows)
                        {
                            monto = decimal.Parse(dr["monto"].ToString());
                            rutTrabajador = dr["rutTrabajador"].ToString();
                            concepto = int.Parse(dr["concepto"].ToString());
                        }
                        tothab = tothab + (Decimal)monto;
                        totdeb = totdeb + (Decimal)monto;
                        detconcepto codigo = new detconcepto();
                        codigo.RUT = rutTrabajador;
                        codigo.NOMBRE = "";
                        decimal valor = (Decimal)monto;
                        if (concepto < 100)
                        {
                            codigo.DEBE = valor.ToString("###,###,###");
                            codigo.HABER = cero.ToString("###,###,###");
                        }
                        else
                        {
                            codigo.DEBE = cero.ToString("###,###,###");
                            codigo.HABER = valor.ToString("###,###,###");
                        }
                        var pers = Generales.BuscaPersona(rutTrabajador, BD_Cli);
                        if (pers != null) { codigo.NOMBRE = pers.Apellidos + " " + pers.Nombres; }
                        lista.Add(codigo);
                    }
                }
                detconcepto codigo1 = new detconcepto();
                codigo1.RUT = "";
                codigo1.NOMBRE = "";
                codigo1.DEBE = totdeb.ToString("###,###,###");
                codigo1.HABER = tothab.ToString("###,###,###");
                lista.Add(codigo1);
            }
            catch (Exception ex)
            {
                var Asunto = "Error al consultar comprobantes de pago";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                return null;
            }
            return lista;
        }
        public bool InsertaComprobante(ComprobantesVM comp, string BD_Cli)
        {
            string fechaing = comp.Fcompro.ToString("yyyy'-'MM'-'dd");
            string fechacon = comp.Fcontab.ToString("yyyy'-'MM'-'dd");
            int mes = comp.Fcontab.Month;
            int anio = comp.Fcontab.Year;
            int nro = UltimoNumeroComprobante(anio, mes,BD_Cli) + 1;
            string query = "INSERT INTO [dbo].[comprobantes] ([numero], [fcomprob], [fcontab], [tipocomprob], [observ], [habilitado])" +
            " VALUES ('" + nro + "', '" + fechaing + "', '" + fechacon + "',3, '" + comp.Observ + "', 1)";
            if (f.EjecutarQuerySQLCli(query, BD_Cli))
            {
                int idcomprob = UltimoId("[dbo].comprobantes",BD_Cli);
                foreach (var d in comp.Detalles)
                {
                    double debe = 0;
                    double haber = 0;
                    if (d.Debehaber == "D") debe = d.Monto;
                    else haber = d.Monto;
                    string query1 = "INSERT INTO [dbo].[comprobantesdet] ([idcomprob],[cuenta],[debe],[haber] ,[notas])" +
                        "VALUES (" + idcomprob + ", '" + d.Cuenta + "', " + debe + ", " + haber + ", '" + d.Glosa + "')";
                    if (f.EjecutarQuerySQLCli(query1, BD_Cli))
                    {
                        FlagsComprobantesVM flags = new FlagsComprobantesVM();
                        flags = d.Flags.FirstOrDefault();
                        string fecemi = flags.fec_emis.ToString("yyyy'-'MM'-'dd");
                        string fecven = flags.fec_venc.ToString("yyyy'-'MM'-'dd");
                        int idcomprobdet = UltimoId("[dbo].comprobantesdet",BD_Cli);
                        string query2 = "INSERT INTO[dbo].[comprobantesflag]  ([idcomprobdet], [cod_rut], [auxi_001], [tip_docu], [num_docu], [fec_emis], [fec_venc], [cod_impu]" +
                                " , [num_cont], [mto_dola], [cod_rcaj], [cod_remu], [tipo_aux], [aux], [cod_presup])" +
                                "  VALUES ( " + idcomprobdet + ", '" + flags.cod_rut + "', '" + flags.auxi_001 + "', '" + flags.tip_docu + "', '" +
                                flags.num_docu + "', '" + fecemi + "', '" + fecven + "', '" +
                                flags.cod_impu + "', '" + flags.num_cont + "', " + flags.mto_dola + ", '" + flags.cod_rcaj +
                                "', '" + flags.cod_remu + "', " + flags.tipo_aux + ", '" + flags.aux + "', '" + flags.cod_presup + "' )";
                        if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                        {
                            continue;
                        }
                    }
                }
                return true;

            }
            return false;
        }
        public int UltimoId(string tabla,string BD_Cli)
        {
            int id = 0;
            BD_Cli = BD_Cli.Replace("remuneracion","contable");
            f.EjecutarConsultaSQLCli("Select MAX(id) from " + tabla, BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                foreach (DataRow dr in f.Tabla.Rows)
                {
                    id = Convert.ToInt32(dr["Column1"]);
                }
            }
            return id;
        }

        public List<detconcepto> MostrarDetallesConcepto(string ids, string BD_Cli)
        {
            List<detconcepto> lista = new List<detconcepto>();
            try
            {

                string[] idsSeparados = ids.Split('*');
                int[] sel = new int[1000];
                int nsel = idsSeparados.Length - 1;
                Decimal totdeb = 0;
                Decimal tothab = 0;
                Decimal cero = 0;
                decimal monto = 0;
                string rut = null;
                int concepto = 0;
                for (int i = 0; i < nsel; i++)
                {
                    int correl = Convert.ToInt32(idsSeparados[i]);
                    f.EjecutarConsultaSQLCli("Select * from resultados " +
                                             "where id =" + correl, BD_Cli);
                    if (f.Tabla.Rows.Count > 0)
                    {
                        foreach (DataRow dr in f.Tabla.Rows)
                        {
                            monto = decimal.Parse(dr["monto"].ToString());
                            rut = dr["rutTrabajador"].ToString();
                            concepto = int.Parse(dr["concepto"].ToString());

                            tothab = tothab + (Decimal)monto;
                            totdeb = totdeb + (Decimal)monto;
                            detconcepto codigo = new detconcepto();
                            codigo.RUT = rut;
                            var pers = Generales.BuscaPersona(rut,BD_Cli);
                            if (pers != null) { codigo.NOMBRE = pers.Apellidos + " " + pers.Nombres ; }
                            codigo.NOMBRE = "";
                            decimal valor = (Decimal)monto;
                            if (concepto < 100)
                            {
                                codigo.DEBE = valor.ToString();
                                codigo.HABER = cero.ToString();
                            }
                            else
                            {
                                codigo.DEBE = cero.ToString();
                                codigo.HABER = valor.ToString();
                            }
                            lista.Add(codigo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                return null;
            }
            return lista;
        }
        public ConversionContableBaseVM BuscaConversion(string modulo,int concepto , string auxiliar,string dh, string BD_Cli)
        {
            if(auxiliar == "" || auxiliar =="0")
            {
                f.EjecutarConsultaSQLCli("SELECT conversionContable.id,conversionContable.modulo,conversionContable.pago,conversionContable.concepto, " +
                                          "conversionContable.cuenta,conversionContable.tipoAuxiliar, conversionContable.codigoAuxiliar, " +
                                          "conversionContable.debeHaber, conversionContable.tipoVencimiento, conversionContable.diaVencimiento,  " +
                                          "conversionContable.mesVencimiento, conversionContable.agrupacion, conversionContable.provision  " +
                                         " FROM conversionContable " +
                                          "WHERE conversionContable.habilitado = 1 and debeHaber = '"+dh+"' and conversionContable.modulo ='" + modulo +
                                          "' and conversionContable.concepto =" + concepto, BD_Cli);

            }
            else
            {
                f.EjecutarConsultaSQLCli("SELECT conversionContable.id,conversionContable.modulo,conversionContable.pago,conversionContable.concepto, " +
                                          "conversionContable.cuenta,conversionContable.tipoAuxiliar, conversionContable.codigoAuxiliar, " +
                                          "conversionContable.debeHaber, conversionContable.tipoVencimiento, conversionContable.diaVencimiento,  " +
                                          "conversionContable.mesVencimiento, conversionContable.agrupacion, conversionContable.provision  " +
                                         " FROM conversionContable " +
                                          "WHERE conversionContable.habilitado = 1 and debeHaber = '"+dh+"' and conversionContable.modulo ='" + modulo +
                                          "' and conversionContable.concepto =" + concepto +" and conversionContable.codigoAuxiliar = '"+auxiliar+"' ", BD_Cli);

            }

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
            return regconv;
        }
        public List<detresultado> BuscaResultados(int concepto, string pago, string inistr,string finstr, string BD_Cli)
        {
            f.EjecutarConsultaSQLCli("SELECT resultados.id,resultados.rutTrabajador, resultados.concepto,resultados.descripcion,resultados.monto " +
                                       " FROM resultados " +
                                        "WHERE resultados.habilitado = 1 and resultados.pago ='" + pago + "' and resultados.monto > 0 " +
                                        " and resultados.fechaPago >= '" + inistr + "' and resultados.fechaPago <= '" + finstr + "' " +
                                        "  and resultados.concepto = "+concepto+ "Order by resultados.concepto", BD_Cli);

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
                                 monto = decimal.Parse(dr["monto"].ToString())
                             }).ToList();
            }
            return conceptos;
        }

    }
}
public class detcomp
{

    public string CONCEPTO { get; set; }
    public string GLOSA { get; set; }
    public string DEHA { get; set; }
    public string DEBE { get; set; }
    public string HABER { get; set; }
    public string CUENTA { get; set; }
    public string DESCRIPCION { get; set; }
    public string RUT { get; set; }
    public int TIPO { get; set; }
    public string ENTIDAD { get; set; }
    public string CANTIDAD { get; set; }
    public string IDS { get; set; }
    public int GRUPOS { get; set; }
    public List<detconcepto> grupos ;
    public detcomp()
    {
        grupos = new List<detconcepto>();
    }

}
public class detresultado
{
    public int id { get; set; }
    public string rut { get; set; }
    public string descripcion { get; set; }
    public int concepto { get; set; }
    public decimal monto { get; set; }
}
public class detconcepto
    {

        public string RUT { get; set; }
    public string NOMBRE { get; set; }
    public string DEBE { get; set; }
    public string HABER { get; set; }
}
    public class ComprobantesVM
    {
        public string Comprobante { get; set; }
    public DateTime Fcompro { get; set; }
    public DateTime Fcontab { get; set; }
    public string Observ { get; set; }
    public string TipoComprob { get; set; }

    public List<DetaComprobanteVM> Detalles ;
        public ComprobantesVM()
        {
            Detalles = new List<DetaComprobanteVM>();
        }
    }
    public class DetaComprobanteVM
    {
        public string Cuenta  { get; set; }
        public string Glosa  { get; set; }
        public string Debehaber  { get; set; }
        public double Monto  { get; set; }
        public string Notas  { get; set; }
        public int iddet  { get; set; } 

        public List<FlagsComprobantesVM> Flags ;
        public DetaComprobanteVM()
        {
            Flags = new List<FlagsComprobantesVM>();
        }
    }

    public class FlagsComprobantesVM
    {
        public string cod_rut  { get; set; }
        public string auxi_001  { get; set; }
        public string tip_docu  { get; set; }
        public string num_docu  { get; set; }
        public DateTime fec_emis  { get; set; }
        public DateTime fec_venc  { get; set; }
        public string cod_impu  { get; set; }
        public string num_cont  { get; set; }
        public double mto_dola  { get; set; }
        public string cod_rcaj  { get; set; }
        public string cod_remu  { get; set; }
        public int tipo_aux  { get; set; }
        public string aux  { get; set; }
        public string cod_presup  { get; set; }

    }