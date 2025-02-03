using AutoMapper;
using AutoMapper.Internal;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Configuration.Api.AutoMapper;
using Desnz.Chmm.Configuration.Api.Entities;
using Desnz.Chmm.Configuration.Api.Handlers.Queries;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Configuration.Common.Queries;
using Desnz.Chmm.Testing.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Desnz.Chmm.Configuration.UnitTests.Handlers.Queries
{
    public class GetSchemeYearFirstQueryHandlerTests : HandlerTestsMockerBase
    {
        private readonly Mock<ISchemeYearRepository> _schemeYearRepository;
        private readonly Mock<IMapper> _mockMapper;

        private GetFirstSchemeYearQueryHandler _handler;

        public GetSchemeYearFirstQueryHandlerTests()
        {
            _schemeYearRepository = new Mock<ISchemeYearRepository>(MockBehavior.Strict);
            _mockMapper = new Mock<IMapper>(MockBehavior.Strict);

            _handler = new GetFirstSchemeYearQueryHandler(
                new Mock<ILogger<BaseRequestHandler<GetFirstSchemeYearQuery, ActionResult<SchemeYearDto>>>>().Object,
                _schemeYearRepository.Object, 
                _mockMapper.Object);
        }

        [Fact]
        public async void GetSchemeYearQuery_Returns_Data()
        {
            // Arrange
            _schemeYearRepository.Setup(x => x.GetFirstSchemeYearAsync()).ReturnsAsync(GenerateSchemeYear());
            _mockMapper.Setup(x => x.Map<SchemeYearDto>(It.IsAny<SchemeYear>())).Returns(new SchemeYearDto());

            // Act
            var result = await _handler.Handle(new GetFirstSchemeYearQuery(), CancellationToken.None);

            // Assert
            result.Result.Should().BeNull();
            result.Value.Should().NotBeNull();
        }
    }
}
