namespace RRHH.Models.Utilities.Configuration
{
    public class DeployEnvironment
    {
        public string Name {  get; set; }   
        public GlobalDatabase GlobalDatabase { get; set; }
        public AzureBlobStorageAccount AzureBlobStorageAccount { get; set; }
        public MongoDatabaseSettings MongoDatabaseSettings { get; set; }

        public static bool isProduction()
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                return true;

            return false;
        }

        public static string GetEnvironmentString()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

		}
    }
}
