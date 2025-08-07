namespace RRHH.Models.Estructuras.Cuentas
{
    public class Cuenta
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public bool Habilitado { get; set; }
        public List<SubCuenta> SubCuentas;

        public Cuenta() { Init(); }

        private void Init()
        {
            SubCuentas = new List<SubCuenta>();
        }

        public SubCuenta AgregarSubCuenta(SubCuenta subCuenta)
        {
            SubCuentas.Add(subCuenta);
            return GetSubCuenta(subCuenta.Nombre);
        }

        public SubCuenta GetSubCuenta(string nombreSubCuenta)
        {
            return SubCuentas.Where(s => s.Nombre.Equals(nombreSubCuenta)).FirstOrDefault();
        }
    }
}
