namespace RRHH.Models.Utilities.Configuration
{
	public class AzureBlobStorageAccount
	{
		public string AccountName { get; set; }
		public string AccountKey { get; set; }

		public AzureBlobStorageAccount(string accountName, string accountKey)
		{
			AccountName = accountName;
			AccountKey = accountKey;
		}
	}
}
