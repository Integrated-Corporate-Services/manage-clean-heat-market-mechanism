using Desnz.Chmm.Common.Configuration.Settings;
using Desnz.Chmm.Common.Enums;
using Desnz.Chmm.Common.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using nClam;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace Desnz.Chmm.Common.Services
{
    public class ClamAvService : IClamAvService
    {
        private readonly IOptions<ClamAvConfig> _clamAvConfig;
        private readonly ILogger<ClamAvService> _logger;
        public ClamAvService(IOptions<ClamAvConfig> clamAvConfig, ILogger<ClamAvService> logger)
        {
            _clamAvConfig = clamAvConfig;
            _logger = logger;
        }

        public async Task<ScanResult> ScanAsync(string fileName, Stream stream)
        {
            ClamScanResult? scanResult = null;

            if (string.IsNullOrWhiteSpace(_clamAvConfig.Value.ClamAvServiceIp))
            {
                _logger.LogCritical("Clam AV IP address is not set");
                throw new SystemException("Clam AV is not configured");
            }

            _logger.LogInformation($"Virus scanning: {fileName} Size: {stream.Length} Bytes");
            var timer = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                stream.Seek(0, SeekOrigin.Begin);

                if (stream.Length < (long)2 * 1024 * 1024 * 1024)
                {
                    scanResult = await SendAndScanFileAsync(fileName, stream);
                }
                else
                {
                    var maxChunkLength = 512 * 1024 * 1024;
                    var bytesRead = 1;
                    while (scanResult == null || (scanResult.Result == ClamScanResults.Clean && bytesRead > 0))
                    {
                        Stream chunkStream = new MemoryStream();
                        bytesRead = CopyStream(stream, chunkStream, maxChunkLength);
                        if (bytesRead > 0)
                        {
                            scanResult = await SendAndScanFileAsync(fileName, chunkStream);
                            // if this is a full chunk move the file pointer back a bit so there is an overlap
                            if (scanResult != null && scanResult.Result == ClamScanResults.Clean && bytesRead == maxChunkLength)
                            {
                                stream.Seek(-1024 * 1024, SeekOrigin.Current);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Virus scanning: {fileName} Exception: {ex.HResult} {ex.Message}");
                scanResult = new ClamScanResult("Error");
            }
            _logger.LogInformation($"Virus scanning: {fileName} Completed");

            timer.Stop();
            stream.Seek(0, SeekOrigin.Begin);

            var metrics = new Dictionary<string, double>
                    {
                        {"FileSize", stream.Length},
                        {"TimeTaken", timer.Elapsed.TotalMilliseconds},
                        {"ScanResult", scanResult == null ? 0 : (double)scanResult.Result},
                        {"BytesPerMS", stream.Length / timer.Elapsed.TotalMilliseconds}
                    };
            var jsonMetrics = JsonConvert.SerializeObject(metrics);
            _logger.LogInformation($"Scan result: {fileName} - {jsonMetrics}");

            return ClamScanResulttoScanResult(fileName, scanResult);
        }

        static int CopyStream(Stream stream, Stream chunkStream, int maxChunkLength)
        {
            int totalBytesRead = 0;
            int bytesRead;

            byte[] buffer = new byte[maxChunkLength];

            while ((bytesRead = stream.Read(buffer, 0, Math.Min(maxChunkLength, (int)(stream.Length - stream.Position)))) > 0)
            {
                chunkStream.Write(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;

                if (totalBytesRead >= maxChunkLength)
                {
                    break;
                }
            }

            return totalBytesRead;
        }

        private ScanResult ClamScanResulttoScanResult(string fileName, ClamScanResult? scanResult)
        {
            return new ScanResult(fileName, scanResult);
        }

        private async Task<ClamScanResult?> SendAndScanFileAsync(string fileName, Stream stream)
        {
            ClamScanResult? scanResult = null;
            var sending = SendState.First;
            while (sending != SendState.Finished)
            {
                try
                {
                    long maxStreamSize = (long)4 * 1024 * 1024 * 1024;
                    ClamClient client = new ClamClient(_clamAvConfig.Value.ClamAvServiceIp, 3310)
                    {
                        MaxStreamSize = maxStreamSize
                    };
                    scanResult = await client.SendAndScanFileAsync(stream);
                    sending = SendState.Finished;
                }
                catch (SocketException ex)
                {
                    if (sending == SendState.First)
                    {
                        sending = SendState.Retry;
                    }
                    else
                    {
                        scanResult = new ClamScanResult("Error");
                        sending = SendState.Finished;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return scanResult;
        }
    }
}
