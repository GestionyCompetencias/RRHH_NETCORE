namespace RRHH.Models.ViewModels
{
    public class BancosVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class BancoProvL
    {
        public int Idregistro { get; set; }
        public int Idbanco { get; set; }
        public string Nombrebanco { get; set; }
        public int Idtipo { get; set; }
        public string Nombretipo { get; set; }
        public string Numero { get; set; }
    }

    public class BancoProvG
    {
        public int Idpers { get; set; }
        public int Idbanco { get; set; }
        public int Idtipo { get; set; }
        public string Numero { get; set; }
    }
}
