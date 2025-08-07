namespace RRHH.Models.Estructuras.Template.Componentes
{
    public class Breadcrumb
    {
        public Breadcrumb(string modulo, string subModulo) { 
            Modulo = modulo;
            SubModulo = subModulo;
        }

        public string Modulo;
        public string SubModulo;
    }
}
