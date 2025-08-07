using MongoDB.Driver.Linq;
using MongoDB.Driver.Search;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.CompensacionSolicitud
{
    /// <summary>
    /// Servicio para gestionar vavaciones.
    /// </summary>
    public interface ICompensacionSolicitudService
    {
        /// <summary>
        /// Genera solicitud de compensacion.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="dias">dias a compensar</param>
        /// <returns>Resultado</returns>
        public Resultado MostrarCompensacionSolicitudService(int idEmpresa, int dias, string rut);

        /// <summary>
        /// Procesa solicitud de compensacion.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="dias">dias a compensar</param>
        /// <returns>Resultado</returns>
        public Resultado ProcesarCompensacionSolicitudService(int idEmpresa, int dias, string rut);
 
        /// <summary>
        /// Verifica la validez de la solicitud.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="dias">dias a compensar</param>
        /// <returns>Resultado</returns>
        public Resultado VerificaCompensacionSolicitudService(int idEmpresa, int dias, string rut);

    }

    public class CompensacionSolicitudService : ICompensacionSolicitudService
    {
        private readonly IDatabaseManager _databaseManager;
        private static Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Generales Generales = new Generales();
        private Seguridad seguridad = new Seguridad();

        public CompensacionSolicitudService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }
        public Resultado MostrarCompensacionSolicitudService(int empresa,  int dias, string rut)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "Puede pedir compensacion";
            List<Detperiodos> ListaPer= Periodos(empresa,rut);
            DateTime inicio = DiaInicial();
            string desde =  inicio.ToString();
            DateTime termino = DiaTermino(inicio,dias);
            string hasta = termino.ToString("yyyy'-'MM'-'dd");
            Double diascorridos = (termino - inicio).TotalDays+1;
            Double diashabiles = 3;
            List<Detperiodos> sugerencia = BuscaPeriodosUsar(ListaPer, diashabiles,desde, hasta,diascorridos);
            if (sugerencia.Count > 0) 
            {
                resultado.result = 1;
                resultado.mensaje = "Existen compensacion";
            }
            resultado.data = sugerencia;
            return resultado;
        }
        public Resultado ProcesarCompensacionSolicitudService(int empresa, int dias, string rut)
       {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            DateTime inicio = DiaInicial();
            string desde = inicio.ToString();
            DateTime termino = DiaTermino(inicio, dias);
            string hasta = termino.ToString("yyyy'-'MM'-'dd");
            Double diascorridos = (termino - inicio).TotalDays + 1;
            Double diashabiles = DiasHabiles(desde, hasta);
            DateTime hoy = DateTime.Now;
            string desstr = inicio.ToString("yyyy'-'MM'-'dd");
            string hoystr = hoy.ToString("yyyy'-'MM'-'dd' 'hh':'mm");
            string query2 = "insert into compensacionSolicitud (rutTrabajador, numeroSolicitud,fechaSolicitud, estadoSolicitud, fechaInicio, diasSolicitados,diasCorridos ,habilitado) " +
           "values " +
           "('" + rut + "'," + 0 + ",'" + hoystr + "','Solicitud', '" +   desstr  + "', " + dias + ", "+diascorridos+" ,1)  ";
            if (f.EjecutarQuerySQLCli(query2, BD_Cli))
            {
                resultado.result = 1;
                resultado.mensaje = "Se grabo la solicitud de compensacion.";
            }
            return resultado;
        }
        public Resultado VerificaCompensacionSolicitudService(int empresa, int dias, string rut)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            DateTime desde = DiaInicial();
            DateTime desdedate = Convert.ToDateTime(desde);
            DateTime inicio = Convert.ToDateTime(desde);
            DateTime termino = inicio.AddDays(dias);
            DateTime hastadate = termino;
            string hasta = termino.ToString("yyyy'-'MM'-'dd");
            if (dias < 3)
            {
                resultado.mensaje = "Dias a compensar, menor que mínimo";
                return resultado;
            }

            resultado.result = 1;
            resultado.mensaje = "Existen compensacion";
            return resultado;
        }

        public DateTime DiaInicial()
        {
            DateTime dia = DateTime.Now;
            DateTime sem = dia.AddDays(7);
            while(dia < sem)
            {
                string nombre = dia.DayOfWeek.ToString();
                if (nombre == "Monday") return dia;
                dia = dia.AddDays(1);
            }
            return dia;
        }
        public DateTime DiaTermino(DateTime fecha, int dias)
        {
            DateTime proc= fecha.AddDays(-1);
            int ind=1;
            while(ind <= dias)
            {
                proc = proc.AddDays(1);
                if (EsDiaHabil(proc))
                {
                    proc = proc.AddDays(1);
                }
                ind++;
            }
            return proc;
        }
        public bool EsDiaHabil(DateTime proc)
        {
            if (proc.DayOfWeek == DayOfWeek.Sunday || proc.DayOfWeek == DayOfWeek.Saturday)
            {
                proc = proc.AddDays(1);
                return true;
            }
            else
            {
                if (Generales.EsFeriado(proc, 1) == true)
                {
                    proc = proc.AddDays(1);
                    return true;
                }
            }
            return false;
        }
        public List<Detperiodos> BuscaPeriodosUsar(List<Detperiodos> periodos,double dias,string desde,string hasta,double diascorridos)
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
                    proc.diastotal = proc.diaslegales + proc.diasprogresivos + proc.diascontrato + proc.diasfaena + proc.diasespeciales + proc.diasotros;
                    DateTime fechacorrido = FechaHasta(partida, proc.diastotal,termino);
                    proc.fechatermino = fechacorrido.ToString("yyyy'-'MM'-'dd");
                    partida = fechacorrido.AddDays(1);
                    proc.diascorridos = (int)diascorridos;
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
                if (EsDiaHabil(fecha))
                {
                    fecha = fecha.AddDays(1);
                }
                ind++;
            }
            return fecha;
        }

        public double DiasHabiles(string desde,string hasta)
        {
            double hab = 0;
            DateTime des = Convert.ToDateTime(desde).AddDays(-1);
            DateTime has = Convert.ToDateTime(hasta);
            while (des <= has)
            {
                des = des.AddDays(1);
                if (EsDiaHabil(des))
                {
                    des = des.AddDays(1);
                }
                hab++;
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
                var Asunto = "Error al generar solicitud de compensacion";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al generar solicitud de compensacion", correos.destinatarioErrores);
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

public class DetPeriodosComp
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
}
