namespace RRHH.Models.Estructuras.Cuentas
{
    public class SubCuenta
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public bool Habilitado { get; set; }
        public List<CuentaContable> CuentasContables;

        public SubCuenta() { Init(); }

        private void Init()
        {
            CuentasContables = new List<CuentaContable>();  
        }

        public void CrearCuentaContable(int id, string glosa, string codigo)
        {
            CuentasContables.Add(new CuentaContable(id, glosa, codigo, true));
        }
    }
}
