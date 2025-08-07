namespace RRHH.Models.ViewModels
{
    public class ConversionContableBaseVM
    {
        public int Id { get; set; }
        public string modulo { get; set; }
        public string pago { get; set; }
        public int concepto { get; set; }
        public string cuenta { get; set; }
        public int tipoauxiliar { get; set; }
        public string codigoauxiliar { get; set; }
        public string debehaber { get; set; }
        public int tipovencimiento { get; set; }
        public int diavencimiento { get; set; }
        public int mesvencimiento { get; set; }
        public int agrupacion { get; set; }
        public int provision { get; set; }
        public int habilitado { get; set; }
    }


    public class ConversionContableDeleteVM
    {
        public string Id { get; set; }
    }
}

