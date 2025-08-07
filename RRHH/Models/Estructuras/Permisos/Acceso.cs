namespace RRHH.Models.Estructuras.Permisos
{
    public class Acceso
    {

        public Acceso(string modulo, string submodulo, string funcion, string nombreAcceso, string value)
        { 
            Modulo = modulo; SubModulo = submodulo; Funcion = funcion; NombreAcceso = nombreAcceso; Value = value; 
        }
            
        public string Modulo { get; set; }
        public string SubModulo { get; set; }
        public string Funcion { get; set; }
        public string NombreAcceso {  get; set; }   
        public string Value {  get; set; }
    }
}
