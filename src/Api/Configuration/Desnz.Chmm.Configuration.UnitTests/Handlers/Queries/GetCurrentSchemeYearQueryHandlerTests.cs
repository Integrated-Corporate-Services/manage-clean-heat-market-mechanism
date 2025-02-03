using AutoMapper;
using AutoMapper.Internal;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Providers;
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
    public class GetCurrentSchemeYearQueryHandlerTests : HandlerTestsMockerBase
    {
        private readonly IMapper _mapper;
        private readonly Mock<ISchemeYearRepository> _schemeYearRepository;
        private readonly DateTimeOverrideProvider dateTimeOverrideProvider;
        private GetCurrentSchemeYearQueryHandler _handler;

        public GetCurrentSchemeYearQueryHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.Internal().MethodMappingEnabled = false;
                cfg.AddProfile<ConfigurationAutoMapperProfile>();
            }).CreateMapper();

            _schemeYearRepository = new Mock<ISchemeYearRepository>(MockBehavior.Strict);

            dateTimeOverrideProvider = new DateTimeOverrideProvider();

            _handler = new GetCurrentSchemeYearQueryHandler(
                new Mock<ILogger<BaseRequestHandler<GetCurrentSchemeYearQuery, ActionResult<SchemeYearDto>>>>().Object,
                _schemeYearRepository.Object, dateTimeOverrideProvider, _mapper);
        }

        [Fact]
        public async void GetCurrentSchemeYearQuery_Returns_Data()
        {
            // Arrange
            dateTimeOverrideProvider.OverrideDate(SchemeYearConstants.StartDate);
            _schemeYearRepository
                .Setup(x => x.GetSchemeYear(It.IsAny<Expression<Func<SchemeYear, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(GenerateSchemeYear());

            // Act
            var data = await _handler.Handle(new GetCurrentSchemeYearQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(data.Value);
        }
    }
}

