using RRHH.Models.Utilities.Formularios;

namespace RRHH.Servicios.Formularios
{
    public interface IFormService
    {
        /// <summary>
        /// Obtiene la información de los íconos de los formularios para ser inyectada en la vista.
        /// </summary>
        /// <returns>Retorna en forma de diccionario el ícono de todos los campos de los formularios.</returns>
        public Dictionary<string, string> GetFormIconData();
    }

    public class FormService : IFormService
    {

        private List<FormItemData> _formItemData;

        public FormService() {
            _formItemData = new List<FormItemData>()
            {
                new FormItemData("RUT", "ph-identification-card"),
                new FormItemData("NOMBRES", "ph-dots-nine"),
                new FormItemData("EMAIL", "ph-envelope-simple"),
                new FormItemData("TELEFONO", "ph-phone-call"),
                new FormItemData("DIRECCION", "ph-house-line"),
                new FormItemData("GIRO", "icon-city"),
                new FormItemData("DIVISA", "ph-currency-circle-dollar"),
                new FormItemData("GENERICO", "ph-dots-nine"),
                new FormItemData("GLOSA", "ph-note-pencil"),
                new FormItemData("BALANCE", "icon-balance"),
                new FormItemData("AGREGAR", "ph-plus"),
                new FormItemData("EDITAR", "fa fa-pen")
            };
        }

        public Dictionary<string, string> GetFormIconData()
        {
            return _formItemData.ToDictionary(i => i.NombreCampo, i=> i.CodigoIcono);
        }
    }
}
