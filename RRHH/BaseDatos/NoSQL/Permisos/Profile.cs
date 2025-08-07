using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RRHH.BaseDatos.NoSQL.Permisos
{
    public class Profile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        public string? Title { get; set; }

        [BsonElement("EmpresaId")]
        public string? EmpresaId { get; set; }

        [BsonElement("Modules")]
        public List<Module> Modules { get; set; }
    }
}
