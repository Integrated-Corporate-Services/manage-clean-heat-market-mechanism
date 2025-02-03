using Desnz.Chmm.Common.Infrastructure.Repositories;
using Desnz.Chmm.Common.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Desnz.Chmm.Common.Tests.Services.AuditServiceTests
{
    public class AuditServiceProcessExceptionTests
    {
        private readonly AuditService _auditService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IAuditItemRepository> _mockAuditItemRepository;
        public AuditServiceProcessExceptionTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockAuditItemRepository = new Mock<IAuditItemRepository>();
            _auditService = new AuditService(_mockHttpContextAccessor.Object, _mockAuditItemRepository.Object);
        }

        [Fact]
        internal void When_Processing_Exception_Details_Are_Extracted()
        {
            var exception = new Exception("Message");

            var details = _auditService.ProcessException(exception);

            Assert.False(details.Success);
            Assert.StartsWith("Message", details.Message);
            Assert.Equal("Message" + Environment.NewLine + exception.StackTrace, details.Message);
        }

        [Fact]
        internal void When_Processing_Exception_If_Null_Is_Passed_Warning_Returned()
        {
            var details = _auditService.ProcessException(null);

            Assert.False(details.Success);
            Assert.Equal("Null exception passed", details.Message);
        }
    }
}
