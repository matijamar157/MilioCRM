using AFuturaCRMV2.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AFuturaCRMV2.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _connectionString;
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
        {
            _connectionString = configuration.GetConnectionString("AzureStorage");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(_connectionString);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

            await cloudBlobContainer.CreateIfNotExistsAsync();
            await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            var blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);

            using (var stream = file.OpenReadStream())
            {
                await cloudBlockBlob.UploadFromStreamAsync(stream);
            }

            return blobName;
        }

        public async Task<bool> DeleteFileAsync(string fileName, string containerName)
        {
            try
            {
                var cloudStorageAccount = CloudStorageAccount.Parse(_connectionString);
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
                var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

                return await cloudBlockBlob.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FileName} from container {ContainerName}", fileName, containerName);
                return false;
            }
        }

        public string GetFileUrl(string fileName, string containerName)
        {
            return $"https://prodajmepictures.blob.core.windows.net/{containerName}/{fileName}";
        }
    }
}
