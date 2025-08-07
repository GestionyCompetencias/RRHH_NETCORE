using Microsoft.AspNetCore.Mvc;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.DescuentosInformados
{
    /// <summary>
    /// Servicio para generar y operar con l0s descuentos informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IDescuentosInformadosService
    {
        /// <summary>
        /// Genera lista de descuento informados.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="descuento">Descuento informados que desea listar</param>
        /// <param name="mes">mes del descuento informados que desea listar</param>
        /// <param name="anio">año del descuento informados que desea listar</param>
        /// <param name="pago">pago del descuento informados que desea listar</param>
        /// <returns>Lista de descuentos informados</returns>
        public List<DetDescuentosInformados> ListarDescuentosInformadosService(int idEmpresa, int descuento, int mes, int anio, string pago);

        /// <summary>
        /// Consulta por descuento informado por id.
        /// </summary>
        /// <param name="id">ID de descuento informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Muestra informacion de descuento informado</returns>
        public List<DescuentosInformadosBaseVM> ConsultaDescuentoInformadoIdService(int id, int idEmpresa);
        /// <summary>
        /// Creación de descuento informado.
        /// </summary>
        /// <param name="opciones">Registro de descuento informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns
        public Resultado CrearDescuentoInformadoService(DescuentosInformadosBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Edita descuento informado.
        /// </summary>
        /// <param name="opciones">Registro de descuento informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns>
        public Resultado EditaDescuentoInformadoService(DescuentosInformadosBaseVM opciones, int idEmpresa);

        /// <summary>
        /// Inhabilitar descuento informado.
        /// </summary>
        /// <param name="id">ID de descuento informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita descuento informado</returns>
        public Resultado InhabilitaDescuentoInformadoService(DescuentosInformadosDeleteVM opciones, int idEmpresa);

        /// <summary>
        /// Carga lista de descuentos.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de descuentos</returns>
        public Resultado ComboDescuentosService(int idEmpresa);

        /// <summary>
        /// Carga lista de trabajadores.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de trabajadores</returns>
        public Resultado ComboTrabajadoresService(int idEmpresa);

    }

    public class DescuentosInformadosService : IDescuentosInformadosService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public DescuentosInformadosService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetDescuentosInformados> ListarDescuentosInformadosService(int empresa, int descuento, int mes,int anio,string pago)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                DateTime fecini = new DateTime(anio, mes, 1);
                DateTime fecfin = f.UltimoDia(fecini);
                string fecinistr = fecini.ToString("yyyy'-'MM'-'dd");
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");


                f.EjecutarConsultaSQLCli("SELECT descuentosinformados.id,descuentosinformados.descuento,descuentosinformados.rutTrabajador,descuentosinformados.correlativo " +
                                            ",descuentosinformados.afecta,descuentosinformados.pago, descuentosinformados.tipoCalculo, descuentosinformados.monto " +
                                            ",descuentosinformados.haber,descuentosinformados.fechaDesde, descuentosinformados.fechaHasta, descuentosinformados.fechaIngreso " +
                                            ",descuentosinformados.pagina " +
                                            "FROM descuentosInformados " +
                                            "WHERE descuentosinformados.habilitado = 1 and descuentosinformados.descuento =" + descuento +
                                            " and descuentosinformados.fechaDesde <= '" + fecfinstr + "' and descuentosinformados.fechaHasta >= '" + fecinistr +
                                            "' and descuentosinformados.pago = '" + pago + "' ", BD_Cli);


                List<DetDescuentosInformados> opcionesList = new List<DetDescuentosInformados>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetDescuentosInformados()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        descuento = int.Parse(dr["descuento"].ToString()),
                                        rutTrabajador = dr["rutTrabajador"].ToString(),
                                        correlativo = int.Parse(dr["correlativo"].ToString()),
                                        afecta = dr["afecta"].ToString(),
                                        pago = dr["pago"].ToString(),
                                        tipoCalculo = dr["tipoCalculo"].ToString(),
                                        monto = dr["monto"].ToString(),
                                        haber = int.Parse(dr["haber"].ToString()),
                                        fechaDesde = dr["fechaDesde"].ToString(),
                                        fechaHasta = dr["fechaHasta"].ToString(),
                                        fechaIngreso = dr["fechaIngreso"].ToString(),
                                        pagina = int.Parse(dr["pagina"].ToString())
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        Decimal monto = Convert.ToDecimal(r.monto);
                        r.monto = monto.ToString("###,###,##0");
                        DateTime desde = Convert.ToDateTime(r.fechaDesde);
                        r.fechaDesde = desde.ToString("dd'-'MM'-'yyyy");
                        DateTime hasta = Convert.ToDateTime(r.fechaHasta);
                        r.fechaHasta = hasta.ToString("dd'-'MM'-'yyyy");
                        PersonasBaseVM pers = Generales.BuscaPersona(r.rutTrabajador, BD_Cli);
                        r.nombre = pers.Nombres + " " + pers.Apellidos;
                    }
                return opcionesList;
                }
                else
                {
                 return null;  
                }
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar descuento informados";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar descuento informados", correos.destinatarioErrores);
                return null;
            }

        }
        public List<DescuentosInformadosBaseVM> ConsultaDescuentoInformadoIdService(int id, int empresa)
        {

            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT descuentosinformados.id,descuentosinformados.descuento,descuentosinformados.rutTrabajador,descuentosinformados.correlativo " +
                                            ",descuentosinformados.afecta,descuentosinformados.pago, descuentosinformados.tipoCalculo, descuentosinformados.monto " +
                                            ",descuentosinformados.haber,descuentosinformados.fechaDesde, descuentosinformados.fechaHasta, descuentosinformados.fechaIngreso " +
                                            ",descuentosinformados.pagina " +
                                           "FROM descuentosinformados " +
                                            "WHERE descuentosinformados.habilitado = 1 and descuentosinformados.id =" + id + " ", BD_Cli);


                List<DescuentosInformadosBaseVM> opcionesList = new List<DescuentosInformadosBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DescuentosInformadosBaseVM()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        descuento = int.Parse(dr["descuento"].ToString()),
                                        rutTrabajador = dr["rutTrabajador"].ToString(),
                                        correlativo = int.Parse(dr["correlativo"].ToString()),
                                        afecta = dr["afecta"].ToString(),
                                        pago = dr["pago"].ToString(),
                                        tipoCalculo = dr["tipoCalculo"].ToString(),
                                        monto = decimal.Parse(dr["monto"].ToString()),
                                        haber = int.Parse(dr["haber"].ToString()),
                                        fechaDesde = dr["fechaDesde"].ToString(),
                                        fechaHasta = dr["fechaHasta"].ToString(),
                                        fechaIngreso = dr["fechaIngreso"].ToString(),
                                        pagina = int.Parse(dr["pagina"].ToString())
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {

                        DateTime desde = Convert.ToDateTime(r.fechaDesde);
                        r.fechaDesde = desde.ToString("yyyy'-'MM'-'dd");
                        DateTime hasta = Convert.ToDateTime(r.fechaHasta);
                        r.fechaHasta = hasta.ToString("yyyy'-'MM'-'dd");
                    }
                    return opcionesList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar descuento informado";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar descuento informado", correos.destinatarioErrores);
                return null;
            }
        }


        public Resultado CrearDescuentoInformadoService(DescuentosInformadosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            try
            {
                if (opciones.descuento == null || opciones.descuento ==0)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar descuento";
                    return resultado;
                }
                if(opciones.monto != null)
                {
                    string monto = opciones.monto.ToString();
                    monto = monto.Replace(",",".");
                    opciones.monto = Convert.ToDecimal(monto);
                }
                DateTime hoy = DateTime.Now.Date;
                string hoystr = hoy.ToString("yyyy'-'MM'-'dd");
                if (opciones.pagina == null) opciones.pagina = 0;

                string query2 = "insert into descuentosinformados (descuento,rutTrabajador,correlativo,afecta,pago,tipoCalculo,monto,haber,fechaDesde,fechaHasta,fechaIngreso,pagina,habilitado) " +
                "values " +
                "(" + opciones.descuento + ",'" + opciones.rutTrabajador + "'," + opciones.correlativo + ",'" + opciones.afecta + "','" + opciones.pago +
                "', '" + opciones.tipoCalculo + "', " + opciones.monto + " , " + opciones.haber + " ,'" + opciones.fechaDesde + "','" + opciones.fechaHasta +
                "', '" + hoystr + "'," + opciones.pagina + " ,1) ";
                if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Descuento Informado ingresada de manera exitosa";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se ingreso la información de descuento informado";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al guardar descuento informado";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar descuento informado en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar descuento informado" + eG.Message.ToString();
                return resultado;
            }

        }
        public Resultado EditaDescuentoInformadoService(DescuentosInformadosBaseVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;



            try
            {
                if (opciones.descuento == null || opciones.descuento ==0)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar descuento";
                    return resultado;
                }

                string query = "update descuentosinformados set [descuento]=" + opciones.descuento + " ,[rutTrabajador]='" + opciones.rutTrabajador + "' ,[correlativo]=" + opciones.correlativo +
                               ",  [afecta]='" + opciones.afecta + "',[pago]='" + opciones.pago + "',[tipoCalculo]= '" + opciones.tipoCalculo +
                               "',  monto]=" + opciones.monto + ",[haber]=" + opciones.haber + ",[fechaDesde]= '" + opciones.fechaDesde +
                                   "', [fechaHasta]= '" + opciones.fechaHasta + "', [pagina]= " + opciones.pagina + 
                                    " where descuentosinformados.id=" + opciones.id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 2;
                    resultado.mensaje = "Descuento Informado editado exitosamente.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se editó la información del descuento informado.";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar descuento informado";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar un descuento informado en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar descuento informado" + eG.Message.ToString();
                return resultado;
            }
        }

        public Resultado InhabilitaDescuentoInformadoService(DescuentosInformadosDeleteVM opciones, int empresa)
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
                    resultado.mensaje = "Debe indicar la informacion del descuento informado que desea eliminar";
                    return resultado;
                }

                string query = "update descuentosinformados set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Descuento Informado eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó la información del descuento informado.";
                }

                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al eliminar descuento informado";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar descuento informado en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una descuento informado" + eG.Message.ToString();
                return resultado;
            }
        }
        public Resultado ComboDescuentosService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            f.EjecutarConsultaSQLCli("SELECT descuentos.descuento,descuentos.Descripcion " +
                                        "FROM descuentos " +
                                        " where descuentos.habilitado = 1 ", BD_Cli);

            List<DescuentoBaseVM> opcionesList = new List<DescuentoBaseVM>();
            if (f.Tabla.Rows.Count > 0)
            {

                opcionesList = (from DataRow dr in f.Tabla.Rows
                                select new DescuentoBaseVM()
                                {
                                    descuento = int.Parse(dr["descuento"].ToString()),
                                    descripcion = dr["descripcion"].ToString(),
                                }).ToList();
            }
            var data = new List<Conceptos>();
            foreach (var h in opcionesList)
            {
                data.Add(new Conceptos() { Codigo = Convert.ToString(h.descuento), Descripcion = h.descripcion });
            }
            resultado.result = 1;
            resultado.data = data;
            return resultado;
        }
        public Resultado ComboTrabajadoresService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            f.EjecutarConsultaSQLCli("SELECT personas.rut,personas.nombres,personas.apellidos " +
                                        "FROM personas " , BD_Cli);

            List<PersonasBaseVM> opcionesList = new List<PersonasBaseVM>();
            if (f.Tabla.Rows.Count > 0)
            {

                opcionesList = (from DataRow dr in f.Tabla.Rows
                                select new PersonasBaseVM()
                                {
                                    Rut = dr["Rut"].ToString(),
                                    Nombres = dr["Nombres"].ToString(),
                                    Apellidos = dr["Apellidos"].ToString()
                                }).ToList();
            }
            var data = new List<Conceptos>();
            foreach (var h in opcionesList)
            {
                data.Add(new Conceptos() { Codigo = h.Rut, Descripcion = h.Nombres+" "+h.Apellidos });
            }
            resultado.result = 1;
            resultado.data = data;
            return resultado;
        }

    }
}
namespace RRHH.Models.ViewModels
{
    public class DetDescuentosInformados
    {
        public int id { get; set; }
        public int descuento { get; set; }
        public string rutTrabajador { get; set; }
        public string nombre { get; set; }
        public int correlativo { get; set; }
        public string afecta { get; set; }
        public string pago { get; set; }
        public string tipoCalculo { get; set; }
        public string monto { get; set; }
        public int haber { get; set; }
        public string fechaDesde { get; set; }
        public string fechaHasta { get; set; }
        public string fechaIngreso { get; set; }
        public int pagina { get; set; }
        public int habilitado { get; set; }
    }
}

