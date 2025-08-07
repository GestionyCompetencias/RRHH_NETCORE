using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RRHH.BaseDatos.NoSQL.Permisos
{
    public class SubModule
    {
        [BsonElement("Title")]
        public string Title {  get; set; }

        [BsonElement("Features")]
        public List<Feature> Features { get; set; }
    }
}
