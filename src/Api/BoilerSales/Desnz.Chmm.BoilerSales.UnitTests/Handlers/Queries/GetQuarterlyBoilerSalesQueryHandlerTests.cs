using AutoMapper;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Api.Handlers.Queries;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Api.Services;
using Desnz.Chmm.BoilerSales.Common.Dtos;
using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Identity.Common.Constants;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using Desnz.Chmm.Testing.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using Xunit;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.UnitTests.Handlers.Queries
{
    public class BoilerSalesSummaryQueryHandlerTests : TestClaimsBase
    {
        private readonly Mock<IAnnualBoilerSalesRepository> _mockAnnualBoilerSalesRepository;
        private readonly Mock<IQuarterlyBoilerSalesRepository> _mockQuarterlyBoilerSalesRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<IOrganisationService> _mockOrganisationService;
        private readonly Mock<ISchemeYearService> _schemeYearService;
        private readonly Mock<ILogger<BoilerSalesSummaryQueryHandler>> _mockLogger;

        private readonly BoilerSalesSummaryQueryHandler _handler;

        private readonly Guid _organisationId = Guid.NewGuid();
        private readonly Guid _schemeYearId = SchemeYearConstants.Id;
        private readonly Guid _schemeYearQuarterOneId = SchemeYearConstants.QuarterOneId;
        private readonly Guid _schemeYearQuarterTwoId = SchemeYearConstants.QuarterTwoId;
        private readonly Guid _schemeYearQuarterThreeId = SchemeYearConstants.QuarterThreeId;
        private readonly Guid _schemeYearQuarterFourId = SchemeYearConstants.QuarterFourId;

        public BoilerSalesSummaryQueryHandlerTests()
        {
            _mockAnnualBoilerSalesRepository = new Mock<IAnnualBoilerSalesRepository>(MockBehavior.Strict);
            _mockQuarterlyBoilerSalesRepository = new Mock<IQuarterlyBoilerSalesRepository>(MockBehavior.Strict);
            _mockCurrentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
            _mockOrganisationService = new Mock<IOrganisationService>(MockBehavior.Strict);
            _schemeYearService = new Mock<ISchemeYearService>();
            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                Quarters = new List<SchemeYearQuarterDto>
                {
                    new SchemeYearQuarterDto {Id = SchemeYearConstants.QuarterOneId, StartDate = SchemeYearConstants.QuarterOneStartDate},
                    new SchemeYearQuarterDto {Id = SchemeYearConstants.QuarterTwoId, StartDate = SchemeYearConstants.QuarterTwoStartDate},
                    new SchemeYearQuarterDto {Id = SchemeYearConstants.QuarterThreeId, StartDate = SchemeYearConstants.QuarterThreeStartDate},
                    new SchemeYearQuarterDto {Id = SchemeYearConstants.QuarterFourId, StartDate = SchemeYearConstants.QuarterFourStartDate}
                }
            }, null);
            _schemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var obligationCalculationsData = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.OK), new ObligationCalculationsDto()
            {
                GasBoilerSalesThreshold = SchemeYearConstants.GasBoilerSalesThreshold,
                OilBoilerSalesThreshold = SchemeYearConstants.OilBoilerSalesthreshold
            }, null);
            _schemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(obligationCalculationsData);

            _schemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);
            _mockLogger = new Mock<ILogger<BoilerSalesSummaryQueryHandler>>();

            var validator = new RequestValidator(
                _mockCurrentUserService.Object,
                _mockOrganisationService.Object,
                _schemeYearService.Object,
                new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

            _handler = new BoilerSalesSummaryQueryHandler(
                _mockLogger.Object,
                _schemeYearService.Object, 
                _mockAnnualBoilerSalesRepository.Object,
                _mockQuarterlyBoilerSalesRepository.Object,
                new BoilerSalesCalculator(),
                validator);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_OrganisationIsNotFound()
        {
            //Arrange
            var query = new BoilerSalesSummaryQuery(_organisationId, _schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var expectedErrorMsg = $"Failed to get Organisation with Id: {_organisationId}, problem: {JsonConvert.SerializeObject(mockGetOrganisationResponse.Problem)}";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeNull();
            result.Result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_OrganisationIsNotActive()
        {
            //Arrange
            var query = new BoilerSalesSummaryQuery(_organisationId, _schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto
            {
                Status = OrganisationConstants.Status.Pending
            }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var expectedErrorMsg = $"Organisation with Id: {query.OrganisationId} has an invalid status: Pending";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeNull();
            result.Result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnSumOfQuarter_When_QuarterlySalesAreSupplied()
        {
            //Arrange
            var query = new BoilerSalesSummaryQuery(_organisationId, _schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var quarterData = new List<QuarterlyBoilerSales>()
            {
                new QuarterlyBoilerSales(_organisationId, _schemeYearId, _schemeYearQuarterOneId, 100000, 200000, null),
                new QuarterlyBoilerSales(_organisationId, _schemeYearId, _schemeYearQuarterTwoId, 100000, 200000, null),
                new QuarterlyBoilerSales(_organisationId, _schemeYearId, _schemeYearQuarterThreeId, 100000, 200000, null),
            };
            _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(quarterData);

            AnnualBoilerSales? annualData = null;

            _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(annualData);

            var expectedErrorMsg = $"Unable to get obligation amendments for organisation with Id: {_organisationId}. Organisation is not active.";
            var expectedResult = new BoilerSalesSummaryDto
                (_organisationId,
                300000,
                600000,
                300000 - SchemeYearConstants.GasBoilerSalesThreshold,
                600000 - SchemeYearConstants.OilBoilerSalesthreshold,
                BoilerSalesSummarySubmissionStatus.QuarterSubmitted
            );

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnZeroIfLessThanThresholdAnnualSum_When_AnnualSalesAreSupplied()
        {
            //Arrange
            var query = new BoilerSalesSummaryQuery(_organisationId, _schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var quarterData = new List<QuarterlyBoilerSales>()
            {
                new QuarterlyBoilerSales(_organisationId, _schemeYearId, _schemeYearQuarterOneId, 100000, 200000, null),
                new QuarterlyBoilerSales(_organisationId, _schemeYearId, _schemeYearQuarterTwoId, 100000, 200000, null),
                new QuarterlyBoilerSales(_organisationId, _schemeYearId, _schemeYearQuarterThreeId, 100000, 200000, null),
            };
            _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(quarterData);

            var gas = SchemeYearConstants.GasBoilerSalesThreshold - 100;
            var oil = SchemeYearConstants.OilBoilerSalesthreshold - 100;
            var annualData = new AnnualBoilerSales(_schemeYearId, _organisationId, gas, oil, null);

            _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(annualData);

            var expectedErrorMsg = $"Unable to get obligation amendments for organisation with Id: {_organisationId}. Organisation is not active.";
            var expectedResult = new BoilerSalesSummaryDto(_organisationId,
                gas,
                oil,
                0,
                0,
                BoilerSalesSummarySubmissionStatus.AnnualSubmitted
            );

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnZeroIfLessThanThresholdAnnualSum_When_QuarterSalesAreSupplied()
        {
            //Arrange
            var query = new BoilerSalesSummaryQuery(_organisationId, _schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var mockGetOrganisationResponse = new HttpObjectResponse<OrganisationStatusDto>(new HttpResponseMessage(HttpStatusCode.OK), new OrganisationStatusDto() { Status = OrganisationConstants.Status.Active }, null);
            _mockOrganisationService.Setup(x => x.GetStatus(_organisationId)).ReturnsAsync(mockGetOrganisationResponse);

            var gas = SchemeYearConstants.GasBoilerSalesThreshold - 100;
            var oil = SchemeYearConstants.OilBoilerSalesthreshold - 100;

            var quarterData = new List<QuarterlyBoilerSales>()
            {
                new QuarterlyBoilerSales(_organisationId, _schemeYearId, _schemeYearQuarterOneId, gas, oil, null),
            };
            _mockQuarterlyBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<QuarterlyBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(quarterData);

            AnnualBoilerSales? annualData = null;

            _mockAnnualBoilerSalesRepository.Setup(x => x.Get(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(annualData);

            var expectedErrorMsg = $"Unable to get obligation amendments for organisation with Id: {_organisationId}. Organisation is not active.";
            var expectedResult = new BoilerSalesSummaryDto(_organisationId,
                gas,
                oil,
                0,
                0,
                BoilerSalesSummarySubmissionStatus.QuarterSubmitted
            );

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeEquivalentTo(expectedResult);
        }
    }
}
