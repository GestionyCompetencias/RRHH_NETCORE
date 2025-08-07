using RRHH.BaseDatos;
using RRHH.Models;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.ApiRRHH
{
    /// <summary>
    /// Servicio para generar archivo de previred.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IApiRRHHService
    {
        /// <summary>
        /// Api de acceso.
        /// <param name="Usuario">Usuario</param>
        /// <param name="Contraseña">Contraseña</param>
        /// <param name="ID_DISPOSITIVO">Id del dispositivo movil</param>
        /// <param name="BD_Cli">Base de datos de la empresa</param>
        /// <returns>Estructura de respuesta en JASON</returns>
        public Reply accesoService(string Usuario, string Contrasena, string ID_DISPOSITIVO, string BD_Cli);

        /// <summary>
        /// Api datos personales.
        /// <param name="Usuario">Usuario</param>
        /// <param name="Contraseña">Contraseña</param>
        /// <param name="ID_DISPOSITIVO">Id del dispositivo movil</param>
        /// <param name="BD_Cli">Base de datos de la empresa</param>
        /// <returns>Estructura de respuesta en JASON</returns>
        public Reply ExistePersonaDetalleService(string rut, string token,string BD_Cli);
    }

    public class ApiRRHHService : IApiRRHHService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();

		public ApiRRHHService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public Reply accesoService(string Usuario, string Contrasena, string ID_DISPOSITIVO, string BD_Cli)
        {
            Reply oR = new Reply();
            oR.result = 0;
            try
            {

                int iddisp = 0;
                int iddispusu = 0;
                int id = 0;
                string token = null;
                f.EjecutarConsultaSQLCli("SELECT ID, ID_DISPOSITIVO FROM DISPOSITIVO WHERE ID_DISPOSITIVO = '" + ID_DISPOSITIVO + "' ", "gycsolcl_dbAndroid");
                if (f.Tabla.Rows.Count > 0)
                {
                    DataRow dr = f.Tabla.Rows[0];
                    id = int.Parse(dr["ID"].ToString());
                    iddisp = int.Parse(dr["ID_DISPOSITIVO"].ToString());
                    f.EjecutarConsultaSQLCli("SELECT ID_DISPOSITIVO, TOKEN FROM USUARIO_DISPOSITIVO WHERE RUT = '" + Usuario + "' and CONTRASENA = '" + Contrasena +
                                             "' AND ESTADO = 1 AND DISPOSITIVO = " + iddisp, "gycsolcl_dbAndroid");
                    if (f.Tabla.Rows.Count > 0)
                    {
                        DataRow du = f.Tabla.Rows[0];
                        iddispusu = int.Parse(du["ID_DISPOSITIVO"].ToString());
                        token = du["TOKEN"].ToString();
                    }
                    else
                    {
                        oR.message = "Usuario invalido";
                        oR.result = 0;
                        return oR;
                    }
                }
                else
                {
                    oR.message = "Dispositivo no encontrado";
                    oR.result = 0;
                    return oR;
                }

                InfoRegistraDispositivo Info = RegistraDispositivo(ID_DISPOSITIVO);
                InfoEmpresaTrabajador DataEmpresa = BuscarDatosEmpresa(Usuario, 45);
                InfoPersonaFaena perFaena = BuscarDatosFaena(DataEmpresa.idFaena, BD_Cli);

                //System.Web.HttpContext.Current.Session["sessionEmpresa"] = DataEmpresa.rut_empresa;

                //if (Info.Token != "Error")
                //{
                //    USUARIO_DISPOSITIVO usuarioToken = lst.First();
                //    usuarioToken.TOKEN = Info.Token;
                //    usuarioToken.DISPOSITIVO = Info.Id_dispositivo;
                //    db.Entry(usuarioToken).State = System.Data.Entity.EntityState.Modified;
                //    db.SaveChanges();

                //    oR.result = 1;
                //    string mensaje = "Token:" + Info.Token + "#Tipo:" + lst.FirstOrDefault().TIPO.ToString();

                //    int id_trabajador = db.TRABAJADOR_AUTORIZADO.Where(x => x.TRABAJADOR == Usuario).Select(x => x.USER_ID).SingleOrDefault();
                //    mensaje = mensaje + "#IdTrabajador:" + id_trabajador + "#Touchless:" + usuarioToken.TOUCHLESS;

                //    InfoLogin Respuesta = new InfoLogin();
                //    Respuesta.token = Info.Token;
                //    Respuesta.modo = lst.FirstOrDefault().TIPO.ToString();
                //    Respuesta.idTrabajador = id_trabajador.ToString();
                //    Respuesta.touchless = usuarioToken.TOUCHLESS.ToString();
                //    Respuesta.urlLogo = DataEmpresa.logo_empresa;


                //    DateTime hoy = DateTime.Now.Date;
                //    DateTime finicio = f.PrimerDia(hoy);
                //    DateTime ffinal = f.UltimoDia(hoy);
                //    if (ffinal > hoy) ffinal = hoy;
                //    int dias_vac = 0, dias_per = 0, dias_lic = 0, dias_ina = 0, dias_des = 0;

                //    List<InformeFinal> asistencia = new List<InformeFinal>();
                //    asistencia = Registro(Usuario, finicio, ffinal);
                //    foreach (var d in asistencia)
                //    {
                //        if (d.inasistencia == "") dias_ina = dias_ina + 1;
                //        if (d.inasistencia == "DESCANSO" || d.inasistencia == "FERIADO") dias_des = dias_des + 1;
                //        if (d.inasistencia == "PERMISO") dias_per = dias_per + 1;
                //        if (d.inasistencia == "LICENCIA") dias_lic = dias_lic + 1;
                //        if (d.inasistencia == "VACACION") dias_vac = dias_vac + 1;

                //    }

                //    Respuesta.diaslicencia = Convert.ToString(dias_lic);
                //    Respuesta.diaspermiso = Convert.ToString(dias_per);
                //    Respuesta.diasvacacion = Convert.ToString(dias_vac);
                //    Respuesta.diasfalla = Convert.ToString(dias_ina);
                //    Respuesta.diasdescanso = Convert.ToString(dias_des);

                //    oR.data = mensaje;
                //    oR.data2 = Respuesta;
                //    oR.dataE = DataEmpresa;
                //    oR.dataF = perFaena;
                //    oR.message = "Acceso Correcto";

                //}
                //else
                //{
                //    oR.message = "Credenciales incorrectas";
                //}

            }
            catch (System.Exception ex)
            {
                oR.result = 0;
                oR.message = "Error, intente denuevo mas tarde";
                //ClaseErrores.LogError(ex, "Api CT", Usuario, "Login");
            }
            return oR;
        }
        private InfoPersonaFaena BuscarDatosFaena(int faebus,string BD_Cli)
        {
            InfoPersonaFaena persFaena = new InfoPersonaFaena();

            FaenasBaseVM faena = Generales.BuscaFaena(faebus, BD_Cli);
                        persFaena.descripcion = faena.Descripcion;
                        persFaena.direccion = faena.Direccion;
                        persFaena.comuna = faena.Comuna;
                    return persFaena;
        }

        private InfoRegistraDispositivo RegistraDispositivo(string Id_Dispositivo)
        {
            InfoRegistraDispositivo info = new InfoRegistraDispositivo();
            try
            {
                int idDisp = 0;
                f.EjecutarConsultaSQLCli("SELECT ID FROM DISPOSITIVO WHERE ID_DISPOSITIVO = '" + Id_Dispositivo + "' ", "gycsolcl_dbAndroid");
                if (f.Tabla.Rows.Count > 0)
                {
                    DataRow dr = f.Tabla.Rows[0];
                    idDisp = int.Parse(dr["ID"].ToString());
                }
                else
                {
                    //Dispositivo.DESCRIPCION = "Equipo agregado el " + DateTime.Now.AddHours(AjusteHora());
                    string DESCRIPCION = "Equipo agregado el " + DateTime.Now.AddHours(3);
                    string ID_DISPOSITIVO = Id_Dispositivo;
                    string query2 = "insert into DISPOSITIVO (DESCRIPCION,ID_DISPOSITIVO,TIEMPO_MONITOR) values ('" + DESCRIPCION + "','" + ID_DISPOSITIVO + "',0) ";
                    if (f.EjecutarQuerySQLCli(query2, "gycsolcl_dbAndroid"))
                    {
                        f.EjecutarConsultaSQLCli("SELECT ID FROM DISPOSITIVO WHERE DESCRIPCION = '" + DESCRIPCION + "' ", "gycsolcl_dbAndroid");
                        if (f.Tabla.Rows.Count > 0)
                        {
                            DataRow dr = f.Tabla.Rows[0];
                            idDisp = int.Parse(dr["ID"].ToString());
                        }
                    }
                    info.Token = Guid.NewGuid().ToString();
                    info.Id_dispositivo = idDisp;
                    return info;
                }
           }
           catch (System.Exception Ex)
           {
                    info.Token = "Error";
                    info.Id_dispositivo = 0;
                    return info;
           }
            return info;
        }
        private InfoEmpresaTrabajador BuscarDatosEmpresa(string rut_Trabajador, int empresa)
        {

            InfoEmpresaTrabajador empreTrab = new InfoEmpresaTrabajador();
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            string hoystr = DateTime.Now.ToString("yyyy'-'MM'-'dd");

            try
            {
                PersonasBaseVM persona = Generales.BuscaPersona(rut_Trabajador, BD_Cli);
                ContratosBaseVM contrato = Generales.BuscaContrato(rut_Trabajador,hoystr, BD_Cli);
                detempresa empres = Generales.BuscaEmpresa(RutEmpresa);

                empreTrab.nombre_empresa = empres.razonsocial;
                empreTrab.direccion_empresa = empres.direccion;
                empreTrab.idFaena = empres.idfaena;
                //var logotipo = registro["URL_LOGO"].ToString();
                //empreTrab.logo_empresa = logotipo;
                return empreTrab;
            }
            catch (System.Exception Ex)
            {
                empreTrab.rut_empresa = "";
                empreTrab.nombre_empresa = "";
                empreTrab.direccion_empresa = "";
                empreTrab.logo_empresa = "";
                return empreTrab;
            }

        }
     // API EXISTE PERSONA
        public Reply ExistePersonaDetalleService(string rut, string token, string BD_Cli)
        {
            Reply oR = new Reply();
            oR.result = 0;
            oR.data = null;
            oR.data2 = null;
            oR.message = "No existe informacion";
            if (ValidaToken(rut, token) == 0)
            {
                oR.message = "Problema de acceso";
                return oR;
            }
            string EXISTE = "N";
            DateTime hoy = DateTime.Now.Date;
            PersonasBaseVM pers = Generales.BuscaPersona(rut,BD_Cli);
            int DIAS;
            string CODIGO;
            DateTime FFERIADO, FSINDICATO;
            string RAZONSOCIAL = null;
            string hoystr = DateTime.Now.ToString("yyyy'-'MM'-'dd");
            if (pers != null)
            {
                ContratosBaseVM cont = Generales.BuscaContrato(rut, hoystr, BD_Cli);

                FFERIADO = hoy;
                CODIGO = Generales.SindicatoTrabajador(rut, hoystr, BD_Cli).ToString();
                FSINDICATO = hoy;
                string ciud = Generales.BuscaComuna(pers.IdComuna);
                string TURNOD = Generales.BuscaJornada(cont.idjornada,BD_Cli);

                EXISTE = "S";
                string FNACIM = pers.nacimiento;
                string FINICI = cont.inicio;
                string FTERMI = cont.termino;
                string FFERIA = FFERIADO.ToString("yyyy'-'MM'-'dd");
                string FSINDI = FSINDICATO.ToString("yyyy'-'MM'-'dd");
                var FAENA = Generales.BuscaFaena(cont.idfaena,BD_Cli);

                int SUELDO = Convert.ToInt32(cont.sueldobase);
                string codsin = Convert.ToString(CODIGO) + "          ";
                string NACIONALIDADD = Generales.BuscaPais(pers.IdPais);
                string REGIOND = Generales.BuscaRegion(pers.IdRegion);
                string COMUNAD = Generales.BuscaComuna(pers.IdComuna);
                BancosTrabajadorBaseVM banco = Generales.BuscaBancoTrabajador(cont.idbancotrab,BD_Cli);
                string BANCOD = banco.descripcionBanco;
                string TCUENTAD = Generales.BuscaTipoCuenta(banco.idtipocta);
                string NCUENTAD = banco.numerocuenta;
                string ECIVILD = "CASADO";
                IsapresTrabajadorBaseVM isapre = Generales.BuscaIsapreTrabajador(cont.idisapretrab,BD_Cli);
                AfpsTrabajadorBaseVM afp = Generales.BuscaAfpTrabajador(cont.idafptrab, BD_Cli);
                SindicatosTrabajadorBaseVM sind = Generales.BuscaSindicatoTrabajador(pers.Rut, BD_Cli);
                string SINDICATO = sind.descripcion;
                string TIPOCONTRATOD = Generales.BuscaTipoContrato(Convert.ToInt32(cont.tipocontrato),BD_Cli);
                FaenasBaseVM faen = Generales.BuscaFaena(Convert.ToInt32(cont.faena), BD_Cli);
                string FAENAD = faen.Descripcion;
                string SUELDOD = Convert.ToString(SUELDO);
                DIAS = Generales.VacacionesEspecialesTrabajador(rut, hoystr, BD_Cli);
                string DIASD = Convert.ToString(DIAS);
                var trabajador = new detalle();
                trabajador = CreaSalida(cont, pers,banco,isapre,afp, FNACIM, NACIONALIDADD, REGIOND, COMUNAD, COMUNAD,
                     ECIVILD,  FAENAD, TURNOD, FINICI,
                     FTERMI, FFERIA, DIASD, FSINDI, SINDICATO, RAZONSOCIAL, TIPOCONTRATOD, SUELDOD, EXISTE);
                oR.result = 1;
                oR.data = token;
                oR.data2 = trabajador;
                oR.message = "Consulta exitosa";
                return oR;
            }
            else
            {
                oR.data2 = null;
                return oR;
            }
        }
        public detalle CreaSalida(ContratosBaseVM cont, PersonasBaseVM pers,BancosTrabajadorBaseVM banco,
            IsapresTrabajadorBaseVM isapre, AfpsTrabajadorBaseVM afp,
            string FNACIM, string NACIONALIDADD, string REGIOND, string CIUDADD, string COMUNAD,
            string ECIVILD, string FAENAPAS, string TURNOD, string FINICID,
            string FTERMID, string FFERIAD, string DIASD, string FSINDID, string SINDICATOD, string RAZONSOCIALD,
            string TIPOCONTRATOD, string SUELDOD, string EXISTED)
        {
            var nr = new detalle();
            nr.nombre = pers.Nombres;
            nr.apaterno = pers.Apellidos;
            nr.amaterno = "";
            nr.fnacimiento = FNACIM;
            nr.nacionalidad = NACIONALIDADD;
            nr.telefono1 = pers.Tlf;
            nr.telefono2 = "";
            nr.correo = pers.Email;
            nr.direccion = pers.direccion;
            nr.region = REGIOND;
            nr.ciudad = CIUDADD;
            nr.comuna = COMUNAD;
            nr.sexo = pers.sexo;
            nr.banco = banco.descripcionBanco;
            nr.tcuenta = banco.descripcionTipo;
            nr.ncuenta = banco.numerocuenta;
            nr.ecivil = ECIVILD;
            nr.nhijos = Convert.ToString(pers.nrohijos);
            nr.salud = isapre.descripcion;
            nr.adicionalsalud = isapre.numeroUf.ToString();
            nr.prevision = afp.descripcion;
            nr.apv = afp.apv.ToString();
            nr.ahorro = afp.apv.ToString();
            nr.empresaactual = "";
            nr.estadocontractual = "";
            nr.faena = FAENAPAS;
            nr.turno = TURNOD;
            nr.finicio = FINICID;
            nr.ftermi = FTERMID;
            nr.fferia = FFERIAD;
            nr.dias = DIASD;
            nr.fsindi = FSINDID;
            nr.sindicato = SINDICATOD;
            nr.razonsocial = RAZONSOCIALD;
            nr.cargo = cont.cargo;
            nr.tipocontrato = TIPOCONTRATOD;
            nr.sueldo = SUELDOD;
            nr.existe = EXISTED;
            return nr;
        }

        public int ValidaToken(string rut, string token)
        {
            int valido = 0;
            f.EjecutarConsultaSQLCli("SELECT * WHERE RUT = '" + rut + "' and TOKE = '" + token + "' ", "gycsolcl_dbAndroid");
            if (f.Tabla.Rows.Count > 0)valido = 1;
            return valido;
        }


        private class InfoRegistraDispositivo
        {
            public int Id_dispositivo { get; set; }
            public string Token { get; set; }
        }


        private class InfoEmpresaTrabajador
        {
            public string rut_empresa { get; set; }
            public string nombre_empresa { get; set; }
            public string direccion_empresa { get; set; }
            public string logo_empresa { get; set; }
            public int idFaena { get; set; }
        }

        private class InfoPersonaFaena
        {
            public string direccion { get; set; }
            public string descripcion { get; set; }
            public string comuna { get; set; }
        }
    }
}
namespace RRHH.Models
{

    public class detalle
    {
        public string nombre { get; set; }
        public string apaterno { get; set; }
        public string amaterno { get; set; }
        public string fnacimiento { get; set; }
        public string nacionalidad { get; set; }
        public string telefono1 { get; set; }
        public string telefono2 { get; set; }
        public string correo { get; set; }
        public string direccion { get; set; }
        public string region { get; set; }
        public string ciudad { get; set; }
        public string comuna { get; set; }
        public string sexo { get; set; }
        public string banco { get; set; }
        public string tcuenta { get; set; }
        public string ncuenta { get; set; }
        public string ecivil { get; set; }
        public string nhijos { get; set; }
        public string salud { get; set; }
        public string adicionalsalud { get; set; }
        public string prevision { get; set; }
        public string apv { get; set; }
        public string ahorro { get; set; }
        public string empresaactual { get; set; }
        public string estadocontractual { get; set; }
        public string faena { get; set; }
        public string turno { get; set; }
        public string finicio { get; set; }
        public string ftermi { get; set; }
        public string fferia { get; set; }
        public string dias { get; set; }
        public string fsindi { get; set; }
        public string sindicato { get; set; }
        public string razonsocial { get; set; }
        public string cargo { get; set; }
        public string tipocontrato { get; set; }
        public string sueldo { get; set; }
        public string existe { get; set; }

    }


    internal class InfoLogin
    {
        public string token { get; set; }
        public string modo { get; set; }
        public string idTrabajador { get; set; }
        public string touchless { get; set; }
        public string urlLogo { get; set; }
        public string diaslicencia { get; set; }
        public string diaspermiso { get; set; }
        public string diasvacacion { get; set; }
        public string diasfalla { get; set; }
        public string diasdescanso { get; set; }
    }
    public class detdoc
    {
        public string trabajador { get; set; }
        public string nombre { get; set; }
        public string tipo { get; set; }
        public string descripcion { get; set; }
        public object inicio { get; set; }
        public string termino { get; set; }
        public object archivo { get; set; }
        public object url { get; set; }
    }

    public class tipodoc
    {
        public string id { get; set; }
        public string descripcion { get; set; }
    }
    public class detvacacion
    {
        public string nrt_ruttr { get; set; }
        public string nro_periodo { get; set; }
        public string fec_inivac { get; set; }
        public string fec_finvac { get; set; }
        public string dias_habil { get; set; }
        public string dias_corri { get; set; }
        public string ano_inicio { get; set; }
        public string ano_termino { get; set; }
        public string tip_uso { get; set; }
        public string nro_solici { get; set; }
        public string dias_legal { get; set; }
        public string dias_progr { get; set; }
        public string dias_contr { get; set; }
        public string dias_admin { get; set; }
        public string dias_faena { get; set; }
        public string dias_especi { get; set; }
        public string dias_otros { get; set; }
        public string idsolicitud { get; set; }
    }
    public class detperiodo
    {
        public int correl { get; set; }
        public string nrt_ruttr { get; set; }
        public int ano_inicio { get; set; }
        public int ano_termino { get; set; }
        public int dias_legal { get; set; }
        public Nullable<int> dias_progr { get; set; }
        public Nullable<int> dias_contr { get; set; }
        public Nullable<int> dias_admin { get; set; }
        public Nullable<int> dias_faena { get; set; }
        public Nullable<int> dias_especi { get; set; }
        public Nullable<int> dias_otros { get; set; }
        public string fec_trans { get; set; }
        public string rut_empr { get; set; }
    }
    public class detusados
    {
        public int correl { get; set; }
        public string nrt_ruttr { get; set; }
        public int ano_inicio { get; set; }
        public int ano_termino { get; set; }
        public string tip_uso { get; set; }
        public string fec_inivac { get; set; }
        public string fec_tervac { get; set; }
        public int nro_solici { get; set; }
        public int dias_corri { get; set; }
        public Nullable<int> dias_legal { get; set; }
        public Nullable<int> dias_progr { get; set; }
        public Nullable<int> dias_contr { get; set; }
        public Nullable<int> dias_admin { get; set; }
        public Nullable<int> dias_faena { get; set; }
        public Nullable<int> dias_especi { get; set; }
        public Nullable<int> dias_otros { get; set; }
        public string fec_transa { get; set; }
        public string rut_empr { get; set; }
    }
    public class detconsulta
    {
        public int ID { get; set; }
        public string FECHA { get; set; }
        public string FINICIO { get; set; }
        public string FTERMINO { get; set; }
        public bool? AUTORIZAEMPRESA { get; set; }
        public bool? AUTORIZATRABAJADOR { get; set; }
        public int? COMPENSADO { get; set; }
        public string TRABAJADOR { get; set; }
        public string MOTIVO { get; set; }
    }
    public class detlicencia
    {
        public int ID { get; set; }
        public string CODIGO_LICENCIA { get; set; }
        public string TRABAJADOR { get; set; }
        public string FINICIO { get; set; }
        public string FTERMINO { get; set; }
        public Nullable<int> DIAS { get; set; }
        public string TIPO_LICENCIA { get; set; }
        public string COMENTARIO { get; set; }
        public string TIPO_MEDICO { get; set; }
        public byte[] PDF { get; set; }
        public string TIPOLICENCIASMEDICAS { get; set; }
        public string TIPOMEDICO { get; set; }
        //public virtual PERSONA PERSONA { get; set; }
    }
    public class detnotifica
    {
        public int ID { get; set; }
        public string TIPO { get; set; }
        public string FECHA { get; set; }
        public string OBSERVACION { get; set; }
        public Nullable<bool> GYC { get; set; }
        public Nullable<bool> NOTIFTRABAJADOR { get; set; }
        public Nullable<bool> NOTIFEMPRESA { get; set; }
        public string TRABAJADOR { get; set; }
        public string EMPRESA { get; set; }
        public string USUARIO { get; set; }
        public string TABLA { get; set; }
        public int IDTABLA { get; set; }
        public Nullable<int> ESTADO { get; set; }
        public Nullable<bool> VISTO { get; set; }
        //public virtual USUARIO USUARIO1 { get; set; }

    }
}
