namespace RRHH.Models.ViewModels
{
    public class AfpsTrabajadorBaseVM
    {
        public int id { get; set; }
        public int idpersona { get; set; }
        public int codigoAfp { get; set; }
        public string descripcion { get; set; }
        public string fechainicio { get; set; }
        public string tipoApv { get; set; }
        public string formaApv { get; set; }
        public decimal apv { get; set; }
    }
}

