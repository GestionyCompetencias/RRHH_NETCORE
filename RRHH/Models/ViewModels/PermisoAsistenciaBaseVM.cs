namespace RRHH.Models.ViewModels
{
    public class PermisoAsistenciaBaseVM
    {
        public int id { get; set; }
        public string ruttrabajador { get; set; }
        public DateTime fechainicio { get; set; }
        public DateTime fechatermino { get; set; }
        public int goseSueldo { get; set; }
        public string comentario { get; set; }
        public int habilitado { get; set; }
    }


    public class PermisoDeleteVM
    {
        public string Id { get; set; }
    }
}

