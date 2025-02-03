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
    public class GetCreditWeightingsQueryHandlerTests : HandlerTestsMockerBase
    {
        private readonly IMapper _mapper;
        private readonly Mock<ISchemeYearRepository> _schemeYearRepository;
        private GetCreditWeightingsQueryHandler _handler;

        public GetCreditWeightingsQueryHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.Internal().MethodMappingEnabled = false;
                cfg.AddProfile<ConfigurationAutoMapperProfile>();
            }).CreateMapper();

            _schemeYearRepository = new Mock<ISchemeYearRepository>(MockBehavior.Strict);

            _handler = new GetCreditWeightingsQueryHandler(
                new Mock<ILogger<BaseRequestHandler<GetCreditWeightingsQuery, ActionResult<CreditWeightingsDto>>>>().Object,
                _schemeYearRepository.Object, _mapper);
        }

        [Fact]
        public async void GetCreditWeightingsQuery_Returns_Data()
        {
            // Arrange
            _schemeYearRepository
                .Setup(x => x.GetCreditWeighting(It.IsAny<Expression<Func<CreditWeighting, bool>>>(), It.IsAny<bool>()))
                .Returns(GenerateCreditWeightings(SchemeYearConstants.Id));

            // Act
            var data = await _handler.Handle(new GetCreditWeightingsQuery(SchemeYearConstants.Id), CancellationToken.None);

            // Assert
            Assert.NotNull(data.Value);
        }
    }
}

