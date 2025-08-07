using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RRHH.BaseDatos.NoSQL.Permisos
{
    public class Module
    {
        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("SubModules")]
        public List<SubModule> SubModules { get; set; }
    }
}
