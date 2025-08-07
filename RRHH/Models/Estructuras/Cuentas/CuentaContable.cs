namespace RRHH.Models.Estructuras.Cuentas
{
    public class CuentaContable
    {
        public int Id { get; set; }
        public string Glosa { get; set; }
        public string Codigo { get; set; }
        public bool Habilitado { get; set; }

        public CuentaContable(int id, string glosa, string codigo, bool habilitado) 
        {
            Id = id;
            Glosa = glosa;
            Codigo = codigo;
            Habilitado = habilitado;
        }
    }
}
