namespace RRHH.Models.ViewModels
{
    public class ParametrosBaseVM
    {
        public int Id { get; set; }
        public string Tabla { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Valor { get; set; }
        public string Fecha { get; set; }
        public string Inicio { get; set; }
        public string Termino { get; set; }
    }


    public class ParametrosDeleteVM
    {
        public string Id { get; set; }
    }
}

