using Amazon.Runtime;
using Amazon.S3.Model;

namespace Desnz.Chmm.Common.Services;

public partial class FileService
{
    public class FileUploadResponse(PutObjectResponse? response, string? fileKey, string? error = null) : FileKeyResponse<PutObjectResponse>(response, fileKey, error)
    {
    }


    public class FileCopyResponse(CopyObjectResponse? response, string? fileKey, string? error = null) : FileKeyResponse<CopyObjectResponse>(response, fileKey, error)
    {
    }

    public class FileDownloadResponse(GetObjectResponse? response, byte[] fileContent, string contentType, string? fileKey, string? error = null) : FileKeyResponse<GetObjectResponse>(response, fileKey, error)
    {
        public byte[] FileContent { get; } = fileContent;

        public string ContentType { get; } = contentType;
    }

    public abstract class FileKeyResponse<T>(T? response, string? fileKey, string? error = null) : FileResponse<T>(response) where T : AmazonWebServiceResponse
    {
        public string? FileKey { get; } = fileKey;
        public string? ValidationError { get; } = error;
    }

    public abstract class FileResponse<T>(T? response) where T : AmazonWebServiceResponse
    {
        public T? Response { get; protected set; } = response;
    }
}