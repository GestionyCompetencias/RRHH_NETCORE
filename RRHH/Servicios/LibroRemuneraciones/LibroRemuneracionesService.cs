using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.Servicios.LibroRemuneraciones
{
    /// <summary>
    /// Servicio para generar el libro de remuenraciones.
    /// Ejem. Oficina central.
    /// </summary>
    public interface ILibroRemuneracionesService
    {
        /// <summary>
        /// Genera libro de remuneraciones.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="mes">mes del haber informados que desea listar</param>
        /// <param name="anio">año del haber informados que desea listar</param>
        /// <returns>Lista de haberes informados</returns>
        public List<DetLibro> ListarLibroRemuneracionesService(int idEmpresa, int mes, int anio);

    }

    public class LibroRemuneracionesService : ILibroRemuneracionesService
    {
        private readonly IDatabaseManager _databaseManager;
		private Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public LibroRemuneracionesService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetLibro> ListarLibroRemuneracionesService(int empresa,  int mes,int anio)
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


                f.EjecutarConsultaSQLCli("SELECT resultados.rutTrabajador,resultados.concepto,resultados.monto,resultados.cantidad " +
                                            "FROM resultados " +
                                            "WHERE resultados.habilitado = 1 " + 
                                            " and resultados.fechaPago <= '"+fecfinstr+"' and resultados.fechaPago >= '"+fecinistr+
                                            "' and resultados.pago = 'L' ", BD_Cli);


                List<DetResultado> opcionesList = new List<DetResultado>();
                if (f.Tabla.Rows.Count > 0)
                {

                    opcionesList = (from DataRow dr in f.Tabla.Rows
                                    select new DetResultado()
                                    {
                                        rutTrabajador = dr["rutTrabajador"].ToString(),
                                        concepto = int.Parse(dr["concepto"].ToString()),
                                        monto = decimal.Parse(dr["monto"].ToString()),
                                        cantidad = decimal.Parse(dr["cantidad"].ToString()),
                                    }).ToList();
                    var trabajadores = opcionesList.GroupBy(x=> x.rutTrabajador).Select(g=> new { rutTrabajador = g.Key }).ToList();
                    List<DetLibro> libro = new List<DetLibro>();
                    foreach (var t in trabajadores)
                    {
                        string rut = t.rutTrabajador;
                        var detalle = opcionesList.Where(x => x.rutTrabajador == rut).ToList();
                        DetLibro linea = new DetLibro();
                        Decimal dctva = 0;

                        foreach (var r in detalle)
                        {
                            linea.rut = rut;
                            linea.nombre = f.NombrePersona(empresa, rut);
                            linea.rut = linea.rut.Substring(0, linea.rut.Length - 1) + "-" + linea.rut.Substring(linea.rut.Length - 1, 1);
                            if (r.concepto == 1)
                            {
                                linea.sbase = (int)r.monto;
                                linea.diast = (int)r.cantidad;
                            }
                            if (r.concepto == 3) linea.hrsex = (int)r.monto;
                            if (r.concepto == 6) linea.grati = (int)r.monto;
                            if (r.concepto == 2010) linea.totim = (int)r.monto;
                            if (r.concepto == 14) linea.famil = (int)r.monto;
                            if (r.concepto == 2020) linea.otron = (int)r.monto;
                            if (r.concepto == 910) linea.pensi = (int)r.monto;
                            if (r.concepto == 921) linea.salud = (int)r.monto;
                            if (r.concepto == 990) linea.impto = (int)r.monto;
                            if (r.concepto == 916) linea.segce = (int)r.monto;
                            if (r.concepto >= 100 && r.concepto < 900) dctva = dctva + (int)r.monto;
                            if (r.concepto == 2404) linea.otrol = 0;
                            if (r.concepto == 2250) linea.dctva = (int)r.monto;
                            if (r.concepto == 2300) linea.liqui = (int)r.monto;
                        }
                        linea.dctva = (int)dctva;
                        decimal otroi = Convert.ToDecimal(linea.totim) - Convert.ToDecimal(linea.sbase) - Convert.ToDecimal(linea.hrsex) - Convert.ToDecimal(linea.grati);
                        linea.otroi = (int)otroi;
                        decimal otron = Convert.ToDecimal(linea.otron) - Convert.ToDecimal(linea.famil);
                        linea.otron = (int)otron;
                        libro.Add(linea);

                    }
                    return libro;
                }
                else
                {
                 return null;  
                }
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error en libro de remuneraciones";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al generar el libro de remuneraciones", correos.destinatarioErrores);
                return null;
            }

        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetLibro
    {

        public string rut { get; set; }
        public string nombre { get; set; }
        public int diast { get; set; }
        public int sbase { get; set; }
        public int hrsex { get; set; }
        public int grati { get; set; }
        public int otroi { get; set; }
        public int totim { get; set; }
        public int famil { get; set; }
        public int otron { get; set; }
        public int pensi { get; set; }
        public int salud { get; set; }
        public int segce { get; set; }
        public int impto { get; set; }
        public int otrol { get; set; }
        public int dctva { get; set; }
        public int liqui { get; set; }
    }
    public class DetResultado
    {
        public string rutTrabajador { get; set; }
        public int concepto { get; set; }
        public decimal monto { get; set; }
        public decimal informado { get; set; }
        public decimal cantidad { get; set; }
    }

}

