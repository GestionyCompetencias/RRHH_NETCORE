namespace RRHH.Models.ViewModels
{
    public class CargosBaseVM
    {
        public int id { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public string inicio { get; set; }
        public string termino { get; set; }
        public string requisitos { get; set; }
        public string funciones { get; set; }
        public int sueldoMinimo { get; set; }
        public int sueldoMaximo { get; set; }
    }


    public class CargosDeleteVM
    {
        public string Id { get; set; }
    }
}

