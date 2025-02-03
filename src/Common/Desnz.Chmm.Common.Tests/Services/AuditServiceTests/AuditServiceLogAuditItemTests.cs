using Desnz.Chmm.Common.Infrastructure.Repositories;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.Common.ValueObjects;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Desnz.Chmm.Common.Tests.Services
{
    public class AuditServiceLogAuditItemTests
    {
        private readonly AuditService _auditService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IAuditItemRepository> _mockAuditItemRepository;
        public AuditServiceLogAuditItemTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockAuditItemRepository = new Mock<IAuditItemRepository>();
            _auditService = new AuditService(_mockHttpContextAccessor.Object, _mockAuditItemRepository.Object);
        }

        [Fact]
        public async Task LogAuditItem_WhenDetailsCanAudit_CreatesAuditItem()
        {
            // Arrange
            var details = new AuditDetails(new object(), "Friendly Name", "Full Name", "Trace Id", Guid.NewGuid(), Guid.NewGuid(), new ResponseDetails(true, "Success"));;

            var duration = 100L;

            // Act
            await _auditService.LogAuditItem(details, duration);

            // Assert
            _mockAuditItemRepository.Verify(repo =>
                repo.Create(It.IsAny<AuditItem>()),
                Times.Once);
        }

        [Fact]
        public async Task LogAuditItem_WhenDetailsCannotAudit_DoesNotCreateAuditItem()
        {
            // Arrange
            var details = new AuditDetails();

            var duration = 100L;

            // Act
            await _auditService.LogAuditItem(details, duration);

            // Assert
            _mockAuditItemRepository.Verify(repo =>
                repo.Create(It.IsAny<AuditItem>()),
                Times.Never);
        }
    }
}
