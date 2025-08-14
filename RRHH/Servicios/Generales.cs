using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;
using System.Diagnostics.Eventing.Reader;


namespace RRHH.Servicios
{
    public class Generales
    {

        Funciones f = new Funciones();
        private Correos correos = new Correos();
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static Random random = new Random();


        public PersonasBaseVM BuscaPersona(string rut, string BD_Cli)
        {
            rut = rut.Replace("-", "");
            rut = rut.Replace(" ", "");
            string rutg=null;
            f.EjecutarConsultaSQLCli("select * from personas where Rut='" + rut + "' ", BD_Cli);
            PersonasBaseVM persona = new PersonasBaseVM();
            List<PersonasBaseVM> opcionesList = new List<PersonasBaseVM>();
            persona.Nombres = "No existe persona";
            if (f.Tabla.Rows.Count > 0)
            {


                opcionesList = (from DataRow dr in f.Tabla.Rows
                                select new PersonasBaseVM()
                                {
                                    Id = int.Parse(dr["id"].ToString()),
                                    Rut = dr["rut"].ToString(),
                                    Nombres = dr["nombres"].ToString(),
                                    Apellidos = dr["apellidos"].ToString(),
                                    Email = dr["email"].ToString(),
                                    Tlf = dr["tlf"].ToString()
                                }).ToList();
                if (opcionesList.Count > 0)
                {
                    persona = opcionesList.First();
                }
                return persona;
            }
            else
            {
                return null;
            }
        }
        public ContratosBaseVM BuscaContrato(string rut, string fecha, string BD_Cli)
        {
            rut = rut.Replace("-", "");
            rut = rut.Replace(" ", "");
            string rutg = null;
            f.EjecutarConsultaSQLCli("select contratos.* from contratos, personas where rut='" + rut + "' and contratos.idPersona = personas.id " +
                                      " and inicio <= '"+fecha+"' and termino>= '"+fecha+"' ", BD_Cli);
            ContratosBaseVM contrato = new ContratosBaseVM();
            List<ContratosBaseVM> opcionesList = new List<ContratosBaseVM>();
            if (f.Tabla.Rows.Count > 0)
            {
                opcionesList = (from DataRow dr in f.Tabla.Rows
                                select new ContratosBaseVM()
                                {
                                    id = int.Parse(dr["id"].ToString()),
                                    contrato = dr["contrato"].ToString(),
                                    inicio = dr["inicio"].ToString(),
                                    termino = dr["termino"].ToString(),
                                    idtipocontrato = int.Parse(dr["idTipoContrato"].ToString()),
                                    idfaena = int.Parse(dr["idFaena"].ToString()),
                                    idcargo = int.Parse(dr["idCargo"].ToString()),
                                    idcentrocosto = int.Parse(dr["idCentroCosto"].ToString()),
                                    sueldobase = dr["SueldoBase"].ToString(),
                                    idafptrab = int.Parse(dr["idAfpTrab"].ToString()),
                                    idbancotrab = int.Parse(dr["idBancoTrab"].ToString()),
                                    idisapretrab = int.Parse(dr["idIsapreTrab"].ToString())
                                }).ToList();
                if (opcionesList.Count > 0)
                {
                    contrato = opcionesList.First();
                }
                return contrato;
            }
            else
            {
                return null;
            }
        }
        public Detdias BuscaDias(int id, string BD_Cli)
        {
            Detdias dias = new Detdias();
            f.EjecutarConsultaSQLCli("select * from vacacionesDias where id=" + id + " ", BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                DataRow dia = f.Tabla.Rows[0];
                dias.diaslegales = int.Parse(dia["diasLegales"].ToString());
                dias.diascontrato = int.Parse(dia["diasContrato"].ToString());
                dias.diasadministrativos = int.Parse(dia["diasAdministrativos"].ToString());
                dias.diasfaena = int.Parse(dia["diasFaena"].ToString());
                dias.diasespeciales = int.Parse(dia["diasEspeciales"].ToString());
                dias.diasotros = int.Parse(dia["diasOtros"].ToString());
            }
            return dias;
        }

        public UsuarioVM BuscaUsuario(int idusuario)
        {
            f.EjecutarConsultaSQLCli("select * from usuarios where id='" + idusuario + "' ", "contable");
            UsuarioVM usuario = new UsuarioVM();
            List<UsuarioVM> opcionesList = new List<UsuarioVM>();
            if (f.Tabla.Rows.Count > 0)
            {
                opcionesList = (from DataRow dr in f.Tabla.Rows
                                select new UsuarioVM()
                                {
                                    id = int.Parse(dr["id"].ToString()),
                                    usuraio = dr["nombreUsu"].ToString(),
                                    nombre = dr["nombres"].ToString(),
                                    apellido = dr["apellidos"].ToString(),
                                    email = dr["email"].ToString(),
                                    perfil = dr["idPerfil"].ToString()
                                }).ToList();
                if (opcionesList.Count > 0)
                {
                    usuario = opcionesList.First();
                    return usuario;
                }
            }
            usuario.nombre = "No existe usuario";
            return usuario;
        }
        public bool EsFeriado(DateTime fecha,int idpais)
        {
            bool feriado = false;
            string fechastr = fecha.ToString("yyyy'-'MM'-'dd"); 
            f.EjecutarConsultaSQLCli("select * from feriados where idpais=" + idpais + " AND fecha ='"+fechastr+"' ", "remuneracion");
            if (f.Tabla.Rows.Count > 0)
            {
                feriado = true;
            }
            return feriado;
        }
        public int VacacionesEspecialesTrabajador(string rut,string fecha, string BD_Cli)
        {
            int dias = 0;
             f.EjecutarConsultaSQLCli("select * from vacacionesEspeciales where rutTrabajador='" + rut + "' AND fecha <='" + fecha + "' ", BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                DataRow vaca = f.Tabla.Rows[0];
                dias = int.Parse(vaca["dias"].ToString());
            }
            return dias;
        }
        public string BuscaCuenta(string cuenta, string BD_Cli)
        {
            string descripcion="No existe cuenta";
            BD_Cli = BD_Cli.Replace("remuneracion", "contable");
            f.EjecutarConsultaSQLCli("select * from plancuentasdet where habilitado= 1 AND contab= '" + cuenta + "' ", BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                DataRow cta = f.Tabla.Rows[0];
                descripcion = cta["glosa"].ToString();
            }
            return descripcion;
        }
        public int SindicatoTrabajador(string rut, string fecha, string BD_Cli)
        {
            int sindicato = 0;
            f.EjecutarConsultaSQLCli("select * from sindicatoTrabajador where rutTrabajador='" + rut + "' AND fecha <='" + fecha + "' ", BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                DataRow sind = f.Tabla.Rows[0];
                sindicato = int.Parse(sind["sindicato"].ToString());
            }
            return sindicato;
        }

        public string BuscaDestinatario(string seccion, string BD_Cli)
        {
            string destinatario = null;
            f.EjecutarConsultaSQLCli("select * from destinatarios where Seccion='" + seccion + "' ", BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                DataRow dest = f.Tabla.Rows[0];
                destinatario = dest["Correos"].ToString();
;
            }
            return destinatario;
        }
        public string BuscaConcepto(int concepto, string BD_Cli)
        {
            string descripcion = null;
            if(concepto <100)
            {
                f.EjecutarConsultaSQLCli("select * from haberes where haber=" + concepto, BD_Cli);
                if (f.Tabla.Rows.Count > 0)
                {
                    DataRow haber = f.Tabla.Rows[0];
                    descripcion = haber["Descripcion"].ToString();
                    ;
                }
            }
            else
            {
                    f.EjecutarConsultaSQLCli("select * from descuentos where descuento=" + concepto, BD_Cli);
                    if (f.Tabla.Rows.Count > 0)
                    {
                        DataRow dscto = f.Tabla.Rows[0];
                        descripcion = dscto["Descripcion"].ToString();
                        ;
                    }
                    else
                    {
                        if (concepto == 910) descripcion = "LEY.SOC.PENSIÓN";
                        if (concepto == 921) descripcion = "LEY.SOC. SALUD";
                        if (concepto == 916) descripcion = "LEY.SOC.CESANTIA";
                        if (concepto == 990) descripcion = "IMPUESTO UNICO";
                        if (concepto == 910) descripcion = "LEY.SOC.PENSIÓN";
                        if (concepto == 910) descripcion = "LEY.SOC.PENSIÓN";
                        if (concepto == 910) descripcion = "LEY.SOC.PENSIÓN";
                        if (concepto == 910) descripcion = "LEY.SOC.PENSIÓN";
                        if (concepto == 999) descripcion = "SALDO LIQUIDO";
                        if (concepto == 2400) descripcion = "LEY.SOC.CESANTIA EMPRESA";
                        if (concepto == 2401) descripcion = "LEY.SOC.SEGURO";
                        if (concepto == 2402) descripcion = "MUTUAL DE SEGURIDAD";
                        if (concepto == 2404) descripcion = "SEGURO APORTE EMPLEADOR";
                        if (concepto == 2405) descripcion = "APORTE EMPLEADOR LEY SANNA";
                        if (concepto == 2421) descripcion = "LEY.SOC. 7 %";
                        if (concepto == 2212) descripcion = "PAGAR ISAPRE";
                    }
            }
            return descripcion;
        }
        public detempresa BuscaEmpresa(string rut)
		{
            detempresa empresa = new detempresa();
			f.EjecutarConsultaSQLCli("select * from empresas where rut='" + rut + "' ", "contable");
			if (f.Tabla.Rows.Count > 0)
			{
				var opcionesList = (from DataRow dr in f.Tabla.Rows
								select new 
								{
									id = int.Parse(dr["id"].ToString()),
									rut = dr["rut"].ToString(),
									razonsocial = dr["razonsocial"].ToString(),
									fantasia = dr["fantasia"].ToString(),
									email = dr["email"].ToString(),
									direccion = dr["direccion"].ToString(),
									idregion = int.Parse(dr["idregion"].ToString()),
									idcomuna = int.Parse(dr["idcomuna"].ToString()),
									idfaena = int.Parse(dr["idFaena"].ToString())
								}).ToList();
				if (opcionesList.Count > 0)
				{
					var unico = opcionesList.First();
                    empresa.id = unico.id;
                    empresa.rut = unico.rut.Substring(0,unico.rut.Length-1)+"-"+unico.rut.Substring(unico.rut.Length-1,1);
                    empresa.razonsocial = unico.razonsocial;
                    empresa.fantasia = unico.fantasia;
                    empresa.email = unico.email;
                    empresa.direccion = unico.direccion;
                    empresa.region = BuscaRegion(unico.idregion);
					empresa.comuna = BuscaComuna(unico.idcomuna);
                    
					return empresa;
				}
			}
			empresa.razonsocial = "No existe usuario";
			return empresa;
		}
        public string BuscaPais(int pais)
        {
            string nombre = "";
            f.EjecutarConsultaSQLCli("select *  from paises where id =" + pais, "contable");
            if (f.Tabla.Rows.Count > 0)
            {
                var opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new
                                    {
                                        nombre = dr["nombre"].ToString()
                                    }).ToList();
                var opcion = opcionesList.First();
                nombre = opcion.nombre;
            }
            return nombre;
        }
        public string BuscaRegion(int region)
		{
            string nombre = "";
			f.EjecutarConsultaSQLCli("select *  from regiones where id =" + region , "contable");
			if (f.Tabla.Rows.Count > 0)
			{
				var opcionesList = (from DataRow dr in f.Tabla.Rows
								select new 
								{
									nombre = dr["nombre"].ToString()
								}).ToList();
				var opcion = opcionesList.First();
                nombre = opcion.nombre;
			}
			return nombre;
		}
		public string BuscaComuna(int comuna)
		{
			string nombre = "";
			f.EjecutarConsultaSQLCli("select *  from comunas where id =" + comuna, "contable");
			if (f.Tabla.Rows.Count > 0)
			{
				var opcionesList = (from DataRow dr in f.Tabla.Rows
									select new
									{
										nombre = dr["nombre"].ToString()
									}).ToList();
				var opcion = opcionesList.First();
				nombre = opcion.nombre;
			}
			return nombre;
		}
        public string BuscaTipoLicencia(int licencia)
        {
            string descripcion = "";
            f.EjecutarConsultaSQLCli("select *  from tipoLicenciaMedica where id =" + licencia, "remuneracion");
            if (f.Tabla.Rows.Count > 0)
            {
                var opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new
                                    {
                                        descripcion = dr["Descripcion"].ToString()
                                    }).ToList();
                var opcion = opcionesList.First();
                descripcion = opcion.descripcion;
            }
            return descripcion;
        }
        public string BuscaTipoCuenta(int idtipo)
        {
            string descripcion = "";
            f.EjecutarConsultaSQLCli("select *  from tiposCuenta where id =" + idtipo, "remuneracion");
            if (f.Tabla.Rows.Count > 0)
            {
                var opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new
                                    {
                                        descripcion = dr["Descripcion"].ToString()
                                    }).ToList();
                var opcion = opcionesList.First();
                descripcion = opcion.descripcion;
            }
            return descripcion;
        }
        public string BuscaTipomedico(int tipo)
        {
            string descripcion = "";
            f.EjecutarConsultaSQLCli("select *  from tipoMedico where id =" + tipo, "remuneracion");
            if (f.Tabla.Rows.Count > 0)
            {
                var opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new
                                    {
                                        descripcion = dr["Descripcion"].ToString()
                                    }).ToList();
                var opcion = opcionesList.First();
                descripcion = opcion.descripcion;
            }
            return descripcion;
        }
        public class SelectActivo
        {
            public string tipo { get; set; }
            public string descripcion { get; set; }
        }

        //public string BuscaBanco(int banco, string BD_Cli)
        //{
        //    string descripcion = 0;
        //    f.EjecutarConsultaSQLCli("select * from bancos where id='" + banco + "' ","remuneracion");
        //    if (f.Tabla.Rows.Count > 0)
        //    {
        //        var opcionesList = (from DataRow dr in f.Tabla.Rows
        //                            select new
        //                            {
        //                                id = int.Parse(dr["id"].ToString()),
        //                                descripcion = dr["descripcion"].ToString()
        //                            }).ToList();
        //        if (opcionesList.Count > 0)
        //        {
        //            descripcion = opcionesList.First().descripcion;
        //        }
        //    }
        //    return descripcion;
        //}
        public BancosTrabajadorBaseVM BuscaBancoTrabajador(int idbancotrab, string BD_Cli)
        {
            BancosTrabajadorBaseVM banco = new BancosTrabajadorBaseVM();

            f.EjecutarConsultaSQLCli("select * from bancosTrabajador where id=" + idbancotrab + " ", BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                var persona = (from DataRow dr in f.Tabla.Rows
                                    select new BancosTrabajadorBaseVM
                                    {
                                        idbanco = int.Parse(dr["idBanco"].ToString()),
                                        idpersona = int.Parse(dr["idPersona"].ToString()),
                                        fechainicio = dr["fechaInicio"].ToString(),
                                        idtipocta = int.Parse(dr["idtipoCuenta"].ToString()),
                                        numerocuenta = dr["numeroCuenta"].ToString(),
                                    }).ToList();
                if (persona.Count > 0)
                {
                    banco = persona.FirstOrDefault();
                    f.EjecutarConsultaSQLCli("select * from tiposCuentas where id=" + banco.idtipocta + " ", "remuneracion");
                    if (f.Tabla.Rows.Count > 0)
                    {
                        var tipolst = (from DataRow dr in f.Tabla.Rows
                                            select new
                                            {
                                                id = int.Parse(dr["id"].ToString()),
                                                descripcion = dr["descripcion"].ToString()
                                            }).ToList();
                        if (tipolst.Count > 0)
                        {
                            banco.descripcionTipo = tipolst.First().descripcion;
                        }
                    }
                    f.EjecutarConsultaSQLCli("select * from bancos where id=" + banco.idbanco + " ", "remuneracion");
                    if (f.Tabla.Rows.Count > 0)
                    {
                        var bancolst = (from DataRow dr in f.Tabla.Rows
                                        select new
                                        {
                                            Id = int.Parse(dr["id"].ToString()),
                                            Descripcion = dr["descripcion"].ToString()
                                        }).ToList();
                        if (bancolst.Count > 0)
                        {
                            banco.descripcionBanco = bancolst.First().Descripcion;
                        }
                    }
                }
            }
            return banco;
        }
        public AfpsTrabajadorBaseVM BuscaAfpTrabajador(int idafptrab, string BD_Cli)
        {
            AfpsTrabajadorBaseVM afp = new AfpsTrabajadorBaseVM();

            f.EjecutarConsultaSQLCli("select * from afpsTrabajador where id=" + idafptrab + " ", BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                var persona = (from DataRow dr in f.Tabla.Rows
                               select new AfpsTrabajadorBaseVM
                               {
                                   id = int.Parse(dr["id"].ToString()),
                                   idpersona = int.Parse(dr["idPersona"].ToString()),
                                   codigoAfp = int.Parse(dr["codigoAfp"].ToString()),
                                   fechainicio = dr["fechaInicio"].ToString(),
                                   tipoApv = dr["tipoApv"].ToString(),
                                   formaApv = dr["formaApv"].ToString(),
                                   apv = decimal.Parse(dr["apv"].ToString())
                               }).ToList();
                if (persona.Count > 0)
                {
                    afp = persona.FirstOrDefault();
                    f.EjecutarConsultaSQLCli("select * from afps where codigo=" + afp.codigoAfp + " ", "remuneracion");
                    if (f.Tabla.Rows.Count > 0)
                    {
                        var tipolst = (from DataRow dr in f.Tabla.Rows
                                       select new
                                       {
                                           id = int.Parse(dr["id"].ToString()),
                                           descripcion = dr["descripcion"].ToString()
                                       }).ToList();
                        if (tipolst.Count > 0)
                        {
                            afp.descripcion = tipolst.First().descripcion;
                        }
                    }
                }
            }
            return afp;
        }
        public IsapresTrabajadorBaseVM BuscaIsapreTrabajador(int idisapretrab, string BD_Cli)
        {
            IsapresTrabajadorBaseVM isapre = new IsapresTrabajadorBaseVM();

            f.EjecutarConsultaSQLCli("select * from isapresTrabajador where id=" + idisapretrab + " ", BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                var persona = (from DataRow dr in f.Tabla.Rows
                               select new IsapresTrabajadorBaseVM
                               {
                                   id = int.Parse(dr["id"].ToString()),
                                   idpersona = int.Parse(dr["idPersona"].ToString()),
                                   codigoisapre = int.Parse(dr["codigoIsapre"].ToString()),
                                   fechainicio = dr["fechaInicio"].ToString(),
                                   numeroUf = decimal.Parse(dr["numeroUf"].ToString())
                               }).ToList();
                if (persona.Count > 0)
                {
                    isapre = persona.FirstOrDefault();
                    f.EjecutarConsultaSQLCli("select * from isapres where codigo=" + isapre.codigoisapre + " ", "remuneracion");
                    if (f.Tabla.Rows.Count > 0)
                    {
                        var tipolst = (from DataRow dr in f.Tabla.Rows
                                       select new
                                       {
                                           id = int.Parse(dr["id"].ToString()),
                                           descripcion = dr["descripcion"].ToString()
                                       }).ToList();
                        if (tipolst.Count > 0)
                        {
                            isapre.descripcion = tipolst.First().descripcion;
                        }
                    }
                }
            }
            return isapre;
        }
        public SindicatosTrabajadorBaseVM BuscaSindicatoTrabajador(string rut , string BD_Cli)
        {
            SindicatosTrabajadorBaseVM sindicato = new SindicatosTrabajadorBaseVM();

            f.EjecutarConsultaSQLCli("select * from sindicatoTrabajador where rutTrabajador='" + rut + "' ", BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                var sindlst = (from DataRow dr in f.Tabla.Rows
                               select new SindicatosTrabajadorBaseVM
                               {
                                   id = int.Parse(dr["id"].ToString()),
                                   ruttrabajador = dr["rutTrabajador"].ToString(),
                                   sindicato = int.Parse(dr["sindicato"].ToString()),
                                   fechainicio = dr["fechaInicio"].ToString()
                               }).ToList();
                if (sindlst.Count > 0)
                {
                    sindicato = sindlst.FirstOrDefault();
                    f.EjecutarConsultaSQLCli("select * from sindicatos where codigo=" + sindicato.sindicato + " ", BD_Cli);
                    if (f.Tabla.Rows.Count > 0)
                    {
                        var tipolst = (from DataRow dr in f.Tabla.Rows
                                       select new
                                       {
                                           id = int.Parse(dr["id"].ToString()),
                                           descripcion = dr["descripcion"].ToString()
                                       }).ToList();
                        if (tipolst.Count > 0)
                        {
                            sindicato.descripcion = tipolst.First().descripcion;
                        }
                    }
                }
            }
            return sindicato;
        }
        public int BuscaAux(string auxiliar, int tipaux, string BD_Cli)
        {
            int idaux = 0;
            f.EjecutarConsultaSQLCli("select * from aux where cod='" + auxiliar + "'  AND idtipoaux = " + tipaux + " ", BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                var opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new
                                    {
                                        Id = int.Parse(dr["id"].ToString()),
                                        Nombre = dr["nombre"].ToString()
                                    }).ToList();
                if (opcionesList.Count > 0)
                {
                    idaux = opcionesList.First().Id;
                }
            }
            return idaux;
        }
        public List<SelectGeneral> ListaPersonas(string BD_Cli)
        {


            f.EjecutarConsultaSQLCli("SELECT Pe.*, Pr.habilitado, Cl.habilitado " +
                "from personas Pe " +
                "left join proveedores Pr on Pr.idpersona = Pe.id " +
                "left join trabajadores Cl on Cl.idpersona = Pe.id " +
                "where Pr.habilitado<>0 or Cl.habilitado<>0 " +
                "order by rut", BD_Cli);
            List<SelectGeneral> lista = new List<SelectGeneral>();
            int lineas = f.Tabla.Rows.Count;
            if (f.Tabla.Rows.Count > 0)
            {


                var sel = (from DataRow dr in f.Tabla.Rows
                           select new
                           {
                               rut = dr["rut"].ToString(),
                               nombres = dr["nombres"].ToString(),
                               apellidos = dr["apellidos"].ToString()
                           }).ToList();
                SelectGeneral per0 = new SelectGeneral();
                per0.codigo = "0";
                per0.descripcion = " - Todas las pesonas";
                lista.Add(per0);
                foreach (var item in sel)
                {
                    SelectGeneral per = new SelectGeneral();
                    per.codigo = item.rut.ToString();
                    per.descripcion = item.rut.ToString() + " - " + item.nombres.ToString() + " " + item.apellidos.ToString();
                    lista.Add(per);
                }
                return lista;
            }
            return null;
        }



        public List<SelectGeneral> ListaCuentas(string BD_Cli)
        {


            f.EjecutarConsultaSQLCli("SELECT * from plancuentasdet " +
                                        "where habilitado = 1 order by contab", BD_Cli);
            List<SelectGeneral> lista = new List<SelectGeneral>();
            int lineas = f.Tabla.Rows.Count;
            if (f.Tabla.Rows.Count > 0)
            {


                var sel = (from DataRow dr in f.Tabla.Rows
                           select new
                           {
                               cuenta = dr["contab"].ToString(),
                               nombres = dr["glosa"].ToString(),
                           }).ToList();
                foreach (var item in sel)
                {
                    SelectGeneral per = new SelectGeneral();
                    per.codigo = item.cuenta.ToString();
                    per.descripcion = item.cuenta.ToString() + " - " + item.nombres.ToString();
                    lista.Add(per);
                }
                return lista;
            }
            return null;
        }


        public List<SelectGeneral> ListaComprobantes(string BD_Cli)
        {
            f.EjecutarConsultaSQLCli("SELECT * from comprobantes order by numero", BD_Cli);
            List<SelectGeneral> lista = new List<SelectGeneral>();
            int lineas = f.Tabla.Rows.Count;
            if (f.Tabla.Rows.Count > 0)
            {
                var sel = (from DataRow dr in f.Tabla.Rows
                           select new
                           {
                               numero = dr["numero"].ToString(),
                               observ = dr["observ"].ToString(),
                               fcontab = DateTime.Parse(dr["fcontab"].ToString())
                           }).ToList();
                foreach (var item in sel)
                {
                    SelectGeneral per = new SelectGeneral();
                    per.codigo = item.numero.ToString() + ";" + item.fcontab.ToString("dd'-'MM'-'yyyy");
                    per.descripcion = item.numero.ToString() + " - " + item.fcontab.ToString("dd'-'MM'-'yyyy") + " - " + item.observ.ToString();
                    lista.Add(per);
                }
                return lista;
            }
            return null;
        }

        public int NumeroComp(string BD_Cli, int mes, int ano)
        {
            int ultimo = 0;
            f.EjecutarConsultaSQLCli("select top 1 numero from comprobantes  " +
                "where MONTH(fcontab) = " + mes + " AND YEAR(fcontab) = " + ano + "  order by id desc;  ", BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                DataRow dm = f.Tabla.Rows[0];
                string ultimos = dm["numero"].ToString();
                if(ultimos != null) ultimo= Convert.ToInt32(ultimos);    
            }
            return ultimo + 1;
        }
        public double Redondeo(double numero, int unidades)
        {
            double numred = numero;
            int numint = Convert.ToInt32(numero);
            numint = numint / unidades;
            numred = Convert.ToDouble(numint * unidades);
            return numred;
        }
        public string BuscaTipoContrato(int tipo, string BD_Cli)
        {
            string descripcion = "";
            f.EjecutarConsultaSQLCli("select *  from tiposContratos where id =" + tipo, "remuneracion");
            if (f.Tabla.Rows.Count > 0)
            {
                var opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new
                                    {
                                        descripcion = dr["descripcion"].ToString()
                                    }).ToList();
                var opcion = opcionesList.First();
                descripcion = opcion.descripcion;
            }
            return descripcion;
        }
        public FaenasBaseVM BuscaFaena(int faena, string BD_Cli)
        {
            FaenasBaseVM fae = new FaenasBaseVM();
            f.EjecutarConsultaSQLCli("select *  from faenas where id =" + faena, BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                var opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new FaenasBaseVM
                                    {

                                        Contrato = dr["contrato"].ToString(),
                                        Descripcion = dr["descripcion"].ToString(),
                                        Inicio = dr["inicio"].ToString(),
                                        Termino = dr["termino"].ToString(),
                                        Direccion = dr["direccion"].ToString(),
                                        idPais = int.Parse(dr["idPais"].ToString()),
                                        idRegion = int.Parse(dr["idRegion"].ToString()),
                                        idComuna = int.Parse(dr["idComuna"].ToString()),
                                    }).ToList();
                fae  = opcionesList.First();
                fae.Pais = BuscaPais(fae.idPais);
                fae.Region = BuscaPais(fae.idRegion);
                fae.Comuna = BuscaPais(fae.idComuna);
            }
            return fae;
        }
        public string BuscaCargo(int cargo, string BD_Cli)
        {
            string descripcion = "";
            f.EjecutarConsultaSQLCli("select *  from cargos where id =" + cargo, BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                var opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new
                                    {
                                        descripcion = dr["descripcion"].ToString()
                                    }).ToList();
                var opcion = opcionesList.First();
                descripcion = opcion.descripcion;
            }
            return descripcion;
        }
        public string BuscaCentroCosto(int centro, string BD_Cli)
        {
            string descripcion = "";
            f.EjecutarConsultaSQLCli("select *  from centroscostos where id =" + centro, BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                var opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new
                                    {
                                        descripcion = dr["descripcion"].ToString()
                                    }).ToList();
                var opcion = opcionesList.First();
                descripcion = opcion.descripcion;
            }
            return descripcion;
        }
        public string BuscaJornada(int jornada, string BD_Cli)
        {
            string descripcion = "";
            f.EjecutarConsultaSQLCli("select *  from jornadas where id =" + jornada, BD_Cli);
            if (f.Tabla.Rows.Count > 0)
            {
                var opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new
                                    {
                                        descripcion = dr["descripcion"].ToString()
                                    }).ToList();
                var opcion = opcionesList.First();
                descripcion = opcion.descripcion;
            }
            return descripcion;
        }

    }
}


namespace RRHH
{
    public class DetFactura
    {
        public int CORREL { get; set; }
        public string CLAVE { get; set; }
        public DateTime FECHA { get; set; }
        public string CUENTA { get; set; }
        public string NOMBRE { get; set; }
        public string GLOSA { get; set; }
        public int TIP_DOCU { get; set; }
        public string FACTURA { get; set; }
        public DateTime EMISION { get; set; }
        public string RUT { get; set; }
        public string RAZON { get; set; }
        public int IVA { get; set; }
        public int VALOR { get; set; }
        public string CALCE { get; set; }
        public string AUXILIAR { get; set; }
        public string ORDEN { get; set; }
    }
}

namespace RRHH
{
    public class Plancta
    {
        public int Id { get; set; }
        public int Idgrupo { get; set; }
        public int Idcuenta { get; set; }
        public int Idsubcuenta { get; set; }
        public int Idauxiliar { get; set; }
        public string Glosa { get; set; }
        public string Contab { get; set; }
        public int Gananperd { get; set; }
        public int Actpascap { get; set; }
        public string Flg_ingres { get; set; }
        public string Flg_gastos { get; set; }
    }
}
namespace RRHH
{
    public class DetObligaciones
    {
        public int ID { get; set; }
        public DateTime FECHA { get; set; }
        public string RUT_PROV { get; set; }
        public string GLOSA { get; set; }
        public string CUENTA { get; set; }
        public string AUXILIAR { get; set; }
        public DateTime FE_VENCE { get; set; }
        public int TIPO_DOC { get; set; }
        public string NUM_DOC { get; set; }
        public decimal TOTAL { get; set; }
        public decimal NETO { get; set; }
        public decimal IMPUESTO { get; set; }
        public string AUXI001 { get; set; } = "";
    }

    public class SelectActivo
    {
        public string tipo { get; set; }
        public string descripcion { get; set; }
    }
    public class SelectGeneral
    {
        public string codigo { get; set; }
        public string descripcion { get; set; }
    }
	public class detempresa
	{
        public int id { get; set; } 
		public string rut { get; set; }
		public string razonsocial { get; set; }
		public string fantasia { get; set; }
		public string email { get; set; }
		public string direccion { get; set; }
		public string region { get; set; }
		public string comuna { get; set; }
        public int idfaena { get; set; }
    }

}
