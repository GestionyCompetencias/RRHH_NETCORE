using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace RRHH.Servicios.ConversionContable
{
    /// <summary>
    /// Servicio para generar y operar con las conversionContable de una empresa.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IConversionContableService
    {
        /// <summary>
        /// Genera lista de conversionContable.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="modulo">Tipo de comprobante</param>
        /// <returns>Lista de conversionContable</returns>
        public List<ConversionContableBaseVM> ListarConversionContableService(int idEmpresa,string modulo);

        /// <summary>
        /// Consulta por id conversion.
        /// </summary>
        /// <param name="id">ID de cuenta especial</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Muestra informacion de conversion</returns>
        public List<ConversionContableBaseVM> ConsultaConversionContableIdService(int id, int idEmpresa);
        /// <summary>
        /// Creación de conversion.
        /// </summary>
        /// <param name="opciones">Registro de conversion</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns
        public Resultado CrearConversionContableService(ConversionContableBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Edita conversion.
        /// </summary>
        /// <param name="opciones">Registro de conversion</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns>
        public Resultado EditaConversionContableService(ConversionContableBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Inhabilitar cuenta especial.
        /// </summary>
        /// <param name="id">ID de cuenta especial</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita cuenta especial</returns>
        public Resultado InhabilitaConversionContableService(ConversionContableDeleteVM opciones, int idEmpresa);
 
        /// <summary>
        /// Carga lista de conceptos.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de descuentos</returns>
        public Resultado CargaConceptosService(int idEmpresa);
 
        /// <summary>
        /// Carga lista de cuentas contables.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de descuentos</returns>
        public Resultado CargaCuentasService(int idEmpresa);

    }

    public class ConversionContableService : IConversionContableService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public ConversionContableService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<ConversionContableBaseVM> ListarConversionContableService(int empresa, string modulo)
        {
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                f.EjecutarConsultaSQLCli("SELECT conversionContable.id,conversionContable.modulo,conversionContable.pago,conversionContable.concepto, "+
                                            "conversionContable.cuenta,conversionContable.tipoAuxiliar, conversionContable.codigoAuxiliar, " +
                                            "conversionContable.debeHaber, conversionContable.tipoVencimiento, conversionContable.diaVencimiento,  " +
                                            "conversionContable.mesVencimiento, conversionContable.agrupacion, conversionContable.provision  " +
                                           " FROM conversionContable " +
                                            "WHERE conversionContable.habilitado = 1 and conversionContable.modulo ='"+modulo+"' ", BD_Cli);

                List<ConversionContableBaseVM> opcionesList = new List<ConversionContableBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new ConversionContableBaseVM()
                                    {
                                        Id = int.Parse(dr["id"].ToString()),
                                        modulo = dr["modulo"].ToString(),
                                        pago = dr["pago"].ToString(),
                                        concepto = int.Parse(dr["concepto"].ToString()),
                                        cuenta = dr["cuenta"].ToString(),
                                        debehaber = dr["debehaber"].ToString(),
                                        tipoauxiliar = int.Parse(dr["tipoAuxiliar"].ToString()),
                                        codigoauxiliar = dr["codigoauxiliar"].ToString(),
                                        tipovencimiento = int.Parse(dr["tipoVencimiento"].ToString()),
                                        diavencimiento = int.Parse(dr["diaVencimiento"].ToString()),
                                        mesvencimiento = int.Parse(dr["mesVencimiento"].ToString()),
                                        agrupacion = int.Parse(dr["agrupacion"].ToString()),
                                        provision = int.Parse(dr["provision"].ToString())
                                    }).ToList();
                    return opcionesList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar conversion Contable";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar conversion Contable", correos.destinatarioErrores);
                return null;
            }

        }
        public List<ConversionContableBaseVM> ConsultaConversionContableIdService(int id, int empresa)
        {

            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                f.EjecutarConsultaSQLCli("SELECT conversionContable.id,conversionContable.modulo,conversionContable.pago,conversionContable.concepto, " +
                                            "conversionContable.cuenta,conversionContable.tipoAuxiliar, conversionContable.codigoAuxiliar, " +
                                            "conversionContable.debeHaber, conversionContable.tipoVencimiento, conversionContable.diaVencimiento  " +
                                            "conversionContable.mesVencimiento, conversionContable.agrupacion, conversionContable.provision  " +
                                           " FROM conversionContable " +
                                            "WHERE conversionContable.habilitado = 1 ", BD_Cli);

                List<ConversionContableBaseVM> opcionesList = new List<ConversionContableBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new ConversionContableBaseVM()
                                    {
                                        Id = int.Parse(dr["id"].ToString()),
                                        modulo = dr["modulo"].ToString(),
                                        pago = dr["pago"].ToString(),
                                        concepto = int.Parse(dr["concepto"].ToString()),
                                        cuenta = dr["cuenta"].ToString(),
                                        tipoauxiliar = int.Parse(dr["tipoAuxiliar"].ToString()),
                                        codigoauxiliar = dr["codigoauxiliar"].ToString(),
                                        tipovencimiento = int.Parse(dr["tipoVencimiento"].ToString()),
                                        diavencimiento = int.Parse(dr["diaVencimiento"].ToString()),
                                        mesvencimiento = int.Parse(dr["mesVencimiento"].ToString()),
                                        agrupacion = int.Parse(dr["agrupacion"].ToString()),
                                        provision = int.Parse(dr["provision"].ToString())
                                    }).ToList();
                    return opcionesList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar conversion";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar conversion", correos.destinatarioErrores);

                return null;
            }
        }


        public Resultado CrearConversionContableService(ConversionContableBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            try
            {
                if (opciones.cuenta == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la cuenta de conversion";
                    return resultado;
                }

                string query2 = "insert into conversionContable (modulo,pago,concepto,cuenta,tipoAuxiliar,codigoAuxiliar,debehaber, "+
                                "tipoVencimiento,diaVencimiento,mesVencimiento,agrupacion,provision,habilitado) " +
                "values " +
                "('" + opciones.modulo + "','" + opciones.pago + "'," + opciones.concepto + ",'" + opciones.cuenta + "'," + opciones.tipoauxiliar +
                ", '" + opciones.codigoauxiliar + "', '" + opciones.debehaber + "' ," + opciones.tipovencimiento + 
                ", " + opciones.diavencimiento + " , " + opciones.mesvencimiento + " ," + opciones.agrupacion+" ,"+ opciones.provision + " ,1)  ";
                if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Conversion ingresada de manera exitosa";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se ingreso la información de conversion";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Guardar conversion";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar conversion en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar conversion" + eG.Message.ToString();
                return resultado;
            }

        }
        public Resultado EditaConversionContableService(ConversionContableBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            try
            {
                if (opciones.cuenta == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la cuenta de conversion";
                    return resultado;
                }


                string query = "update conversionContable set [modulo]='" + opciones.modulo + "',[pago]=" + opciones.pago + "' ,[concepto]=" + opciones.concepto + 
                                   ", [cuenta]='" + opciones.cuenta + "',[tipoAuxiliar]=" + opciones.tipoauxiliar + ",[codigoAuxiliar]= '" + opciones.codigoauxiliar +
                                   "', [debeHaber]='" + opciones.debehaber + "',[tipoVencimiento]=" + opciones.tipovencimiento + ",[diaVencimiento]= " + opciones.diavencimiento +
                                   ", [mesVencimiento]= " + opciones.mesvencimiento + ", [agrupacion]= " + opciones.agrupacion +", [provision]="+opciones.provision+
                                    " where conversionContable.id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Conversion editada exitosamente.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se editó la información de la conversion.";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar conversion";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar una conversion en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar conversion" + eG.Message.ToString();
                return resultado;
            }

        }

        public Resultado InhabilitaConversionContableService(ConversionContableDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            try
            {
                if (opciones.Id == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la informacion de la conversion que desea eliminar";
                    return resultado;
                }

                string query = "update conversionContable set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Conversion eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó la información de la conversion.";
                }

                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al eliminar conversion";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar conversion en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una conversion" + eG.Message.ToString();
                return resultado;
            }
        }
        public Resultado CargaConceptosService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            f.EjecutarConsultaSQLCli("SELECT haberes.id,haberes.haber,haberes.descripcion,haberes.imponible,haberes.tributable,haberes.numeroMeses, " +
                                        " haberes.garantizado, haberes.retenible, haberes.calculado, haberes.tiempo, haberes.deducible, " +
                                        " haberes.baseLicencia, haberes.baseSobretiempo, " +
                                        " haberes.baseIndemnizacion, haberes.baseVariable, haberes.codigoDT, haberes.codigoPrevired" +
                                        " FROM haberes " +
                                        " WHERE haberes.habilitado = 1 ", BD_Cli);


            List<HaberBaseVM> opcionesList = new List<HaberBaseVM>();
            if (f.Tabla.Rows.Count > 0)
            {

                opcionesList = (from DataRow dr in f.Tabla.Rows
                                select new HaberBaseVM()
                                {
                                    id = int.Parse(dr["id"].ToString()),
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
            }
            var data = new List<Conceptos>();
            foreach (var h in opcionesList)
            {
                data.Add(new Conceptos() { Codigo = Convert.ToString(h.haber), Descripcion = Convert.ToString(h.haber)+" "+ h.descripcion });
            }
            f.EjecutarConsultaSQLCli("SELECT descuentos.id,descuentos.descuento,descuentos.descripcion,descuentos.prioridad,descuentos.minimo,descuentos.maximo, " +
                                        " descuentos.codigoDT, descuentos.codigoPrevired" +
                                        " FROM descuentos " +
                                        " WHERE descuentos.habilitado = 1 ", BD_Cli);


            List<DescuentoBaseVM> opcionesdesc = new List<DescuentoBaseVM>();
            if (f.Tabla.Rows.Count > 0)
            {

                opcionesdesc = (from DataRow dr in f.Tabla.Rows
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
                foreach (var d in opcionesdesc)
                {
                    DetDescuentos registro = new DetDescuentos();
                    registro.id = d.id;
                    registro.descuento = d.descuento;
                    registro.descripcion = d.descripcion;
                    registro.minimo = d.minimo.ToString("###,###,###");
                    registro.maximo = d.maximo.ToString("###,###,###");
                    registro.codigoDT = d.codigoDT;
                    salida.Add(registro);
                }
            }
            foreach (var d in opcionesdesc)
            {
                data.Add(new Conceptos() { Codigo = Convert.ToString(d.descuento), Descripcion = Convert.ToString(d.descuento)+" "+ d.descripcion });
            }
            data.Add(new Conceptos() { Codigo = "910", Descripcion = " 910 LEY.SOC.PENSIÓN" });
            data.Add(new Conceptos() { Codigo = "916", Descripcion = "916 LEY.SOC.CESANTIA " });
            data.Add(new Conceptos() { Codigo = "921", Descripcion = "921 LEY.SOC. SALUD" });
            data.Add(new Conceptos() { Codigo = "990", Descripcion = "990 IMPUESTO UNICO" });
            data.Add(new Conceptos() { Codigo = "2300", Descripcion = "2300 SALDO LIQUIDO" });
            data.Add(new Conceptos() { Codigo = "2400", Descripcion = "2400 FONDO DE CESANTIA" });
            data.Add(new Conceptos() { Codigo = "2404", Descripcion = "2404 SEGURO DE INVALIDES(SIS)" });
            data.Add(new Conceptos() { Codigo = "2402", Descripcion = "2402 MUTUAL DE SEGURIDAD" });
            resultado.result = 1;
            resultado.data = data.OrderBy(x => x.Descripcion);
            return resultado;
        }

        public Resultado CargaCuentasService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "contable_" + RutEmpresa;

            SqlConnection cn;
            DataSet ds;
            SqlDataAdapter da;
            DataTable dt;
            string CadenaSqlCli = "Data Source=179.61.13.236,1433;Initial Catalog='" + BD_Cli + "';User ID=gycsolcl_gestionAdmin;Password=.7VzG#{Ty(Gvu{!:(fm}4:4/LT;TrustServerCertificate=True";
            cn = new SqlConnection(CadenaSqlCli);
            cn.Open();
            da = new SqlDataAdapter("Select * from [dbo].plancuentasdet " +
                                     "where habilitado = 1", cn);
            ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];
            cn.Close();
            var data = new List<Conceptos>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow h in dt.Rows)
                {
                    string codigo = h["contab"].ToString();
                    string glosa = h["glosa"].ToString();
                    data.Add(new Conceptos() { Codigo = codigo, Descripcion = codigo + " " + glosa });
                }
            }
            resultado.result = 1;
            resultado.data = data.OrderBy(x => x.Codigo);
            return resultado;
        }
    }
}
