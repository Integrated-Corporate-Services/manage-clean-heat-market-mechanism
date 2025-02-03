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
    public class GetCurrentSchemeYearQuarterQueryHandlerTests : HandlerTestsMockerBase
    {
        private readonly IMapper _mapper;
        private readonly Mock<ISchemeYearRepository> _schemeYearRepository;
        private readonly DateTimeOverrideProvider dateTimeOverrideProvider;
        private GetCurrentSchemeYearQuarterQueryHandler _handler;

        public GetCurrentSchemeYearQuarterQueryHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.Internal().MethodMappingEnabled = false;
                cfg.AddProfile<ConfigurationAutoMapperProfile>();
            }).CreateMapper();

            _schemeYearRepository = new Mock<ISchemeYearRepository>(MockBehavior.Strict);
            
            dateTimeOverrideProvider = new DateTimeOverrideProvider();

            _handler = new GetCurrentSchemeYearQuarterQueryHandler(
                new Mock<ILogger<BaseRequestHandler<GetCurrentSchemeYearQuarterQuery, ActionResult<SchemeYearQuarterDto>>>>().Object,
                _schemeYearRepository.Object, dateTimeOverrideProvider, _mapper);
        }

        [Fact]
        public async void GetCurrentSchemeYearQuarterQuery_Returns_Data()
        {
            // Arrange
            dateTimeOverrideProvider.OverrideDate(SchemeYearConstants.QuarterOneStartDate);
            _schemeYearRepository
                .Setup(x => x.GetSchemeYearQuarter(It.IsAny<Expression<Func<SchemeYearQuarter, bool>>>(), It.IsAny<bool>()))
                .Returns(GenerateSchemeYearQuarter());

            // Act
            var data = await _handler.Handle(new GetCurrentSchemeYearQuarterQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(data.Value);
        }
    }
}

