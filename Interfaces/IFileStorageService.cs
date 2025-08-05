namespace AFuturaCRMV2.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string containerName);
        Task<bool> DeleteFileAsync(string fileName, string containerName);
        string GetFileUrl(string fileName, string containerName);
    }
}
