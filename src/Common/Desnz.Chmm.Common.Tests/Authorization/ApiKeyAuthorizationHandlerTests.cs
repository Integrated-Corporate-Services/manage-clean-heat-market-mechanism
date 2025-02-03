using Desnz.Chmm.Common.Authorization.Handlers;
using Desnz.Chmm.Common.Authorization.Requirements;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Desnz.Chmm.Common.Tests.Authorization
{
    public class ApiKeyAuthorizationHandlerTests
    {
        private Mock<IHttpContextAccessor> _mockContextAccessor;
        private Mock<ILogger<ApiKeyAuthorizationHandler>> _mockLogger;
        private ApiKeyAuthorizationHandler _handler;
        private AuthorizationHandlerContext _authorizationContext;

        public ApiKeyAuthorizationHandlerTests()
        {
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLogger = new Mock<ILogger<ApiKeyAuthorizationHandler>>();

            _handler = new ApiKeyAuthorizationHandler(_mockLogger.Object, _mockContextAccessor.Object);

            var requirements = new ApiKeyRequirement("api-key", "123");
            _authorizationContext = new AuthorizationHandlerContext(new[] { requirements }, new ClaimsPrincipal(), null);
        }

        [Fact]
        internal async Task ShouldFail_When_ApiKeyIsMissingFromHeader()
        {
            //Arrange
            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(new DefaultHttpContext());

            // Act
            await _handler.HandleAsync(_authorizationContext);

            // Assert
            _authorizationContext.HasSucceeded.Should().BeFalse();
        }

        [Fact]
        internal async Task ShouldFail_When_ApiKeyDoesNotMatch()
        {
            //Arrange
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers["api-key"] = "1234";

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext);

            // Act
            await _handler.HandleAsync(_authorizationContext);

            // Assert
            _authorizationContext.HasSucceeded.Should().BeFalse();
        }

        [Fact]
        internal async Task ShouldSucceed_When_ApiKeyMatches()
        {
            //Arrange
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers["api-key"] = "123";

            _mockContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext);

            // Act
            await _handler.HandleAsync(_authorizationContext);

            // Assert
            _authorizationContext.HasSucceeded.Should().BeTrue();
        }
    }
}
