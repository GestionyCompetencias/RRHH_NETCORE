namespace RRHH.Models.ViewModels
{
    public class LicenciasMedicasBaseVM
    {
        public int id { get; set; }
        public string codigolicencia { get; set; }
        public string rutTrabajador { get; set; }
        public DateTime fechainicio { get; set; }
        public DateTime fechatermino { get; set; }
        public int dias { get; set; }
        public int tipolicencia { get; set; }
        public string comentario { get; set; }
        public int tipomedico { get; set; }
        public int habilitado { get; set; }
        public BinaryData pdf { get; set; }
    }


    public class LicenciaDeleteVM
    {
        public string Id { get; set; }
    }
}

