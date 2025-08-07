namespace RRHH.Models.ViewModels
{
    public class MarcacionBaseVM
    {

        public string rutTrabajador { get; set; }
        public DateTime marca { get; set; }
        public string tipoMarca  { get; set; }
        public int diaSemana { get; set; }
        public string sensorId { get; set; }
        public int modificada { get; set; }
        public int habilitado { get; set; }
    }


    public class MarcacionDeleteVM
    {
        public string Id { get; set; }
    }
}

