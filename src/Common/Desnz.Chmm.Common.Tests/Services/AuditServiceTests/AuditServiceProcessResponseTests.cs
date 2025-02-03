using Desnz.Chmm.Common.Infrastructure.Repositories;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Desnz.Chmm.Common.Tests.Services
{
    public class AuditServiceProcessResponseTests
    {
        private readonly AuditService _auditService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IAuditItemRepository> _mockAuditItemRepository;
        public AuditServiceProcessResponseTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockAuditItemRepository = new Mock<IAuditItemRepository>();
            _auditService = new AuditService(_mockHttpContextAccessor.Object, _mockAuditItemRepository.Object);
        }
        private ActionResult<string> GetOk()
        {
            return Responses.Ok();
        }
        private ActionResult<string> GetBadRequest(string error)
        {
            return Responses.BadRequest(error);
        }

        [Fact]
        public void ProcessResponse_SuccessfulActionResult_ReturnsSuccessResponse()
        {
            // Arrange
            var response = GetOk();

            // Act
            var result = _auditService.ProcessResponse(response);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("(200 OK) - Success", result.Message);
        }

        [Fact]
        public void ProcessResponse_UnsuccessfulActionResult_ReturnsErrorResponse()
        {
            // Arrange
            var response = GetBadRequest("Error");

            // Act
            var result = _auditService.ProcessResponse(response);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("(400 BadRequest) - \"Error\"", result.Message);
        }

        [Fact]
        public void ProcessResponse_NullResponse_ReturnsErrorResponse()
        {
            // Arrange
            IActionResult response = null;

            // Act
            var result = _auditService.ProcessResponse(response);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Success - Null Response", result.Message); // Since the response is null, the default success message should be used
        }
    }
}
