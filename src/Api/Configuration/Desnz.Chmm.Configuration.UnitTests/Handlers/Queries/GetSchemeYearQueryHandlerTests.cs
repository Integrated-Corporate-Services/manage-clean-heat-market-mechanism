using AutoMapper;
using AutoMapper.Internal;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Configuration.Api.AutoMapper;
using Desnz.Chmm.Configuration.Api.Entities;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Testing.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetSchemeYearQueryHandlerTests : HandlerTestsMockerBase
    {
        private readonly IMapper _mapper;
        private readonly Mock<ISchemeYearRepository> _schemeYearRepository;
        private GetSchemeYearQueryHandler _handler;

        public GetSchemeYearQueryHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.Internal().MethodMappingEnabled = false;
                cfg.AddProfile<ConfigurationAutoMapperProfile>();
            }).CreateMapper();

            _schemeYearRepository = new Mock<ISchemeYearRepository>(MockBehavior.Strict);

            _handler = new GetSchemeYearQueryHandler(
                new Mock<ILogger<BaseRequestHandler<GetSchemeYearQuery, ActionResult<SchemeYearDto>>>>().Object,
                _schemeYearRepository.Object, _mapper);
        }

        [Fact]
        public async void GetSchemeYearQuery_Returns_Data()
        {
            // Arrange
            _schemeYearRepository
                .Setup(x => x.GetSchemeYear(It.IsAny<Expression<Func<SchemeYear, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(GenerateSchemeYear());

            // Act
            var data = await _handler.Handle(new GetSchemeYearQuery(SchemeYearConstants.Id), CancellationToken.None);

            // Assert
            Assert.NotNull(data.Value);
        }
    }
}