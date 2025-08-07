using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.ReajusteSueldos
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface IReajusteSueldosService
    {
        /// <summary>
        /// Genera lista de reajustes.
        /// </summary>
        /// <param name="empresa">Rut de empesa</param>
        /// <returns>Lista de reajustes</returns>
        public List<DetReajusteSueldos> ListarReajusteSueldosService(int empresa, decimal reajuste);

    }

    public class ReajusteSueldosService : IReajusteSueldosService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public ReajusteSueldosService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetReajusteSueldos> ListarReajusteSueldosService(int empresa, decimal reajuste)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                String RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                f.EjecutarConsultaSQLCli("SELECT personas.rut,personas.nombres,personas.apellidos,personas.email,personas.tlf, " +
                                            "contratos.id , contratos.contrato, contratos.idpersona, contratos.sueldoBase, " +
                                            "contratos.inicio, contratos.termino, contratos.idTipoContrato, contratos.idfaena,  " +
                                            "contratos.idcargo,  contratos.idcentroCosto, contratos.idjornada,  contratos.observaciones,  " +
                                            "contratos.firmaTrabajador,  contratos.firmaEmpresa, contratos.rutValidador,  contratos.fechaValidacion,  " +
                                            "contratos.rechazado,  contratos.fechaCreacion, contratos.tipoCarga,  contratos.articulo22  " +
                                           "FROM personas " +
                                            "inner join contratos on personas.id = contratos.idPersona " +
                                            "where contratos.habilitado = 1 ", BD_Cli);


                List<ContratosBaseVM> opcionesList = new List<ContratosBaseVM>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new ContratosBaseVM()
                                    {
                                        rut = dr["rut"].ToString(),
                                        nombres = dr["nombres"].ToString(),
                                        apellidos = dr["apellidos"].ToString(),
                                        sueldobase = dr["sueldoBase"].ToString(),
                                    }).ToList();
                    List<DetReajusteSueldos> lista = new List<DetReajusteSueldos>();
                    foreach(var t in opcionesList)
                    {
                        DetReajusteSueldos registro = new DetReajusteSueldos();
                        registro.rutTrabajador = t.rut;
                        registro.nombre = t.nombres + " " + t.apellidos;
                        registro.sueldo = Convert.ToDecimal(t.sueldobase);
                        registro.reajuste = registro.sueldo * reajuste/100;
                        registro.nuevo = registro.sueldo + registro.reajuste;
                        lista.Add(registro);

                    }

                    return lista;
                }
                return null;
            }
            catch (Exception ex)
            {
                var Asunto = "Error en cuadratura de habres";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar cusdrar haberes", correos.destinatarioErrores);
                return null;
            }
        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetReajusteSueldos
    {
        public String rutTrabajador { get; set; }
        public string nombre { get; set; }
        public decimal sueldo { get; set; }
        public decimal reajuste { get; set; }
        public decimal nuevo { get; set; }
    }

}

