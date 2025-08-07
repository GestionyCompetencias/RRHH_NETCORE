namespace RRHH.Models.Estructuras.Cuentas
{
    public class SubGrupo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public bool Habilitado { get; set; }
        public List<Cuenta> Cuentas;

        public SubGrupo() { Init(); }

        private void Init()
        {
            Cuentas = new List<Cuenta>();
        }

        public Cuenta AgregarCuenta(Cuenta cuenta)
        {
            Cuentas.Add(cuenta);
            return GetCuenta(cuenta.Nombre);
        }

        public Cuenta GetCuenta(string nombreCuenta)
        {
            return Cuentas.Where(c => c.Nombre.Equals(nombreCuenta)).FirstOrDefault();
        }
    }
}
