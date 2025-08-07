using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using RRHH.Servicios.SolicitudVacaciones;
using System.Collections.Generic;
using System.Data;

namespace RRHH.Servicios.VacacionesAutorizaPersonal
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IVacacionesAutorizaPersonalService
    {
        /// <summary>
        /// Genera listado de vacaciones.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista vacaciones</returns>
        public Resultado ListarVacacionesAutorizaPersonalService(int idEmpresa, string desde, string hasta);

        /// <summary>
        /// Autoriza permosos.
        /// </summary>
        /// <param name="ids">ID de los vacaciones autorizados</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="idusuario">ID de la persona que aprueba.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista vacaciones</returns>
        public Resultado AutorizaVacacionesService(string ids, int idempresa, string idusuario, string desde, string hasta);

        /// <summary>
        /// Rechaza vacaciones
        /// </summary>
        /// <param name="ids">ID de los vacaciones autorizados</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="idusuario">ID de la persona que aprueba.</param>
        /// <param name="desde">Fecha inicial de consulta</param>
        /// <param name="hasta">Fecha final de consulta</param>
        /// <returns>Lista vacaciones</returns>
        public Resultado RechazaVacacionesService(string ids, int idempresa, string idusuario, string desde, string hasta);

    }

    public class VacacionesAutorizaPersonalService : IVacacionesAutorizaPersonalService
    {
        private readonly IDatabaseManager _databaseManager;
        private Funciones f = new Funciones();
        private Correos correos = new Correos();
        private Generales Generales = new Generales();
        private Seguridad seguridad = new Seguridad();
        private readonly ISolicitudVacacionesService _solicitudvacacionesService;

        public VacacionesAutorizaPersonalService(IDatabaseManager databaseManager, ISolicitudVacacionesService solicitudvacacionesService)
        {
            _databaseManager = databaseManager;
            _solicitudvacacionesService = solicitudvacacionesService;
        }

        public Resultado ListarVacacionesAutorizaPersonalService(int empresa, string desde, string hasta)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existen registros";
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                DateTime fecfin = Convert.ToDateTime(hasta).AddDays(1);
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");
                f.EjecutarConsultaSQLCli("SELECT vacacionesSolicitud.id,vacacionesSolicitud.rutTrabajador " +
                                            ",vacacionesSolicitud.fechaInicio,vacacionesSolicitud.fechaTermino " +
                                            "FROM vacacionesSolicitud " +
                                            "WHERE vacacionesSolicitud.habilitado = 1 and vacacionesSolicitud.estadoSolicitud = 'Aprobada'" +
                                            " and vacacionesSolicitud.fechainicio <= '" + fecfinstr + "' and vacacionesSolicitud.fechatermino >= '" + desde + "' ", BD_Cli);
                List<DetAutorizaPersonal> opcionesList = new List<DetAutorizaPersonal>();
                if (f.Tabla.Rows.Count > 0)
                {
                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetAutorizaPersonal()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        ruttrabajador = dr["rutTrabajador"].ToString(),
                                        fechainicio = dr["fechainicio"].ToString(),
                                        fechatermino = dr["fechatermino"].ToString()
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        DateTime inicio = Convert.ToDateTime(r.fechainicio);
                        r.fechainicio = inicio.ToString("dd'-'MM'-'yyyy");
                        DateTime termino = Convert.ToDateTime(r.fechatermino);
                        r.fechatermino = termino.ToString("dd'-'MM'-'yyyy");
                        PersonasBaseVM pers = Generales.BuscaPersona(r.ruttrabajador, BD_Cli);
                        r.nombre = pers.Nombres + " " + pers.Apellidos;
                        r.diastotal = (int)termino.Subtract(inicio).TotalDays + 1;
                    }
                    resultado.result = 1;
                    resultado.mensaje = "Existen registros";
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
        public Resultado AutorizaVacacionesService(string ids, int empresa, string idusuario, string desde, string hasta)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No existen vacaciones por registrar";
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
                            string query = "update vacacionesSolicitud set estadoSolicitud='Registrada', fechaApruebaPers='" + hoystr + "', idUsuarioPersonal= " + idusuario +
                                " where id=" + idsolicitud;

                            if (f.EjecutarQuerySQLCli(query, BD_Cli))
                            {
                                f.EjecutarConsultaSQLCli("SELECT vacacionesSolicitud.id,vacacionesSolicitud.rutTrabajador " +
                                                           ",vacacionesSolicitud.fechaInicio,vacacionesSolicitud.fechaTermino " +
                                                           "FROM vacacionesSolicitud " +
                                                           "WHERE vacacionesSolicitud.id = " + idsolicitud, BD_Cli);
                                List<DetAutorizaPersonal> opcionesList = new List<DetAutorizaPersonal>();
                                if (f.Tabla.Rows.Count > 0)
                                {
                                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                                    select new DetAutorizaPersonal()
                                                    {
                                                        id = int.Parse(dr["id"].ToString()),
                                                        ruttrabajador = dr["rutTrabajador"].ToString(),
                                                        fechainicio = dr["fechainicio"].ToString(),
                                                        fechatermino = dr["fechatermino"].ToString()
                                                    }).ToList();
                                    foreach (var r in opcionesList)
                                    {
                                        DateTime inicio = Convert.ToDateTime(r.fechainicio);
                                        r.fechainicio = inicio.ToString("yyyy'-'MM'-'dd");
                                        DateTime termino = Convert.ToDateTime(r.fechatermino);
                                        r.fechatermino = termino.ToString("yyyy'-'MM'-'dd");
                                        r.diastotal = (int)termino.Subtract(inicio).TotalDays + 1;
                                        resultado = _solicitudvacacionesService.MostrarSolicitudVacacionesService(empresa, r.fechainicio, r.fechatermino, r.ruttrabajador);
                                        if (resultado.result==1)
                                        {
                                            List<Detperiodos> sugerencia =(List<Detperiodos>) resultado.data;
                                            foreach (var s in sugerencia)
                                            {
                                                string query2 = "insert into vacacionesUsos (rutTrabajador,anoInicio,anoTermino,tipoUso,fechaInicio,fechaTermino" +
                                                                ", numeroSolicitud,diasCorridos,diasLegales,diasProgresivos,diasContrato,diasAdministrativos " +
                                                                ",diasFaena,diasEspeciales, diasOtros,fechaProceso,habilitado) " +
                                                                "values " +
                                                                "('" + s.ruttrabajador + "'," + s.anoinicio + ", " + s.anotermino + ",'V', '"+ s.fechainicio + 
                                                                "', '"+ s.fechatermino + "', "+idsolicitud+", "+s.diastotal +
                                                                ", "+s.diaslegales + "," + s.diasprogresivos +
                                                                ", " + s.diascontrato + "," + s.diasadministrativos + "," + s.diasfaena + "," + s.diasespeciales +
                                                                ", " + s.diasotros + ", '" + hoystr + "' ,1) ";
                                                f.EjecutarQuerySQLCli(query2, BD_Cli);
                                            }
                                            resultado.mensaje = "Vacaciones registradas exitosamente";
                                            resultado.result = 1;
                                        }
                                        else
                                        {
                                            resultado.mensaje = "Vacaciones no se generan";
                                            resultado.result = 0;
                                        }
                                    }
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
        public Resultado RechazaVacacionesService(string ids, int empresa, string idusuario, string desde, string hasta)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "No existe registro";
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
                        string query = "update vacacionesSolicitud set estadoSolicitud='Rechaza', fechaApruebaPersonal='" + hoystr + "', idUsuarioPersonal= " + idusuario +
                            " where id=" + id ;

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
            return resultado;
        }
        //public List<Detperiodos>  ConsultarSolicitudVacaciones(int empresa, string desde, string hasta, string rut)
        //{
        //    List<Detperiodos> ListaPer = Periodos(empresa, rut);
        //    Double diascorridos = (Convert.ToDateTime(hasta).Date - Convert.ToDateTime(desde).Date).TotalDays + 1;
        //    Double diashabiles = DiasHabiles(desde, hasta);
        //    List<Detperiodos> sugerencia = BuscaPeriodosUsar(ListaPer, diashabiles, desde, hasta);
        //    return sugerencia;
        //}
        //public List<Detperiodos> Periodos(int empresa, string rut)
        //{
        //    UsuarioVM perfiles = new UsuarioVM();
        //    try
        //    {

        //        string RutEmpresa = f.obtenerRUT(empresa);
        //        var BD_Cli = "remuneracion_" + RutEmpresa;

        //        f.EjecutarConsultaSQLCli("SELECT vacacionesPeriodos.rutTrabajador, vacacionesPeriodos.anoInicio, vacacionesPeriodos.anoTermino, vacacionesPeriodos.diasLegales " +
        //                                    ", vacacionesPeriodos.diasProgresivos, vacacionesPeriodos.diasContrato, vacacionesPeriodos.diasAdministrativos " +
        //                                    ", vacacionesPeriodos.diasFaena, vacacionesPeriodos.diasEspeciales, vacacionesPeriodos.diasOtros " +
        //                                    "FROM vacacionesPeriodos " +
        //                                    "WHERE  vacacionesPeriodos.rutTrabajador = '" + rut + "' Order by anoInicio ", BD_Cli);


        //        List<Detperiodos> Lista = new List<Detperiodos>();
        //        Lista = (from DataRow dr in f.Tabla.Rows
        //                 select new Detperiodos()
        //                 {
        //                     ruttrabajador = dr["rutTrabajador"].ToString(),
        //                     anoinicio = int.Parse(dr["anoInicio"].ToString()),
        //                     anotermino = int.Parse(dr["anoTermino"].ToString()),
        //                     diaslegales = int.Parse(dr["diasLegales"].ToString()),
        //                     diasprogresivos = int.Parse(dr["diasProgresivos"].ToString()),
        //                     diascontrato = int.Parse(dr["diasContrato"].ToString()),
        //                     diasadministrativos = int.Parse(dr["diasAdministrativos"].ToString()),
        //                     diasfaena = int.Parse(dr["diasFaena"].ToString()),
        //                     diasespeciales = int.Parse(dr["diasEspeciales"].ToString()),
        //                     diasotros = int.Parse(dr["diasOtros"].ToString())
        //                 }).ToList();
        //        List<Detperiodos> saldo = new List<Detperiodos>();
        //        if (Lista.Count > 0)
        //        {
        //            foreach (var l in Lista)
        //            {
        //                l.periodo = l.anoinicio.ToString("####") + "-" + l.anotermino.ToString("####");
        //                l.diastotal = l.diaslegales + l.diasprogresivos + l.diascontrato + l.diasfaena + l.diasespeciales + l.diasotros;
        //                Detperiodos usados = UsadosPeriodo(rut, l.anoinicio, BD_Cli);
        //                l.diaslegales = l.diaslegales - usados.diaslegales;
        //                l.diasprogresivos = l.diasprogresivos - usados.diasprogresivos;
        //                l.diascontrato = l.diascontrato - usados.diascontrato;
        //                l.diasadministrativos = l.diasadministrativos - usados.diasadministrativos;
        //                l.diasfaena = l.diasfaena - usados.diasfaena;
        //                l.diasespeciales = l.diasespeciales - usados.diasespeciales;
        //                l.diasotros = l.diasotros - usados.diasotros;
        //                l.diastotal = l.diastotal - usados.diastotal;
        //                if (l.diastotal > 0)
        //                {
        //                    saldo.Add(l);
        //                }
        //            }
        //        }
        //        return saldo;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        var Asunto = "Error al generar solicitud de vacaciones";
        //        var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
        //        correos.SendEmail(Mensaje, Asunto, "Se generó un error al generar solicitud de vacaciones", correos.destinatarioErrores);
        //        return null;
        //    }
        //}
        //public List<Detperiodos> BuscaPeriodosUsar(List<Detperiodos> periodos, double dias, string desde, string hasta)
        //{
        //    List<Detperiodos> ausar = new List<Detperiodos>();
        //    DateTime inicio = Convert.ToDateTime(desde);
        //    DateTime termino = Convert.ToDateTime(hasta);
        //    DateTime partida = Convert.ToDateTime(inicio);
        //    foreach (Detperiodos d in periodos)
        //    {
        //        if (dias > 0)
        //        {
        //            Detperiodos proc = new Detperiodos();
        //            proc.ruttrabajador = d.ruttrabajador;
        //            proc.anoinicio = d.anoinicio;
        //            proc.anotermino = d.anotermino;
        //            proc.fechainicio = partida.ToString("yyyy'-'MM'-'dd");
        //            if (dias >= d.diaslegales)
        //            {
        //                proc.diaslegales = d.diaslegales;
        //                dias = dias - d.diaslegales;
        //            }
        //            else
        //            {
        //                proc.diaslegales = (int)dias;
        //                dias = 0;
        //            }
        //            if (dias >= d.diasprogresivos)
        //            {
        //                proc.diasprogresivos = d.diasprogresivos;
        //                dias = dias - d.diasprogresivos;
        //            }
        //            else
        //            {
        //                proc.diasprogresivos = (int)dias;
        //                dias = 0;
        //            }
        //            if (dias >= d.diascontrato)
        //            {
        //                proc.diascontrato = d.diascontrato;
        //                dias = dias - d.diascontrato;
        //            }
        //            else
        //            {
        //                proc.diascontrato = (int)dias;
        //                dias = 0;
        //            }
        //            if (dias >= d.diasfaena)
        //            {
        //                proc.diasfaena = d.diasfaena;
        //                dias = dias - d.diasfaena;
        //            }
        //            else
        //            {
        //                proc.diasfaena = (int)dias;
        //                dias = 0;
        //            }
        //            if (dias >= d.diasespeciales)
        //            {
        //                proc.diasespeciales = d.diasespeciales;
        //                dias = dias - d.diasespeciales;
        //            }
        //            else
        //            {
        //                proc.diasespeciales = (int)dias;
        //                dias = 0;
        //            }
        //            proc.periodo = d.anoinicio.ToString("####") + "-" + d.anotermino.ToString("####");
        //            proc.diastotal = proc.diaslegales + proc.diasprogresivos + proc.diascontrato + proc.diasadministrativos + proc.diasfaena + proc.diasespeciales + proc.diasotros;
        //            DateTime fechacorrido = FechaHasta(partida, proc.diastotal, termino);
        //            proc.fechatermino = fechacorrido.ToString("yyyy'-'MM'-'dd");
        //            partida = fechacorrido.AddDays(1);
        //            ausar.Add(proc);
        //        }
        //    }
        //    return ausar;
        //}
        //public DateTime FechaHasta(DateTime desde, int dias, DateTime termino)
        //{
        //    DateTime fecha = desde.AddDays(-1);
        //    int ind = 1;
        //    while (ind <= dias)
        //    {
        //        fecha = fecha.AddDays(1);
        //        if (fecha.DayOfWeek == DayOfWeek.Sunday || fecha.DayOfWeek == DayOfWeek.Saturday)
        //        {
        //            fecha = fecha.AddDays(1); continue;
        //        }
        //        if (Generales.EsFeriado(fecha, 1) == true)
        //        {
        //            fecha = fecha.AddDays(1); continue;
        //        }
        //        ind++;
        //    }
        //    if (fecha != termino)
        //    {
        //        fecha = fecha.AddDays(-1);
        //    }
        //    return fecha;
        //}

        //public Detperiodos UsadosPeriodo(string rut, int ano, string BD_Cli)
        //{
        //    Detperiodos peri = new Detperiodos();

        //    f.EjecutarConsultaSQLCli("select * from vacacionesUsos where rutTrabajador='" + rut + "' and anoInicio = " + ano, BD_Cli);
        //    List<Detperiodos> Lista = new List<Detperiodos>();
        //    Lista = (from DataRow dr in f.Tabla.Rows
        //             select new Detperiodos()
        //             {
        //                 ruttrabajador = dr["rutTrabajador"].ToString(),
        //                 anoinicio = int.Parse(dr["anoInicio"].ToString()),
        //                 anotermino = int.Parse(dr["anoTermino"].ToString()),
        //                 diaslegales = int.Parse(dr["diasLegales"].ToString()),
        //                 diasprogresivos = int.Parse(dr["diasProgresivos"].ToString()),
        //                 diascontrato = int.Parse(dr["diasContrato"].ToString()),
        //                 diasadministrativos = int.Parse(dr["diasAdministrativos"].ToString()),
        //                 diasfaena = int.Parse(dr["diasFaena"].ToString()),
        //                 diasespeciales = int.Parse(dr["diasEspeciales"].ToString()),
        //                 diasotros = int.Parse(dr["diasOtros"].ToString())
        //             }).ToList();
        //    foreach (Detperiodos d in Lista)
        //    {
        //        peri.diaslegales = peri.diaslegales + d.diaslegales;
        //        peri.diasprogresivos = peri.diasprogresivos + d.diasprogresivos;
        //        peri.diascontrato = peri.diascontrato + d.diascontrato;
        //        peri.diasadministrativos = peri.diasadministrativos + d.diasadministrativos;
        //        peri.diasfaena = peri.diasfaena + d.diasfaena;
        //        peri.diasespeciales = peri.diasespeciales + d.diasespeciales;
        //        peri.diasotros = peri.diasotros + d.diasotros;
        //    }
        //    peri.diastotal = peri.diaslegales + peri.diasprogresivos + peri.diascontrato + peri.diasfaena + peri.diasespeciales + peri.diasotros;
        //    return peri;
        //}
        //public double DiasHabiles(string desde, string hasta)
        //{
        //    double hab = 0;
        //    DateTime des = Convert.ToDateTime(desde);
        //    DateTime has = Convert.ToDateTime(hasta);
        //    while (des <= has)
        //    {
        //        if (des.DayOfWeek == DayOfWeek.Sunday || des.DayOfWeek == DayOfWeek.Saturday)
        //        {
        //            des = des.AddDays(1); continue;
        //        }
        //        if (Generales.EsFeriado(des, 1) == true)
        //        {
        //            des = des.AddDays(1); continue;
        //        }
        //        hab++;
        //        des = des.AddDays(1);
        //    }
        //    return hab;
        //}
    }
}

public class DetAutorizaPersonal
{
    public int id { get; set; }
    public string ruttrabajador { get; set; }
    public string nombre { get; set; }
    public string fechainicio { get; set; }
    public string fechatermino { get; set; }
    public int diastotal { get; set; }
}
