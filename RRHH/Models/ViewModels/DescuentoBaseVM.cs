namespace RRHH.Models.ViewModels
{
    public class DescuentoBaseVM
    {
        public int id { get; set; }
        public int descuento { get; set; }
        public string descripcion { get; set; }
        public int prioridad { get; set; }
        public int minimo { get; set; }
        public int maximo { get; set; }
        public string codigoDT { get; set; }
        public string codigoprevired { get; set; }
        public int habilitado { get; set; }
    }


    public class DescuentoDeleteVM
    {
        public string Id { get; set; }
    }
}

