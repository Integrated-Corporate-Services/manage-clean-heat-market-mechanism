using Microsoft.AspNetCore.Http;
using static Desnz.Chmm.Common.Services.FileService;

namespace Desnz.Chmm.Common.Services
{
    public interface IFileService
    {
        Task<List<string>> GetFileNamesAsync(string bucketName, string prefix);
        Task<List<string>> GetFileFullPathsAsync(string bucketName, string prefix);
        Task<FileUploadResponse> UploadFileAsync(string bucketName, string key, IFormFile file);
        Task<FileCopyResponse> CopyFileAsync(string sourceBucketName, string sourceKey, string destinationBucketName, string destinationKey);
        Task<FileUploadResponse> DeleteObjectNonVersionedBucketAsync(string bucketName, string key);
        Task<FileDownloadResponse> DownloadFileAsync(string bucketName, string key);
    }
}
