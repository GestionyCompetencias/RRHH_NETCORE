using MongoDB.Driver.Search;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.SolicitudVacaciones
{
    /// <summary>
    /// Servicio para gestionar vavaciones.
    /// </summary>
    public interface ISolicitudVacacionesService
    {
        /// <summary>
        /// Genera solicitud de vacaciones.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="desde">desde fecha </param>
        /// <param name="hasta">hasta fecha</param>
        /// <returns>Resultado</returns>
        public Resultado MostrarSolicitudVacacionesService(int idEmpresa, string desde, string hasta, string rut);

        /// <summary>
        /// Procesa solicitud de vacaciones.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="desde">desde fecha </param>
        /// <param name="hasta">hasta fecha</param>
        /// <returns>Resultado</returns>
        public Resultado ProcesarSolicitudVacacionesService(int idEmpresa, string desde, string hasta, string rut);
 
        /// <summary>
        /// Verifica la validez de la solicitud.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="desde">desde fecha </param>
        /// <param name="hasta">hasta fecha</param>
        /// <returns>Resultado</returns>
        public Resultado VerificaSolicitudVacacionesService(int idEmpresa, string desde, string hasta, string rut);

    }

    public class SolicitudVacacionesService : ISolicitudVacacionesService
    {
        private readonly IDatabaseManager _databaseManager;
        private static Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Generales Generales = new Generales();
        private Seguridad seguridad = new Seguridad();

        public SolicitudVacacionesService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }
        public Resultado MostrarSolicitudVacacionesService(int empresa, string desde, string hasta, string rut)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "Puede pedir vacaciones";
            List<Detperiodos> ListaPer= Periodos(empresa,rut);
            Double diascorridos = (Convert.ToDateTime(hasta).Date - Convert.ToDateTime(desde).Date).TotalDays +1;
            Double diashabiles = DiasHabiles(desde,hasta);
            List<Detperiodos> sugerencia = BuscaPeriodosUsar(ListaPer, diashabiles, desde, hasta);
            if (sugerencia.Count > 0) 
            {
                resultado.result = 1;
                resultado.mensaje = "Existen vacaciones";
            }
            resultado.data = sugerencia;
            return resultado;
        }
        public Resultado ProcesarSolicitudVacacionesService(int empresa, string desde, string hasta, string rut)
       {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            DateTime hoy = DateTime.Now;
            string hoystr = hoy.ToString("yyyy'-'MM'-'dd' 'hh':'mm");
            string query2 = "insert into vacacionesSolicitud (rutTrabajador, numeroSolicitud,fechaSolicitud, estadoSolicitud, fechaInicio, fechaTermino ,habilitado) " +
           "values " +
           "('" + rut + "'," + 0 + ",'" + hoystr + "','Solicitud', '" +   desde  + "', '" + hasta + "' ,1)  ";
            if (f.EjecutarQuerySQLCli(query2, BD_Cli))
            {
                resultado.result = 1;
                resultado.mensaje = "Se grabo la solicitud de vacaciones.";
            }
            return resultado;
        }
        //public Resultado ProcesarUsosVacacionesService(int empresa, string desde, string hasta, string rut)
        //{
        //    Resultado resultado = new Resultado();
        //    resultado.result = 0;
        //    resultado.mensaje = "No se grabo solicitud de vacaciones";
        //    DateTime hoy = DateTime.Now;
        //    string RutEmpresa = f.obtenerRUT(empresa);
        //    var BD_Cli = "remuneracion_" + RutEmpresa;
        //    List<Detperiodos> ListaPer = Periodos(empresa, rut);
        //    Double diascorridos = (Convert.ToDateTime(hasta).Date - Convert.ToDateTime(desde).Date).TotalDays + 1;
        //    Double diashabiles = DiasHabiles(desde, hasta);
        //    List<Detperiodos> sugerencia = BuscaPeriodosUsar(ListaPer, diashabiles,desde,hasta);
        //    if (sugerencia.Count > 0)
        //    {
        //        foreach (var s in sugerencia)
        //        {
        //            string query2 = "insert into vacacionesUsos (rutTrabajador, anoInicio, anoTermino, tipoUso, fechaInicio, fechaTermino, numeroSolicitud " +
        //            ",diasCorridos, diasLegales, diasProgresivos, diasContrato" +
        //            ", diasAdministrativos,diasFaena,diasEspeciales, diasOtros,fechaProceso,habilitado) " +
        //            "values " +
        //            "('" + s.ruttrabajador.Trim() + "'," + s.anoinicio + "," + s.anotermino + ",'V', '" + 
        //             s.fechainicio + "', '" +s.fechatermino+"', 0, " +diascorridos+", " + s.diaslegales + "," + s.diasprogresivos +
        //            ", " + s.diascontrato + "," + s.diasadministrativos + "," + s.diasfaena + "," + s.diasespeciales +
        //            ", " + s.diasotros + ", '" + hoy + "' ,1)  ";
        //            if(f.EjecutarQuerySQLCli(query2, BD_Cli))
        //            {
        //                resultado.result = 1;
        //                resultado.mensaje = "Se grabo la solicitud de vacaciones.";
        //            }
        //        }
        //    }
        //    resultado.data = sugerencia;
        //    return resultado;
        //}
        public Resultado VerificaSolicitudVacacionesService(int empresa, string desde, string hasta, string rut)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            DateTime desdedate = Convert.ToDateTime(desde);
            DateTime hastadate = Convert.ToDateTime(hasta);
            if(desdedate > hastadate)
            {
                resultado.mensaje = "Fecha de inicio mayor que fecha de termino";
                return resultado ;
            }
            if (desdedate.DayOfWeek == DayOfWeek.Sunday || desdedate.DayOfWeek == DayOfWeek.Saturday)
            {
                resultado.mensaje = "Fecha de inicio es dia de descanso";
                return resultado;
            }
            if (Generales.EsFeriado(desdedate, 1) == true)
            {
                resultado.mensaje = "Fecha de inicio es dia feriado";
                return resultado;
            }
            if (hastadate.DayOfWeek == DayOfWeek.Sunday || hastadate.DayOfWeek == DayOfWeek.Saturday)
            {
                resultado.mensaje = "Fecha de termino es dia de descanso";
                return resultado;
            }
            if (Generales.EsFeriado(hastadate, 1) == true)
            {
                resultado.mensaje = "Fecha de termino es dia feriado";
                return resultado;
            }

            resultado.result = 1;
            resultado.mensaje = "Existen vacaciones";
            return resultado;
        }

        public List<Detperiodos> BuscaPeriodosUsar(List<Detperiodos> periodos,double dias,string desde,string hasta)
        {
            List<Detperiodos> ausar = new List<Detperiodos>();
            DateTime inicio = Convert.ToDateTime(desde);
            DateTime termino = Convert.ToDateTime(hasta);
            DateTime partida = Convert.ToDateTime(inicio);
            foreach (Detperiodos d in periodos)
            {
                if (dias > 0)
                {
                    Detperiodos proc = new Detperiodos();
                    proc.ruttrabajador = d.ruttrabajador;
                    proc.anoinicio = d.anoinicio;
                    proc.anotermino = d.anotermino;
                    proc.fechainicio = partida.ToString("yyyy'-'MM'-'dd");
                    if (dias >= d.diaslegales)
                    {   proc.diaslegales = d.diaslegales;
                        dias = dias - d.diaslegales;
                    }
                    else
                    {   proc.diaslegales = (int)dias;
                        dias = 0;
                    }
                    if (dias >= d.diasprogresivos)
                    {   proc.diasprogresivos = d.diasprogresivos;
                        dias = dias - d.diasprogresivos;
                    }
                    else
                    {   proc.diasprogresivos = (int)dias;
                        dias = 0;
                    }
                    if (dias >= d.diascontrato)
                    {   proc.diascontrato = d.diascontrato;
                        dias = dias - d.diascontrato;
                    }
                    else
                    {   proc.diascontrato = (int)dias;
                        dias = 0;
                    }
                    if (dias >= d.diasfaena)
                    {   proc.diasfaena = d.diasfaena;
                        dias = dias - d.diasfaena;
                    }
                    else
                    {   proc.diasfaena = (int)dias;
                        dias = 0;
                    }
                    if (dias >= d.diasespeciales)
                    {   proc.diasespeciales = d.diasespeciales;
                        dias = dias - d.diasespeciales;
                    }
                    else
                    {   proc.diasespeciales = (int)dias;
                        dias = 0;
                    }
                    proc.periodo = d.anoinicio.ToString("####") + "-" + d.anotermino.ToString("####");
                    proc.diastotal = proc.diaslegales + proc.diasprogresivos + proc.diascontrato + proc.diasadministrativos + proc.diasfaena + proc.diasespeciales + proc.diasotros;
                    DateTime fechacorrido = FechaHasta(partida, proc.diastotal,termino);
                    proc.fechatermino = fechacorrido.ToString("yyyy'-'MM'-'dd");
                    partida = fechacorrido.AddDays(1);
                    ausar.Add(proc);
                }
            }
            return ausar;
        }
        public DateTime FechaHasta(DateTime desde, int dias,DateTime termino)
        {
            DateTime fecha = desde.AddDays(-1);
            int ind = 1;
            while (ind <= dias)
            {
                fecha = fecha.AddDays(1);
                if (fecha.DayOfWeek == DayOfWeek.Sunday || fecha.DayOfWeek == DayOfWeek.Saturday)
                {
                    fecha = fecha.AddDays(1);continue;
                }
                if (Generales.EsFeriado(fecha, 1) == true)
                {
                    fecha = fecha.AddDays(1);continue;
                }
                ind++;
            }
            if (fecha != termino)
            {
                fecha=fecha.AddDays(-1);
            }
            return fecha;
        }

        public double DiasHabiles(string desde,string hasta)
        {
            double hab = 0;
            DateTime des = Convert.ToDateTime(desde);
            DateTime has = Convert.ToDateTime(hasta);
            while (des <= has)
            {
                if (des.DayOfWeek == DayOfWeek.Sunday || des.DayOfWeek == DayOfWeek.Saturday)
                {
                    des = des.AddDays(1); continue;
                }
                if (Generales.EsFeriado(des, 1) == true)
                {
                    des = des.AddDays(1); continue;
                }
                hab++;
                des = des.AddDays(1);   
            }
            return hab;
        }
        public List<Detperiodos> Periodos(int empresa, string rut)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                GeneraPeriodos(rut, BD_Cli);

                f.EjecutarConsultaSQLCli("SELECT vacacionesPeriodos.rutTrabajador, vacacionesPeriodos.anoInicio, vacacionesPeriodos.anoTermino, vacacionesPeriodos.diasLegales " +
                                            ", vacacionesPeriodos.diasProgresivos, vacacionesPeriodos.diasContrato, vacacionesPeriodos.diasAdministrativos " +
                                            ", vacacionesPeriodos.diasFaena, vacacionesPeriodos.diasEspeciales, vacacionesPeriodos.diasOtros " +
                                            "FROM vacacionesPeriodos " +
                                            "WHERE  vacacionesPeriodos.rutTrabajador = '" + rut + "' Order by anoInicio ", BD_Cli);


                List<Detperiodos> Lista = new List<Detperiodos>();
                Lista = (from DataRow dr in f.Tabla.Rows
                         select new Detperiodos()
                         {
                             ruttrabajador = dr["rutTrabajador"].ToString(),
                             anoinicio = int.Parse(dr["anoInicio"].ToString()),
                             anotermino = int.Parse(dr["anoTermino"].ToString()),
                             diaslegales = int.Parse(dr["diasLegales"].ToString()),
                             diasprogresivos = int.Parse(dr["diasProgresivos"].ToString()),
                             diascontrato = int.Parse(dr["diasContrato"].ToString()),
                             diasadministrativos = int.Parse(dr["diasAdministrativos"].ToString()),
                             diasfaena = int.Parse(dr["diasFaena"].ToString()),
                             diasespeciales = int.Parse(dr["diasEspeciales"].ToString()),
                             diasotros = int.Parse(dr["diasOtros"].ToString())
                         }).ToList();
                List<Detperiodos> saldo = new List<Detperiodos>();
                if(Lista.Count > 0)
                {
                    foreach (var l in Lista)
                    {
                        l.periodo = l.anoinicio.ToString("####") + "-" + l.anotermino.ToString("####");
                        l.diastotal = l.diaslegales + l.diasprogresivos + l.diascontrato + l.diasfaena + l.diasespeciales + l.diasotros;
                        Detperiodos usados = UsadosPeriodo(rut, l.anoinicio, BD_Cli);
                        l.diaslegales = l.diaslegales - usados.diaslegales;
                        l.diasprogresivos = l.diasprogresivos - usados.diasprogresivos;
                        l.diascontrato = l.diascontrato - usados.diascontrato;
                        l.diasadministrativos = l.diasadministrativos - usados.diasadministrativos;
                        l.diasfaena = l.diasfaena - usados.diasfaena;
                        l.diasespeciales = l.diasespeciales - usados.diasespeciales;
                        l.diasotros = l.diasotros - usados.diasotros;
                        l.diastotal = l.diastotal - usados.diastotal;
                        if (l.diastotal > 0)
                        {
                            saldo.Add(l);
                        }
                    }
                }
                return saldo;
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error al generar solicitud de vacaciones";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al generar solicitud de vacaciones", correos.destinatarioErrores);
                return null;
            }
        }
        public Detperiodos UsadosPeriodo(string rut, int ano, string BD_Cli)
        {
            Detperiodos peri = new Detperiodos();
            
            f.EjecutarConsultaSQLCli("select * from vacacionesUsos where rutTrabajador='" + rut + "' and anoInicio = " + ano, BD_Cli);
            List<Detperiodos> Lista = new List<Detperiodos>();
            Lista = (from DataRow dr in f.Tabla.Rows
                     select new Detperiodos()
                     {
                         ruttrabajador = dr["rutTrabajador"].ToString(),
                         anoinicio = int.Parse(dr["anoInicio"].ToString()),
                         anotermino = int.Parse(dr["anoTermino"].ToString()),
                         diaslegales = int.Parse(dr["diasLegales"].ToString()),
                         diasprogresivos = int.Parse(dr["diasProgresivos"].ToString()),
                         diascontrato = int.Parse(dr["diasContrato"].ToString()),
                         diasadministrativos = int.Parse(dr["diasAdministrativos"].ToString()),
                         diasfaena = int.Parse(dr["diasFaena"].ToString()),
                         diasespeciales = int.Parse(dr["diasEspeciales"].ToString()),
                         diasotros = int.Parse(dr["diasOtros"].ToString())
                     }).ToList();
            foreach(Detperiodos d in Lista)
            {
                peri.diaslegales = peri.diaslegales + d.diaslegales;
                peri.diasprogresivos = peri.diasprogresivos + d.diasprogresivos;
                peri.diascontrato = peri.diascontrato + d.diascontrato;
                peri.diasadministrativos = peri.diasadministrativos + d.diasadministrativos;
                peri.diasfaena = peri.diasfaena + d.diasfaena;
                peri.diasespeciales = peri.diasespeciales + d.diasespeciales;
                peri.diasotros = peri.diasotros + d.diasotros;
            }
            peri.diastotal = peri.diaslegales+peri.diasprogresivos+peri.diascontrato+peri.diasfaena+peri.diasespeciales+peri.diasotros;
            return peri;
        }
        public void GeneraPeriodos(string rut, string BD_Cli)
        {
            string hoystr = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            ContratosBaseVM con = Generales.BuscaContrato(rut,hoystr, BD_Cli);
            List<Detperiodos> periodos = new List<Detperiodos>();  
            Detdias dias = Generales.BuscaDias(1,BD_Cli);
            if (con != null)
            {
                DateTime fecing = Convert.ToDateTime(con.inicio);
                DateTime hoy = DateTime.Now.Date.AddYears(-1);
                DateTime fecproc = fecing;
                while (fecproc < hoy)
                {
                    f.EjecutarConsultaSQLCli("select * from vacacionesPeriodos where rutTrabajador='" + rut + "' and anoInicio = "+ fecproc.Year , BD_Cli);
                    if (f.Tabla.Rows.Count == 0)
                    {
                        Detperiodos peri = new Detperiodos();
                        peri.ruttrabajador = rut;
                        peri.anoinicio = fecproc.Year;
                        peri.anotermino = fecproc.Year + 1;
                        peri.diaslegales = dias.diaslegales;
                        peri.diascontrato = dias.diascontrato;
                        peri.diasadministrativos = dias.diasadministrativos;
                        peri.diasfaena = dias.diasfaena;
                        peri.diasespeciales = dias.diasespeciales;
                        peri.diasotros = dias.diasotros;
                        peri.diasprogresivos = Progresivos(fecing,fecproc.Year);
                        string query2 = "insert into vacacionesPeriodos (rutTrabajador,anoInicio,anoTermino,diasLegales,diasProgresivos,diasContrato"+
                                        ", diasAdministrativos,diasFaena,diasEspeciales, diasOtros,fechaProceso,habilitado) " +
                                        "values " +
                                        "('" + peri.ruttrabajador + "'," + peri.anoinicio + "," + peri.anotermino + "," + peri.diaslegales + "," + peri.diasprogresivos +
                                        ", " + peri.diascontrato + "," + peri.diasadministrativos + "," + peri.diasfaena+"," + peri.diasespeciales +
                                        ", " + peri.diasotros  + ", '" +hoy +"' ,1) ! ";
                        f.EjecutarQuerySQLCli(query2, BD_Cli);
                    }
                    fecproc = fecproc.AddYears(1);
                }
            }
            return;
        }
        public int Progresivos(DateTime fecing, int anopro)
        {
            int prog = 0;
            int anoing = fecing.Year;

            int difer = anopro - anoing;
            if (difer >= 13)
            {
                if (difer >= 16)
                {
                    if (difer >= 19)
                    {
                        if (difer >= 21)
                        {
                            if (difer >= 24)
                            {
                                if (difer >= 27)
                                {
                                    if (difer >= 30)
                                    { prog = 7; }
                                    else { prog = 6; }
                                }
                                else { prog = 5; }
                            }
                            else { prog = 4; }
                        }
                        else { prog = 3; }
                    }
                    else { prog = 2; }
                }
                else { prog = 1; }
            }
            return prog;
        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class Detperiodos
    {
        public int id { get; set; }
        public string ruttrabajador { get; set; }
        public string periodo { get; set; }
        public int anoinicio { get; set; }
        public int anotermino { get; set; }
        public string fechainicio { get; set; }
        public string fechatermino { get; set; }
        public int diaslegales { get; set; }
        public int diasprogresivos { get; set; }
        public int diascontrato { get; set; }
        public int diasadministrativos { get; set; }
        public int diasfaena { get; set; }
        public int diasespeciales { get; set; }
        public int diasotros { get; set; }
        public int diastotal { get; set; }
        public int diascorridos { get; set; }
    }
    public class Detdias
    {
        public int id { get; set; }
        public string horario { get; set; }
        public int diaslegales { get; set; }
        public int diasprogresivos { get; set; }
        public int diascontrato { get; set; }
        public int diasadministrativos { get; set; }
        public int diasfaena { get; set; }
        public int diasespeciales { get; set; }
        public int diasotros { get; set; }
    }


}

