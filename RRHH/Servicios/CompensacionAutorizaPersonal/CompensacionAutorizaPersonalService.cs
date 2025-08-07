using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.CompensacionAutorizaPersonal
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface ICompensacionAutorizaPersonalService
    {
        /// <summary>
        /// Genera listado desolicitud de compensacion.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista solicitud de compensación pendientes</returns>
        public Resultado ListarCompensacionAutorizaPersonalService(int idEmpresa, string desde,string hasta);

        /// <summary>
        /// Autoriza compensacion.
        /// </summary>
        /// <param name="ids">ID de los compensacion autorizados</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="idusuario">ID de la persona que aprueba.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista compensacion</returns>
        public Resultado AutorizaCompensacionService(string ids, int idempresa,string idusuario, string desde, string hasta);
 
        /// <summary>
        /// Rechaza compensacion
        /// </summary>
        /// <param name="ids">ID de los compensacion autorizados</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="idusuario">ID de la persona que aprueba.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista compensacion</returns>
        public Resultado RechazaCompensacionService(string ids, int idempresa, string idusuario, string desde, string hasta);

    }

    public class CompensacionAutorizaPersonalService : ICompensacionAutorizaPersonalService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public CompensacionAutorizaPersonalService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public Resultado ListarCompensacionAutorizaPersonalService(int empresa,  string desde,string hasta)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen compensaciones que autorizar.";
            resultado.result = 0;
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                DateTime fecfin = Convert.ToDateTime(hasta).AddDays(1);
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");
                f.EjecutarConsultaSQLCli("SELECT compensacionSolicitud.id,compensacionSolicitud.rutTrabajador " +
                                            ",compensacionSolicitud.fechaInicio,compensacionSolicitud.diasSolicitados, compensacionSolicitud.diasCorridos " +
                                            "FROM compensacionSolicitud " +
                                            "WHERE compensacionSolicitud.habilitado = 1 and compensacionSolicitud.estadoSolicitud = 'Solicitud'" +
                                            " and compensacionSolicitud.fechainicio <= '" + fecfinstr+ "' and compensacionSolicitud.fechainicio >= '" + desde+"' ", BD_Cli);
                List<DetAutorizaCompensacion> opcionesList = new List<DetAutorizaCompensacion>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetAutorizaCompensacion()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        ruttrabajador = dr["rutTrabajador"].ToString(),
                                        fechainicio = dr["fechainicio"].ToString(),
                                        diassolicitados = int.Parse(dr["diasSolicitados"].ToString()),
                                        diascorridos = int.Parse(dr["diasCorridos"].ToString()),
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        DateTime inicio = Convert.ToDateTime(r.fechainicio);
                        r.fechainicio = inicio.ToString("dd'-'MM'-'yyyy");
                        PersonasBaseVM pers = Generales.BuscaPersona(r.ruttrabajador, BD_Cli);
                        r.nombre = pers.Nombres + " " + pers.Apellidos;
                    }
                    resultado.mensaje = "Si existen compensaciones que autorizar.";
                    resultado.result = 1;
                    resultado.data = opcionesList;
                }
                return resultado;
            }
            catch (Exception ex)
            {
                var Asunto = "Error en listar autorizacion";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar autorizacion", correos.destinatarioErrores);
                return resultado;
            }
        }
        public Resultado AutorizaCompensacionService(string ids, int empresa, string idusuario, string desde, string hasta)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen compensaciones que autorizar.";
            resultado.result = 0;
            if (ids != null)
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                string[] idsSeparados = ids.Split("*");
                int nsel = idsSeparados.Length - 1;
                DateTime hoy = DateTime.Now;
                string hoystr = hoy.ToString("yyyy'-'MM'-'dd' 'hh':'mm");
                try
                {
                    int ind;
                    for (ind = 0; ind < nsel; ind++)
                    {
                        if (idsSeparados[ind] != null)
                        {
                            int idsolicitud = Convert.ToInt32(idsSeparados[ind]);
                            string query = "update compensacionSolicitud set estadoSolicitud='Registrada', fechaAprobacion='" + hoystr + "', idUsuario= " + idusuario +
                                " where id=" + idsolicitud;

                            if (f.EjecutarQuerySQLCli(query, BD_Cli))
                            {
                                f.EjecutarConsultaSQLCli("SELECT compensacionSolicitud.id,compensacionSolicitud.rutTrabajador " +
                                                           ",compensacionSolicitud.fechaInicio,compensacionSolicitud.diasSolicitados " +
                                                           "FROM compensacionSolicitud " +
                                                           "WHERE compensacionSolicitud.id = " + idsolicitud, BD_Cli);
                                List<DetPeriodosComp> opcionesList = new List<DetPeriodosComp>();
                                if (f.Tabla.Rows.Count > 0)
                                {
                                    List<DetPeriodosComp> peri = new List<DetPeriodosComp>();
                                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                                    select new DetPeriodosComp()
                                                    {
                                                        id = int.Parse(dr["id"].ToString()),
                                                        ruttrabajador = dr["rutTrabajador"].ToString(),
                                                        fechainicio = dr["fechainicio"].ToString(),
                                                        diastotal = int.Parse(dr["diasSolicitados"].ToString())
                                                    }).ToList();
                                    foreach (var r in opcionesList)
                                    {
                                        DateTime inicio = Convert.ToDateTime(r.fechainicio);
                                        r.fechainicio = inicio.ToString("yyyy'-'MM'-'dd");
                                        DateTime termino = inicio.AddDays(r.diastotal);
                                        r.fechatermino = termino.ToString("yyyy'-'MM'-'dd");
                                        peri = ConsultarSolicitudCompensacion(empresa, r.diastotal, r.ruttrabajador,inicio);
                                        if (peri.Count > 0)
                                        {
                                            foreach (var s in peri)
                                            {
                                                Double diascorridos = (termino - inicio).TotalDays + 1;
                                                string query2 = "insert into vacacionesUsos (rutTrabajador,anoInicio,anoTermino,tipoUso,fechaInicio,fechaTermino" +
                                                                ", numeroSolicitud,diasCorridos,diasLegales,diasProgresivos,diasContrato,diasAdministrativos " +
                                                                ",diasFaena,diasEspeciales, diasOtros,fechaProceso,habilitado) " +
                                                                "values " +
                                                                "('" + s.ruttrabajador + "'," + s.anoinicio + ", " + s.anotermino + ",'C', '" + s.fechainicio +
                                                                "', '" + s.fechatermino + "', " + idsolicitud + ", " + diascorridos +
                                                                ", " + s.diaslegales + "," + s.diasprogresivos +
                                                                ", " + s.diascontrato + "," + s.diasadministrativos + "," + s.diasfaena + "," + s.diasespeciales +
                                                                ", " + s.diasotros + ", '" + hoystr + "' ,1) ";
                                                f.EjecutarQuerySQLCli(query2, BD_Cli);
                                            }
                                        }
                                    }
                                    resultado.mensaje = "Compensaciones registradas exitosamente";
                                    resultado.result = 1;
                                }
                            }
                        }
                    }
                }
                catch (Exception eG)
                {
                    var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();
                    resultado.result = -1;
                    resultado.mensaje = "Fallo al intentar actualizar autorizacion" + eG.Message.ToString();
                }
            }
            return resultado;
        }
        public List<DetPeriodosComp> ConsultarSolicitudCompensacion(int empresa, int dias, string rut, DateTime inicio)
        {
            List<Detperiodos> ListaPer = Periodos(empresa, rut);
            string desde = inicio.ToString();
            DateTime termino = DiaTermino(inicio, dias);
            string hasta = termino.ToString("yyyy'-'MM'-'dd");
            Double diascorridos = (termino - inicio).TotalDays + 1;
            List<DetPeriodosComp> sugerencia = BuscaPeriodosUsar(ListaPer, dias, desde, hasta, diascorridos);
            return sugerencia;
        }
        public Resultado RechazaCompensacionService(string ids, int empresa, string idusuario, string desde, string hasta)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen compensaciones que autorizar.";
            resultado.result = 0;
            if (ids != null)
            {
                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                string[] idsSeparados = ids.Split("*");
                int nsel = idsSeparados.Length - 1;
                DateTime hoy = DateTime.Now;
                string hoystr = hoy.ToString("yyyy'-'MM'-'dd' 'hh':'mm");
                try
                {
                    int ind;
                    for (ind = 0; ind < nsel; ind++)
                    {
                        if (idsSeparados[ind] != null)
                        {
                            int id = Convert.ToInt32(idsSeparados[ind]);
                            string query = "update compensacionSolicitud set estadoSolicitud='Rechaza', fechaApruebaJefe='" + hoystr + "', idUsuarioJefe= " + idusuario +
                                " where id=" + id;

                            if (f.EjecutarQuerySQLCli(query, BD_Cli))
                            {
                                resultado.result = 1;
                                resultado.mensaje = "Registro actualizado de manera exitosa.";
                            }
                        }
                    }
                }
                catch (Exception eG)
                {
                    var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();
                    resultado.result = -1;
                    resultado.mensaje = "Fallo al intentar actualizar autorizacion" + eG.Message.ToString();
                }
            }
            return resultado;
        }
        public List<DetPeriodosComp> BuscaPeriodosUsar(List<Detperiodos> periodos, double dias, string desde, string hasta, double diascorridos)
        {
            List<DetPeriodosComp> ausar = new List<DetPeriodosComp>();
            DateTime inicio = Convert.ToDateTime(desde);
            DateTime termino = Convert.ToDateTime(hasta);
            DateTime partida = Convert.ToDateTime(inicio);
            foreach (Detperiodos d in periodos)
            {
                if (dias > 0)
                {
                    DetPeriodosComp proc = new DetPeriodosComp();
                    proc.ruttrabajador = d.ruttrabajador;
                    proc.anoinicio = d.anoinicio;
                    proc.anotermino = d.anotermino;
                    proc.fechainicio = partida.ToString("yyyy'-'MM'-'dd");
                    if (dias >= d.diaslegales)
                    {
                        proc.diaslegales = d.diaslegales;
                        dias = dias - d.diaslegales;
                    }
                    else
                    {
                        proc.diaslegales = (int)dias;
                        dias = 0;
                    }
                    if (dias >= d.diasprogresivos)
                    {
                        proc.diasprogresivos = d.diasprogresivos;
                        dias = dias - d.diasprogresivos;
                    }
                    else
                    {
                        proc.diasprogresivos = (int)dias;
                        dias = 0;
                    }
                    if (dias >= d.diascontrato)
                    {
                        proc.diascontrato = d.diascontrato;
                        dias = dias - d.diascontrato;
                    }
                    else
                    {
                        proc.diascontrato = (int)dias;
                        dias = 0;
                    }
                    if (dias >= d.diasfaena)
                    {
                        proc.diasfaena = d.diasfaena;
                        dias = dias - d.diasfaena;
                    }
                    else
                    {
                        proc.diasfaena = (int)dias;
                        dias = 0;
                    }
                    if (dias >= d.diasespeciales)
                    {
                        proc.diasespeciales = d.diasespeciales;
                        dias = dias - d.diasespeciales;
                    }
                    else
                    {
                        proc.diasespeciales = (int)dias;
                        dias = 0;
                    }
                    proc.periodo = d.anoinicio.ToString("####") + "-" + d.anotermino.ToString("####");
                    proc.diastotal = proc.diaslegales + proc.diasprogresivos + proc.diascontrato + proc.diasfaena + proc.diasespeciales + proc.diasotros;
                    DateTime fechacorrido = FechaHasta(partida, proc.diastotal, termino);
                    proc.fechatermino = fechacorrido.ToString("yyyy'-'MM'-'dd");
                    partida = fechacorrido.AddDays(1);
                    ausar.Add(proc);
                }
            }
            return ausar;
        }
        public DateTime DiaInicial()
        {
            DateTime dia = DateTime.Now;
            DateTime sem = dia.AddDays(7);
            while (dia < sem)
            {
                string nombre = dia.DayOfWeek.ToString();
                if (nombre == "Monday") return dia;
                dia = dia.AddDays(1);
            }
            return dia;
        }
        public DateTime DiaTermino(DateTime fecha, int dias)
        {
            DateTime proc = fecha.AddDays(-1);
            int ind = 1;
            while (ind <= dias)
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
        public DateTime FechaHasta(DateTime desde, int dias, DateTime termino)
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

        public double DiasHabiles(string desde, string hasta)
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
                if (Lista.Count > 0)
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
            foreach (Detperiodos d in Lista)
            {
                peri.diaslegales = peri.diaslegales + d.diaslegales;
                peri.diasprogresivos = peri.diasprogresivos + d.diasprogresivos;
                peri.diascontrato = peri.diascontrato + d.diascontrato;
                peri.diasadministrativos = peri.diasadministrativos + d.diasadministrativos;
                peri.diasfaena = peri.diasfaena + d.diasfaena;
                peri.diasespeciales = peri.diasespeciales + d.diasespeciales;
                peri.diasotros = peri.diasotros + d.diasotros;
            }
            peri.diastotal = peri.diaslegales + peri.diasprogresivos + peri.diascontrato + peri.diasfaena + peri.diasespeciales + peri.diasotros;
            return peri;
        }


    }
}

public class DetAutorizaCompensacion
{
    public int id { get; set; }
    public string ruttrabajador { get; set; }
    public string nombre { get; set; }
    public string fechainicio { get; set; }
    public int diassolicitados { get; set; }
    public int diascorridos { get; set; }
}
