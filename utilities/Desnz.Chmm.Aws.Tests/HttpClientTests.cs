namespace Desnz.Chmm.Aws.Tests
{
    public class HttpClientTests
    {
        /// <summary>
        /// This test was written to debug an issue with the Api Clients having the /development/ dropped from the base uri.
        /// </summary>
        [Fact]
        public void BaseAddress()
        {
            var baseUri = new Uri("https://xxxxxxxxxx.execute-api.eu-west-2.amazonaws.com/development/");
            var request = new HttpRequestMessage(HttpMethod.Get, "api/identity/token");

            var endUri = new Uri(baseUri, request.RequestUri);

            Assert.Equal("https://xxxxxxxxxx.execute-api.eu-west-2.amazonaws.com/development/api/identity/token", endUri.AbsoluteUri);
        }
    }
}