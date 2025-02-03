using nClam;

namespace Desnz.Chmm.Common.ValueObjects
{
    public class ScanResult
    {
        public string FileName;
        public ClamScanResult? ClamResult;

        public ScanResult(string fileName, ClamScanResult? scanResult)
        {
            FileName = fileName;
            ClamResult = scanResult;
        }
    }
}
