using Amazon.S3;
using Amazon.S3.Model;
using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace Desnz.Chmm.Common.Services
{
    public partial class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly IClamAvService _clamAvService;
        private readonly IOptions<EnvironmentConfig> _environmentConfig;
        private readonly IAmazonS3 _client;
        private readonly string _bucketPrefix;

        public FileService(
            IClamAvService clamAvService,
            IAmazonS3 client,
            ILogger<FileService> logger,
            IOptions<EnvironmentConfig> environmentConfig)
        {
            _clamAvService = clamAvService;
            _logger = logger;
            _environmentConfig = environmentConfig;
            _client = client;
            _bucketPrefix = $"chmm-{_environmentConfig.Value.EnvironmentName}";
        }

        public async Task<FileDownloadResponse> DownloadFileAsync(string bucketName, string key)
        {
            try
            {
                var fullBucketName = GetFullBucketName(bucketName);
                var exists = await FileExistsAsync(fullBucketName, key);
                if (!exists)
                {
                    return new FileDownloadResponse(null, null, null, key, "NotFound");
                }

                using var response = await _client.GetObjectAsync(new GetObjectRequest { BucketName = fullBucketName, Key = key });

                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    var errorMessage = string.Format("Failed to get document '{0}' from bucket '{1}'", key, fullBucketName);
                    _logger.LogError(errorMessage);
                    return new FileDownloadResponse(response, null, null, fileKey: key, error: errorMessage);
                }

                using var ms = new MemoryStream();
                await response.ResponseStream.CopyToAsync(ms);
                var contentType = response.Headers.ContentType;

                return new FileDownloadResponse(response, ms.ToArray(), contentType, key);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Error: {0} when downloading at key: {1} from bucket: {2}",
                    JsonConvert.SerializeObject(ex), key, bucketName));
                return new FileDownloadResponse(null, null, null, key, string.Format("Error when downloading at key: {0} from bucket: {1}",
                    key, bucketName));
            }
        }

        public async Task<List<string>> GetFileFullPathsAsync(string bucketName, string prefix)
            => await GetAllObjectKeysAsync(bucketName, prefix, true);

        public async Task<List<string>> GetFileNamesAsync(string bucketName, string prefix)
            => await GetAllObjectKeysAsync(bucketName, prefix, false);


        public async Task<FileUploadResponse> UploadFileAsync(string bucketName, string key, IFormFile file)
        {
            try
            {
                bucketName = GetFullBucketName(bucketName);

                if (await FileExistsAsync(bucketName, key))
                {
                    return new FileUploadResponse(null, key, "The selected file could not be uploaded – a file with the same name already exists");
                }

                if (!file.HasValidMaxFileSize(out var error))
                {
                    return new FileUploadResponse(null, key, error);
                };

                if (!file.HasValidFileType(out error))
                {
                    return new FileUploadResponse(null, key, error);
                };

                using (var fileStream = file.OpenReadStream())
                {
#if DEBUG
                    return await PutObjectAsync(bucketName, key, fileStream);
#else
                    var result = await _clamAvService.ScanAsync(key, fileStream);

                    return result.ClamResult.Result == nClam.ClamScanResults.Clean
                        ? await PutObjectAsync(bucketName, key, fileStream)
                        : new FileUploadResponse(null, key, result.ClamResult.Result.ToString());
#endif
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Error: {0} uploading object at key: {1} to bucket: {2}", JsonConvert.SerializeObject(ex), key, bucketName));
                return new FileUploadResponse(null, key, string.Format("Error uploading object at key: {0} to bucket: {1}", key, bucketName));
            }
        }

        public async Task<FileCopyResponse> CopyFileAsync(string sourceBucketName, string sourceKey, string destinationBucketName, string destinationKey)
        {
            try
            {
                sourceBucketName = GetFullBucketName(sourceBucketName);
                destinationBucketName = GetFullBucketName(destinationBucketName);
                return await CopyObjectAsync(sourceBucketName, sourceKey, destinationBucketName, destinationKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Error: {0} when copying object from bucket: {1} at key {2} to bucket: {3} at key {4}", JsonConvert.SerializeObject(ex), sourceBucketName, sourceKey, destinationBucketName, destinationKey));
                return new FileCopyResponse(null, sourceKey, string.Format("Error when copying object from bucket: {0} at key {1} to bucket: {2} at key {3}", sourceBucketName, sourceKey, destinationBucketName, destinationKey));
            }
        }

        public async Task<FileUploadResponse> DeleteObjectNonVersionedBucketAsync(string bucketName, string key)
        {
            if (key == "Quarantined.docx")
            {
                _logger.LogError("Error: Someone is trying to delete Quarantined.docx");
                return new FileUploadResponse(null, key, "Deleting Quarantined.docx is not allowed.");
            }

            bucketName = GetFullBucketName(bucketName);

            if (!await FileExistsAsync(bucketName, key))
            {
                return new FileUploadResponse(null, key, "The selected file does not exists");
            }

            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key,
            };

            try
            {
                _logger.LogInformation("Deleting object at key: {key} from bucket: {bucketName}", key, bucketName);
                var response = await _client.DeleteObjectAsync(deleteObjectRequest);
                _logger.LogInformation("Object at key: {key} deleted from bucket: {bucketName}", key, bucketName);
                return new FileUploadResponse(null, key);
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(string.Format("Error: {0} deleteting object at key: {1} from bucket: {2}", JsonConvert.SerializeObject(ex), key, bucketName));
                return new FileUploadResponse(null, key, string.Format("Error deleteting object at key: {0} from bucket: {1}", key, bucketName));
            }
        }

        #region Private methods

        private async Task<FileUploadResponse> PutObjectAsync(string bucketName, string key, Stream fileStream)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = fileStream,
                AutoCloseStream = true
            };

            try
            {
                _logger.LogInformation("Attempting to upload object to bucket: {bucketName} at key {key}", bucketName, key);
                var response = await _client.PutObjectAsync(request);

                if ((response?.HttpStatusCode == HttpStatusCode.OK))
                {
                    _logger.LogInformation("Successfully uploaded object to bucket: {bucketName} at key: {key}", bucketName, key);
                    return new FileUploadResponse(response, key);
                }
                else
                {
                    return new FileUploadResponse(response, string.Empty);
                }
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(string.Format("Error: {0} uploading object to bucket: {1} at key: {2}", JsonConvert.SerializeObject(ex), bucketName, key));
                return new FileUploadResponse(null, key, string.Format("Error uploading object to bucket: {0} at key: {1}", bucketName, key));
            }
        }

        private async Task<FileCopyResponse> CopyObjectAsync(string sourceBucketName, string sourceKey, string destinationBucketName, string destinationKey)
        {
            var request = new CopyObjectRequest
            {
                SourceBucket = sourceBucketName,
                SourceKey = sourceKey,
                DestinationBucket = destinationBucketName,
                DestinationKey = destinationKey
            };

            CopyObjectResponse? response;
            try
            {
                _logger.LogInformation("Attempting to copy object from bucket: {sourceBucketName} at key {sourceKey} to bucket: {destinationBucketName} at key {destinationKey}", sourceBucketName, sourceKey, destinationBucketName, destinationKey);
                response = await _client.CopyObjectAsync(request);
                if (response?.HttpStatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("Successfully copied object from bucket: {sourceBucketName} at key {sourceKey} to bucket: {destinationBucketName} at key {destinationKey}", sourceBucketName, sourceKey, destinationBucketName, destinationKey);
                    return new FileCopyResponse(response, destinationKey);
                }
                else
                {
                    return new FileCopyResponse(response, string.Empty);
                }
                
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(string.Format("Error: {0} when copying object from bucket: {1} at key {2} to bucket: {3} at key {4}", JsonConvert.SerializeObject(ex), sourceBucketName, sourceKey, destinationBucketName, destinationKey));
                return new FileCopyResponse(null, sourceKey, string.Format("Error when copying object from bucket: {0} at key {1} to bucket: {2} at key {3}", sourceBucketName, sourceKey, destinationBucketName, destinationKey));
            }
        }

        private async Task<bool> FileExistsAsync(string bucketName, string key)
        {
            try
            {
                _logger.LogInformation("Checking if object from bucket: {bucketName} exists at key: {key} by retrieving its metadata", bucketName, key);
                var metadataResponse = await _client.GetObjectMetadataAsync(bucketName, key);
                _logger.LogInformation("Successfully retrieved object metadata from bucket: {bucketName} at key: {key}", bucketName, key);
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
                _logger.LogError("Error: {error} getting object at key: {key} from bucket: {bucket}", JsonConvert.SerializeObject(ex), key, bucketName);
                throw;
            }
            return true;
        }

        private string GetFullBucketName(string bucketName) => $"{_bucketPrefix}-{bucketName}";

        private async Task<List<string>> GetAllObjectKeysAsync(string bucketName, string prefix, bool returnFullPath)
        {
            bucketName = GetFullBucketName(bucketName);
            IList<string> fileNames = new List<string>();
            try
            {
                _logger.LogInformation("Getting all object keys from bucket: {bucketName}", bucketName);
                fileNames = await _client.GetAllObjectKeysAsync(bucketName, prefix, null);
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError("Error: {error} getting all object keys from bucket: {bucketName}", JsonConvert.SerializeObject(ex), bucketName);
                throw;
            }

            return returnFullPath ?
                fileNames.ToList() :
                fileNames.Select(fileName => fileName.Split('/').Last()).ToList();
        }

        #endregion
    }
}
