using System.ComponentModel.DataAnnotations;

namespace RRHH.Models.ViewModels
{
    public class UsuarioVM
    {
        public int id { get; set; }
        [Required]
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string? usuraio { get; set; }
        public string? contra { get; set; }
        public string? perfil { get; set; }
        [Required]
        public string email { get; set; }
        public string telef { get; set; }
        public int idPerfil { get; set; }
    }

    public class recupContraVM
    {
        [Required]
        public string email { get; set; }
    }

    public class editContraVM
    {
        [Required]
        public int id { get; set; }
        public int idEmpresa { get; set; }
        [Required]
        public string contra { get; set; }
    }

    public class resultLoginVM
    {
        public int idUsu { get; set; }
        public string token { get; set; }

        public List<userEmpre> UsuarioEmpresas { get; set; }
        public resultLoginVM()
        {
            UsuarioEmpresas = new List<userEmpre>();
        }

        public resultLoginVM(int _idUsu, string _token)
        {
            idUsu = _idUsu;
            token = _token;
        }
    }

    public class userEmpre
    {
        public int idempre { get; set; }

    }
}
