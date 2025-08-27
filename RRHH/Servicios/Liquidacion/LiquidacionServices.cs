using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;
using Newtonsoft.Json;

namespace RRHH.Servicios.Liquidacion
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface ILiquidacionService
    {
        /// <summary>
        /// Genera lista de haber informados.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes del haber informados que desea listar</param>
        /// <param name="anio">año del haber informados que desea listar</param>
        /// <param name="pago">pago del haber informados que desea listar</param>
        /// <returns>Lista de haberes informados</returns>
        public Resultado ProcesarLiquidacionService(int idEmpresa, int mes, int anio, string pago, string usuario);

    }

    public class LiquidacionService : ILiquidacionService
    {
        private readonly IDatabaseManager _databaseManager;
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Generales Generales = new Generales();
        private Seguridad seguridad = new Seguridad();

        haber[] haberes = new haber[100];
        descuentos[] desctos = new descuentos[1000];
        Decimal[] tramo = new decimal[10];
        Decimal[] tasa = new decimal[10];
        Decimal[] rebaja = new decimal[10];
        Decimal[] valorcarga = new decimal[10];
        parametrosPago para = new parametrosPago();
        totales tot = new totales();
        tiempo tie = new tiempo();
        trabajo tra = new trabajo();
        List<afps> listaafp = new List<afps>();
        string BD_Cli = null;
        DateTime fechainicio, fechatermino;
        string fechaproceso, fechainiciostr, fechaterminostr;
        public LiquidacionService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public Resultado ProcesarLiquidacionService(int empresa, int mes, int anio, string pago, string usuario)
        {
            string RutEmpresa = f.obtenerRUT(empresa);
            BD_Cli = "remuneracion_" + RutEmpresa;
            Resultado resultado = new Resultado();
            fechainicio = new DateTime(anio, mes, 1);
            fechatermino = f.UltimoDia(fechainicio);
            fechainiciostr = fechainicio.Date.ToString("yyyy'-'MM'-'dd");
            fechaterminostr = fechatermino.Date.ToString("yyyy'-'MM'-'dd");
            fechaproceso = fechatermino.ToString();
            try
            {
                if (MesCerrado())
                {
                    resultado.result = 0;
                    resultado.mensaje = "Proceso se encuentra cerrado";
                    return resultado;
                }
                GrabaBitacoraProcesos(usuario, pago);
                BorraResultados();
                if (CargaHaberes(empresa) == false)
                {
                    resultado.mensaje = "No se cargaron haberes";
                    resultado.result = 0;
                    return resultado;
                }
                if (CargaDescuentos(empresa) == false)
                {
                    resultado.mensaje = "No se cargaron descuentos";
                    resultado.result = 0;
                    return resultado;
                }
                if (CargaParametros(empresa) == false)
                {
                    resultado.mensaje = "No se cargaron parametros";
                    resultado.result = 0;
                    return resultado;
                }

                if (CargaAfps(empresa) == false)
                {
                    resultado.mensaje = "No se cargaron afps";
                    resultado.result = 0;
                    return resultado;
                }


                f.EjecutarConsultaSQLCli("SELECT personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf,trabajadores.nrohijos, " +
                                            "contratos.id , contratos.contrato, contratos.idpersona, contratos.sueldoBase, " +
                                            "contratos.inicio, contratos.termino, contratos.idTipoContrato, contratos.idfaena,  " +
                                            "contratos.idcargo,  contratos.idcentroCosto, contratos.idjornada,  contratos.observaciones,  " +
                                            "contratos.firmaTrabajador,  contratos.firmaEmpresa, contratos.rutValidador,  contratos.fechaValidacion,  " +
                                            "contratos.rechazado,  contratos.fechaCreacion, contratos.tipoCarga,  contratos.articulo22,  " +
                                            "trabajadores.nacimiento,  trabajadores.sexo, trabajadores.nrohijos, afpsTrabajador.codigoAfp," +
                                            "afpsTrabajador.tipoApv, afpsTrabajador.formaApv,afpsTrabajador.apv, isapresTrabajador.codigoIsapre, isapresTrabajador.numeroUf " +
                                           "FROM personas " +
                                            "inner join contratos on personas.id = contratos.idPersona " +
                                            "inner join trabajadores on personas.id = trabajadores.idPersona " +
                                            "inner join afpsTrabajador on contratos.idAfpTrab = afpsTrabajador.id " +
                                            "inner join isapresTrabajador on contratos.idIsapreTrab = isapresTrabajador.id " +
                                           "where contratos.habilitado = 1 ", BD_Cli);


                List<ProcLiquidacion> trabajadores = new List<ProcLiquidacion>();
                if (f.Tabla.Rows.Count > 0)
                {

                    trabajadores = (from DataRow dr in f.Tabla.Rows
                                    select new ProcLiquidacion()
                                    {
                                        idJornada = int.Parse(dr["idJornada"].ToString()),
                                        rut = dr["rut"].ToString(),
                                        fechaingreso = dr["inicio"].ToString(),
                                        nacimiento = dr["nacimiento"].ToString(),
                                        sexo = dr["sexo"].ToString(),
                                        tipocontrato = int.Parse(dr["idTipoContrato"].ToString()),
                                        nrohijos = int.Parse(dr["nroHijos"].ToString()),
                                        codigoafp = int.Parse(dr["codigoAfp"].ToString()),
                                        tipoapv = int.Parse(dr["tipoApv"].ToString()),
                                        formaapv = dr["formaApv"].ToString(),
                                        apv = decimal.Parse(dr["apv"].ToString()),
                                        codigoisapre = int.Parse(dr["codigoIsapre"].ToString()),
                                        ufs = decimal.Parse(dr["numeroUf"].ToString()),
                                        sueldoBase = int.Parse(dr["sueldobase"].ToString()),
                                        tipoCarga = dr["tipoCarga"].ToString(),
                                        articulo22 = dr["articulo22"].ToString()
                                    }).ToList();

                    foreach (var r in trabajadores)
                    {
                        if (InicializaTrabajador())
                        {
                            CargaTiempo(r);
                            CalculaHaberes(r);
                            CalculaHaberesInformados(r);
                            CalculaDescuentos(r);
                            CalculaDescuentosInformados(r);
                            CalculaResultados(r);
                            resultado.mensaje = "proceso termino en forma correcta";
                            resultado.result = 1;
                        }
                    }
                }
                else
                {
                    resultado.mensaje = "No se encuentran trabajadores";
                    resultado.result = 0;
                    return resultado;

                }
                return resultado;
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error en proceso de liquidacion";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al procesar las remuneraciones", correos.destinatarioErrores);
                resultado.mensaje = "proceso termina con error";
                resultado.result = 0;
                return resultado;
            }
        }
        public void CargaTiempo(ProcLiquidacion proc)
        {
            int fallas = 0, licencias = 0, permisos = 0, trabajos = 0, inasis = 0, diaspagar = 0;
            int ultimodia = 0;
            tie.fallas = 0;
            tie.licencias = 0;
            tie.pagar = 30;
            f.EjecutarConsultaSQLCli("SELECT asistenciasinformadas.id,asistenciasinformadas.rutTrabajador,asistenciasinformadas.fechaAsistencia,asistenciasinformadas.codigoInasis " +
                                        ",asistenciasinformadas.dias,asistenciasinformadas.horasExtras1, asistenciasinformadas.horasExtras2, asistenciasinformadas.horasExtras3 " +
                                        ",asistenciasinformadas.diasColacion,asistenciasinformadas.horasColacion, asistenciasinformadas.diasMovilizacion " +
                                        "FROM asistenciasInformadas " +
                                        "WHERE asistenciasinformadas.habilitado = 1 " +
                                        " and asistenciasinformadas.fechaAsistencia >= '" + fechainiciostr + "' and asistenciasinformadas.fechaAsistencia <= '" + fechaterminostr + "' ", BD_Cli);


            List<DetAsistenciasInformadas> opcionesList = new List<DetAsistenciasInformadas>();
            if (f.Tabla.Rows.Count > 0)
            {

                opcionesList = (from DataRow dr in f.Tabla.Rows
                                select new DetAsistenciasInformadas()
                                {
                                    id = int.Parse(dr["id"].ToString()),
                                    rutTrabajador = dr["rutTrabajador"].ToString(),
                                    fechaAsistencia = dr["fechaasistencia"].ToString(),
                                    codigoInasis = dr["codigoInasis"].ToString(),
                                    dias = int.Parse(dr["dias"].ToString()),
                                    horasExtras1 = dr["horasExtras1"].ToString(),
                                    horasExtras2 = dr["horasExtras2"].ToString(),
                                    horasExtras3 = dr["horasExtras3"].ToString(),
                                    diasColacion = int.Parse(dr["diasColacion"].ToString()),
                                    horasColacion = dr["horasColacion"].ToString(),
                                    diasMovilizacion = int.Parse(dr["diasMovilizacion"].ToString()),
                                }).ToList();
                foreach (var r in opcionesList)
                {

                    if (r.codigoInasis.Trim() == "AI") fallas += r.dias;
                    if (r.codigoInasis.Trim() == "LM") licencias += r.dias;
                    if (r.codigoInasis.Trim() == "PR") permisos += r.dias;
                    tie.horas1 += Convert.ToDecimal(r.horasExtras1);
                    tie.horas2 += Convert.ToDecimal(r.horasExtras2);
                    tie.horas3 += Convert.ToDecimal(r.horasExtras3);
                    tie.horascolacion += Convert.ToDecimal(r.horasColacion);
                    tie.diascolacion += Convert.ToInt32(r.diasColacion);
                    tie.diasmovilizacion += Convert.ToInt32(r.diasMovilizacion);
                    if (r.fechaAsistencia == fechatermino.ToString() && r.codigoInasis.Trim() == "AI") ultimodia = 1;
                }
                if (licencias > 30) licencias = 30;
                if (fallas >= 30) fallas = 30;
                inasis = fallas + licencias;
                trabajos = 30 - inasis;
                if (trabajos >= 30) trabajos = 30;
                if (fechainicio.Month == 2)
                {
                    if (trabajos > 27) trabajos = 30;
                    if (licencias > 27) licencias = 30;
                    if (fallas > 27) fallas = 30;
                }
                diaspagar = trabajos;
                if ((trabajos + inasis) > 30)
                {
                    if (ultimodia == 1) diaspagar += -1;
                }
                tie.fallas = fallas;
                tie.licencias = licencias;
                tie.pagar = diaspagar;
            }
        }
        public void CalculaHaberes(ProcLiquidacion proc)
        {
            int ind;
            for (ind = 0; ind < 100; ind++)
            {

                if (haberes[ind].calculado == "S")
                {
                    //Calculos de sueldo Base
                    if (haberes[ind].codigohaber == 1)
                    {
                        Decimal resultado = Sueldo(proc.sueldoBase);
                        Graba_Resultados(proc.rut, para.pago, 1, haberes[1].descripcion, tie.pagar, proc.sueldoBase, resultado, fechaproceso);
                    }

                    //Calculos de Asiganción casa
                    if (haberes[ind].codigohaber == 2)
                    {

                    }
                    // Calculo familiar normal
                    if (haberes[ind].codigohaber == 7)
                    {
                        Decimal valfam = 0;
                        if (proc.tipoCarga == "A") valfam = valorcarga[1];
                        if (proc.tipoCarga == "B") valfam = valorcarga[2];
                        if (proc.tipoCarga == "C") valfam = valorcarga[3];
                        Decimal pagar = Familiar(valfam, proc.nrohijos);
                        Graba_Resultados(proc.rut, para.pago, 7, haberes[7].descripcion, proc.nrohijos, valfam, Convert.ToInt32(pagar), fechaproceso);
                        tot.pagarisapre = tot.pagarisapre - pagar;
                    }
                    // Calculo familiar duplo
                    //if (haberes[ind].codigohaber == 8)
                    //{
                    //    Decimal valfam = 0;
                    //    if (proc.tipoCarga == "A") valfam = valorcarga[1];
                    //    if (proc.tipoCarga == "B") valfam = valorcarga[2];
                    //    if (proc.tipoCarga == "C") valfam = valorcarga[3];
                    //    Decimal pagar = Familiar(valfam, proc.nrohijos);
                    //    Graba_Resultados(proc.rut, para.pago, 8, haberes[8].descripcion, proc.nrohijos, valfam, Convert.ToInt32(pagar), fechaproceso);
                    //    tot.pagarisapre = tot.pagarisapre - pagar;
                    //}

                    //Calculo de asignacion colacion
                    if (haberes[ind].codigohaber == 9)
                    {
                        Decimal resuldec = Colacion(para.colacionDia, tie.diascolacion);
                        Graba_Resultados(proc.rut, para.pago, 9, haberes[9].descripcion, tie.diascolacion, para.colacionDia, Convert.ToInt32(resuldec), fechaproceso);
                    }

                    //Calculo de asignacion Movilizacion
                    if (haberes[ind].codigohaber == 10)
                    {
                        Decimal resuldec = Movilizacion(para.movilizacionDia, tie.diasmovilizacion);
                        Graba_Resultados(proc.rut, para.pago, 10, haberes[10].descripcion, tie.diasmovilizacion, para.movilizacionDia, Convert.ToInt32(resuldec), fechaproceso);
                    }

                    //Calculo de asignacion familiar
                }
            }
        }
        public void CalculaHaberesInformados(ProcLiquidacion proc)
        {
            int grainf = 0;
            f.EjecutarConsultaSQLCli("SELECT haberesinformados.haber,haberesinformados.tipoCalculo, haberesinformados.monto, haberesinformados.dias " +
                                        "FROM haberesinformados " +
                                        "WHERE haberesinformados.habilitado = 1 and haberesinformados.rutTrabajador = '" + proc.rut + "' " +
                                        " and haberesinformados.fechaDesde <= '" + fechaterminostr + "' and haberesinformados.fechaHasta >= '" + fechainiciostr +
                                        "' and haberesinformados.pago = '" + para.pago + "' ", BD_Cli);


            List<DetCuadraturaHaberes> habinformados = new List<DetCuadraturaHaberes>();
            if (f.Tabla.Rows.Count > 0)
            {

                habinformados = (from DataRow dr in f.Tabla.Rows
                                 select new DetCuadraturaHaberes()
                                 {
                                     haber = int.Parse(dr["haber"].ToString()),
                                     tipoCalculo = dr["tipoCalculo"].ToString(),
                                     monto = decimal.Parse(dr["monto"].ToString()),
                                     dias = int.Parse(dr["dias"].ToString()),
                                 }).ToList();

                foreach (var h in habinformados)
                {

                    Decimal dias = Convert.ToDecimal(h.dias);
                    Decimal valhab = Convert.ToDecimal(h.monto);
                    Decimal habinf = HaberInformado(h.haber, dias, h.tipoCalculo, proc.sueldoBase, valhab);
                    if (h.haber == 7) grainf = 1;
                    Graba_Resultados(proc.rut, para.pago, h.haber, haberes[h.haber].descripcion, dias, valhab, Convert.ToInt32(habinf), fechaproceso);
                }
            }

            //Calculo de sobretiempo con recargo de 50 %
            Decimal sobre1 = Sobretiempo(tot.basesobretiempo, tie.horas1, 50, tie.horasmes);
            Graba_Resultados(proc.rut, para.pago, 3, haberes[3].descripcion, tie.horas1, 50, Convert.ToInt32(sobre1), fechaproceso);


            //Calculo de sobretiempo festivos
            Decimal sobre2 = Sobretiempo(tot.basesobretiempo, tie.horas2, 100, tie.horasmes);
            Graba_Resultados(proc.rut, para.pago, 4, haberes[4].descripcion, tie.horas2, 100, Convert.ToInt32(sobre2), fechaproceso);


            //Calculo de sobretiempo festivo especial
            Decimal sobre3 = Sobretiempo(tot.basesobretiempo, tie.horas3, 300, tie.horasmes);
            Graba_Resultados(proc.rut, para.pago, 5, haberes[5].descripcion, tie.horas3, 300, Convert.ToInt32(sobre3), fechaproceso);

            //Calculo de Gratificacion ordinaria
            if (grainf == 0)
            {
                Decimal resuldec = Gratificacion(tot.basevariable, para.porGratificacion, para.mtoTopeGratificacion);
                Graba_Resultados(proc.rut, para.pago, 6, haberes[6].descripcion, para.porGratificacion, para.mtoTopeGratificacion, Convert.ToInt32(resuldec), fechaproceso);
            }
        }
        public void CalculaDescuentos(ProcLiquidacion proc)
        {
            tra.imponible = tot.imponible;
            tra.imponiblesc = tot.imponible;
            tra.imponiblelic = tot.imponible;
            tra.imponibledia = tot.imponible;
            if (tie.licencias > 0)
            {
                Decimal ultimoimponible = UltimoImponible(proc);
                tra.imponiblesc = tra.imponiblesc + ultimoimponible * tie.licencias / 30;
                tra.imponiblelic = tra.imponiblelic + ultimoimponible * tie.licencias / 30;
                tra.imponibledia = ultimoimponible * (30 - tie.licencias) / 30;
            }
            if (tra.imponible > para.mtoTopeAfp) tra.imponible = para.mtoTopeAfp;
            if (tra.imponiblelic > para.mtoTopeAfp) tra.imponiblelic = para.mtoTopeAfp;
            if (tra.imponiblesc > para.mtoTopeSc) tra.imponiblesc = para.mtoTopeSc;

            Procesa_Instituciones(proc);

            //CALCULO MUTUAL
            Mutual(proc, tra.imponible);

            //CALCULO DE IMPUESTO
            tot.tributable = tot.tributable - tot.leyessocimp;
            Impuesto(proc, tot.tributable);
        }
        //INSTITUCIONES DL TRABAJADO
        void Procesa_Instituciones(ProcLiquidacion proc)
        {
            DateTime fecha = fechatermino.AddYears(-11);
            DateTime fecing = Convert.ToDateTime(proc.fechaingreso);
            Decimal val_pen = 0;
            Decimal val_seg = 0;
            Decimal val_apv = 0;
            Decimal val_isapre = 0;

            afps regafp = listaafp.Where(x => x.codigo == proc.codigoafp).FirstOrDefault();
            val_pen = Afp_Pen(tra.imponible, regafp.pension);
            Graba_Resultados(proc.rut, para.pago, 910, "LEY.SOC.PENSIÓN", regafp.codigo, regafp.pension, Convert.ToInt32(val_pen), fechaproceso);

            val_seg = Afp_Seg(tra.imponible, regafp.seguro);
            Graba_Resultados(proc.rut, para.pago, 2401, "LEY.SOC.SEGURO", regafp.codigo, regafp.seguro, Convert.ToInt32(val_seg), fechaproceso);

            Decimal val_emp = Seg_Aporte(tra.imponiblesc, para.aporteEmpleador);
            Graba_Resultados(proc.rut, para.pago, 2404, "SEGURO APORTE EMPLEADOR", regafp.codigo, para.aporteEmpleador, Convert.ToInt32(val_emp), fechaproceso);

            Decimal val_ley = Seg_Aporte(tra.imponibledia, para.leySanna);
            Graba_Resultados(proc.rut, para.pago, 2405, "APORTE EMPLEADOR LEY SANNA", regafp.codigo, para.leySanna, Convert.ToInt32(val_ley), fechaproceso);

            Decimal val_cap = Seg_Aporte(tra.imponibledia, para.AporteCapital);
            Graba_Resultados(proc.rut, para.pago, 2411, "APORTE EMPLEADOR A CAPITAL INDIVIDUAL", regafp.codigo, para.AporteCapital, Convert.ToInt32(val_cap), fechaproceso);

            Decimal val_exp = Seg_Aporte(tra.imponibledia, para.EspectativaVida);
            Graba_Resultados(proc.rut, para.pago, 2412, "SEGURO SOCIAL ESPECTATIVA DE VIDA", regafp.codigo, para.EspectativaVida, Convert.ToInt32(val_exp), fechaproceso);

            Decimal val_ren = Seg_Aporte(tra.imponibledia, para.Rentaprotegida);
            Graba_Resultados(proc.rut, para.pago, 2413, "RENTA PROTEGIDA", regafp.codigo, para.Rentaprotegida, Convert.ToInt32(val_ren), fechaproceso);

            if (proc.tipocontrato == 1)
            {

                if (fecing > fecha)
                {
                    tra.seguroCesEmp = CesantiaE(tra.imponiblesc, para.afc1);
                    Graba_Resultados(proc.rut, para.pago, 2400, "LEY.SOC.CESANTIA EMPRESA", regafp.codigo, para.afc1, Convert.ToInt32(tra.seguroCesEmp), fechaproceso);

                    tra.seguroCesTra = CesantiaT(tra.imponiblesc, para.afc2);
                    Graba_Resultados(proc.rut, para.pago, 916, "LEY.SOC.CESANTIA ", regafp.codigo, para.afc2, Convert.ToInt32(tra.seguroCesTra), fechaproceso);
                }
                else
                {
                    tra.seguroCesEmp = CesantiaE(tra.imponiblesc, para.afc4);
                    Graba_Resultados(proc.rut, para.pago, 2400, "LEY.SOC.CESANTIA EMPRESA", regafp.codigo, para.afc4, Convert.ToInt32(tra.seguroCesEmp), fechaproceso);
                }
            }
            else
            {
                tra.seguroCesEmp = CesantiaE(tra.imponiblesc, para.afc3);
                Graba_Resultados(proc.rut, para.pago, 2400, "LEY.SOC.CESANTIA EMPRESA", regafp.codigo, para.afc3, Convert.ToInt32(tra.seguroCesEmp), fechaproceso);
            }
            if (proc.apv != 0)
            {
                val_apv = Convert.ToInt32(proc.apv);
                if (proc.formaapv == "M")
                {
                    Graba_Resultados(proc.rut, para.pago, 120, "AHORRO VOLUNTARIO MONTO", regafp.codigo, proc.apv, Convert.ToInt32(proc.apv), fechaproceso);

                }
                if (proc.formaapv == "U")
                {
                    Graba_Resultados(proc.rut, para.pago, 120, "AHORRO VOLUNTARIO UF", regafp.codigo, proc.apv, Convert.ToInt32(proc.apv * para.valorUf), fechaproceso);
                }
            }


            val_isapre = Isapre(tra.imponible, para.cotIsapre, proc.ufs);
            if (proc.ufs > 0) Graba_Resultados(proc.rut, para.pago, 921, "LEY.SOC. SALUD", proc.codigoisapre, proc.ufs, Convert.ToInt32(val_isapre), fechaproceso);
            if (proc.ufs <= 0) Graba_Resultados(proc.rut, para.pago, 921, "LEY.SOC. SALUD", proc.codigoisapre, para.cotIsapre, Convert.ToInt32(val_isapre), fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2421, "LEY.SOC. 7 %", proc.codigoisapre, para.cotIsapre, Convert.ToInt32(tra.isapresiete), fechaproceso);


            // OTRAS INSTITUCIONES DE AHORRO VOLUNTARIO 
            //if (proc.tipoapv >10)
            //{

            //    if (i.tip_aporte == "M         ")
            //    {
            //        val_apv = (decimal)i.val_aporte;
            //        resultado2 = Graba_Resultados(rut_trab, pago, 940, "APV TRIBUTABLE MONTO", codins, val_apv, val_apv, fecproc, rutEmpresa);
            //    }
            //    else
            //    {
            //        if (i.tip_aporte == "U         ")
            //        {
            //            val_apv = (decimal)i.val_aporte * val_uf;
            //            resultado2 = Graba_Resultados(rut_trab, pago, 940, "APV TRIBUTABLE UF", codins, val_apv, val_apv, fecproc, rutEmpresa);
            //        }
            //        else
            //        {
            //            val_apv = (decimal)i.val_aporte * sueldo_base / 100;
            //            resultado2 = Graba_Resultados(rut_trab, pago, 940, "APV TRIBUTABLE % SUELDO", codins, val_apv, val_apv, fecproc, rutEmpresa);

            //        }
            //    }
            //}

            //if (i.tip_instit == "APV       ")
            //{

            //    if (i.tip_aporte == "M         ")
            //    {
            //        val_apvn = (decimal)i.val_aporte;
            //        resultado2 = Graba_Resultados(rut_trab, pago, 941, "APORTE VOLUNTARIO", codins, val_apvn, val_apvn, fecproc, rutEmpresa);

            //    }
            //    else
            //    {
            //        if (i.tip_aporte == "U         ")
            //        {
            //            val_apvn = (decimal)i.val_aporte * val_uf;
            //            resultado2 = Graba_Resultados(rut_trab, pago, 940, "APV TRIBUTABLE UF", codins, val_apvn, val_apvn, fecproc, rutEmpresa);
            //        }
            //        else
            //        {
            //            val_apvn = (decimal)i.val_aporte * sueldo_base / 100;
            //            resultado2 = Graba_Resultados(rut_trab, pago, 940, "APV TRIBUTABLE % SUELDO", codins, val_apvn, val_apvn, fecproc, rutEmpresa);

            //        }
            //    }

            //}

            tot.leyessoc = val_pen + val_isapre + val_apv + tra.seguroCesTra;
            tot.leyessocimp = tot.leyessoc;
            tot.pagarisapre = val_isapre;
            if (val_isapre > para.mtoTopeIsapre)
            {
                tot.leyessocimp = val_pen + para.mtoTopeIsapre + val_apv + tra.seguroCesTra;
            }
        }

        public void CalculaDescuentosInformados(ProcLiquidacion proc)
        {
            f.EjecutarConsultaSQLCli("SELECT descuentosinformados.descuento,descuentosinformados.tipoCalculo, descuentosinformados.monto " +
                                        "FROM descuentosinformados " +
                                        "WHERE descuentosinformados.habilitado = 1 and descuentosinformados.rutTrabajador ='" + proc.rut + "' " +
                                        " and descuentosinformados.fechaDesde <= '" + fechaterminostr + "' and descuentosinformados.fechaHasta >= '" + fechainiciostr +
                                        "' and descuentosinformados.pago = '" + para.pago + "' ", BD_Cli);


            List<DetCuadraturaDescuentos> dctinformados = new List<DetCuadraturaDescuentos>();
            if (f.Tabla.Rows.Count > 0)
            {

                dctinformados = (from DataRow dr in f.Tabla.Rows
                                 select new DetCuadraturaDescuentos()
                                 {
                                     descuento = int.Parse(dr["descuento"].ToString()),
                                     tipocalculo = dr["tipoCalculo"].ToString(),
                                     monto = decimal.Parse(dr["monto"].ToString()),
                                 }).ToList();

                foreach (var d in dctinformados)
                {
                    Decimal dctinf = Dscto_inf(d.descuento, d.tipocalculo, proc.sueldoBase, (Decimal)d.monto);
                    Graba_Resultados(proc.rut, para.pago, d.descuento, desctos[d.descuento].descripcion, 0, dctinf, dctinf, fechaproceso);
                }
            }
        }
        public void CalculaResultados(ProcLiquidacion proc)
        {
            tot.saldo = tot.haberes - tot.garantizado - tot.descuentos;
            if (tot.saldo < 0)
            {
                tot.sobregiro = tot.saldo;
                tot.saldo = tot.garantizado;
            }
            else
            {
                tot.saldo = tot.saldo + tot.garantizado;
            }
            // Graba Resultados
            Graba_Resultados(proc.rut, para.pago, 2010, "TOTAL IMPONIBLE", 1, 0, tot.imponible, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2011, "TOTAL IMPONIBLE CON TOPE", 1, 0, tra.imponible, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2012, "TOTAL IMPONIBLE SC", tie.pagar, 0, tra.imponiblesc, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2013, "TOTAL IMPONIBLE LICENCIA", tie.pagar, 0, tra.imponiblelic, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2020, "TOTAL NO IMPONIBLE", 1, 0, tot.noimponible, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2030, "TOTAL TRIBUTABLE", 1, 0, tot.tributable, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2040, "TOTAL GARANTIZADO", 1, 0, tot.garantizado, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2050, "TOTAL RETENIBLE", 1, 0, tot.retenible, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2060, "TOTAL DEDUCIBLE", 1, 0, tot.descuentos, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2090, "BASE LICENCIAS", 1, 0, tot.baselicencias, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2091, "BASE HORAS EXTRAS", 1, 0, tot.basesobretiempo, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2092, "BASE INDEMNIZACIÓN", 1, 0, tot.baseindemnizacion, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2093, "BASE RENTAS VARIABLES", 1, 0, tot.basevariable, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2100, "TOTAL HABERES", 1, 0, tot.haberes, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2200, "TOTAL DESCUENTOS", 1, 0, tot.descuentos, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2210, "TOTAL LEYES SOCIALES", 1, 0, tot.leyessoc, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2211, "TOTAL LEYES TRIBUTABLES", 1, 0, tot.leyessocimp, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2212, "PAGAR ISAPRE", 1, 0, tot.pagarisapre, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2250, "TOTAL OTROS DESCUENTOS", 1, 0, tot.otrosdct, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2300, "SALDO", 1, 0, tot.saldo, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2301, "SOBREGIRO", 1, 0, tot.sobregiro, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 2302, "HORAS DE TRABAJO", 1, 0, tie.horasmes, fechaproceso);
            // Graba Parametros
            Graba_Resultados(proc.rut, para.pago, 3001, "VALOR UF ", 1, 0, para.valorUf, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 3002, "VALOR UTM ", 1, 0, para.valorUtm, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 3003, "VALOR UF de PAGO", 1, 0, para.valorUf, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 3101, "TOPE AFP", 1, 0, para.topeAfp, fechaproceso);
            Graba_Resultados(proc.rut, para.pago, 3102, "TOPE SEGURO CESANTIA", 1, 0, para.topeSc, fechaproceso);
        }
        public decimal UltimoImponible(ProcLiquidacion proc)
        {
            Decimal valor = 0;
            f.EjecutarConsultaSQLCli("SELECT resultado.monto, resultado.fechaPago FROM resultados " +
                                        "WHERE resultado.habilitado = 1 " +
                                        " and resultado.rutTrabajador = '" + proc.rut + "' and resultado.concepto= " + 2012 +
                                        " and resultado.cantidad = 30 ", BD_Cli);
            List<ultimo> ultimoimponible = new List<ultimo>();
            if (f.Tabla.Rows.Count > 0)
            {

                ultimoimponible = (from DataRow dr in f.Tabla.Rows
                                   select new ultimo()
                                   {
                                       fecha = dr["fechapago"].ToString(),
                                       monto = decimal.Parse(dr["monto"].ToString()),
                                   }).ToList();

            }
            var lista = ultimoimponible.OrderByDescending(x => x.fecha).ToList();
            var ultimo = lista.FirstOrDefault();
            valor = ultimo.monto;
            return valor;
        }

        public bool CargaHaberes(int empresa)
        {
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            int ind;

            f.EjecutarConsultaSQLCli("SELECT haberes.id,haberes.haber,haberes.descripcion,haberes.imponible,haberes.tributable,haberes.numeroMeses, " +
                                        " haberes.garantizado, haberes.retenible, haberes.calculado, haberes.tiempo, haberes.deducible, " +
                                        " haberes.baseLicencia, haberes.baseSobretiempo, " +
                                        " haberes.baseIndemnizacion, haberes.baseVariable, haberes.codigoDT, haberes.codigoPrevired" +
                                        " FROM haberes " +
                                        " WHERE haberes.habilitado = 1 ", BD_Cli);


            List<HaberBaseVM> lista = new List<HaberBaseVM>();
            if (f.Tabla.Rows.Count > 0)
            {

                lista = (from DataRow dr in f.Tabla.Rows
                         select new HaberBaseVM()
                         {
                             haber = int.Parse(dr["haber"].ToString()),
                             descripcion = dr["Descripcion"].ToString(),
                             imponible = dr["imponible"].ToString(),
                             tributable = dr["tributable"].ToString(),
                             numeromeses = int.Parse(dr["numeroMeses"].ToString()),
                             garantizado = dr["garantizado"].ToString(),
                             retenible = dr["retenible"].ToString(),
                             calculado = dr["calculado"].ToString(),
                             tiempo = dr["tiempo"].ToString(),
                             deducible = dr["deducible"].ToString(),
                             baselicencia = dr["baseLicencia"].ToString(),
                             basesobretiempo = dr["baseSobretiempo"].ToString(),
                             baseindemnizacion = dr["baseIndemnizacion"].ToString(),
                             basevariable = dr["baseVariable"].ToString(),
                             codigoDT = dr["CodigoDT"].ToString(),
                             codigoprevired = dr["codigoPrevired"].ToString(),
                         }).ToList();

                for (ind = 0; ind < 100; ind++)
                {
                    haberes[ind].codigohaber = 0;
                    haberes[ind].imponible = "N";
                    haberes[ind].tributable = "N";
                    haberes[ind].numeromeses = 0;
                    haberes[ind].garantizado = "N";
                    haberes[ind].retenible = "N";
                    haberes[ind].calculado = "N";
                    haberes[ind].tiempo = "N";
                    haberes[ind].deducible = "N";
                    haberes[ind].baselicencias = "N";
                    haberes[ind].baseobretiempo = "N";
                    haberes[ind].baseindemnizacion = "N";
                    haberes[ind].basevariable = "N";

                }
                foreach (var hab in lista)
                {
                    ind = hab.haber;
                    if (ind > 0 && ind < 100)
                    {
                        haberes[ind].codigohaber = ind;
                        haberes[ind].descripcion = hab.descripcion;
                        haberes[ind].imponible = hab.imponible;
                        haberes[ind].tributable = hab.tributable;
                        haberes[ind].numeromeses = hab.numeromeses;
                        haberes[ind].garantizado = hab.garantizado;
                        haberes[ind].retenible = hab.retenible;
                        haberes[ind].calculado = hab.calculado;
                        haberes[ind].tiempo = hab.tiempo;
                        haberes[ind].deducible = hab.deducible;
                        haberes[ind].baselicencias = hab.baselicencia;
                        haberes[ind].baseobretiempo = hab.basesobretiempo;
                        haberes[ind].baseindemnizacion = hab.baseindemnizacion;
                        haberes[ind].basevariable = hab.basevariable;
                    }
                }
                return true;
            }
            return false;
        }
        public bool CargaDescuentos(int empresa)
        {
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            int ind;

            f.EjecutarConsultaSQLCli("SELECT descuentos.id,descuentos.descuento,descuentos.descripcion,descuentos.prioridad,descuentos.minimo,descuentos.maximo, " +
                                        " descuentos.codigoDT, descuentos.codigoPrevired" +
                                        " FROM descuentos " +
                                        " WHERE descuentos.habilitado = 1 ", BD_Cli);


            List<DescuentoBaseVM> lista = new List<DescuentoBaseVM>();
            if (f.Tabla.Rows.Count > 0)
            {

                lista = (from DataRow dr in f.Tabla.Rows
                         select new DescuentoBaseVM()
                         {
                             id = int.Parse(dr["id"].ToString()),
                             descuento = int.Parse(dr["descuento"].ToString()),
                             descripcion = dr["Descripcion"].ToString(),
                             prioridad = int.Parse(dr["prioridad"].ToString()),
                             minimo = int.Parse(dr["minimo"].ToString()),
                             maximo = int.Parse(dr["maximo"].ToString()),
                             codigoDT = dr["CodigoDT"].ToString(),
                             codigoprevired = dr["codigoPrevired"].ToString(),
                         }).ToList();
                List<DetDescuentos> salida = new List<DetDescuentos>();
                foreach (var d in lista)
                {
                    DetDescuentos registro = new DetDescuentos();
                    registro.id = d.id;
                    registro.descuento = d.descuento;
                    registro.descripcion = d.descripcion;
                    registro.minimo = d.minimo.ToString("###,###,###");
                    registro.maximo = d.maximo.ToString("###,###,###");
                    registro.codigoDT = d.codigoDT;
                    registro.prioridad = d.prioridad.ToString();
                    salida.Add(registro);
                }
                for (ind = 0; ind < 1000; ind++)
                {
                    desctos[ind].codigodescuento = 0;
                    desctos[ind].descripcion = " ";
                    desctos[ind].prioridad = 0;
                    desctos[ind].mindescuento = 0;
                    desctos[ind].maxdescuento = 0;
                }
                foreach (var des in lista)
                {
                    ind = (int)des.descuento;
                    if (ind > 0 && ind < 1000)
                    {
                        desctos[ind].codigodescuento = ind;
                        desctos[ind].descripcion = des.descripcion;
                        desctos[ind].prioridad = (int)des.prioridad;
                        desctos[ind].mindescuento = (int)des.minimo;
                        desctos[ind].maxdescuento = (int)des.maximo;
                    }
                }
                return true;
            }
            return false;
        }
        public bool CargaParametros(int empresa)
        {
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            f.EjecutarConsultaSQLCli("SELECT parametrosGenerales.tabla,parametrosGenerales.codigo,parametrosGenerales.valor, " +
                                        " parametrosGenerales.fecha " +
                                        " FROM parametrosGenerales " +
                                        " WHERE parametrosGenerales.habilitado = 1 ", "remuneracion");


            List<parametros> lista = new List<parametros>();
            if (f.Tabla.Rows.Count > 0)
            {

                lista = (from DataRow dr in f.Tabla.Rows
                         select new parametros()
                         {
                             tabla = dr["tabla"].ToString(),
                             codigo = dr["codigo"].ToString(),
                             valor = dr["valor"].ToString(),
                             fecha = dr["fecha"].ToString(),
                         }).ToList();
                para.sueldominimo1 = Convert.ToDecimal(BuscaValor(lista, "MINIMOS", "1"));
                para.sueldominimo2 = Convert.ToDecimal(BuscaValor(lista, "MINIMOS", "2"));
                para.sueldominimo3 = Convert.ToDecimal(BuscaValor(lista, "MINIMOS", "3"));
                para.afc1 = Convert.ToDecimal(BuscaValor(lista, "AFC", "1"));
                para.afc2 = Convert.ToDecimal(BuscaValor(lista, "AFC", "2"));
                para.afc3 = Convert.ToDecimal(BuscaValor(lista, "AFC", "3"));
                para.afc4 = Convert.ToDecimal(BuscaValor(lista, "AFC", "4"));
                para.cotIsapre = Convert.ToDecimal(BuscaValor(lista, "ISAPRE", "1"));
                para.topeAfp = Convert.ToDecimal(BuscaValor(lista, "TOPES", "1"));
                para.topeSc = Convert.ToDecimal(BuscaValor(lista, "TOPES", "3"));
                para.topeIsapre = Convert.ToDecimal(BuscaValor(lista, "TOPES", "4"));
                para.trabajoPesado1 = Convert.ToInt32(BuscaValor(lista, "PESADO", "1"));
                para.trabajoPesado2 = Convert.ToInt32(BuscaValor(lista, "PESADO", "2"));
                para.pago = "L";
                valorcarga[0] = 0;
                valorcarga[1] = Convert.ToInt32(BuscaValor(lista, "FAMILIAR", "1"));
                valorcarga[2] = Convert.ToInt32(BuscaValor(lista, "FAMILIAR", "2"));
                valorcarga[3] = Convert.ToInt32(BuscaValor(lista, "FAMILIAR", "3"));
                valorcarga[4] = Convert.ToInt32(BuscaValor(lista, "FAMILIAR", "4"));
                tasa[0] = 0;
                tasa[1] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "1"));
                tasa[2] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "2"));
                tasa[3] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "3"));
                tasa[4] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "4"));
                tasa[5] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "5"));
                tasa[6] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "6"));
                tasa[7] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "7"));
                tasa[8] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "8"));
                tramo[0] = 0;
                tramo[1] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "11"));
                tramo[2] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "12"));
                tramo[3] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "13"));
                tramo[4] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "14"));
                tramo[5] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "15"));
                tramo[6] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "16"));
                tramo[7] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "17"));
                tramo[8] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "18"));
                rebaja[0] = 0;
                rebaja[1] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "21"));
                rebaja[2] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "22"));
                rebaja[3] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "23"));
                rebaja[4] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "24"));
                rebaja[5] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "25"));
                rebaja[6] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "26"));
                rebaja[7] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "27"));
                rebaja[8] = Convert.ToDecimal(BuscaValor(lista, "IMPTO", "28"));
                para.aporteEmpleador = Convert.ToDecimal(BuscaValor(lista, "AFP", "4000"));
                para.leySanna = Convert.ToDecimal(BuscaValor(lista, "AFP", "4001"));
                para.AporteCapital = Convert.ToDecimal(BuscaValor(lista, "AFP", "4011"));
                para.EspectativaVida = Convert.ToDecimal(BuscaValor(lista, "AFP", "4012"));
                para.Rentaprotegida = Convert.ToDecimal(BuscaValor(lista, "AFP", "4013"));
                para.pensionAfp = Convert.ToDecimal(BuscaValor(lista, "AFP", "1000"));
                Carga_Apis();
                f.EjecutarConsultaSQLCli("SELECT parametros.tabla,parametros.codigo,parametros.valor, " +
                                            " parametros.fecha " +
                                            " FROM parametros " +
                                            " WHERE parametros.habilitado = 1 ", BD_Cli);


                List<parametros> lista1 = new List<parametros>();
                if (f.Tabla.Rows.Count > 0)
                {

                    lista1 = (from DataRow dr in f.Tabla.Rows
                              select new parametros()
                              {
                                  tabla = dr["tabla"].ToString(),
                                  codigo = dr["codigo"].ToString(),
                                  valor = dr["valor"].ToString(),
                                  fecha = dr["fecha"].ToString(),
                              }).ToList();
                    para.colacionDia = Convert.ToDecimal(BuscaValor(lista1, "GENERAL", "1"));
                    para.movilizacionDia = Convert.ToDecimal(BuscaValor(lista1, "GENERAL", "2"));
                    para.porGratificacion = Convert.ToDecimal(BuscaValor(lista1, "GENERAL", "3"));
                    para.topGratificacion = Convert.ToDecimal(BuscaValor(lista1, "GENERAL", "4"));
                    para.mutual = Convert.ToDecimal(BuscaValor(lista1, "GENERAL", "5"));
                    para.mtoTopeGratificacion = RedondeaValor((para.sueldominimo1 * para.topGratificacion / 12), 1);
                    return true;
                }
            }
            return false;
        }
        public decimal RedondeaValor(decimal valor, decimal red)
        {
            if (valor == 0) return 0;
            decimal valorVal = valor / red;
            valorVal = valorVal + 1 / 2;
            double valorent = Convert.ToInt64(valorVal);
            valorVal = (Decimal)valorent * red;
            return valorVal;
        }
        public bool CargaAfps(int empresa)
        {
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion";

            f.EjecutarConsultaSQLCli("SELECT afps.codigo, afps.descripcion,afps.cotizacionPension,afps.cotizacionSeguro " +
                                        " FROM afps ", BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                listaafp = (from DataRow dr in f.Tabla.Rows
                            select new afps()
                            {
                                codigo = int.Parse(dr["codigo"].ToString()),
                                descripcion = dr["descripcion"].ToString(),
                                pension = decimal.Parse(dr["cotizacionPension"].ToString()),
                                seguro = decimal.Parse(dr["cotizacionSeguro"].ToString())
                            }).ToList();
                return true;
            }
            return false;
        }
        public string BuscaValor(List<parametros> lista, string tabla, string codigo)
        {
            string valor = "0";
            valor = lista.Where(x => x.tabla.Trim() == tabla.Trim() && x.codigo.Trim() == codigo.Trim()).FirstOrDefault().valor;
            if (valor == null) valor = "0";
            return valor;
        }
        public bool InicializaTrabajador()
        {
            tot.imponible = 0;
            tot.imponiblegratificacion = 0;
            tot.noimponible = 0;
            tot.tributable = 0;
            tot.garantizado = 0;
            tot.retenible = 0;
            tot.deducible = 0;
            tot.baselicencias = 0;
            tot.basesobretiempo = 0;
            tot.baseindemnizacion = 0;
            tot.basevariable = 0;
            tot.haberes = 0;
            tot.descuentos = 0;
            tot.otrosdct = 0;
            tot.leyessoc = 0;
            tot.saldo = 0;
            tot.sobregiro = 0;
            tie.fallas = 0;
            tie.licencias = 0;
            tie.horas1 = 0;
            tie.horas2 = 0;
            tie.horas3 = 0;
            tie.horascolacion = 0;
            tie.diascolacion = 0;
            tie.diasmovilizacion = 0;
            return true;
        }
        Decimal Sueldo(Decimal sbase)
        {
            Decimal pagar;
            pagar = (sbase / 30) * tie.pagar;
            int pagarI = Convert.ToInt32(pagar);
            return pagarI;
        }
        decimal Sobretiempo(Decimal sbase, decimal horas, decimal reca, Decimal hmes)
        {
            decimal pagar = 0;
            if (horas > 0)
            {
                pagar = (sbase * 28 / (hmes * 30)) * horas * (1 + (reca / 100));
            }
            return pagar;
        }
        Decimal Movilizacion(decimal valdia, int ndias)
        {
            decimal pagar = 0;
            if (ndias > 0)
            {
                pagar = valdia * ndias;
            }
            return pagar;
        }
        Decimal Colacion(decimal valdia, int ndias)
        {
            decimal pagar = 0;
            if (ndias > 0)
            {
                pagar = (int)valdia * ndias;
            }
            return pagar;
        }
        Decimal Familiar(Decimal valcarga, Decimal ncargas)
        {
            Decimal pagar = 0;
            if (ncargas > 0)
            {
                pagar = valcarga * ncargas;
            }
            return pagar;
        }

        Decimal Gratificacion(Decimal sbase, Decimal pgra, Decimal tgra)
        {
            Decimal pagar = sbase * pgra / 100;
            if (pagar > tgra) pagar = tgra;
            return pagar;
        }

        Decimal HaberInformado(int codigo, Decimal dias, string mtopor, Decimal sbase, decimal valhab)
        {
            Decimal pagar;
            pagar = 0;
            if (mtopor == "M")
            {
                pagar = valhab;
            }
            else
            {
                if (mtopor == "%")
                {
                    pagar = sbase * valhab / 100;
                }
                else
                {
                    pagar = para.valorUf * valhab;
                }
            }
            return pagar;
        }
        public Decimal Afp_Pen(Decimal impon1, decimal cot)
        {
            decimal pagar;
            pagar = impon1 * cot / 100;
            int pagarI = Convert.ToInt32(pagar);
            return pagarI;
        }
        public Decimal Afp_Seg(Decimal impon1, decimal cot)
        {
            decimal pagar;
            pagar = impon1 * cot / 100;
            int pagarI = Convert.ToInt32(pagar);
            return pagarI;
        }
        public Decimal Seg_Aporte(Decimal impon1, decimal cot)
        {
            decimal pagar;
            pagar = impon1 * cot / 100;
            int pagarI = Convert.ToInt32(pagar);
            return pagarI;
        }
        //CALCULO MUTUAL
        int Mutual(ProcLiquidacion proc, Decimal impon)
        {
            Decimal pagar = impon * para.mutual / 100;
            int pagarI = Convert.ToInt32(pagar);
            pagar = Convert.ToDecimal(pagarI);
            Graba_Resultados(proc.rut, para.pago, 2402, "MUTUAL DE SEGURIDAD", 1, para.mutual, pagar, fechaproceso);
            return 1;
        }


        Decimal Isapre(Decimal impon1, decimal cot, decimal nuf)
        {
            decimal pagar;
            decimal val7;
            val7 = impon1 * cot / 100;

            pagar = para.valorUf * nuf;
            if (tie.licencias != 0) pagar = (pagar / 30) * (30 - tie.licencias);
            if (val7 >= pagar) pagar = val7;
            tra.isapresiete = val7;
            int pagarI = Convert.ToInt32(pagar);
            return pagarI;
        }

        Decimal CesantiaE(Decimal impon2, decimal cot)
        {
            // Jaime Sacar imponibles meses anteriores

            decimal pagar;
            pagar = impon2 * cot / 100;
            int pagarI = Convert.ToInt32(pagar);
            return pagarI;
        }
        Decimal CesantiaT
            (Decimal impon2, decimal cot)
        {
            decimal pagar;
            pagar = impon2 * cot / 100;
            int pagarI = Convert.ToInt32(pagar);
            return pagarI;
        }


        Decimal Dscto_inf(int codigo, string mtopor, Decimal sbase, decimal valhab)
        {
            decimal pagar = 0;
            if (mtopor == "M")
            {
                pagar = valhab;
            }
            else
            {
                if (mtopor == "P")
                {
                    pagar = sbase * valhab / 100;
                }
                else
                {
                    if (codigo == 204)
                    {
                        pagar = Sobretiempo(sbase, valhab, 0, tie.horasmes);
                    }
                }

            }
            int pagarI = Convert.ToInt32(pagar);
            return pagarI;
        }
        //CALCULO IMPUESTO UNICO
        public void Impuesto(ProcLiquidacion proc, decimal tribut)
        {
            int ind, pun;
            decimal pagar;
            pun = 0;
            pagar = 0;
            for (ind = 0; ind <= 8; ind++)
            {
                if (tribut >= tramo[ind]) pun = ind;
            }
            if (pun > 0 && pun <= 8)
            {
                pun++;
                pagar = tribut * tasa[pun] / 100 - rebaja[pun];
                if (pagar < 0) pagar = 0;
                int pagarI = Convert.ToInt32(pagar);
                pagar = Convert.ToDecimal(pagarI);
                Graba_Resultados(proc.rut, para.pago, 990, "IMPUESTO UNICO", pun, tasa[pun], pagar, fechaproceso);
            }
        }

        void Graba_Resultados(string rutt, string pago, int conce, string descripcion, Decimal nro, Decimal informado, Decimal mto, string proce)
        {
            // DISTRIBUCION HABERES
            //salida = "conce=" + conce.ToString() + "  mto=" + mto.ToString() + "   Valor=" + valor.ToString();
            //bit = graba_bita("Distrib", 671, salida);
            int entero = Convert.ToInt32(mto);
            mto = entero;
            if (conce < 100 && conce > 0)
            {
                if (haberes[conce].imponible == "S")
                {
                    tot.imponible = tot.imponible + mto;
                }
                else
                {
                    tot.noimponible = tot.noimponible + mto;
                }

                if (haberes[conce].tributable == "S") { tot.tributable += mto; }
                if (haberes[conce].garantizado == "S") { tot.garantizado += mto; }
                if (haberes[conce].retenible == "S") { tot.retenible += mto; }
                if (haberes[conce].deducible == "S") { tot.deducible += mto; }
                if (haberes[conce].baselicencias == "S") { tot.baselicencias += mto; }
                if (haberes[conce].baseobretiempo == "S")
                {
                    if (conce == 1 || conce == 3)
                        tot.basesobretiempo += mto;
                    else
                        tot.basesobretiempo += informado;
                }
                if (haberes[conce].baseindemnizacion == "S") { tot.baseindemnizacion += mto; }
                if (haberes[conce].basevariable == "S") { tot.basevariable += mto; }
                tot.haberes += mto;


            }
            if (conce > 99 && conce < 1000)
            {
                tot.descuentos += mto;
            }

            //INSERTAR A resultado
            if (descripcion != null)
            {
                string nrostr = nro.ToString();
                nrostr = nrostr.Replace(",", ".");
                string informadostr = informado.ToString();
                informadostr = informadostr.Replace(",", ".");
                string mtostr = mto.ToString();
                mtostr = mtostr.Replace(",", ".");
                string fecha = fechatermino.ToString("yyyy'-'MM'-'dd");
                if (descripcion.Length > 30) descripcion = descripcion.Substring(0, 29);
                string query2 = "insert into resultados (rutTrabajador,pago,concepto,descripcion,cantidad,informado,monto,fechapago,habilitado) " +
                "values " +
                "('" + rutt + "','" + pago + "'," + conce + ",'" + descripcion + "','" + nrostr +
                "', '" + informadostr + "','" + mtostr + "','" + fechaterminostr + "' ,1) ";
                f.EjecutarQuerySQLCli(query2, BD_Cli);
            }
        }
        public void Carga_Apis()
        {
            para.valorUf = TraeUF(fechatermino);
            para.valorUtm = TraeUTM(fechatermino);
            para.mtoTopeAfp = RedondeaValor(para.valorUf * para.topeAfp, 1);
            para.mtoTopeSc = RedondeaValor(para.valorUf * para.topeSc, 1);
            para.mtoTopeIsapre = RedondeaValor(para.mtoTopeAfp * para.cotIsapre / 100, 1);
            int valent = Convert.ToInt32(para.mtoTopeGratificacion);
            para.mtoTopeGratificacion = valent;
            int ind;
            for (ind = 0; ind < 9; ind++)
            {
                tramo[ind] = tramo[ind] * para.valorUtm;
                rebaja[ind] = rebaja[ind] * para.valorUtm;
            }


            return;
        }
        public Decimal TraeUF(DateTime fecha)
        {
            Decimal valor = 0;
            var Client = new HttpClient();
            string url = "https://mindicador.cl/api/uf/" + fecha.ToString("dd'-'MM'-'yyyy");
            var response = Client.GetStringAsync(url).Result;
            if (response != null)
            {
                string valorCLP = response.ToString();
                respuestaAPI respuesta = JsonConvert.DeserializeObject<respuestaAPI>(valorCLP);
                valor = respuesta.serie[0].valor;
            }
            return valor;
        }
        public Decimal TraeUTM(DateTime fecha)
        {
            Decimal valor = 0;
            var Client = new HttpClient();
            string url = "https://mindicador.cl/api/utm/" + fecha.ToString("dd'-'MM'-'yyyy");
            var response = Client.GetStringAsync(url).Result;
            if (response != null)
            {
                respuestaAPI respuesta = JsonConvert.DeserializeObject<respuestaAPI>(response.ToString());
                valor = respuesta.serie[0].valor;
            }
            return valor;
        }
        void BorraResultados()
        {

            f.EjecutarConsultaSQLCli("Delete from resultados Where fechaPago >= '" + fechainiciostr + "' and fechaPago <='" + fechaterminostr + "' and pago= 'L'", BD_Cli);
        }
        void GrabaBitacoraProcesos(string usuario, string pago)
        {
            DateTime fecha = Convert.ToDateTime(fechaproceso);
            string fechastr = fecha.ToString("yyyy'-'MM'-'dd");
            string query2 = "insert into procesos (estado, idUsuario,pago,fechaProceso,fechaInicio,fechaTermino) " +
            "values " +
            "( 1, " + Convert.ToInt32(usuario) + ",'" + pago + "', '" + fechastr + "' ,'" + fechainiciostr + "','" + fechaterminostr +"' ) ";
            f.EjecutarQuerySQLCli(query2, BD_Cli);
        }
        public bool MesCerrado()
        {

            f.EjecutarConsultaSQLCli("select * from procesos Where fechaProceso >= '" + fechainiciostr + "' and fechaProceso <='" +
                                      fechaterminostr + "' and pago= 'L' and estado = 9 ", BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
public class respuestaAPI
{
    public string version { get; set; }
    public string autor { get; set; }
    public string codigo { get; set; }
    public string nombre { get; set; }
    public string unidad_medida { get; set; }
    public List<serie> serie { get; set; }
    public respuestaAPI()
    {
        serie = new List<serie>();
    }
}
public class serie
{
    public string fecha { get; set; }
    public Decimal valor { get; set; }
}

public struct ProcLiquidacion
    {
    public string rut;
    public string fechaingreso;
    public string nacimiento;
    public int nrohijos;
    public string sexo;
    public string horassemanales;
    public int sueldoBase;
    public int tipocontrato;
    public decimal porPension;
    public decimal porSalud;
    public decimal porSeguro;
    public string tipoCarga;
    public string articulo22;
    public int idJornada;
    public int codigoafp;
    public int tipoapv;
    public string formaapv;
    public decimal apv;
    public int codigoisapre;
    public decimal ufs;

}
public struct ResuLiquidacion
{
    public string rut;
    public string pago;
    public int concepto;
    public string descripcion;
    public decimal cantidad;
    public decimal informado;
    public decimal monto;
    public string fechapago;
}

public struct haber
{
    public int codigohaber;
    public string descripcion;
    public string imponible;
    public string tributable;
    public int numeromeses;
    public string garantizado;
    public string retenible;
    public string calculado;
    public string tiempo;
    public string deducible;
    public string baselicencias;
    public string baseobretiempo;
    public string baseindemnizacion;
    public string basevariable;
};
public struct descuentos
{
    public int codigodescuento;
    public string descripcion;
    public int prioridad;
    public int mindescuento;
    public int maxdescuento;
}
public struct parametros
{
    public string tabla;
    public string codigo;
    public string valor;
    public string fecha;
}
public struct afps
{
    public int codigo;
    public string descripcion;
    public decimal pension;
    public decimal seguro;
}
public struct parametrosPago
{
    public decimal valorUf;
    public decimal valorUtm;
    public decimal sueldominimo1;
    public decimal sueldominimo2;
    public decimal sueldominimo3;
    public decimal topeAfp;
    public decimal topeIsapre;
    public decimal topeSc;
    public decimal afc1;
    public decimal afc2;
    public decimal afc3;
    public decimal afc4;
    public decimal aporteEmpleador;
    public decimal leySanna;
    public decimal AporteCapital;
    public decimal EspectativaVida;
    public decimal Rentaprotegida;
    public int trabajoPesado1;
    public int trabajoPesado2;
    public decimal pensionAfp;
    public decimal colacionDia;
    public decimal cotIsapre;
    public decimal movilizacionDia;
    public decimal porGratificacion;
    public decimal topGratificacion;
    public decimal mutual;
    public string pago;
    public Decimal mtoTopeAfp;
    public Decimal mtoTopeSc;
    public Decimal mtoTopeIsapre;
    public Decimal mtoTopeGratificacion;


};
public struct totales
{
    public decimal imponible;
    public decimal imponiblegratificacion;
    public decimal noimponible;
    public decimal tributable;
    public decimal garantizado;
    public decimal retenible;
    public decimal deducible;
    public decimal baselicencias;
    public decimal basesobretiempo;
    public decimal baseindemnizacion;
    public decimal basevariable;
    public decimal haberes;
    public decimal descuentos;
    public decimal otrosdct;
    public decimal leyessoc;
    public decimal leyessocimp;
    public decimal pagarisapre;
    public decimal saldo;
    public decimal sobregiro;
}
public struct tiempo
{
    public decimal fallas;
    public decimal licencias;
    public decimal pagar;
    public decimal horas1;
    public decimal horas2;
    public decimal horas3;
    public decimal horascolacion;
    public int diascolacion;
    public int diasmovilizacion;
    public int horasmes;
}
public struct trabajo
{
    public decimal imponible;
    public decimal imponibledia;
    public decimal imponiblelic;
    public decimal imponiblesc;
    public decimal seguroCesEmp;
    public decimal seguroCesTra;
    public Decimal aporteVoltrib;
    public Decimal aporteVolnotrib;
    public Decimal isapresiete;

}
public struct ultimo
{
    public string fecha;
    public decimal monto;
}

