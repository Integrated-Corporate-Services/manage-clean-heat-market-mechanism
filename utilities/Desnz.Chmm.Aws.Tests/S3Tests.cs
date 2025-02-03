using Amazon.S3.Model;
using Amazon.S3;
using Desnz.Chmm.Aws.Tests.Helpers;

namespace Desnz.Chmm.ClamAV.Tests
{
    public class S3Tests
    {
        [Fact(Skip = "TODO")]
        public void CanUploadFile()
        {
            // Setup
            var bucketName = "chmm-dev-boilersales";
            var fileName = "testFile.txt";

            IAmazonS3 client = new AmazonS3Client();

            // Act
            var success = UploadFileAsync(client, bucketName , fileName, "testFile.txt").Result;

            // Assert
            Assert.True(success);

            // Cleanup
            DeleteObjectNonVersionedBucketAsync(client, bucketName, fileName).Wait();
        }


        private async Task<bool> UploadFileAsync(
        IAmazonS3 client,
        string bucketName,
        string objectName,
        string filePath)
        {
            var file = FileHelper.CreateTestFormFile(filePath);

            PutObjectRequest request;
                PutObjectResponse response;
            using (var stream = file.OpenReadStream())
            {
                request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectName,
                    InputStream = stream
                };

                response = await client.PutObjectAsync(request);
            }
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Successfully uploaded {objectName} to {bucketName}.");
                return true;
            }
            else
            {
                Console.WriteLine($"Could not upload {objectName} to {bucketName}.");
                return false;
            }
        }

        public async Task DeleteObjectNonVersionedBucketAsync(IAmazonS3 client, string bucketName, string keyName)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                };

                Console.WriteLine($"Deleting object: {keyName}");
                await client.DeleteObjectAsync(deleteObjectRequest);
                Console.WriteLine($"Object: {keyName} deleted from {bucketName}.");
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error encountered on server. Message:'{ex.Message}' when deleting an object.");
            }
        }
    }
}