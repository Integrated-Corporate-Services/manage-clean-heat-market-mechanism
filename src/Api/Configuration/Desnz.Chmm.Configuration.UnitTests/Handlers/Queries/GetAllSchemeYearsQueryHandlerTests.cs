using AutoMapper;
using AutoMapper.Internal;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Configuration.Api.AutoMapper;
using Desnz.Chmm.Configuration.Api.Infrastructure.Repositories;
using Desnz.Chmm.Configuration.Common.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetAllSchemeYearsQueryHandlerTests : HandlerTestsMockerBase
    {
        private readonly IMapper _mapper;
        private readonly Mock<ISchemeYearRepository> _schemeYearRepository;
        private GetAllSchemeYearsQueryHandler _handler;

        public GetAllSchemeYearsQueryHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.Internal().MethodMappingEnabled = false;
                cfg.AddProfile<ConfigurationAutoMapperProfile>();
            }).CreateMapper();

            _schemeYearRepository = new Mock<ISchemeYearRepository>(MockBehavior.Strict);

            _handler = new GetAllSchemeYearsQueryHandler(
                new Mock<ILogger<BaseRequestHandler<GetAllSchemeYearsQuery, ActionResult<List<SchemeYearDto>>>>>().Object,
                _schemeYearRepository.Object, _mapper);
        }

        [Fact]
        public async void GetAllSchemeYearsQuery_Returns_Data()
        {
            // Arrange
            _schemeYearRepository.Setup(x => x.GetAllSchemeYears()).ReturnsAsync(GenerateSchemeYears());

            // Act
            var data = await _handler.Handle(new GetAllSchemeYearsQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(data.Value);
        }
    }
}

