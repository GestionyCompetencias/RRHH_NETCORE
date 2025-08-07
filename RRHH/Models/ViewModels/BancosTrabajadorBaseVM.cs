namespace RRHH.Models.ViewModels
{
    public class BancosTrabajadorBaseVM
    {
        public int id { get; set; }
        public int idbanco { get; set; }
        public string descripcionBanco { get; set; }
        public int idpersona { get; set; }
        public string fechainicio { get; set; }
        public int idtipocta { get; set; }
        public string descripcionTipo { get; set; }
        public string numerocuenta { get; set; }
    }
}

