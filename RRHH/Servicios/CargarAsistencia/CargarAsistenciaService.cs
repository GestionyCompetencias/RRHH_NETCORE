using Microsoft.AspNetCore.Mvc;
using Rollbar.DTOs;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;
using System.Diagnostics.Contracts;

namespace RRHH.Servicios.CargaAsistencia
{
    /// <summary>
    /// Servicio para cargar la asistencia del sistema de tiempo a remunerciones.
    /// </summary>
    public interface ICargaAsistenciaService
    {
        /// <summary>
        /// Genera asistencia mes.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes de proceso</param>
        /// <param name="anio">año de proceso</param>
        /// <returns>Cargar asistencia de sistema de tiempo</returns>
        public Resultado TraspasaAsistenciaService(int idEmpresa,  int mes, int anio);

    }

    public class CargaAsistenciaService : ICargaAsistenciaService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();
        List<parametros> lista = new List<parametros>();

        public CargaAsistenciaService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }
        public Resultado TraspasaAsistenciaService(int empresa, int mes, int anio)
        {
            Resultado resultado = new Resultado();
            resultado.mensaje = "No se encuentran registros";
            resultado.result = 0;
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                DateTime fecini = new DateTime(anio, mes, 1);
                DateTime fecfin = f.UltimoDia(fecini);
                string fecinistr = fecini.ToString("yyyy'-'MM'-'dd");
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");
                int holguraent = Convert.ToInt32(BuscaValor(lista, "TIEMPO", "1"));

                f.EjecutarConsultaSQLCli("DELETE FROM asistenciasInformadas WHERE asistenciasinformadas.habilitado = 1 " +
                            " and asistenciasinformadas.fechaAsistencia <= '" + fecfinstr + "' and asistenciasinformadas.fechaAsistencia >= '" + fecinistr + "' ", BD_Cli);


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
                                        sueldoBase = int.Parse(dr["rut"].ToString()),
                                        tipoCarga = dr["tipoCarga"].ToString(),
                                        articulo22 = dr["articulo22"].ToString()
                                    }).ToList();

                    foreach (var r in trabajadores)
                    {
                        List<DetalleTiempo> detdia = new List<DetalleTiempo>();
                        detdia = AsistenciaDiaria(r.rut, fecini, fecfin,lista, holguraent);
                    }
                    resultado.mensaje = "Traspaso realizado con exito";
                    resultado.result = 1;
                }
                return resultado;
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error al Consultar asistencias informadas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar asistencias informadas", correos.destinatarioErrores);
                return null;
            }
        }

        public List<DetalleTiempo> AsistenciaDiaria(String RUT, DateTime finicio, DateTime ftermino, List<parametros> infoFeria, int holguraent)
        {
            int Descanso = 0;
            int Trabajados = 0;
            int Permiso = 0;
            int Vacaciones = 0;
            int Licencia = 0;
            int Feriado;

            DateTime fecloop;
            int ind, diasem, extras, tardas;
            DateTime salpro, salrea;
            string rut;
            DateTime entrada, salida;
            string modent, modsal;
            string t_entrada, t_salida, codina;
            var nrdia = new List<DetalleTiempo>();
            //string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"].ToString(); ;

            //var infoMarca = new List<GYCEmpresa.Models.MARCACIONyRUT>();
            //var infoLicen = new List<GYCEmpresa.Models.LICENCIAMEDICA>();
            //var infoPermi = new List<GYCEmpresa.Models.PERMISOINASISTENCIA>();
            //var infoVacac = new List<GYCEmpresa.Models.rhuesolv>();
            //var infoTurno = new List<GYCEmpresa.Models.TurnoTrabajador>();
            //var infoDescu = new List<GYCEmpresa.Models.COMPENSADO_UTILIZADO>();
            //var infoExtra = new List<GYCEmpresa.Models.HHEE>();
            //var infodetal = new List<GYCEmpresa.Models.TurnoDetalle>();
            //TurnoDetalle turdet = new TurnoDetalle();
            //TurnoTrabajador turtra = new TurnoTrabajador();
            //TimeSpan TotTar = new TimeSpan();

            //var detMarca = new List<GYCEmpresa.Models.MARCACIONyRUT>();
            //var detLicen = new List<GYCEmpresa.Models.LICENCIAMEDICA>();
            //var detPermi = new List<GYCEmpresa.Models.PERMISOINASISTENCIA>();
            //var detVacac = new List<GYCEmpresa.Models.rhuesolv>();
            //var detDescu = new List<GYCEmpresa.Models.COMPENSADO_UTILIZADO>();
            //var detExtra = new List<GYCEmpresa.Models.HHEE>();
            //var detFeria = new List<GYCEmpresa.Models.remepage>();
            //var detContr = new CONTRATO();

            //int diastot = ftermino.Subtract(finicio).Days + 1;

            //rut = RUT;
            //Descanso = 0;
            //Trabajados = 0;
            //Permiso = 0;
            //Vacaciones = 0;
            //Licencia = 0;
            //Feriado = 0;
            //modent = "";
            //modsal = "";
            //t_entrada = "";
            //t_salida = "";

            //infoTurno = db.TurnoTrabajador.Where(x => x.RutTrabajador == rut && x.FechaTerminoTurno > finicio).ToList();
            //detContr = db.CONTRATO.Where(x => x.PERSONA == rut && x.EMPRESA == empresa && x.FTERMNO > finicio && x.FIRMAEMPRESA == true && x.FIRMATRABAJADOR == true && x.RECHAZADO == false).SingleOrDefault();
            //int tratur = 1;
            //if (infoTurno.Count() > 0)
            //{
            //    turtra = infoTurno.Last();
            //    if (turtra.IdTurno != null)
            //        tratur = (int)turtra.IdTurno;
            //}

            //DateTime diasig = ftermino.Date.AddDays(1);
            //infoMarca = db.MARCACIONyRUT.Where(x => x.RUT == rut && x.CHECKTIME >= finicio && x.CHECKTIME < diasig).ToList();
            //infoLicen = db.LICENCIAMEDICA.Where(x => x.TRABAJADOR == rut && !((x.FINICIO < finicio && x.FTERMINO < finicio) || (x.FINICIO > ftermino && x.FTERMINO > ftermino))).ToList();
            //infoPermi = db.PERMISOINASISTENCIA.Where(x => x.TRABAJADOR == rut && !((x.FINICIO < finicio && x.FTERMINO < finicio) || (x.FINICIO > ftermino && x.FTERMINO > ftermino))).ToList();
            //infoVacac = db.rhuesolv.Where(x => x.nrt_ruttr == rut && !((x.fec_inicio < finicio && x.fec_termin < finicio) || (x.fec_inicio > ftermino && x.fec_termin > ftermino)) && x.est_solici == "Aceptado  ").ToList();
            //infodetal = db.TurnoDetalle.Where(x => x.IdTurno == tratur).ToList();

            //fecloop = finicio;
            //for (ind = 1; ind <= diastot; ind++)
            //{
            //    if (fecloop >= detContr.FINICIO && fecloop <= detContr.FTERMNO)
            //    {

            //        entrada = Convert.ToDateTime("1999-09-09");
            //        salida = Convert.ToDateTime("1999-09-09");
            //        extras = 0;
            //        tardas = 0;
            //        codina = "";
            //        modsal = "";
            //        modent = "";
            //        diasem = (int)fecloop.DayOfWeek;
            //        if (diasem == 0) diasem = 7;
            //        turdet = infodetal.Where(x => x.Dia == diasem).SingleOrDefault();
            //        t_entrada = Convert.ToString(turdet.HoraInicio);
            //        t_salida = Convert.ToString(turdet.HoraTermino);
            //        diasig = fecloop.Date.AddDays(1);

            //        detMarca = infoMarca.Where(x => x.CHECKTIME >= fecloop && x.CHECKTIME < diasig).ToList();
            //        detLicen = infoLicen.Where(x => fecloop >= x.FINICIO && fecloop <= x.FTERMINO).ToList();
            //        detPermi = infoPermi.Where(x => fecloop >= x.FINICIO && fecloop <= x.FTERMINO).ToList();
            //        detVacac = infoVacac.Where(x => fecloop >= x.fec_inicio && fecloop <= x.fec_termin).ToList();
            //        detExtra = db.HHEE.Where(x => x.TRABAJADOR == rut && (x.FINICIO >= fecloop && x.FINICIO < diasig)).ToList();
            //        detFeria = infoFeria.Where(x => fecloop == x.fec_param).ToList();
            //        if (detMarca.Count() > 0)
            //        {
            //            Trabajados++;
            //            codina = "TRABAJADO";
            //            foreach (var m in detMarca)
            //            {
            //                if (m.CHECKTYPE == "O")
            //                { salida = m.CHECKTIME; if (Convert.ToBoolean(m.MODIFICADA)) modsal = "*"; }
            //                else
            //                {
            //                    entrada = m.CHECKTIME; if (Convert.ToBoolean(m.MODIFICADA)) { modent = "*"; }
            //                    TotTar = (TimeSpan)turdet.HoraInicio;
            //                    int mintur = TotTar.Minutes + (TotTar.Hours) * 60;
            //                    if (mintur > 0)
            //                    {

            //                        int minent = m.CHECKTIME.Hour * 60 + m.CHECKTIME.Minute;
            //                        tardas = minent - mintur;
            //                        if (tardas < holguraent) tardas = 0;
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (detLicen.Count() > 0) { Licencia++; codina = "LICENCIA"; }
            //            else
            //            {
            //                if (detFeria.Count() > 0) { Feriado++; codina = "FERIADO"; }
            //                else
            //                {
            //                    if (detPermi.Count() > 0) { Permiso++; codina = "PERMISO"; }
            //                    else
            //                    {
            //                        if (detVacac.Count() > 0) { Vacaciones++; codina = "VACACION"; }
            //                        else
            //                        {
            //                            string paso = Convert.ToString(turdet.TotalHora);
            //                            int hrs = Convert.ToInt32(paso.Substring(0, 2));

            //                            if (hrs == 0 || diasem == 7)
            //                            {
            //                                Descanso = Descanso + 1; codina = "DESCANSO";
            //                            }

            //                        }
            //                    }
            //                }
            //            }
            //            //Horas extras
            //        }
            //        if (detExtra.Count() > 0)
            //        {
            //            foreach (var t in detExtra)
            //            {
            //                salpro = t.FINICIO;
            //                salrea = t.FTERMINO;
            //                extras = (salrea.Subtract(salpro).Hours) * 60 + salrea.Subtract(salpro).Minutes;
            //            }
            //        }

            //        nrdia.Add(new DetalleTiempo()
            //        {
            //            RUT = rut,
            //            FECHA = fecloop,
            //            C_INASI = codina,
            //            H_ENTRADA = entrada,
            //            H_SALIDA = salida,
            //            M_ENTRADA = modent,
            //            M_SALIDA = modsal,
            //            T_ENTRADA = t_entrada,
            //            T_SALIDA = t_salida,
            //            D_DESCANSO = Descanso,
            //            D_TOTALES = Feriado,
            //            M_EXTRA = extras,
            //            M_TARDA = tardas,
            //            M_PERMI = 0
            //        });
            //    }
            //    fecloop = fecloop.AddDays(1);
            //}
            return nrdia;
        }

        public bool CargaParametros(int empresa)
        {
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            f.EjecutarConsultaSQLCli("SELECT parametrosGenerales.tabla,parametrosGenerales.codigo,parametrosGenerales.valor, " +
                                        " parametrosGenerales.fecha " +
                                        " FROM parametrosGenerales " +
                                        " WHERE parametrosGenerales.habilitado = 1 ", "remuneracion");


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
    }
}
public class DetalleTiempo
{
    public string RUT;
    public DateTime FECHA;
    public String C_INASI;
    public DateTime H_ENTRADA;
    public DateTime H_SALIDA;
    public string M_ENTRADA;
    public string M_SALIDA;
    public string T_ENTRADA;
    public string T_SALIDA;
    public int M_EXTRA;
    public int M_TARDA;
    public int M_PERMI;
    public string H_TRABA;
    public string H_TURNO;
    public int D_DESCANSO;
    public int D_TOTALES;

}
