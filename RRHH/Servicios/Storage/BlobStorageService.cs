using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Data.SqlClient;
using Rollbar;
using RRHH.BaseDatos;
using RRHH.Models.Utilities.Configuration;
using System.Data;

namespace RRHH.Servicios.Storage
{
    public enum BlobDirectory
	{
		Empresas
	}

	/// <summary>
	/// Maneja todas las operaciones relacionadas al servicio de blobs de Azure.
	/// </summary>
	public interface IBlobStorageService
	{
		/// <summary>
		/// Sube un archivo a Azure Blob Service y lo registra en la base de datos de una empresa en particular.
		/// </summary>
		/// <param name="fileStream">FileStream del archivo a subir.</param>
		/// <param name="idEmpresa">ID de la empresa.</param>
		/// <param name="filename">Nombre del archivo.</param>
		/// <param name="fileExtension">Extensión del archivo.</param>
		/// <param name="className">Nombre en la base de datos de la tabla a la cual se va a asociar el documento.</param>
		/// <param name="classId">ID del objeto al cual se asociará el archivo.</param>
		public void UploadToEmpresa(MemoryStream fileStream, int idEmpresa, string filename, string fileExtension, string className, int classId);
		/// <summary>
		/// Sube un archivo a Azure Blob Service en un directorio especifico de la empresa. Registra también esta información en la base de datos.
		/// </summary>
		/// <param name="fileStream">FileStream del archivo a subir.</param>
		/// <param name="idEmpresa">ID de la empresa.</param>
		/// <param name="filename">Nombre del archivo.</param>
		/// <param name="fileExtension">Extensión del archivo.</param>
		/// <param name="className">Nombre en la base de datos de la tabla a la cual se va a asociar el documento.</param>
		/// <param name="classId">ID del objeto al cual se asociará el archivo.</param>
		/// <param name="directoryName">Nombre del directorio a subir el archivo.</param>
		public void UploadToEmpresaDirectory(MemoryStream fileStream, int idEmpresa, string filename, string fileExtension, string className, int classId, string directoryName);
		/// <summary>
		/// Genera un URL firmado con una llave que otorga permisos de lectura del archivo a un usuario autorizado. SOLO sirve para modelos que tengan solo 1 archivo asociado.
		/// </summary>
		/// <param name="className">Nombre de la tabla SQL asociada al archivo.</param>
		/// <param name="classId">ID del objeto asociado al archivo.</param>
		/// <returns>URL para leer el archivo.</returns>
		public string GenerarUriArchivoEmpresa(string className, int classId, int idEmpresa);
	}

	public class BlobStorageService : IBlobStorageService
	{
		private readonly IDatabaseManager _databaseManager;
		private readonly IConfiguration _configuration;
		private DeployEnvironment _deployEnvironment;
		private string _connectionString;
		private string _containerName;

		public BlobStorageService(IDatabaseManager databaseManager, IConfiguration configuration)
		{
			_databaseManager = databaseManager;
			_configuration = configuration;
			_deployEnvironment = _configuration.GetSection("Environments:" + DeployEnvironment.GetEnvironmentString()).Get<DeployEnvironment>();

			// Inicializar datos necesarios
			ConfigurarConnectionString();
			ConfigurarNombreContenedor();
		}

		public async void UploadToEmpresa(MemoryStream fileStream, int idEmpresa, string filename, string fileExtension, string className, int classId)
		{
			// Ruta final del archivo a subir
			string finalBlobPath = GetDirectoryString(BlobDirectory.Empresas) + idEmpresa.ToString();
			string finalFilename = finalBlobPath + "\\" + String.Format("{0}-{1}-{2}.{3}", className, classId.ToString(), filename, fileExtension);
		
			// Conexión al servicio de blobs
			BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
			BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
			BlobClient blobClient = blobContainerClient.GetBlobClient(finalFilename);

			// Subir archivo
			fileStream.Position = 0;
			await blobClient.UploadAsync(fileStream, true);

			// Guardar esta información en la base de datos de la empresa
			string query = "INSERT INTO blobInfo (className, classId, filename, fileExtension, blobUrl) " +
				"VALUES (@className, @classId, @filename, @fileExtension, @blobUrl);";

			List<SqlParameter> sqlParameters = new List<SqlParameter>()
			{
				new SqlParameter("@className", className),
				new SqlParameter("@classId", classId.ToString()),
				new SqlParameter("@filename", filename),
				new SqlParameter("@fileExtension", fileExtension),
				new SqlParameter("@blobUrl", finalFilename)
			};

			bool databaseOperationResult = _databaseManager.ExecuteEmpresaTransaction(query, idEmpresa, sqlParameters);

			if (!databaseOperationResult)
			{
				// Log en rollbar solo si es ambiente de producción
				if (DeployEnvironment.isProduction())
					RollbarLocator.RollbarInstance.Error("Error al registrar blob en la database de la empresa con ID = " + idEmpresa.ToString());

			}
		}

		public async void UploadToEmpresaDirectory(MemoryStream fileStream, int idEmpresa, string filename, string fileExtension, string className, int classId, string directoryName)
		{
			// Ruta final del archivo a subir
			string relativeBlobPath = GetDirectoryString(BlobDirectory.Empresas) + idEmpresa.ToString();
			string finalBlobPath = relativeBlobPath +  "\\" + directoryName;
			string finalFilename = finalBlobPath + "\\" + String.Format("{0}-{1}-{2}.{3}", className, classId.ToString(), filename, fileExtension);

			// Conexión al servicio de blobs
			BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
			BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
			BlobClient blobClient = blobContainerClient.GetBlobClient(finalFilename);

			// Subir archivo
			fileStream.Position = 0;
			try
			{
				await blobClient.UploadAsync(fileStream, true);
			}
			catch (Exception ex) {
				RollbarLocator.RollbarInstance.Error(ex);
			}

			// Guardar esta información en la base de datos de la empresa
			string query = "INSERT INTO blobInfo (className, classId, filename, fileExtension, blobUrl) " +
				"VALUES (@className, @classId, @filename, @fileExtension, @blobUrl);";

			List<SqlParameter> sqlParameters = new List<SqlParameter>()
			{
				new SqlParameter("@className", className),
				new SqlParameter("@classId", classId.ToString()),
				new SqlParameter("@filename", filename),
				new SqlParameter("@fileExtension", fileExtension),
				new SqlParameter("@blobUrl", finalFilename)
			};

			bool databaseOperationResult = _databaseManager.ExecuteEmpresaTransaction(query, idEmpresa, sqlParameters);

			if (!databaseOperationResult)
			{
				// Log en rollbar solo si es ambiente de producción
				if (DeployEnvironment.isProduction())
					RollbarLocator.RollbarInstance.Error("Error al registrar blob en la database de la empresa con ID = " + idEmpresa.ToString() + " en el directorio = " + directoryName);

			}
		}

		public string GenerarUriArchivoEmpresa(string className, int classId, int idEmpresa) 
		{
			// Obtener URL del blob
			string query = "SELECT * FROM blobInfo WHERE className = @className AND classId = @classId";

			List<SqlParameter> sqlParameters = new List<SqlParameter>()
			{
				new SqlParameter("@className", className),
				new SqlParameter("@classId", classId)
			};

			DataTable dt = _databaseManager.ExecuteEmpresaQuery(query, idEmpresa, sqlParameters);

			// Si no existe un registro asociado, devolver un valor nulo
			if (dt.Rows.Count == 0) return null;

			// Obtener el URL de la blob desde la base de datos.
			string blobUrl = dt.Rows[0]["blobUrl"].ToString();

			// Conexión al servicio de blobs
			BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
			BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
			BlobClient blobClient = blobContainerClient.GetBlobClient(blobUrl);

			// Crear SAS (Shared Access Signature)
			// Es una firma que da autorización a un cliente para acceder a un archivo
			BlobSasBuilder sasBuilder = new BlobSasBuilder();
			sasBuilder.ExpiresOn =  DateTimeOffset.Now.ToUniversalTime().AddSeconds(60);
			sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

			// Generar Uri
			return blobClient.GenerateSasUri(sasBuilder).ToString();
		}


		// --- HELPERS ---
		private void ConfigurarConnectionString()
		{
			_connectionString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};" +
				"AccountKey={1};EndpointSuffix=core.windows.net", 
				_deployEnvironment.AzureBlobStorageAccount.AccountName, 
				_deployEnvironment.AzureBlobStorageAccount.AccountKey);
		}

		private void ConfigurarNombreContenedor()
		{
			string environmentName = DeployEnvironment.GetEnvironmentString();

			// TODO: Agregar blob storage en producción
			switch (environmentName)
			{
				case "Development":
					_containerName = "gycsol-development";
					break;
				case "Beta":
					_containerName = "gycsol-development";
					break;
				default:
					_containerName = "gycsol-development";
					break;
			}
		}

		private string GetDirectoryString(BlobDirectory blobDirectory)
		{
			string directoryString = null;
			switch(blobDirectory)
			{
				case BlobDirectory.Empresas:
					directoryString = String.Format("empresas\\");
					break;
				default:
					directoryString = "";
					break;
			}

			return directoryString;
		}
		
	}
}
