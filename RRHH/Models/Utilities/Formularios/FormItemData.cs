namespace RRHH.Models.Utilities.Formularios
{
    public class FormItemData
    {
        public string NombreCampo { get; set; }   
        public string CodigoIcono { get; set; }

        public FormItemData(string nombreCampo, string codigoIcono) { 
            NombreCampo = nombreCampo;
            CodigoIcono = codigoIcono;
        }
    }
}
