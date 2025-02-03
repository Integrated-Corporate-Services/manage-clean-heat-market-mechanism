using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.Common.Services
{
    internal interface IFileValidationService
    {
        string? ValidateMaxFileSize(IFormFile file);
        string? ValidateFileType(IFormFile file);
    }
}
