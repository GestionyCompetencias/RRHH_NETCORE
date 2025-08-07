namespace RRHH.Models.ViewModels
{
    public class AsistenciasInformadasBaseVM
    {
        public int id { get; set; }
        public string rutTrabajador { get; set; }
        public string fechaAsistencia { get; set; }
        public string codigoInasis  { get; set; }
        public int dias { get; set; }
        public Decimal horasExtras1 { get; set; }
        public Decimal horasExtras2 { get; set; }
        public Decimal horasExtras3 { get; set; }
        public int diasColacion { get; set; }
        public Decimal horasColacion { get; set; }
        public int diasMovilizacion { get; set; }
        public int habilitado { get; set; }
    }


    public class AsistenciasInformadasDeleteVM
    {
        public string Id { get; set; }
    }
}

