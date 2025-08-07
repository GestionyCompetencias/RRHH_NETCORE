namespace RRHH.Models.ViewModels
{
    public class EmpresasVM
    {
        public int id { get; set; }
        public string rut { get; set; }
        public string razonsocial { get; set; }
        public string? fantasia { get; set; } 
        public string giro { get; set; }
        public int idPais { get; set; }
        public int idregion { get; set; }
        public int idcomuna { get; set; }
        public int idrepresentante { get; set; }
        public int idencargado { get; set; }
        public string email { get; set; }
        public string obs { get; set; }
        public string? padre { get; set; } 
        public string? direccion { get; set; } 
        public string? rutencargado { get; set; } 
        public string? rutrepresentante { get; set; } 
    }

    public class ListEmpresasVM
    {
        public int id { get; set; }
        public string rut { get; set; }
        public string razonsocial { get; set; }
    }

    public class EmpresasDtoVM
    {
        public int id { get; set; }
        public string rut { get; set; }
        public string razonsocial { get; set; }
        public string? fantasia { get; set; }
        public string giro { get; set; }
        public string? direccion { get; set; }
        public int idPais { get; set; }
        public int idregion { get; set; }
        public int idcomuna { get; set; }
        public string? rutrepresentante { get; set; }
        public string? nombresrepresentante { get; set; }
        public string? apellidosrepresentante { get; set; }
        public string? rutencargado { get; set; }
        public string? nombresencargado { get; set; }
        public string? apellidosencargado { get; set; }
        public string? emailencargado { get; set; }
        public string? emailrepresentante { get; set; }
        public string email { get; set; }
        public string obs { get; set; }
        public string? padre { get; set; }                   
        
    }

    public class EmpresasConsultaVM
    {
        public int id { get; set; }
        public string rut { get; set; }
        public string razonsocial { get; set; }
        public string? fantasia { get; set; }
        public string giro { get; set; }
        public int idPais { get; set; }
        public int idregion { get; set; }
        public int idcomuna { get; set; }
        public int idrepresentante { get; set; }
        public int idencargado { get; set; }
        public string email { get; set; }
        public string obs { get; set; }
        public string? padre { get; set; }
        public string? direccion { get; set; }
    }
}
