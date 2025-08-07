using DnsClient;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;
using static Azure.Core.HttpHeader;

namespace RRHH.Servicios.ArchivoDireccionTrabajo
{
    /// <summary>
    /// Servicio para generar archivo de previred.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IArchivoDireccionTrabajoService
    {
        /// <summary>
        /// Genera archivo de la direccion del trabajo.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes de proceso</param>
        /// <param name="anio">año de proceso</param>
        /// <param name="path">ruta del archivo</param>
        /// <returns>Archivo de la direccion del trabajo</returns>
        public string ListarArchivoDireccionTrabajoService(int idEmpresa, int mes, int anio, string path);

    }

    public class ArchivoDireccionTrabajoService : IArchivoDireccionTrabajoService
    {
        private readonly IDatabaseManager _databaseManager;
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Generales Generales = new Generales();

        public ArchivoDireccionTrabajoService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public string ListarArchivoDireccionTrabajoService(int empresa, int mes, int anio, string path)
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
                List<Isapres> listaisapres = CargaIsapre();
                List<HaberBaseVM> listahaberes = CargaHaberes(BD_Cli);
                List<DescuentoBaseVM> listadescuentos = CargaDescuentos(BD_Cli);
                string archivo = "/temp/" + empr.rut + "-DT-" + anio.ToString("000#") + mes.ToString("0#") + ".csv";
                string ruta = path + archivo;

                f.EjecutarConsultaSQLCli("SELECT resultados.rutTrabajador,resultados.concepto,resultados.monto,resultados.cantidad,resultados.informado " +
                                            "FROM resultados " +
                                           "WHERE resultados.habilitado = 1 " +
                                            " and resultados.fechaPago <= '" + fecfinstr + "' and resultados.fechaPago >= '" + fecinistr +
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
                    var trabajadores = resultados.GroupBy(x => x.rutTrabajador).Select(g => new { rutTrabajador = g.Key }).ToList();
                    List<Detdireccion> libro = new List<Detdireccion>();
                    foreach (var t in trabajadores)
                    {
                        Detdireccion linea = new Detdireccion();
                        DetalleContrato cont = ConsultaContratoRUT(t.rutTrabajador, BD_Cli);
                        string rut = t.rutTrabajador;
                        linea.cod_1101 = rut.Substring(0, rut.Length - 1)+"-"+ rut.Substring(rut.Length - 1, 1);  // Rut trabajador
                        linea.cod_1102 = cont.inicio;     // Fecha inicio contrato (cód 1102)
                        linea.cod_1103 = null;     // Fecha de término de contrato (cód 1103)
                        linea.cod_1104 = null;     // Causal de término del contrato (cód 1104)
                        linea.cod_1105 = "8";     // Región de prestación de los servicios (cód 1105)
                        linea.cod_1106 = "8101";   // Comuna de prestación de los servicios (cód 1106)
                        linea.cod_1107 = "101";    // Código tipo de jornada (cód 1107)
                        linea.cod_1108 = "0";      // Persona con discapacidad/pensionado por invalidez (cód 1108)
                        linea.cod_1109 = "0";      // Pensionado por vejez (cód 1109)
                        linea.cod_1141 = "0";     // AFP (cód 1141)
                        linea.cod_1142 = "0";      // IPS (ExINP) (cód 1142)
                        linea.cod_1143 = "102";    // FONASA / ISAPRE (cód 1143)
                        linea.cod_1151 = "1";      // AFC (cód 1151)
                        linea.cod_1110 = "0";      // CCAF (cód 1110)
                        linea.cod_1152 = "0";      // Org. Administrador Ley 16.744 (cód 1152)
                        linea.cod_1111 = 0;           // Número cargas familiares legales autorizadas (cód 1111)
                        linea.cod_1112 = 0;           // Número de cargas familiares maternales (cód 1112)
                        linea.cod_1113 = 0;           // Número de cargas familiares invalidez (cód 1113)
                        linea.cod_1114 = "D";      // Tramo asignación familiar (cód 1114)
                        linea.cod_1146 = "0";      // Técnico extranjero exención de cotizaciones previsionales (Ley 18.156)(cód 1146)
                        linea.cod_1170 = "1";      // Tipo de impuesto a la renta (cód 1170)
                        linea.cod_1171 = "";       // Rut organización sindical 1 (cód 1171)
                        linea.cod_1172 = "";       // Rut organización sindical 1 (cód 1172) 
                        linea.cod_1173 = "";        // Rut organización sindical 1 (cód 1173) 
                        linea.cod_1174 = "";       // Rut organización sindical 1 (cód 1174) 
                        linea.cod_1175 = "";       // Rut organización sindical 1 (cód 1175) 
                        linea.cod_1176 = "";       // Rut organización sindical 1 (cód 1176) 
                        linea.cod_1177 = "";       // Rut organización sindical 1 (cód 1177) 
                        linea.cod_1178 = "";       // Rut organización sindical 1 (cód 1178)
                        linea.cod_1179 = "";       // Rut organización sindical 1 (cód 1179) 
                        linea.cod_1180 = "";       // Rut organización sindical 1 (cód 1180) 
                        linea.cod_1115 = 0;           // Número de días trabajados en el mes (cód 1115)
                        linea.cod_1116 = 0;           // Número días de licencia médica en el mes (cód 1116)   
                        linea.cod_1117 = 0;           // Número días de vacaciones en el mes (cód 1117)    
                        linea.cod_1118 = "0";      // Subsidio trabajador Joven (cód 1118)
                        linea.cod_1154 = "";       // Puesto trabajo pesado (cód 1154)
                        linea.cod_1155 = "0";      // Ahorro previsional voluntario individual (cód 1155)
                        linea.cod_1157 = "0";      // Ahorro previsional voluntario colectivo (cód 1157)
                        linea.cod_1131 = "0";      // Indemnización a todo evento (art 164) (cód 1131)
                        linea.cod_1132 = 0;       // Tasa indemnización a todo evento (Art 164) (cód 1132)

        // Parrafo B haberes
                        linea.cod_2101 = 0;         // Sueldo (cód 2101)
                        linea.cod_2102 = 0;         // Sobresueldo (cód 2102)
                        linea.cod_2103 = 0;         // Comisiones (mensual) (cód 2103)
                        linea.cod_2104 = 0;         // Semana corrida mensual (Art 45) (cód 2104)
                        linea.cod_2105 = 0;         // Participación (mensual) (cód 2105)
                        linea.cod_2106 = 0;         // Gratificación (mensual) (cód 2106)
                        linea.cod_2107 = 0;         // Recargo 30% día domingo (Art. 38) (cód 2107)
                        linea.cod_2108 = 0;         // Remuneración variable pagada en vacaciones (Art 71) (cód 2108)
                        linea.cod_2109 = 0;         // Remuneración variable pagada en clausura (Art. 38 DFL 2) (cód 2109)
                        linea.cod_2110 = 0;         // Aguinaldo (cód 2110)
                        linea.cod_2111 = 0;         // Bonos u otras remuneraciones fijas mensuales (cód 2111)
                        linea.cod_2112 = 0;         // Tratos (mensual) (cód 2112)
                        linea.cod_2113 = 0;         // Bonos u otras remuneraciones variables mensuales o superiores a un mes(cód 2113)
                        linea.cod_2114 = 0;         // Ejercicio opción no pactada en contrato (Art. 17 N°8 LIR) (cód 2114)
                        linea.cod_2115 = 0;         // Beneficios en especie constitutivos de remuneración (cód 2115)
                        linea.cod_2116 = 0;         // Remuneraciones bimestrales (devengo en dos meses) (cód 2116)
                        linea.cod_2117 = 0;         // Remuneraciones trimestrales (devengo en tres meses) (cód 2117)
                        linea.cod_2118 = 0;         // Remuneraciones cuatrimestrales (devengo en cuatro meses) (cód 2118)
                        linea.cod_2119 = 0;         // Remuneraciones semestrales (devengo en sesis meses) (cód 2119)
                        linea.cod_2120 = 0;         // Remuneraciones anuales (devengo en doce meses) (cód 2120)
                        linea.cod_2121 = 0;         // Participación anual (devengo en doce meses (cód 2121)
                        linea.cod_2122 = 0;         // Gratificación anual (devengo en doce meses) (cód 2122)
                        linea.cod_2123 = 0;         // Otras remuneraciones superiores a un mes (cód 2123)
                        linea.cod_2124 = 0;         // Pago por horas de trabajo sindical (cód 2124)
                        linea.cod_2161 = 0;         // Sueldo empresarial (2161)
                        linea.cod_2201 = 0;         // Subsidio por incapacidad laboral por licencia médica - total mensual (cód 2201)
                        linea.cod_2202 = 0;         // Beca de estudio (Art. 17 N°18 LIR) (cód 2202)
                        linea.cod_2203 = 0;         // Gratificaciones de zona (Art.17 N°27) (cód 2203)
                        linea.cod_2204 = 0;         // Otros ingresos no constitutivos de renta (Art 17 N°29 LIR) (cód 2204)
                        linea.cod_2301 = 0;         // Colación total mensual (Art 41) (cód 2301)
                        linea.cod_2302 = 0;         // Movilización total mensual (Art 41) (cód 2302)
                        linea.cod_2303 = 0;         // Viáticos total mensual (Art 41) (cód 2303)
                        linea.cod_2304 = 0;         // Asignación de pérdida de caja total mensual (Art 41) (cód 2304)
                        linea.cod_2305 = 0;         // Asignación de desgaste herramientas total mensual (Art 41) (cód 2305)
                        linea.cod_2311 = 0;         // Asignación familiar legal total mensual (Art 41) (cód 2311)
                        linea.cod_2306 = 0;         // Gastos por causa del trabajo (Art 41 CdT) (2306)
                        linea.cod_2307 = 0;         // Gastos por cambio de residencia (Art 53) (cód 2307)
                        linea.cod_2308 = 0;         // Sala cuna (Art 203) (cód 2308)
                        linea.cod_2309 = 0;         // Asignación trabajo a distancia o teletrabajo (cód 2309)
                        linea.cod_2347 = 0;         // Depósito convenido hasta UF 900 (cód 2347)
                        linea.cod_2310 = 0;         // Alojamiento por razones de trabajo (Art 17 N°14 LIR) (cód 2310)
                        linea.cod_2312 = 0;         // Asignación de traslación (Art. 17 N°15 LIR) (cód 2312)
                        linea.cod_2313 = 0;         // Indemnización por feriado legal (cód 2313)
                        linea.cod_2314 = 0;         // Indemnización años de servicio (cód 2314)
                        linea.cod_2315 = 0;         // Indemnización sustitutiva del aviso previo (cód 2315)
                        linea.cod_2316 = 0;         // Indemnización fuero maternal (Art 163 bis) (cód 2316)
                        linea.cod_2331 = 0;         // Indemnización a todo evento (Art.164) (cód 2331)
                        linea.cod_2417 = 0;         // Indemnizaciones voluntarias tributables (cód 2417)
                        linea.cod_2418 = 0;         // Indemnizaciones contractuales tributables (cód 2418)

        // Parrafo C descuentos
                        linea.cod_3141 = 0;           // Cotización obligatoria previsional (AFP o IPS) (cód 3141)
                        linea.cod_3143 = 0;           // Cotización obligatoria salud 7% (cód 3143)
                        linea.cod_3144 = 0;           // Cotización voluntaria para salud (cód 3144)
                        linea.cod_3151 = 0;           // Cotización AFC -Trabajador (cód 3151) 
                        linea.cod_3146 = 0;           // Cotizaciones técnico extranjero para seguridad social fuera de Chile (cód 3146)
                        linea.cod_3147 = 0;           // Descuento depósito convenido hasta UF 900 anual (cód 3147)
                        linea.cod_3155 = 0;           // Cotización ahorro previsional voluntario individual modalidad A (cód 3155)
                        linea.cod_3156 = 0;           // Cotización ahorro previsional voluntario individual modalidad B hasta UF 50 (cod 3156)
                        linea.cod_3157 = 0;           // Cotización ahorro previsional voluntario colectivo modalidad A (cód 3157)
                        linea.cod_3158 = 0;           // Cotización ahorro previsional voluntario colectivo modalidad B hasta UF 50 (cod 3158) 
                        linea.cod_3161 = 0;           // Impuesto retenido por remuneraciones (cód 3161)
                        linea.cod_3162 = 0;           // Impuesto retenido por indemnizaciones (cód 3162)
                        linea.cod_3163 = 0;           // Mayor retención de impuesto solicitada por el trabajador (cód 3163)
                        linea.cod_3164 = 0;             // Impuesto retenido por reliquidación de remuneraciones devengadas en otros períodos mensuales(cód 3164)
                        linea.cod_3165 = 0;           // Diferencia de impuesto por reliquidación de remuneraciones  devengadas en este período(cód 3165)
                        linea.cod_3166 = 0;           // Retencion presatmo clase media 2020(cód 3166)
                        linea.cod_3167 = 0;           // Rebaja zona extrema DL 889(cód 3167)
                        linea.cod_3171 = 0;           // Cuota sindical 1 (cód 3171)
                        linea.cod_3172 = 0;           // Cuota sindical 2 (cód 3172)
                        linea.cod_3173 = 0;           // Cuota sindical 3 (cód 3173)
                        linea.cod_3174 = 0;           // Cuota sindical 4 (cód 3174)
                        linea.cod_3175 = 0;           // Cuota sindical 5 (cód 3175)
                        linea.cod_3176 = 0;           // Cuota sindical 6 (cód 3176)
                        linea.cod_3177 = 0;           // Cuota sindical 7 (cód 3177)
                        linea.cod_3178 = 0;           // Cuota sindical 8 (cód 3178)
                        linea.cod_3179 = 0;           // Cuota sindical 9 (cód 3179)
                        linea.cod_3180 = 0;           // Cuota sindical 10 (cód 3180)
                        linea.cod_3110 = 0;           // Crédito social CCAF (cód 3110)
                        linea.cod_3181 = 0;           // Cuota vivienda o educación Art. 58 (cód 3181)
                        linea.cod_3182 = 0;           // Crédito cooperativas de ahorro (Art 54 Ley Coop.) (cód 3182)
                        linea.cod_3183 = 0;           // Otros descuentos autorizados y solicitados por el trabajador (cód 3183)
                        linea.cod_3154 = 0;           // Cotización adicional trabajo pesado- trabajador (cód 3154)
                        linea.cod_3184 = 0;           // Donaciones culturales y de reconstrucción (cód 3184)
                        linea.cod_3185 = 0;           // Otros descuentos (Art 58) (cód 3185)
                        linea.cod_3186 = 0;           // Pensiones de alimentos (cód 3186)
                        linea.cod_3187 = 0;           // Descuento mujer casada (Art 59) (cód 3187)
                        linea.cod_3188 = 0;           // Descuento por anticipos o préstamos (cód 3188)

                        linea.cod_4151 = 0;           //Aporte AFC -empleador(cód 4151)
                        linea.cod_4152 = 0;           //Aporte empleador seguro accidentes del trabajo y Ley SANNA(Ley16.744) (cód 4152)
                        linea.cod_4131 = 0;           //Aporte empleador indemnización a todo evento(Art 164) (cód 4131)
                        linea.cod_4154 = 0;           //Aporte adicional trabajo pesado- empleador(cód 4154)
                        linea.cod_4155 = 0;           //Aporte empleador seguro invalidez y sobrevivencia(cód 4155)
                        linea.cod_4157 = 0;           //Aporte empleador ahorro previsional voluntario colectivo(cód 4157)
                        linea.cod_5201 = 0;           //Total haberes(cód 5201)
                        linea.cod_5210 = 0;           //Total haberes imponibles y tributables(cód 5210)
                        linea.cod_5220 = 0;           //Total haberes imponibles y no tributables(cód 5220)
                        linea.cod_5230 = 0;           //Total haberes no imponibles y no tributables(cód 5230)
                        linea.cod_5240 = 0;           //Total haberes no imponibles y tributables(cód 5240)
                        linea.cod_5301 = 0;           //Total descuentos(cód 5301)
                        linea.cod_5361 = 0;           //Total descuentos impuestos a las remuneraciones(cód 5361)
                        linea.cod_5362 = 0;           //Total descuentos impuestos por indemnizaciones(cód 5362)
                        linea.cod_5341 = 0;           //Total descuentos por cotizaciones del trabajador(cód 5341)
                        linea.cod_5302 = 0;           //Total otros descuentos(cód 5302)
                        linea.cod_5410 = 0;           //Total aportes empleador(cód 5410)
                        linea.cod_5501 = 0;           //Total líquido(cód 5501)
                        linea.cod_5502 = 0;           //Total indemnizaciones(cód 5502)
                        linea.cod_5564 = 0;           //Total indemnizaciones tributables(cód 5564)
                        linea.cod_5565 = 0;           //Total indemnizaciones no tributables(cód 5565)


                        var detalle = resultados.Where(x => x.rutTrabajador == t.rutTrabajador).ToList();
                        foreach (var r in detalle)
                        {
                            string codigodt=null;
                            if(r.concepto < 100)
                            {
                                codigodt = listahaberes.Where(x=> x.haber == r.concepto).Select(x=> x.codigoDT).FirstOrDefault();
                            }
                            else
                            {
                                if(r.concepto < 900)
                                {
                                    codigodt = listadescuentos.Where(x => x.descuento == r.concepto).Select(x => x.codigoDT).FirstOrDefault();
                                }
                                else
                                {
                                    switch (r.concepto)
                                    {
                                        case 910:codigodt = "3141";break;
                                        case 2100: codigodt = "5201"; break;
                                        case 2010: codigodt = "5210"; break;
                                        case 2020: codigodt = "5230"; break;
                                        case 2400: codigodt = "4151"; break;
                                        case 2405: codigodt = "4152"; break;
                                        case 2404: codigodt = "4155"; break;
                                        case 2421: codigodt = "3143"; break;
                                        case 916: codigodt = "3151"; break;
                                        case 990: codigodt = "3161"; break;
                                        case 940: codigodt = "3156"; break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            int codigo = Convert.ToInt32(codigodt);
                            switch (codigo)
                            {
                                case 2101:
                                    linea.cod_2101 = linea.cod_2101 + (int)r.monto;
                                    linea.cod_1115 = (int)r.cantidad;
                                    break;
                                case 2102:linea.cod_2102 = linea.cod_2102 + (int)r.monto;break;
                                case 2103:linea.cod_2103 = linea.cod_2103 + (int)r.monto;break;
                                case 2104:linea.cod_2104 = linea.cod_2104 + (int)r.monto;break;
                                case 2105:linea.cod_2105 = linea.cod_2105 + (int)r.monto;break;
                                case 2106:linea.cod_2106 = linea.cod_2106 + (int)r.monto;break;
                                case 2107: linea.cod_2107 = linea.cod_2107 + (int)r.monto; break;
                                case 2108: linea.cod_2108 = linea.cod_2108 + (int)r.monto; break;
                                case 2109: linea.cod_2109 = linea.cod_2109 + (int)r.monto; break;
                                case 2110: linea.cod_2110 = linea.cod_2110 + (int)r.monto; break;
                                case 2111: linea.cod_2111 = linea.cod_2111 + (int)r.monto; break;
                                case 2112: linea.cod_2112 = linea.cod_2112 + (int)r.monto; break;
                                case 2113: linea.cod_2113 = linea.cod_2113 + (int)r.monto; break;
                                case 2114: linea.cod_2114 = linea.cod_2114 + (int)r.monto; break;
                                case 2115: linea.cod_2115 = linea.cod_2115 + (int)r.monto; break;
                                case 2116: linea.cod_2116 = linea.cod_2116 + (int)r.monto; break;
                                case 2117: linea.cod_2117 = linea.cod_2117 + (int)r.monto; break;
                                case 2118: linea.cod_2118 = linea.cod_2118 + (int)r.monto; break;
                                case 2119: linea.cod_2119 = linea.cod_2119 + (int)r.monto; break;
                                case 2120: linea.cod_2120 = linea.cod_2120 + (int)r.monto; break;
                                case 2121: linea.cod_2121 = linea.cod_2121 + (int)r.monto; break;
                                case 2122: linea.cod_2122 = linea.cod_2122 + (int)r.monto; break;
                                case 2123: linea.cod_2123 = linea.cod_2123 + (int)r.monto; break;
                                case 2124: linea.cod_2124 = linea.cod_2124 + (int)r.monto; break;
                                case 2201: linea.cod_2201 = linea.cod_2201 + (int)r.monto; break;
                                case 2202: linea.cod_2202 = linea.cod_2202 + (int)r.monto; break;
                                case 2203: linea.cod_2203 = linea.cod_2203 + (int)r.monto; break;
                                case 2204: linea.cod_2204 = linea.cod_2204 + (int)r.monto; break;
                                case 2301: linea.cod_2301 = linea.cod_2301 + (int)r.monto; break;
                                case 2302: linea.cod_2302 = linea.cod_2302 + (int)r.monto; break;
                                case 2303: linea.cod_2303 = linea.cod_2303 + (int)r.monto; break;
                                case 2304: linea.cod_2304 = linea.cod_2304 + (int)r.monto; break;
                                case 2305: linea.cod_2305 = linea.cod_2305 + (int)r.monto; break;
                                case 2306: linea.cod_2306 = linea.cod_2306 + (int)r.monto; break;
                                case 2307: linea.cod_2307 = linea.cod_2307 + (int)r.monto; break;
                                case 2308: linea.cod_2308 = linea.cod_2308 + (int)r.monto; break;
                                case 2309: linea.cod_2309 = linea.cod_2309 + (int)r.monto; break;
                                case 2311: 
                                    linea.cod_2311 = linea.cod_2311 + (int)r.monto;
                                    linea.cod_1111 = linea.cod_1111 + (int)r.cantidad;
                                    break;
                                case 2310: linea.cod_2310 = linea.cod_2310 + (int)r.monto; break;
                                case 2312: linea.cod_2312 = linea.cod_2312 + (int)r.monto; break;
                                case 2313: linea.cod_2313 = linea.cod_2313 + (int)r.monto; break;
                                case 2314: linea.cod_2314 = linea.cod_2314 + (int)r.monto; break;
                                case 2315: linea.cod_2315 = linea.cod_2315 + (int)r.monto; break;
                                case 2316: linea.cod_2316 = linea.cod_2316 + (int)r.monto; break;
                                case 2331: linea.cod_2331 = linea.cod_2331 + (int)r.monto; break;
                                case 2347: linea.cod_2347 = linea.cod_2347 + (int)r.monto; break;
                                case 2417: linea.cod_2417 = linea.cod_2417 + (int)r.monto; break;
                                case 2418: linea.cod_2418 = linea.cod_2418 + (int)r.monto; break;

                         // Descuentos
                                case 3110: linea.cod_3110 = linea.cod_3110 + (int)r.monto; break;
                                case 3154: linea.cod_3154 = linea.cod_3154 + (int)r.monto; break;
                                case 3163: linea.cod_3163 = linea.cod_3163 + (int)r.monto; break;
                                case 3166: linea.cod_3166 = linea.cod_3166 + (int)r.monto; break;
                                case 3171: linea.cod_3171 = linea.cod_3171 + (int)r.monto; break;
                                case 3172: linea.cod_3172 = linea.cod_3172 + (int)r.monto; break;
                                case 3173: linea.cod_3173 = linea.cod_3173 + (int)r.monto; break;
                                case 3174: linea.cod_3174 = linea.cod_3174 + (int)r.monto; break;
                                case 3175: linea.cod_3175 = linea.cod_3175 + (int)r.monto; break;
                                case 3176: linea.cod_3176 = linea.cod_3176 + (int)r.monto; break;
                                case 3177: linea.cod_3177 = linea.cod_3177 + (int)r.monto; break;
                                case 3178: linea.cod_3178 = linea.cod_3178 + (int)r.monto; break;
                                case 3179: linea.cod_3179 = linea.cod_3179 + (int)r.monto; break;
                                case 3180: linea.cod_3180 = linea.cod_3180 + (int)r.monto; break;
                                case 3181: linea.cod_3181 = linea.cod_3181 + (int)r.monto; break;
                                case 3182: linea.cod_3182 = linea.cod_3182 + (int)r.monto; break;
                                case 3183: linea.cod_3183 = linea.cod_3183 + (int)r.monto; break;
                                case 3184: linea.cod_3184 = linea.cod_3184 + (int)r.monto; break;
                                case 3185: linea.cod_3185 = linea.cod_3185 + (int)r.monto; break;
                                case 3186: linea.cod_3186 = linea.cod_3186 + (int)r.monto; break;
                                case 3187: linea.cod_3187 = linea.cod_3187 + (int)r.monto; break;
                                case 3188: linea.cod_3188 = linea.cod_3188 + (int)r.monto; break;
                                // Otros conceptos
                                case 5201: linea.cod_5201 = linea.cod_5201 + (int)r.monto; break;
                                case 5210: linea.cod_5210 = linea.cod_5210 + (int)r.monto; break;
                                case 5230: linea.cod_5230 = linea.cod_5230 + (int)r.monto; break;
                                case 3141:
                                    {
                                        linea.cod_3141 = linea.cod_3141 + (int)r.monto;
                                        linea.cod_1141 = linea.cod_1141 + (int)r.cantidad; break;
                                    }
                                case 4151: linea.cod_4151 = linea.cod_4151 + (int)r.monto; break;
                                case 4152: linea.cod_4152 = linea.cod_4152 + (int)r.monto; break;
                                case 4155: linea.cod_4155 = linea.cod_4155 + (int)r.monto; break;
                                case 3143: linea.cod_3143 = linea.cod_3143 + (int)r.monto; break;
                                case 3151: linea.cod_3151 = linea.cod_3151 + (int)r.monto; break;
                                case 3161: linea.cod_3161 = linea.cod_3161 + (int)r.monto; break;
                                case 3156: linea.cod_3156 = linea.cod_3156 + (int)r.monto; break;
                                case 3144: linea.cod_3144 = linea.cod_3144 + (int)r.monto; break;
                                default:
                                    break;
                            }
                        }
                        // totales
                        linea.cod_5301 = linea.cod_3163 + linea.cod_3166 + linea.cod_3171 + linea.cod_3172 + linea.cod_3173 + linea.cod_3174 
                                 + linea.cod_3175 + linea.cod_3176 + linea.cod_3177 + linea.cod_3178 + linea.cod_3179
                                 + linea.cod_3180 + linea.cod_3110 + linea.cod_3181 + linea.cod_3182 + linea.cod_3183 + linea.cod_3154 
                                 + linea.cod_3184 + linea.cod_3185 + linea.cod_3186 + linea.cod_3187 + linea.cod_3188 + linea.cod_3161
                                 + linea.cod_3162 + linea.cod_3165 + linea.cod_3141 + linea.cod_3143 + linea.cod_3144 + linea.cod_3146
                                 + linea.cod_3151 + linea.cod_3154 + linea.cod_3155 + linea.cod_3156 + linea.cod_3157 + linea.cod_3158;

                        linea.cod_5201 = linea.cod_5210 + linea.cod_5220 + linea.cod_5230 + linea.cod_5240;
                        linea.cod_5361 = linea.cod_3161 + linea.cod_3165;
                        linea.cod_5362 = linea.cod_3162;
                        linea.cod_5341 = linea.cod_3141 + linea.cod_3143 + linea.cod_3144 + linea.cod_3146 + linea.cod_3151 + linea.cod_3154
                                 + linea.cod_3155 + linea.cod_3156 + linea.cod_3157 + linea.cod_3158;
                        linea.cod_5302 = linea.cod_5301 - linea.cod_5361 - linea.cod_5362 - linea.cod_5341;
                        linea.cod_5410 = linea.cod_4151 + linea.cod_4152 + linea.cod_4131 + linea.cod_4154 + linea.cod_4155 + linea.cod_4157;
                        linea.cod_5501 = linea.cod_5201 - linea.cod_5301;
                        linea.cod_5502 = linea.cod_2313 + linea.cod_2314 + linea.cod_2315 + linea.cod_2316 + linea.cod_2331 + linea.cod_2417 + linea.cod_2418;
                        linea.cod_5564 = linea.cod_2417 + linea.cod_2418;
                        linea.cod_5565 = linea.cod_2313 + linea.cod_2314 + linea.cod_2315 + linea.cod_2316 + linea.cod_2331;

                        libro.Add(linea);
                    }
                    TextWriter Escribir = new StreamWriter(ruta);
                    foreach (var s in libro)
                    {
                        string lineaa =  s.cod_1101 + ";" + s.cod_1102 + ";" + s.cod_1103 + ";" + s.cod_1104 + ";" + s.cod_1105 + ";" + s.cod_1106 + ";" + s.cod_1170 + ";" +
                                         s.cod_1146 + ";" + s.cod_1107 + ";" +
                                         s.cod_1108 + ";" + s.cod_1109 + ";" + s.cod_1141 + ";" + s.cod_1142 + ";" + s.cod_1143 + ";" + s.cod_1151 + ";" + s.cod_1110 + ";" +
                                         s.cod_1152 + ";" + s.cod_1111 + ";" + s.cod_1112 + ";" + s.cod_1113 + ";" + s.cod_1114 + ";" +
                                         s.cod_1171 + ";" + s.cod_1172 + ";" + s.cod_1173 + ";" + s.cod_1174 + ";" + s.cod_1175 + ";" + s.cod_1176 + ";" + s.cod_1177 + ";" +
                                         s.cod_1178 + ";" + s.cod_1179 + ";" + s.cod_1180 + ";" + s.cod_1115 + ";" + s.cod_1116 + ";" + s.cod_1117 + ";" +
                                         s.cod_1118 + ";" + s.cod_1154 + ";" + s.cod_1155 + ";" + s.cod_1157 + ";" +
                                         s.cod_1131 + ";" + s.cod_1132 + ";" +
                                         s.cod_2101 + ";" + s.cod_2102 + ";" + s.cod_2103 + ";" + s.cod_2104 + ";" + s.cod_2105 + ";" + s.cod_2106 + ";" + s.cod_2107 + ";" +
                                         s.cod_2108 + ";" + s.cod_2109 + ";" + s.cod_2110 + ";" + s.cod_2111 + ";" + s.cod_2112 + ";" + s.cod_2113 + ";" + s.cod_2114 + ";" +
                                         s.cod_2115 + ";" + s.cod_2116 + ";" + s.cod_2117 + ";" + s.cod_2118 + ";" + s.cod_2119 + ";" + s.cod_2120 + ";" + s.cod_2121 + ";" +
                                         s.cod_2122 + ";" + s.cod_2123 + ";" + s.cod_2124 + ";" + s.cod_2161 + ";" +
                                         s.cod_2201 + ";" + s.cod_2202 + ";" + s.cod_2203 + ";" + s.cod_2204 + ";" +
                                         s.cod_2301 + ";" + s.cod_2302 + ";" + s.cod_2303 + ";" + s.cod_2304 + ";" + s.cod_2305 + ";" + s.cod_2306 + ";" + s.cod_2307 + ";" +
                                         s.cod_2308 + ";" + s.cod_2309 + ";" + s.cod_2310 + ";" + s.cod_2311 + ";" + s.cod_2312 + ";" + s.cod_2313 + ";" + s.cod_2314 + ";" +
                                         s.cod_2315 + ";" + s.cod_2316 + ";" + s.cod_2331 + ";" + s.cod_2347 + ";" + s.cod_2417 + ";" + s.cod_2418 + ";" +
                                         s.cod_3141 + ";" + s.cod_3143 + ";" + s.cod_3144 + ";" + s.cod_3151 + ";" + s.cod_3146 + ";" + s.cod_3147 + ";" + s.cod_3155 + ";" +
                                         s.cod_3156 + ";" + s.cod_3157 + ";" + s.cod_3158 + ";" + s.cod_3161 + ";" +

                                         s.cod_3162 + ";" + s.cod_3163 + ";" + s.cod_3164 + ";" + s.cod_3165 + ";" + s.cod_3166 + ";" + s.cod_3167 + ";" + s.cod_3171 + ";" +
                                         s.cod_3172 + ";" + s.cod_3173 + ";" + s.cod_3174 + ";" +
                                         s.cod_3175 + ";" + s.cod_3176 + ";" + s.cod_3177 + ";" + s.cod_3178 + ";" + s.cod_3179 + ";" + s.cod_3180 + ";" + s.cod_3110 + ";" +
                                         s.cod_3181 + ";" +
                                         s.cod_3182 + ";" + s.cod_3183 + ";" + s.cod_3154 + ";" + s.cod_3184 + ";" + s.cod_3185 + ";" + s.cod_3186 + ";" + s.cod_3187 + ";" +
                                         s.cod_3188 + ";" +
                                         s.cod_4151 + ";" + s.cod_4152 + ";" + s.cod_4131 + ";" + s.cod_4154 + ";" + s.cod_4155 + ";" + s.cod_4157 + ";" + s.cod_5201 + ";" +
                                         s.cod_5210 + ";" + s.cod_5220 + ";" + s.cod_5230 + ";" + s.cod_5240 + ";" + s.cod_5301 + ";" + s.cod_5361 + ";" + s.cod_5362 + ";" +
                                         s.cod_5341 + ";" + s.cod_5302 + ";" + s.cod_5410 + ";" + s.cod_5501 + ";" + s.cod_5502 + ";" + s.cod_5564 + ";" + s.cod_5565;

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
        public List<HaberBaseVM> CargaHaberes(string BD_Cli)
        {
            try
            {
                f.EjecutarConsultaSQLCli("select * from haberes ", BD_Cli);
                List<HaberBaseVM> lista = new List<HaberBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {
                    lista = (from DataRow dr in f.Tabla.Rows
                             select new HaberBaseVM()
                             {
                                 haber = int.Parse(dr["haber"].ToString()),
                                 codigoDT = dr["codigoDT"].ToString()
                             }).ToList();
                }
                return lista;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
        public List<DescuentoBaseVM> CargaDescuentos(string BD_Cli)
        {
            try
            {
                f.EjecutarConsultaSQLCli("select * from descuentos ", BD_Cli);
                List<DescuentoBaseVM> lista = new List<DescuentoBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {
                    lista = (from DataRow dr in f.Tabla.Rows
                             select new DescuentoBaseVM()
                             {
                                 descuento = int.Parse(dr["descuento"].ToString()),
                                 codigoDT = dr["codigoDT"].ToString()
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
    public class Detdireccion
    {
        //  1 Datos trabajador
        // Parrafo A
        public string cod_1101 = "";       // Rut trabajador
        public string cod_1102 = null;     // Fecha inicio contrato (cód 1102)
        public string cod_1103 = null;     // Fecha de término de contrato (cód 1103)
        public string cod_1104 = null;     // Causal de término del contrato (cód 1104)
        public string cod_1105 = null;     // Región de prestación de los servicios (cód 1105)
        public string cod_1106 = "1100";   // Comuna de prestación de los servicios (cód 1106)
        public string cod_1170 = "1";      // Tipo de impuesto a la renta (cód 1170)
        public string cod_1146 = "0";      // Técnico extranjero exención de cotizaciones previsionales (Ley 18.156)(cód 1146)
        public string cod_1107 = "101";    // Código tipo de jornada (cód 1107)
        public string cod_1108 = "0";      // Persona con discapacidad/pensionado por invalidez (cód 1108)
        public string cod_1109 = "0";      // Pensionado por vejez (cód 1109)
        public string cod_1141 = "0";     // AFP (cód 1141)
        public string cod_1142 = "0";      // IPS (ExINP) (cód 1142)
        public string cod_1143 = "102";    // FONASA / ISAPRE (cód 1143)
        public string cod_1151 = "1";      // AFC (cód 1151)
        public string cod_1110 = "0";      // CCAF (cód 1110)
        public string cod_1152 = "0";      // Org. Administrador Ley 16.744 (cód 1152)
        public int cod_1111 = 0;           // Número cargas familiares legales autorizadas (cód 1111)
        public int cod_1112 = 0;           // Número de cargas familiares maternales (cód 1112)
        public int cod_1113 = 0;           // Número de cargas familiares invalidez (cód 1113)
        public string cod_1114 = "D";      // Tramo asignación familiar (cód 1114)
        public string cod_1171 = "";       // Rut organización sindical 1 (cód 1171)
        public string cod_1172 = "";       // Rut organización sindical 1 (cód 1172) 
        public string cod_1173 = "";        // Rut organización sindical 1 (cód 1173) 
        public string cod_1174 = "";       // Rut organización sindical 1 (cód 1174) 
        public string cod_1175 = "";       // Rut organización sindical 1 (cód 1175) 
        public string cod_1176 = "";       // Rut organización sindical 1 (cód 1176) 
        public string cod_1177 = "";       // Rut organización sindical 1 (cód 1177) 
        public string cod_1178 = "";       // Rut organización sindical 1 (cód 1178)
        public string cod_1179 = "";       // Rut organización sindical 1 (cód 1179) 
        public string cod_1180 = "";       // Rut organización sindical 1 (cód 1180) 
        public int cod_1115 = 0;           // Número de días trabajados en el mes (cód 1115)
        public int cod_1116 = 0;           // Número días de licencia médica en el mes (cód 1116)   
        public int cod_1117 = 0;           // Número días de vacaciones en el mes (cód 1117)    
        public string cod_1118 = "0";      // Subsidio trabajador Joven (cód 1118)
        public string cod_1154 = "";       // Puesto trabajo pesado (cód 1154)
        public string cod_1155 = "0";      // Ahorro previsional voluntario individual (cód 1155)
        public string cod_1157 = "0";      // Ahorro previsional voluntario colectivo (cód 1157)
        public string cod_1131 = "0";      // Indemnización a todo evento (art 164) (cód 1131)
        public Decimal cod_1132 = 0;       // Tasa indemnización a todo evento (Art 164) (cód 1132)

        // Parrafo B haberes
        public int cod_2101 = 0;         // Sueldo (cód 2101)
        public int cod_2102 = 0;         // Sobresueldo (cód 2102)
        public int cod_2103 = 0;         // Comisiones (mensual) (cód 2103)
        public int cod_2104 = 0;         // Semana corrida mensual (Art 45) (cód 2104)
        public int cod_2105 = 0;         // Participación (mensual) (cód 2105)
        public int cod_2106 = 0;         // Gratificación (mensual) (cód 2106)
        public int cod_2107 = 0;         // Recargo 30% día domingo (Art. 38) (cód 2107)
        public int cod_2108 = 0;         // Remuneración variable pagada en vacaciones (Art 71) (cód 2108)
        public int cod_2109 = 0;         // Remuneración variable pagada en clausura (Art. 38 DFL 2) (cód 2109)
        public int cod_2110 = 0;         // Aguinaldo (cód 2110)
        public int cod_2111 = 0;         // Bonos u otras remuneraciones fijas mensuales (cód 2111)
        public int cod_2112 = 0;         // Tratos (mensual) (cód 2112)
        public int cod_2113 = 0;         // Bonos u otras remuneraciones variables mensuales o superiores a un mes(cód 2113)
        public int cod_2114 = 0;         // Ejercicio opción no pactada en contrato (Art. 17 N°8 LIR) (cód 2114)
        public int cod_2115 = 0;         // Beneficios en especie constitutivos de remuneración (cód 2115)
        public int cod_2116 = 0;         // Remuneraciones bimestrales (devengo en dos meses) (cód 2116)
        public int cod_2117 = 0;         // Remuneraciones trimestrales (devengo en tres meses) (cód 2117)
        public int cod_2118 = 0;         // Remuneraciones cuatrimestrales (devengo en cuatro meses) (cód 2118)
        public int cod_2119 = 0;         // Remuneraciones semestrales (devengo en sesis meses) (cód 2119)
        public int cod_2120 = 0;         // Remuneraciones anuales (devengo en doce meses) (cód 2120)
        public int cod_2121 = 0;         // Participación anual (devengo en doce meses (cód 2121)
        public int cod_2122 = 0;         // Gratificación anual (devengo en doce meses) (cód 2122)
        public int cod_2123 = 0;         // Otras remuneraciones superiores a un mes (cód 2123)
        public int cod_2124 = 0;         // Pago por horas de trabajo sindical (cód 2124)
        public int cod_2161 = 0;         // Sueldo empresarial (2161)
        public int cod_2201 = 0;         // Subsidio por incapacidad laboral por licencia médica - total mensual (cód 2201)
        public int cod_2202 = 0;         // Beca de estudio (Art. 17 N°18 LIR) (cód 2202)
        public int cod_2203 = 0;         // Gratificaciones de zona (Art.17 N°27) (cód 2203)
        public int cod_2204 = 0;         // Otros ingresos no constitutivos de renta (Art 17 N°29 LIR) (cód 2204)
        public int cod_2301 = 0;         // Colación total mensual (Art 41) (cód 2301)
        public int cod_2302 = 0;         // Movilización total mensual (Art 41) (cód 2302)
        public int cod_2303 = 0;         // Viáticos total mensual (Art 41) (cód 2303)
        public int cod_2304 = 0;         // Asignación de pérdida de caja total mensual (Art 41) (cód 2304)
        public int cod_2305 = 0;         // Asignación de desgaste herramientas total mensual (Art 41) (cód 2305)
        public int cod_2311 = 0;         // Asignación familiar legal total mensual (Art 41) (cód 2311)
        public int cod_2306 = 0;         // Gastos por causa del trabajo (Art 41 CdT) (2306)
        public int cod_2307 = 0;         // Gastos por cambio de residencia (Art 53) (cód 2307)
        public int cod_2308 = 0;         // Sala cuna (Art 203) (cód 2308)
        public int cod_2309 = 0;         // Asignación trabajo a distancia o teletrabajo (cód 2309)
        public int cod_2347 = 0;         // Depósito convenido hasta UF 900 (cód 2347)
        public int cod_2310 = 0;         // Alojamiento por razones de trabajo (Art 17 N°14 LIR) (cód 2310)
        public int cod_2312 = 0;         // Asignación de traslación (Art. 17 N°15 LIR) (cód 2312)
        public int cod_2313 = 0;         // Indemnización por feriado legal (cód 2313)
        public int cod_2314 = 0;         // Indemnización años de servicio (cód 2314)
        public int cod_2315 = 0;         // Indemnización sustitutiva del aviso previo (cód 2315)
        public int cod_2316 = 0;         // Indemnización fuero maternal (Art 163 bis) (cód 2316)
        public int cod_2331 = 0;         // Indemnización a todo evento (Art.164) (cód 2331)
        public int cod_2417 = 0;         // Indemnizaciones voluntarias tributables (cód 2417)
        public int cod_2418 = 0;         // Indemnizaciones contractuales tributables (cód 2418)

        // Parrafo C descuentos
        public int cod_3141 = 0;           // Cotización obligatoria previsional (AFP o IPS) (cód 3141)
        public int cod_3143 = 0;           // Cotización obligatoria salud 7% (cód 3143)
        public int cod_3144 = 0;           // Cotización voluntaria para salud (cód 3144)
        public int cod_3151 = 0;           // Cotización AFC -Trabajador (cód 3151) 
        public int cod_3146 = 0;           // Cotizaciones técnico extranjero para seguridad social fuera de Chile (cód 3146)
        public int cod_3147 = 0;           // Descuento depósito convenido hasta UF 900 anual (cód 3147)
        public int cod_3155 = 0;           // Cotización ahorro previsional voluntario individual modalidad A (cód 3155)
        public int cod_3156 = 0;           // Cotización ahorro previsional voluntario individual modalidad B hasta UF 50 (cod 3156)
        public int cod_3157 = 0;           // Cotización ahorro previsional voluntario colectivo modalidad A (cód 3157)
        public int cod_3158 = 0;           // Cotización ahorro previsional voluntario colectivo modalidad B hasta UF 50 (cod 3158) 
        public int cod_3161 = 0;           // Impuesto retenido por remuneraciones (cód 3161)
        public int cod_3162 = 0;           // Impuesto retenido por indemnizaciones (cód 3162)
        public int cod_3163 = 0;           // Mayor retención de impuesto solicitada por el trabajador (cód 3163)
        public int cod_3164 = 0;           // Impuesto retenido por reliquidación de remuneraciones devengadas en otros períodos mensuales(cód 3164)
        public int cod_3165 = 0;           // Diferencia de impuesto por reliquidación de remuneraciones  devengadas en este período(cód 3165)
        public int cod_3166 = 0;           // Retencion presatmo clase media 2020(cód 3166)
        public int cod_3167 = 0;           // Rebaja zona extrema DL 889(cód 3167)
        public int cod_3171 = 0;           // Cuota sindical 1 (cód 3171)
        public int cod_3172 = 0;           // Cuota sindical 2 (cód 3172)
        public int cod_3173 = 0;           // Cuota sindical 3 (cód 3173)
        public int cod_3174 = 0;           // Cuota sindical 4 (cód 3174)
        public int cod_3175 = 0;           // Cuota sindical 5 (cód 3175)
        public int cod_3176 = 0;           // Cuota sindical 6 (cód 3176)
        public int cod_3177 = 0;           // Cuota sindical 7 (cód 3177)
        public int cod_3178 = 0;           // Cuota sindical 8 (cód 3178)
        public int cod_3179 = 0;           // Cuota sindical 9 (cód 3179)
        public int cod_3180 = 0;           // Cuota sindical 10 (cód 3180)
        public int cod_3110 = 0;           // Crédito social CCAF (cód 3110)
        public int cod_3181 = 0;           // Cuota vivienda o educación Art. 58 (cód 3181)
        public int cod_3182 = 0;           // Crédito cooperativas de ahorro (Art 54 Ley Coop.) (cód 3182)
        public int cod_3183 = 0;           // Otros descuentos autorizados y solicitados por el trabajador (cód 3183)
        public int cod_3154 = 0;           // Cotización adicional trabajo pesado- trabajador (cód 3154)
        public int cod_3184 = 0;           // Donaciones culturales y de reconstrucción (cód 3184)
        public int cod_3185 = 0;           // Otros descuentos (Art 58) (cód 3185)
        public int cod_3186 = 0;           // Pensiones de alimentos (cód 3186)
        public int cod_3187 = 0;           // Descuento mujer casada (Art 59) (cód 3187)
        public int cod_3188 = 0;           // Descuento por anticipos o préstamos (cód 3188)

        public int cod_4151 = 0;           //Aporte AFC -empleador(cód 4151)
        public int cod_4152 = 0;           //Aporte empleador seguro accidentes del trabajo y Ley SANNA(Ley16.744) (cód 4152)
        public int cod_4131 = 0;           //Aporte empleador indemnización a todo evento(Art 164) (cód 4131)
        public int cod_4154 = 0;           //Aporte adicional trabajo pesado- empleador(cód 4154)
        public int cod_4155 = 0;           //Aporte empleador seguro invalidez y sobrevivencia(cód 4155)
        public int cod_4157 = 0;           //Aporte empleador ahorro previsional voluntario colectivo(cód 4157)
        public int cod_5201 = 0;           //Total haberes(cód 5201)
        public int cod_5210 = 0;           //Total haberes imponibles y tributables(cód 5210)
        public int cod_5220 = 0;           //Total haberes imponibles y no tributables(cód 5220)
        public int cod_5230 = 0;           //Total haberes no imponibles y no tributables(cód 5230)
        public int cod_5240 = 0;           //Total haberes no imponibles y tributables(cód 5240)
        public int cod_5301 = 0;           //Total descuentos(cód 5301)
        public int cod_5361 = 0;           //Total descuentos impuestos a las remuneraciones(cód 5361)
        public int cod_5362 = 0;           //Total descuentos impuestos por indemnizaciones(cód 5362)
        public int cod_5341 = 0;           //Total descuentos por cotizaciones del trabajador(cód 5341)
        public int cod_5302 = 0;           //Total otros descuentos(cód 5302)
        public int cod_5410 = 0;           //Total aportes empleador(cód 5410)
        public int cod_5501 = 0;           //Total líquido(cód 5501)
        public int cod_5502 = 0;           //Total indemnizaciones(cód 5502)
        public int cod_5564 = 0;           //Total indemnizaciones tributables(cód 5564)
        public int cod_5565 = 0;           //Total indemnizaciones no tributables(cód 5565)

    }
}