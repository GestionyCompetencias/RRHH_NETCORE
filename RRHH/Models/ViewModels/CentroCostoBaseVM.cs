namespace RRHH.Models.ViewModels
{
    public class CentrosCostosBaseVM
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public string rutJefe { get; set; }
        public string observaciones { get; set; }
    }


    public class CentrosCostosDeleteVM
    {
        public string Id { get; set; }
    }
}

