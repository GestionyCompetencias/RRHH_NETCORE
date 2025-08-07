namespace RRHH.Models.ViewModels
{
    public class FaenasBaseVM
    {
        public int Id { get; set; }
        public string Contrato { get; set; }
        public string Descripcion { get; set; }
        public string Inicio { get; set; }
        public string Termino { get; set; }
        public string Direccion { get; set; }
        public string? Pais { get; set; }
        public string? Region { get; set; }
        public string? Comuna { get; set; }
        public int idPais { get; set; }
        public int idRegion { get; set; }
        public int idComuna { get; set; }
    }


    public class FaenasDeleteVM
    {
        public string Id { get; set; }
    }
}

