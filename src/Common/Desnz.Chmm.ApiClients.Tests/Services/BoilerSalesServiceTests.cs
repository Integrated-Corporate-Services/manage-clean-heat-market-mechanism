using Amazon.Runtime;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Sockets;
using Xunit;

namespace Desnz.Chmm.ApiClients.Tests.Services
{
    public class BoilerSalesServiceTests
    {
        private const string BEARER_TOKEN_VALUE = "bearer-token";

        /// <summary>
        /// Class used to mock the AuthenticationProperties class
        /// </summary>
        public class MockAuthenticationProperties : AuthenticationProperties
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            public MockAuthenticationProperties() 
            {
                // The Token Key Prefix was taken from the source code for AuthenticationTokenExtensions
                Items.Add(".Token.chmm_token", BEARER_TOKEN_VALUE);
            }
        }

        /// <summary>
        /// Class used to mock the AuthenticateResult class
        /// </summary>
        public class MockAuthenticateResult : AuthenticateResult
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            public MockAuthenticateResult()
            {
                Properties = new MockAuthenticationProperties();
            }
        }

        private static async Task ExecuteTest(Func<Mock<HttpMessageHandler>, BoilerSalesService, Task> test)
        {
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(httpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://dummy.com")
            };
            var authenticateResult = new MockAuthenticateResult();
            var authenticationService = new Mock<IAuthenticationService>(MockBehavior.Strict);
            authenticationService.Setup(mock => mock.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
                .ReturnsAsync(authenticateResult);
            var requestServices = new Mock<IServiceProvider>(MockBehavior.Strict);
            requestServices.Setup(mock => mock.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationService.Object);
            var httpContext = new Mock<HttpContext>(MockBehavior.Strict);
            httpContext.SetupGet(mock => mock.RequestServices)
                .Returns(requestServices.Object);
            var httpRequest = new Mock<HttpRequest>(MockBehavior.Strict);
            var mockHeaderDictionary = new Mock<IHeaderDictionary>();
            httpRequest.SetupGet(x => x.Headers).Returns(mockHeaderDictionary.Object);
            httpContext.SetupGet(x => x.Request).Returns(httpRequest.Object);
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.SetupGet(mock => mock.HttpContext)
                .Returns(httpContext.Object);
            var service = new BoilerSalesService(httpClient, httpContextAccessor.Object);
            await test(httpMessageHandler, service);
        }

        private static bool Validate(HttpRequestMessage m, HttpMethod expectedMethod, string expectedPath)
        {
            Assert.Equal(expectedMethod, m.Method);
            Assert.Equal(expectedPath, m.RequestUri?.PathAndQuery);
            Assert.NotNull(m.Headers.Authorization);
            Assert.Equal("Bearer", m.Headers.Authorization.Scheme);
            Assert.Equal(BEARER_TOKEN_VALUE, m.Headers.Authorization.Parameter);
            return true;
        }

        private static async Task ExecuteHandlesProblemTest<T>(Func<BoilerSalesService, Task<HttpObjectResponse<T>>> test)
        {
            await ExecuteTest(async (httpMessageHandler, service) =>
            {
                // Arrange
                var json = @"{
                    ""Type"": ""BadRequest"", 
                    ""Title"": ""Test Error"",
                    ""TraceId"": ""1234567890abc"",
                    ""Status"": 400, 
                    ""Detail"": ""Name is async mandatory field and must be supplied"",
                    ""Errors"": {
                        ""Key1"": [ ""Value1"", ""Value2"" ],
                        ""Key2"": [ ""Value3"" ],
                        ""Key3"": [ ""Value4"", ""Value5"", ""Value6"" ]
                    },
                    ""ExceptionDetails"": [
                        { ""Message"": ""Test1"", ""Type"": ""Test2"", ""Raw"": ""Test3"" }
                    ]
                }";

                httpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(json) })
                    .Verifiable();

                // Act
                var result = await test(service);

                // Assert
                Assert.NotNull(result);
                Assert.Null(result.Result);
                Assert.NotNull(result.Problem);
                Assert.Equal("Name is async mandatory field and must be supplied", result.Problem.Detail);
                Assert.NotNull(result.Problem.Errors);
                Assert.NotEmpty(result.Problem.Errors);
                Assert.Equal(3, result.Problem.Errors.Count);
                Assert.NotNull(result.Problem.ExceptionDetails);
                Assert.NotEmpty(result.Problem.ExceptionDetails);
                Assert.Single(result.Problem.ExceptionDetails);
                Assert.Equal(400, result.Problem.Status);
                Assert.Equal("Test Error", result.Problem.Title);
                Assert.Equal("1234567890abc", result.Problem.TraceId);
                Assert.Equal("BadRequest", result.Problem.Type);

                httpMessageHandler.Verify();
                httpMessageHandler.VerifyNoOtherCalls();
            });
        }

        private static async Task ExecuteHandlesErrorResponseTest<T>(Func<BoilerSalesService, Task<HttpObjectResponse<T>>> test)
        {
            await ExecuteTest(async (httpMessageHandler, service) =>
            {
                // Arrange
                var content = "<Error><Type>BadRequest</Type><Message>Test Error</Message></Error>";

                httpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(content) })
                    .Verifiable();

                // Act
                var result = await test(service);

                // Assert
                Assert.NotNull(result);
                Assert.Null(result.Result);
                Assert.NotNull(result.Problem);
                Assert.Equal("Unknown error occurred", result.Problem.Detail);
                Assert.Null(result.Problem.Errors);
                Assert.Null(result.Problem.ExceptionDetails);
                Assert.Equal(500, result.Problem.Status);
                Assert.Equal("Error", result.Problem.Title);
                Assert.Null(result.Problem.TraceId);
                Assert.Null(result.Problem.Type);

                httpMessageHandler.Verify();
                httpMessageHandler.VerifyNoOtherCalls();
            });
        }

        private static async Task ExecuteHandlesErrorTest<T>(Func<BoilerSalesService, Task<HttpObjectResponse<T>>> test)
        {
            await ExecuteTest(async (httpMessageHandler, service) =>
            {
                // Arrange
                httpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ThrowsAsync(new SocketException())
                    .Verifiable();

                // Act

                // Assert
                await Assert.ThrowsAsync<SocketException>(() => test(service));
            });
        }

        #region GetAnnualBoilerSales

        /// <summary>
        /// Tests that the GetAnnualBoilerSales method makes a request to the expected endpoint and handles a successful request
        /// </summary>
        [Fact]
        public async Task BoilerSalesService_GetAnnualBoilerSales_Retrieves_Annual_Boiler_Sales()
        {
            await ExecuteTest(async (httpMessageHandler, service) => 
            {
                // Arrange
                var organisationId = new Guid("11111111-1111-1111-1111-111111111111");
                var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");
                var expectedMethod = HttpMethod.Get;
                var expectedPath = $"/api/boilersales/organisation/{organisationId}/year/{schemeYearId}/annual";
                var json = @"{
                    ""SchemeYearId"": """ + schemeYearId.ToString() + @""", 
                    ""OrganisationId"": """ + organisationId.ToString() + @""", 
                    ""Gas"": 500, 
                    ""Oil"": 250,
                    ""Status"": ""Submitted""
                }";

                httpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(m => Validate(m, expectedMethod, expectedPath)), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(json) })
                    .Verifiable();

                // Act
                var result = await service.GetAnnualBoilerSales(organisationId, schemeYearId);

                // Assert
                Assert.NotNull(result);
                Assert.NotNull(result.Result);
                Assert.Equal(schemeYearId, result.Result.SchemeYearId);
                Assert.Equal(organisationId, result.Result.OrganisationId);
                Assert.Equal("Submitted", result.Result.Status);
                Assert.Equal(500, result.Result.Gas);
                Assert.Equal(250, result.Result.Oil);

                httpMessageHandler.Verify();
                httpMessageHandler.VerifyNoOtherCalls();
            });
        }

        /// <summary>
        /// Tests that the GetAnnualBoilerSales method handles "problem" errors caught by the API
        /// </summary>
        [Fact]
        public async Task BoilerSalesService_GetAnnualBoilerSales_Handles_Problem()
        {
            await ExecuteHandlesProblemTest(service =>
            {
                // Arrange
                var organisationId = new Guid("11111111-1111-1111-1111-111111111111");
                var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");

                // Act
                return service.GetAnnualBoilerSales(organisationId, schemeYearId);

                // Assert
            });
        }

        /// <summary>
        /// Tests that the GetAnnualBoilerSales method handles errors returned by the API
        /// </summary>
        [Fact]
        public async Task BoilerSalesService_GetAnnualBoilerSales_Handles_Error_Response()
        {
            await ExecuteHandlesErrorResponseTest(service =>
            {
                // Arrange
                var organisationId = new Guid("11111111-1111-1111-1111-111111111111");
                var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");

                // Act
                return service.GetAnnualBoilerSales(organisationId, schemeYearId);

                // Assert
            });
        }

        /// <summary>
        /// Tests that the GetAnnualBoilerSales method handles an error when calling the API
        /// </summary>
        [Fact]
        public async Task BoilerSalesService_GetAnnualBoilerSales_Handles_Error()
        {
            await ExecuteHandlesErrorTest(service =>
            {
                // Arrange
                var organisationId = new Guid("11111111-1111-1111-1111-111111111111");
                var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");

                // Act
                return service.GetAnnualBoilerSales(organisationId, schemeYearId);

                // Assert
            });
        }

        #endregion GetAnnualBoilerSales

        #region GetQuarterlyBoilerSales

        /// <summary>
        /// Tests that the GetQuarterlyBoilerSales method makes a request to the expected endpoint and handles a successful response
        /// </summary>
        [Fact]
        public async Task BoilerSalesService_GetQuarterlyBoilerSales_Retrieves_Quarterly_Boiler_Sales()
        {
            await ExecuteTest(async (httpMessageHandler, service) =>
            {
                // Arrange
                var organisationId = new Guid("11111111-1111-1111-1111-111111111111");
                var schemeYearQuarterId = new Guid("22222222-2222-2222-2222-222222222222");
                var expectedMethod = HttpMethod.Get;
                var expectedPath = $"/api/boilersales/organisation/{organisationId}/year/{schemeYearQuarterId}/quarters";
                var json = @"[{
                    ""SchemeYearQuarterId"": """ + schemeYearQuarterId.ToString() + @""", 
                    ""OrganisationId"": """ + organisationId.ToString() + @""", 
                    ""Gas"": 500, 
                    ""Oil"": 250,
                    ""Status"": ""Submitted""
                }]";

                httpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(m => Validate(m, expectedMethod, expectedPath)), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(json) })
                    .Verifiable();

                // Act
                var result = await service.GetQuarterlyBoilerSales(organisationId, schemeYearQuarterId);

                // Assert
                Assert.NotNull(result);
                Assert.NotNull(result.Result);
                Assert.Single(result.Result);
                Assert.Equal(schemeYearQuarterId, result.Result[0].SchemeYearQuarterId);
                Assert.Equal(organisationId, result.Result[0].OrganisationId);
                Assert.Equal("Submitted", result.Result[0].Status);
                Assert.Equal(500, result.Result[0].Gas);
                Assert.Equal(250, result.Result[0].Oil);

                httpMessageHandler.Verify();
                httpMessageHandler.VerifyNoOtherCalls();
            });
        }

        /// <summary>
        /// Tests that the GetQuarterlyBoilerSales method handles "problem" responses returned by the API
        /// </summary>
        [Fact]
        public async Task BoilerSalesService_GetQuarterlyBoilerSales_Handles_Problem()
        {
            await ExecuteHandlesProblemTest(service =>
            {
                // Arrange
                var organisationId = new Guid("11111111-1111-1111-1111-111111111111");
                var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");

                // Act
                return service.GetQuarterlyBoilerSales(organisationId, schemeYearId);

                // Assert
            });
        }

        /// <summary>
        /// Tests that the GetQuarterlyBoilerSales method handles errors when calling the API
        /// </summary>
        [Fact]
        public async Task BoilerSalesService_GetQuarterlyBoilerSales_Handles_Error_Response()
        {
            await ExecuteHandlesErrorResponseTest(service =>
            {
                // Arrange
                var organisationId = new Guid("11111111-1111-1111-1111-111111111111");
                var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");

                // Act
                return service.GetQuarterlyBoilerSales(organisationId, schemeYearId);

                // Assert
            });
        }

        /// <summary>
        /// Tests that the GetQuarterlyBoilerSales method handles an error when calling the API
        /// </summary>
        [Fact]
        public async Task BoilerSalesService_GetQuarterlyBoilerSales_Handles_Error()
        {
            await ExecuteHandlesErrorTest(service =>
            {
                // Arrange
                var organisationId = new Guid("11111111-1111-1111-1111-111111111111");
                var schemeYearId = new Guid("22222222-2222-2222-2222-222222222222");

                // Act
                return service.GetQuarterlyBoilerSales(organisationId, schemeYearId);

                // Assert
            });
        }

        #endregion GetQuarterlyBoilerSales
    }
}
