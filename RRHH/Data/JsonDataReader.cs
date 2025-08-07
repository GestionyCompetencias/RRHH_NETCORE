using MongoDB.Bson.Serialization;

namespace RRHH.Data
{
    public interface IJsonDataReader
    {
        /// <summary>
        /// Convierte un archivo JSON en un objeto T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns>Objeto T</returns>
        public T DeserializeData<T>(string filePath);
    }

    public class JsonDataReader : IJsonDataReader
    {
        public T DeserializeData<T>(string filePath)
        {
            string jsonText = File.ReadAllText(filePath);
            return BsonSerializer.Deserialize<T>(jsonText);
        }
    }
}
