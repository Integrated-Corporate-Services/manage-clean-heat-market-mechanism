using Amazon.Runtime.Internal.Util;
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
using Xunit;

namespace Desnz.Chmm.Configuration.Common.Queries
{
    public class GetAllCreditWeightingsQueryHandlerTests : HandlerTestsMockerBase
    {
        private readonly IMapper _mapper;
        private readonly Mock<ISchemeYearRepository> _schemeYearRepository;
        private GetAllCreditWeightingsQueryHandler _handler;

        public GetAllCreditWeightingsQueryHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.Internal().MethodMappingEnabled = false;
                cfg.AddProfile<ConfigurationAutoMapperProfile>();
            }).CreateMapper();

            _schemeYearRepository = new Mock<ISchemeYearRepository>(MockBehavior.Strict);

            _handler = new GetAllCreditWeightingsQueryHandler(
                new Mock<ILogger<BaseRequestHandler<GetAllCreditWeightingsQuery, ActionResult<List<CreditWeightingsDto>>>>>().Object,
                _schemeYearRepository.Object, _mapper);
        }

        [Fact]
        public async void GetAllCreditWeightingsQuery_Returns_Data()
        {
            // Arrange
            var mockData = new List<CreditWeighting> { GenerateCreditWeightings(SchemeYearConstants.Id) };
            _schemeYearRepository.Setup(x => x.GetAllCreditWeightings()).ReturnsAsync(mockData);

            // Act
            var data = await _handler.Handle(new GetAllCreditWeightingsQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(data.Value);
        }
    }
}
