using Microsoft.AspNetCore.Mvc;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.CuadraturaDescuentos
{
    /// <summary>
    /// Servicio para generar y operar con l0s descuentos informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface ICuadraturaDescuentosService
    {
        /// <summary>
        /// Genera cuadratura de descuentos.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes de cuadratura</param>
        /// <param name="anio">año de cuadratura</param>
        /// <param name="pago">pago de cuedratura</param>
        /// <returns>Lista de cuadratura de descuentos</returns>
        public List<ResCuadraturaDescuentos> ListarCuadraturaDescuentosService(int idEmpresa, int mes, int anio, string pago);

    }

    public class CuadraturaDescuentosService : ICuadraturaDescuentosService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public CuadraturaDescuentosService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<ResCuadraturaDescuentos> ListarCuadraturaDescuentosService(int empresa,  int mes,int anio,string pago)
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


                f.EjecutarConsultaSQLCli("SELECT descuentosinformados.descuento, descuentosinformados.monto " +
                                            "FROM descuentosinformados " +
                                            "WHERE descuentosinformados.habilitado = 1 " + 
                                            " and descuentosinformados.fechaDesde <= '"+fecfinstr+"' and descuentosinformados.fechaHasta >= '"+fecinistr+
                                            "' and descuentosinformados.pago = '"+pago+"' ", BD_Cli);


                List<DetCuadraturaDescuentos> opcionesList = new List<DetCuadraturaDescuentos>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetCuadraturaDescuentos()
                                    {
                                        descuento = int.Parse(dr["descuento"].ToString()),
                                        monto = decimal.Parse(dr["monto"].ToString())
                                    }).ToList();
                    var descuentos = opcionesList.GroupBy(x=> x.descuento).Select(g=> new { descuento = g.Key }).ToList();
                    List<ResCuadraturaDescuentos> resumen = new List<ResCuadraturaDescuentos>();
                    int totmonto = 0;
                    int totcan = 0;
                    foreach (var r in descuentos)
                    {
                        int codigo = r.descuento;
                        var detalle = opcionesList.Where(x => x.descuento == r.descuento).ToList();

                        foreach (var d in detalle)
                        {
                            totmonto = Convert.ToInt32(d.monto) +totmonto;
                            totcan++;
                        }
                        ResCuadraturaDescuentos res = new ResCuadraturaDescuentos();
                        res.descuento = r.descuento;
                        res.descripcion = f.DescripcionDescuentos(empresa, r.descuento);
                        res.monto = totmonto;
                        res.cantidad = totcan;
                        resumen.Add(res);
                        totmonto = 0;
                        totcan = 0;
                    }
                    return resumen;
                }
                else
                {
                 return null;  
                }
            }
            catch (Exception ex)
            {
                var Asunto = "Error en cuadratura de descuentos";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar cuadrar descuentos", correos.destinatarioErrores);
                return null;
            }

        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetCuadraturaDescuentos
    {
        public int descuento { get; set; }
        public string tipocalculo { get; set; }
        public decimal monto { get; set; }
    }
    public class ResCuadraturaDescuentos
    {
        public int descuento { get; set; }
        public string descripcion { get; set; }
        public decimal monto { get; set; }
        public int cantidad { get; set; }
    }

}

