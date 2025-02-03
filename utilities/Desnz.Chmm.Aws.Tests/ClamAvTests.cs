using Desnz.Chmm.Aws.Tests.Helpers;
using Desnz.Chmm.Common.Enums;
using Desnz.Chmm.Common.ValueObjects;
using Microsoft.AspNetCore.Http;
using nClam;
using System.Net.Sockets;
using System.Text;

namespace Desnz.Chmm.ClamAV.Tests
{
    public class ClamAvTests
    {
        private static string _server = ""; // This is liable to change.

        [Fact(Skip ="TODO")]
        public void CanCheckFile()
        {
            var file = FileHelper.CreateTestFormFile("TestFile.txt");
            ScanResult result;
            using (var stream = file.OpenReadStream())
            {
                result = ScanAsync("No Logging", "TestFile.txt", stream).Result;
            }

            Assert.Equal(ClamScanResults.Clean, result.ClamResult.Result);
        }

        public async Task<ScanResult> ScanAsync(string msgPrefix, string fileName, Stream stream, CancellationToken cancellationToken = default)
        {
            var scanResult = new ClamScanResult("OK");
            if (!string.IsNullOrWhiteSpace(_server))
            {
                // _logger.LogWarning($"{msgPrefix} Virus scanning: {fileName} Size: {stream.Length} Bytes");
                var timer = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    if (stream.Length < (long)2 * 1024 * 1024 * 1024)
                    {
                        scanResult = await SendAndScanFileAsync(msgPrefix, fileName, stream, cancellationToken);
                    }
                    else
                    {
                        var maxChunkLength = 512 * 1024 * 1024;
                        var bytesRead = 1;
                        while (scanResult.Result == ClamScanResults.Clean && bytesRead > 0)
                        {
                            Stream chunkStream = new MemoryStream();
                            bytesRead = CopyStream(stream, chunkStream, maxChunkLength);
                            if (bytesRead > 0)
                            {
                                scanResult = await SendAndScanFileAsync(msgPrefix, fileName, chunkStream, cancellationToken);
                                // if this is a full chunk move the file pointer back a bit so there is an overlap
                                if (scanResult.Result == ClamScanResults.Clean && bytesRead == maxChunkLength)
                                {
                                    stream.Seek(-1024 * 1024, SeekOrigin.Current);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // _logger.LogError(ex, $"{msgPrefix} Virus scanning: {fileName} Exception: {ex.HResult} {ex.Message}");
                    scanResult = new ClamScanResult("Error");
                }
                // _logger.LogWarning($"{msgPrefix} Virus scanning: {fileName} Completed");

                timer.Stop();
                stream.Seek(0, SeekOrigin.Begin);

                var properties = new Dictionary<string, string>
                    {{"FileName",fileName}};
                var metrics = new Dictionary<string, double>
                    {
                        {"FileSize", stream.Length},
                        {"TimeTaken", timer.Elapsed.TotalMilliseconds},
                        {"ScanResult", (double)scanResult.Result},
                        {"BytesPerMS", stream.Length / timer.Elapsed.TotalMilliseconds}
                    };
            }

            return ClamScanResulttoScanResult(msgPrefix, fileName, scanResult);
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

        private ScanResult ClamScanResulttoScanResult(string msgPrefix, string fileName, ClamScanResult scanResult)
        {
            return new ScanResult(fileName, scanResult);
        }

        private async Task<ClamScanResult> SendAndScanFileAsync(string msgPrefix, string fileName, Stream stream, CancellationToken cancellationToken = default)
        {
            ClamScanResult scanResult = new ClamScanResult("OK");
            var sending = SendState.First;
            while (sending != SendState.Finished)
            {
                try
                {
                    long maxStreamSize = (long)4 * 1024 * 1024 * 1024;
                    ClamClient client = new ClamClient(_server, 3310)
                    {
                        MaxStreamSize = maxStreamSize
                    };
                    scanResult = await client.SendAndScanFileAsync(stream, cancellationToken);
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