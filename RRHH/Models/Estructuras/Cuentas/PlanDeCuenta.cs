namespace RRHH.Models.Estructuras.Cuentas
{
    public class PlanDeCuenta
    {
        public List<Grupo> GruposContables;

        public PlanDeCuenta() { Init(); }

        private void Init()
        {
            GruposContables = new List<Grupo>();
        }

        public Grupo AgregarGrupo(Grupo grupo)
        {
            GruposContables.Add(grupo);
            return GetGrupo(grupo.Nombre);
        }

        public Grupo GetGrupo(string nombreGrupo)
        {
            return GruposContables.Where(g => g.Nombre.Equals(nombreGrupo)).FirstOrDefault();         
        }
    }
}
