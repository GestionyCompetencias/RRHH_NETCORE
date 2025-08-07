namespace RRHH.Models.ViewModels
{
    public class DescuentosInformadosBaseVM
    {
        public int id { get; set; }
        public int descuento { get; set; }
        public string rutTrabajador { get; set; }
        public int correlativo { get; set; }
        public string afecta { get; set; }
        public int haber { get; set; }
        public string pago { get; set; }
        public string tipoCalculo { get; set; }
        public decimal monto { get; set; }
        public string fechaDesde { get; set; }
        public string fechaHasta { get; set; }
        public string fechaIngreso { get; set; }
        public int pagina { get; set; }
        public int habilitado { get; set; }
    }


    public class DescuentosInformadosDeleteVM
    {
        public string Id { get; set; }
    }
}

