using Microsoft.AspNetCore.Mvc;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.CuadraturaHaberes
{
    /// <summary>
    /// Servicio para generar y operar con l0s haberes informados de un trabajador.
    /// Ejem. Oficina central.
    /// </summary>
    public interface ICuadraturaHaberesService
    {
        /// <summary>
        /// Genera lista de haber informados.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes del haber informados que desea listar</param>
        /// <param name="anio">año del haber informados que desea listar</param>
        /// <param name="pago">pago del haber informados que desea listar</param>
        /// <returns>Lista de haberes informados</returns>
        public List<ResCuadraturaHaberes> ListarCuadraturaHaberesService(int idEmpresa, int mes, int anio, string pago);

    }

    public class CuadraturaHaberesService : ICuadraturaHaberesService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public CuadraturaHaberesService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<ResCuadraturaHaberes> ListarCuadraturaHaberesService(int empresa,  int mes,int anio,string pago)
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


                f.EjecutarConsultaSQLCli("SELECT haberesinformados.haber,haberesinformados.tipoCalculo, haberesinformados.monto, haberesinformados.dias " +
                                            "FROM haberesinformados " +
                                            "WHERE haberesinformados.habilitado = 1 " + 
                                            " and haberesinformados.fechaDesde <= '"+fecfinstr+"' and haberesinformados.fechaHasta >= '"+fecinistr+
                                            "' and haberesinformados.pago = '"+pago+"' ", BD_Cli);


                List<DetCuadraturaHaberes> opcionesList = new List<DetCuadraturaHaberes>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetCuadraturaHaberes()
                                    {
                                        haber = int.Parse(dr["haber"].ToString()),
                                        tipoCalculo = dr["tipoCalculo"].ToString(),
                                        monto = decimal.Parse(dr["monto"].ToString()),
                                        dias = int.Parse(dr["dias"].ToString()),
                                    }).ToList();
                    var haberes = opcionesList.GroupBy(x=> x.haber).Select(g=> new { haber = g.Key }).ToList();
                    List<ResCuadraturaHaberes> resumen = new List<ResCuadraturaHaberes>();
                    int totmonto = 0;
                    decimal totpor = 0;
                    decimal totufs = 0;
                    int totcan = 0;
                    int totdias = 0;
                    foreach (var r in haberes)
                    {
                        int codigo = r.haber;
                        var detalle = opcionesList.Where(x => x.haber == r.haber).ToList();

                        foreach (var d in detalle)
                        {
                            if(d.tipoCalculo == "M")totmonto = Convert.ToInt32(d.monto) +totmonto;
                            if (d.tipoCalculo == "P") totpor = Convert.ToDecimal(d.monto) + totpor;
                            if (d.tipoCalculo == "U") totufs = Convert.ToDecimal(d.monto) + totufs;
                            totcan++;
                            totdias += d.dias;
                        }
                        ResCuadraturaHaberes res = new ResCuadraturaHaberes();
                        res.haber = r.haber;
                        res.descripcion = f.DescripcionHaber(empresa, r.haber);
                        res.monto = totmonto;
                        res.uefes = totufs;
                        res.porcentaje = totpor;
                        res.cantidad = totcan;
                        res.dias = totdias;
                        resumen.Add(res);
                        totmonto = 0;
                        totpor = 0;
                        totufs = 0;
                        totcan = 0;
                        totdias = 0;
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
    public class DetCuadraturaHaberes
    {
        public int haber { get; set; }
        public string tipoCalculo { get; set; }
        public decimal monto { get; set; }
        public int dias { get; set; }
    }
    public class ResCuadraturaHaberes
    {
        public int haber { get; set; }
        public string descripcion { get; set; }
        public int cantidad { get; set; }
        public decimal monto { get; set; }
        public decimal porcentaje { get; set; }
        public decimal uefes { get; set; }
        public int dias { get; set; }
    }

}

