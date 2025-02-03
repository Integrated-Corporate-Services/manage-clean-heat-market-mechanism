
namespace Desnz.Chmm.Common.Dtos
{
    public class AwsFileDto
    {
        /// <summary>
        /// File key (AWS reference)
        /// </summary>
        public string FileKey { get; private set; }

        /// <summary>
        /// Name of file
        /// </summary>
        public string FileName { get; private set; }

        public AwsFileDto(string fileName, string fileKey)
        {
            FileName = fileName;
            FileKey = fileKey;
        }
    }
}
