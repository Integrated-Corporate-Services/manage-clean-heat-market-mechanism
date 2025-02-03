using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Obligation.Api.Constants;
using Desnz.Chmm.Obligation.Api.Entities;
using Desnz.Chmm.Obligation.Api.Handlers.Queries;
using Desnz.Chmm.Obligation.Api.Infrastructure.Repositories;
using Desnz.Chmm.Obligation.Api.Services;
using Desnz.Chmm.Obligation.Common.Dtos;
using Desnz.Chmm.Obligation.Common.Queries;
using Desnz.Chmm.Testing.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Net;
using Xunit;

namespace Desnz.Chmm.Obligation.UnitTests.Handlers.Queries
{
    public class GetSchemeYearObligationTotalsQueryHandlerTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IOrganisationService> _mockOrganisationService;
        private readonly Mock<ILogger<BaseRequestHandler<GetSchemeYearObligationTotalsQuery, ActionResult<List<ObligationTotalDto>>>>> _mockLogger;
        private readonly Mock<ISchemeYearService> _mockSchemeYearService;
        private readonly ObligationCalculator _obligationCalculator;

        private readonly GetSchemeYearObligationTotalsQueryHandler _handler;

        public GetSchemeYearObligationTotalsQueryHandlerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>(MockBehavior.Strict);
            _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
            _mockLogger = new Mock<ILogger<BaseRequestHandler<GetSchemeYearObligationTotalsQuery, ActionResult<List<ObligationTotalDto>>>>>();
            _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);
            _obligationCalculator = new ObligationCalculator(new Mock<ILogger<ObligationCalculator>>().Object);

            var validator = new RequestValidator(
                new Mock<ICurrentUserService>(MockBehavior.Strict).Object,
                _mockOrganisationService.Object,
                _mockSchemeYearService.Object,
                new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

            _handler = new GetSchemeYearObligationTotalsQueryHandler(
                _mockLogger.Object,
                _mockTransactionRepository.Object,
                _mockOrganisationService.Object,
                _obligationCalculator,
                validator
            );
        }

        [Fact]
        public async void WhenNoSchemeYear_Returns_BadRequest()
        {
            var blankSchemeYearResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _mockSchemeYearService.Setup(i => i.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(blankSchemeYearResponse);

            var result = await _handler.Handle(new GetSchemeYearObligationTotalsQuery(SchemeYearConstants.Id), CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void WhenNoOrganisations_Returns_BadRequest()
        {
            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                Year = SchemeYearConstants.Year
            }, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var blankOrganisationResponse = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _mockOrganisationService.Setup(i => i.GetManufacturers(It.IsAny<string>())).ReturnsAsync(blankOrganisationResponse);

            var result = await _handler.Handle(new GetSchemeYearObligationTotalsQuery(SchemeYearConstants.Id), CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async void WhenDataAvailable_Returns_ObligationTotals()
        {
            var _organisationId = Guid.NewGuid();

            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                Year = SchemeYearConstants.Year
            }, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var organisationResponse = new HttpObjectResponse<List<ViewOrganisationDto>>(new HttpResponseMessage(HttpStatusCode.OK), new List<ViewOrganisationDto>
            {
                new ViewOrganisationDto { Id = _organisationId, Status = OrganisationConstants.Status.Active }
            }, null);
            _mockOrganisationService.Setup(i => i.GetManufacturers(It.IsAny<string>())).ReturnsAsync(organisationResponse);

            var transactions = new List<Transaction>
            {
                new Transaction(Guid.NewGuid(), TransactionConstants.TransactionType.AdminAdjustment, _organisationId, SchemeYearConstants.Id, 100, DateTime.Now)
            };
            _mockTransactionRepository.Setup(i => i.GetAll(It.IsAny<Expression<Func<Transaction, bool>>>(), It.IsAny<RepositoryConstants.SortOrder>(), It.IsAny<bool>())).ReturnsAsync(transactions);

            var result = await _handler.Handle(new GetSchemeYearObligationTotalsQuery(SchemeYearConstants.Id), CancellationToken.None);

            Assert.Equal(1, result.Value.Count());
            Assert.Equal(100, result.Value.First().Value);
        }
    }
}
