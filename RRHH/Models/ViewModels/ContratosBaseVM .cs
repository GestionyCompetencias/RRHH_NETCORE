namespace RRHH.Models.ViewModels
{
    public class ContratosBaseVM
    {
        public int id { get; set; }
        public int idPersona { get; set; }
        public string rut { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string tipocontrato { get; set; }
        public int idtipocontrato { get; set; }
        public string contrato { get; set; }
        public string inicio { get; set; }
        public string termino { get; set; }
        public string faena { get; set; }
        public int idfaena { get; set; }
        public string cargo { get; set; }
        public int idcargo { get; set; }
        public string centrocosto { get; set; }
        public int idcentrocosto { get; set; }
        public string jornada { get; set; }
        public int idjornada { get; set; }
        public string sueldobase { get; set; }
        public string observaciones { get; set; }
        public string firmatrabajador { get; set; }
        public string firmaempresa { get; set; }
        public string tipocarga { get; set; }
        public string articulo22 { get; set; }
        public int idbancotrab { get; set; }
        public int idafptrab { get; set; }
        public int idisapretrab { get; set; }
    }
    public class BancoTrabajadorVM
    {
        public string id { get; set; }
        public string descripcion { get; set; }
        public string tipoCuenta { get; set; }
        public string numeroCuenta { get; set; }
    }
    public class afpsTrabajadorVM
    {
        public string id { get; set; }
        public string descripcion { get; set; }
        public string tipoApv { get; set; }
        public string apv { get; set; }
    }
    public class isapreTrabajadorVM
    {
        public string id { get; set; }
        public string descripcion { get; set; }
        public string ufs { get; set; }
    }


    public class ContratosDeleteVM
    {
        public string Id { get; set; }
    }
}

