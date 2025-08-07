namespace RRHH.Models.ViewModels
{
    public class Resultados
    {
        public int id { get; set; }
        public string rutTrabajador{ get; set; }
        public string pago { get; set; }
        public int concepto { get; set; }
        public string descripcion { get; set; }
        public decimal cantidad{ get; set; }
        public decimal informado { get; set; }
        public decimal monto { get; set; }
    }
}
