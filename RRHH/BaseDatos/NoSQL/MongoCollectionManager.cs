using RRHH.BaseDatos.NoSQL.Permisos;
using MongoDB.Driver;

namespace RRHH.BaseDatos.NoSQL
{
    public interface IMongoCollectionManager
    {
        /// <summary>
        /// Se utiliza esta función para obtener la colección sin necesidad de proporcionar el nombre de la colección. 
        /// De esta manera solo utilizamos el nombre del modelo, ya que el nombre de la colección está propenso a modificaciones.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Devuelve una colección MongoDB del tipo solicitado.</returns>
        public IMongoCollection<T> GetCollection<T>();
    }

    public class MongoCollectionManager : IMongoCollectionManager
    {
        private readonly IMongoDBContext _mongoDBContext;
        public MongoCollectionManager(IMongoDBContext mongoDBContext)
        {
            _mongoDBContext = mongoDBContext;
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            Type collectionType = typeof(T);

            if (collectionType == typeof(Profile))
            {
                return (IMongoCollection<T>)_mongoDBContext.GetCollection<Profile>("RRHH_permisos");
            }

            // Si el tipo no existe en la base MongoDB, devolver nulo
            return null;
        }
    }
}
