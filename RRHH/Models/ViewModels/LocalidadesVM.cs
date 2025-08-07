namespace RRHH.Models.ViewModels
{
    public class LocalidadesVM
    {
        public int idPais { get; set; }
        public string nombre { get; set; }
    }

    public class RegionesVM
    {
        public int idPais { get; set; }
        public int idRegion { get; set; }
        public string nombre { get; set; }
    }

    public class ComunaVM
    {
        public int idRegion { get; set; }
        public int idComuna { get; set; }
        public string nombre { get; set; }
    }
}
