namespace RRHH.Models.ViewModels
{
    public class JornadasBaseVM
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public int diasTrabajo { get; set; }
        public int diasDescanso { get; set; }
        public int numeroCiclos { get; set; }
        public int horasSemanales { get; set; }
        public string fechaCreacion { get; set; }
        public string resolucion { get; set; }
        public string fechaResolucion { get; set; }
    }


    public class JornadasDeleteVM
    {
        public string Id { get; set; }
    }
}

