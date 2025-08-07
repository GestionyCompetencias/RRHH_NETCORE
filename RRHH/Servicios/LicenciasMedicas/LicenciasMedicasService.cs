using Microsoft.AspNetCore.Mvc;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.LicenciasMedicas
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface ILicenciasMedicasService
    {
        /// <summary>
        /// Genera lista de licencias del trabajador.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="rut">HRut del trabajador a listar</param>
        /// <returns>Lista de licencias de trabajador</returns>
        public List<DetLicenciasMedicas> ListarLicenciasMedicasService(int idEmpresa, string rut);

        /// <summary>
        /// Consulta por licencia medica
        /// </summary>
        /// <param name="id">ID de haber informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Muestra informacion de haber informado</returns>
        public List<DetLicenciasMedicas> ConsultaLicenciaMedicaIdService(int id, int idEmpresa);

        /// <summary>
        /// Creación licencia medica.
        /// </summary>
        /// <param name="opciones">Registro de licencia medica</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns
        public Resultado CrearLicenciaMedicaService(DetLicenciasMedicas opciones, int idEmpresa);

        /// <summary>
        /// Edita licencia medica.
        /// </summary>
        /// <param name="opciones">Registro de licencia medica</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Estructura de resultado</returns>
        public Resultado EditaLicenciaMedicaService(DetLicenciasMedicas opciones, int idEmpresa);

        /// <summary>
        /// Inhabilitar licencia medica.
        /// </summary>
        /// <param name="id">ID de haber informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita licencia medica</returns>
        public Resultado InhabilitaLicenciaMedicaService(LicenciaDeleteVM opciones, int idEmpresa);

        /// <summary>
        /// Carga lista de trabajadores.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de trabajadores</returns>
        public Resultado ComboTrabajadoresService(int idEmpresa);

        /// <summary>
        /// Carga lista de tipos de licencias.
        /// </summary>
        /// <returns>Lista de tipos de licencias</returns>
        public Resultado ComboTipoLicenciasService();

        /// <summary>
        /// Carga lista de tipos de medicos.
        /// </summary>
        /// <returns>Lista de tipos de medicos</returns>
        public Resultado ComboTipoMedicosService();
    }

    public class LicenciasMedicasService : ILicenciasMedicasService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public LicenciasMedicasService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetLicenciasMedicas> ListarLicenciasMedicasService(int empresa, string rut)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;


                f.EjecutarConsultaSQLCli("SELECT licenciasmedicas.id,licenciasmedicas.codigoLicencia,licenciasmedicas.rutTrabajador,licenciasmedicas.fechaInicio " +
                                            ",licenciasmedicas.fechaTermino,licenciasmedicas.dias, licenciasmedicas.tipoLicencia, licenciasmedicas.comentario " +
                                            ",licenciasmedicas.tipoMedico,licenciasmedicas.PDF " +
                                            "FROM licenciasmedicas " +
                                            "WHERE licenciasmedicas.habilitado = 1 and licenciasmedicas.rutTrabajador ='" + rut + "' ", BD_Cli);


                List<DetLicenciasMedicas> opcionesList = new List<DetLicenciasMedicas>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetLicenciasMedicas()
                                    {
                                        id = int.Parse(dr["id"].ToString()),
                                        ruttrabajador = dr["rutTrabajador"].ToString(),
                                        codigolicencia = dr["codigoLicencia"].ToString(),
                                        fechainicio = dr["fechainicio"].ToString(),
                                        fechatermino = dr["fechatermino"].ToString(),
                                        dias = int.Parse(dr["dias"].ToString()),
                                        tipolicencia = int.Parse(dr["tipoLicencia"].ToString()),
                                        comentario = dr["comentario"].ToString(),
                                        tipomedico = int.Parse(dr["tipoMedico"].ToString())
                                    }).ToList();
                    foreach (var r in opcionesList)
                    {
                        DateTime inicio = Convert.ToDateTime(r.fechainicio);
                        r.fechainicio = inicio.ToString("dd'-'MM'-'yyyy");
                        DateTime termino = Convert.ToDateTime(r.fechatermino);
                        r.fechatermino = termino.ToString("dd'-'MM'-'yyyy");
                        PersonasBaseVM pers = Generales.BuscaPersona(r.ruttrabajador, BD_Cli);
                        r.nombre = pers.Nombres + " " + pers.Apellidos;
                        r.desclicencia = Generales.BuscaTipoLicencia(r.tipolicencia);
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
                var Asunto = "Error al Consultar haber informados";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar haber informados", correos.destinatarioErrores);
                return null;
            }

        }
        public List<DetLicenciasMedicas> ConsultaLicenciaMedicaIdService(int id, int empresa)
        {

            try
            {

                {

                    string RutEmpresa = f.obtenerRUT(empresa);
                    var BD_Cli = "remuneracion_" + RutEmpresa;


                    f.EjecutarConsultaSQLCli("SELECT licenciasmedicas.id,licenciasmedicas.codigoLicencia,licenciasmedicas.rutTrabajador,licenciasmedicas.fechaInicio " +
                                                ",licenciasmedicas.fechaTermino,licenciasmedicas.dias, licenciasmedicas.tipoLicencia, licenciasmedicas.comentario " +
                                                ",licenciasmedicas.tipoMedico,licenciasmedicas.PDF " +
                                                "FROM licenciasmedicas " +
                                                "WHERE licenciasmedicas.habilitado = 1 and licenciasmedicas.id =" + id + " ", BD_Cli);


                    List<DetLicenciasMedicas> opcionesList = new List<DetLicenciasMedicas>();
                    if (f.Tabla.Rows.Count > 0)
                    {

                        opcionesList = (from DataRow dr in f.Tabla.Rows
                                        select new DetLicenciasMedicas()
                                        {
                                            id = int.Parse(dr["id"].ToString()),
                                            ruttrabajador = dr["rutTrabajador"].ToString(),
                                            codigolicencia = dr["codigoLicencia"].ToString(),
                                            fechainicio = dr["fechainicio"].ToString(),
                                            fechatermino = dr["fechatermino"].ToString(),
                                            dias = int.Parse(dr["dias"].ToString()),
                                            tipolicencia = int.Parse(dr["tipoLicencia"].ToString()),
                                            comentario = dr["comentario"].ToString(),
                                            tipomedico = int.Parse(dr["tipoMedico"].ToString())
                                        }).ToList();
                        foreach (var r in opcionesList)
                        {
                            DateTime inicio = Convert.ToDateTime(r.fechainicio);
                            r.fechainicio = inicio.ToString("yyyy'-'MM'-'dd");
                            DateTime termino = Convert.ToDateTime(r.fechatermino);
                            r.fechatermino = termino.ToString("yyyy'-'MM'-'dd");
                            PersonasBaseVM pers = Generales.BuscaPersona(r.ruttrabajador, BD_Cli);
                            r.nombre = pers.Nombres + " " + pers.Apellidos;
                        }
                        return opcionesList;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                var Asunto = "Error al Consultar licencia";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar Licencia", correos.destinatarioErrores);
                return null;
            }
        }


        public Resultado CrearLicenciaMedicaService(DetLicenciasMedicas opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            try
            {
                DateTime hoy = DateTime.Now.Date;
                string hoystr = hoy.ToString("yyyy'-'MM'-'dd");
                opciones.PDF = null;
                string query2 = "insert into licenciasmedicas (rutTrabajador,codigoLicencia,fechaInicio,fechaTermino,dias,tipoLicencia" +
                                ",comentario,tipoMedico,fechaIngreso,PDF,habilitado) " +
                                "values " +
                                "('" + opciones.ruttrabajador + "','" + opciones.codigolicencia + "','" + opciones.fechainicio + "','" + opciones.fechatermino +
                                "', " + opciones.dias + ", " + opciones.tipolicencia + " , '" + opciones.comentario + "' ," + opciones.tipomedico +  
                                ", '" + hoystr + "',null ,1) ";
                if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Licencia ingresada de manera exitosa";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se ingreso la información de licencia";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al guardar licencia";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar guardar licenia en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar guardar licencia" + eG.Message.ToString();
                return resultado;
            }

        }
        public Resultado EditaLicenciaMedicaService(DetLicenciasMedicas opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;



            try
            {
                string query = "update licenciasmedicas set [rutTrabajador]='" + opciones.ruttrabajador + "' ,[codigoLicencia]='" + opciones.codigolicencia +
                               "',  [fechaInicio]='" + opciones.fechainicio + "',[fechaTermino]='" + opciones.fechatermino + "',[dias]= " + opciones.dias +
                               ", [tipolicencia]=" + opciones.tipolicencia + ",[comentario]='" + opciones.comentario + "',[tipoMedico]= " + opciones.tipomedico +
                               ", [PDF]= " + opciones.PDF + 
                               " where licenciasmedicas.id=" + opciones.id ;

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 2;
                    resultado.mensaje = "Licencia editada exitosamente.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se editó la información de licencia.";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al Editar licencia";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar editar un licencia en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar editar licencia" + eG.Message.ToString();
                return resultado;
            }
        }

        public Resultado InhabilitaLicenciaMedicaService(LicenciaDeleteVM opciones, int empresa)
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
                    resultado.mensaje = "Debe indicar la informacion de la licencia  que desea eliminar";
                    return resultado;
                }

                string query = "update licenciasmedicas set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Registro eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó el registro.";
                }
                return resultado;
            }
            catch (Exception eG)
            {
                var Asunto = "Error al eliminar registro";
                var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminar licencia en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una licencia" + eG.Message.ToString();
                return resultado;
            }
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
        public Resultado ComboTipoLicenciasService()
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            f.EjecutarConsultaSQLCli("SELECT tipoLicenciaMedica.id,tipoLicenciaMedica.descripcion " +
                                        "FROM tipoLicenciaMedica ", "remuneracion");

            if (f.Tabla.Rows.Count > 0)
            {
                var opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new
                                    {
                                        id = dr["id"].ToString(),
                                        descripcion = dr["descripcion"].ToString()
                                    }).ToList();
                var data = new List<Conceptos>();
                foreach (var h in opcionesList)
                {
                    data.Add(new Conceptos() { Codigo = h.id, Descripcion = h.descripcion });
                }
                resultado.result = 1;
                resultado.data = data;
            }
            return resultado;
        }
        public Resultado ComboTipoMedicosService()
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            f.EjecutarConsultaSQLCli("SELECT tipoMedico.id,tipoMedico.descripcion " +
                                        "FROM tipoMedico ", "remuneracion");

            if (f.Tabla.Rows.Count > 0)
            {
                var opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new
                                    {
                                        id = dr["id"].ToString(),
                                        descripcion = dr["descripcion"].ToString()
                                    }).ToList();
                var data = new List<Conceptos>();
                foreach (var h in opcionesList)
                {
                    data.Add(new Conceptos() { Codigo = h.id, Descripcion = h.descripcion });
                }
                resultado.result = 1;
                resultado.data = data;
            }
            return resultado;
        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetLicenciasMedicas
    {
        public int id { get; set; }
        public string ruttrabajador { get; set; }
        public string nombre { get; set; }
        public string codigolicencia { get; set; }
        public string fechainicio { get; set; }
        public string fechatermino { get; set; }
        public int dias { get; set; }
        public int tipolicencia { get; set; }
        public string desclicencia { get; set; }
        public string comentario { get; set; }
        public int tipomedico { get; set; }
        public int habilitado { get; set; }
        public BinaryData PDF { get; set; }
    }
}

