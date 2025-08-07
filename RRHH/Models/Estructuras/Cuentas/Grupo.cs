namespace RRHH.Models.Estructuras.Cuentas
{
    public class Grupo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public bool Habilitado { get; set; }
        public List<SubGrupo> SubGrupos;
        public Grupo() { Init(); }

        private void Init()
        {
            SubGrupos = new List<SubGrupo>();
        }

        public SubGrupo AgregarSubGrupo(SubGrupo subGrupo)
        {
            SubGrupos.Add(subGrupo);
            return GetSubGrupo(subGrupo.Nombre);
        }

        public SubGrupo GetSubGrupo(string nombreSubGrupo)
        {
            return SubGrupos.Where(s => s.Nombre.Equals(nombreSubGrupo)).FirstOrDefault();
        }
    }
}
