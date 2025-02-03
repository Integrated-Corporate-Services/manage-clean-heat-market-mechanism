using Desnz.Chmm.Common.ValueObjects;

namespace Desnz.Chmm.Common.Services
{
    public interface IClamAvService
    {
        Task<ScanResult> ScanAsync(string fileName, Stream stream);
    }
}
