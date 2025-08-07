namespace RRHH.Models.ViewModels
{
    public class PersonasBaseVM
    {
        public int Id { get; set; }
        public string Rut { get; set; }
        public string Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string Email { get; set; }
        public string? Pais { get; set; }
        public string? Region { get; set; }
        public string? Comuna { get; set; }
        public string Tlf { get; set; }
        public int IdPais { get; set; }
        public int IdRegion { get; set; }
        public int IdComuna { get; set; }
        public string direccion { get; set; }
        public string? nacimiento { get; set; }
        public string? sexo { get; set; }
        public int nrohijos { get; set; }
    }


    public class PersonasDeleteVM
    {
        public string Id { get; set; }
    }
}

