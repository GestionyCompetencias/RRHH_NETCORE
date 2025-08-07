namespace RRHH.Models.Estructuras.Permisos
{
    public class Perfil
    {

        public Perfil() {
            Accesos = new List<Acceso>();
        }
        ~Perfil() { }

        public string Id { get; set; }
        public string Title { get; set; }
        public int EmpresaId { get; set; }
        public List<Acceso> Accesos { get; set; }

        public void AddAccessValue(string modulo, string submodulo, string funcion, string nombreAcceso, string value)
        {
             Accesos.Add(new Acceso(modulo, submodulo, funcion, nombreAcceso, value));
        }

        public void RemoveAccessValue(string modulo, string submodulo, string funcion, string nombreAcceso)
        {
            var acc = Accesos.Where(a => a.Modulo == modulo && a.SubModulo == submodulo &&
                a.Funcion == funcion && a.NombreAcceso == nombreAcceso).FirstOrDefault();

            if (acc != null) 
                Accesos.Remove(acc);
        }

        public string GetAccessValue(string modulo, string submodulo, string funcion, string nombreAcceso)
        {
            var acc = Accesos.Where(a => a.Modulo == modulo && a.SubModulo == submodulo &&
                a.Funcion == funcion && a.NombreAcceso == nombreAcceso).FirstOrDefault();
            return acc != null ? acc.Value.ToString() : String.Empty;
        }
    }
}
