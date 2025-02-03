using AutoMapper;
using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.BoilerSales.Api.Entities;
using Desnz.Chmm.BoilerSales.Api.Handlers.Queries;
using Desnz.Chmm.BoilerSales.Api.Infrastructure.Repositories;
using Desnz.Chmm.BoilerSales.Api.Services;
using Desnz.Chmm.BoilerSales.Common.Dtos;
using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.Testing.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using Xunit;
using static Desnz.Chmm.Common.Constants.BoilerSalesStatusConstants;

namespace Desnz.Chmm.BoilerSales.UnitTests.Handlers.Queries
{
    public class BoilerSalesSummaryAllQueryHandlerTests : TestClaimsBase
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IAnnualBoilerSalesRepository> _mockAnnualBoilerSalesRepository;
        private readonly Mock<IQuarterlyBoilerSalesRepository> _mockQuarterlyBoilerSalesRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<ISchemeYearService> _mockSchemeYearService;

        private readonly BoilerSalesSummaryAllQueryHandler _handler;

        private readonly Guid _organisationId = Guid.NewGuid();
        private readonly Guid _schemeYearId = SchemeYearConstants.Id;
        private readonly Guid _schemeYearQuarterOneId = SchemeYearConstants.QuarterOneId;
        private readonly Guid _schemeYearQuarterTwoId = SchemeYearConstants.QuarterTwoId;
        private readonly Guid _schemeYearQuarterThreeId = SchemeYearConstants.QuarterThreeId;
        private readonly Guid _schemeYearQuarterFourId = SchemeYearConstants.QuarterFourId;

        public BoilerSalesSummaryAllQueryHandlerTests()
        {
            _mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            _mockAnnualBoilerSalesRepository = new Mock<IAnnualBoilerSalesRepository>(MockBehavior.Strict);
            _mockQuarterlyBoilerSalesRepository = new Mock<IQuarterlyBoilerSalesRepository>(MockBehavior.Strict);
            _mockCurrentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
            _mockSchemeYearService = new Mock<ISchemeYearService>();
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
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var obligationCalculationsData = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.OK), new ObligationCalculationsDto()
            {
                GasBoilerSalesThreshold = SchemeYearConstants.GasBoilerSalesThreshold,
                OilBoilerSalesThreshold = SchemeYearConstants.OilBoilerSalesthreshold
            }, null);
            _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(obligationCalculationsData);

            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var validator = new RequestValidator(
                _mockCurrentUserService.Object,
                new Mock<IOrganisationService>(MockBehavior.Strict).Object,
                _mockSchemeYearService.Object,
                new ValidationMessenger(new Mock<ILogger<ValidationMessenger>>().Object));

            _handler = new BoilerSalesSummaryAllQueryHandler(
                new Mock<ILogger<BaseRequestHandler<BoilerSalesSummaryAllQuery, ActionResult<List<BoilerSalesSummaryDto>>>>>().Object,
                _mockSchemeYearService.Object,
                _mockAnnualBoilerSalesRepository.Object,
                _mockQuarterlyBoilerSalesRepository.Object, 
                new BoilerSalesCalculator(),
                validator);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_Invalid_SchemeYearId()
        {
            //Arrange
            var query = new BoilerSalesSummaryAllQuery(_schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var expectedErrorMsg = $"Failed to get Scheme Year with Id: {_schemeYearId}, problem: null";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeNull();
            result.Result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnBadRequest_When_ObligationCalculations_IsNotFound()
        {
            //Arrange
            var query = new BoilerSalesSummaryAllQuery(_schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                Year = SchemeYearConstants.Year,
                Quarters = new List<SchemeYearQuarterDto> { new SchemeYearQuarterDto { Id = SchemeYearConstants.QuarterOneId } }
            }, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var mockGetObligationCalculationsResponse = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockGetObligationCalculationsResponse);

            var expectedErrorMsg = $"Failed to get Obligation Calculations for Scheme Year with Id: {_schemeYearId}, problem: null";
            var expectedResult = Responses.BadRequest(expectedErrorMsg);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeNull();
            result.Result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnOk_When_No_Annual_And_Quarterly_Sales_Exist()
        {
            //Arrange
            var query = new BoilerSalesSummaryAllQuery(_schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                Year = SchemeYearConstants.Year,
                Quarters = new List<SchemeYearQuarterDto> { new SchemeYearQuarterDto { Id = SchemeYearConstants.QuarterOneId } }
            }, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var obligationCalculations = new ObligationCalculationsDto();
            var mockGetObligationCalculationsResponse = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.OK), obligationCalculations, null);
            _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockGetObligationCalculationsResponse);

            _mockAnnualBoilerSalesRepository.Setup(i => i.GetAll(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(new List<AnnualBoilerSales>());
            _mockQuarterlyBoilerSalesRepository.Setup(i => i.GetAllNonAnnual(It.IsAny<Guid>())).ReturnsAsync(new List<QuarterlyBoilerSales>());

            var expectedResult = new List<BoilerSalesSummaryDto>();

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnOk_When_Annual_Exists_And_No_Quarterly_Sales_Exists()
        {
            //Arrange
            var query = new BoilerSalesSummaryAllQuery(_schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                Year = SchemeYearConstants.Year,
                Quarters = new List<SchemeYearQuarterDto> { new SchemeYearQuarterDto { Id = SchemeYearConstants.QuarterOneId } }
            }, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var obligationCalculations = new ObligationCalculationsDto();
            var mockGetObligationCalculationsResponse = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.OK), obligationCalculations, null);
            _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockGetObligationCalculationsResponse);
            var annualSales = new List<AnnualBoilerSales>() 
            { 
                new AnnualBoilerSales(_schemeYearId, _organisationId, 0, 0, new List<AnnualBoilerSalesFile>())
            };
            _mockAnnualBoilerSalesRepository.Setup(i => i.GetAll(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(annualSales);
            _mockQuarterlyBoilerSalesRepository.Setup(i => i.GetAllNonAnnual(It.IsAny<Guid>())).ReturnsAsync(new List<QuarterlyBoilerSales>());

            var dtos = new List<BoilerSalesSummaryDto> { new BoilerSalesSummaryDto(_organisationId, 0, 0, 0, 0, BoilerSalesSummarySubmissionStatus.AnnualSubmitted) };

            _mockMapper.Setup(x => x.Map<List<BoilerSalesSummaryDto>>(It.IsAny<List<AnnualBoilerSales>>())).Returns(dtos);

            var expectedResult = dtos;

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnOk_When_Annual_DoesNotExist_And_Quarterly_Sales_Exist()
        {
            //Arrange
            var query = new BoilerSalesSummaryAllQuery(_schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                Year = SchemeYearConstants.Year,
                Quarters = new List<SchemeYearQuarterDto> { new SchemeYearQuarterDto { Id = SchemeYearConstants.QuarterOneId } }
            }, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var obligationCalculations = new ObligationCalculationsDto();
            var mockGetObligationCalculationsResponse = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.OK), obligationCalculations, null);
            _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockGetObligationCalculationsResponse);
            var annualSales = new List<AnnualBoilerSales>(){};
            var quarterlySales = new List<QuarterlyBoilerSales>()
            {
                new QuarterlyBoilerSales(_organisationId, _schemeYearId, _schemeYearQuarterOneId, 0, 0, new List<QuarterlyBoilerSalesFile>())
            };
            _mockAnnualBoilerSalesRepository.Setup(i => i.GetAll(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(annualSales);
            _mockQuarterlyBoilerSalesRepository.Setup(i => i.GetAllNonAnnual(It.IsAny<Guid>())).ReturnsAsync(quarterlySales);

            var dtos = new List<BoilerSalesSummaryDto> { new BoilerSalesSummaryDto(_organisationId, 0, 0, 0, 0, BoilerSalesSummarySubmissionStatus.QuarterSubmitted) };

            _mockMapper.Setup(x => x.Map<List<BoilerSalesSummaryDto>>(It.IsAny<List<QuarterlyBoilerSales>>())).Returns(dtos);

            var expectedResult = dtos;

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task ShouldReturnOk_When_Both_Annual_And_Quarterly_Sales_Exist()
        {
            //Arrange
            var query = new BoilerSalesSummaryAllQuery(_schemeYearId);

            var mockUser = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Principal Technical Officer")
            }));
            _mockCurrentUserService.SetupGet(x => x.CurrentUser).Returns(mockUser);

            var schemYearData = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                Year = SchemeYearConstants.Year,
                Quarters = new List<SchemeYearQuarterDto> { new SchemeYearQuarterDto { Id = SchemeYearConstants.QuarterOneId } }
            }, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(schemYearData);

            var obligationCalculations = new ObligationCalculationsDto();
            var mockGetObligationCalculationsResponse = new HttpObjectResponse<ObligationCalculationsDto>(new HttpResponseMessage(HttpStatusCode.OK), obligationCalculations, null);
            _mockSchemeYearService.Setup(x => x.GetObligationCalculations(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockGetObligationCalculationsResponse);
            var annualSales = new List<AnnualBoilerSales>()
            {
                new AnnualBoilerSales(_schemeYearId, _organisationId, 0, 0, new List<AnnualBoilerSalesFile>())
            };

            var quarterlySales = new List<QuarterlyBoilerSales>()
            {
                new QuarterlyBoilerSales(_organisationId, _schemeYearId, _schemeYearQuarterOneId, 0, 0, new List<QuarterlyBoilerSalesFile>())
            };
            _mockAnnualBoilerSalesRepository.Setup(i => i.GetAll(It.IsAny<Expression<Func<AnnualBoilerSales, bool>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(annualSales);
            _mockQuarterlyBoilerSalesRepository.Setup(i => i.GetAllNonAnnual(It.IsAny<Guid>())).ReturnsAsync(quarterlySales);

            var dtos = new List<BoilerSalesSummaryDto> 
            { 
                new BoilerSalesSummaryDto(_organisationId, 0, 0, 0, 0, BoilerSalesSummarySubmissionStatus.AnnualSubmitted)
            };

            _mockMapper.Setup(x => x.Map<List<BoilerSalesSummaryDto>>(It.IsAny<List<QuarterlyBoilerSales>>())).Returns(dtos);

            var expectedResult = dtos;

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.Value.Should().BeEquivalentTo(expectedResult);
        }
    }
}
