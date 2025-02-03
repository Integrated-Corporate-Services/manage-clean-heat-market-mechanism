using Desnz.Chmm.ApiClients.Http;
using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Configuration.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Desnz.Chmm.McsSynchronisation.Api.Handlers.Queries;
using Desnz.Chmm.McsSynchronisation.Api.Infrastructure.Repositories;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Desnz.Chmm.Testing.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Net;
using FluentAssertions;
using Xunit;

namespace Desnz.Chmm.McsSynchronisation.UnitTests.Handlers.Query
{
    public class GetInstallationRequestsQueryHandlerTests
    {
        private Mock<ILogger<GetInstallationRequestsQueryHandler>> _logger;
        private readonly Mock<ISchemeYearService> _mockSchemeYearService;

        private HttpObjectResponse<SchemeYearDto> _schemeYear;

        public GetInstallationRequestsQueryHandlerTests()
        {
            _logger = new Mock<ILogger<GetInstallationRequestsQueryHandler>>();
            _mockSchemeYearService = new Mock<ISchemeYearService>(MockBehavior.Strict);
        }


        [Fact]
        internal async Task ShouldReturnBadRequest_When_SchemeYearIsNotFound()
        {
            //Assert
            var _schemeYearId = SchemeYearConstants.Id;
            var repository = new Mock<IInstallationRequestRepository>(MockBehavior.Strict);
            var referenceRepository = new Mock<IMcsReferenceDataRepository>();

            var requests = new List<InstallationRequest>() { };

            repository.Setup(i => i.GetAll(It.IsAny<Expression<Func<InstallationRequest, bool>>>(), It.IsAny<bool>())).ReturnsAsync(requests);

            var httpResponse = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.NotFound), null, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(httpResponse);

            var expectedResult = Responses.BadRequest($"Failed to get Scheme Year with Id: {_schemeYearId}, problem: null");

            var handler = new GetInstallationRequestsQueryHandler(_logger.Object, _mockSchemeYearService.Object, repository.Object, referenceRepository.Object);

            // Act
            var actionResult = await handler.Handle(new Common.Queries.GetInstallationRequestsQuery(_schemeYearId), CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        internal async Task Empty_Query_Returns_All_Requests()
        {
            var repository = new Mock<IInstallationRequestRepository>(MockBehavior.Strict);
            var referenceRepository = new Mock<IMcsReferenceDataRepository>();

            var requests = new List<InstallationRequest>() { };

            repository.Setup(i => i.GetAll(It.IsAny<Expression<Func<InstallationRequest, bool>>>(), It.IsAny<bool>())).ReturnsAsync(requests);

            _schemeYear = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                EndDate = new DateOnly(2025, 1, 1)
            }, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(_schemeYear);

            var handler = new GetInstallationRequestsQueryHandler(_logger.Object, _mockSchemeYearService.Object, repository.Object, referenceRepository.Object);

            var actionResult = await handler.Handle(new Common.Queries.GetInstallationRequestsQuery(It.IsAny<Guid>()), CancellationToken.None);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<List<InstallationRequestSummaryDto>>>(actionResult);
        }

        [Fact]
        internal async Task Empty_Query_Returns_All_Requests_Fully_Mapped()
        {
            var installationRepository = new Mock<IInstallationRequestRepository>(MockBehavior.Strict);
            var referenceRepository = new Mock<IMcsReferenceDataRepository>();

            var techTypes = new List<TechnologyType> { new TechnologyType { Id = 1, Description = "Desc 1" }, new TechnologyType { Id = 2, Description = "Desc 2" } };
            referenceRepository.Setup(r => r.GetAllTechnologyTypes(null, false)).ReturnsAsync(techTypes);

            var newBuilds  = new List<NewBuildOption> { new NewBuildOption { Id = 1, Description = "Desc 1" }, new NewBuildOption { Id = 2, Description = "Desc 2" } };
            referenceRepository.Setup(r => r.GetAllNewBuildOptions(null, false)).ReturnsAsync(newBuilds);

            var now = DateTime.Now;
            var requests = new List<InstallationRequest>() { new InstallationRequest() {
                RequestDate = now,
                EndDate = now,
                StartDate = now.AddDays(1),
                IsNewBuildIds = new []{1,2},
                TechnologyTypeIds  = new []{1,2}
            } };
            installationRepository.Setup(i => i.GetAll(It.IsAny<Expression<Func<InstallationRequest, bool>>>(), It.IsAny<bool>())).ReturnsAsync(requests);

            _schemeYear = new HttpObjectResponse<SchemeYearDto>(new HttpResponseMessage(HttpStatusCode.OK), new SchemeYearDto()
            {
                Id = SchemeYearConstants.Id,
                EndDate = new DateOnly(2025, 1, 1)
            }, null);
            _mockSchemeYearService.Setup(x => x.GetSchemeYear(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<string>())).ReturnsAsync(_schemeYear);

            var handler = new GetInstallationRequestsQueryHandler(_logger.Object, _mockSchemeYearService.Object, installationRepository.Object, referenceRepository.Object);

            var actionResult = await handler.Handle(new Common.Queries.GetInstallationRequestsQuery(It.IsAny<Guid>()), CancellationToken.None);

            Assert.NotNull(actionResult);
            Assert.IsType<ActionResult<List<InstallationRequestSummaryDto>>>(actionResult);
            Assert.Equal(1, actionResult.Value.Count());
            Assert.True(actionResult.Value.First().TechnologyTypes.Contains("Desc 1"));
            Assert.True(actionResult.Value.First().TechnologyTypes.Contains("Desc 2"));
            Assert.True(actionResult.Value.First().IsNewBuilds.Contains("Desc 1"));
            Assert.True(actionResult.Value.First().IsNewBuilds.Contains("Desc 2"));
        }
    }
}
