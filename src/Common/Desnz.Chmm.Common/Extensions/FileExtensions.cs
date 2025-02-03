
using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.Common.Extensions
{
    public static class FileExtensions
    {
        private const int MaxFileSize = 5242880;
        private static readonly IReadOnlyCollection<string> PermittedExtensions = new List<string>() { ".doc", ".docx", ".pdf", ".ppt", ".pptx", ".xls", ".csv", ".xlsx", ".jpg", ".png", ".bmp", ".eml", ".msg" };

        public static bool HasValidMaxFileSize(this IFormFile file, out string? error)
        {
            error = null;

            if (file.Length > MaxFileSize)
            {
                error = $"The selected file must be smaller than 5MB";
                return false;
            }

            return true;
        }

        public static bool HasValidFileType(this IFormFile file, out string? error)
        {
            error = null;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !PermittedExtensions.Contains(extension))
            {
                error = $"The selected file must be a .doc, .docx, .pdf, .ppt, .pptx, .xls, .csv, .xlsx, .jpg, .png, .bmp, .eml, or .msg";
                return false;
            }

            return true;
        }
    }
}
