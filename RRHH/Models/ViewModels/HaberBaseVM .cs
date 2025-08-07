namespace RRHH.Models.ViewModels
{
    public class HaberBaseVM
    {
        public int id { get; set; }
        public int haber { get; set; }
        public string descripcion { get; set; }
        public string imponible { get; set; }
        public string tributable { get; set; }
        public int numeromeses { get; set; }
        public string garantizado { get; set; }
        public string retenible { get; set; }
        public string calculado { get; set; }
        public string tiempo { get; set; }
        public string deducible { get; set; }
        public string baselicencia { get; set; }
        public string basesobretiempo { get; set; }
        public string baseindemnizacion{ get; set; }
        public string basevariable { get; set; }
        public string codigoDT { get; set; }
        public string codigoprevired { get; set; }
        public int habilitado { get; set; }
    }


    public class HaberDeleteVM
    {
        public string Id { get; set; }
    }
}

