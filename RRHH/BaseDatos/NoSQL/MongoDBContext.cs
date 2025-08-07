using MongoDB.Driver;

namespace RRHH.BaseDatos.NoSQL
{
    public interface IMongoDBContext
    {
        /// <summary>
        /// Obtiene una colección en MongoDB utilizando su modelo.
        /// </summary>
        /// <typeparam name="T">Modelo MongoDB</typeparam>
        /// <param name="collectionName">Nombre del modelo</param>
        /// <returns>Colección utilizando el nombre del modelo.</returns>
        public IMongoCollection<T> GetCollection<T>(string collectionName);
    }

    public class MongoDBContext : IMongoDBContext
    {
        private readonly IMongoDatabase _db; 
        private readonly IConfiguration _configuration;

        public MongoDBContext(IConfiguration configuration)
        {
            _configuration = configuration;

            // Si por alguna razón el environment no se encuentra, asumimos que es dev
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            env = env != null ? "Environments:" + env : "Environments:Development";

            var client = new MongoClient(_configuration[$"{env}:MongoDatabaseSettings:ConnectionString"]);
            _db = client.GetDatabase(_configuration[$"{env}:MongoDatabaseSettings:DatabaseName"]);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _db.GetCollection<T>(collectionName);
        }
    }
}
