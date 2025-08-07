using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RRHH.BaseDatos.NoSQL.Permisos
{
    public class Feature
    {
        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonExtraElements]
        public Dictionary<string, object> AccessList {  get; set; } 
    }
}
