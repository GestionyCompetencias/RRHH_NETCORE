
using RRHH.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;

namespace RRHH.BaseDatos
{
    public class Funciones
    {

        private static String servidor = @"DESKTOP-1NC595O//SQLEXPRESS";
        private static String contra = "123456";
        private static String base_datos = "contable";
        private static String usuario_db = "sa";
        private static String port_db = "3306";

        public DataTable dt = new DataTable();
        public DataSet objDS = new DataSet();
        public DataTable table = new DataTable("FillSN");
        public DataTable Tabla = new DataTable();

        private byte[] _key = Encoding.ASCII.GetBytes("GudiÑo..*ñ==2548832019trRYxñOprs");
        private byte[] _iv = Encoding.ASCII.GetBytes("ñÑopYcFFsnjs*/+.");

        public SqlDataAdapter adapter = new SqlDataAdapter();


        public string CadenaConexion()
        {

            var EntornoEjecucion = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string CadenaSqlCli = "";

            if (EntornoEjecucion.Contains("contab.test"))
            {
                CadenaSqlCli = "Data Source=179.61.13.236,1498;Initial Catalog=contable;User ID=gycsolcl_gestionAdmin;Password=–E!**Qi(*ygG(Ft[85Ñ;Trust Server Certificate=true";
            }
            else if (EntornoEjecucion.Contains("contab.beta"))
            {
                CadenaSqlCli = "Data Source=179.61.13.236,1499;Initial Catalog=contable;User ID=gycsolcl_gestionAdmin;Password=5<!>ZÑE4V{XhPoñm_sO*;Trust Server Certificate=true";
            }
            else if (EntornoEjecucion.Contains("contab.gycsol"))
            {
                CadenaSqlCli = "Data Source=179.61.13.236;Initial Catalog=contable;User ID=gycsolcl_gestionAdmin;Password=.7VzG#{Ty(Gvu{!:(fm}4:4/LT;Trust Server Certificate=true";
            }
            else if (EntornoEjecucion.Contains("RRHH"))
            {
                CadenaSqlCli = "Data Source=179.61.13.236,1499;Initial Catalog=contable;User ID=gycsolcl_gestionAdmin;Password=5<!>ZÑE4V{XhPoñm_sO*;Trust Server Certificate=true";
            }

            return CadenaSqlCli;
        }

        public string CadenaConexionCliente(String catalogo)
        {
            var EntornoEjecucion = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string CadenaSqlCli = "";

            //CadenaSqlCli = "Data Source=179.61.13.236;Initial Catalog=" + catalogo + ";User ID=gycsolcl_gestionAdmin;Password=.7VzG#{Ty(Gvu{!:(fm}4:4/LT;Trust Server Certificate=true";
            if (EntornoEjecucion.Contains("contab.test"))
            {
                CadenaSqlCli = "Data Source=179.61.13.236,1498;Initial Catalog=" + catalogo + ";User ID=gycsolcl_gestionAdmin;Password=–E!**Qi(*ygG(Ft[85Ñ;Trust Server Certificate=true";
            }
            else if (EntornoEjecucion.Contains("contab.beta"))
            {
                CadenaSqlCli = "Data Source=179.61.13.236,1499;Initial Catalog=" + catalogo + ";User ID=gycsolcl_gestionAdmin;Password=5<!>ZÑE4V{XhPoñm_sO*;Trust Server Certificate=true";
            }
            else if (EntornoEjecucion.Contains("contab.gycsol"))
            {
                CadenaSqlCli = "Data Source=179.61.13.236;Initial Catalog=" + catalogo + ";User ID=gycsolcl_gestionAdmin;Password=.7VzG#{Ty(Gvu{!:(fm}4:4/LT;Trust Server Certificate=true";
            }
            else if (EntornoEjecucion.Contains("Visual Studio 2022"))
            {
                CadenaSqlCli = "Data Source=179.61.13.236,1499;Initial Catalog=" + catalogo + ";User ID=gycsolcl_gestionAdmin;Password=5<!>ZÑE4V{XhPoñm_sO*;Trust Server Certificate=true";
            }

            // Utilizar cadena de conexión de desarrollo en caso de que el entorno de ejecucción no sea encontrado
            if (CadenaSqlCli == String.Empty)
                CadenaSqlCli = "Data Source=179.61.13.236,1499;Initial Catalog=" + catalogo + ";User ID=gycsolcl_gestionAdmin;Password=5<!>ZÑE4V{XhPoñm_sO*;Trust Server Certificate=true";


            return CadenaSqlCli;
        }


        private static Random random = new Random();


        public Funciones()
        {

        }

        [HttpPost]
        public Boolean EjecutarConsultaSQL(String consulta)
        {

            var connstr = CadenaConexion();
            SqlConnection conexion;
            conexion = new SqlConnection(connstr);

            try
            {

                DataRow returValue = null;
                using (SqlConnection cmd = new SqlConnection(connstr))
                {

                    if (conexion.State == ConnectionState.Open)
                    {
                        conexion.Close();
                    }

                    conexion.Open();

                    adapter = new SqlDataAdapter(consulta, conexion);
                    Tabla = new DataTable();
                    adapter.Fill(Tabla);
                    if (Tabla.Rows.Count > 0)
                    {
                        returValue = Tabla.Rows[0];
                    }

                    conexion.Close();

                }

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool EjecutarQuerySQL(String query)
        {
            var connstr = CadenaConexion();
            string phrase = query;
            string[] consultas = phrase.Split('!');

            SqlTransaction transaccion;
            SqlCommand comando;
            SqlConnection cn;
            cn = new SqlConnection(connstr);

            try
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cn.Open();
                    transaccion = cn.BeginTransaction();
                    for (int c = 0; c < consultas.Length; c++)
                    {
                        if (consultas[c] != "")
                        {
                            comando = new SqlCommand(consultas[c], cn);
                            comando.Transaction = transaccion;
                            comando.ExecuteNonQuery();
                        }

                    }

                    transaccion.Commit();
                    cn.Close();

                }
                return true;
            }
            catch (Exception ex)
            {
                cn.Close();
                return false;
            }
        }

        public string method2(int length)
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(characters, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public Boolean EjecutarConsultaSQLCli(String consulta, String catalogo)
        {

            var EntornoEjecucion = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string connstrCliente = CadenaConexionCliente(catalogo);

            // Utilizar cadena de conexión de desarrollo en caso de que el entorno de ejecucción no sea encontrado
            if (connstrCliente == String.Empty)
                connstrCliente = "Data Source=179.61.13.236,1499;Initial Catalog=" + catalogo + ";User ID=gycsolcl_gestionAdmin;Password=5<!>ZÑE4V{XhPoñm_sO*;Trust Server Certificate=true";


            SqlConnection conexion;
            conexion = new SqlConnection(connstrCliente);

            try
            {
                DataRow returValue = null;
                using (SqlConnection cmd = new SqlConnection(connstrCliente))
                {
                    conexion.Open();

                    adapter = new SqlDataAdapter(consulta, conexion);
                    Tabla = new DataTable();
                    adapter.Fill(Tabla);
                    if (Tabla.Rows.Count > 0)
                    {
                        returValue = Tabla.Rows[0];
                    }

                    conexion.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                conexion.Close();
                return false;
            }

        }

        public bool EjecutarQuerySQLCli(String query, String catalogo)
        {

            var EntornoEjecucion = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string connstrCliente = CadenaConexionCliente(catalogo);

            // Utilizar cadena de conexión de desarrollo en caso de que el entorno de ejecucción no sea encontrado
            if (connstrCliente == String.Empty)
                connstrCliente = "Data Source=179.61.13.236,1499;Initial Catalog=" + catalogo + ";User ID=gycsolcl_gestionAdmin;Password=5<!>ZÑE4V{XhPoñm_sO*;Trust Server Certificate=true";

            string phrase = query;
            string[] consultas = phrase.Split('!');

            SqlTransaction transaccion;
            SqlCommand comando;
            SqlConnection cn;
            cn = new SqlConnection(connstrCliente);

            try
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cn.Open();
                    transaccion = cn.BeginTransaction();
                    for (int c = 0; c < consultas.Length; c++)
                    {
                        if (consultas[c] != "")
                        {
                            comando = new SqlCommand(consultas[c], cn);
                            comando.Transaction = transaccion;
                            comando.ExecuteNonQuery();
                        }
                    }
                    transaccion.Commit();
                    cn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                cn.Close();
                return false;
            }
        }

        public EmpresasVM obtenerEmpresa(int idEmpresa)
        {
            EmpresasVM emp = new EmpresasVM();
            EjecutarConsultaSQL("select * from empresas where id=" + idEmpresa + "");
            if (Tabla.Rows.Count > 0)
            {
                emp.rut = Tabla.Rows[0]["rut"].ToString();
                emp.razonsocial = Tabla.Rows[0]["razonsocial"].ToString();
                emp.fantasia = Tabla.Rows[0]["fantasia"].ToString();
                emp.email = Tabla.Rows[0]["email"].ToString();
                emp.direccion= Tabla.Rows[0]["direccion"].ToString();
                emp.idPais = int.Parse(Tabla.Rows[0]["idpais"].ToString());
                emp.idregion = int.Parse(Tabla.Rows[0]["idregion"].ToString());
                emp.idcomuna = int.Parse(Tabla.Rows[0]["idcomuna"].ToString());
            }
            return emp;
        }
        public string obtenerRUT(int idEmpresa)
        {
            string Rut = "";
            EjecutarConsultaSQL("select * from empresas where id=" + idEmpresa + "");
            if (Tabla.Rows.Count > 0)
            {
                Rut = Tabla.Rows[0]["rut"].ToString();
            }
            return Rut;
        }

        public string BuscaBancoId(string BD_Cli, int id)
        {
            string descripcion = "No existe banco";
            EjecutarConsultaSQLCli("select * from bancos where id='" + id + "'", "remuneracion");
            if (Tabla.Rows.Count > 0)
            {
                DataRow salida = Tabla.Rows[0];
                descripcion = salida["descripcion"].ToString();
            }
            return descripcion;
        }
        public string BuscaAfpCodigo(string BD_Cli, int codigo)
        {
            string descripcion = "No existe afp";
            EjecutarConsultaSQLCli("select * from afps where codigo='" + codigo + "'", "remuneracion");
            if (Tabla.Rows.Count > 0)
            {
                DataRow salida = Tabla.Rows[0];
                descripcion = salida["descripcion"].ToString();
            }
            return descripcion;
        }
        public string BuscaIsapreCodigo(string BD_Cli, int codigo)
        {
            string descripcion = "No existe isapre";
            EjecutarConsultaSQLCli("select * from isapres where codigo='" + codigo + "'", "remuneracion");
            if (Tabla.Rows.Count > 0)
            {
                DataRow salida = Tabla.Rows[0];
                descripcion = salida["descripcion"].ToString();
            }
            return descripcion;
        }

        public string Encrit(string inputText)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputText);
            byte[] encripted; RijndaelManaged cripto = new RijndaelManaged();
            using (MemoryStream ms = new MemoryStream(inputBytes.Length))
            {
                using (CryptoStream objCryptoStream = new CryptoStream(ms, cripto.CreateEncryptor(_key, _iv), CryptoStreamMode.Write))
                {
                    objCryptoStream.Write(inputBytes, 0, inputBytes.Length);
                    objCryptoStream.FlushFinalBlock();
                    objCryptoStream.Close();
                }
                encripted = ms.ToArray();
            }
            return Convert.ToBase64String(encripted);
        }

        public string Desencrit(string inputText)
        {
            byte[] inputBytes = Convert.FromBase64String(inputText);
            byte[] resultBytes = new byte[inputBytes.Length];
            string textoLimpio = String.Empty;
            RijndaelManaged cripto = new RijndaelManaged();
            using (MemoryStream ms = new MemoryStream(inputBytes))
            {
                using (CryptoStream objCryptoStream = new CryptoStream(ms, cripto.CreateDecryptor(_key, _iv), CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(objCryptoStream, true))
                    {
                        textoLimpio = sr.ReadToEnd();
                    }
                }
            }
            return textoLimpio;
        }
        public Resultado Meses()
        {
            Resultado resultado = new Resultado();
            List<ListaSelecionar> nr = new List<ListaSelecionar>();
            nr.Add(new ListaSelecionar() { codigo = "1", descripcion = "ENERO" });
            nr.Add(new ListaSelecionar() { codigo = "2", descripcion = "FEBRERO" });
            nr.Add(new ListaSelecionar() { codigo = "3", descripcion = "MARZO" });
            nr.Add(new ListaSelecionar() { codigo = "4", descripcion = "ABRIL" });
            nr.Add(new ListaSelecionar() { codigo = "5", descripcion = "MAYO" });
            nr.Add(new ListaSelecionar() { codigo = "6", descripcion = "JUNIO" });
            nr.Add(new ListaSelecionar() { codigo = "7", descripcion = "JULIO" });
            nr.Add(new ListaSelecionar() { codigo = "8", descripcion = "AGOSTO" });
            nr.Add(new ListaSelecionar() { codigo = "9", descripcion = "SEPTIEMBRE" });
            nr.Add(new ListaSelecionar() { codigo = "10", descripcion = "OCTUBRE" });
            nr.Add(new ListaSelecionar() { codigo = "11", descripcion = "NOVIEMBRE" });
            nr.Add(new ListaSelecionar() { codigo = "12", descripcion = "DICIEMBRE" });
            var data = new List<Conceptos>();
            foreach (var h in nr)
            {
                data.Add(new Conceptos() { Codigo = h.codigo, Descripcion = h.descripcion });
            }
            resultado.result = 1;
            resultado.mensaje = "Se cargaron los meses";
            resultado.data = data;
            return resultado;
        }
        public Resultado Anos()
        {
            List<ListaSelecionar> nr = new List<ListaSelecionar>();
            DateTime hoy = DateTime.Now.Date;    
            int ini = hoy.Year - 10 ;
            for (var i = hoy.Year; i > ini; i--)
            {
                string anio = i.ToString();
                nr.Add(new ListaSelecionar() { codigo = i.ToString(), descripcion = anio });
            }
            var data = new List<Conceptos>();
            foreach (var h in nr)
            {
                data.Add(new Conceptos() { Codigo = h.codigo, Descripcion = h.descripcion });
            }
            Resultado resultado = new Resultado();
            resultado.result = 1;
            resultado.mensaje = "Se cargaron los meses";
            resultado.data = data;

            return resultado;
        }
         public Resultado TiposPagos(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = obtenerRUT(empresa);
            var BD_Cli = "remuneracion";

            EjecutarConsultaSQLCli("SELECT parametrosGenerales.id,parametrosGenerales.tabla,parametrosGenerales.codigo,parametrosGenerales.descripcion, " +
                                        " parametrosGenerales.valor,parametrosGenerales.fecha, " +
                                        "parametrosGenerales.inicio, parametrosGenerales.termino " +
                                        "FROM parametrosGenerales " +
                                        " where parametrosGenerales.habilitado = 1 and parametrosGenerales.tabla = 'TIPOPAGO' ", BD_Cli);

            List<ParametrosBaseVM> opcionesList = new List<ParametrosBaseVM>();
            if (Tabla.Rows.Count > 0)
            {

                opcionesList = (from DataRow dr in Tabla.Rows
                                select new ParametrosBaseVM()
                                {
                                    Id = int.Parse(dr["id"].ToString()),
                                    Tabla = dr["tabla"].ToString(),
                                    Codigo = dr["codigo"].ToString(),
                                    Descripcion = dr["Descripcion"].ToString(),
                                    Valor = dr["valor"].ToString(),
                                    Fecha = dr["fecha"].ToString(),
                                    Inicio = dr["inicio"].ToString(),
                                    Termino = dr["termino"].ToString()
                                }).ToList();
            }
            var data = new List<Conceptos>();
            foreach (var h in opcionesList)
            {
                data.Add(new Conceptos() { Codigo = Convert.ToString(h.Codigo), Descripcion = h.Descripcion });
            }
            resultado.result = 1;
            resultado.data = data;
            return resultado;
        }
        public DateTime PrimerDia(DateTime fecha)
        {
            DateTime primero = Convert.ToDateTime("1900-01-01");
            if (fecha == null) return primero;
            string fecst;
            int dia, mes, año;
            mes = fecha.Month;
            año = fecha.Year;
            dia = 01;
            fecst = año + "-" + mes + "-" + dia;
            primero = Convert.ToDateTime(fecst);
            return primero;

        }
        public DateTime UltimoDia(DateTime fecha)
        {
            DateTime ultimo = Convert.ToDateTime("1900-01-01");
            if (fecha == null) return ultimo;
            DateTime messig = PrimerDia(fecha.AddMonths(1));
            ultimo = messig.AddDays(-1);
            return ultimo;

        }
        public string MesLetras(int mes)
        {
            string messtr = null;
            if (mes == 1) messtr = "ENERO";
            if (mes == 2) messtr = "FEBRERO";
            if (mes == 3) messtr = "MARZO";
            if (mes == 4) messtr = "ABRIL";
            if (mes == 5) messtr = "MAYO";
            if (mes == 6) messtr = "JUNIO";
            if (mes == 7) messtr = "JULIO";
            if (mes == 8) messtr = "AGOSTO";
            if (mes == 9) messtr = "SEPTIEMBRE";
            if (mes == 10) messtr = "OCTUBRE";
            if (mes == 11) messtr = "NOVIEMBRE";
            if (mes == 12) messtr = "DICIEMBRE";
            return messtr;

        }
        public string SemanaLetras(string diasem)
        {
            string diastr = "Domingo";
            if (diasem == "Monday") diastr = "Lunes";
            if (diasem == "Tuesday") diastr = "Martes";
            if (diasem == "Wednesday") diastr = "Miercoles";
            if (diasem == "Thursday") diastr = "Jueves";
            if (diasem == "Friday") diastr = "Viernes";
            if (diasem == "Saturday") diastr = "Sabado";
            return diastr;
        }
        public void Bitacora(string idUsuario, string modulo, string BD_Cli)
        {
            DateTime ahora = DateTime.Now;
            int idusu = Convert.ToInt32(idUsuario);
            string query2 = "insert into bitacora (IdUsuario,fechaConeccion,modulo) values (" + idusu + " , '" + ahora + "', " + modulo + "' )";
            EjecutarQuerySQLCli(query2, BD_Cli);
            return;

        }
        public string DescripcionHaber(int empresa, int haber)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            string descripcion = "No existe haber";
            EjecutarConsultaSQLCli("SELECT haberes.descripcion " +
                                        "FROM haberes " +
                                        " where haberes.habilitado = 1 and haberes.haber = "+haber+" ", BD_Cli);

            if (Tabla.Rows.Count > 0)
            {
                DataRow dr = Tabla.Rows[0];
                descripcion = dr["descripcion"].ToString() ;
            }
            return descripcion;
        }
        public string DescripcionDescuentos(int empresa, int descuento)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            string descripcion = "No existe descuento";
            EjecutarConsultaSQLCli("SELECT descuentos.descripcion " +
                                        "FROM descuentos " +
                                        " where descuentos.habilitado = 1 and descuentos.descuento = " + descuento + " ", BD_Cli);

            if (Tabla.Rows.Count > 0)
            {
                DataRow dr = Tabla.Rows[0];
                descripcion = dr["descripcion"].ToString();
            }
            return descripcion;
        }
        public string NombrePersona(int empresa, string rut)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            string nombre = "No existe persona";
            EjecutarConsultaSQLCli("SELECT personas.nombres,personas.apellidos " +
                                        "FROM personas " +
                                        " where personas.rut = '" + rut + "' ", BD_Cli);

            if (Tabla.Rows.Count > 0)
            {
                DataRow dr = Tabla.Rows[0];
                nombre = dr["nombres"].ToString() + " " + dr["apellidos"].ToString();
            }
            return nombre;
        }

        public bool crearNuevaBD(string Rut)
        {

            //****************************************************************  CREAR NUEVA BASE DE DATOS

            var queryCrearBD = "CREATE DATABASE contable_" + Rut;

            EjecutarConsultaSQL(queryCrearBD);

            //****************************************************************  CREAR TABLA activos

            string queryCrearTablaActivos = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[activos] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[num_activo] [int] NULL, " +
                "[num_sufijo] [int] NULL, " +
                "[tipo_activo] [int] NULL, " +
                "[cod_cenres] [int] NULL, " +
                "[fec_marcha] [date] NULL, " +
                "[cod_propie] [varchar](10) NULL, " +
                "[cod_cuenta] [varchar](10) NULL, " +
                "[cod_seguro] [varchar](10) NULL, " +
                "[gls_activo] [varchar](50) NULL, " +
                "[gls_activ1] [varchar](100) NULL, " +
                "[fec_invent] [date] NULL, " +
                "[tip_docu] [int] NULL, " +
                "[num_docume] [varchar](20) NULL, " +
                "[fec_docume] [date] NULL, " +
                "[fec_termino] [date] NULL, " +
                "[tip_despres] [int] NULL, " +
                "[cod_ifrs] [varchar](10) NULL, " +
                "[valor] [decimal](18, 2) NULL, " +
                "[habilitado] [int] NULL, " +
                "CONSTRAINT [PK_activos] PRIMARY KEY CLUSTERED  " +
                "( [id] ASC ) " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaActivos);

            //****************************************************************  CREAR TABLA archivos

            string queryCrearTablaArchivos = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[archivos] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[nombre] [varchar](150) NOT NULL, " +
                "[extencion] [varchar](8) NOT NULL, " +
                "[tamanio] [float] NOT NULL, " +
                "[ubicacion] [text] NOT NULL, " +
                "[fecha] [date] NOT NULL, " +
                "CONSTRAINT [PK_archivos] PRIMARY KEY CLUSTERED  " +
                "([id] ASC ) " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaArchivos);

            //****************************************************************  CREAR TABLA Aux

            string queryCrearTablaAux = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[Aux] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[idtipoaux] [int] NULL, " +
                "[cod] [varchar](15) NULL, " +
                "[nombre] [varchar](150) NULL, " +
                "[anio] [int] NULL, " +
                "[habilitado] [int] NOT NULL, " +
                "[operacional] [int] NOT NULL, " +
                "CONSTRAINT [PK_Aux_1] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaAux);

            //****************************************************************  CREAR TABLA auxiliares

            string queryCrearTablaauxiliares = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[auxiliares] " +
                "([id][int] IDENTITY(1, 1) NOT NULL, " +
                "[idsubcuenta] [int] NOT NULL, " +
                "[nombre] [varchar] (max)NOT NULL, " +
                "[codigo][varchar] (2) NULL, " +
                "[contab][varchar] (7) NULL, " +
                "CONSTRAINT[PK_auxiliares] PRIMARY KEY CLUSTERED " +
                "([id] ASC)  " +
                "WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY] " +
                ") ON[PRIMARY] TEXTIMAGE_ON[PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaauxiliares);

            //****************************************************************  CREAR TABLA bancoprov

            string queryCrearTablabancoprov = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[bancoprov] " +
                "([id][int] IDENTITY(1, 1) NOT NULL, " +
                "[idprov] [int] NULL, " +
                "[idbanco][int] NULL, " +
                "[idtipo][int] NULL, " +
                "[numero][varchar] (30) NULL, " +
                "[habilitado][int] NULL, " +
                "CONSTRAINT[PK_bancoprov] PRIMARY KEY CLUSTERED " +
                "([id] ASC)  " +
                "WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY] " +
                ") ON[PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablabancoprov);

            //****************************************************************  CREAR TABLA bancos

            string queryCrearTablabancos = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[bancos] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[nombre] [varchar](50) NULL, " +
                "[habilitado] [int] NULL, " +
                "CONSTRAINT [PK_bancos] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablabancos);

            //****************************************************************  CREAR TABLA cierres

            string queryCrearTablacierres = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[cierres] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[Mesproc] [int] NULL, " +
                "[Anoproc] [int] NULL, " +
                "[Usuario] [varchar](50) NULL, " +
                "[Fecha] [date] NULL, " +
                "[Estado] [varchar](2) NULL, " +
                "CONSTRAINT [PK_cierres] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablacierres);

            //****************************************************************  CREAR TABLA clientes

            string queryCrearTablaclientes = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[clientes] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[idpersona] [int] NULL, " +
                "[idregion] [int] NULL, " +
                "[idcomuna] [int] NULL, " +
                "[direccion] [varchar](max) NULL, " +
                "[tlf] [varchar](50) NULL, " +
                "[email] [varchar](max) NULL, " +
                "[giro] [text] NULL, " +
                "[habilitado] [int] NULL, " +
                "CONSTRAINT [PK_clientes] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaclientes);

            //****************************************************************  CREAR TABLA comprasVentas

            string queryCrearTablacomprasVentas = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[comprasVentas] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[rut] [varchar](20) NULL, " +
                "[operacion] [varchar](50) NULL, " +
                "[periodoTrib] [int] NULL, " +
                "[fecCarga] [date] NULL, " +
                "[fecEmis] [date] NULL, " +
                "[tipoDoc] [varchar](50) NULL, " +
                "[folio] [varchar](50) NULL, " +
                "[rutContraparte] [varchar](20) NULL, " +
                "[razonSoc] [varchar](150) NULL, " +
                "[tipoCompVent] [int] NULL, " +
                "[numInterno] [varchar](50) NULL, " +
                "[nacionalidad] [varchar](50) NULL, " +
                "[CodSucur] [varchar](50) NULL, " +
                "[IndSinCosto] [varchar](10) NULL, " +
                "[montoExcen] [decimal](18, 2) NULL, " +
                "[montoNeto] [decimal](18, 2) NULL, " +
                "[montoIva] [decimal](18, 2) NULL, " +
                "[codIvaNoRec] [varchar](10) NULL, " +
                "[montoIvaNoRec] [decimal](18, 2) NULL, " +
                "[montoTotal] [decimal](18, 2) NULL, " +
                "[montoIvaPropio] [decimal](18, 0) NULL, " +
                "[montoIvaTerceros] [decimal](18, 2) NULL, " +
                "[ivaUsoComun] [decimal](18, 2) NULL, " +
                "[otrosImpSinCred] [decimal](18, 2) NULL, " +
                "[tabacos] [varchar](50) NULL, " +
                "[montoNoFacturable] [decimal](18, 2) NULL, " +
                "[montoPeriodo] [decimal](18, 2) NULL, " +
                "[montoActFijo] [decimal](18, 2) NULL, " +
                "[montoIvaActFijo] [decimal](18, 2) NULL, " +
                "[numRecepExtrj] [varchar](50) NULL, " +
                "[tipoDocRef] [varchar](50) NULL, " +
                "[folioDocRef] [varchar](50) NULL, " +
                "[codSii] [nchar](10) NULL, " +
                "[tipoImp] [varchar](50) NULL, " +
                "[indServ] [varchar](50) NULL, " +
                "[habilitado] [int] NOT NULL, " +
                "[usuario] [int] NULL, " +
                "[compVenta] [int] NULL, " +
                "CONSTRAINT [PK_comprasVentas] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablacomprasVentas);

            //****************************************************************  CREAR TABLA comprobantes

            string queryCrearTablacomprobantes = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[comprobantes] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[numero] [varchar](20) NULL, " +
                "[fcomprob] [date] NULL, " +
                "[fcontab] [date] NULL, " +
                "[tipocomprob] [int] NULL, " +
                "[observ] [text] NULL, " +
                "[habilitado] [int] NULL, " +
                "CONSTRAINT [PK_comprobantes] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablacomprobantes);

            //****************************************************************  CREAR TABLA comprobantesdet

            string queryCrearTablacomprobantesdet = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[comprobantesdet] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[idcomprob] [int] NOT NULL, " +
                "[cuenta] [varchar](120) NULL, " +
                "[debe] [numeric](18, 2) NULL, " +
                "[haber] [numeric](18, 2) NULL, " +
                "[notas] [text] NULL, " +
                "CONSTRAINT [PK_comprobantesdet] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablacomprobantesdet);

            //****************************************************************  CREAR TABLA comprobantesflag

            string queryCrearTablacomprobantesflag = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[comprobantesflag] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[idcomprobdet] [int] NOT NULL, " +
                "[cod_rut] [varchar](20) NULL, " +
                "[auxi_001] [varchar](20) NULL, " +
                "[tip_docu] [varchar](50) NULL, " +
                "[num_docu] [varchar](20) NULL, " +
                "[fec_emis] [date] NULL, " +
                "[fec_venc] [date] NULL, " +
                "[cod_impu] [varchar](5) NULL, " +
                "[num_cont] [varchar](20) NULL, " +
                "[mto_dola] [decimal](18, 2) NULL, " +
                "[cod_rcaj] [varchar](10) NULL, " +
                "[cod_remu] [varchar](20) NULL, " +
                "[tipo_aux] [int] NULL, " +
                "[aux] [varchar](50) NULL, " +
                "CONSTRAINT [PK_comprobantesflag] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablacomprobantesflag);

            //****************************************************************  CREAR TABLA comunas

            string queryCrearTablacomunas = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[comunas] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[idregion] [int] NULL, " +
                "[nombre] [varchar](50) NULL, " +
                "CONSTRAINT [PK_comunas] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablacomunas);

            //****************************************************************  CREAR TABLA cuentas

            string queryCrearTablacuentas = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[cuentas] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[idgrupo] [int] NOT NULL, " +
                "[nombre] [varchar](max) NOT NULL, " +
                "[codigo] [nchar](2) NULL, " +
                "[contab] [varchar](3) NULL, " +
                "CONSTRAINT [PK_cuentas] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablacuentas);

            //****************************************************************  CREAR TABLA depreciacion

            string queryCrearTabladepreciacion = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[depreciacion]" +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[num_activo] [int] NULL, " +
                "[num_sufijo] [int] NULL, " +
                "[fec_deprec] [date] NULL, " +
                "[valor] [decimal](18, 2) NULL, " +
                "[tipo_reg] [char](1) NULL, " +
                "[habilitado] [int] NULL, " +
                "CONSTRAINT [PK_depreciacion] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTabladepreciacion);

            //****************************************************************  CREAR TABLA empresas

            string queryCrearTablaempresas = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[empresas] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[rut] [varchar](20) NOT NULL, " +
                "[razonsocial] [varchar](80) NULL, " +
                "[fantasia] [varchar](50) NULL, " +
                "[giro] [text] NULL, " +
                "[idpais] [int] NULL, " +
                "[idregion] [int] NULL, " +
                "[idcomuna] [int] NULL, " +
                "[idrepresentante] [int] NULL, " +
                "[idencargado] [int] NULL, " +
                "[email] [varchar](50) NULL, " +
                "[obs] [text] NULL, " +
                "[habilitado] [int] NOT NULL, " +
                "[conexion] [varchar](50) NULL, " +
                "[usuario] [varchar](50) NULL, " +
                "[contra] [varchar](50) NULL, " +
                "[basedatos] [varchar](50) NULL, " +
                "[padre] [varchar](20) NULL, " +
                "CONSTRAINT [PK_empresas] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaempresas);

            //****************************************************************  CREAR TABLA errores

            string queryCrearTablaerrores = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[errores] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[fecha] [date] NULL, " +
                "[hora] [time](7) NULL, " +
                "[empresa] [varchar](20) NULL, " +
                "[usuario] [int] NULL, " +
                "[mensaje] [text] NULL, " +
                "[trace] [text] NULL, " +
                "CONSTRAINT [PK_errores] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaerrores);

            //****************************************************************  CREAR TABLA gastosimg

            string queryCrearTablagastosimg = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[gastosimg] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[idgasto] [int] NOT NULL, " +
                "[imagen] [varchar](500) NULL, " +
                "[habilitado] [int] NOT NULL, " +
                "CONSTRAINT [PK_gastosimg] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablagastosimg);

            //****************************************************************  CREAR TABLA gastosregistro

            string queryCrearTablagastosregistro = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[gastosregistro] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[tipogasto] [int] NULL, " +
                "[fecha] [date] NULL, " +
                "[rutprov] [varchar](20) NULL, " +
                "[razonsocial] [varchar](250) NULL, " +
                "[motivogasto] [text] NULL, " +
                "[tipodoc] [int] NULL, " +
                "[docnro] [varchar](20) NULL, " +
                "[monto] [decimal](18, 2) NULL, " +
                "[observ] [text] NULL, " +
                "[habilitado] [int] NULL, " +
                "[aprobado] [int] NULL, " +
                "[motivo] [varchar](250) NULL, " +
                "[origen] [int] NULL, " +
                "[usuario] [int] NULL, " +
                "[fregistro] [date] NULL, " +
                "[hregistro] [time](7) NOT NULL, " +
                "[longitug] [decimal](18, 10) NULL, " +
                "[latitud] [decimal](18, 10) NULL, " +
                "[dispositivo] [varchar](50) NULL, " +
                "[imagen] [image] NULL, " +
                "CONSTRAINT [PK_gastosregistro] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablagastosregistro);

            //****************************************************************  CREAR TABLA grupos

            string queryCrearTablagrupos = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[grupos] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[Nombre] [varchar](max) NOT NULL, " +
                "[codigo] [nchar](2) NULL, " +
                "CONSTRAINT [PK_grupos] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablagrupos);

            //****************************************************************  CREAR TABLA moneda

            string queryCrearTablamoneda = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[moneda] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[nombre] [varchar](50) NULL, " +
                "[simbolo] [varchar](50) NULL, " +
                "[factor] [numeric](7, 2) NULL, " +
                "CONSTRAINT [PK_moneda] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablamoneda);

            //****************************************************************  CREAR TABLA movcaja

            string queryCrearTablamovcaja = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[movcaja] " +
                "([id][int] NULL, " +
                "[tipo][varchar] (10) NULL, " +
                "[descripcion][varchar] (60) NULL, " +
                "[egreso][int] NULL " +
                ") ON[PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablamovcaja);

            //****************************************************************  CREAR TABLA opcionesMenu

            string queryCrearTablaopcionesMenu = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[opcionesMenu] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[padre] [int] NULL, " +
                "[obs] [varchar](50) NULL, " +
                "[texto] [varchar](50) NULL, " +
                "[url] [varchar](50) NULL, " +
                "[orden] [int] NULL, " +
                "[habilitado] [varchar](1) NULL, " +
                "CONSTRAINT [PK_opcionesMenu] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaopcionesMenu);

            //****************************************************************  CREAR TABLA paises

            string queryCrearTablapaises = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[paises] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[nombre] [varchar](50) NULL, " +
                "[moneda] [varchar](50) NULL, " +
                "[simbolo] [varchar](10) NULL, " +
                "CONSTRAINT [PK_paises] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablapaises);

            //****************************************************************  CREAR TABLA perfiles

            string queryCrearTablaperfiles = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[perfiles] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[nombrePerfil] [varchar](50) NULL, " +
                "[habilitado] [int] NULL, " +
                "CONSTRAINT [PK_perfiles] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaperfiles);

            //****************************************************************  CREAR TABLA perfilMenu

            string queryCrearTablaperfilMenu = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[perfilMenu] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[idPerfil] [int] NOT NULL, " +
                "[idMenu] [int] NOT NULL, " +
                "CONSTRAINT [PK_perfilMenu] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaperfilMenu);

            //****************************************************************  CREAR TABLA personas

            string queryCrearTablapersonas = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[personas] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[rut] [varchar](20) NULL, " +
                "[nombres] [varchar](150) NULL, " +
                "[apellidos] [varchar](150) NULL, " +
                "[email] [varchar](80) NULL, " +
                "[tlf] [varchar](50) NULL, " +
                "CONSTRAINT [PK_personas] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablapersonas);

            //****************************************************************  CREAR TABLA plancuentas

            string queryCrearTablaplancuentas = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[plancuentas] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[nombreplan] [varchar](50) NULL, " +
                "[fini] [date] NULL, " +
                "[ffin] [date] NULL, " +
                "[creacion] [date] NULL, " +
                "CONSTRAINT [PK_plancuentas] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaplancuentas);

            //****************************************************************  CREAR TABLA plancuentasdet

            string queryCrearTablaplancuentasdet = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[plancuentasdet] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[idplan] [int] NULL, " +
                "[idgrupo] [int] NULL, " +
                "[idcuenta] [int] NULL, " +
                "[idsubcuenta] [int] NULL, " +
                "[idauxiliar] [int] NULL, " +
                "[codigo] [int] NULL, " +
                "[glosa] [varchar](50) NULL, " +
                "[idmoneda] [int] NULL, " +
                "[impuesto] [int] NULL, " +
                "[emision] [int] NULL, " +
                "[vencimiento] [int] NULL, " +
                "[calce] [int] NULL, " +
                "[realcaja] [int] NULL, " +
                "[remuneracion] [int] NULL, " +
                "[contrato] [int] NULL, " +
                "[conversion] [int] NULL, " +
                "[conciliacion] [int] NULL, " +
                "[cheque] [int] NULL, " +
                "[ctacte] [int] NULL, " +
                "[ctacod] [int] NULL, " +
                "[pago] [int] NULL, " +
                "[gastos] [int] NULL, " +
                "[faena] [int] NULL, " +
                "[contab] [int] NULL, " +
                "[codsii] [varchar](50) NULL, " +
                "[gananperd] [int] NULL, " +
                "[actpascap] [int] NULL, " +
                "[habilitado] [int] NULL, " +
                "[flg_ingres] [varchar](1) NULL, " +
                "[flg_gastos] [varchar](1) NULL, " +
                "CONSTRAINT [PK_plancuentasdet] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaplancuentasdet);

            //****************************************************************  CREAR TABLA proveedores

            string queryCrearTablaproveedores = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[proveedores] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[idpersona] [int] NULL, " +
                "[idregion] [int] NULL, " +
                "[idcomuna] [int] NULL, " +
                "[direccion] [varchar](max) NULL, " +
                "[tlf] [varchar](50) NULL, " +
                "[email] [varchar](max) NULL, " +
                "[giro] [text] NULL, " +
                "[habilitado] [int] NULL, " +
                "CONSTRAINT [PK_proveedores] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaproveedores);

            //****************************************************************  CREAR TABLA rcv_compras

            string queryCrearTablarcv_compras = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[rcv_compras] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[id_archivo] [int] NULL, " +
                "[tipo_doc] [varchar](5) NULL, " +
                "[tipo_compra] [varchar](50) NULL, " +
                "[rut_prov] [varchar](20) NULL, " +
                "[razon_soc] [varchar](150) NULL, " +
                "[folio] [varchar](50) NULL, " +
                "[fecha_docto] [date] NULL, " +
                "[fecha_recep] [datetime] NULL, " +
                "[fecha_acuse] [date] NULL, " +
                "[monto_exento] [decimal](18, 2) NULL, " +
                "[monto_neto] [decimal](18, 2) NULL, " +
                "[iva_recuperable] [decimal](18, 2) NULL, " +
                "[iva_no_recuperable] [decimal](18, 2) NULL, " +
                "[cod_iva_nor_recup] [varchar](5) NULL, " +
                "[monto_total] [decimal](18, 2) NULL, " +
                "[monto_neto_act_fijo] [decimal](18, 2) NULL, " +
                "[iva_act_fijo] [decimal](18, 2) NULL, " +
                "[iva_uso_comun] [decimal](18, 2) NULL, " +
                "[imp_sin_derecho_cred] [decimal](18, 2) NULL, " +
                "[iva_no_retenido] [decimal](18, 2) NULL, " +
                "[tabacos_puros] [varchar](50) NULL, " +
                "[tabacos_cigarrillos] [varchar](50) NULL, " +
                "[tabacos_elaborados] [varchar](50) NULL, " +
                "[nce_nde_fact_compra] [decimal](18, 2) NULL, " +
                "[codigo_otro_imp] [varchar](5) NULL, " +
                "[valor_otro_imp] [decimal](18, 2) NULL, " +
                "[tasa_otro_imp] [decimal](18, 2) NULL, " +
                "CONSTRAINT [PK_rcv_compras] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablarcv_compras);

            //****************************************************************  CREAR TABLA rcv_ventas

            string queryCrearTablarcv_ventas = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[rcv_ventas] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[id_archivo] [int] NULL, " +
                "[nro] [int] NULL, " +
                "[tipo_venta] [varchar](50) NULL, " +
                "[rut_cliente] [varchar](20) NULL, " +
                "[razon_social] [varchar](150) NULL, " +
                "[folio] [varchar](20) NULL, " +
                "[fecha_docto] [date] NULL, " +
                "[fecha_recep] [datetime] NULL, " +
                "[fecha_acuse_recib] [datetime] NULL, " +
                "[fecha_reclamo] [date] NULL, " +
                "[monto_exento] [decimal](18, 2) NULL, " +
                "[monto_neto] [decimal](18, 2) NULL, " +
                "[monto_iva] [decimal](18, 2) NULL, " +
                "[monto_total] [decimal](18, 2) NULL, " +
                "[iva_retenido_total] [decimal](18, 2) NULL, " +
                "[iva_retenido_parcial] [decimal](18, 2) NULL, " +
                "[ina_no_retenido] [decimal](18, 2) NULL, " +
                "[iva_propio] [decimal](18, 2) NULL, " +
                "[iva_terceros] [decimal](18, 2) NULL, " +
                "[rut_emisor_liquid_fact] [varchar](20) NULL, " +
                "[neto_comision_liquid_fact] [decimal](18, 2) NULL, " +
                "[exent_comision_liquid_fact] [decimal](18, 2) NULL, " +
                "[iva_comision_liquid_fact] [decimal](18, 2) NULL, " +
                "[iva_fuera_plazo] [decimal](18, 2) NULL, " +
                "[tipo_docto_ref] [varchar](5) NULL, " +
                "[folio_docto_ref] [varchar](20) NULL, " +
                "[num_ident_receptor_extranj] [varchar](20) NULL, " +
                "[nacionalidad_receptor_extranj] [varchar](50) NULL, " +
                "[credito_empresa_construc] [decimal](18, 2) NULL, " +
                "[impto_zona_franca] [decimal](18, 2) NULL, " +
                "[garanti_dep_envases] [decimal](18, 2) NULL, " +
                "[indicador_venta_sin_costo] [varchar](5) NULL, " +
                "[indicador_serv_periodico] [varchar](5) NULL, " +
                "[monto_no_facturable] [decimal](18, 2) NULL, " +
                "[total_monto_periodo] [decimal](18, 2) NULL, " +
                "[venta_pasaje_transp_nac] [varchar](50) NULL, " +
                "[venta_pasaje_transp_internac] [varchar](50) NULL, " +
                "[numero_interno] [varchar](50) NULL, " +
                "[codigo_sucursal] [varchar](20) NULL, " +
                "[nce_nde_fact_compra] [varchar](50) NULL, " +
                "[cod_otro_imp] [varchar](5) NULL, " +
                "[valor_otro_imp] [decimal](18, 2) NULL, " +
                "[tasa_otro_imp] [decimal](18, 2) NULL, " +
                "CONSTRAINT [PK_rcv_ventas] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablarcv_ventas);

            //****************************************************************  CREAR TABLA regcaja

            string queryCrearTablaregcaja = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[regcaja] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[tip_docume] [varchar](10) NULL, " +
                "[num_docume] [int] NULL, " +
                "[fac_docume] [date] NULL, " +
                "[cod_cuenta] [varchar](10) NULL, " +
                "[tip_auxiliar] [int] NULL, " +
                "[cod_auxiliar] [varchar](9) NULL, " +
                "[num_calce] [int] NULL, " +
                "[fec_emision] [date] NULL, " +
                "[fec_vencim] [date] NULL, " +
                "[cod_rcaja] [varchar](10) NULL, " +
                "[monto] [decimal](18, 2) NULL, " +
                "[nro_cuota] [int] NULL, " +
                "[observacion] [varchar](150) NULL, " +
                "[fec_contable] [date] NULL, " +
                "CONSTRAINT [PK_regcaja] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaregcaja);

            //****************************************************************  CREAR TABLA regiones

            string queryCrearTablaregiones = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[regiones] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[idpais] [int] NULL, " +
                "[nombre] [varchar](50) NULL, " +
                "CONSTRAINT [PK_regiones] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaregiones);

            //****************************************************************  CREAR TABLA subcuenta

            string queryCrearTablasubcuenta = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[subcuenta] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[idcuenta] [int] NULL, " +
                "[nombre] [varchar](max) NULL, " +
                "[codigo] [nchar](2) NULL, " +
                "[contab] [varchar](5) NULL, " +
                "CONSTRAINT [PK_subcuenta] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablasubcuenta);

            //****************************************************************  CREAR TABLA tipoact

            string queryCrearTablatipoact = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[tipoact] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[tipo] [int] NULL, " +
                "[descripcion] [varchar](50) NULL, " +
                "[habilitado] [int] NULL, " +
                "CONSTRAINT [PK_tipoact] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablatipoact);

            //****************************************************************  CREAR TABLA tipocta

            string queryCrearTablatipocta = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[tipocta] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[nombre] [varchar](50) NULL, " +
                "[habilitado] [int] NULL, " +
                "CONSTRAINT [PK_tipocta] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablatipocta);

            //****************************************************************  CREAR TABLA tipodep

            string queryCrearTablatipodep = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[tipodep] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[descripcion] [varchar](50) NULL, " +
                "[periodo] [int] NULL, " +
                "[habilitado] [int] NULL, " +
                "CONSTRAINT [PK_tipodep] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablatipodep);

            //****************************************************************  CREAR TABLA tipoImpuesto

            string queryCrearTablatipoImpuesto = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[tipoImpuesto] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[nombre] [varchar](50) NULL, " +
                "[porc] [decimal](18, 2) NULL, " +
                "[habilitado] [int] NULL, " +
                "CONSTRAINT [PK_tipoImpuesto] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablatipoImpuesto);

            //****************************************************************  CREAR TABLA TiposAux

            string queryCrearTablaTiposAux = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[TiposAux] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[nombre] [varchar](150) NULL, " +
                "[habilitado] [int] NOT NULL, " +
                "CONSTRAINT [PK_Aux] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablaTiposAux);

            //****************************************************************  CREAR TABLA tiposdocs

            string queryCrearTablatiposdocs = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[tiposdocs] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[nombre] [varchar](80) NULL, " +
                "[habilitado] [int] NULL, " +
                "CONSTRAINT [PK_tiposdocs] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablatiposdocs);

            //****************************************************************  CREAR TABLA usuarioperfil

            string queryCrearTablausuarioperfil = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[usuarioperfil] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[idusuario] [int] NOT NULL, " +
                "[idperfil] [int] NOT NULL, " +
                "[habilitado] [int] NOT NULL, " +
                "CONSTRAINT [PK_usuarioperfil] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablausuarioperfil);

            //****************************************************************  CREAR TABLA usuarios

            string queryCrearTablausuarios = "USE [contable_" + Rut + "] " +
                "CREATE TABLE [dbo].[usuarios] " +
                "([id] [int] IDENTITY(1,1) NOT NULL, " +
                "[nombreUsu] [varchar](50) NULL, " +
                "[idPerfil] [int] NULL, " +
                "[contra] [varchar](max) NULL, " +
                "[habilitado] [int] NOT NULL, " +
                "[email] [varchar](80) NULL, " +
                "[nombres] [varchar](50) NULL, " +
                "[apellidos] [varchar](50) NULL, " +
                "[whatsapp] [varchar](20) NULL, " +
                "CONSTRAINT [PK_usuarios] PRIMARY KEY CLUSTERED  " +
                "([id] ASC)  " +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY] " +
                ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

            EjecutarConsultaSQL(queryCrearTablausuarios);


            //****************************************************************
            //****************************************************************  CREAR CLAVES FORANEAS ENTRE TABLAS
            //****************************************************************


            string queryForaneasOpcionesMenu = "USE [contable_" + Rut + "] " +
                    "ALTER TABLE [dbo].[opcionesMenu] ADD  CONSTRAINT [DF_opcionesMenu_habilitado]  DEFAULT ((1)) FOR [habilitado]";

            EjecutarConsultaSQL(queryForaneasOpcionesMenu);


            string queryForaneasPerfiles = "USE [contable_" + Rut + "] " +
                    "ALTER TABLE [dbo].[perfiles] ADD  CONSTRAINT [DF_perfiles_habilitado]  DEFAULT ((1)) FOR [habilitado]";

            EjecutarConsultaSQL(queryForaneasPerfiles);


            string queryForaneasPerfilMenu = "USE [contable_" + Rut + "] " +
                    "ALTER TABLE [dbo].[perfilMenu]  WITH CHECK ADD  CONSTRAINT [FK_perfilMenu_opcionesMenu] FOREIGN KEY([idMenu]) REFERENCES [dbo].[opcionesMenu] ([id]); " +
                    "ALTER TABLE [dbo].[perfilMenu] CHECK CONSTRAINT [FK_perfilMenu_opcionesMenu]; " +
                    "ALTER TABLE [dbo].[perfilMenu]  WITH CHECK ADD  CONSTRAINT [FK_perfilMenu_perfiles] FOREIGN KEY([idPerfil]) REFERENCES [dbo].[perfiles] ([id]); " +
                    "ALTER TABLE [dbo].[perfilMenu] CHECK CONSTRAINT [FK_perfilMenu_perfiles]; ";

            EjecutarConsultaSQL(queryForaneasPerfilMenu);


            string queryForaneasUsuarios = "USE [contable_" + Rut + "] " +
                    "ALTER TABLE [dbo].[usuarios] ADD  CONSTRAINT [DF_usuarios_habilitado]  DEFAULT ((1)) FOR [habilitado]; " +
                    "ALTER TABLE [dbo].[usuarios]  WITH CHECK ADD  CONSTRAINT [FK_usuarios_perfiles] FOREIGN KEY([idPerfil]) REFERENCES [dbo].[perfiles] ([id]); " +
                    "ALTER TABLE [dbo].[usuarios] CHECK CONSTRAINT [FK_usuarios_perfiles]";

            EjecutarConsultaSQL(queryForaneasUsuarios);


            //****************************************************************
            //****************************************************************  INSERTAR VALORES POR DEFECTO
            //****************************************************************

            string queryValoresOpcionesMenu = "insert into [contable_" + Rut + "].[dbo].[opcionesMenu] ([padre],[obs],[texto],[url],[orden],[habilitado]) " +
                "SELECT [padre],[obs],[texto],[url],[orden],[habilitado] " +
                "FROM[contable_181029060].[dbo].[opcionesMenu]";

            EjecutarConsultaSQL(queryValoresOpcionesMenu);


            string queryValoresPaises = "insert into [contable_" + Rut + "].[dbo].[paises] ([nombre],[moneda],[simbolo]) " +
                "SELECT [nombre],[moneda],[simbolo] " +
                "FROM[contable_181029060].[dbo].[paises]";

            EjecutarConsultaSQL(queryValoresPaises);


            string queryValoresRegiones = "insert into [contable_" + Rut + "].[dbo].[regiones] ([idpais],[nombre]) " +
                "SELECT [idpais],[nombre] " +
                "FROM[contable_181029060].[dbo].[regiones]";

            EjecutarConsultaSQL(queryValoresRegiones);


            string queryValoresComunas = "insert into [contable_" + Rut + "].[dbo].[regiones] ([idregion],[nombre]) " +
                "SELECT [idregion],[nombre] " +
                "FROM[contable_181029060].[dbo].[regiones]";

            EjecutarConsultaSQL(queryValoresComunas);


            return true;
        }



    }
}
namespace RRHH.Models.ViewModels
{
    public class ListaSelecionar
    {

        public string codigo;
        public string descripcion;
    }
}

